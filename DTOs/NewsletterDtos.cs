using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class NewsletterInscricaoDto
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;
}
