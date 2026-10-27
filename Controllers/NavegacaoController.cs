using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/navegacao")]
public class NavegacaoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NavegacaoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Menu do cabeçalho: itens de primeiro nível; os que têm links viram submenu
    // em colunas, agrupadas pelo título da coluna.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NavItemDto>>> Obter()
    {
        var itens = await _context.MenuItens
            .AsNoTracking()
            .Where(m => m.Ativo)
            .Include(m => m.Links.Where(l => l.Ativo))
            .OrderBy(m => m.Ordem).ThenBy(m => m.Id)
            .ToListAsync();

        var resultado = itens.Select(item =>
        {
            var colunas = item.Links
                .OrderBy(l => l.Ordem).ThenBy(l => l.Id)
                .GroupBy(l => l.ColunaTitulo ?? string.Empty)
                .Select(g => new SubmenuColumnDto
                {
                    Title = g.Key,
                    Items = g.Select(l => new SubmenuItemDto { Label = l.Label, Href = l.Href }).ToList(),
                })
                .ToList();

            return new NavItemDto
            {
                Label = item.Label,
                Href = item.Href,
                Columns = colunas.Count > 0 ? colunas : null,
            };
        });

        return Ok(resultado);
    }
}
