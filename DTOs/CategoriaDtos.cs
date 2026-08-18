using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class CategoriaCreateDto
{
    [Required, MaxLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(80), RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens.")]
    public string Slug { get; set; } = string.Empty;
}

public class CategoriaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int QuantidadeArtigos { get; set; }
}
