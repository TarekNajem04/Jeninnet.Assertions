//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Creates delegates that can be asserted fluently in a single statement.</summary>
public static class FluentActions {
    /// <summary>Returns <paramref name="action"/> unchanged so it can be asserted with <c>Should()</c>.</summary>
    /// <param name="action">The synchronous delegate to assert on.</param>
    /// <returns><paramref name="action"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static Action Invoking(Action action) => action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Returns <paramref name="action"/> unchanged so it can be asserted with <c>Should()</c>.</summary>
    /// <param name="action">The asynchronous delegate to assert on.</param>
    /// <returns><paramref name="action"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static Func<Task> Awaiting(Func<Task> action) => action ?? throw new ArgumentNullException(nameof(action));

    /// <summary>Returns an action that fully enumerates the collection produced by <paramref name="enumerable"/> when invoked.</summary>
    /// <param name="enumerable">The delegate producing the collection to enumerate.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>An action that forces enumeration.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is <see langword="null"/>.</exception>
    /// <remarks>Deferred iterators may only support a single enumeration; invoking the returned action twice can fail.</remarks>
    public static Action Enumerating<T>(Func<IEnumerable<T>> enumerable) {
        if(enumerable is null) {
            throw new ArgumentNullException(nameof(enumerable));
        }

        return () => {
            using var enumerator = enumerable().GetEnumerator();
            while(enumerator.MoveNext()) {
                _ = enumerator.Current;
            }
        };
    }
}
