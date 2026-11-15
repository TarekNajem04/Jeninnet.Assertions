//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Configures how an <see cref="EventMonitor"/> attaches to its source. Grows gradually; start with one fluent option.</summary>
public class EventMonitorOptions {
    private readonly HashSet<string> _includedEvents = new(StringComparer.Ordinal);
    private readonly HashSet<string> _excludedEvents = new(StringComparer.Ordinal);

    /// <summary>Gets whether events whose subscription accessor throws are skipped instead of failing monitor creation.</summary>
    public bool IgnoreEventAccessorExceptions { get; private set; }

    /// <summary>Skips events whose subscription accessor throws instead of failing monitor creation.</summary>
    /// <returns>This options instance for chaining.</returns>
    public EventMonitorOptions IgnoringEventAccessorExceptions() {
        IgnoreEventAccessorExceptions = true;
        return this;
    }

    /// <summary>Restricts monitoring to the named events; events are filtered when the monitor subscribes, not when they are recorded.</summary>
    /// <param name="names">The event names to include (case-sensitive); an empty list keeps the default of all events.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="names"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="names"/> contains a <see langword="null"/> or empty name.</exception>
    public EventMonitorOptions IncludingEvents(params string[] names) {
        AddNames(_includedEvents, names, nameof(names));
        return this;
    }

    /// <summary>Excludes the named events from monitoring; events are filtered when the monitor subscribes, not when they are recorded.</summary>
    /// <param name="names">The event names to exclude (case-sensitive).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="names"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="names"/> contains a <see langword="null"/> or empty name.</exception>
    public EventMonitorOptions ExcludingEvents(params string[] names) {
        AddNames(_excludedEvents, names, nameof(names));
        return this;
    }

    /// <summary>Determines whether an event should be subscribed to under the current filters.</summary>
    /// <param name="event">The event metadata.</param>
    /// <returns><see langword="true"/> when the event passes the include/exclude filters.</returns>
    internal bool ShouldMonitor(EventInfo @event) =>
        (_includedEvents.Count == 0 || _includedEvents.Contains(@event.Name))
        && !_excludedEvents.Contains(@event.Name);

    private static void AddNames(HashSet<string> destination, string[] names, string parameterName) {
        if(names is null) {
            throw new ArgumentNullException(parameterName);
        }

        foreach(var name in names) {
            if(string.IsNullOrEmpty(name)) {
                throw new ArgumentException("Event names must not be null or empty.", parameterName);
            }

            _ = destination.Add(name);
        }
    }
}
