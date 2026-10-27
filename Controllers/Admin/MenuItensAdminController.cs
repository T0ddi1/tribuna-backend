using Microsoft.AspNetCore.Mvc;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/menu-itens")]
public class MenuItensAdminController : AdminCrudControllerBase<MenuItem, MenuItemSalvarDto, MenuItemAdminDto>
{
    public MenuItensAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<MenuItem> Consulta() => Context.MenuItens.OrderBy(m => m.Ordem).ThenBy(m => m.Id);

    protected override MenuItemAdminDto Mapear(MenuItem m) => new()
    {
        Id = m.Id,
        Label = m.Label,
        Href = m.Href,
        Ordem = m.Ordem,
        Ativo = m.Ativo,
    };

    protected override void Aplicar(MenuItem m, MenuItemSalvarDto dto)
    {
        m.Label = dto.Label.Trim();
        m.Href = dto.Href.Trim();
        m.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) m.Ativo = dto.Ativo.Value;
    }
}
