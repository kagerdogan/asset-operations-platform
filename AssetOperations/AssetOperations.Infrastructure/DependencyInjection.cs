using Microsoft.Extensions.DependencyInjection;
using AssetOperations.Application.Abstractions;
using AssetOperations.Infrastructure.Repositories;

namespace AssetOperations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMaintenanceTaskRepository, InMemoryMaintenanceTaskRepository>();
        return services;
    }
}