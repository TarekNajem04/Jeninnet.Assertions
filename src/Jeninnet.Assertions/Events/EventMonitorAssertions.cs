//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Provides assertion methods for verifying events recorded by an <see cref="EventMonitor"/>.</summary>
/// <param name="monitor">The monitor to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
/// <exception cref="ArgumentNullException"><paramref name="monitor"/> is <see langword="null"/>.</exception>
public class EventMonitorAssertions(EventMonitor monitor, string? expression = null) : AssertionsBase(expression, "monitor") {
    private readonly EventMonitor _monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));

    /// <summary>Asserts that <paramref name="eventName"/> was raised and exposes the first occurrence for further assertions.</summary>
    /// <param name="eventName">The event name (case-sensitive).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="OccurredEventAssertions"/> wrapping the first matching occurrence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="eventName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The event was never raised.</exception>
    public OccurredEventAssertions Raise(string eventName, string? because = null, params object?[] becauseArgs) {
        if(eventName is null) {
            throw new ArgumentNullException(nameof(eventName));
        }

        var occurrence = _monitor.OccurredEvents.FirstOrDefault(e => string.Equals(e.EventName, eventName, StringComparison.Ordinal))
                         ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.EVENT_RAISE, Expression, eventName), because, becauseArgs));
        return new OccurredEventAssertions(this, occurrence, $"event '{eventName}'");
    }

    /// <summary>Asserts that <paramref name="eventName"/> was never raised.</summary>
    /// <param name="eventName">The event name (case-sensitive).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="eventName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The event was raised.</exception>
    public AndConstraint<EventMonitorAssertions> NotRaise(string eventName, string? because = null, params object?[] becauseArgs) {
        if(eventName is null) {
            throw new ArgumentNullException(nameof(eventName));
        }

        var count = _monitor.OccurredEvents.Count(e => string.Equals(e.EventName, eventName, StringComparison.Ordinal));
        if(count != 0) {
            Fail(Strings.Format(StringKeys.EVENT_NOT_RAISE, Expression, eventName, AssertionScope.FormatInvariant(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the <c>PropertyChanged</c> event was raised for <paramref name="propertyName"/> and exposes the first occurrence.</summary>
    /// <remarks>The match is a literal, ordinal comparison against <see cref="PropertyChangedEventArgs.PropertyName"/>.</remarks>
    /// <param name="propertyName">The property name to look for (case-sensitive).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="OccurredEventAssertions"/> wrapping the first matching occurrence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such property change was recorded.</exception>
    public OccurredEventAssertions RaisePropertyChangeFor(string propertyName, string? because = null, params object?[] becauseArgs) {
        if(propertyName is null) {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var matches = FindPropertyChanges(propertyName);
        var occurrence = matches.FirstOrDefault();
        if(occurrence is null) {
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.EVENT_PROPERTY_CHANGE, Expression, propertyName), because, becauseArgs));
        }

        return new OccurredEventAssertions(this, occurrence, "event 'PropertyChanged'");
    }

    /// <summary>Asserts that the <c>PropertyChanged</c> event was never raised for <paramref name="propertyName"/>.</summary>
    /// <remarks>The match is a literal, ordinal comparison against <see cref="PropertyChangedEventArgs.PropertyName"/>.</remarks>
    /// <param name="propertyName">The property name that must be absent (case-sensitive).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such a property change was recorded.</exception>
    public AndConstraint<EventMonitorAssertions> NotRaisePropertyChangeFor(string propertyName, string? because = null, params object?[] becauseArgs) {
        if(propertyName is null) {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var count = FindPropertyChanges(propertyName).Count;
        if(count != 0) {
            Fail(Strings.Format(StringKeys.EVENT_NOT_PROPERTY_CHANGE, Expression, propertyName, AssertionScope.FormatInvariant(count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Finds recorded <c>PropertyChanged</c> occurrences for one property name (ordinal match).</summary>
    /// <param name="propertyName">The property name to match.</param>
    /// <returns>The matching recorded events, in occurrence order.</returns>
    private List<OccurredEvent> FindPropertyChanges(string propertyName) {
        var matches = new List<OccurredEvent>();
        foreach(var occurrence in _monitor.OccurredEvents) {
            if(string.Equals(occurrence.EventName, "PropertyChanged", StringComparison.Ordinal)
                && occurrence.Args.Length == 1
                && occurrence.Args[0] is PropertyChangedEventArgs changed
                && string.Equals(changed.PropertyName, propertyName, StringComparison.Ordinal)) {
                matches.Add(occurrence);
            }
        }

        return matches;
    }
}
