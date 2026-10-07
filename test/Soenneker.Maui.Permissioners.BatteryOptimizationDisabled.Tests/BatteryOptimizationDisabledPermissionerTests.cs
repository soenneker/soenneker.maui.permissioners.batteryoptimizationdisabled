using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Abstract;
using Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Registrars;

namespace Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Tests;

public sealed class BatteryOptimizationDisabledPermissionerTests
{
    [Test]
    public async Task Unsupported_platform_never_reports_granted()
    {
        var permissioner = new BatteryOptimizationDisabledPermissioner();
        await Assert.That(permissioner.IsSupported).IsFalse();
        await Assert.That(await permissioner.Has()).IsFalse();
        await Assert.That(await permissioner.Request()).IsFalse();
        await Assert.That(await permissioner.RequestIfNotGranted()).IsFalse();
        await Assert.That(await permissioner.RequestExemption()).IsFalse();
    }

    [Test]
    public async Task Canceled_requests_are_observed_even_when_unsupported()
    {
        var permissioner = new BatteryOptimizationDisabledPermissioner();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.That(async () => await permissioner.Has(cancellation.Token)).Throws<OperationCanceledException>();
        await Assert.That(async () => await permissioner.Request(cancellation.Token)).Throws<OperationCanceledException>();
        await Assert.That(async () => await permissioner.RequestIfNotGranted(cancellation.Token)).Throws<OperationCanceledException>();
        await Assert.That(async () => await permissioner.RequestExemption(cancellation.Token)).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task Registration_preserves_existing_lifetime()
    {
        var services = new ServiceCollection();
        services.AddBatteryOptimizationDisabledPermissionerAsScoped();
        services.AddBatteryOptimizationDisabledPermissionerAsSingleton();
        await Assert.That(services.Count).IsEqualTo(1);
        await Assert.That(services[0].ServiceType).IsEqualTo(typeof(IBatteryOptimizationDisabledPermissioner));
        await Assert.That(services[0].ImplementationType).IsEqualTo(typeof(BatteryOptimizationDisabledPermissioner));
        await Assert.That(services[0].Lifetime).IsEqualTo(ServiceLifetime.Scoped);
    }
}
