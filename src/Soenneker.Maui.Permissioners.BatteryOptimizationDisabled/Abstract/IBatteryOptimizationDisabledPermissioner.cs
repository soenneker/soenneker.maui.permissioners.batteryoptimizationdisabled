using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Maui.Permissioners.BatteryOptimizationDisabled.Abstract;

/// <summary>Checks Android battery optimization exemption. Other platforms are unsupported. Direct exemption requests require android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS and an eligible app use case.</summary>
public interface IBatteryOptimizationDisabledPermissioner
{
    /// <summary>Whether this build supports the platform's permission API.</summary>
    bool IsSupported { get; }

    /// <summary>Checks current authorization without displaying UI. Returns false on unsupported platforms.</summary>
    ValueTask<bool> Has(CancellationToken cancellationToken = default);

    /// <summary>Requests authorization when missing and returns the resulting status. Returns false when unsupported or no settings activity is available.</summary>
    /// <remarks>Call from a foreground app. Cancellation stops waiting, not the system UI. Waits for the app to return from settings, up to five minutes; timeout throws TimeoutException. No app lifecycle forwarding is required.</remarks>
    ValueTask<bool> Request(CancellationToken cancellationToken = default);

    /// <summary>Checks authorization and requests it only when missing.</summary>
    ValueTask<bool> RequestIfNotGranted(CancellationToken cancellationToken = default);

    /// <summary>Requests a direct Android battery optimization exemption instead of opening the exemption list.</summary>
    /// <remarks>The caller must declare android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS and ensure its use case qualifies for direct exemption. Cancellation stops waiting, not the system UI. Times out after five minutes.</remarks>
    ValueTask<bool> RequestExemption(CancellationToken cancellationToken = default);
}

