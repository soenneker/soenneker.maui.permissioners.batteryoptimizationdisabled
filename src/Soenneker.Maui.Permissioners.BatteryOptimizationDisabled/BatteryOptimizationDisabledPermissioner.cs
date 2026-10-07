using System.Threading;
using System.Threading.Tasks;
using Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Abstract;
#if ANDROID
using Android.Content;
using Android.Provider;
using Microsoft.Maui.ApplicationModel;
#endif

namespace Soenneker.Maui.Permissioners.BatteryOptimizationDisabled;

public sealed class BatteryOptimizationDisabledPermissioner : IBatteryOptimizationDisabledPermissioner
{
    public bool IsSupported =>
#if ANDROID
        true;
#else
        false;
#endif

    public ValueTask<bool> Has(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
#if ANDROID
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
            return ValueTask.FromResult(true);
        var context = Android.App.Application.Context;
        using var manager = (Android.OS.PowerManager?)context.GetSystemService(Context.PowerService);
        return ValueTask.FromResult(manager?.IsIgnoringBatteryOptimizations(context.PackageName!) == true);
#else
        return ValueTask.FromResult(false);
#endif
    }

    public ValueTask<bool> Request(CancellationToken cancellationToken = default) => RequestCore(false, cancellationToken);

    public ValueTask<bool> RequestIfNotGranted(CancellationToken cancellationToken = default) => Request(cancellationToken);

    public ValueTask<bool> RequestExemption(CancellationToken cancellationToken = default) => RequestCore(true, cancellationToken);

    private async ValueTask<bool> RequestCore(bool directExemption, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsSupported)
            return false;
        if (await Has(cancellationToken).ConfigureAwait(false))
            return true;
#if ANDROID
        return await AndroidSettingsRequest.Run(async () =>
        {
            if (await Has(cancellationToken).ConfigureAwait(false))
                return true;
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!OperatingSystem.IsAndroidVersionAtLeast(23))
                    return true;
                using var intent = new Intent(directExemption ? Settings.ActionRequestIgnoreBatteryOptimizations : Settings.ActionIgnoreBatteryOptimizationSettings);
                if (directExemption)
                    intent.SetData(Android.Net.Uri.Parse("package:" + Android.App.Application.Context.PackageName));
                if (!await AndroidSettingsRequest.Open(intent, cancellationToken).ConfigureAwait(false))
                    return false;
                return await Has(cancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
#else
        return false;
#endif
    }
}
