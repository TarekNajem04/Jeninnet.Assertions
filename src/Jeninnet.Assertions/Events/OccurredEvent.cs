//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Records a single raised .NET event observed by an <see cref="EventMonitor"/>.</summary>
/// <param name="eventName">The name of the raised event.</param>
/// <param name="sender">The first handler argument (conventionally the sender).</param>
/// <param name="args">The remaining handler arguments.</param>
/// <param name="timestamp">The UTC time the event was observed.</param>
public sealed class OccurredEvent(string eventName, object? sender, object?[] args, DateTime timestamp) {
    /// <summary>Gets the name of the raised event.</summary>
    public string EventName { get; } = eventName;

    /// <summary>Gets the first handler argument (conventionally the sender).</summary>
    public object? Sender { get; } = sender;

    /// <summary>Gets the remaining handler arguments.</summary>
    public object?[] Args { get; } = args;

    /// <summary>Gets the UTC time the event was observed.</summary>
    public DateTime Timestamp { get; } = timestamp;
}
