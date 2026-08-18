using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class NewsletterInscricaoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;
}
