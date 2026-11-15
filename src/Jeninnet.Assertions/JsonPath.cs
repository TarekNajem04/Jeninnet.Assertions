//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>The kind of navigation performed by one mini JSON path segment.</summary>
internal enum JsonPathSegmentKind {
    Property = 0,
    Element = 1,
    Wildcard = 2,
    RecursiveProperty = 3,
}

/// <summary>A single navigation step of a mini JSON path.</summary>
internal readonly struct JsonPathSegment {
    /// <summary>Creates a segment (use the factory methods).</summary>
    /// <param name="kind">The segment kind.</param>
    /// <param name="name">The property name, when applicable.</param>
    /// <param name="index">The array index, when applicable.</param>
    private JsonPathSegment(JsonPathSegmentKind kind, string? name, int index) {
        Kind = kind;
        Name = name;
        Index = index;
    }

    /// <summary>Gets the segment kind.</summary>
    public JsonPathSegmentKind Kind { get; }

    /// <summary>Gets the property name for property and recursive segments.</summary>
    public string? Name { get; }

    /// <summary>Gets the array index for element segments.</summary>
    public int Index { get; }

    /// <summary>Gets a value indicating whether this segment is an object property.</summary>
    public bool IsProperty => Kind == JsonPathSegmentKind.Property;

    /// <summary>Gets a value indicating whether this segment is an array element.</summary>
    public bool IsElement => Kind == JsonPathSegmentKind.Element;

    /// <summary>Gets a value indicating whether this segment selects every child.</summary>
    public bool IsWildcard => Kind == JsonPathSegmentKind.Wildcard;

    /// <summary>Gets a value indicating whether this segment recursively selects a property.</summary>
    public bool IsRecursive => Kind == JsonPathSegmentKind.RecursiveProperty;

    /// <summary>Gets a value indicating whether this segment can produce more than one match.</summary>
    public bool IsMultiMatch => IsWildcard || IsRecursive;

    /// <summary>Creates an object-property segment.</summary>
    /// <param name="name">The property name.</param>
    /// <returns>The segment.</returns>
    public static JsonPathSegment Property(string name) => new(JsonPathSegmentKind.Property, name, 0);

    /// <summary>Creates an array-element segment.</summary>
    /// <param name="index">The zero-based or negative-from-end element index.</param>
    /// <returns>The segment.</returns>
    public static JsonPathSegment Element(int index) => new(JsonPathSegmentKind.Element, null, index);

    /// <summary>Creates a wildcard segment selecting every object value or array element.</summary>
    /// <returns>The segment.</returns>
    public static JsonPathSegment Wildcard() => new(JsonPathSegmentKind.Wildcard, null, 0);

    /// <summary>Creates a recursive-descent segment selecting a property at any depth.</summary>
    /// <param name="name">The property name to find.</param>
    /// <returns>The segment.</returns>
    public static JsonPathSegment RecursiveProperty(string name) => new(JsonPathSegmentKind.RecursiveProperty, name, 0);
}

/// <summary>Parses the supported mini JSON path subset.</summary>
/// <remarks>
/// Supported: <c>$</c>, <c>.property</c>, <c>['property.with.dots']</c>, <c>["name"]</c>,
/// <c>[index]</c>, <c>[-index]</c>, <c>[*]</c>, <c>.*</c>, and <c>..name</c>. Filters, recursive
/// wildcards, and expressions are deliberately unsupported. Malformed input is reported as an
/// <see cref="ArgumentException"/> naming the offending construct and position.
/// </remarks>
internal static class JsonPath {
    /// <summary>Parses <paramref name="path"/> into navigation segments.</summary>
    /// <param name="path">The path to parse (must be non-null; callers guard).</param>
    /// <returns>The segments in navigation order (empty for the bare root <c>$</c>).</returns>
    /// <exception cref="ArgumentException">The path does not start with <c>$</c> or is malformed.</exception>
    internal static IReadOnlyList<JsonPathSegment> Parse(string path) {
        if(path.Length == 0 || path[0] != '$') {
            throw Malformed(path, 0, "it must start with '$'");
        }

        var segments = new List<JsonPathSegment>();
        var position = 1;
        while(position < path.Length) {
            if(path[position] == '.') {
                position++;
                position = position < path.Length && path[position] == '.'
                    ? ParseRecursive(path, position + 1, segments)
                    : ParseProperty(path, position, segments);
            } else if(path[position] == '[') {
                position = ParseBracket(path, position, segments);
            } else {
                throw Malformed(path, position, $"unexpected character '{path[position]}'");
            }
        }

        return segments;
    }

    /// <summary>Parses a normal property after a single dot.</summary>
    /// <param name="path">The full path.</param>
    /// <param name="position">The position after the dot.</param>
    /// <param name="segments">The segment list to extend.</param>
    /// <returns>The position after the property name.</returns>
    /// <exception cref="ArgumentException">The property name is missing.</exception>
    private static int ParseProperty(string path, int position, List<JsonPathSegment> segments) {
        if(position < path.Length && path[position] == '*') {
            segments.Add(JsonPathSegment.Wildcard());
            return position + 1;
        }

        var start = position;
        while(position < path.Length && path[position] != '.' && path[position] != '[' && path[position] != ']') {
            position++;
        }

        if(position == start) {
            throw Malformed(path, position, "a property name is required after '.'");
        }

        segments.Add(JsonPathSegment.Property(path.Substring(start, position - start)));
        return position;
    }

