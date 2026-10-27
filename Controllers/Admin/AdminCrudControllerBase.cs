using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;

namespace NewsPortal.Api.Controllers.Admin;

/// <summary>
/// CRUD padrão do painel (listar, criar, alterar, excluir) para os recursos de
/// conteúdo do site — menu, rodapé, avisos. Cada controller concreto só informa
/// como ordenar, mapear e aplicar o DTO na entidade. O frontend fala com todos
/// pelo mesmo contrato (<c>/api/admin/{recurso}</c>), via AdminApiService.
/// </summary>
[ApiController]
[Authorize(Roles = "Admin,Editor")]
public abstract class AdminCrudControllerBase<TEntidade, TSalvarDto, TAdminDto> : ControllerBase
    where TEntidade : class, new()
{
    protected readonly ApplicationDbContext Context;

    protected AdminCrudControllerBase(ApplicationDbContext context)
    {
        Context = context;
    }

    protected abstract IQueryable<TEntidade> Consulta();
    protected abstract TAdminDto Mapear(TEntidade entidade);
    protected abstract void Aplicar(TEntidade entidade, TSalvarDto dto);

    /// <summary>Retorna uma mensagem de erro se o DTO referenciar algo que não existe; null se estiver ok.</summary>
    protected virtual Task<string?> ValidarAsync(TSalvarDto dto) => Task.FromResult<string?>(null);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TAdminDto>>> Listar()
    {
        var itens = await Consulta().AsNoTracking().ToListAsync();
        return Ok(itens.Select(Mapear));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TAdminDto>> Obter(int id)
    {
        var entidade = await Consulta().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        return entidade is null ? NotFound() : Ok(Mapear(entidade));
    }

    [HttpPost]
    public async Task<ActionResult<TAdminDto>> Criar(TSalvarDto dto)
    {
        var erro = await ValidarAsync(dto);
        if (erro is not null)
        {
            return BadRequest(new { mensagem = erro });
        }

        var entidade = new TEntidade();
        Aplicar(entidade, dto);
        Context.Set<TEntidade>().Add(entidade);
        await Context.SaveChangesAsync();

        var id = (int)Context.Entry(entidade).Property("Id").CurrentValue!;
        return Ok(Mapear(await Consulta().AsNoTracking().FirstAsync(e => EF.Property<int>(e, "Id") == id)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TAdminDto>> Alterar(int id, TSalvarDto dto)
    {
        var entidade = await Context.Set<TEntidade>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        if (entidade is null)
        {
            return NotFound();
        }

        var erro = await ValidarAsync(dto);
        if (erro is not null)
        {
            return BadRequest(new { mensagem = erro });
        }

        Aplicar(entidade, dto);
        await Context.SaveChangesAsync();

        return Ok(Mapear(await Consulta().AsNoTracking().FirstAsync(e => EF.Property<int>(e, "Id") == id)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var entidade = await Context.Set<TEntidade>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        if (entidade is null)
        {
            return NotFound();
        }

        Context.Set<TEntidade>().Remove(entidade);
        await Context.SaveChangesAsync();
        return NoContent();
    }
}
