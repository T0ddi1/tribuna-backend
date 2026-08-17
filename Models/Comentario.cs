namespace NewsPortal.Api.Models;

public class Comentario
{
    public int Id { get; set; }
    public int ArtigoId { get; set; }
    public Artigo? Artigo { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Comentários entram como pendentes e só aparecem publicamente após moderação
    // (proteção contra spam/conteúdo malicioso publicado sem revisão).
    public bool Aprovado { get; set; } = false;
    public string? IpOrigem { get; set; }
}
