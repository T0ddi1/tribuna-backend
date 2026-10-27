using Microsoft.AspNetCore.Mvc;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/footer-links")]
public class FooterLinksAdminController : AdminCrudControllerBase<FooterLink, FooterLinkSalvarDto, FooterLinkAdminDto>
{
    public FooterLinksAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<FooterLink> Consulta() =>
        Context.FooterLinks.OrderBy(f => f.Coluna).ThenBy(f => f.Ordem).ThenBy(f => f.Id);

    protected override FooterLinkAdminDto Mapear(FooterLink f) => new()
    {
        Id = f.Id,
        Coluna = f.Coluna,
        Label = f.Label,
        Href = f.Href,
        Ordem = f.Ordem,
        Ativo = f.Ativo,
    };

    protected override void Aplicar(FooterLink f, FooterLinkSalvarDto dto)
    {
        f.Coluna = dto.Coluna.Trim();
        f.Label = dto.Label.Trim();
        f.Href = dto.Href.Trim();
        f.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) f.Ativo = dto.Ativo.Value;
    }
}
