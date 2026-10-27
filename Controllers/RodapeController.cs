using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/rodape")]
public class RodapeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RodapeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<FooterDataDto>> Obter()
    {
        var config = await ObterOuCriarConfiguracaoAsync(_context);

        var links = await _context.FooterLinks.AsNoTracking()
            .Where(f => f.Ativo)
            .OrderBy(f => f.Ordem).ThenBy(f => f.Id)
            .ToListAsync();

        var redes = await _context.FooterRedesSociais.AsNoTracking()
            .Where(f => f.Ativo)
            .OrderBy(f => f.Ordem).ThenBy(f => f.Id)
            .ToListAsync();

        var projetos = await _context.FooterProjetosEspeciais.AsNoTracking()
            .Where(f => f.Ativo)
            .OrderBy(f => f.Ordem).ThenBy(f => f.Id)
            .ToListAsync();

        // A ordem das colunas segue a primeira aparição de cada uma na lista já ordenada por Ordem.
        var colunas = links
            .GroupBy(l => l.Coluna)
            .Select(g => new FooterLinkColumnDto
            {
                Title = g.Key,
                Links = g.Select(l => new FooterLinkItemDto { Label = l.Label, Href = l.Href }).ToList(),
            })
            .ToList();

        return Ok(new FooterDataDto
        {
            Tagline = config.Tagline,
            Newsletter = new FooterNewsletterDto
            {
                Title = config.NewsletterTitulo,
                Description = config.NewsletterDescricao,
                Placeholder = config.NewsletterPlaceholder,
                CtaLabel = config.NewsletterCta,
            },
            Columns = colunas,
            Social = redes.Select(r => new SocialLinkDto { Icon = r.Icone, Href = r.Href, Label = r.Label }).ToList(),
            SpecialProjects = projetos
                .Select(p => new SpecialProjectDto { Icon = p.Icone, Title = p.Titulo, Description = p.Descricao, Href = p.Href })
                .ToList(),
        });
    }

    internal static async Task<ConfiguracaoRodape> ObterOuCriarConfiguracaoAsync(ApplicationDbContext context)
    {
        var config = await context.ConfiguracoesRodape.FirstOrDefaultAsync();
        if (config is null)
        {
            config = new ConfiguracaoRodape();
            context.ConfiguracoesRodape.Add(config);
            await context.SaveChangesAsync();
        }

        return config;
    }
}
