namespace NewsPortal.Api.Models;

public class Artigo
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string Resumo { get; set; } = string.Empty;

    // Corpo do artigo em HTML rico (saída do editor Quill), sempre sanitizado
    // no servidor antes de ser persistido — ver Services/HtmlSanitizerService.
    public string ConteudoHtml { get; set; } = string.Empty;

    public string? ImagemCapaUrl { get; set; }
    public bool Patrocinado { get; set; } = false;
    public bool Destaque { get; set; } = false;

    public bool Publicada { get; set; } = false;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? PublicadoEm { get; set; }

    public int Visualizacoes { get; set; } = 0;

    public int VerticalId { get; set; }
    public Vertical? Vertical { get; set; }

    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public string AutorId { get; set; } = string.Empty;
    public ApplicationUser? Autor { get; set; }

    // Nome exibido como autor, quando quem publicou não é quem escreveu
    // (ex.: Admin subindo um texto de um colunista externo). Se vazio, usa o
    // nome da conta que publicou (Autor.NomeCompleto).
    public string? AutorExibicao { get; set; }

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
