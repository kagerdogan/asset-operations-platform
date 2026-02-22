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
    public DateTime? NextDueDate { get; private set; }
    private readonly List<DateTime> _completionHistory = new();
    public IReadOnlyCollection<DateTime> CompletionHistory => _completionHistory.AsReadOnly();
    public bool IsPaused { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public int? LastCompletedRunningHour { get; private set; }
    public int? NextDueRunningHour { get; private set; }
    private int? _remainingHours;
    private TimeSpan? _remainingTime;
    public int? RemainingHours => _remainingHours;
    public TimeSpan? RemainingTime => _remainingTime;
    private MaintenanceTask() { }

    // Periodic bakım için constructor (sonradan ekleriz diğer paremetreleri)
    public MaintenanceTask(
       Guid assetId,
       string title,
       string instruction,
       MaintenancePeriod period,
       int? lastCompletedRunningHour,
       DateTime? lastCompletedAt)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        Title = title;
        Instruction = instruction;
        Type = MaintenancePeriodType.Periodic;
        Period = period;

        if (period.Unit == PeriodUnit.Hours)
        {
            if (lastCompletedRunningHour is null)
                throw new InvalidMaintenanceStateException(
                    "Running hour is required for hour-based maintenance.");

            LastCompletedRunningHour = lastCompletedRunningHour;
            NextDueRunningHour = lastCompletedRunningHour + period.Value;

            LastCompletedAt = null;
            NextDueDate = null;
        }
        else
        {
            if (lastCompletedAt is null)
                throw new InvalidMaintenanceStateException(
                    "Last completion date is required for date-based maintenance.");

            LastCompletedAt = lastCompletedAt;
            NextDueDate = period.AddTo(lastCompletedAt.Value);

            LastCompletedRunningHour = null;
            NextDueRunningHour = null;
        }
    }

    // When Necessary bakım için factory method
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

    public MaintenanceStatus GetStatus(
    DateTime now,
    int currentRunningHour,
    TimeSpan dueWindow)
    {
        if (IsPaused)
            return MaintenanceStatus.Deactive;

        if (Type == MaintenancePeriodType.WhenNecessary)
            return MaintenanceStatus.Normal;

        if (Period is null)
            throw new InvalidMaintenanceStateException("Period is required.");

        return Period.Unit switch
        {
            PeriodUnit.Hours => GetHourStatus(currentRunningHour, dueWindow),
            _ => GetDateStatus(now, dueWindow)
        };
    }
    private MaintenanceStatus GetDateStatus(
    DateTime now,
    TimeSpan dueWindow)
    {
        if (NextDueDate is null)
            throw new InvalidMaintenanceStateException(
                "NextDueDate must be calculated for date-based maintenance.");

        if (now > NextDueDate.Value)
            return MaintenanceStatus.Overdue;

        return (NextDueDate.Value - now) <= dueWindow
            ? MaintenanceStatus.Due
            : MaintenanceStatus.Normal;
    }
    private MaintenanceStatus GetHourStatus(
    int currentRunningHour,
    TimeSpan dueWindow)
    {
        if (NextDueRunningHour is null)
            throw new InvalidMaintenanceStateException(
                "NextDueRunningHour must be calculated for hour-based maintenance.");

        if (currentRunningHour > NextDueRunningHour.Value)
            return MaintenanceStatus.Overdue;

        int hourWindow = (int)dueWindow.TotalHours;

        return (NextDueRunningHour.Value - currentRunningHour) <= hourWindow
            ? MaintenanceStatus.Due
            : MaintenanceStatus.Normal;
    }
    public void Complete(DateTime completionDate, int? currentRunningHour)
    {
        // Pause state temizlenir
        if (IsPaused)
        {
            IsPaused = false;
            _remainingTime = null;
            _remainingHours = null;
        }

        if (Type == MaintenancePeriodType.WhenNecessary)
        {
            _completionHistory.Add(completionDate);
            LastCompletedAt = completionDate;
            return;
        }

        if (Period is null)
            throw new InvalidMaintenanceStateException("Period is required.");

        if (Period.Unit == PeriodUnit.Hours)
        {
            if (currentRunningHour is null)
                throw new InvalidMaintenanceStateException(
                    "Running hour required for hour-based completion.");

            LastCompletedRunningHour = currentRunningHour.Value;
            NextDueRunningHour = currentRunningHour.Value + Period.Value;
        }
        else
        {
            LastCompletedAt = completionDate;
            NextDueDate = Period.AddTo(completionDate);
        }

        _completionHistory.Add(completionDate);
    }
    public void Deactivate(DateTime now, int? currentRunningHour)
    {
        if (IsPaused) return;
        if (Type == MaintenancePeriodType.WhenNecessary) return;

        if (Period is null)
            throw new InvalidMaintenanceStateException("Period is required.");

        if (Period.Unit == PeriodUnit.Hours)
            DeactivateHour(currentRunningHour);
        else
            DeactivateDate(now);

        IsPaused = true;
        PausedAt = now;
    }
    private void DeactivateDate(DateTime now)
    {
        if (NextDueDate is null)
            throw new InvalidMaintenanceStateException(
                "Cannot deactivate without a calculated NextDueDate.");

        _remainingTime = NextDueDate.Value - now;
    }
    private void DeactivateHour(int? currentRunningHour)
    {
        if (NextDueRunningHour is null || currentRunningHour is null)
            throw new InvalidMaintenanceStateException(
                "Cannot deactivate hour-based task.");

        _remainingHours = NextDueRunningHour.Value - currentRunningHour.Value;
    }
    public void Reactivate(DateTime now, int? currentRunningHour)
    {
        if (!IsPaused) return;

        if (Period is null)
            throw new InvalidMaintenanceStateException("Period is required.");

        if (Period.Unit == PeriodUnit.Hours)
            ReactivateHour(currentRunningHour);
        else
            ReactivateDate(now);

        IsPaused = false;
        PausedAt = null;
    }
    private void ReactivateDate(DateTime now)
    {
        if (_remainingTime is null)
            throw new InvalidMaintenanceStateException(
                "Cannot reactivate date-based task.");

        NextDueDate = now + _remainingTime.Value;
        _remainingTime = null;
    }
    private void ReactivateHour(int? currentRunningHour)
    {
        if (_remainingHours is null || currentRunningHour is null)
            throw new InvalidMaintenanceStateException(
                "Cannot reactivate hour-based task.");

        NextDueRunningHour = currentRunningHour.Value + _remainingHours.Value;
        _remainingHours = null;
    }
}