    /// <summary>Parses a recursive-descent property after two dots.</summary>
    /// <param name="path">The full path.</param>
    /// <param name="position">The position after the second dot.</param>
    /// <param name="segments">The segment list to extend.</param>
    /// <returns>The position after the property name.</returns>
    /// <exception cref="ArgumentException">The recursive property name is missing or a recursive wildcard is used.</exception>
    private static int ParseRecursive(string path, int position, List<JsonPathSegment> segments) {
        if(position >= path.Length) {
            throw Malformed(path, position, "a property name is required after '..'");
        }

        if(path[position] == '[') {
            var (name, next) = ParseQuotedName(path, position + 1);
            segments.Add(JsonPathSegment.RecursiveProperty(name));
            return next;
        }

        if(path[position] == '*') {
            throw Malformed(path, position, "recursive wildcards are not supported");
        }

        var start = position;
        while(position < path.Length && path[position] != '.' && path[position] != '[' && path[position] != ']') {
            position++;
        }

        if(position == start) {
            throw Malformed(path, position, "a property name is required after '..'");
        }

        segments.Add(JsonPathSegment.RecursiveProperty(path.Substring(start, position - start)));
        return position;
    }

    /// <summary>Parses a bracketed segment: an index, a quoted name, or a wildcard.</summary>
    /// <param name="path">The full path.</param>
    /// <param name="position">The position of the opening bracket.</param>
    /// <param name="segments">The segment list to extend.</param>
    /// <returns>The position after the closing bracket.</returns>
    /// <exception cref="ArgumentException">The bracket segment is malformed.</exception>
    private static int ParseBracket(string path, int position, List<JsonPathSegment> segments) {
        var content = position + 1;
        if(content < path.Length && path[content] == '*') {
            var closing = content + 1;
            if(closing >= path.Length || path[closing] != ']') {
                throw Malformed(path, closing, "expected ']' after '*'");
            }

            segments.Add(JsonPathSegment.Wildcard());
            return closing + 1;
        }

        if(content < path.Length && (path[content] == '\'' || path[content] == '"')) {
            var (name, next) = ParseQuotedName(path, content);
            segments.Add(JsonPathSegment.Property(name));
            return next;
        }

        var end = position + 1;
        while(end < path.Length && path[end] != ']') {
            end++;
        }

        if(end >= path.Length) {
            throw Malformed(path, position, "missing ']'");
        }

        var text = path.Substring(content, end - content);
        if(!TryParseIndex(text, out var index)) {
            throw Malformed(path, position, $"'[{text}]' is not an integer index");
        }

        segments.Add(JsonPathSegment.Element(index));
        return end + 1;
    }

    /// <summary>Parses a single- or double-quoted property name and its closing bracket.</summary>
    /// <param name="path">The full path.</param>
    /// <param name="position">The position of the opening quote.</param>
    /// <returns>The unescaped name and the position after the closing bracket.</returns>
    /// <exception cref="ArgumentException">The quoted name or its closing bracket is malformed.</exception>
    private static (string Name, int Next) ParseQuotedName(string path, int position) {
        var quote = path[position];
        position++;
        var builder = new StringBuilder();
        while(position < path.Length) {
            var current = path[position];
            if(current == '\\') {
                position++;
                if(position >= path.Length) {
                    throw Malformed(path, position, "unterminated escape in a quoted property name");
                }

                builder.Append(path[position]);
                position++;
                continue;
            }

            if(current == quote) {
                position++;
                if(position >= path.Length || path[position] != ']') {
                    throw Malformed(path, position, "expected ']' after a quoted property name");
                }

                return (builder.ToString(), position + 1);
            }

            builder.Append(current);
            position++;
        }

        throw Malformed(path, position, "unterminated quoted property name");
    }

    /// <summary>Parses a non-negative or negative-from-end integer index.</summary>
    /// <param name="text">The bracketed text (without brackets).</param>
    /// <param name="index">The parsed index on success.</param>
    /// <returns><see langword="true"/> when the text is an integer index.</returns>
    private static bool TryParseIndex(string text, out int index) {
        index = 0;
        if(text.Length == 0) {
            return false;
        }

        var start = text[0] == '-' ? 1 : 0;
        if(start == text.Length) {
            return false;
        }

        for(var position = start; position < text.Length; position++) {
            if(text[position] is < '0' or > '9') {
                return false;
            }
        }

        return int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out index);
    }

    /// <summary>Creates a consistent malformed-path exception with its position.</summary>
    /// <param name="path">The malformed path.</param>
    /// <param name="position">The position of the offending construct.</param>
    /// <param name="reason">The reason.</param>
    /// <returns>The exception to throw.</returns>
    private static ArgumentException Malformed(string path, int position, string reason) =>
        new($"The JSON path '{path}' is malformed at position {position}: {reason}.", nameof(path));
}
