using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;
using NewsPortal.Api.Services;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(ApplicationDbContext context, IEmailService emailService, IConfiguration config, ILogger<NewsletterController> logger)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [EnableRateLimiting("escrita-publica")]
    public async Task<IActionResult> Inscrever(NewsletterInscricaoDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var jaInscrito = await _context.NewsletterAssinantes.AnyAsync(n => n.Email == email);
        if (jaInscrito)
        {
            // Idempotente: não revela se o e-mail já estava cadastrado.
            return Ok(new { mensagem = "Inscrição confirmada." });
        }

        _context.NewsletterAssinantes.Add(new NewsletterAssinante { Email = email });
        await _context.SaveChangesAsync();

        await NotificarEditorialAsync(email);

        return Ok(new { mensagem = "Inscrição confirmada." });
    }

    // Best-effort: falha no envio de e-mail não deve derrubar a inscrição do leitor.
    private async Task NotificarEditorialAsync(string emailAssinante)
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
                "Nova inscrição na newsletter — Tribuna",
                $"<p>Novo assinante da newsletter: <strong>{emailAssinante}</strong></p>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao notificar equipe editorial sobre nova inscrição na newsletter.");
        }
    }
}
