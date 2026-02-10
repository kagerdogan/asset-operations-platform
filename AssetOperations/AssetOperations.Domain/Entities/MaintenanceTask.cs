using AssetOperations.Domain.Enums;
using AssetOperations.Domain.ValueObjects;
using AssetOperations.Domain.Exceptions;
namespace AssetOperations.Domain.Entities;

public class MaintenanceTask
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Instruction { get; private set; } = default!;
    public MaintenancePeriodType Type { get; private set; }
    public MaintenancePeriod? Period { get; private set; }

    public DateTime? LastCompletedAt { get; private set; }


    private MaintenanceTask() { }

    // Periodic bakım
    public MaintenanceTask(
        Guid assetId,
        string title,
        string instruction,
        MaintenancePeriod period,
        DateTime lastCompletedAt)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        Title = title;
        Instruction = instruction;
        Type = MaintenancePeriodType.Periodic;
        Period = period;
        LastCompletedAt = lastCompletedAt;
    }

    // When Necessary bakım
    public static MaintenanceTask CreateWhenNecessary(
        Guid assetId,
        string title, string instruction)
    {
        return new MaintenanceTask
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            Title = title,
            Instruction = instruction,
            Type = MaintenancePeriodType.WhenNecessary,
            Period = null,
            LastCompletedAt = null
        };
    }

    public MaintenanceStatus GetStatus(DateTime now, TimeSpan dueWindow)
    {
        if (Type == MaintenancePeriodType.WhenNecessary)
            return MaintenanceStatus.Normal;

        if (Period is null || LastCompletedAt is null)
            throw new InvalidMaintenanceStateException(
                "Periodic maintenance must have period and last completion date."
            );

        var dueDate = Period.AddTo(LastCompletedAt.Value);

        if (now > dueDate)
            return MaintenanceStatus.Overdue;

        return (dueDate - now) <= dueWindow
            ? MaintenanceStatus.Due
            : MaintenanceStatus.Normal;
    }



}
