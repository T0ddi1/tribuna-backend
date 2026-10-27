using Microsoft.AspNetCore.Mvc;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/footer-projetos-especiais")]
public class FooterProjetosEspeciaisAdminController : AdminCrudControllerBase<FooterProjetoEspecial, FooterProjetoEspecialSalvarDto, FooterProjetoEspecialAdminDto>
{
    public FooterProjetosEspeciaisAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<FooterProjetoEspecial> Consulta() =>
        Context.FooterProjetosEspeciais.OrderBy(f => f.Ordem).ThenBy(f => f.Id);

    protected override FooterProjetoEspecialAdminDto Mapear(FooterProjetoEspecial f) => new()
    {
        Id = f.Id,
        Icone = f.Icone,
        Titulo = f.Titulo,
        Descricao = f.Descricao,
        Href = f.Href,
        Ordem = f.Ordem,
        Ativo = f.Ativo,
    };

    protected override void Aplicar(FooterProjetoEspecial f, FooterProjetoEspecialSalvarDto dto)
    {
        f.Icone = dto.Icone.Trim();
        f.Titulo = dto.Titulo.Trim();
        f.Descricao = dto.Descricao.Trim();
        f.Href = dto.Href.Trim();
        f.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) f.Ativo = dto.Ativo.Value;
    }
}
