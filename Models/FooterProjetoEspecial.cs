namespace NewsPortal.Api.Models;

public class FooterProjetoEspecial
{
    public int Id { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
}
