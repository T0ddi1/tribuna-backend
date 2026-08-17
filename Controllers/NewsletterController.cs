using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NewsletterController(ApplicationDbContext context)
    {
        _context = context;
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

        return Ok(new { mensagem = "Inscrição confirmada." });
    }
}
