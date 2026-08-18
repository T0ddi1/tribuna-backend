namespace NewsPortal.Api.Models;

// Tema/assunto livre (tag) usado pra filtrar artigos dentro do blog —
// independente de vertical, um artigo pode ter no máximo uma (opcional).
public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<Artigo> Artigos { get; set; } = new List<Artigo>();
}
