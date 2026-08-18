using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;
using NewsPortal.Api.Services;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api")]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComentariosController> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public ComentariosController(ApplicationDbContext context, ILogger<ComentariosController> logger, IEmailService emailService, IConfiguration config)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
        _config = config;
    }

    // Remove qualquer marcação HTML do texto enviado — comentários são texto puro,
    // então não há motivo legítimo para conter tags (mitiga stored XSS na origem).
    private static string RemoverHtml(string texto) => Regex.Replace(texto, "<.*?>", string.Empty).Trim();

    private static ComentarioResponseDto ParaDto(Comentario c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Texto = c.Texto,
        CriadoEm = c.CriadoEm,
    };

    [HttpGet("artigos/{artigoId:int}/comentarios")]
    public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> ListarPorArtigo(int artigoId)
    {
        var comentarios = await _context.Comentarios
            .AsNoTracking()
            .Where(c => c.ArtigoId == artigoId && c.Aprovado)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();

        return Ok(comentarios.Select(ParaDto));
    }

    [HttpPost("artigos/{artigoId:int}/comentarios")]
    [EnableRateLimiting("escrita-publica")]
    public async Task<IActionResult> Criar(int artigoId, ComentarioCreateDto dto)
    {
        // Honeypot: bots preenchem todos os campos do formulário, incluindo o
        // campo invisível "website". Se veio preenchido, descartamos silenciosamente.
        if (!string.IsNullOrWhiteSpace(dto.Website))
        {
            _logger.LogInformation("Comentário descartado por honeypot preenchido.");
            return Accepted();
        }

        var artigo = await _context.Artigos.FirstOrDefaultAsync(a => a.Id == artigoId && a.Publicada);
        if (artigo is null)
        {
            return NotFound(new { mensagem = "Artigo não encontrado." });
        }

        var comentario = new Comentario
        {
            ArtigoId = artigoId,
            Nome = RemoverHtml(dto.Nome),
            Email = dto.Email.Trim(),
            Texto = RemoverHtml(dto.Texto),
            IpOrigem = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Aprovado = false,
        };

        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();

        await NotificarEditorialAsync(comentario, artigo.Titulo);

        return Accepted(new { mensagem = "Comentário enviado para moderação." });
    }

    [HttpGet("comentarios/pendentes")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<IEnumerable<ComentarioModeracaoDto>>> ListarPendentes()
    {
        var comentarios = await _context.Comentarios
            .AsNoTracking()
            .Include(c => c.Artigo)
            .Where(c => !c.Aprovado)
            .OrderBy(c => c.CriadoEm)
            .ToListAsync();

        return Ok(comentarios.Select(c => new ComentarioModeracaoDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Email = c.Email,
            Texto = c.Texto,
            CriadoEm = c.CriadoEm,
            Aprovado = c.Aprovado,
            ArtigoId = c.ArtigoId,
            ArtigoTitulo = c.Artigo?.Titulo ?? string.Empty,
        }));
    }

    [HttpPost("comentarios/{id:int}/aprovar")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Aprovar(int id)
    {
        var comentario = await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        if (comentario is null)
        {
            return NotFound();
        }

        comentario.Aprovado = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("comentarios/{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Deletar(int id)
    {
        var comentario = await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        if (comentario is null)
        {
            return NotFound();
        }

        _context.Comentarios.Remove(comentario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Best-effort: falha no envio de e-mail não deve derrubar a requisição do comentarista.
    private async Task NotificarEditorialAsync(Comentario comentario, string artigoTitulo)
    {
        var emailEditorial = _config["Notificacoes:EmailEditorial"];
        if (string.IsNullOrWhiteSpace(emailEditorial))
        {
            return;
        }

        try
        {
            await _emailService.EnviarAsync(
                emailEditorial,
                $"Novo comentário aguardando moderação — {artigoTitulo}",
                $"<p><strong>{comentario.Nome}</strong> ({comentario.Email}) comentou em \"{artigoTitulo}\":</p>" +
                $"<blockquote>{comentario.Texto}</blockquote>" +
                "<p>Acesse o painel admin para aprovar ou remover.</p>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao notificar equipe editorial sobre novo comentário {Id}.", comentario.Id);
        }
    }
}
