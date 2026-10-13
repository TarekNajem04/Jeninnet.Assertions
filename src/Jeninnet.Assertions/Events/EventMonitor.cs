//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>
/// Records every .NET event raised by a source object. Create via <c>source.Monitor()</c> and dispose
/// to detach the handlers. The first handler argument is stored as <see cref="OccurredEvent.Sender"/>.
/// </summary>
public sealed class EventMonitor : IDisposable, IMonitor<OccurredEvent> {
    private readonly object _source;
    private readonly List<(EventInfo Event, Delegate Handler)> _handlers = [];
    private readonly List<OccurredEvent> _events = [];
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>Creates a monitor that attaches to every public instance event of <paramref name="source"/>.</summary>
    /// <param name="source">The event source to observe.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public EventMonitor(object source)
        : this(source, null) {
    }

    /// <summary>Creates a monitor that attaches to every public instance event of <paramref name="source"/>.</summary>
    /// <param name="source">The event source to observe.</param>
    /// <param name="options">The attach behavior (a <see langword="null"/> value selects the defaults).</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public EventMonitor(object source, EventMonitorOptions? options) {
        if(source is null) {
            throw new ArgumentNullException(nameof(source));
        }

        _source = source;

        foreach(var @event in source.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance)) {
            Delegate handler;
            try {
                handler = BuildHandler(@event);
                @event.AddEventHandler(source, handler);
            }
            catch when(options?.IgnoreEventAccessorExceptions == true) {
                continue;
            }

            _handlers.Add((@event, handler));
        }
    }

    /// <summary>Gets the observed event source.</summary>
    public object Subject => _source;

    /// <summary>Gets a snapshot of the events observed so far.</summary>
    public IReadOnlyList<OccurredEvent> OccurredEvents {
        get {
            lock(_lock) {
                return [.. _events];
            }
        }
    }

    /// <summary>Gets a snapshot of the events observed so far.</summary>
    public IReadOnlyList<OccurredEvent> MonitoredEvents => OccurredEvents;

    /// <summary>Gets a snapshot of the recordings for the event named <paramref name="eventName"/> (case-sensitive).</summary>
    /// <param name="eventName">The event name to look up.</param>
    /// <returns>The matching recordings, in observation order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="eventName"/> is <see langword="null"/>.</exception>
    public IReadOnlyList<OccurredEvent> GetRecordingFor(string eventName) {
        if(eventName is null) {
            throw new ArgumentNullException(nameof(eventName));
        }

        lock(_lock) {
            return _events.FindAll(occurred => string.Equals(occurred.EventName, eventName, StringComparison.Ordinal));
        }
    }

    /// <summary>Discards every recording observed so far.</summary>
    public void Clear() {
        lock(_lock) {
            _events.Clear();
        }
    }

    /// <summary>Detaches all handlers from the source.</summary>
    public void Dispose() {
        if(_disposed) {
            return;
        }

        _disposed = true;
        foreach(var (eventInfo, handler) in _handlers) {
            try {
                eventInfo.RemoveEventHandler(_source, handler);
            }
            catch {
                // Best effort: the source may already be collected or torn down.
            }
        }
    }

    /// <summary>Records one raised event, splitting the sender from the payload arguments.</summary>
    /// <param name="eventName">The event name.</param>
    /// <param name="args">The handler arguments (sender first, when present).</param>
    private void Record(string eventName, object?[] args) {
        var sender = args.Length > 0 ? args[0] : null;
        object?[] payload = args.Length > 1 ? [.. args.Skip(1)] : [];
        lock(_lock) {
            _events.Add(new OccurredEvent(eventName, sender, payload, DateTime.UtcNow));
        }
    }

    /// <summary>Builds a compiled delegate that records every raised event of one event into this monitor.</summary>
    /// <param name="event">The event metadata to attach to.</param>
    /// <returns>A handler matching the event's delegate type that forwards sender and args to <see cref="Record"/>.</returns>
    private Delegate BuildHandler(EventInfo @event) {
        // The operators assert non-null reflection results; netstandard2.0 reference assemblies are
        // unannotated, so the operators stay valid there while modern targets ignore them.
        var handlerType = @event.EventHandlerType!;
        var parameters = handlerType.GetMethod("Invoke")!.GetParameters()
            .Select(static parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
            .ToArray();
        Action<string, object?[]> record = Record;
        var recordCall = Expression.Call(
            Expression.Constant(this),
            record.Method,
            Expression.Constant(@event.Name),
            Expression.NewArrayInit(typeof(object), parameters.Select(static parameter => Expression.Convert(parameter, typeof(object)))));
        return Expression.Lambda(handlerType, recordCall, parameters).Compile();
    }
}
