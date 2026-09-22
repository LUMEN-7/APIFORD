using APIFORD.Middleware;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;

namespace APIFORD.Services;

public class GmailEmailSenderService : IEmailSenderService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _refreshToken;
    private readonly string _remetente;

    public GmailEmailSenderService(IConfiguration configuration)
    {
        _clientId = configuration["Gmail:ClientId"]
            ?? throw new InvalidOperationException("Gmail:ClientId não configurado.");
        _clientSecret = configuration["Gmail:ClientSecret"]
            ?? throw new InvalidOperationException("Gmail:ClientSecret não configurado.");
        _refreshToken = configuration["Gmail:RefreshToken"]
            ?? throw new InvalidOperationException("Gmail:RefreshToken não configurado.");
        _remetente = configuration["Gmail:Remetente"]
            ?? throw new InvalidOperationException("Gmail:Remetente não configurado.");
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    {
        try
        {
            var credential = CriarCredencial();
            using var gmail = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "APIFORD"
            });

            var mime = new MimeMessage();
            mime.From.Add(MailboxAddress.Parse(_remetente));
            mime.To.Add(MailboxAddress.Parse(destinatario));
            mime.Subject = assunto;
            mime.Body = new TextPart("html") { Text = corpo };

            using var stream = new MemoryStream();
            await mime.WriteToAsync(stream);

            var raw = Convert.ToBase64String(stream.ToArray())
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            await gmail.Users.Messages.Send(new Message { Raw = raw }, "me").ExecuteAsync();
        }
        catch (Exception ex) when (ex is not ExternalServiceException)
        {
            throw new ExternalServiceException("Gmail API", ex.Message);
        }
    }

    private UserCredential CriarCredencial()
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            },
            Scopes = new[] { GmailService.Scope.GmailSend }
        });

        return new UserCredential(flow, "apiford-sender", new TokenResponse
        {
            RefreshToken = _refreshToken
        });
    }
}