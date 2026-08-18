using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ComentariosController> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public ComentariosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<ComentariosController> logger,
        IEmailService emailService,
        IConfiguration config)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
        _config = config;
    }

    // Remove qualquer marcação HTML do texto enviado — comentários são texto puro,
    // então não há motivo legítimo para conter tags (mitiga stored XSS na origem).
    private static string RemoverHtml(string texto) => Regex.Replace(texto, "<.*?>", string.Empty).Trim();

    [HttpGet("artigos/{artigoId:int}/comentarios")]
    public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> ListarPorArtigo(int artigoId)
    {
        // Autenticação é opcional aqui (endpoint público) — só usamos o id do
        // usuário, se houver, pra marcar quais comentários ele já curtiu.
        var usuarioAtualId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var comentarios = await _context.Comentarios
            .AsNoTracking()
            .Where(c => c.ArtigoId == artigoId && c.Aprovado)
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => new ComentarioResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Texto = c.Texto,
                CriadoEm = c.CriadoEm,
                Curtidas = _context.ComentarioLikes.Count(l => l.ComentarioId == c.Id),
                CurtidoPeloUsuarioAtual = usuarioAtualId != null &&
                    _context.ComentarioLikes.Any(l => l.ComentarioId == c.Id && l.UsuarioId == usuarioAtualId),
            })
            .ToListAsync();

        return Ok(comentarios);
    }

    // Comentar exige login (conta de Leitor ou de equipe editorial) — nome e
    // e-mail vêm da conta autenticada, não de campos de texto livre.
    [HttpPost("artigos/{artigoId:int}/comentarios")]
    [Authorize]
    [EnableRateLimiting("escrita-publica")]
    public async Task<IActionResult> Criar(int artigoId, ComentarioCreateDto dto)
    {
        var artigo = await _context.Artigos.FirstOrDefaultAsync(a => a.Id == artigoId && a.Publicada);
        if (artigo is null)
        {
            return NotFound(new { mensagem = "Artigo não encontrado." });
        }

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null || !usuario.Ativo)
        {
            return Unauthorized();
        }

        var comentario = new Comentario
        {
            ArtigoId = artigoId,
            UsuarioId = usuario.Id,
            Nome = usuario.NomeCompleto,
            Email = usuario.Email ?? string.Empty,
            Texto = RemoverHtml(dto.Texto),
            IpOrigem = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Aprovado = false,
        };

        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();

        await NotificarEditorialAsync(comentario, artigo.Titulo);

        return Accepted(new { mensagem = "Comentário enviado para moderação." });
    }

    // Curtir/descurtir alterna no mesmo endpoint: 1 clique curte, o próximo desfaz.
    [HttpPost("comentarios/{id:int}/curtir")]
    [Authorize]
    [EnableRateLimiting("escrita-publica")]
    public async Task<IActionResult> Curtir(int id)
    {
        var comentarioExiste = await _context.Comentarios.AnyAsync(c => c.Id == id && c.Aprovado);
        if (!comentarioExiste)
        {
            return NotFound();
        }

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existente = await _context.ComentarioLikes
            .FirstOrDefaultAsync(l => l.ComentarioId == id && l.UsuarioId == usuarioId);

        bool curtido;
        if (existente is not null)
        {
            _context.ComentarioLikes.Remove(existente);
            curtido = false;
        }
        else
        {
            _context.ComentarioLikes.Add(new ComentarioLike { ComentarioId = id, UsuarioId = usuarioId });
            curtido = true;
        }

        await _context.SaveChangesAsync();

        var total = await _context.ComentarioLikes.CountAsync(l => l.ComentarioId == id);
        return Ok(new { curtido, totalCurtidas = total });
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
