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

public class LoginResponseDTO
{
    public bool RequerDoisFatores { get; set; }
    public string? TokenDesafio { get; set; }  // presente só quando RequerDoisFatores == true
    public string? AccessToken { get; set; }   // presente só quando RequerDoisFatores == false
}

public class GoogleLoginDTO
{
    public string IdToken { get; set; } = string.Empty;
}

public class VerificarDoisFatoresDTO
{
    public string TokenDesafio { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
}

public class ConfirmarAtivacaoDoisFatoresDTO
{
    public string Codigo { get; set; } = string.Empty;
}

public class HabilitarDoisFatoresResponseDTO
{
    public string QrCodeUri { get; set; } = string.Empty;
    public string ChaveManual { get; set; } = string.Empty; // pra quem não consegue escanear QR, digita na mão
}