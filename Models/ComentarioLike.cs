namespace NewsPortal.Api.Models;

public class ComentarioLike
{
    public int Id { get; set; }

    public int ComentarioId { get; set; }
    public Comentario? Comentario { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
