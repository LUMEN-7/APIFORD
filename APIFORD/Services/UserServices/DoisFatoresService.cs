using APIFORD.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace APIFORD.Services.UserServices;

public class DoisFatoresService
{
    private readonly UserManager<User> _userManager;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan ValidadeDesafio = TimeSpan.FromMinutes(5);

    public DoisFatoresService(UserManager<User> userManager, IMemoryCache cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public string GerarTokenDesafio(User user)
    {
        var token = Guid.NewGuid().ToString("N");
        _cache.Set($"2fa_desafio:{token}", user.Id, ValidadeDesafio);
        return token;
    }

    public async Task<User?> ValidarDesafioAsync(string tokenDesafio, string codigo)
    {
        if (!_cache.TryGetValue($"2fa_desafio:{tokenDesafio}", out string? userId) || userId == null)
            return null; // expirou ou nunca existiu

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        bool codigoValido = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, codigo);
        if (!codigoValido) return null;

        _cache.Remove($"2fa_desafio:{tokenDesafio}"); // uso único, não dá pra reusar o mesmo desafio 2x
        return user;
    }

    public async Task<(string QrCodeUri, string ChaveManual)> IniciarConfiguracaoAsync(User user)
    {
        await _userManager.ResetAuthenticatorKeyAsync(user); // gera chave nova, invalida qualquer configuração anterior
        var chave = await _userManager.GetAuthenticatorKeyAsync(user);
        var uri = $"otpauth://totp/APIFORD:{Uri.EscapeDataString(user.Email!)}?secret={chave}&issuer=APIFORD&digits=6";
        return (uri, chave!);
    }

    public async Task<bool> ConfirmarAtivacaoAsync(User user, string codigo)
    {
        bool valido = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, codigo);
        if (!valido) return false;

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return true;
    }

    public async Task DesativarAsync(User user)
    {
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user); // limpa a chave antiga, força reconfiguração se ligar de novo
    }
}
