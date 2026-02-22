using AssetOperations.Domain.Entities;

namespace AssetOperations.Application.Policies;

public sealed class PercentBasedMaintenanceDuePolicy : IMaintenanceDuePolicy
{
    // MVP: sabit due window (sonra config/vessel/company bazlı yaparız)
    public TimeSpan CalculateDueWindow(MaintenanceTask maintenance)
    {
        // Örn: 5 gün kala "Due" say (date tasks)
        // Hour tasks için de 20 saat kala "Due" gibi davranır (GetStatus içinde TotalHours cast ediyorsun)
        return TimeSpan.FromDays(5);
    }
}