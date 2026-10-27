namespace NewsPortal.Api.Models;

// Páginas fixas do portal (Quem somos, Termos de uso...) editáveis pelo admin.
public class PaginaInstitucional
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ResumoBusca { get; set; } = string.Empty;
    public string ConteudoHtml { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? AnexoUrl { get; set; }
    public string? AnexoLabel { get; set; }
    public bool Ativo { get; set; } = true;
}
