//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Supplies the current instant for time-relative assertions, so a test can inject a deterministic clock.</summary>
/// <remarks>Time-relative verbs resolve the active clock through <see cref="AssertionClock.Current"/>, which defaults
/// to <see cref="SystemClock.Instance"/>. Implementations should return a stable instant per reading, letting tests
/// cross window boundaries without wall-clock sleeps.</remarks>
public interface IClock {
    /// <summary>Gets the current local time.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Gets the current UTC time.</summary>
    DateTimeOffset UtcNow { get; }
}
