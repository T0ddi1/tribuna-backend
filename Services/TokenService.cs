using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expiraEm) GerarAccessToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var chave = jwtSettings["Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada. Defina via variável de ambiente ou user-secrets.");
        var expiraMinutos = int.Parse(jwtSettings["AccessTokenMinutos"] ?? "15");
        var expiraEm = DateTime.UtcNow.AddMinutes(expiraMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("nomeCompleto", user.NomeCompleto),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }

    // Refresh token: valor opaco de alta entropia. Só o hash SHA-256 é persistido,
    // então um vazamento do banco não expõe tokens utilizáveis.
    public static string GerarRefreshTokenBruto()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public static string CalcularHash(string valor)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));
        return Convert.ToHexString(bytes);
    }
}
