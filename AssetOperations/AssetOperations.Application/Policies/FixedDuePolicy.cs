using AssetOperations.Application.Policies;
using AssetOperations.Domain.Entities;

namespace AssetOperations.Tests;

public sealed class FixedDuePolicy : IMaintenanceDuePolicy
{
    private readonly TimeSpan _window;

    public FixedDuePolicy(TimeSpan window)
    {
        _window = window;
    }

    public TimeSpan CalculateDueWindow(MaintenanceTask maintenance)
    {
        return _window;
    }
}