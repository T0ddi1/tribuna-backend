namespace NewsPortal.Api.Models;

// Textos fixos do rodapé (tagline e bloco de newsletter). Linha única.
public class ConfiguracaoRodape
{
    public int Id { get; set; }
    public string Tagline { get; set; } = string.Empty;
    public string NewsletterTitulo { get; set; } = string.Empty;
    public string NewsletterDescricao { get; set; } = string.Empty;
    public string NewsletterPlaceholder { get; set; } = string.Empty;
    public string NewsletterCta { get; set; } = string.Empty;
}
