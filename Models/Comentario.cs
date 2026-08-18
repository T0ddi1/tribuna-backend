namespace NewsPortal.Api.Models;

public class Comentario
{
    public int Id { get; set; }
    public int ArtigoId { get; set; }
    public Artigo? Artigo { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;

    // Nullable pra não quebrar comentários antigos, feitos antes de comentar exigir
    // login (Nome/Email continuam preenchidos nesses casos, só sem conta vinculada).
    public string? UsuarioId { get; set; }
    public ApplicationUser? Usuario { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Comentários entram como pendentes e só aparecem publicamente após moderação
    // (proteção contra spam/conteúdo malicioso publicado sem revisão).
    public bool Aprovado { get; set; } = false;
    public string? IpOrigem { get; set; }
}
