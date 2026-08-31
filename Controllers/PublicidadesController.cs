using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicidadesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PublicidadesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private static PublicidadePublicaDto ParaPublicaDto(Publicidade p) => new()
    {
        Eyebrow = p.Eyebrow,
        Titulo = p.Titulo,
        Descricao = p.Descricao,
        CtaLabel = p.CtaLabel,
        Href = p.Href,
        ImagemUrl = p.ImagemUrl,
        LogoUrl = p.LogoUrl,
        Tema = p.Tema,
        Tamanho = p.Tamanho,
        SiteLabel = p.SiteLabel,
        Externo = p.Externo,
    };

    private static PublicidadeAdminDto ParaAdminDto(Publicidade p) => new()
    {
        Id = p.Id,
        Eyebrow = p.Eyebrow,
        Titulo = p.Titulo,
        Descricao = p.Descricao,
        CtaLabel = p.CtaLabel,
        Href = p.Href,
        ImagemUrl = p.ImagemUrl,
        LogoUrl = p.LogoUrl,
        Tema = p.Tema,
        Tamanho = p.Tamanho,
        SiteLabel = p.SiteLabel,
        Externo = p.Externo,
        Ativo = p.Ativo,
        Ordem = p.Ordem,
        VerticalId = p.VerticalId,
        VerticalNome = p.Vertical?.Nome,
        ExibirEmTodasAsPaginas = p.ExibirEmTodasAsPaginas,
    };

    // Consumido pelo site: "local" é "home" (padrão) ou o slug de uma vertical
    // (ex: "capital"). Só retorna publicidade ativa, na ordem configurada.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublicidadePublicaDto>>> Listar([FromQuery] string? local)
    {
        var query = _context.Publicidades.AsNoTracking().Where(p => p.Ativo);

        if (string.IsNullOrWhiteSpace(local) || local == "home")
        {
            query = query.Where(p => p.VerticalId == null || p.ExibirEmTodasAsPaginas);
        }
        else
        {
            var vertical = await _context.Verticais.AsNoTracking().FirstOrDefaultAsync(v => v.Slug == local);
            if (vertical is null)
            {
                return Ok(Enumerable.Empty<PublicidadePublicaDto>());
            }

            query = query.Where(p => p.VerticalId == vertical.Id || p.ExibirEmTodasAsPaginas);
        }

        var publicidades = await query.OrderBy(p => p.Ordem).ToListAsync();
        return Ok(publicidades.Select(ParaPublicaDto));
    }

    [HttpGet("gerenciar")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<IEnumerable<PublicidadeAdminDto>>> ListarParaAdmin()
    {
        var publicidades = await _context.Publicidades
            .AsNoTracking()
            .Include(p => p.Vertical)
            .OrderBy(p => p.Ordem)
            .ToListAsync();

        return Ok(publicidades.Select(ParaAdminDto));
    }

    [HttpGet("gerenciar/{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<PublicidadeAdminDto>> ObterParaAdmin(int id)
    {
        var publicidade = await _context.Publicidades
            .AsNoTracking()
            .Include(p => p.Vertical)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (publicidade is null)
        {
            return NotFound();
        }

        return Ok(ParaAdminDto(publicidade));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<PublicidadeAdminDto>> Criar(PublicidadeCreateDto dto)
    {
        if (dto.VerticalId is not null && !await _context.Verticais.AnyAsync(v => v.Id == dto.VerticalId))
        {
            return BadRequest(new { mensagem = "Vertical informada não existe." });
        }

        var publicidade = new Publicidade
        {
            Eyebrow = dto.Eyebrow,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            CtaLabel = dto.CtaLabel,
            Href = dto.Href,
            ImagemUrl = dto.ImagemUrl,
            LogoUrl = dto.LogoUrl,
            Tema = dto.Tema,
            Tamanho = dto.Tamanho,
            SiteLabel = dto.SiteLabel,
            Externo = dto.Externo,
            Ativo = dto.Ativo,
            Ordem = dto.Ordem,
            VerticalId = dto.VerticalId,
            ExibirEmTodasAsPaginas = dto.ExibirEmTodasAsPaginas,
        };

        _context.Publicidades.Add(publicidade);
        await _context.SaveChangesAsync();

        await _context.Entry(publicidade).Reference(p => p.Vertical).LoadAsync();
        return CreatedAtAction(nameof(ObterParaAdmin), new { id = publicidade.Id }, ParaAdminDto(publicidade));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Atualizar(int id, PublicidadeUpdateDto dto)
    {
        var publicidade = await _context.Publicidades.FirstOrDefaultAsync(p => p.Id == id);
        if (publicidade is null)
        {
            return NotFound();
        }

        if (dto.VerticalId is not null && !await _context.Verticais.AnyAsync(v => v.Id == dto.VerticalId))
        {
            return BadRequest(new { mensagem = "Vertical informada não existe." });
        }

        publicidade.Eyebrow = dto.Eyebrow;
        publicidade.Titulo = dto.Titulo;
        publicidade.Descricao = dto.Descricao;
        publicidade.CtaLabel = dto.CtaLabel;
        publicidade.Href = dto.Href;
        publicidade.ImagemUrl = dto.ImagemUrl;
        publicidade.LogoUrl = dto.LogoUrl;
        publicidade.Tema = dto.Tema;
        publicidade.Tamanho = dto.Tamanho;
        publicidade.SiteLabel = dto.SiteLabel;
        publicidade.Externo = dto.Externo;
        publicidade.Ativo = dto.Ativo;
        publicidade.Ordem = dto.Ordem;
        publicidade.VerticalId = dto.VerticalId;
        publicidade.ExibirEmTodasAsPaginas = dto.ExibirEmTodasAsPaginas;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Deletar(int id)
    {
        var publicidade = await _context.Publicidades.FirstOrDefaultAsync(p => p.Id == id);
        if (publicidade is null)
        {
            return NotFound();
        }

        _context.Publicidades.Remove(publicidade);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
