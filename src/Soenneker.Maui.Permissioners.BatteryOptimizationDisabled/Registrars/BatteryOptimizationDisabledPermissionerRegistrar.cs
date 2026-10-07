using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Abstract;

namespace Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Registrars;

/// <summary>Registers the BatteryOptimizationDisabledPermissioner service.</summary>
public static class BatteryOptimizationDisabledPermissionerRegistrar
{
    /// <summary>Registers the permissioner with singleton lifetime.</summary>
    public static IServiceCollection AddBatteryOptimizationDisabledPermissionerAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IBatteryOptimizationDisabledPermissioner, BatteryOptimizationDisabledPermissioner>();
        return services;
    }

    /// <summary>Registers the permissioner with scoped lifetime.</summary>
    public static IServiceCollection AddBatteryOptimizationDisabledPermissionerAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<IBatteryOptimizationDisabledPermissioner, BatteryOptimizationDisabledPermissioner>();
        return services;
    }
}

