using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin,Editor")]
[Route("api/admin/configuracao-site")]
public class ConfiguracaoSiteAdminController : ControllerBase
{
    private static readonly Regex RegexCor = new("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    private readonly ApplicationDbContext _context;

    public ConfiguracaoSiteAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ConfiguracaoSiteAdminDto>> Obter()
    {
        return Ok(ParaDto(await ObterOuCriarAsync()));
    }

    [HttpPut]
    public async Task<ActionResult<ConfiguracaoSiteAdminDto>> Alterar(ConfiguracaoSiteSalvarDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.CoresJson) && !CoresValidas(dto.CoresJson))
        {
            return BadRequest(new { mensagem = "A paleta de cores é inválida: use um JSON com valores no formato #rrggbb." });
        }

        var config = await ObterOuCriarAsync();
        config.LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim();
        config.FaviconUrl = string.IsNullOrWhiteSpace(dto.FaviconUrl) ? null : dto.FaviconUrl.Trim();
        config.CoresJson = string.IsNullOrWhiteSpace(dto.CoresJson) ? null : dto.CoresJson;
        await _context.SaveChangesAsync();

        return Ok(ParaDto(config));
    }

    // Cores vão parar em CSS no navegador de todo visitante: só aceitamos mapa
    // chave -> #rrggbb, nada de valores livres que poderiam virar injeção de CSS.
    private static bool CoresValidas(string json)
    {
        try
        {
            var cores = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return cores is not null && cores.Values.All(v => RegexCor.IsMatch(v));
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private async Task<ConfiguracaoSite> ObterOuCriarAsync()
    {
        var config = await _context.ConfiguracoesSite.FirstOrDefaultAsync();
        if (config is null)
        {
            config = new ConfiguracaoSite();
            _context.ConfiguracoesSite.Add(config);
            await _context.SaveChangesAsync();
        }

        return config;
    }

    private static ConfiguracaoSiteAdminDto ParaDto(ConfiguracaoSite c) => new()
    {
        Id = c.Id,
        LogoUrl = c.LogoUrl,
        FaviconUrl = c.FaviconUrl,
        CoresJson = c.CoresJson,
    };
}
