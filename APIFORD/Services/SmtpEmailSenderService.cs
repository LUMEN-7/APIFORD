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
    private readonly HttpClient _httpClient;
    private readonly string _remetente;



    public SmtpEmailSenderService(IOptions<EmailSettings> settings, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", configuration["Resend:ApiKey"]);
        _remetente = configuration["Resend:Remetente"] ?? "onboarding@resend.dev"; // domínio de teste do próprio Resend, funciona sem verificar domínio
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    {
        var payload = new { from = _remetente, to = new[] { destinatario }, subject = assunto, html = corpo };

        var resposta = await _httpClient.PostAsJsonAsync("emails", payload);
        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadAsStringAsync();
            throw new ExternalServiceException("Não foi possível enviar o e-mail.", erro);
        }
    }

    /// <inheritdoc />
    //public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    //{
    //    var mensagem = new MimeMessage();
    //    mensagem.From.Add(new MailboxAddress(_settings.NomeRemetente, _settings.EmailRemetente));
    //    mensagem.To.Add(MailboxAddress.Parse(destinatario));
    //    mensagem.Subject = assunto;
    //    mensagem.Body = new TextPart("plain") { Text = corpo };

    //    using var client = new SmtpClient();


    //    client.CheckCertificateRevocation = false;


    //    try
    //    {
    //        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
    //        client.AuthenticationMechanisms.Remove("XOAUTH2");
    //        await client.AuthenticateAsync(_settings.EmailRemetente, _settings.SenhaApp);
    //        await client.SendAsync(mensagem);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new ExternalServiceException("Não foi possível enviar o e-mail.", $"{ex}");
    //    }
    //    finally
    //    {
    //        await client.DisconnectAsync(true);
    //    }
    //}
}
