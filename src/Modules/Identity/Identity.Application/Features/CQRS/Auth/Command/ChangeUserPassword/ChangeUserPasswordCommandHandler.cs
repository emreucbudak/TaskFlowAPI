using FlashMediator;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Command.ChangeUserPassword;

public sealed class ChangeUserPasswordCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<ChangeUserPasswordCommandRequest>
{
    public async Task Handle(ChangeUserPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null)
        {
            throw new AuthExceptions("Kullanıcı doğrulanamadı.");
        }

        if (request.UserId == Guid.Empty)
        {
            throw new UserNotFoundExceptions();
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw CreatePasswordValidationException("Yeni şifre zorunludur.");
        }

        var actor = await userManager.FindByIdAsync(currentUserId.Value.ToString());
        if (actor is null)
        {
            throw new AuthExceptions("Kullanıcı doğrulanamadı.");
        }

        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            throw new UserNotFoundExceptions();
        }

        if (user.CompanyId != actor.CompanyId)
        {
            throw new AuthExceptions("Bu çalışanın şifresini güncelleme yetkiniz yok.");
        }

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Any(role => string.Equals(role, "Worker", StringComparison.OrdinalIgnoreCase)))
        {
            throw new AuthExceptions("Sadece çalışan rolündeki kullanıcıların şifresi değiştirilebilir.");
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (resetResult.Succeeded)
        {
            return;
        }

        var passwordErrors = resetResult.Errors
            .Select(error => error.Description)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (passwordErrors.Count == 0)
        {
            throw new AuthExceptions("Çalışan şifresi güncellenemedi.");
        }

        throw CreatePasswordValidationException(string.Join(" ", passwordErrors));
    }

    private static ValidationException CreatePasswordValidationException(string message)
    {
        var failures = new[]
        {
            new ValidationFailure(nameof(ChangeUserPasswordCommandRequest.NewPassword), message)
        };

        return new ValidationException(failures);
    }
}
