using APIFORD.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIFORD.Services.UserServices;

public class TokenService
{
    private IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public TokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("username", user.UserName ?? string.Empty),
            new Claim("email", user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("loginTimestamp", DateTime.UtcNow.ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var chave = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
            (_configuration["SymmetricSecurityKey"])
            ); //maneira certa

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var horasExpiracao = _configuration.GetValue<double>("TokenExpiracaoHoras", 12);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(horasExpiracao),
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
