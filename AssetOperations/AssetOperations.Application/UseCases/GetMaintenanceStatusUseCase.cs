using AssetOperations.Application.Policies;
using AssetOperations.Domain.Entities;
using AssetOperations.Domain.Enums;

namespace AssetOperations.Application.UseCases;

public sealed class GetMaintenanceStatusUseCase
{
    private readonly IMaintenanceDuePolicy _duePolicy;

    public GetMaintenanceStatusUseCase(IMaintenanceDuePolicy duePolicy)
    {
        _duePolicy = duePolicy;
    }

    public MaintenanceStatus Execute(MaintenanceTask maintenance, DateTime now)
    {
        var dueWindow = _duePolicy.CalculateDueWindow(maintenance);
        return maintenance.GetStatus(now, dueWindow);
    }
}
