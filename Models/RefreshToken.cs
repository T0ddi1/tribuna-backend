namespace NewsPortal.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }

    // Nunca armazenamos o token em texto puro — só o hash (SHA-256).
    public string TokenHash { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime ExpiraEm { get; set; }
    public DateTime? RevogadoEm { get; set; }
    public string? SubstituidoPorHash { get; set; }
    public string? CriadoPorIp { get; set; }
    public string? RevogadoPorIp { get; set; }

    public bool Ativo => RevogadoEm is null && DateTime.UtcNow < ExpiraEm;
}
