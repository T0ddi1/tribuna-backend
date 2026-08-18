using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Api.Services;

/// <summary>
/// Traduz as mensagens de validação padrão do ASP.NET Identity (senha, e-mail
/// duplicado etc.) pra português — por padrão elas vêm em inglês e vazam direto
/// pro usuário final nas respostas de erro de cadastro/criação de conta.
/// </summary>
public class IdentityErrorDescriberPtBr : IdentityErrorDescriber
{
    public override IdentityError DefaultError() => new()
    {
        Code = nameof(DefaultError),
        Description = "Ocorreu um erro desconhecido.",
    };

    public override IdentityError ConcurrencyFailure() => new()
    {
        Code = nameof(ConcurrencyFailure),
        Description = "Os dados foram alterados por outra pessoa. Tente novamente.",
    };

    public override IdentityError PasswordMismatch() => new()
    {
        Code = nameof(PasswordMismatch),
        Description = "Senha incorreta.",
    };

    public override IdentityError InvalidToken() => new()
    {
        Code = nameof(InvalidToken),
        Description = "Token inválido.",
    };

    public override IdentityError RecoveryCodeRedemptionFailed() => new()
    {
        Code = nameof(RecoveryCodeRedemptionFailed),
        Description = "Não foi possível usar o código de recuperação informado.",
    };

    public override IdentityError LoginAlreadyAssociated() => new()
    {
        Code = nameof(LoginAlreadyAssociated),
        Description = "Já existe uma conta com este e-mail.",
    };

    public override IdentityError InvalidUserName(string? userName) => new()
    {
        Code = nameof(InvalidUserName),
        Description = $"O e-mail '{userName}' é inválido.",
    };

    public override IdentityError InvalidEmail(string? email) => new()
    {
        Code = nameof(InvalidEmail),
        Description = $"O e-mail '{email}' é inválido.",
    };

    public override IdentityError DuplicateUserName(string userName) => new()
    {
        Code = nameof(DuplicateUserName),
        Description = "Já existe uma conta com este e-mail.",
    };

    public override IdentityError DuplicateEmail(string email) => new()
    {
        Code = nameof(DuplicateEmail),
        Description = "Já existe uma conta com este e-mail.",
    };

    public override IdentityError InvalidRoleName(string? role) => new()
    {
        Code = nameof(InvalidRoleName),
        Description = $"O nome de role '{role}' é inválido.",
    };

    public override IdentityError DuplicateRoleName(string role) => new()
    {
        Code = nameof(DuplicateRoleName),
        Description = $"A role '{role}' já existe.",
    };

    public override IdentityError UserAlreadyHasPassword() => new()
    {
        Code = nameof(UserAlreadyHasPassword),
        Description = "Este usuário já tem uma senha definida.",
    };

    public override IdentityError UserLockoutNotEnabled() => new()
    {
        Code = nameof(UserLockoutNotEnabled),
        Description = "O bloqueio de conta não está habilitado para este usuário.",
    };

    public override IdentityError UserAlreadyInRole(string role) => new()
    {
        Code = nameof(UserAlreadyInRole),
        Description = $"O usuário já pertence à role '{role}'.",
    };

    public override IdentityError UserNotInRole(string role) => new()
    {
        Code = nameof(UserNotInRole),
        Description = $"O usuário não pertence à role '{role}'.",
    };

    public override IdentityError PasswordTooShort(int length) => new()
    {
        Code = nameof(PasswordTooShort),
        Description = $"A senha precisa ter pelo menos {length} caracteres.",
    };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
    {
        Code = nameof(PasswordRequiresUniqueChars),
        Description = $"A senha precisa ter pelo menos {uniqueChars} caracteres diferentes entre si.",
    };

    public override IdentityError PasswordRequiresNonAlphanumeric() => new()
    {
        Code = nameof(PasswordRequiresNonAlphanumeric),
        Description = "A senha precisa ter pelo menos um caractere especial (ex: ! @ # $ %).",
    };

    public override IdentityError PasswordRequiresDigit() => new()
    {
        Code = nameof(PasswordRequiresDigit),
        Description = "A senha precisa ter pelo menos um número (0-9).",
    };

    public override IdentityError PasswordRequiresLower() => new()
    {
        Code = nameof(PasswordRequiresLower),
        Description = "A senha precisa ter pelo menos uma letra minúscula (a-z).",
    };

    public override IdentityError PasswordRequiresUpper() => new()
    {
        Code = nameof(PasswordRequiresUpper),
        Description = "A senha precisa ter pelo menos uma letra maiúscula (A-Z).",
    };
}
