//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Controls the pause between retries for the polling assertions <c>SucceedWithin</c> and <c>EventuallySatisfy</c>.</summary>
/// <remarks>
/// A <see cref="Constant"/> backoff always pauses the same amount; an <see cref="Exponential"/> backoff starts
/// fast and slows down (curved backoff), which keeps long waits from hammering the subject while still reacting
/// quickly to fast transitions. Pass an instance to the polling overloads that accept a <see cref="PollingBackoff"/>.
/// </remarks>
public sealed class PollingBackoff {
    private const double DEFAULT_FACTOR = 2.0;

    private readonly bool _curved;

    /// <summary>Creates a backoff (construct via <see cref="Constant"/> or <see cref="Exponential"/>).</summary>
    /// <param name="initialInterval">The pause before the first retry.</param>
    /// <param name="factor">The multiplier applied after every attempt.</param>
    /// <param name="maxInterval">The upper bound the pause never exceeds.</param>
    private PollingBackoff(TimeSpan initialInterval, double factor, TimeSpan maxInterval) {
        InitialInterval = initialInterval;
        Factor = factor;
        MaxInterval = maxInterval;
        _curved = factor > 1.0;
    }

    /// <summary>Gets the pause before the first retry.</summary>
    public TimeSpan InitialInterval { get; }

    /// <summary>Gets the multiplier applied to the pause after every attempt (always <c>1</c> for a constant backoff).</summary>
    public double Factor { get; }

    /// <summary>Gets the upper bound the pause never exceeds.</summary>
    public TimeSpan MaxInterval { get; }

    /// <summary>Creates a backoff that pauses the same amount before every retry.</summary>
    /// <param name="interval">The fixed pause between attempts (must not be negative).</param>
    /// <returns>A constant backoff.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is negative.</exception>
    public static PollingBackoff Constant(TimeSpan interval) {
        if(interval < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(interval), "Poll interval must not be negative.");
        }

        return new PollingBackoff(interval, 1.0, interval);
    }

    /// <summary>Creates an exponential (curved) backoff that multiplies the pause after every attempt, up to a cap.</summary>
    /// <param name="initialInterval">The pause before the first retry (must not be negative).</param>
    /// <param name="factor">The multiplier applied after every attempt (must be <c>1</c> or greater; defaults to <c>2</c>).</param>
    /// <param name="maxInterval">The cap the pause never exceeds (defaults to no cap); when set it must be at least <paramref name="initialInterval"/>.</param>
    /// <returns>An exponential backoff.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialInterval"/> is negative, <paramref name="factor"/> is less than <c>1</c>, or <paramref name="maxInterval"/> is smaller than <paramref name="initialInterval"/>.</exception>
    public static PollingBackoff Exponential(TimeSpan initialInterval, double factor = DEFAULT_FACTOR, TimeSpan? maxInterval = null) {
        if(initialInterval < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(initialInterval), "Poll interval must not be negative.");
        }

        if(factor < 1.0) {
            throw new ArgumentOutOfRangeException(nameof(factor), "Backoff factor must be at least 1.");
        }

        var maximum = maxInterval ?? TimeSpan.MaxValue;
        if(maximum < initialInterval) {
            throw new ArgumentOutOfRangeException(nameof(maxInterval), "Maximum poll interval must not be smaller than the initial interval.");
        }

        return new PollingBackoff(initialInterval, factor, maximum);
    }

    /// <summary>Gets the pause before the retry that follows the given zero-based attempt number.</summary>
    /// <param name="attempt">The zero-based index of the attempt just completed.</param>
    /// <returns>The pause to take before the next attempt; never greater than <see cref="MaxInterval"/>.</returns>
    public TimeSpan DelayFor(int attempt) {
        if(attempt <= 0 || !_curved) {
            return InitialInterval;
        }

        var scaled = InitialInterval.Ticks * Math.Pow(Factor, attempt);
        return scaled >= MaxInterval.Ticks ? MaxInterval : TimeSpan.FromTicks((long)scaled);
    }
}
