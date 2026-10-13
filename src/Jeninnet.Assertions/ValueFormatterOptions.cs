//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Holds an ordered set of <see cref="IValueFormatter"/> instances used when rendering failure messages.</summary>
/// <remarks>Each <see cref="AssertionScope"/> owns an instance (nested scopes inherit a snapshot of the parent);
/// <see cref="AssertionConfiguration.Formatting"/> holds the process-wide instance.</remarks>
public sealed class ValueFormatterOptions {
    private readonly List<IValueFormatter> _formatters = [];
    private readonly object _sync = new();

    /// <summary>Adds a formatter. Earlier formatters win over later ones.</summary>
    /// <param name="formatter">The formatter to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="formatter"/> is <see langword="null"/>.</exception>
    public void AddFormatter(IValueFormatter formatter) {
        if(formatter is null) {
            throw new ArgumentNullException(nameof(formatter));
        }

        lock(_sync) {
            _formatters.Add(formatter);
        }
    }

    /// <summary>Removes a formatter.</summary>
    /// <param name="formatter">The formatter to remove.</param>
    /// <returns><see langword="true"/> when removed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="formatter"/> is <see langword="null"/>.</exception>
    public bool RemoveFormatter(IValueFormatter formatter) {
        if(formatter is null) {
            throw new ArgumentNullException(nameof(formatter));
        }

        lock(_sync) {
            return _formatters.Remove(formatter);
        }
    }

    /// <summary>Removes every formatter of type <typeparamref name="TFormatter"/>.</summary>
    /// <typeparam name="TFormatter">The formatter type to remove.</typeparam>
    /// <returns><see langword="true"/> when at least one formatter was removed.</returns>
    public bool RemoveFormatter<TFormatter>() where TFormatter : IValueFormatter {
        lock(_sync) {
            return _formatters.RemoveAll(static formatter => formatter is TFormatter) > 0;
        }
    }

    /// <summary>Removes all formatters.</summary>
    public void Clear() {
        lock(_sync) {
            _formatters.Clear();
        }
    }

    /// <summary>Takes a snapshot of the registered formatters.</summary>
    /// <returns>The formatters in registration order.</returns>
    internal IReadOnlyList<IValueFormatter> Snapshot() {
        lock(_sync) {
            return [.. _formatters];
        }
    }

    /// <summary>Replaces the registered formatters with a previously captured snapshot (used by <see cref="AssertionIsolation"/>).</summary>
    /// <param name="formatters">The snapshot to restore.</param>
    internal void RestoreFrom(IReadOnlyList<IValueFormatter> formatters) {
        lock(_sync) {
            _formatters.Clear();
            _formatters.AddRange(formatters);
        }
    }

    /// <summary>Copies the registered formatters into a new instance.</summary>
    /// <returns>The copy.</returns>
    internal ValueFormatterOptions Copy() {
        var copy = new ValueFormatterOptions();
        lock(_sync) {
            copy._formatters.AddRange(_formatters);
        }

        return copy;
    }
}
