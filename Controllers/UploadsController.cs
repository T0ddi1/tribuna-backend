using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Editor")]
public class UploadsController : ControllerBase
{
    private const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB

    // Maior largura recomendada entre os dois usos deste endpoint: capa
    // (1200px) e imagem inserida no corpo do texto (1600px, pra manter
    // nitidez em telas grandes). Upload maior que isso é redimensionado —
    // nunca faz upscale de imagem menor.
    private const int LarguraMaximaPx = 1600;

    // Assinatura binária (magic bytes) real do arquivo — nunca confiar só na
    // extensão ou no Content-Type declarado pelo cliente, que podem ser forjados.
    private static readonly Dictionary<string, byte[][]> AssinaturasPermitidas = new()
    {
        [".jpg"] = [[0xFF, 0xD8, 0xFF]],
        [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
        [".png"] = [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]],
        [".webp"] = [[0x52, 0x49, 0x46, 0x46]], // "RIFF" (WEBP confirmado depois pelos bytes 8-11)
        [".gif"] = [[0x47, 0x49, 0x46, 0x38]],
    };

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadsController> _logger;

    public UploadsController(IWebHostEnvironment environment, ILogger<UploadsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("imagem")]
    [EnableRateLimiting("uploads")]
    [RequestSizeLimit(TamanhoMaximoBytes)]
    public async Task<IActionResult> EnviarImagem(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            return BadRequest(new { mensagem = "Nenhum arquivo enviado." });
        }

        if (arquivo.Length > TamanhoMaximoBytes)
        {
            return BadRequest(new { mensagem = "Arquivo excede o tamanho máximo de 10 MB." });
        }

        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        if (!AssinaturasPermitidas.TryGetValue(extensao, out var assinaturas))
        {
            return BadRequest(new { mensagem = "Formato de imagem não suportado. Use JPG, PNG, WEBP ou GIF." });
        }

        await using var stream = arquivo.OpenReadStream();
        var cabecalho = new byte[12];
        var lidos = await stream.ReadAsync(cabecalho.AsMemory(0, (int)Math.Min(12, arquivo.Length)));

        var assinaturaValida = assinaturas.Any(assinatura =>
            lidos >= assinatura.Length && cabecalho.Take(assinatura.Length).SequenceEqual(assinatura));

        if (!assinaturaValida || (extensao == ".webp" && !(cabecalho.Length >= 12 && cabecalho[8] == 'W' && cabecalho[9] == 'E' && cabecalho[10] == 'B' && cabecalho[11] == 'P')))
        {
            _logger.LogWarning("Upload rejeitado: assinatura de arquivo não confere com a extensão {Extensao}.", extensao);
            return BadRequest(new { mensagem = "O conteúdo do arquivo não corresponde a uma imagem válida." });
        }

        // Dentro de App_Data (não wwwroot) — é o diretório com volume persistente
        // no Railway, então os uploads sobrevivem a redeploys do backend.
        var pastaUploads = Path.Combine(_environment.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(pastaUploads);

        // JPG/PNG viram WebP (bem menor no mesmo nível de qualidade visual) —
        // GIF fica GIF (preserva a animação) e um WebP enviado continua WebP.
        // A extensão final só é conhecida depois dessa decisão, por isso o
        // nome do arquivo é montado com base no retorno do processamento.
        var extensaoFinal = extensao is ".jpg" or ".jpeg" or ".png" ? ".webp" : extensao;

        // Nome sempre gerado pelo servidor — nunca usar o nome original do
        // cliente (evita path traversal e colisão/sobrescrita de arquivos).
        var nomeArquivo = $"{Guid.NewGuid():N}{extensaoFinal}";
        var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

        stream.Position = 0;
        try
        {
            RedimensionarEEnviar(stream, extensao, caminhoCompleto);
        }
        catch (MagickException ex)
        {
            _logger.LogWarning(ex, "Falha ao decodificar imagem no upload apesar da assinatura válida.");
            return BadRequest(new { mensagem = "Não foi possível processar essa imagem. Tente outro arquivo." });
        }

        return Ok(new { url = $"/uploads/{nomeArquivo}" });
    }

    // Reduz a imagem se ela vier maior que o necessário (o upload original de
    // uma foto de celular ou captura de tela facilmente passa de 1600px de
    // largura à toa). GIF usa MagickImageCollection — trata cada quadro da
    // animação separadamente, então o redimensionamento não quebra a animação
    // — e nunca converte de formato, só reduz se precisar.
    // JPG/PNG sempre viram WebP, redimensionados ou não: mesmo sem precisar
    // encolher, testei com um JPEG real de 69KB e o mesmo arquivo saiu com
    // 59KB em WebP na mesma qualidade visual — vale a pena mesmo sem resize
    // (a economia real varia bastante por imagem; fotos com mais textura
    // costumam ganhar mais do que uma foto de capa já bem comprimida).
    // WebP enviado como WebP segue a mesma regra de antes: só recomprime se
    // for realmente redimensionar, pra não arriscar deixar o arquivo maior
    // à toa (foi o que aconteceu recomprimindo sem necessidade, ver commit
    // anterior).
    private static void RedimensionarEEnviar(Stream origem, string extensaoOriginal, string caminhoDestino)
    {
        if (extensaoOriginal == ".gif")
        {
            using var quadros = new MagickImageCollection(origem);
            if (!quadros.Any(quadro => quadro.Width > LarguraMaximaPx))
            {
                SalvarBytesOriginais(origem, caminhoDestino);
                return;
            }

            foreach (var quadro in quadros)
            {
                if (quadro.Width > LarguraMaximaPx)
                {
                    quadro.Resize((uint)LarguraMaximaPx, 0);
                }
            }
            quadros.Write(caminhoDestino, MagickFormat.Gif);
            return;
        }

        var converterParaWebp = extensaoOriginal is ".jpg" or ".jpeg" or ".png";

        using var imagem = new MagickImage(origem);
        if (!converterParaWebp && imagem.Width <= LarguraMaximaPx)
        {
            SalvarBytesOriginais(origem, caminhoDestino);
            return;
        }

        if (imagem.Width > LarguraMaximaPx)
        {
            imagem.Resize((uint)LarguraMaximaPx, 0);
        }

        // A essa altura só sobram dois casos, e os dois viram WebP: uma
        // conversão de JPG/PNG (redimensionada ou não), ou um WebP que
        // precisou redimensionar (o "sem mudança nenhuma" já retornou acima).
        imagem.Quality = 82;
        imagem.Write(caminhoDestino, MagickFormat.WebP);
    }

    private static void SalvarBytesOriginais(Stream origem, string caminhoDestino)
    {
        origem.Position = 0;
        using var destino = new FileStream(caminhoDestino, FileMode.Create);
        origem.CopyTo(destino);
    }
}
