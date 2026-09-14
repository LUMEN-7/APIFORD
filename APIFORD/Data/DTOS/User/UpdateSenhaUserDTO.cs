namespace APIFORD.Data.DTOS.User;

public class UpdateSenhaUserDTO
{
    public string SenhaAtual { get; set; } = string.Empty;
    public string SenhaNova { get; set; } = string.Empty;
}

public record TrocarSenhaDTO(string SenhaAtual, string SenhaNova);
public record EsqueciSenhaDTO(string Email);
public record RedefinirSenhaDTO(string Email, string Codigo, string SenhaNova);