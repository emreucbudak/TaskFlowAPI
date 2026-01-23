using FlashMediator;
using Identity.Application.TokenService;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;

        public LoginCommandHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new Exception("Kullanıcı Bulunamadı!");
            }
            bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (checkPassword)
            {
                throw new Exception("Şifre Yanlış");
            }
            IList<string> roles = await userManager.GetRolesAsync(user);
            JwtSecurityToken accessToken =  tokenService.CreateToken(user, roles);
            string token = new JwtSecurityTokenHandler().WriteToken(accessToken);
            string refreshToken = tokenService.CreateRefreshToken();
            return new LoginCommandResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };
      
        }
    }
}
