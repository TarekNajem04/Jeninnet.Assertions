//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Exposes the recordings of an event monitor over records of type <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type of the recorded event entries.</typeparam>
public interface IMonitor<out T> {
    /// <summary>Gets the observed event source.</summary>
    object Subject { get; }

    /// <summary>Gets a snapshot of every recorded event.</summary>
    IReadOnlyList<T> MonitoredEvents { get; }

    /// <summary>Gets a snapshot of the recordings for the event named <paramref name="eventName"/> (case-sensitive).</summary>
    /// <param name="eventName">The event name to look up.</param>
    /// <returns>The matching recordings, in observation order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="eventName"/> is <see langword="null"/>.</exception>
    IReadOnlyList<T> GetRecordingFor(string eventName);

    /// <summary>Discards every recording observed so far.</summary>
    void Clear();
}
