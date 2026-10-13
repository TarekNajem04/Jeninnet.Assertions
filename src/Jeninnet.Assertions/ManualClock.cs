//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>A ready-made deterministic <see cref="IClock"/> for tests: starts at a fixed instant and moves only when told to.</summary>
/// <remarks>
/// Assign it to <see cref="AssertionClock.Current"/> and drive the subject under test, calling <see cref="Advance"/> or
/// <see cref="Set"/> at the same points the production code would experience time passing. Reads and writes are
/// thread-safe, and the injection point stays isolated per async flow like every other ambient setting. The local
/// offset is fixed at construction: <see cref="Now"/> reports the stored instant at that offset while
/// <see cref="UtcNow"/> always reports the raw instant at <see cref="TimeSpan.Zero"/>.
/// </remarks>
/// <param name="utcNow">The instant the clock starts at.</param>
/// <param name="localOffset">
/// The offset used when reporting <see cref="Now"/>, or <see langword="null"/> to reuse <paramref name="utcNow"/>'s offset.
/// </param>
public sealed class ManualClock(DateTimeOffset utcNow, TimeSpan? localOffset = null) : IClock {
    private readonly object _sync = new();
    private readonly TimeSpan _localOffset = localOffset ?? utcNow.Offset;
    private DateTimeOffset _utcNow = utcNow;

    /// <summary>Gets the current local time: the stored instant reported at the configured offset.</summary>
    public DateTimeOffset Now {
        get {
            lock(_sync) {
                return _utcNow.ToOffset(_localOffset);
            }
        }
    }

    /// <summary>Gets the current UTC time: the stored instant at <see cref="TimeSpan.Zero"/>.</summary>
    public DateTimeOffset UtcNow {
        get {
            lock(_sync) {
                return _utcNow.ToUniversalTime();
            }
        }
    }

    /// <summary>Moves the clock by <paramref name="delta"/> (a negative value moves it backwards).</summary>
    /// <param name="delta">The amount of time to move the clock by.</param>
    public void Advance(TimeSpan delta) {
        lock(_sync) {
            _utcNow += delta;
        }
    }

    /// <summary>Moves the clock to <paramref name="utcNow"/>, keeping the configured local offset.</summary>
    /// <param name="utcNow">The new instant.</param>
    public void Set(DateTimeOffset utcNow) {
        lock(_sync) {
            _utcNow = utcNow;
        }
    }
}
