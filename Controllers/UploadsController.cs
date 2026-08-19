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
        await using (var destino = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await stream.CopyToAsync(destino);
        }

        return Ok(new { url = $"/uploads/{nomeArquivo}" });
    }
}
