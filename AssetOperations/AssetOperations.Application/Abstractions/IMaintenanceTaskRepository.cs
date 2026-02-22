using AssetOperations.Domain.Entities;

namespace AssetOperations.Application.Abstractions;

public interface IMaintenanceTaskRepository
{
    Task<MaintenanceTask?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MaintenanceTask>> ListAsync(CancellationToken ct = default);
    Task AddAsync(MaintenanceTask task, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}