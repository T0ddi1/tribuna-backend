using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerticaisController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VerticaisController(ApplicationDbContext context)
    {
        _context = context;
    }

    private static VerticalResponseDto ParaDto(Vertical v) => new()
    {
        Id = v.Id,
        Nome = v.Nome,
        Slug = v.Slug,
        Tagline = v.Tagline,
        Descricao = v.Descricao,
        Icone = v.Icone,
        CorAccent = v.CorAccent,
        CorAccentDark = v.CorAccentDark,
        CorAccentTint = v.CorAccentTint,
        TemaEscuro = v.TemaEscuro,
        Ordem = v.Ordem,
        QuantidadeArtigos = v.Artigos.Count(a => a.Publicada),
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VerticalResponseDto>>> Listar()
    {
        var verticais = await _context.Verticais
            .AsNoTracking()
            .Include(v => v.Artigos)
            .OrderBy(v => v.Ordem)
            .ToListAsync();

        return Ok(verticais.Select(ParaDto));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<VerticalResponseDto>> ObterPorSlug(string slug)
    {
        var vertical = await _context.Verticais
            .AsNoTracking()
            .Include(v => v.Artigos)
            .FirstOrDefaultAsync(v => v.Slug == slug);

        if (vertical is null)
        {
            return NotFound();
        }

        return Ok(ParaDto(vertical));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<VerticalResponseDto>> Criar(VerticalCreateDto dto)
    {
        var slugEmUso = await _context.Verticais.AnyAsync(v => v.Slug == dto.Slug);
        if (slugEmUso)
        {
            return Conflict(new { mensagem = "Já existe uma vertical com este slug." });
        }

        var vertical = new Vertical
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

        _context.Verticais.Add(vertical);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorSlug), new { slug = vertical.Slug }, ParaDto(vertical));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Atualizar(int id, VerticalCreateDto dto)
    {
        var vertical = await _context.Verticais.FirstOrDefaultAsync(v => v.Id == id);
        if (vertical is null)
        {
            return NotFound();
        }

        var slugEmUso = await _context.Verticais.AnyAsync(v => v.Slug == dto.Slug && v.Id != id);
        if (slugEmUso)
        {
            return Conflict(new { mensagem = "Já existe uma vertical com este slug." });
        }

        vertical.Nome = dto.Nome;
        vertical.Slug = dto.Slug;
        vertical.Tagline = dto.Tagline;
        vertical.Descricao = dto.Descricao;
        vertical.Icone = dto.Icone;
        vertical.CorAccent = dto.CorAccent;
        vertical.CorAccentDark = dto.CorAccentDark;
        vertical.CorAccentTint = dto.CorAccentTint;
        vertical.TemaEscuro = dto.TemaEscuro;
        vertical.Ordem = dto.Ordem;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Deletar(int id)
    {
        var vertical = await _context.Verticais.Include(v => v.Artigos).FirstOrDefaultAsync(v => v.Id == id);
        if (vertical is null)
        {
            return NotFound();
        }

        if (vertical.Artigos.Count > 0)
        {
            return Conflict(new { mensagem = "Não é possível excluir uma vertical com artigos vinculados." });
        }

        _context.Verticais.Remove(vertical);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
