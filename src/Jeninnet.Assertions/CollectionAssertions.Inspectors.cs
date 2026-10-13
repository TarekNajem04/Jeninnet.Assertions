//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>Asserts that every item in the collection appears exactly once.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or contains a duplicate item.</exception>
    public AndConstraint<CollectionAssertions<T>> OnlyHaveUniqueItems(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ONLY_UNIQUE_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var duplicate = _value.GroupBy(static x => x).FirstOrDefault(group => group.Count() > 1);
        if(duplicate is not null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ONLY_UNIQUE, Expression, Format(duplicate.Key)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the projected keys every item maps to via <paramref name="selector"/> are unique.</summary>
    /// <typeparam name="TKey">The projected key type.</typeparam>
    /// <param name="selector">The projection selecting the key of each item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or two items share a key.</exception>
    public AndConstraint<CollectionAssertions<T>> OnlyHaveUniqueItems<TKey>(Expression<Func<T, TKey>> selector, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ONLY_UNIQUE_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var select = selector.Compile();
        var duplicate = _value.GroupBy(select).FirstOrDefault(group => group.Count() > 1);
        if(duplicate is not null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ONLY_UNIQUE, Expression, Format(duplicate.Key)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that no item in the collection is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or contains a null item.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainNulls(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_NULLS_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var nullIndex = _value.ToList().FindIndex(item => item is null);
        if(nullIndex >= 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_NULLS_FOUND, Expression, Format(nullIndex)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that no projected key (via <paramref name="selector"/>) is <see langword="null"/>.</summary>
    /// <typeparam name="TKey">The projected key type.</typeparam>
    /// <param name="selector">The projection selecting the key of each item.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or a projected key is null.</exception>
    public AndConstraint<CollectionAssertions<T>> NotContainNulls<TKey>(Expression<Func<T, TKey>> selector, string? because = null, params object?[] becauseArgs) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_NULLS_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var select = selector.Compile();
        var nullIndex = _value.ToList().FindIndex(item => select(item) is null);
        if(nullIndex >= 0) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_CONTAIN_NULLS_FOUND, Expression, Format(nullIndex)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that each element satisfies its corresponding inspector, in order.</summary>
    /// <remarks>Inspectors run against the first <c>N</c> elements where <c>N</c> is the inspector count,
    /// which must not exceed the element count. Each inspector carries its own <c>because</c> clauses;
    /// this method takes none because <c>params</c> must be the last parameter.</remarks>
    /// <param name="inspectors">One inspector per element, applied in order to the first elements.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="inspectors"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="inspectors"/> is empty, holds more inspectors than elements, or holds a <see langword="null"/> inspector.</exception>
    /// <exception cref="AssertionFailedException">Any inspected element fails its inspector.</exception>
    public AndConstraint<CollectionAssertions<T>> SatisfyRespectively(params Action<T>[] inspectors) {
        if(inspectors is null) {
            throw new ArgumentNullException(nameof(inspectors));
        }

        if(inspectors.Length == 0) {
            throw new ArgumentException("At least one inspector is required.", nameof(inspectors));
        }

        for(var index = 0; index < inspectors.Length; index++) {
            if(inspectors[index] is null) {
                throw new ArgumentException($"Inspector at index {AssertionScope.FormatInvariant(index)} is null.", nameof(inspectors));
            }
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_SATISFY_RESPECTIVELY_NULL, Expression, AssertionScope.FormatInvariant(inspectors.Length)));
            return new(this);
        }

        var list = _value.ToList();
        if(inspectors.Length > list.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_SATISFY_RESPECTIVELY_COUNT, Expression, AssertionScope.FormatInvariant(inspectors.Length), AssertionScope.FormatInvariant(list.Count)));
            return new(this);
        }

        var failures = new List<string>();
        for(var index = 0; index < inspectors.Length; index++) {
            var messages = CollectInspectorFailures(inspectors[index], list[index]);
            for(var messageIndex = 0; messageIndex < messages.Length; messageIndex++) {
                failures.Add($"{AssertionScope.FormatInvariant(index + 1)}: {messages[messageIndex]}");
            }
        }

        if(failures.Count != 0) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, Strings.Get(StringKeys.COLLECTION_SATISFY_RESPECTIVELY_FAILURES), Expression, AssertionScope.FormatInvariant(inspectors.Length), AssertionScope.FormatInvariant(failures.Count));
            foreach(var failure in failures) {
                builder.Append('\n').Append(failure);
            }

            Fail(builder.ToString());
        }

        return new(this);
    }

    /// <summary>Asserts that every element satisfies <paramref name="action"/>.</summary>
    /// <param name="action">The check applied to every element. An empty collection passes vacuously.</param>
    /// <param name="because">The reason for the assertion, used as report context.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Any element fails <paramref name="action"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> AllSatisfy(Action<T> action, string? because = null, params object?[] becauseArgs) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ALL_SATISFY_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var list = _value.ToList();
        var failures = new List<string>();
        for(var index = 0; index < list.Count; index++) {
            var messages = CollectInspectorFailures(action, list[index]);
            for(var messageIndex = 0; messageIndex < messages.Length; messageIndex++) {
                failures.Add($"{AssertionScope.FormatInvariant(index + 1)}: {messages[messageIndex]}");
            }
        }

        if(failures.Count != 0) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, Strings.Get(StringKeys.COLLECTION_ALL_SATISFY_FAILURES), Expression, AssertionScope.FormatInvariant(list.Count), AssertionScope.FormatInvariant(failures.Count));
            foreach(var failure in failures) {
                builder.Append('\n').Append(failure);
            }

            Fail(builder.ToString(), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that every predicate matches at least one element, in any order.</summary>
    /// <remarks>Each predicate carries no <c>because</c> of its own; this method takes none because
    /// <c>params</c> must be the last parameter. A predicate matching several elements still counts once.</remarks>
    /// <param name="predicates">The predicates, each of which must match at least one element.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicates"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="predicates"/> is empty or holds a <see langword="null"/> predicate.</exception>
    /// <exception cref="AssertionFailedException">Any predicate matches no element.</exception>
    public AndConstraint<CollectionAssertions<T>> Satisfy(params Expression<Func<T, bool>>[] predicates) {
        if(predicates is null) {
            throw new ArgumentNullException(nameof(predicates));
        }

        if(predicates.Length == 0) {
            throw new ArgumentException("At least one predicate is required.", nameof(predicates));
        }

        for(var index = 0; index < predicates.Length; index++) {
            if(predicates[index] is null) {
                throw new ArgumentException($"Predicate at index {AssertionScope.FormatInvariant(index)} is null.", nameof(predicates));
            }
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_SATISFY_NULL, Expression, AssertionScope.FormatInvariant(predicates.Length)));
            return new(this);
        }

        var list = _value.ToList();
        var unmatched = new List<string>();
        for(var index = 0; index < predicates.Length; index++) {
            var compiled = predicates[index].Compile();
            if(!list.Any(compiled)) {
                unmatched.Add($"{AssertionScope.FormatInvariant(index + 1)}: {predicates[index]}");
            }
        }

        if(unmatched.Count != 0) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, Strings.Get(StringKeys.COLLECTION_SATISFY_UNMATCHED), Expression, AssertionScope.FormatInvariant(predicates.Length), AssertionScope.FormatInvariant(unmatched.Count));
            foreach(var entry in unmatched) {
                builder.Append('\n').Append(entry);
            }

            Fail(builder.ToString());
        }

        return new(this);
    }

    /// <summary>Asserts that every element is of type <typeparamref name="TExpectation"/> and exposes the first one for further assertions.</summary>`r`n    /// <typeparam name="TExpectation">The type every element must have.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element (default on an empty collection, which passes vacuously).</returns>
    /// <exception cref="AssertionFailedException">Any element is not of type <typeparamref name="TExpectation"/>.</exception>
    public WhichConstraint<TExpectation> AllBeOfType<TExpectation>(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_OF_TYPE_NULL, Expression, typeof(TExpectation).Name), because, becauseArgs);
            return new WhichConstraint<TExpectation>(default!);
        }

        var index = 0;
        TExpectation? first = default;
        var found = false;
        foreach(var item in _value) {
            if(item is TExpectation match) {
                if(!found) {
                    first = match;
                    found = true;
                }
            } else {
                Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_OF_TYPE, Expression, typeof(TExpectation).Name, Format(item), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                return new WhichConstraint<TExpectation>(default!);
            }

            index++;
        }

        return new WhichConstraint<TExpectation>(first!);
    }

    /// <summary>Asserts that every element is assignable to type <typeparamref name="TExpectation"/> and exposes the first one for further assertions.</summary>`r`n    /// <typeparam name="TExpectation">The type every element must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching element (default on an empty collection, which passes vacuously).</returns>
    /// <exception cref="AssertionFailedException">Any element is not assignable to <typeparamref name="TExpectation"/>.</exception>
    public WhichConstraint<TExpectation> AllBeAssignableTo<TExpectation>(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_ASSIGNABLE_TO_NULL, Expression, typeof(TExpectation).Name), because, becauseArgs);
            return new WhichConstraint<TExpectation>(default!);
        }

        var index = 0;
        TExpectation? first = default;
        var found = false;
        foreach(var item in _value) {
            if(item is TExpectation match) {
                if(!found) {
                    first = match;
                    found = true;
                }
            } else {
                Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_ASSIGNABLE_TO, Expression, typeof(TExpectation).Name, Format(item), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                return new WhichConstraint<TExpectation>(default!);
            }

            index++;
        }

        return new WhichConstraint<TExpectation>(first!);
    }

    /// <summary>Runs an inspector against one item and collects every failure message it produced.</summary>
    /// <param name="inspector">The inspector to run.</param>
    /// <param name="item">The item to inspect.</param>
    /// <returns>The collected failure messages (direct throws plus scope-collected failures).</returns>
    private static string[] CollectInspectorFailures(Action<T> inspector, T item) {
        var messages = new List<string>();
        using(var inner = new AssertionScope()) {
            try {
                inspector(item);
            }
            catch(AssertionFailedException ex) {
                messages.Add(ex.Message);
            }

            messages.AddRange(inner.Discard());
        }

        return [.. messages];
    }
}
