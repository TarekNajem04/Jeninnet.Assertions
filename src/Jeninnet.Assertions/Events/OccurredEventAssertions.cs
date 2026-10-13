//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Events;

/// <summary>Provides assertion methods for verifying a single recorded event occurrence.</summary>
/// <param name="parent">The monitor assertions to continue from through <see cref="And"/>.</param>
/// <param name="occurrence">The recorded occurrence under assertion.</param>
/// <param name="expression">The caller expression naming the occurrence under test.</param>
public class OccurredEventAssertions(EventMonitorAssertions parent, OccurredEvent occurrence, string? expression = null) : AssertionsBase(expression, "event") {
    private readonly EventMonitorAssertions _parent = parent ?? throw new ArgumentNullException(nameof(parent));
    private readonly OccurredEvent _occurrence = occurrence ?? throw new ArgumentNullException(nameof(occurrence));

    /// <summary>Gets the recorded occurrence under assertion.</summary>
    public OccurredEvent Which => _occurrence;

    /// <summary>Gets the monitor assertions to continue asserting on.</summary>
    public EventMonitorAssertions And => _parent;

    /// <summary>Asserts that the occurrence was raised by <paramref name="expectedSender"/>.</summary>
    /// <param name="expectedSender">The expected sender (compared with <see cref="object.Equals(object?, object?)"/>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The sender differs from <paramref name="expectedSender"/>.</exception>
    public AndConstraint<OccurredEventAssertions> WithSender(object? expectedSender, string? because = null, params object?[] becauseArgs) {
        if(!Equals(_occurrence.Sender, expectedSender)) {
            Fail(Strings.Format(StringKeys.EVENT_WITH_SENDER, Expression, Format(expectedSender), Format(_occurrence.Sender)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the single event argument satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the argument must satisfy. Exceptions thrown by the predicate propagate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <typeparam name="TEventArgs">The expected argument type.</typeparam>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The occurrence has no single argument of type <typeparamref name="TEventArgs"/>, or the predicate fails.</exception>
    public AndConstraint<OccurredEventAssertions> WithArgs<TEventArgs>(Expression<Func<TEventArgs, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_occurrence.Args.Length != 1 || _occurrence.Args[0] is not TEventArgs argument || !predicate.Compile().Invoke(argument)) {
            Fail(Strings.Format(StringKeys.EVENT_WITH_ARGS, Expression, 0, Format(_occurrence.Args.Length == 1 ? _occurrence.Args[0] : null)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every event argument satisfies its predicate, in order.</summary>
    /// <remarks><c>params</c> must be last in C#, so this overload carries no <c>because</c> clause.</remarks>
    /// <param name="predicates">One predicate per argument, in order. Exceptions thrown by a predicate propagate.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicates"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The predicate count differs from the argument count, or a predicate fails.</exception>
    public AndConstraint<OccurredEventAssertions> WithArgs(params Expression<Func<object?, bool>>[] predicates) {
        if(predicates is null) {
            throw new ArgumentNullException(nameof(predicates));
        }

        if(predicates.Length != _occurrence.Args.Length) {
            Fail(Strings.Format(StringKeys.EVENT_WITH_ARGS_COUNT, Expression, AssertionScope.FormatInvariant(predicates.Length), AssertionScope.FormatInvariant(_occurrence.Args.Length)));
            return new(this);
        }

        for(var index = 0; index < predicates.Length; index++) {
            if(!predicates[index].Compile().Invoke(_occurrence.Args[index])) {
                Fail(Strings.Format(StringKeys.EVENT_WITH_ARGS, Expression, AssertionScope.FormatInvariant(index), Format(_occurrence.Args[index])));
                return new(this);
            }
        }

        return new(this);
    }

    private static string Format(object? value) => value is null ? "<null>" : AssertionScope.FormatInvariant(value);
}
