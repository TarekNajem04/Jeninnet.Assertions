//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>A single navigation step of a mini JSON path: an object property or an array element.</summary>
internal readonly struct JsonPathSegment {
    /// <summary>Creates a segment (use <see cref="Property"/> or <see cref="Element"/>).</summary>
    /// <param name="name">The property name, or <see langword="null"/> for an element segment.</param>
    /// <param name="index">The array index, or a negative sentinel for a property segment.</param>
    private JsonPathSegment(string? name, int index) {
        Name = name;
        Index = index;
    }

    /// <summary>Gets the property name, or <see langword="null"/> for an element segment.</summary>
    public string? Name { get; }

    /// <summary>Gets the array index, or a negative sentinel for a property segment.</summary>
    public int Index { get; }

    /// <summary>Gets a value indicating whether this segment addresses an array element.</summary>
    public bool IsIndex => Name is null;

    /// <summary>Creates an object-property segment.</summary>
    /// <param name="name">The property name.</param>
    /// <returns>The segment.</returns>
    public static JsonPathSegment Property(string name) => new(name, -1);

    /// <summary>Creates an array-element segment.</summary>
    /// <param name="index">The zero-based element index.</param>
    /// <returns>The segment.</returns>
    public static JsonPathSegment Element(int index) => new(null, index);
}

/// <summary>Parses the supported mini JSON path subset: <c>$</c>, <c>.property</c>, and <c>[index]</c>.</summary>
/// <remarks>
/// Wildcards, filters, quoted names, and recursive descent are deliberately unsupported; property names may
/// not contain <c>.</c>, <c>[</c>, or <c>]</c>. Malformed input is reported as an <see cref="ArgumentException"/>
/// naming the offending construct.
/// </remarks>
internal static class JsonPath {
    /// <summary>Parses <paramref name="path"/> into navigation segments.</summary>
    /// <param name="path">The path to parse (must be non-null; callers guard).</param>
    /// <returns>The segments in navigation order (empty for the bare root <c>$</c>).</returns>
    /// <exception cref="ArgumentException">The path does not start with <c>$</c> or is malformed.</exception>
    internal static IReadOnlyList<JsonPathSegment> Parse(string path) {
        if(path.Length == 0 || path[0] != '$') {
            throw new ArgumentException($"The JSON path '{path}' is malformed: it must start with '$'.", nameof(path));
        }

        var segments = new List<JsonPathSegment>();
        var position = 1;
        while(position < path.Length) {
            var current = path[position];
            if(current == '.') {
                position++;
                var start = position;
                while(position < path.Length && path[position] != '.' && path[position] != '[' && path[position] != ']') {
                    position++;
                }

                if(position == start) {
                    throw new ArgumentException($"The JSON path '{path}' is malformed: a property name is required after '.'.", nameof(path));
                }

                segments.Add(JsonPathSegment.Property(path.Substring(start, position - start)));
            } else if(current == '[') {
                position++;
                var closing = path.IndexOf(']', position);
                if(closing < 0) {
                    throw new ArgumentException($"The JSON path '{path}' is malformed: missing ']'.", nameof(path));
                }

                var text = path.Substring(position, closing - position);
                if(!IsDigits(text) || !int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var index)) {
                    throw new ArgumentException($"The JSON path '{path}' is malformed: '[{text}]' is not a non-negative integer index.", nameof(path));
                }

                segments.Add(JsonPathSegment.Element(index));
                position = closing + 1;
                if(position < path.Length && path[position] != '.' && path[position] != '[') {
                    throw new ArgumentException($"The JSON path '{path}' is malformed: unexpected character '{path[position]}' after ']'.", nameof(path));
                }
            } else {
                throw new ArgumentException($"The JSON path '{path}' is malformed: unexpected character '{current}'.", nameof(path));
            }
        }

        return segments;
    }

    /// <summary>Determines whether a string consists solely of ASCII digits and is non-empty.</summary>
    /// <param name="text">The text to inspect.</param>
    /// <returns><see langword="true"/> when every character is <c>0</c>–<c>9</c>.</returns>
    private static bool IsDigits(string text) {
        if(text.Length == 0) {
            return false;
        }

        for(var index = 0; index < text.Length; index++) {
            if(text[index] is < '0' or > '9') {
                return false;
            }
        }

        return true;
    }
}
