namespace NewsPortal.Api.Models;

// Item de primeiro nível do menu do cabeçalho. Se tiver MenuLinks, vira um
// submenu em colunas (agrupadas por MenuLink.ColunaTitulo).
public class MenuItem
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;

    public ICollection<MenuLink> Links { get; set; } = new List<MenuLink>();
}
