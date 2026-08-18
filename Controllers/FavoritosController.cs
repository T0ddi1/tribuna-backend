using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/favoritos")]
[Authorize]
public class FavoritosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritosController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string UsuarioId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoritoResponseDto>>> Listar()
    {
        var favoritos = await _context.Favoritos
            .AsNoTracking()
            .Include(f => f.Artigo)
            .Where(f => f.UsuarioId == UsuarioId && f.Artigo != null)
            .OrderByDescending(f => f.CriadoEm)
            .Select(f => new FavoritoResponseDto
            {
                Slug = f.Artigo!.Slug,
                Titulo = f.Artigo!.Titulo,
                ImagemCapaUrl = f.Artigo!.ImagemCapaUrl,
                PublicadoEm = f.Artigo!.PublicadoEm,
            })
            .ToListAsync();

        return Ok(favoritos);
    }

    [HttpPost("{slug}")]
    [EnableRateLimiting("escrita-publica")]
    public async Task<IActionResult> Adicionar(string slug)
    {
        var artigo = await _context.Artigos.FirstOrDefaultAsync(a => a.Slug == slug && a.Publicada);
        if (artigo is null)
        {
            return NotFound(new { mensagem = "Artigo não encontrado." });
        }

        var jaExiste = await _context.Favoritos.AnyAsync(f => f.UsuarioId == UsuarioId && f.ArtigoId == artigo.Id);
        if (!jaExiste)
        {
            _context.Favoritos.Add(new Favorito { UsuarioId = UsuarioId, ArtigoId = artigo.Id });
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Remover(string slug)
    {
        var favorito = await _context.Favoritos
            .Include(f => f.Artigo)
            .FirstOrDefaultAsync(f => f.UsuarioId == UsuarioId && f.Artigo != null && f.Artigo.Slug == slug);

        if (favorito is not null)
        {
            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }
}
