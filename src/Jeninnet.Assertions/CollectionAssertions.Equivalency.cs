//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class CollectionAssertions<T> {
    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/>, ignoring order.
    /// The comparison is a multiset equality: every item must appear with the same cardinality,
    /// so duplicates in <paramref name="expected"/> must be matched by distinct items in the actual collection.
    /// </summary>
    /// <param name="expected">The expected items that must match the collection, regardless of order.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the contents do not match.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEquivalentTo(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) =>
        BeEquivalentTo(expected, (Func<CollectionEquivalencyOptions<T>, CollectionEquivalencyOptions<T>>?)null, because, becauseArgs);

    /// <summary>
    /// Asserts that the collection contains exactly the items in <paramref name="expected"/>, with ordering,
    /// casing, and equality controlled by <paramref name="config"/>.
    /// </summary>
    /// <param name="expected">The expected items that must match the collection.</param>
    /// <param name="config">Optional equivalency configuration (strict ordering, casing, custom comparer).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the contents do not match.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEquivalentTo(
        IEnumerable<T> expected,
        Func<CollectionEquivalencyOptions<T>, CollectionEquivalencyOptions<T>>? config,
        string? because = null,
        params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_EQUIVALENT_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var options = config?.Invoke(new CollectionEquivalencyOptions<T>()) ?? new CollectionEquivalencyOptions<T>();
        var equality = options.ResolveEquality();
        var expectedList = expected.ToList();
        var actualList = _value.ToList();

        if(actualList.Count != expectedList.Count) {
            Fail(Strings.Format(StringKeys.COLLECTION_HAVE_EXACTLY, Expression, Format(expectedList.Count), Format(actualList.Count)), because, becauseArgs);
            return new(this);
        }

        if(options.StrictOrdering) {
            for(var index = 0; index < expectedList.Count; index++) {
                if(!equality.Equals(actualList[index], expectedList[index])) {
                    Fail(Strings.Format(StringKeys.COLLECTION_EQUIVALENT_ORDER, Expression, Format(index), Format(actualList[index]), Format(expectedList[index])), because, becauseArgs);
                    return new(this);
                }
            }

            return new(this);
        }

        var expectedCounts = CountByElement(expectedList, equality);
        var actualCounts = CountByElement(actualList, equality);

        foreach(var item in expectedList) {
            var expectedCount = expectedCounts[item!];
            actualCounts.TryGetValue(item!, out var actualCount);
            if(expectedCount != actualCount) {
                Fail(Strings.Format(StringKeys.COLLECTION_EQUIVALENT_CARDINALITY, Expression, Format(item), Format(actualCount), Format(expectedCount)), because, becauseArgs);
                return new(this);
            }
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the collection is structurally equivalent to <paramref name="expected"/> using whole-object
    /// graph options authored against the element type.
    /// </summary>
    /// <param name="expected">The expected items that must match the collection.</param>
    /// <param name="config">Element-typed equivalency configuration ending in <c>AsCollection()</c> (exclusions, ordering, by-value types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collection is <see langword="null"/> or the contents do not match.</exception>
    public AndConstraint<CollectionAssertions<T>> BeEquivalentTo(
        IEnumerable<T> expected,
        Func<SelfReferenceEquivalencyOptions<T>, SelfReferenceEquivalencyOptions<IEnumerable<T>>> config,
        string? because = null,
        params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_BE_EQUIVALENT_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(config is null) {
            throw new ArgumentNullException(nameof(config));
        }

        var options = config.Invoke(new SelfReferenceEquivalencyOptions<T>());
        var differences = EquivalencyEngine.FindDifferences<IEnumerable<T>>(_value, expected, options);
        if(differences.Count == 1) {
            Fail(AppendDiagnostics(Strings.Format(StringKeys.EQUIVALENCE_DETAIL, Expression, differences[0].TrimEnd('.')), options, _value, expected), because, becauseArgs);
        } else if(differences.Count > 1) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "Expected {0} to be equivalent, but found {1} difference(s):", Expression, AssertionScope.FormatInvariant(differences.Count));
            for(var index = 0; index < differences.Count; index++) {
                builder.AppendLine().AppendFormat(CultureInfo.InvariantCulture, "{0}. {1}", AssertionScope.FormatInvariant(index + 1), differences[index]);
            }

            Fail(AppendDiagnostics(builder.ToString(), options, _value, expected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the collection is equivalent to the expected items (ignoring order).
    /// </summary>
    /// <param name="expected">The expected items that must each appear in the collection.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    public AndConstraint<CollectionAssertions<T>> BeEquivalentTo(params T[] expected) => BeEquivalentTo((IEnumerable<T>)expected);

    /// <summary>Asserts that the collection is not structurally equivalent to the specified collection.</summary>
    /// <param name="unexpected">The collection that must not be equivalent.</param>
    /// <param name="config">Optional equivalency configuration.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The collections are equivalent.</exception>
    public AndConstraint<CollectionAssertions<T>> NotBeEquivalentTo(
        IEnumerable<T> unexpected,
        Func<CollectionEquivalencyOptions<T>, CollectionEquivalencyOptions<T>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_EQUIVALENT_NULL, Expression, JoinItems([.. unexpected])), because, becauseArgs);
            return new(this);
        }

        var options = config?.Invoke(new CollectionEquivalencyOptions<T>()) ?? new CollectionEquivalencyOptions<T>();
        var unexpectedList = unexpected.ToList();
        var actualList = _value.ToList();

        if(actualList.Count == unexpectedList.Count && MatchesEquivalently(unexpectedList, actualList, options)) {
            Fail(Strings.Format(StringKeys.COLLECTION_NOT_BE_EQUIVALENT, Expression, JoinItems(unexpectedList)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Appends the configuration summary and, when opted in, full dumps of both collections.</summary>
    /// <param name="message">The failure message so far.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <param name="subject">The actual collection.</param>
    /// <param name="expectation">The expected collection.</param>
    /// <returns><paramref name="message"/> with diagnostics appended when tracing or dumping is on.</returns>
    private static string AppendDiagnostics(
        string message,
        SelfReferenceEquivalencyOptions<IEnumerable<T>> options,
        IEnumerable<T>? subject,
        IEnumerable<T>? expectation) =>
        EquivalencyDiagnostics.Append(message, options, subject, expectation);

    /// <summary>Dispatches to in-order or multiset matching based on the configured ordering.</summary>
    /// <param name="unexpected">The first sequence.</param>
    /// <param name="actual">The second sequence.</param>
    /// <param name="options">The equivalency options providing the element comparer.</param>
    /// <returns><see langword="true"/> when the sequences match.</returns>
    private static bool MatchesEquivalently(List<T> unexpected, List<T> actual, CollectionEquivalencyOptions<T> options) {
        var equality = options.ResolveEquality();
        if(options.StrictOrdering) {
            return MatchesInOrder(unexpected, actual, equality);
        }

        return MatchesAsMultiset(unexpected, actual, equality);
    }

    /// <summary>Determines whether both sequences match element by element in order.</summary>
    /// <param name="unexpected">The first sequence.</param>
    /// <param name="actual">The second sequence.</param>
    /// <param name="equality">The element comparer.</param>
    /// <returns><see langword="true"/> when every position matches.</returns>
    private static bool MatchesInOrder(List<T> unexpected, List<T> actual, IEqualityComparer<T> equality) {
        for(var index = 0; index < unexpected.Count; index++) {
            if(!equality.Equals(actual[index], unexpected[index])) {
                return false;
            }
        }

        return true;
    }

    /// <summary>Determines whether both sequences contain the same elements with the same multiplicities.</summary>
    /// <param name="unexpected">The first sequence.</param>
    /// <param name="actual">The second sequence.</param>
    /// <param name="equality">The element comparer.</param>
    /// <returns><see langword="true"/> when the multisets are equal.</returns>
    private static bool MatchesAsMultiset(List<T> unexpected, List<T> actual, IEqualityComparer<T> equality) {
        var unexpectedCounts = CountByElement(unexpected, equality);
        var actualCounts = CountByElement(actual, equality);
        foreach(var item in unexpected) {
            var unexpectedCount = unexpectedCounts[item!];
            actualCounts.TryGetValue(item!, out var actualCount);
            if(unexpectedCount != actualCount) {
                return false;
            }
        }

        return true;
    }

    /// <summary>Asserts that every element is structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <param name="expectation">The single expectation every element must match.</param>
    /// <param name="config">Optional equivalency configuration.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Any element is not equivalent to <paramref name="expectation"/>.</exception>
    public AndConstraint<CollectionAssertions<T>> AllBeEquivalentTo(
        T expectation,
        Func<CollectionEquivalencyOptions<T>, CollectionEquivalencyOptions<T>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_EQUIVALENT_NULL, Expression, Format(expectation)), because, becauseArgs);
            return new(this);
        }

        var options = config?.Invoke(new CollectionEquivalencyOptions<T>()) ?? new CollectionEquivalencyOptions<T>();
        var equality = options.ResolveEquality();
        var index = 0;
        foreach(var item in _value) {
            if(!equality.Equals(item, expectation)) {
                Fail(Strings.Format(StringKeys.COLLECTION_ALL_BE_EQUIVALENT, Expression, Format(expectation), Format(item), AssertionScope.FormatInvariant(index)), because, becauseArgs);
                return new(this);
            }

            index++;
        }

        return new(this);
    }
}
