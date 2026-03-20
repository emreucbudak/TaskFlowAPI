using Identity.Domain.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Application.TokenService
{
    public interface ITokenService
    {
        JwtSecurityToken CreateToken(User user, IList<string> roles);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        string CreateRefreshToken();

    }
}
