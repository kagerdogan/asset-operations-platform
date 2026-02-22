using AssetOperations.Domain.Enums;

namespace AssetOperations.Application.Dtos;

public sealed class MaintenanceListItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public MaintenanceStatus Status { get; init; }

    public DateTime? NextDueDate { get; init; }
    public int? NextDueRunningHour { get; init; }
}