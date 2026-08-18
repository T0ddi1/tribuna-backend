using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres."), RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens.")]
    public string Slug { get; set; } = string.Empty;
}

public class CategoriaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int QuantidadeArtigos { get; set; }
}
