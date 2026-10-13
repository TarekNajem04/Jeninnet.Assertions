//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET8_0_OR_GREATER
namespace Jeninnet.Assertions;

/// <summary>Adapts any <see cref="TimeProvider"/> to <see cref="IClock"/>, so existing providers and fakes plug into <see cref="AssertionClock.Current"/>.</summary>
/// <remarks>
/// Requires .NET 8+; the older targets keep <see cref="ManualClock"/> for deterministic time. The adapter reads
/// <see cref="TimeProvider.GetLocalNow"/> for <see cref="Now"/> and <see cref="TimeProvider.GetUtcNow"/> for
/// <see cref="UtcNow"/>, honoring the provider's <see cref="TimeProvider.LocalTimeZone"/>.
/// </remarks>
public sealed class TimeProviderClock : IClock {
    private readonly TimeProvider _timeProvider;

    /// <summary>Creates the adapter around <paramref name="timeProvider"/>.</summary>
    /// <param name="timeProvider">The provider supplying the current instant.</param>
    /// <exception cref="ArgumentNullException"><paramref name="timeProvider"/> is <see langword="null"/>.</exception>
    public TimeProviderClock(TimeProvider timeProvider) {
        if(timeProvider is null) {
            throw new ArgumentNullException(nameof(timeProvider));
        }

        _timeProvider = timeProvider;
    }

    /// <summary>Gets the current local time from <see cref="TimeProvider.GetLocalNow"/>.</summary>
    public DateTimeOffset Now => _timeProvider.GetLocalNow();

    /// <summary>Gets the current UTC time from <see cref="TimeProvider.GetUtcNow"/>.</summary>
    public DateTimeOffset UtcNow => _timeProvider.GetUtcNow();
}
#endif
