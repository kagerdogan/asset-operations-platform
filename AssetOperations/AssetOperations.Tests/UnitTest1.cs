using AssetOperations.Application.UseCases;
using AssetOperations.Domain.Entities;
using AssetOperations.Domain.Enums;
using AssetOperations.Domain.ValueObjects;
using Xunit;
using Xunit.Abstractions;

namespace AssetOperations.Tests;

public class MaintenanceTaskTests
{
    private readonly ITestOutputHelper _output;

    public MaintenanceTaskTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void HourBased_Task_Should_Return_Due_And_Overdue_Correctly()
    {
        var period = new MaintenancePeriod(500, PeriodUnit.Hours);

        var task = new MaintenanceTask(
            Guid.NewGuid(),
            "Main Engine",
            "Check lubrication",
            period,
            lastCompletedRunningHour: 10000,
            lastCompletedAt: null);

        var statusDue = task.GetStatus(
            DateTime.UtcNow,
            10490,
            TimeSpan.FromHours(20));

        _output.WriteLine($"Status at 10490: {statusDue}");

        var statusOverdue = task.GetStatus(
            DateTime.UtcNow,
            10510,
            TimeSpan.FromHours(20));

        _output.WriteLine($"Status at 10510: {statusOverdue}");

        Assert.Equal(MaintenanceStatus.Due, statusDue);
        Assert.Equal(MaintenanceStatus.Overdue, statusOverdue);
    }
    [Fact]
    public void HourBased_Task_Should_Pause_And_Resume_Correctly()
    {
        // Arrange
        var period = new MaintenancePeriod(500, PeriodUnit.Hours);

        var task = new MaintenanceTask(
            Guid.NewGuid(),
            "Main Engine",
            "Check lubrication",
            period,
            lastCompletedRunningHour: 10000,
            lastCompletedAt: null);

        // 10490 saat -> pause ediyoruz
        task.Deactivate(DateTime.UtcNow, currentRunningHour: 10490);

        // Simulate vessel running more hours
        int resumeHour = 10600;

        // Act
        task.Reactivate(DateTime.UtcNow, currentRunningHour: resumeHour);

        // Assert
        Assert.Equal(10610, task.NextDueRunningHour);
    }
    [Fact]
    public void DateBased_Task_Should_Pause_And_Resume_Correctly()
    {
        // Arrange
        var period = new MaintenancePeriod(30, PeriodUnit.Day);

        var lastCompleted = new DateTime(2025, 1, 1);

        var task = new MaintenanceTask(
            Guid.NewGuid(),
            "Generator Check",
            "Inspect cooling system",
            period,
            lastCompletedRunningHour: null,
            lastCompletedAt: lastCompleted);

        // 15 Jan 2025 -> pause
        var pauseDate = new DateTime(2025, 1, 15);

        task.Deactivate(pauseDate, currentRunningHour: null);

        // Reactivate at 1 Feb 2025
        var reactivateDate = new DateTime(2025, 2, 1);

        task.Reactivate(reactivateDate, currentRunningHour: null);

        // Assert
        var expectedNextDue = new DateTime(2025, 2, 17);

        Assert.Equal(expectedNextDue, task.NextDueDate);
    }
    [Fact]
    public void Should_Return_Only_Overdue_Maintenances()
    {
        var policy = new FixedDuePolicy(TimeSpan.FromDays(0)); // test policy
        var useCase = new GetOverdueMaintenancesUseCase(policy);

        var period = new MaintenancePeriod(1, PeriodUnit.Day);

        var overdueTask = new MaintenanceTask(
            Guid.NewGuid(),
            "Overdue Task",
            "Test",
            period,
            null,
            DateTime.UtcNow.AddDays(-5));

        var normalTask = new MaintenanceTask(
            Guid.NewGuid(),
            "Normal Task",
            "Test",
            period,
            null,
            DateTime.UtcNow);

        var list = new List<MaintenanceTask> { overdueTask, normalTask };

        var result = useCase.Execute(
            list,
            DateTime.UtcNow,
            0);

        Assert.Equal("Overdue Task", result.First().Title);
    }
}