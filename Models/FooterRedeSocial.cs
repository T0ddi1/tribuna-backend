namespace NewsPortal.Api.Models;

public class FooterRedeSocial
{
    public int Id { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
}
