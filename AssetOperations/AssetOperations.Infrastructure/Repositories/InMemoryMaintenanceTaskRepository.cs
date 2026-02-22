using AssetOperations.Application.Abstractions;
using AssetOperations.Domain.Entities;

namespace AssetOperations.Infrastructure.Repositories;

public sealed class InMemoryMaintenanceTaskRepository : IMaintenanceTaskRepository
{
    private static readonly List<MaintenanceTask> _store = new();

    public Task<MaintenanceTask?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.FirstOrDefault(x => x.Id == id));

    public Task<IReadOnlyList<MaintenanceTask>> ListAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<MaintenanceTask>)_store.ToList());

    public Task AddAsync(MaintenanceTask task, CancellationToken ct = default)
    {
        _store.Add(task);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => Task.CompletedTask;
}