using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Api.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
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
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MinLength(10, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Role { get; set; } = Models.Roles.Editor;
}

public class CadastroLeitorDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MinLength(10, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

public class EsqueciSenhaDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;
}

public class RedefinirSenhaDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório."), EmailAddress(ErrorMessage = "O campo {0} não é um e-mail válido."), MaxLength(256, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório."), MinLength(10, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres."), MaxLength(200, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}

public class UsuarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
}
