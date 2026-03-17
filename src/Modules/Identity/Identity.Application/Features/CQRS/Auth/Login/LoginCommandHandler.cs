using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.TokenService;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IDepartmentMemberRepository departmentMemberRepository) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
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

            var leaderMembership = await departmentMemberRepository
                .GetLeaderMembershipAsync(user.Id, cancellationToken);

            bool isDepartmentLeader = leaderMembership is not null;
            Guid? departmentId = leaderMembership?.DepartmentId;

            JwtSecurityToken accessToken = tokenService.CreateToken(user, roles, isDepartmentLeader, departmentId);
            string token = new JwtSecurityTokenHandler().WriteToken(accessToken);
            string refreshToken = tokenService.CreateRefreshToken();

            return new LoginCommandResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                UserId = user.Id,
                CompanyId = user.CompanyId,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsDepartmentLeader = isDepartmentLeader,
                DepartmentId = departmentId
            };
        }
    }
}
