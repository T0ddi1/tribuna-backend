using System.Net;
using System.Net.Mail;

namespace NewsPortal.Api.Services;

/// <summary>
/// Envia e-mails via SMTP. Se "Smtp:Host" não estiver configurado (ex.: ambiente
/// de desenvolvimento sem credenciais), apenas loga a intenção de envio em vez de falhar.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpoHtml)
    {
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogInformation("Smtp:Host não configurado — e-mail para {Destinatario} ({Assunto}) não foi enviado.", destinatario, assunto);
            return;
        }

        var porta = int.Parse(_config["Smtp:Port"] ?? "587");
        var usuario = _config["Smtp:User"];
        var senha = _config["Smtp:Password"];
        var usarSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");
        var remetenteEmail = _config["Smtp:FromEmail"] ?? usuario ?? "no-reply@otribuna.com.br";
        var remetenteNome = _config["Smtp:FromName"] ?? "Tribuna";

        using var mensagem = new MailMessage
        {
            From = new MailAddress(remetenteEmail, remetenteNome),
            Subject = assunto,
            Body = corpoHtml,
            IsBodyHtml = true,
        };
        mensagem.To.Add(destinatario);

        using var cliente = new SmtpClient(host, porta)
        {
            EnableSsl = usarSsl,
        };

        if (!string.IsNullOrWhiteSpace(usuario))
        {
            cliente.Credentials = new NetworkCredential(usuario, senha);
        }

        try
        {
            await cliente.SendMailAsync(mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail para {Destinatario}.", destinatario);
        }
    }
}
