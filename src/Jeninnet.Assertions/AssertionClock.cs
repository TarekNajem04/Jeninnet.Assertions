//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Selects the <see cref="IClock"/> used by time-relative assertions (default: <see cref="SystemClock"/>).</summary>
/// <example>AssertionClock.Current = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));</example>
public static class AssertionClock {
    private static readonly AsyncLocal<IClock> _current = new();

    /// <summary>Gets or sets the clock used by time-relative assertions (flow-local, so parallel tests stay isolated).</summary>
    /// <value>Falls back to <see cref="SystemClock.Instance"/> when no clock was injected for the current flow.</value>
    /// <exception cref="ArgumentNullException"><c>value</c> is <see langword="null"/>.</exception>
    public static IClock Current {
        get => _current.Value ?? SystemClock.Instance;
        set {
            if(value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            _current.Value = value;
        }
    }
}
