namespace NewsPortal.Api.Models;

public class MenuLink
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
    public string? ColunaTitulo { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
}
