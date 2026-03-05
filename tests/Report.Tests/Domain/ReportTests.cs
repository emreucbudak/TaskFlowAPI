using ReportEntity = Report.Domain.Entities.Report;

namespace Report.Tests.Domain;

public class ReportTests
{
    [Fact]
    public void Constructor_WithValidParams_ShouldCreate()
    {
        // Arrange
        var reportTopicId = 1;
        var description = "Description";
        var reportingUserId = Guid.NewGuid();
        var reportStatusId = 1;
        var title = "Title";
        var notifiedDepartmentId = Guid.NewGuid();

        // Act
        var report = new ReportEntity(
            reportTopicId,
            description,
            reportingUserId,
            reportStatusId,
            title,
            notifiedDepartmentId);

        // Assert
        Assert.Equal(reportTopicId, report.ReportTopicId);
        Assert.Equal(description, report.Description);
        Assert.Equal(reportingUserId, report.ReportingUserId);
        Assert.Equal(reportStatusId, report.ReportStatusId);
        Assert.Equal(title, report.Title);
        Assert.Equal(notifiedDepartmentId, report.NotifiedDepartmantId);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldThrow()
    {
        // Arrange
        var reportingUserId = Guid.NewGuid();

        // Act
        Action act = () => new ReportEntity(1, string.Empty, reportingUserId, 1, "Title", Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ShouldThrow()
    {
        // Arrange
        var reportingUserId = Guid.NewGuid();

        // Act
        Action act = () => new ReportEntity(1, "Desc", reportingUserId, 1, string.Empty, Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrow()
    {
        // Arrange
        var emptyUserId = Guid.Empty;

        // Act
        Action act = () => new ReportEntity(1, "Desc", emptyUserId, 1, "Title", Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateReportStatus_ShouldChange()
    {
        // Arrange
        var report = CreateReport();

        // Act
        report.UpdateReportStatus(3);

        // Assert
        Assert.Equal(3, report.ReportStatusId);
    }

    [Fact]
    public void UpdateTitle_ShouldChange()
    {
        // Arrange
        var report = CreateReport();

        // Act
        report.UpdateTitle("New Title");

        // Assert
        Assert.Equal("New Title", report.Title);
    }

    [Fact]
    public void UpdateDescription_ShouldChange()
    {
        // Arrange
        var report = CreateReport();

        // Act
        report.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", report.Description);
    }

    [Fact]
    public void UpdateReportTopic_ShouldChange()
    {
        // Arrange
        var report = CreateReport();

        // Act
        report.UpdateReportTopic(7);

        // Assert
        Assert.Equal(7, report.ReportTopicId);
    }

    private static ReportEntity CreateReport()
    {
        return new ReportEntity(
            1,
            "Description",
            Guid.NewGuid(),
            1,
            "Title",
            Guid.NewGuid());
    }
}
