//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Applies formatting limits and optional structural collection formatting.</summary>
internal static class FormattingContext {
    /// <summary>Formats a value with the default representation and configured limits.</summary>
    /// <param name="value">The value to format.</param>
    /// <param name="options">The active formatting options.</param>
    /// <returns>The limited display text.</returns>
    internal static string DefaultFormat(object? value, ValueFormatterOptions options) {
        if(value is null) {
            return string.Empty;
        }

        if(options.RespectRuntimeTypes && value is IEnumerable enumerable && value is not string) {
            return ApplyLimits(FormatEnumerable(enumerable, options, 0, new HashSet<object>(ReferenceComparer.Instance)), options);
        }

        return ApplyLimits(string.Format(CultureInfo.InvariantCulture, "{0}", value), options);
    }

    /// <summary>Applies line-break, line-count, and string-length limits to formatted text.</summary>
    /// <remarks>Line endings are preserved unless <see cref="ValueFormatterOptions.UseLineBreaks"/> is
    /// disabled; truncation markers always use the platform newline so they read as their own line.</remarks>
    /// <param name="text">The formatted text.</param>
    /// <param name="options">The active formatting options.</param>
    /// <returns>The limited text, with explicit truncation markers.</returns>
    internal static string ApplyLimits(string text, ValueFormatterOptions options) {
        var result = options.UseLineBreaks ? text : CollapseWhitespace(text);
        if(options.MaxLines > 0) {
            result = TruncateLines(result, options.MaxLines);
        }

        if(options.MaxStringLength > 0 && result.Length > options.MaxStringLength) {
            var hidden = result.Length - options.MaxStringLength;
            result = result.TruncateTo(options.MaxStringLength) + Strings.Format(StringKeys.FORMATTING_TRUNCATED_CHARS, hidden);
        }

        return result;
    }

    /// <summary>Keeps at most <paramref name="maxLines"/> lines and appends the hidden-line count.</summary>
    /// <param name="text">The text to limit.</param>
    /// <param name="maxLines">The maximum number of lines to keep (positive).</param>
    /// <returns>The original text when it fits; otherwise the prefix followed by the truncation marker.</returns>
    private static string TruncateLines(string text, int maxLines) {
        var total = 1;
        var cutIndex = -1;
        var index = 0;
        while(index < text.Length) {
            var character = text[index];
            if(character == '\r') {
                if(index + 1 < text.Length && text[index + 1] == '\n') {
                    index++;
                }

                total++;
                if(total == maxLines + 1) {
                    cutIndex = index + 1;
                }
            } else if(character == '\n') {
                total++;
                if(total == maxLines + 1) {
                    cutIndex = index + 1;
                }
            }

            index++;
        }

        if(text.Length != 0 && (text[text.Length - 1] == '\n' || text[text.Length - 1] == '\r')) {
            total--;
        }

        if(cutIndex < 0 || total <= maxLines) {
            return text;
        }

        var hidden = total - maxLines;
        var kept = text.Substring(0, cutIndex).TrimEnd('\r', '\n');
        return kept + Environment.NewLine + Strings.Format(StringKeys.FORMATTING_TRUNCATED_LINES, hidden);
    }

    /// <summary>Formats an enumerable structurally, respecting depth and item limits.</summary>
    /// <param name="enumerable">The enumerable to format.</param>
    /// <param name="options">The active formatting options.</param>
    /// <param name="depth">The current nesting depth.</param>
    /// <param name="visited">Reference-identity cycle guard.</param>
    /// <returns>The bracketed representation.</returns>
    private static string FormatEnumerable(IEnumerable enumerable, ValueFormatterOptions options, int depth, HashSet<object> visited) {
        if(options.MaxDepth > 0 && depth >= options.MaxDepth) {
            return Strings.Get(StringKeys.FORMATTING_MAX_DEPTH);
        }

        if(!visited.Add(enumerable)) {
            return Strings.Get(StringKeys.FORMATTING_CYCLE);
        }

        var items = new List<string>();
        var truncated = false;
        foreach(var item in enumerable) {
            if(options.MaxEnumerables > 0 && items.Count >= options.MaxEnumerables) {
                truncated = true;
                break;
            }

            items.Add(FormatItem(item, options, depth + 1, visited));
        }

        visited.Remove(enumerable);
        var suffix = truncated ? ", " + Strings.Get(StringKeys.FORMATTING_TRUNCATED_ITEMS) : string.Empty;
        return "[" + string.Join(", ", items) + suffix + "]";
    }

    /// <summary>Formats one enumerable item, recursing into nested collections.</summary>
    /// <param name="item">The item to format.</param>
    /// <param name="options">The active formatting options.</param>
    /// <param name="depth">The item's nesting depth.</param>
    /// <param name="visited">Reference-identity cycle guard.</param>
    /// <returns>The item representation.</returns>
    private static string FormatItem(object? item, ValueFormatterOptions options, int depth, HashSet<object> visited) {
        if(item is null) {
            return "<null>";
        }

        if(item is string text) {
            return $"\"{text}\"";
        }

        return item is IEnumerable nested && item is not string
            ? FormatEnumerable(nested, options, depth, visited)
            : string.Format(CultureInfo.InvariantCulture, "{0}", item);
    }

    /// <summary>Collapses whitespace runs to single spaces and trims.</summary>
    /// <param name="text">The text to collapse.</param>
    /// <returns>The single-line text.</returns>
    private static string CollapseWhitespace(string text) {
        var builder = new StringBuilder(text.Length);
        var pendingSpace = false;
        foreach(var character in text) {
            if(char.IsWhiteSpace(character)) {
                pendingSpace = builder.Length != 0;
                continue;
            }

            if(pendingSpace) {
                builder.Append(' ');
                pendingSpace = false;
            }

            builder.Append(character);
        }

        return builder.ToString();
    }

    /// <summary>Reference-identity comparer for cycle detection.</summary>
    private sealed class ReferenceComparer : IEqualityComparer<object> {
        internal static readonly ReferenceComparer Instance = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
