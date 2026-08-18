namespace NewsPortal.Api.Services;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string assunto, string corpoHtml);
}
