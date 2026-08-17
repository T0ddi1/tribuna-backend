namespace NewsPortal.Api.Models;

public class NewsletterAssinante
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
