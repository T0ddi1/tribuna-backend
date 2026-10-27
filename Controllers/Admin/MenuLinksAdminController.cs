using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/menu-links")]
public class MenuLinksAdminController : AdminCrudControllerBase<MenuLink, MenuLinkSalvarDto, MenuLinkAdminDto>
{
    public MenuLinksAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<MenuLink> Consulta() =>
        Context.MenuLinks.Include(l => l.MenuItem).OrderBy(l => l.Ordem).ThenBy(l => l.Id);

    protected override MenuLinkAdminDto Mapear(MenuLink l) => new()
    {
        Id = l.Id,
        MenuItemId = l.MenuItemId,
        MenuItemLabel = l.MenuItem?.Label,
        ColunaTitulo = l.ColunaTitulo,
        Label = l.Label,
        Href = l.Href,
        Ordem = l.Ordem,
        Ativo = l.Ativo,
    };

    protected override void Aplicar(MenuLink l, MenuLinkSalvarDto dto)
    {
        l.MenuItemId = dto.MenuItemId;
        l.ColunaTitulo = string.IsNullOrWhiteSpace(dto.ColunaTitulo) ? null : dto.ColunaTitulo.Trim();
        l.Label = dto.Label.Trim();
        l.Href = dto.Href.Trim();
        l.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) l.Ativo = dto.Ativo.Value;
    }

    protected override async Task<string?> ValidarAsync(MenuLinkSalvarDto dto) =>
        await Context.MenuItens.AnyAsync(m => m.Id == dto.MenuItemId) ? null : "Item de menu informado não existe.";
}
