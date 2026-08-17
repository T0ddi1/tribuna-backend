namespace NewsPortal.Api.Models;

public class Artigo
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string Resumo { get; set; } = string.Empty;

    // Corpo do artigo em parágrafos (armazenado como JSON: string[])
    public string ConteudoJson { get; set; } = "[]";

    public string? ImagemCapaUrl { get; set; }
    public bool Patrocinado { get; set; } = false;
    public bool Destaque { get; set; } = false;

    public bool Publicada { get; set; } = false;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? PublicadoEm { get; set; }

    public int Visualizacoes { get; set; } = 0;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public string AutorId { get; set; } = string.Empty;
    public ApplicationUser? Autor { get; set; }

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
