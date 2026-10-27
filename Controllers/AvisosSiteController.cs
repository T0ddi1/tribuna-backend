using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/avisos-site")]
public class AvisosSiteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AvisosSiteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Avisos ativos dentro da janela InicioEm–FimEm. Sem cache: o aviso precisa
    // sumir/aparecer na hora que o admin liga ou desliga.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvisoSitePublicoDto>>> ListarAtivos()
    {
        var agora = DateTime.UtcNow;

        var avisos = await _context.AvisosSite
            .AsNoTracking()
            .Where(a => a.Ativo && (a.InicioEm == null || a.InicioEm <= agora) && (a.FimEm == null || a.FimEm > agora))
            .OrderBy(a => a.Ordem).ThenBy(a => a.Id)
            .ToListAsync();

        Response.Headers.CacheControl = "no-store";

        return Ok(avisos.Select(a => new AvisoSitePublicoDto
        {
            Id = a.Id,
            Versao = (a.AtualizadoEm ?? a.CriadoEm).Ticks.ToString(),
            Titulo = a.Titulo,
            Mensagem = a.Mensagem,
            ImagemUrl = a.ImagemUrl,
            SomenteImagem = a.SomenteImagem,
            TextoBotao = a.TextoBotao,
            Link = a.Link,
            Externo = a.Externo,
            PermiteNaoVerMais = a.PermiteNaoVerMais,
        }));
    }
}
