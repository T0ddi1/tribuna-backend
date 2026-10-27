using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;
using NewsPortal.Api.Services;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/paginas-institucionais")]
public class PaginasInstitucionaisAdminController : AdminCrudControllerBase<PaginaInstitucional, PaginaInstitucionalSalvarDto, PaginaInstitucionalAdminDto>
{
    private readonly ConteudoSanitizerService _sanitizer;

    public PaginasInstitucionaisAdminController(ApplicationDbContext context, ConteudoSanitizerService sanitizer) : base(context)
    {
        _sanitizer = sanitizer;
    }

    protected override IQueryable<PaginaInstitucional> Consulta() => Context.PaginasInstitucionais.OrderBy(p => p.Titulo);

    protected override PaginaInstitucionalAdminDto Mapear(PaginaInstitucional p) => new()
    {
        Id = p.Id,
        Titulo = p.Titulo,
        Slug = p.Slug,
        ResumoBusca = p.ResumoBusca,
        ConteudoHtml = p.ConteudoHtml,
        ImagemUrl = p.ImagemUrl,
        AnexoUrl = p.AnexoUrl,
        AnexoLabel = p.AnexoLabel,
        Ativo = p.Ativo,
    };

    // O slug nasce do título só na criação e nunca mais muda: as rotas públicas
    // (/quem-somos, /termos-de-uso...) apontam pra ele, e renomear a página
    // não deve derrubar o link.
    protected override void Aplicar(PaginaInstitucional p, PaginaInstitucionalSalvarDto dto)
    {
        p.Titulo = dto.Titulo.Trim();
        p.ResumoBusca = dto.ResumoBusca?.Trim() ?? string.Empty;
        p.ConteudoHtml = _sanitizer.Sanitizar(dto.ConteudoHtml);
        p.ImagemUrl = string.IsNullOrWhiteSpace(dto.ImagemUrl) ? null : dto.ImagemUrl.Trim();
        p.AnexoUrl = string.IsNullOrWhiteSpace(dto.AnexoUrl) ? null : dto.AnexoUrl.Trim();
        p.AnexoLabel = string.IsNullOrWhiteSpace(dto.AnexoLabel) ? null : dto.AnexoLabel.Trim();
        if (dto.Ativo.HasValue) p.Ativo = dto.Ativo.Value;

        if (p.Id == 0)
        {
            p.Slug = GerarSlugUnico(p.Titulo);
        }
    }

    private string GerarSlugUnico(string titulo)
    {
        var slugBase = GerarSlug(titulo);
        if (string.IsNullOrEmpty(slugBase)) slugBase = "pagina";

        var slug = slugBase;
        var sufixo = 1;
        while (Context.PaginasInstitucionais.Any(p => p.Slug == slug))
        {
            slug = $"{slugBase}-{++sufixo}";
        }

        return slug;
    }

    private static string GerarSlug(string titulo)
    {
        var decomposto = titulo.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var semAcento = new StringBuilder();
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) semAcento.Append(c);
        }

        var slug = Regex.Replace(semAcento.ToString(), "[^a-z0-9\\s-]", "");
        return Regex.Replace(slug, "\\s+", "-").Trim('-');
    }
}
