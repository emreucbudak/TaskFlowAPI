using Moq;
using Report.Application.Features.CQRS.Reports.Query.GetByDepartment;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;
using ReportEntity = Report.Domain.Entities.Report;

namespace Report.Tests.CQRS;

public class GetDepartmentReportsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldMapPagedDepartmentReports()
    {
        var departmentId = Guid.NewGuid();
        var report = new ReportEntity(
            2,
            "Sunucu odasinda sicaklik problemi var.",
            Guid.NewGuid(),
            1,
            "Sunucu alarmi",
            departmentId);

        var repositoryMock = new Mock<IReportReadRepository>();
        repositoryMock
            .Setup(repository => repository.GetByDepartmentAsync(departmentId, 5, 2, false, null))
            .ReturnsAsync(new PagedResult<ReportEntity>
            {
                Items = [report],
                TotalCount = 7,
                Page = 2,
                PageSize = 5
            });

        var handler = new GetDepartmentReportsQueryHandler(repositoryMock.Object);
        var request = new GetDepartmentReportsQueryRequest
        {
            DepartmentId = departmentId,
            Page = 2,
            PageSize = 5
        };

        var result = await handler.Handle(request, CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(report.Id, item.Id);
        Assert.Equal(report.Title, item.Title);
        Assert.Equal(report.Description, item.Description);
        Assert.Equal(report.NotifiedDepartmantId, item.NotifiedDepartmantId);
        Assert.Equal(7, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
    }
}