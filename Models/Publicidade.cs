namespace NewsPortal.Api.Models;

// Bloco de publicidade/patrocínio (o que aparece como "PUBLICIDADE",
// "CONTEÚDO PATROCINADO" ou "PATROCÍNIO" na home e nas verticais) — não é
// matéria de notícia, é inventário comercial gerenciado pelo admin.
public class Publicidade
{
    public int Id { get; set; }

    // Selo mostrado acima do título: "PUBLICIDADE", "CONTEÚDO PATROCINADO"...
    public string Eyebrow { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CtaLabel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? LogoUrl { get; set; }

    // "light" | "brand" — estilo visual do card.
    public string Tema { get; set; } = "light";
    // "sm" | "md" | "lg" — controla quanto espaço o card ocupa na grade.
    public string Tamanho { get; set; } = "md";
    public string? SiteLabel { get; set; }
    public bool Externo { get; set; } = true;

    public bool Ativo { get; set; } = true;
    public int Ordem { get; set; } = 0;

    // Onde aparece: null = só na home; setado = só naquela vertical;
    // ExibirEmTodasAsPaginas true = home + toda vertical (ignora VerticalId).
    public int? VerticalId { get; set; }
    public Vertical? Vertical { get; set; }
    public bool ExibirEmTodasAsPaginas { get; set; } = false;
}
