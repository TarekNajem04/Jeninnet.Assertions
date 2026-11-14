//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Holds the ordered value formatters and rendering limits used when producing failure messages.</summary>
/// <remarks>Each <see cref="AssertionScope"/> owns an instance (nested scopes inherit a snapshot of the parent);
/// <see cref="AssertionConfiguration.Formatting"/> holds the process-wide instance. Limits are applied after
/// formatters and never affect comparison.</remarks>
public sealed class ValueFormatterOptions {
    private readonly List<IValueFormatter> _formatters = [];
    private readonly object _sync = new();

    /// <summary>Gets or sets the maximum collection nesting depth used when <see cref="RespectRuntimeTypes"/> is enabled.</summary>
    public int MaxDepth { get; set; } = 10;

    /// <summary>Gets or sets the maximum number of rendered lines (zero or less disables the limit).</summary>
    public int MaxLines { get; set; } = 100;

    /// <summary>Gets or sets the maximum number of items rendered from one enumerable (zero or less disables the limit).</summary>
    public int MaxEnumerables { get; set; } = 100;

    /// <summary>Gets or sets the maximum rendered string length (zero or less disables the limit).</summary>
    public int MaxStringLength { get; set; } = 8_000;

    /// <summary>Gets or sets whether formatter output may span multiple lines.</summary>
    public bool UseLineBreaks { get; set; } = true;

    /// <summary>Gets or sets whether structured collection formatting uses runtime element shapes instead of <c>ToString()</c>.</summary>
    /// <remarks>Defaults to <see langword="false"/> so existing messages keep their historical display.</remarks>
    public bool RespectRuntimeTypes { get; set; }

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

    /// <summary>Replaces the registered formatters and limits with a previously captured snapshot (used by <see cref="AssertionIsolation"/>).</summary>
    /// <param name="options">The snapshot to restore.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    internal void RestoreFrom(ValueFormatterOptions options) {
        if(options is null) {
            throw new ArgumentNullException(nameof(options));
        }

        MaxDepth = options.MaxDepth;
        MaxLines = options.MaxLines;
        MaxEnumerables = options.MaxEnumerables;
        MaxStringLength = options.MaxStringLength;
        UseLineBreaks = options.UseLineBreaks;
        RespectRuntimeTypes = options.RespectRuntimeTypes;
        lock(_sync) {
            _formatters.Clear();
            _formatters.AddRange(options.Snapshot());
        }
    }

    /// <summary>Copies the formatters and limits into a new instance.</summary>
    /// <returns>The copy.</returns>
    internal ValueFormatterOptions Copy() {
        var copy = new ValueFormatterOptions {
            MaxDepth = MaxDepth,
            MaxLines = MaxLines,
            MaxEnumerables = MaxEnumerables,
            MaxStringLength = MaxStringLength,
            UseLineBreaks = UseLineBreaks,
            RespectRuntimeTypes = RespectRuntimeTypes,
        };
        lock(_sync) {
            copy._formatters.AddRange(_formatters);
        }

        return copy;
    }
}
