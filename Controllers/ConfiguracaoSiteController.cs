using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/configuracao-site")]
public class ConfiguracaoSiteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConfiguracaoSiteController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ConfiguracaoSitePublicaDto>> Obter()
    {
        var config = await _context.ConfiguracoesSite.AsNoTracking().FirstOrDefaultAsync();

        return Ok(new ConfiguracaoSitePublicaDto
        {
            LogoUrl = config?.LogoUrl,
            FaviconUrl = config?.FaviconUrl,
            CoresJson = config?.CoresJson,
        });
    }
}
