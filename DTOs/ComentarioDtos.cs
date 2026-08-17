using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class ComentarioCreateDto
{
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(2000), MinLength(3)]
    public string Texto { get; set; } = string.Empty;

    // Honeypot: campo invisível no formulário real; se vier preenchido, é bot.
    public string? Website { get; set; }
}

public class ComentarioResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}

public class ComentarioModeracaoDto : ComentarioResponseDto
{
    public string Email { get; set; } = string.Empty;
    public bool Aprovado { get; set; }
    public int ArtigoId { get; set; }
    public string ArtigoTitulo { get; set; } = string.Empty;
}
