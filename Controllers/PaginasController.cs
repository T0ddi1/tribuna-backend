using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;

namespace NewsPortal.Api.Controllers;

// Páginas institucionais (Quem somos, Termos de uso...) editáveis pelo admin.
[ApiController]
[Route("api/paginas")]
public class PaginasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaginasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PaginaInstitucionalPublicaDto>> ObterPorSlug(string slug)
    {
        var pagina = await _context.PaginasInstitucionais
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Ativo);

        if (pagina is null)
        {
            return NotFound();
        }

        return Ok(new PaginaInstitucionalPublicaDto
        {
            Title = pagina.Titulo,
            Description = string.IsNullOrWhiteSpace(pagina.ResumoBusca) ? null : pagina.ResumoBusca,
            ContentHtml = pagina.ConteudoHtml,
            Image = pagina.ImagemUrl,
            AttachmentUrl = pagina.AnexoUrl,
            AttachmentLabel = pagina.AnexoLabel,
        });
    }
}
