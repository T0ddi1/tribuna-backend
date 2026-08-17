using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    private static CategoriaResponseDto ParaDto(Categoria c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Slug = c.Slug,
        Tagline = c.Tagline,
        Descricao = c.Descricao,
        Icone = c.Icone,
        CorAccent = c.CorAccent,
        CorAccentDark = c.CorAccentDark,
        CorAccentTint = c.CorAccentTint,
        TemaEscuro = c.TemaEscuro,
        Ordem = c.Ordem,
        QuantidadeArtigos = c.Artigos.Count(a => a.Publicada),
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> Listar()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .Include(c => c.Artigos)
            .OrderBy(c => c.Ordem)
            .ToListAsync();

        return Ok(categorias.Select(ParaDto));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CategoriaResponseDto>> ObterPorSlug(string slug)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .Include(c => c.Artigos)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (categoria is null)
        {
            return NotFound();
        }

        return Ok(ParaDto(categoria));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<CategoriaResponseDto>> Criar(CategoriaCreateDto dto)
    {
        var slugEmUso = await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug);
        if (slugEmUso)
        {
            return Conflict(new { mensagem = "Já existe uma categoria com este slug." });
        }

        var categoria = new Categoria
        {
            Nome = dto.Nome,
            Slug = dto.Slug,
            Tagline = dto.Tagline,
            Descricao = dto.Descricao,
            Icone = dto.Icone,
            CorAccent = dto.CorAccent,
            CorAccentDark = dto.CorAccentDark,
            CorAccentTint = dto.CorAccentTint,
            TemaEscuro = dto.TemaEscuro,
            Ordem = dto.Ordem,
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorSlug), new { slug = categoria.Slug }, ParaDto(categoria));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Atualizar(int id, CategoriaCreateDto dto)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound();
        }

        var slugEmUso = await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug && c.Id != id);
        if (slugEmUso)
        {
            return Conflict(new { mensagem = "Já existe uma categoria com este slug." });
        }

        categoria.Nome = dto.Nome;
        categoria.Slug = dto.Slug;
        categoria.Tagline = dto.Tagline;
        categoria.Descricao = dto.Descricao;
        categoria.Icone = dto.Icone;
        categoria.CorAccent = dto.CorAccent;
        categoria.CorAccentDark = dto.CorAccentDark;
        categoria.CorAccentTint = dto.CorAccentTint;
        categoria.TemaEscuro = dto.TemaEscuro;
        categoria.Ordem = dto.Ordem;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Deletar(int id)
    {
        var categoria = await _context.Categorias.Include(c => c.Artigos).FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound();
        }

        if (categoria.Artigos.Count > 0)
        {
            return Conflict(new { mensagem = "Não é possível excluir uma categoria com artigos vinculados." });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
