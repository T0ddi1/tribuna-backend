using Microsoft.AspNetCore.Mvc;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers.Admin;

[Route("api/admin/avisos-site")]
public class AvisosSiteAdminController : AdminCrudControllerBase<AvisoSite, AvisoSiteSalvarDto, AvisoSiteAdminDto>
{
    public AvisosSiteAdminController(ApplicationDbContext context) : base(context) { }

    protected override IQueryable<AvisoSite> Consulta() => Context.AvisosSite.OrderBy(a => a.Ordem).ThenBy(a => a.Id);

    protected override AvisoSiteAdminDto Mapear(AvisoSite a) => new()
    {
        Id = a.Id,
        Titulo = a.Titulo,
        Mensagem = a.Mensagem,
        ImagemUrl = a.ImagemUrl,
        SomenteImagem = a.SomenteImagem,
        TextoBotao = a.TextoBotao,
        Link = a.Link,
        Externo = a.Externo,
        PermiteNaoVerMais = a.PermiteNaoVerMais,
        // SQLite devolve DateTime sem Kind; marcamos como UTC pro frontend não deslocar o horário.
        InicioEm = a.InicioEm.HasValue ? DateTime.SpecifyKind(a.InicioEm.Value, DateTimeKind.Utc) : null,
        FimEm = a.FimEm.HasValue ? DateTime.SpecifyKind(a.FimEm.Value, DateTimeKind.Utc) : null,
        Ordem = a.Ordem,
        Ativo = a.Ativo,
    };

    protected override void Aplicar(AvisoSite a, AvisoSiteSalvarDto dto)
    {
        var novo = a.Id == 0;
        a.Titulo = dto.Titulo.Trim();
        a.Mensagem = string.IsNullOrWhiteSpace(dto.Mensagem) ? null : dto.Mensagem.Trim();
        a.ImagemUrl = string.IsNullOrWhiteSpace(dto.ImagemUrl) ? null : dto.ImagemUrl.Trim();
        a.SomenteImagem = dto.SomenteImagem;
        a.TextoBotao = string.IsNullOrWhiteSpace(dto.TextoBotao) ? null : dto.TextoBotao.Trim();
        a.Link = string.IsNullOrWhiteSpace(dto.Link) ? null : dto.Link.Trim();
        a.Externo = dto.Externo;
        a.PermiteNaoVerMais = dto.PermiteNaoVerMais;
        a.InicioEm = ComoUtc(dto.InicioEm);
        a.FimEm = ComoUtc(dto.FimEm);
        a.Ordem = dto.Ordem;
        if (dto.Ativo.HasValue) a.Ativo = dto.Ativo.Value;
        if (!novo) a.AtualizadoEm = DateTime.UtcNow;
    }

    private static DateTime? ComoUtc(DateTime? data) =>
        data.HasValue
            ? (data.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(data.Value, DateTimeKind.Utc) : data.Value.ToUniversalTime())
            : null;
}
