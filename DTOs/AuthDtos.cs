using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class LoginDto
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Senha { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
}

public class CriarUsuarioDto
{
    [Required, MaxLength(150)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(10), MaxLength(200)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = Models.Roles.Editor;
}

public class UsuarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
}
