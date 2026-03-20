using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.TokenService;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new UserNotFoundExceptions(request.Email);
            }
            bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPassword)
            {
                throw new WrongPasswordExceptions();
            }

            IList<string> roles = await userManager.GetRolesAsync(user);

            JwtSecurityToken accessToken = tokenService.CreateToken(user, roles);
            string token = new JwtSecurityTokenHandler().WriteToken(accessToken);
            string refreshToken = tokenService.CreateRefreshToken();

            return new LoginCommandResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                UserId = user.Id,
                CompanyId = user.CompanyId,
                Role = roles.FirstOrDefault() ?? string.Empty
            };
        }
    }
}
