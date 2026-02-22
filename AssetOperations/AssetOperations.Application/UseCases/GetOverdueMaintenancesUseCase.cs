using AssetOperations.Application.Dtos;
using AssetOperations.Application.Policies;
using AssetOperations.Domain.Entities;
using AssetOperations.Domain.Enums;

namespace AssetOperations.Application.UseCases;

public sealed class GetOverdueMaintenancesUseCase
{
    private readonly IMaintenanceDuePolicy _duePolicy;

    public GetOverdueMaintenancesUseCase(IMaintenanceDuePolicy duePolicy)
    {
        _duePolicy = duePolicy;
    }

    public IReadOnlyList<MaintenanceListItemDto> Execute(
        IEnumerable<MaintenanceTask> maintenances,
        DateTime now,
        int currentRunningHour)
    {
        var result = new List<MaintenanceListItemDto>();

        foreach (var maintenance in maintenances)
        {
            var dueWindow = _duePolicy.CalculateDueWindow(maintenance);

            var status = maintenance.GetStatus(
                now,
                currentRunningHour,
                dueWindow);

            if (status == MaintenanceStatus.Overdue)
            {
                result.Add(new MaintenanceListItemDto
                {
                    Id = maintenance.Id,
                    Title = maintenance.Title,
                    Status = status,
                    NextDueDate = maintenance.NextDueDate,
                    NextDueRunningHour = maintenance.NextDueRunningHour
                });
            }
        }

        return result;
    }
}