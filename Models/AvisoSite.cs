namespace NewsPortal.Api.Models;

// Aviso em popup exibido aos visitantes dentro da janela InicioEm–FimEm.
public class AvisoSite
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
    public string? ImagemUrl { get; set; }
    public bool SomenteImagem { get; set; }
    public string? TextoBotao { get; set; }
    public string? Link { get; set; }
    public bool Externo { get; set; }
    public bool PermiteNaoVerMais { get; set; } = true;
    public DateTime? InicioEm { get; set; }
    public DateTime? FimEm { get; set; }
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}
