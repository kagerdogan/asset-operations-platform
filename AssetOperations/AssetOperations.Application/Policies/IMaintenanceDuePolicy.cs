using AssetOperations.Domain.Entities;

namespace AssetOperations.Application.Policies;

public interface IMaintenanceDuePolicy
{
    TimeSpan CalculateDueWindow(MaintenanceTask maintenance);
}