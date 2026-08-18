using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class VerticalCreateDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres."), RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens.")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Tagline { get; set; }

    [MaxLength(1000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Descricao { get; set; }

    [MaxLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Icone { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? CorAccent { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? CorAccentDark { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? CorAccentTint { get; set; }

    public bool TemaEscuro { get; set; } = false;
    public int Ordem { get; set; } = 0;
}

public class VerticalResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Descricao { get; set; }
    public string? Icone { get; set; }
    public string? CorAccent { get; set; }
    public string? CorAccentDark { get; set; }
    public string? CorAccentTint { get; set; }
    public bool TemaEscuro { get; set; }
    public int Ordem { get; set; }
    public int QuantidadeArtigos { get; set; }
}
