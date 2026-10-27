namespace NewsPortal.Api.Models;

// Identidade global do portal (logo, favicon e paleta de cores editável).
// Sempre existe uma única linha — criada com valores vazios na primeira leitura.
public class ConfiguracaoSite
{
    public int Id { get; set; }
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }

    // JSON { "color-primary": "#aabbcc", ... } — só chaves conhecidas pelo frontend.
    public string? CoresJson { get; set; }
}
