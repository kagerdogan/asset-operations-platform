using AssetOperations.Application.Policies;
using AssetOperations.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AssetOperations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Policies
        services.AddScoped<IMaintenanceDuePolicy, PercentBasedMaintenanceDuePolicy>();

        // UseCases
        services.AddScoped<GetMaintenanceStatusUseCase>();
        services.AddScoped<GetOverdueMaintenancesUseCase>();

        return services;
    }
}