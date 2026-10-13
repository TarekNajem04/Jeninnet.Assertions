//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>The default <see cref="IClock"/>, reading the machine clock; used when no clock is injected.</summary>
public sealed class SystemClock : IClock {
    /// <summary>Gets the shared instance backed by the system clock.</summary>
    public static IClock Instance { get; } = new SystemClock();

    /// <summary>Gets the current local time from <see cref="DateTimeOffset.Now"/>.</summary>
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <summary>Gets the current UTC time from <see cref="DateTimeOffset.UtcNow"/>.</summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
