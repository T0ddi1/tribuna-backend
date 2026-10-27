using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin,Editor")]
[Route("api/admin/configuracao-rodape")]
public class ConfiguracaoRodapeAdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConfiguracaoRodapeAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ConfiguracaoRodapeAdminDto>> Obter()
    {
        return Ok(ParaDto(await ObterOuCriarAsync()));
    }

    [HttpPut]
    public async Task<ActionResult<ConfiguracaoRodapeAdminDto>> Alterar(ConfiguracaoRodapeSalvarDto dto)
    {
        var config = await ObterOuCriarAsync();
        config.Tagline = dto.Tagline.Trim();
        config.NewsletterTitulo = dto.NewsletterTitulo.Trim();
        config.NewsletterDescricao = dto.NewsletterDescricao.Trim();
        config.NewsletterPlaceholder = dto.NewsletterPlaceholder.Trim();
        config.NewsletterCta = dto.NewsletterCta.Trim();
        await _context.SaveChangesAsync();

        return Ok(ParaDto(config));
    }

    private async Task<ConfiguracaoRodape> ObterOuCriarAsync() => await RodapeController.ObterOuCriarConfiguracaoAsync(_context);

    private static ConfiguracaoRodapeAdminDto ParaDto(ConfiguracaoRodape c) => new()
    {
        Id = c.Id,
        Tagline = c.Tagline,
        NewsletterTitulo = c.NewsletterTitulo,
        NewsletterDescricao = c.NewsletterDescricao,
        NewsletterPlaceholder = c.NewsletterPlaceholder,
        NewsletterCta = c.NewsletterCta,
    };
}
