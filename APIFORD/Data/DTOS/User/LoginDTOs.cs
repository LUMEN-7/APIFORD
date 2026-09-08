using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.User;

public class LoginUserDTO
{
    [Required(ErrorMessage = "O nome do usuário ou o e-mail é obrigatório.")]
    public string UserIdentifier { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

// Resposta — nunca é bindada a partir de input do usuário, então sem DataAnnotations.
public class LoginResponseDTO
{
    public bool RequerDoisFatores { get; set; }
    public string? TokenDesafio { get; set; }
    public string? AccessToken { get; set; }
}

public class GoogleLoginDTO
{
    [Required(ErrorMessage = "O token do Google é obrigatório.")]
    public string IdToken { get; set; } = string.Empty;
}

public class VerificarDoisFatoresDTO
{
    [Required(ErrorMessage = "O token de desafio é obrigatório.")]
    public string TokenDesafio { get; set; } = string.Empty;

    [Required(ErrorMessage = "O código é obrigatório.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O código deve ter 6 dígitos.")]
    public string Codigo { get; set; } = string.Empty;
}

public class ConfirmarAtivacaoDoisFatoresDTO
{
    [Required(ErrorMessage = "O código é obrigatório.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O código deve ter 6 dígitos.")]
    public string Codigo { get; set; } = string.Empty;
}

// Resposta — mesma lógica do LoginResponseDTO, sem validação.
public class HabilitarDoisFatoresResponseDTO
{
    public string QrCodeUri { get; set; } = string.Empty;
    public string ChaveManual { get; set; } = string.Empty;
}