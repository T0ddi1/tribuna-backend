using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class PublicidadeCreateDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Eyebrow { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string CtaLabel { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Href { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? ImagemUrl { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? LogoUrl { get; set; }

    [RegularExpression("^(light|brand)$", ErrorMessage = "Tema deve ser 'light' ou 'brand'.")]
    public string Tema { get; set; } = "light";

    [RegularExpression("^(sm|md|lg)$", ErrorMessage = "Tamanho deve ser 'sm', 'md' ou 'lg'.")]
    public string Tamanho { get; set; } = "md";

    [MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? SiteLabel { get; set; }

    public bool Externo { get; set; } = true;
    public bool Ativo { get; set; } = true;
    public int Ordem { get; set; } = 0;
    public int? VerticalId { get; set; }
    public bool ExibirEmTodasAsPaginas { get; set; } = false;
}

public class PublicidadeUpdateDto : PublicidadeCreateDto
{
}

public class PublicidadeAdminDto
{
    public int Id { get; set; }
    public string Eyebrow { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CtaLabel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string Tema { get; set; } = string.Empty;
    public string Tamanho { get; set; } = string.Empty;
    public string? SiteLabel { get; set; }
    public bool Externo { get; set; }
    public bool Ativo { get; set; }
    public int Ordem { get; set; }
    public int? VerticalId { get; set; }
    public string? VerticalNome { get; set; }
    public bool ExibirEmTodasAsPaginas { get; set; }
}

// Formato consumido pelo site — kind/theme já resolvidos pro que o
// componente de exibição espera, sem os campos administrativos.
public class PublicidadePublicaDto
{
    public string Eyebrow { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CtaLabel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string Tema { get; set; } = string.Empty;
    public string Tamanho { get; set; } = string.Empty;
    public string? SiteLabel { get; set; }
    public bool Externo { get; set; }
}
