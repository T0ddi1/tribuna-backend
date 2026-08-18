namespace NewsPortal.Api.Models;

public class Favorito
{
    public int Id { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }

    public int ArtigoId { get; set; }
    public Artigo? Artigo { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
