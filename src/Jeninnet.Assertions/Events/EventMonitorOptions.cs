//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Configures how an <see cref="EventMonitor"/> attaches to its source. Grows gradually; start with one fluent option.</summary>
public class EventMonitorOptions {
    /// <summary>Gets whether events whose subscription accessor throws are skipped instead of failing monitor creation.</summary>
    public bool IgnoreEventAccessorExceptions { get; private set; }

    /// <summary>Skips events whose subscription accessor throws instead of failing monitor creation.</summary>
    /// <returns>This options instance for chaining.</returns>
    public EventMonitorOptions IgnoringEventAccessorExceptions() {
        IgnoreEventAccessorExceptions = true;
        return this;
    }
}
