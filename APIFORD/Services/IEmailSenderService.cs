namespace APIFORD.Services;

public interface IEmailSenderService
{
    Task EnviarAsync(string destinatario, string assunto, string corpo);
}