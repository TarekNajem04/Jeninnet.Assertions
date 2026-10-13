//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Holds the evaluation state shared by every selector of one <c>Given(...)</c> chain.</summary>
internal sealed class GivenChainState {
    /// <summary>Gets or sets whether the steps before the current one succeeded.</summary>
    internal bool PreviousSucceeded = true;

    /// <summary>Gets or sets the outcome of the latest <see cref="GivenSelector{T}.ForCondition"/> (<see langword="null"/> until a condition runs).</summary>
    internal bool? Succeeded;
}

/// <summary>
/// A chaining object returned from <see cref="AssertionScope.Given{T}"/> that continues an assertion using a
/// value selected from the subject.
/// </summary>
/// <typeparam name="T">The type of the selected value.</typeparam>
public class GivenSelector<T> {
    private readonly AssertionScope _scope;
    private readonly GivenChainState _state;

    internal GivenSelector(Func<T> selector, AssertionScope scope, GivenChainState state) {
        _scope = scope;
        _state = state;
        Selected = state.PreviousSucceeded ? selector() : default!;
    }

    /// <summary>Gets the value selected by the latest selector in the chain.</summary>
    internal T Selected { get; }

    /// <summary>Gets whether every step so far succeeded and no failure has been recorded.</summary>
    public bool Succeeded => _state.PreviousSucceeded && (_state.Succeeded is null or true);

    /// <summary>Specifies the condition the selected value must satisfy.</summary>
    /// <param name="predicate">The condition; when <see langword="false"/> the chain fails once a <see cref="FailWith(string, object?[])"/> runs.</param>
    /// <returns>This selector for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public GivenSelector<T> ForCondition(Func<T, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_state.PreviousSucceeded) {
            _state.Succeeded = predicate(Selected);
        }

        return this;
    }

    /// <summary>Continues the chain with a value derived from the selected value.</summary>
    /// <typeparam name="TOut">The type of the derived value.</typeparam>
    /// <param name="selector">The projection from the current value.</param>
    /// <returns>A selector over the derived value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public GivenSelector<TOut> Given<TOut>(Func<T, TOut> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        return new GivenSelector<TOut>(() => selector(Selected), _scope, _state);
    }

    /// <summary>Reports the assertion failure with a format message.</summary>
    /// <param name="message">The failure message; may contain <c>{0}</c> etc. placeholders for the non-empty arg overloads.</param>
    /// <returns>The continuation of the chain.</returns>
    public ContinuationOfGiven<T> FailWith(string message) => FailWith(message, Array.Empty<object?>());

    /// <summary>Reports the assertion failure with a format message and value-typed arguments.</summary>
    /// <param name="message">The failure message; may contain <c>{0}</c> etc. placeholders.</param>
    /// <param name="args">Arguments formatted against <paramref name="message"/>.</param>
    /// <returns>The continuation of the chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
    public ContinuationOfGiven<T> FailWith(string message, params object?[] args) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        if(args is null) {
            throw new ArgumentNullException(nameof(args));
        }

        RecordFailure(message, args.Length == 0 ? [] : args);
        return new ContinuationOfGiven<T>(this);
    }

    /// <summary>Reports the assertion failure with a format message whose arguments derive from the selected value.</summary>
    /// <param name="message">The failure message; may contain <c>{0}</c> etc. placeholders.</param>
    /// <param name="args">Argument providers evaluated against the selected value only when the chain can still fail.</param>
    /// <returns>The continuation of the chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
    public ContinuationOfGiven<T> FailWith(string message, params Func<T, object?>[] args) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        if(args is null) {
            throw new ArgumentNullException(nameof(args));
        }

        if(!_state.PreviousSucceeded) {
            _state.Succeeded = null;
        } else {
            _state.PreviousSucceeded = _state.Succeeded == true;
            if(_state.Succeeded != true) {
                var mapped = new object?[args.Length];
                for(var index = 0; index < args.Length; index++) {
                    mapped[index] = args[index] == null ? null : args[index](Selected);
                }

                _scope.AddPreFormattedFailure(Format(message, mapped));
            }

            _state.Succeeded = null;
        }

        return new ContinuationOfGiven<T>(this);
    }

    /// <summary>Records a failure for the current step, honoring the short-circuit state of the given chain.</summary>
    /// <param name="message">The message template to record.</param>
    /// <param name="args">The template arguments.</param>
    private void RecordFailure(string message, object?[] args) {
        if(!_state.PreviousSucceeded) {
            _state.Succeeded = null;
            return;
        }

        _state.PreviousSucceeded = _state.Succeeded == true;
        if(_state.Succeeded != true) {
            _scope.AddPreFormattedFailure(Format(message, args));
        }

        _state.Succeeded = null;
    }

    /// <summary>Formats a message template and capitalizes its first letter for report consistency.</summary>
    /// <param name="message">The template (or literal message).</param>
    /// <param name="args">The template arguments, formatted invariantly.</param>
    /// <returns>The formatted, capitalized message.</returns>
    private static string Format(string message, object?[] args) {
        var formatted = args.Length == 0 ? message : string.Format(CultureInfo.InvariantCulture, message, args);
        var trimmed = formatted.Trim();
        return trimmed.Length == 0 ? formatted : char.ToUpperInvariant(trimmed[0]) + trimmed.Substring(1);
    }
}

/// <summary>Enables chaining further assertions from an <see cref="AssertionScope.Given{T}"/> call.</summary>
/// <typeparam name="T">The type of the value selected by the given chain.</typeparam>
public class ContinuationOfGiven<T> {
    internal ContinuationOfGiven(GivenSelector<T> parent) => Then = parent;

    /// <summary>Continues the chain if the previous steps succeeded.</summary>
    public GivenSelector<T> Then { get; }

    /// <summary>Gets whether every step so far succeeded and no failure has been recorded.</summary>
    public bool Succeeded => Then.Succeeded;
}
