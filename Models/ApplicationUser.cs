using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
