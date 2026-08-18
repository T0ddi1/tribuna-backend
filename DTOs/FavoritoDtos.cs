namespace NewsPortal.Api.DTOs;

public class FavoritoResponseDto
{
    public string Slug { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? ImagemCapaUrl { get; set; }
    public DateTime? PublicadoEm { get; set; }
}
