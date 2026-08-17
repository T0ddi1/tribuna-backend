using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class ArtigoCreateDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Subtitulo { get; set; }

    [Required, MaxLength(500)]
    public string Resumo { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<string> Paragrafos { get; set; } = [];

    [MaxLength(500)]
    public string? ImagemCapaUrl { get; set; }

    public bool Patrocinado { get; set; } = false;
    public bool Destaque { get; set; } = false;
    public bool Publicada { get; set; } = false;

    [Required]
    public int CategoriaId { get; set; }
}

public class ArtigoUpdateDto : ArtigoCreateDto
{
}

public class ArtigoListItemDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Resumo { get; set; } = string.Empty;
    public string? ImagemCapaUrl { get; set; }
    public bool Patrocinado { get; set; }
    public bool Publicada { get; set; }
    public DateTime? PublicadoEm { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public string CategoriaSlug { get; set; } = string.Empty;
    public string AutorNome { get; set; } = string.Empty;
}

public class ArtigoDetailDto : ArtigoListItemDto
{
    public string? Subtitulo { get; set; }
    public List<string> Paragrafos { get; set; } = [];
    public int Visualizacoes { get; set; }
    public IEnumerable<ArtigoListItemDto> RelacionadosMesmaCategoria { get; set; } = [];
}

public class PaginaResultDto<T>
{
    public IEnumerable<T> Itens { get; set; } = [];
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }
}
