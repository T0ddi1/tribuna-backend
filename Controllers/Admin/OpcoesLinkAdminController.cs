using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;

namespace NewsPortal.Api.Controllers.Admin;

public record ArtigoOpcaoDto(int Id, string Titulo, string Slug, string? CategoriaSlug);

public record CategoriaOpcaoDto(int Id, string Nome, string Slug, bool Ativo);

/// <summary>
/// Listas enxutas que o painel usa no seletor "escolha um artigo, página ou tópico"
/// ao cadastrar links (menu, rodapé, avisos) — assim o editor não digita caminho na mão.
/// </summary>
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public class OpcoesLinkAdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OpcoesLinkAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/artigos")]
    public async Task<ActionResult<IEnumerable<ArtigoOpcaoDto>>> Artigos()
    {
        var artigos = await _context.Artigos
            .AsNoTracking()
            .Where(a => a.Publicada)
            .OrderByDescending(a => a.PublicadoEm ?? a.CriadoEm)
            .Take(500)
            .Select(a => new ArtigoOpcaoDto(a.Id, a.Titulo, a.Slug, a.Categoria != null ? a.Categoria.Slug : null))
            .ToListAsync();

        return Ok(artigos);
    }

    [HttpGet("api/admin/categorias")]
    public async Task<ActionResult<IEnumerable<CategoriaOpcaoDto>>> Categorias()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaOpcaoDto(c.Id, c.Nome, c.Slug, true))
            .ToListAsync();

        return Ok(categorias);
    }
}
