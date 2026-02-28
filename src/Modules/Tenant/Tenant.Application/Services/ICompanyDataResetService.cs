namespace Tenant.Application.Services;

public interface ICompanyDataResetService
{
    Task ResetCompanyDataAsync(Guid companyId, CancellationToken cancellationToken);
}
