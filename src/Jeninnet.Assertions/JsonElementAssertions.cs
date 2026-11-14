//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="JsonElement"/> and <see cref="JsonDocument"/> values.</summary>
/// <remarks>
/// The element is snapshotted into a <see cref="JsonNode"/> when the assertion object is created, so the
/// underlying <see cref="JsonDocument"/> must stay alive until <c>Should()</c> returns. Assertion syntax
/// and failure messages intentionally match <see cref="JsonNodeAssertions"/>.
/// </remarks>
/// <param name="value">The JSON element to assert on (a <see langword="null"/> value represents a missing document).</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class JsonElementAssertions(JsonElement? value, string? expression = null) : AssertionsBase(expression, "JSON element") {
    private static readonly JsonElement _jsonNull = JsonSerializer.SerializeToElement<object?>(null);

    private readonly JsonNode? _node = ParseNode(value, expression);

    private JsonNodeAssertions Node => new(_node, Expression);

    /// <summary>Asserts that the element is a JSON null or a missing document.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element holds a non-null JSON value.</exception>
    public AndConstraint<JsonElementAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        _ = Node.BeNull(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element holds a non-null JSON value.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is missing or a JSON null.</exception>
    public AndConstraint<JsonElementAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeNull(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON object.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON object.</exception>
    public AndConstraint<JsonElementAssertions> BeObject(string? because = null, params object?[] becauseArgs) {
        _ = Node.BeObject(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is not a JSON object.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is a JSON object.</exception>
    public AndConstraint<JsonElementAssertions> NotBeObject(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeObject(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON array.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON array.</exception>
    public AndConstraint<JsonElementAssertions> BeAnArray(string? because = null, params object?[] becauseArgs) {
        _ = Node.BeAnArray(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is not a JSON array.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is a JSON array.</exception>
    public AndConstraint<JsonElementAssertions> NotBeAnArray(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeAnArray(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON string and exposes its value.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the string value.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON string.</exception>
    public WhichConstraint<string?> BeString(string? because = null, params object?[] becauseArgs) =>
        new(Node.BeString(because, becauseArgs).Which);

    /// <summary>Asserts that the element is not a JSON string.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is a JSON string.</exception>
    public AndConstraint<JsonElementAssertions> NotBeString(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeString(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON number and exposes its text.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the number text.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON number.</exception>
    public WhichConstraint<string?> BeNumeric(string? because = null, params object?[] becauseArgs) =>
        new(Node.BeNumeric(because, becauseArgs).Which);

    /// <summary>Asserts that the element is not a JSON number.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is a JSON number.</exception>
    public AndConstraint<JsonElementAssertions> NotBeNumeric(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeNumeric(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON boolean and exposes its value.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the boolean value.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON boolean.</exception>
    public WhichConstraint<bool> BeBool(string? because = null, params object?[] becauseArgs) =>
        new(Node.BeBool(because, becauseArgs).Which);

    /// <summary>Asserts that the element is not a JSON boolean.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is a JSON boolean.</exception>
    public AndConstraint<JsonElementAssertions> NotBeBool(string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeBool(because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is structurally equivalent to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected JSON element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are not equivalent.</exception>
    public AndConstraint<JsonElementAssertions> BeEquivalentTo(JsonElement expected, string? because = null, params object?[] becauseArgs) {
        _ = Node.BeEquivalentTo(JsonNode.Parse(expected.GetRawText()), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is not structurally equivalent to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The unexpected JSON element.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The elements are equivalent.</exception>
    public AndConstraint<JsonElementAssertions> NotBeEquivalentTo(JsonElement unexpected, string? because = null, params object?[] becauseArgs) {
        _ = Node.NotBeEquivalentTo(JsonNode.Parse(unexpected.GetRawText()), because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON array with exactly <paramref name="expected"/> elements.</summary>
    /// <param name="expected">The expected element count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element is not a JSON array or its count differs.</exception>
    public AndConstraint<JsonElementAssertions> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        _ = Node.HaveCount(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element holds the scalar <paramref name="expected"/> value.</summary>
    /// <typeparam name="T">The scalar type.</typeparam>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The element does not hold <paramref name="expected"/>.</exception>
    public AndConstraint<JsonElementAssertions> HaveValue<T>(T? expected, string? because = null, params object?[] becauseArgs) {
        _ = Node.HaveValue(expected, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the element is a JSON object with property <paramref name="name"/> and exposes it.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the property value (a JSON null becomes a null-kind element).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is not a JSON object or lacks <paramref name="name"/>.</exception>
    public WhichConstraint<JsonElement> HaveProperty(string name, string? because = null, params object?[] becauseArgs) =>
        new(ToElement(Node.HaveProperty(name, because, becauseArgs).Which));

    /// <summary>Asserts that the element is a JSON object without the specified property.</summary>
    /// <param name="name">The property name that must not be present.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The element is a JSON object with the specified property.</exception>
    public AndConstraint<JsonElementAssertions> NotHaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        _ = Node.NotHaveProperty(name, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the mini JSON path resolves to exactly one element and exposes it.</summary>
    /// <remarks>The supported syntax is documented on <see cref="JsonNodeAssertions.HaveJsonPath"/>.</remarks>
    /// <param name="path">The path to resolve.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the resolved element (which may be a JSON null).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is malformed.</exception>
    /// <exception cref="AssertionFailedException">The path does not resolve or resolves to multiple elements.</exception>
    public WhichConstraint<JsonElement> HaveJsonPath(string path, string? because = null, params object?[] becauseArgs) =>
        new(ToElement(Node.HaveJsonPath(path, because, becauseArgs).Which));

    /// <summary>Asserts that the mini JSON path resolves to one or more elements and exposes every match.</summary>
    /// <remarks>The supported syntax is documented on <see cref="JsonNodeAssertions.HaveJsonPath"/>.</remarks>
    /// <param name="path">The path to resolve.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping every match in document order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is malformed.</exception>
    /// <exception cref="AssertionFailedException">The path does not resolve to any element.</exception>
    public WhichConstraint<IReadOnlyList<JsonElement>> HaveJsonPathValues(string path, string? because = null, params object?[] becauseArgs) =>
        new([.. Node.HaveJsonPathValues(path, because, becauseArgs).Which.Select(ToElement)]);

    /// <summary>Asserts that the mini JSON path does not resolve against the element.</summary>
    /// <remarks>The supported syntax is documented on <see cref="JsonNodeAssertions.HaveJsonPath"/>.</remarks>
    /// <param name="path">The path that must not resolve.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is malformed.</exception>
    /// <exception cref="AssertionFailedException">The path resolves.</exception>
    public AndConstraint<JsonElementAssertions> NotHaveJsonPath(string path, string? because = null, params object?[] becauseArgs) {
        _ = Node.NotHaveJsonPath(path, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the deterministic JSON representation matches the committed snapshot.</summary>
    /// <remarks>
    /// Object keys are sorted ordinally, arrays keep their order, indentation is two spaces, and line
    /// endings are LF. A missing baseline writes a <c>.received</c> file for review; mismatches write a
    /// diff. Set <c>JENINNET_SNAPSHOT_UPDATE=1</c> to promote a received file explicitly.
    /// </remarks>
    /// <param name="snapshotName">The snapshot name (<c>{name}.verified.json</c>), or <see langword="null"/> to derive it from the subject expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to the calling source file.</param>
    /// <param name="config">Optional snapshot configuration (explicit scrubbers).</param>
    /// <param name="callerFilePath">The calling source file (provided by the compiler).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException">The snapshot name is empty or unsafe.</exception>
    /// <exception cref="AssertionFailedException">No verified snapshot exists, or the content differs.</exception>
    public AndConstraint<JsonElementAssertions> MatchSnapshot(
        string? snapshotName = null,
        string? snapshotsDirectory = null,
        Func<SnapshotOptions, SnapshotOptions>? config = null,
        [CallerFilePath] string? callerFilePath = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        var json = _node is null ? "null" : _node.ToJsonString(Snapshot.JsonOptions);
        Snapshot.Verify(json.Replace("\r\n", "\n").Replace('\r', '\n'), snapshotName, snapshotsDirectory, ".json", options, new(Expression, callerFilePath, because, becauseArgs));
        return new(this);
    }

    /// <summary>Parses a JSON element into a node so both models share one assertion engine.</summary>
    /// <param name="value">The element to parse.</param>
    /// <param name="expression">The captured subject expression, used in the disposed-document message.</param>
    /// <returns>The parsed node, or <see langword="null"/> for a missing or JSON null element.</returns>
    /// <exception cref="AssertionFailedException">The element comes from a disposed document.</exception>
    private static JsonNode? ParseNode(JsonElement? value, string? expression) {
        if(value is null) {
            return null;
        }

        try {
            return JsonNode.Parse(value.Value.GetRawText());
        }
        catch(ObjectDisposedException) {
            throw new AssertionFailedException(Strings.Format(StringKeys.JSON_DOCUMENT_DISPOSED, expression ?? "JSON element"));
        }
    }

    /// <summary>Converts a node back into an independent JSON element.</summary>
    /// <param name="node">The node to convert (a <see langword="null"/> node becomes a JSON null element).</param>
    /// <returns>The converted element.</returns>
    private static JsonElement ToElement(JsonNode? node) {
        if(node is null) {
            return _jsonNull;
        }

        using var document = JsonDocument.Parse(node.ToJsonString());
        return document.RootElement.Clone();
    }

}
