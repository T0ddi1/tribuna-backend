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

        // Nome sempre gerado pelo servidor — nunca usar o nome original do
        // cliente (evita path traversal e colisão/sobrescrita de arquivos).
        var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
        var pastaUploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(pastaUploads);
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
    // largura à toa). Se já está dentro do limite, salva os bytes originais
    // sem recomprimir — testado com um JPEG real já otimizado por outra
    // ferramenta: recodificar sem necessidade deixou o arquivo MAIOR (81KB
    // contra 69KB), não menor, então só vale a pena mexer quando redimensiona
    // de verdade. GIF usa MagickImageCollection — trata cada quadro da
    // animação separadamente, então o redimensionamento não quebra a animação.
    private static void RedimensionarEEnviar(Stream origem, string extensao, string caminhoDestino)
    {
        if (extensao == ".gif")
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

        using var imagem = new MagickImage(origem);
        if (imagem.Width <= LarguraMaximaPx)
        {
            SalvarBytesOriginais(origem, caminhoDestino);
            return;
        }

        imagem.Resize((uint)LarguraMaximaPx, 0);

        switch (extensao)
        {
            case ".jpg":
            case ".jpeg":
                imagem.Quality = 82;
                imagem.Write(caminhoDestino, MagickFormat.Jpeg);
                break;
            case ".webp":
                imagem.Quality = 82;
                imagem.Write(caminhoDestino, MagickFormat.WebP);
                break;
            case ".png":
                imagem.Write(caminhoDestino, MagickFormat.Png);
                break;
            default:
                imagem.Write(caminhoDestino);
                break;
        }
    }

    private static void SalvarBytesOriginais(Stream origem, string caminhoDestino)
    {
        origem.Position = 0;
        using var destino = new FileStream(caminhoDestino, FileMode.Create);
        origem.CopyTo(destino);
    }
}
