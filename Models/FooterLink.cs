namespace NewsPortal.Api.Models;

// Link de uma coluna de navegação do rodapé (agrupado pelo nome da coluna).
public class FooterLink
{
    public int Id { get; set; }
    public string Coluna { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
}
