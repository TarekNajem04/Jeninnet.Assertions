//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="JsonNode"/> values (System.Text.Json).</summary>
/// <param name="value">The JSON node to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class JsonNodeAssertions(JsonNode? value, string? expression = null) : AssertionsBase(expression, "JSON") {
    private const int PREVIEW_LENGTH = 120;
    private const int AVAILABLE_NAMES_LIMIT = 10;

    private readonly JsonNode? _value = value;

    /// <summary>Asserts that the node is a JSON null value or a missing (C# <see langword="null"/>) node.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node holds a non-null JSON value.</exception>
    public AndConstraint<JsonNodeAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && !IsJsonNull(_value)) {
            Fail(Strings.Format(StringKeys.JSON_BE_NULL, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node holds a non-null JSON value.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is missing or a JSON null.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null || IsJsonNull(_value)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_NULL, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is structurally equivalent to <paramref name="expected"/> (order-insensitive for objects).</summary>
    /// <param name="expected">The expected JSON node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The nodes are not equivalent.</exception>
    public AndConstraint<JsonNodeAssertions> BeEquivalentTo(JsonNode? expected, string? because = null, params object?[] becauseArgs) {
        if(!JsonNode.DeepEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.JSON_BE_EQUIVALENT, Expression, Preview(_value), Preview(expected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is not structurally equivalent to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The unexpected JSON node.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The nodes are equivalent.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeEquivalentTo(JsonNode? unexpected, string? because = null, params object?[] becauseArgs) {
        if(JsonNode.DeepEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_EQUIVALENT, Expression, Preview(_value), Preview(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a JSON array with exactly <paramref name="expected"/> elements.</summary>
    /// <param name="expected">The expected element count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is not a JSON array or its count differs.</exception>
    public AndConstraint<JsonNodeAssertions> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_value is not JsonArray array || array.Count != expected) {
            Fail(Strings.Format(StringKeys.JSON_CONTAIN_COUNT, Expression, AssertionScope.FormatInvariant(expected), Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a JSON string and exposes its value for further assertions.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the string value.</returns>
    /// <exception cref="AssertionFailedException">The node is not a JSON string.</exception>
    public WhichConstraint<string?> BeString(string? because = null, params object?[] becauseArgs) {
        if(_value is not JsonValue json || json.GetValueKind() != JsonValueKind.String || !json.TryGetValue(out string? actual)) {
            Fail(Strings.Format(StringKeys.JSON_BE_STRING, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<string?>(null);
        }

        return new WhichConstraint<string?>(actual);
    }

    /// <summary>Asserts that the node is not a JSON string.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a JSON string.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeString(string? because = null, params object?[] becauseArgs) {
        if(_value is JsonValue json && json.GetValueKind() == JsonValueKind.String) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_STRING, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a JSON number and exposes its canonical text for further assertions.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the number text.</returns>
    /// <exception cref="AssertionFailedException">The node is not a JSON number.</exception>
    public WhichConstraint<string?> BeNumeric(string? because = null, params object?[] becauseArgs) {
        if(_value is not JsonValue json || json.GetValueKind() != JsonValueKind.Number) {
            Fail(Strings.Format(StringKeys.JSON_BE_NUMERIC, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<string?>(null);
        }

        return new WhichConstraint<string?>(json.ToJsonString());
    }

    /// <summary>Asserts that the node is not a JSON number.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a JSON number.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeNumeric(string? because = null, params object?[] becauseArgs) {
        if(_value is JsonValue json && json.GetValueKind() == JsonValueKind.Number) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_NUMERIC, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a JSON boolean and exposes its value for further assertions.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the boolean value.</returns>
    /// <exception cref="AssertionFailedException">The node is not a JSON boolean.</exception>
    public WhichConstraint<bool> BeBool(string? because = null, params object?[] becauseArgs) {
        if(_value is not JsonValue json
            || (json.GetValueKind() != JsonValueKind.True && json.GetValueKind() != JsonValueKind.False)
            || !json.TryGetValue(out bool actual)) {
            Fail(Strings.Format(StringKeys.JSON_BE_BOOL, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<bool>(false);
        }

        return new WhichConstraint<bool>(actual);
    }

    /// <summary>Asserts that the node is not a JSON boolean.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a JSON boolean.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeBool(string? because = null, params object?[] becauseArgs) {
        if(_value is JsonValue json && (json.GetValueKind() == JsonValueKind.True || json.GetValueKind() == JsonValueKind.False)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_BOOL, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a JSON array and exposes it for further assertions.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the array.</returns>
    /// <exception cref="AssertionFailedException">The node is not a JSON array.</exception>
    public WhichConstraint<JsonArray> BeAnArray(string? because = null, params object?[] becauseArgs) {
        if(_value is not JsonArray array) {
            Fail(Strings.Format(StringKeys.JSON_BE_ARRAY, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<JsonArray>([]);
        }

        return new WhichConstraint<JsonArray>(array);
    }

    /// <summary>Asserts that the node is not a JSON array.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a JSON array.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeAnArray(string? because = null, params object?[] becauseArgs) {
        if(_value is JsonArray) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_ARRAY, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a UTC date and exposes its value for further assertions.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the date.</returns>
    /// <exception cref="AssertionFailedException">The node is not a UTC date.</exception>
    public WhichConstraint<DateTime> BeUtcDate(string? because = null, params object?[] becauseArgs) {
        if(!TryGetDate(static date => date.Kind == DateTimeKind.Utc, out var actual)) {
            Fail(Strings.Format(StringKeys.JSON_BE_UTC_DATE, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<DateTime>(default);
        }

        return new WhichConstraint<DateTime>(actual);
    }

    /// <summary>Asserts that the node is not a UTC date.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a UTC date.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeUtcDate(string? because = null, params object?[] becauseArgs) {
        if(TryGetDate(static date => date.Kind == DateTimeKind.Utc, out _)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_UTC_DATE, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node is a successfully parsed date that is not UTC, and exposes its value for further assertions.</summary>
    /// <remarks>An ISO date without an offset parses as <see cref="DateTimeKind.Unspecified"/>; this assertion treats that local-clock representation as local.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the date.</returns>
    /// <exception cref="AssertionFailedException">The node is not a local date.</exception>
    public WhichConstraint<DateTime> BeLocalDate(string? because = null, params object?[] becauseArgs) {
        if(!TryGetDate(static date => date.Kind != DateTimeKind.Utc, out var actual)) {
            Fail(Strings.Format(StringKeys.JSON_BE_LOCAL_DATE, Expression, Preview(_value)), because, becauseArgs);
            return new WhichConstraint<DateTime>(default);
        }

        return new WhichConstraint<DateTime>(actual);
    }

    /// <summary>Asserts that the node is not a successfully parsed non-UTC date.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node is a local date.</exception>
    public AndConstraint<JsonNodeAssertions> NotBeLocalDate(string? because = null, params object?[] becauseArgs) {
        if(TryGetDate(static date => date.Kind != DateTimeKind.Utc, out _)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_BE_LOCAL_DATE, Expression, Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the node holds the scalar <paramref name="expected"/> value.</summary>
    /// <typeparam name="T">The scalar type.</typeparam>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The node does not hold <paramref name="expected"/>.</exception>
    public AndConstraint<JsonNodeAssertions> HaveValue<T>(T? expected, string? because = null, params object?[] becauseArgs) {
        var expectedNode = JsonValue.Create(expected);
        if(!JsonNode.DeepEquals(_value, expectedNode)) {
            Fail(Strings.Format(StringKeys.JSON_HAVE_VALUE, Expression, Preview(expectedNode), Preview(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the node is a JSON object with property <paramref name="name"/> and returns a
    /// <see cref="WhichConstraint{T}"/> exposing its value for further assertions.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the property value (may be <see langword="null"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is not a JSON object or lacks <paramref name="name"/>.</exception>
    public WhichConstraint<JsonNode?> HaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value is JsonObject obj && obj.TryGetPropertyValue(name, out var child)) {
            return new WhichConstraint<JsonNode?>(child);
        }

        var suggestion = _value is JsonObject owner ? NearestCandidate.Describe(name, owner.Select(property => property.Key)) : string.Empty;
        throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.JSON_HAVE_PROPERTY, Expression, name, Preview(_value)) + suggestion, because, becauseArgs));
    }

    /// <summary>
    /// Asserts that the node is a JSON object without the specified property.
    /// </summary>
    /// <param name="name">The property name that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The node is a JSON object with the specified property.</exception>
    public AndConstraint<JsonNodeAssertions> NotHaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value is JsonObject obj && obj.TryGetPropertyValue(name, out _)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_HAVE_PROPERTY, Expression, Format(name)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>
    /// Asserts that the mini JSON path <paramref name="path"/> resolves against the node and returns a
    /// <see cref="WhichConstraint{T}"/> exposing the resolved node for further assertions.
    /// </summary>
    /// <remarks>
    /// Supported subset: <c>$</c> for the root, <c>.property</c> for object members (case-sensitive),
    /// and <c>[index]</c> for array elements. A path that ends on a JSON null still counts as present.
    /// Wildcards, filters, and recursive descent are not supported; property names may not contain
    /// <c>.</c>, <c>[</c>, or <c>]</c>.
    /// </remarks>
    /// <param name="path">The path to resolve (for example, <c>$.users[0].name</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the resolved node (which may be a JSON null).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is malformed.</exception>
    /// <exception cref="AssertionFailedException">The path does not resolve.</exception>
    public WhichConstraint<JsonNode?> HaveJsonPath(string path, string? because = null, params object?[] becauseArgs) {
        if(path is null) {
            throw new ArgumentNullException(nameof(path));
        }

        var segments = JsonPath.Parse(path);
        if(TryResolvePath(segments, out var node, out var reason, out var suggestion)) {
            return new WhichConstraint<JsonNode?>(node);
        }

        Fail(Strings.Format(StringKeys.JSON_HAVE_PATH, Expression, path, reason) + suggestion, because, becauseArgs);
        return new WhichConstraint<JsonNode?>(null);
    }

    /// <summary>Asserts that the mini JSON path <paramref name="path"/> does not resolve against the node.</summary>
    /// <remarks>The supported subset is documented on <see cref="HaveJsonPath"/>.</remarks>
    /// <param name="path">The path that must not resolve.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is malformed.</exception>
    /// <exception cref="AssertionFailedException">The path resolves.</exception>
    public AndConstraint<JsonNodeAssertions> NotHaveJsonPath(string path, string? because = null, params object?[] becauseArgs) {
        if(path is null) {
            throw new ArgumentNullException(nameof(path));
        }

        var segments = JsonPath.Parse(path);
        if(TryResolvePath(segments, out _, out _, out _)) {
            Fail(Strings.Format(StringKeys.JSON_NOT_HAVE_PATH, Expression, path), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Walks a parsed JSON path from the root, collecting the first failure reason and typo suggestion.</summary>
    /// <param name="segments">The parsed path segments.</param>
    /// <param name="node">The resolved node when the path exists.</param>
    /// <param name="reason">The localized failure reason when resolution fails.</param>
    /// <param name="suggestion">A "did you mean" suggestion for a missing property, or an empty string.</param>
    /// <returns><see langword="true"/> when every segment resolved.</returns>
    private bool TryResolvePath(IReadOnlyList<JsonPathSegment> segments, out JsonNode? node, out string reason, out string suggestion) {
        node = _value;
        reason = string.Empty;
        suggestion = string.Empty;
        var prefix = "$";
        foreach(var segment in segments) {
            if(segment.IsIndex) {
                if(!TryDescendIntoIndex(segment, ref node, ref prefix, out reason)) {
                    return false;
                }
            } else if(!TryDescendIntoProperty(segment, ref node, ref prefix, out reason, out suggestion)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>Descends one array index, failing when the node is not an array or the index is out of range.</summary>
    /// <param name="segment">The index segment.</param>
    /// <param name="node">The current node; replaced by the element on success.</param>
    /// <param name="prefix">The rendered path prefix; extended with the index on success.</param>
    /// <param name="reason">The localized failure reason when descent fails.</param>
    /// <returns><see langword="true"/> when the element was reached.</returns>
    private static bool TryDescendIntoIndex(JsonPathSegment segment, ref JsonNode? node, ref string prefix, out string reason) {
        reason = string.Empty;
        if(node is not JsonArray array) {
            reason = Strings.Format(node is null || IsJsonNull(node) ? StringKeys.JSON_PATH_IS_NULL : StringKeys.JSON_PATH_NOT_ARRAY, prefix);
            return false;
        }

        if(segment.Index >= array.Count) {
            reason = Strings.Format(StringKeys.JSON_PATH_INDEX_OUT_OF_RANGE, AssertionScope.FormatInvariant(segment.Index), prefix, AssertionScope.FormatInvariant(array.Count));
            return false;
        }

        node = array[segment.Index];
        prefix = $"{prefix}[{AssertionScope.FormatInvariant(segment.Index)}]";
        return true;
    }

    /// <summary>Descends one object property, failing when the node is not an object or the property is missing.</summary>
    /// <param name="segment">The property segment.</param>
    /// <param name="node">The current node; replaced by the property value on success.</param>
    /// <param name="prefix">The rendered path prefix; extended with the property name on success.</param>
    /// <param name="reason">The localized failure reason (with available names) when descent fails.</param>
    /// <param name="suggestion">A "did you mean" suggestion for a missing property, or an empty string.</param>
    /// <returns><see langword="true"/> when the property value was reached.</returns>
    private static bool TryDescendIntoProperty(JsonPathSegment segment, ref JsonNode? node, ref string prefix, out string reason, out string suggestion) {
        reason = string.Empty;
        suggestion = string.Empty;
        if(node is not JsonObject obj) {
            reason = Strings.Format(node is null || IsJsonNull(node) ? StringKeys.JSON_PATH_IS_NULL : StringKeys.JSON_PATH_NOT_OBJECT, prefix);
            return false;
        }

        if(!obj.TryGetPropertyValue(segment.Name!, out var child)) {
            reason = Strings.Format(StringKeys.JSON_PATH_MISSING_PROPERTY, segment.Name!, prefix) + AvailableNames(obj);
            suggestion = NearestCandidate.Describe(segment.Name, obj.Select(property => property.Key));
            return false;
        }

        node = child;
        prefix = $"{prefix}.{segment.Name}";
        return true;
    }

    /// <summary>Renders the existing property names of an object for a missing-property message.</summary>
    /// <param name="obj">The object whose names are listed.</param>
    /// <returns>A leading-space phrase with up to <c>AVAILABLE_NAMES_LIMIT</c> names, or an empty string.</returns>
    private static string AvailableNames(JsonObject obj) {
        if(obj.Count == 0) {
            return string.Empty;
        }

        var names = new List<string>();
        var truncated = false;
        foreach(var property in obj) {
            if(names.Count == AVAILABLE_NAMES_LIMIT) {
                truncated = true;
                break;
            }

            names.Add(property.Key);
        }

        var joined = string.Join(", ", names);
        if(truncated) {
            joined += ", …";
        }

        return " " + Strings.Format(StringKeys.JSON_PATH_AVAILABLE, joined);
    }

    /// <summary>Reads the node as a <see cref="DateTime"/> when it is a JSON date matching the predicate.</summary>
    /// <param name="isMatch">The predicate selecting the date kind (UTC or local).</param>
    /// <param name="value">The parsed date when the node is a matching date.</param>
    /// <returns><see langword="true"/> when the node is a matching date.</returns>
    private bool TryGetDate(Func<DateTime, bool> isMatch, out DateTime value) {
        value = default;
        return _value is JsonValue json && json.TryGetValue(out value) && isMatch(value);
    }

    /// <summary>Determines whether a node is a JSON <c>null</c> literal (distinct from a missing node).</summary>
    /// <param name="node">The node to inspect.</param>
    /// <returns><see langword="true"/> when the node is a <see cref="JsonValue"/> of kind <see cref="JsonValueKind.Null"/>.</returns>
    private static bool IsJsonNull(JsonNode node) =>
        node is JsonValue value && value.GetValueKind() == JsonValueKind.Null;

    private static string Format(string? value) => value is null ? "<null>" : $"'{value}'";

    /// <summary>Renders a compact JSON preview of a node for failure messages.</summary>
    /// <param name="node">The node to render (may be <see langword="null"/>).</param>
    /// <returns><c>&lt;null&gt;</c> or the serialized node, truncated when long.</returns>
    private static string Preview(JsonNode? node) {
        if(node is null) {
            return "<null>";
        }

        var text = node.ToJsonString();
        return text.Length > PREVIEW_LENGTH ? text.TruncateTo(PREVIEW_LENGTH) + "…" : text;
    }
}

/// <summary>Provides the same JSON assertions while preserving the closed JSON node type.</summary>
/// <typeparam name="T">The closed JSON node type under assertion.</typeparam>
/// <param name="value">The JSON node to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class JsonNodeAssertions<T>(T? value, string? expression = null) : JsonNodeAssertions(value, expression)
    where T : JsonNode {
}
