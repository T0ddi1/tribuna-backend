using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;
using NewsPortal.Api.Services;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtigosController : ControllerBase
{
    private const int TamanhoPaginaMaximo = 50;

    private readonly ApplicationDbContext _context;
    private readonly ConteudoSanitizerService _sanitizer;

    public ArtigosController(ApplicationDbContext context, ConteudoSanitizerService sanitizer)
    {
        _context = context;
        _sanitizer = sanitizer;
    }

    private static ArtigoListItemDto ParaListItemDto(Artigo a) => new()
    {
        Id = a.Id,
        Slug = a.Slug,
        Titulo = a.Titulo,
        Resumo = a.Resumo,
        ImagemCapaUrl = a.ImagemCapaUrl,
        Patrocinado = a.Patrocinado,
        Publicada = a.Publicada,
        PublicadoEm = a.PublicadoEm,
        VerticalNome = a.Vertical?.Nome ?? string.Empty,
        VerticalSlug = a.Vertical?.Slug ?? string.Empty,
        CategoriaNome = a.Categoria?.Nome,
        CategoriaSlug = a.Categoria?.Slug,
        AutorNome = !string.IsNullOrWhiteSpace(a.AutorExibicao) ? a.AutorExibicao : a.Autor?.NomeCompleto ?? string.Empty,
    };

    private static ArtigoAdminDto ParaAdminDto(Artigo a) => new()
    {
        Id = a.Id,
        Slug = a.Slug,
        Titulo = a.Titulo,
        Subtitulo = a.Subtitulo,
        Resumo = a.Resumo,
        ConteudoHtml = a.ConteudoHtml,
        ImagemCapaUrl = a.ImagemCapaUrl,
        Patrocinado = a.Patrocinado,
        Destaque = a.Destaque,
        Publicada = a.Publicada,
        CriadoEm = a.CriadoEm,
        AtualizadoEm = a.AtualizadoEm,
        PublicadoEm = a.PublicadoEm,
        Visualizacoes = a.Visualizacoes,
        VerticalId = a.VerticalId,
        VerticalNome = a.Vertical?.Nome ?? string.Empty,
        CategoriaId = a.CategoriaId,
        CategoriaNome = a.Categoria?.Nome,
        AutorId = a.AutorId,
        AutorNome = !string.IsNullOrWhiteSpace(a.AutorExibicao) ? a.AutorExibicao : a.Autor?.NomeCompleto ?? string.Empty,
        AutorExibicao = a.AutorExibicao,
    };

    [HttpGet]
    public async Task<ActionResult<PaginaResultDto<ArtigoListItemDto>>> Listar(
        [FromQuery] string? vertical,
        [FromQuery] string? categoria,
        [FromQuery] string? busca,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        pagina = Math.Max(pagina, 1);
        tamanhoPagina = Math.Clamp(tamanhoPagina, 1, TamanhoPaginaMaximo);

        var query = _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .Where(a => a.Publicada);

        if (!string.IsNullOrWhiteSpace(vertical))
        {
            query = query.Where(a => a.Vertical!.Slug == vertical);
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(a => a.Categoria != null && a.Categoria.Slug == categoria);
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            // Termo limitado e sempre usado como parâmetro (EF Core parametriza a query),
            // evitando SQL injection e prevenindo buscas custosas com input gigante.
            var termo = busca.Trim();
            if (termo.Length > 100)
            {
                termo = termo[..100];
            }

            query = query.Where(a => EF.Functions.Like(a.Titulo, $"%{termo}%") || EF.Functions.Like(a.Resumo, $"%{termo}%"));
        }

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderByDescending(a => a.PublicadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return Ok(new PaginaResultDto<ArtigoListItemDto>
        {
            Itens = itens.Select(ParaListItemDto),
            PaginaAtual = pagina,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina),
            TotalItens = totalItens,
        });
    }

    [HttpGet("destaque")]
    public async Task<ActionResult<IEnumerable<ArtigoListItemDto>>> Destaques(
        [FromQuery] string? vertical,
        [FromQuery] int quantidade = 5)
    {
        quantidade = Math.Clamp(quantidade, 1, 20);

        var query = _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .Where(a => a.Publicada && a.Destaque);

        if (!string.IsNullOrWhiteSpace(vertical))
        {
            query = query.Where(a => a.Vertical!.Slug == vertical);
        }

        var artigos = await query
            .OrderByDescending(a => a.PublicadoEm)
            .Take(quantidade)
            .ToListAsync();

        return Ok(artigos.Select(ParaListItemDto));
    }

    // "Em alta": ranking por visualizações reais (Artigo.Visualizacoes, incrementado
    // a cada leitura em ObterPorSlug) — não é curadoria manual, é o que mais é lido.
    [HttpGet("em-alta")]
    public async Task<ActionResult<IEnumerable<ArtigoListItemDto>>> EmAlta(
        [FromQuery] string? vertical,
        [FromQuery] int quantidade = 4)
    {
        quantidade = Math.Clamp(quantidade, 1, 20);

        var query = _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .Where(a => a.Publicada);

        if (!string.IsNullOrWhiteSpace(vertical))
        {
            query = query.Where(a => a.Vertical!.Slug == vertical);
        }

        var artigos = await query
            .OrderByDescending(a => a.Visualizacoes)
            .ThenByDescending(a => a.PublicadoEm)
            .Take(quantidade)
            .ToListAsync();

        return Ok(artigos.Select(ParaListItemDto));
    }

    // Telas de gestão: lista TODOS os artigos (inclusive rascunhos). Editor só
    // vê os próprios; Admin vê tudo — mesma regra de posse usada em editar/excluir.
    [HttpGet("gerenciar")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<PaginaResultDto<ArtigoAdminDto>>> ListarParaGestao(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20)
    {
        pagina = Math.Max(pagina, 1);
        tamanhoPagina = Math.Clamp(tamanhoPagina, 1, TamanhoPaginaMaximo);

        var query = _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .AsQueryable();

        if (!User.IsInRole(Roles.Admin))
        {
            var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            query = query.Where(a => a.AutorId == autorId);
        }

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderByDescending(a => a.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return Ok(new PaginaResultDto<ArtigoAdminDto>
        {
            Itens = itens.Select(ParaAdminDto),
            PaginaAtual = pagina,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina),
            TotalItens = totalItens,
        });
    }

    [HttpGet("gerenciar/{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<ArtigoAdminDto>> ObterParaGestao(int id)
    {
        var artigo = await _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artigo is null)
        {
            return NotFound();
        }

        var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!User.IsInRole(Roles.Admin) && artigo.AutorId != autorId)
        {
            return Forbid();
        }

        return Ok(ParaAdminDto(artigo));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ArtigoDetailDto>> ObterPorSlug(string slug)
    {
        var artigo = await _context.Artigos
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Slug == slug && a.Publicada);

        if (artigo is null)
        {
            return NotFound();
        }

        artigo.Visualizacoes++;
        await _context.SaveChangesAsync();

        var relacionados = await _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .Where(a => a.Publicada && a.VerticalId == artigo.VerticalId && a.Id != artigo.Id)
            .OrderByDescending(a => a.PublicadoEm)
            .Take(4)
            .ToListAsync();

        var dto = new ArtigoDetailDto
        {
            Id = artigo.Id,
            Slug = artigo.Slug,
            Titulo = artigo.Titulo,
            Subtitulo = artigo.Subtitulo,
            Resumo = artigo.Resumo,
            ConteudoHtml = artigo.ConteudoHtml,
            ImagemCapaUrl = artigo.ImagemCapaUrl,
            Patrocinado = artigo.Patrocinado,
            Publicada = artigo.Publicada,
            PublicadoEm = artigo.PublicadoEm,
            Visualizacoes = artigo.Visualizacoes,
            VerticalNome = artigo.Vertical?.Nome ?? string.Empty,
            VerticalSlug = artigo.Vertical?.Slug ?? string.Empty,
            CategoriaNome = artigo.Categoria?.Nome,
            CategoriaSlug = artigo.Categoria?.Slug,
            AutorNome = !string.IsNullOrWhiteSpace(artigo.AutorExibicao) ? artigo.AutorExibicao : artigo.Autor?.NomeCompleto ?? string.Empty,
            RelacionadosMesmaVertical = relacionados.Select(ParaListItemDto),
        };

        return Ok(dto);
    }

    // Mesmo formato de ObterPorSlug, mas por id (rascunho não tem URL pública
    // ainda), sem o filtro de Publicada e sem contar visualização — é só o
    // admin/editor conferindo como o artigo vai ficar antes de publicar.
    // Editor só pode ver prévia dos próprios artigos, igual à regra de edição.
    [HttpGet("preview/{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<ArtigoDetailDto>> Preview(int id)
    {
        var artigo = await _context.Artigos
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artigo is null)
        {
            return NotFound();
        }

        var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ehAdmin = User.IsInRole(Roles.Admin);
        if (!ehAdmin && artigo.AutorId != autorId)
        {
            return Forbid();
        }

        var relacionados = await _context.Artigos
            .AsNoTracking()
            .Include(a => a.Vertical)
            .Include(a => a.Categoria)
            .Include(a => a.Autor)
            .Where(a => a.Publicada && a.VerticalId == artigo.VerticalId && a.Id != artigo.Id)
            .OrderByDescending(a => a.PublicadoEm)
            .Take(4)
            .ToListAsync();

        var dto = new ArtigoDetailDto
        {
            Id = artigo.Id,
            Slug = artigo.Slug,
            Titulo = artigo.Titulo,
            Subtitulo = artigo.Subtitulo,
            Resumo = artigo.Resumo,
            ConteudoHtml = artigo.ConteudoHtml,
            ImagemCapaUrl = artigo.ImagemCapaUrl,
            Patrocinado = artigo.Patrocinado,
            Publicada = artigo.Publicada,
            PublicadoEm = artigo.PublicadoEm,
            Visualizacoes = artigo.Visualizacoes,
            VerticalNome = artigo.Vertical?.Nome ?? string.Empty,
            VerticalSlug = artigo.Vertical?.Slug ?? string.Empty,
            CategoriaNome = artigo.Categoria?.Nome,
            CategoriaSlug = artigo.Categoria?.Slug,
            AutorNome = !string.IsNullOrWhiteSpace(artigo.AutorExibicao) ? artigo.AutorExibicao : artigo.Autor?.NomeCompleto ?? string.Empty,
            RelacionadosMesmaVertical = relacionados.Select(ParaListItemDto),
        };

        return Ok(dto);
    }

    private static string GerarSlug(string titulo)
    {
        var normalizado = titulo.Trim().ToLowerInvariant();
        normalizado = Regex.Replace(normalizado, "[^a-z0-9\\s-]", "");
        normalizado = Regex.Replace(normalizado, "\\s+", "-").Trim('-');
        return normalizado;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<ActionResult<ArtigoDetailDto>> Criar(ArtigoCreateDto dto)
    {
        var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var verticalExiste = await _context.Verticais.AnyAsync(v => v.Id == dto.VerticalId);
        if (!verticalExiste)
        {
            return BadRequest(new { mensagem = "Vertical informada não existe." });
        }

        if (dto.CategoriaId is not null && !await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
        {
            return BadRequest(new { mensagem = "Categoria informada não existe." });
        }

        var slugBase = GerarSlug(dto.Titulo);
        if (string.IsNullOrWhiteSpace(slugBase))
        {
            return BadRequest(new { mensagem = "Não foi possível gerar um slug a partir do título." });
        }

        var slug = slugBase;
        var sufixo = 1;
        while (await _context.Artigos.AnyAsync(a => a.Slug == slug))
        {
            slug = $"{slugBase}-{++sufixo}";
        }

        var artigo = new Artigo
        {
            Slug = slug,
            Titulo = dto.Titulo,
            Subtitulo = dto.Subtitulo,
            Resumo = dto.Resumo,
            ConteudoHtml = _sanitizer.Sanitizar(dto.ConteudoHtml),
            ImagemCapaUrl = dto.ImagemCapaUrl,
            Patrocinado = dto.Patrocinado,
            Destaque = dto.Destaque,
            Publicada = dto.Publicada,
            PublicadoEm = dto.Publicada ? DateTime.UtcNow : null,
            VerticalId = dto.VerticalId,
            CategoriaId = dto.CategoriaId,
            AutorId = autorId,
            AutorExibicao = string.IsNullOrWhiteSpace(dto.AutorExibicao) ? null : dto.AutorExibicao.Trim(),
        };

        _context.Artigos.Add(artigo);
        await _context.SaveChangesAsync();

        await _context.Entry(artigo).Reference(a => a.Vertical).LoadAsync();
        await _context.Entry(artigo).Reference(a => a.Categoria).LoadAsync();
        await _context.Entry(artigo).Reference(a => a.Autor).LoadAsync();

        return CreatedAtAction(nameof(ObterPorSlug), new { slug = artigo.Slug }, ParaListItemDto(artigo));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Atualizar(int id, ArtigoUpdateDto dto)
    {
        var artigo = await _context.Artigos.FirstOrDefaultAsync(a => a.Id == id);
        if (artigo is null)
        {
            return NotFound();
        }

        var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ehAdmin = User.IsInRole(Roles.Admin);
        if (!ehAdmin && artigo.AutorId != autorId)
        {
            return Forbid();
        }

        var verticalExiste = await _context.Verticais.AnyAsync(v => v.Id == dto.VerticalId);
        if (!verticalExiste)
        {
            return BadRequest(new { mensagem = "Vertical informada não existe." });
        }

        if (dto.CategoriaId is not null && !await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
        {
            return BadRequest(new { mensagem = "Categoria informada não existe." });
        }

        var jaEstavaPublicada = artigo.Publicada;

        artigo.Titulo = dto.Titulo;
        artigo.Subtitulo = dto.Subtitulo;
        artigo.Resumo = dto.Resumo;
        artigo.ConteudoHtml = _sanitizer.Sanitizar(dto.ConteudoHtml);
        artigo.ImagemCapaUrl = dto.ImagemCapaUrl;
        artigo.Patrocinado = dto.Patrocinado;
        artigo.Destaque = dto.Destaque;
        artigo.Publicada = dto.Publicada;
        artigo.AtualizadoEm = DateTime.UtcNow;
        artigo.VerticalId = dto.VerticalId;
        artigo.CategoriaId = dto.CategoriaId;
        artigo.AutorExibicao = string.IsNullOrWhiteSpace(dto.AutorExibicao) ? null : dto.AutorExibicao.Trim();

        if (dto.Publicada && !jaEstavaPublicada)
        {
            artigo.PublicadoEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Deletar(int id)
    {
        var artigo = await _context.Artigos.FirstOrDefaultAsync(a => a.Id == id);
        if (artigo is null)
        {
            return NotFound();
        }

        var autorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ehAdmin = User.IsInRole(Roles.Admin);
        if (!ehAdmin && artigo.AutorId != autorId)
        {
            return Forbid();
        }

        _context.Artigos.Remove(artigo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
