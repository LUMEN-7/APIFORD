namespace APIFORD.Services;

using APIFORD.Data.DTOS;
using APIFORD.Middleware;
using APIFORD.Services;
using DocumentFormat.OpenXml.Vml;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

/// <summary>
/// Implementação de <see cref="IEmailSenderService"/> que envia e-mails via SMTP usando MailKit.
/// </summary>
public class SmtpEmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _settings;


    public SmtpEmailSenderService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;

    }

    /// <inheritdoc />
    public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(new MailboxAddress(_settings.NomeRemetente, _settings.EmailRemetente));
        mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = assunto;
        mensagem.Body = new TextPart("plain") { Text = corpo };

        using var client = new SmtpClient();


        client.CheckCertificateRevocation = false;

        
        try
        {
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_settings.EmailRemetente, _settings.SenhaApp);
            await client.SendAsync(mensagem);
        }
        catch (Exception ex)
        {
            throw new ExternalServiceException("Não foi possível enviar o e-mail.", $"{ex}");
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
