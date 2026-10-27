using Microsoft.AspNetCore.Mvc;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/footer-redes-sociais")]
public class FooterRedesSociaisAdminController : AdminCrudControllerBase<FooterRedeSocial, FooterRedeSocialSalvarDto, FooterRedeSocialAdminDto>
{
    public FooterRedesSociaisAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<FooterRedeSocial> Consulta() =>
        Context.FooterRedesSociais.OrderBy(f => f.Ordem).ThenBy(f => f.Id);

    protected override FooterRedeSocialAdminDto Mapear(FooterRedeSocial f) => new()
    {
        Id = f.Id,
        Icone = f.Icone,
        Label = f.Label,
        Href = f.Href,
        Ordem = f.Ordem,
        Ativo = f.Ativo,
    };

    protected override void Aplicar(FooterRedeSocial f, FooterRedeSocialSalvarDto dto)
    {
        f.Icone = dto.Icone.Trim();
        f.Label = dto.Label.Trim();
        f.Href = dto.Href.Trim();
        f.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) f.Ativo = dto.Ativo.Value;
    }
}
