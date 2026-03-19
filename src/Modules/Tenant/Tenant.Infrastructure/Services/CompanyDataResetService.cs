using Chat.Persistence.Data.ChatDb;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Infrastructure.Data.NotificationDb;
using ProjectManagement.Persistence.Data.ProjectManagementDb;
using Report.Persistence.Data;
using Stats.Persistence.Data;
using Tenant.Application.Services;

namespace Tenant.Infrastructure.Services;

public sealed class CompanyDataResetService : ICompanyDataResetService
{
    private readonly IdentityManagementDbContext _identityContext;
    private readonly ProjectManagementDbContext _projectManagementContext;
    private readonly ReportDbContext _reportContext;
    private readonly StatsDbContext _statsContext;
    private readonly ChatDbContext _chatContext;
    private readonly NotificationDbContext _notificationContext;
    private readonly ILogger<CompanyDataResetService> _logger;

    public CompanyDataResetService(
        IdentityManagementDbContext identityContext,
        ProjectManagementDbContext projectManagementContext,
        ReportDbContext reportContext,
        StatsDbContext statsContext,
        ChatDbContext chatContext,
        NotificationDbContext notificationContext,
        ILogger<CompanyDataResetService> logger)
    {
        _identityContext = identityContext;
        _projectManagementContext = projectManagementContext;
        _reportContext = reportContext;
        _statsContext = statsContext;
        _chatContext = chatContext;
        _notificationContext = notificationContext;
        _logger = logger;
    }

    public async Task ResetCompanyDataAsync(Guid companyId, CancellationToken cancellationToken)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("CompanyId zorunludur.", nameof(companyId));
        }

        _logger.LogInformation("Company data reset started. CompanyId={CompanyId}", companyId);

        var allCompanyUserIds = await _identityContext.Users
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId)
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);

        var companyGroupIds = await _identityContext.Groups
            .AsNoTracking()
            .Where(group => group.CompanyId == companyId)
            .Select(group => group.Id)
            .ToListAsync(cancellationToken);

        var companyDepartmentIds = await _identityContext.Departments
            .AsNoTracking()
            .Where(department => department.CompanyId == companyId)
            .Select(department => department.Id)
            .ToListAsync(cancellationToken);

        if (allCompanyUserIds.Count > 0)
        {
            var relatedTaskIds = await _projectManagementContext.Subtasks
                .AsNoTracking()
                .Where(subtask => allCompanyUserIds.Contains(subtask.AssignedUserId))
                .Select(subtask => subtask.TaskId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var relatedSubtaskIds = await _projectManagementContext.Subtasks
                .AsNoTracking()
                .Where(subtask =>
                    allCompanyUserIds.Contains(subtask.AssignedUserId)
                    || relatedTaskIds.Contains(subtask.TaskId))
                .Select(subtask => subtask.Id)
                .ToListAsync(cancellationToken);

            if (relatedSubtaskIds.Count > 0)
            {
                await _projectManagementContext.SubTaskAnswers
                    .Where(answer =>
                        (EF.Property<Guid?>(answer, "SubtaskId").HasValue
                            && relatedSubtaskIds.Contains(EF.Property<Guid?>(answer, "SubtaskId")!.Value))
                        || allCompanyUserIds.Contains(answer.SenderId))
                    .ExecuteDeleteAsync(cancellationToken);
            }
            else
            {
                await _projectManagementContext.SubTaskAnswers
                    .Where(answer => allCompanyUserIds.Contains(answer.SenderId))
                    .ExecuteDeleteAsync(cancellationToken);
            }

            await _projectManagementContext.Subtasks
                .Where(subtask =>
                    allCompanyUserIds.Contains(subtask.AssignedUserId)
                    || relatedTaskIds.Contains(subtask.TaskId))
                .ExecuteDeleteAsync(cancellationToken);

            if (relatedTaskIds.Count > 0)
            {
                await _projectManagementContext.Tasks
                    .Where(task => relatedTaskIds.Contains(task.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }

            await _projectManagementContext.IndividualTasks
                .Where(task => allCompanyUserIds.Contains(task.AssignedUserId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (allCompanyUserIds.Count > 0 || companyDepartmentIds.Count > 0)
        {
            await _reportContext.Reports
                .Where(report =>
                    (allCompanyUserIds.Count > 0 && allCompanyUserIds.Contains(report.ReportingUserId))
                    || (companyDepartmentIds.Count > 0 && companyDepartmentIds.Contains(report.NotifiedDepartmantId)))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (allCompanyUserIds.Count > 0)
        {
            await _statsContext.UserStats
                .Where(stat => allCompanyUserIds.Contains(stat.UserId))
                .ExecuteDeleteAsync(cancellationToken);

            await _notificationContext.NotificationMessages
                .Where(notification => allCompanyUserIds.Contains(notification.ReceiverUserId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (allCompanyUserIds.Count > 0 || companyGroupIds.Count > 0)
        {
            await _chatContext.Messages
                .Where(message =>
                    (allCompanyUserIds.Count > 0
                     && (allCompanyUserIds.Contains(message.SenderId)
                         || (message.ReceiverId.HasValue && allCompanyUserIds.Contains(message.ReceiverId.Value))))
                    || (companyGroupIds.Count > 0
                        && message.GroupId.HasValue
                        && companyGroupIds.Contains(message.GroupId.Value)))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (companyGroupIds.Count > 0)
        {
            await _identityContext.GroupsMembers
                .Where(member => companyGroupIds.Contains(member.GroupId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (companyDepartmentIds.Count > 0)
        {
            await _identityContext.DepartmentMembers
                .Where(member => companyDepartmentIds.Contains(member.DepartmentId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (companyGroupIds.Count > 0)
        {
            await _identityContext.Groups
                .Where(group => companyGroupIds.Contains(group.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (companyDepartmentIds.Count > 0)
        {
            await _identityContext.Departments
                .Where(department => companyDepartmentIds.Contains(department.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (allCompanyUserIds.Count > 0)
        {
            var protectedRoleIds = await _identityContext.Roles
                .AsNoTracking()
                .Where(role =>
                    role.NormalizedName != null
                    && (role.NormalizedName == "COMPANY"
                        || role.NormalizedName == "ADMIN"))
                .Select(role => role.Id)
                .ToListAsync(cancellationToken);

            List<Guid> protectedUserIds = [];
            if (protectedRoleIds.Count > 0)
            {
                protectedUserIds = await _identityContext.UserRoles
                    .AsNoTracking()
                    .Where(userRole => protectedRoleIds.Contains(userRole.RoleId))
                    .Select(userRole => userRole.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }

            var workerUserIds = allCompanyUserIds
                .Where(userId => !protectedUserIds.Contains(userId))
                .ToList();

            if (workerUserIds.Count > 0)
            {
                await _identityContext.UserClaims
                    .Where(claim => workerUserIds.Contains(claim.UserId))
                    .ExecuteDeleteAsync(cancellationToken);

                await _identityContext.UserLogins
                    .Where(login => workerUserIds.Contains(login.UserId))
                    .ExecuteDeleteAsync(cancellationToken);

                await _identityContext.UserTokens
                    .Where(token => workerUserIds.Contains(token.UserId))
                    .ExecuteDeleteAsync(cancellationToken);

                await _identityContext.UserRoles
                    .Where(userRole => workerUserIds.Contains(userRole.UserId))
                    .ExecuteDeleteAsync(cancellationToken);

                await _identityContext.Users
                    .Where(user => workerUserIds.Contains(user.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Company data reset completed. CompanyId={CompanyId}", companyId);
    }
}
