using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class ArtigoCreateDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Subtitulo { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Resumo { get; set; } = string.Empty;

    // HTML produzido pelo editor Quill. Sanitizado no servidor antes de salvar.
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MinLength(1, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres."), MaxLength(200_000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string ConteudoHtml { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? ImagemCapaUrl { get; set; }

    public bool Patrocinado { get; set; } = false;
    public bool Destaque { get; set; } = false;
    public bool Publicada { get; set; } = false;

    // Seção do site onde o artigo aparece (Capital, Esportes, Tech, Games...).
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int VerticalId { get; set; }

    // Tema/tag opcional, livre entre verticais — usado só pra filtrar no blog.
    public int? CategoriaId { get; set; }

    // Nome exibido como autor. Vazio = usa o nome de quem está publicando
    // (útil quando o Admin sobe um texto de terceiros).
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? AutorExibicao { get; set; }
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
    public string VerticalNome { get; set; } = string.Empty;
    public string VerticalSlug { get; set; } = string.Empty;
    public string? CategoriaNome { get; set; }
    public string? CategoriaSlug { get; set; }
    public string AutorNome { get; set; } = string.Empty;
}

public class ArtigoDetailDto : ArtigoListItemDto
{
    public string? Subtitulo { get; set; }
    public string ConteudoHtml { get; set; } = string.Empty;
    public int Visualizacoes { get; set; }
    public IEnumerable<ArtigoListItemDto> RelacionadosMesmaVertical { get; set; } = [];
}

// Usado só nas telas de gestão (admin/editor): inclui rascunhos e os IDs
// crus, necessários para pré-preencher o formulário de edição.
public class ArtigoAdminDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string Resumo { get; set; } = string.Empty;
    public string ConteudoHtml { get; set; } = string.Empty;
    public string? ImagemCapaUrl { get; set; }
    public bool Patrocinado { get; set; }
    public bool Destaque { get; set; }
    public bool Publicada { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? PublicadoEm { get; set; }
    public int Visualizacoes { get; set; }
    public int VerticalId { get; set; }
    public string VerticalNome { get; set; } = string.Empty;
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public string AutorId { get; set; } = string.Empty;
    public string AutorNome { get; set; } = string.Empty;
    public string? AutorExibicao { get; set; }
}

public class PaginaResultDto<T>
{
    public IEnumerable<T> Itens { get; set; } = [];
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }
}
