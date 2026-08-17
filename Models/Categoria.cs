namespace NewsPortal.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Descricao { get; set; }
    public string? Icone { get; set; }

    // Tema visual usado nas páginas de vertical (capital, esportes, tech, gg...)
    public string? CorAccent { get; set; }
    public string? CorAccentDark { get; set; }
    public string? CorAccentTint { get; set; }
    public bool TemaEscuro { get; set; } = false;

    public int Ordem { get; set; } = 0;

    public ICollection<Artigo> Artigos { get; set; } = new List<Artigo>();
}
