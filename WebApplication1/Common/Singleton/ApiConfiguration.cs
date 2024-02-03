using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Model;

namespace WebApplication1;

public class ApiConfiguration(IConfiguration configuration)
{
    private readonly string _issuer = configuration.GetValue<string>("Jwt:Issuer") ??
                                      throw new Exception("Issuer not found in appsettings.json");

    private readonly string _audience = configuration.GetValue<string>("Jwt:Audience") ??
                                        throw new Exception("Audience not found in appsettings.json");

    private readonly string _securityKey = configuration.GetValue<string>("Jwt:SecurityKey") ??
                                           throw new Exception("SecurityKey not found in appsettings.json");


    public string GenerateToken(User userData, out JwtSecurityToken token)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim("Id", userData.Id.ToString()),
        };
        var days = configuration.GetValue<int>("Jwt:ExpireDays");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddDays(days),
            signingCredentials: signIn
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey))
        };
    }
}