//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Snapshots;

/// <summary>Normalization, JSON key sorting, and object-activated scrubbing for snapshots.</summary>
internal static class SnapshotComparison {
    private static readonly JsonSerializerOptions _indentedJson = new() { WriteIndented = true };

    /// <summary>Formats received content for storage and comparison.</summary>
    /// <param name="text">The received text after global scrubbers.</param>
    /// <param name="options">The snapshot options.</param>
    /// <param name="fileExtension">The snapshot extension, including the dot.</param>
    /// <returns>The formatted text (LF for JSON/XML, sorted/reindented when requested).</returns>
    internal static string FormatForStorage(string text, SnapshotOptions options, string fileExtension) {
        var result = text;
        if(options.SortPropertiesEnabled && IsExtension(fileExtension, ".json")) {
            result = SortJson(result);
        }

        if(options.IndentSizeValue != 2) {
            result = Reindent(result, options.IndentSizeValue);
        }

        return IsExtension(fileExtension, ".json") || IsExtension(fileExtension, ".xml")
            ? NormalizeLineEndings(result)
            : result;
    }

    /// <summary>Normalizes text for comparison according to line-ending and whitespace options.</summary>
    /// <param name="text">The text to normalize.</param>
    /// <param name="options">The snapshot options.</param>
    /// <param name="fileExtension">The snapshot extension, including the dot.</param>
    /// <returns>The comparable text.</returns>
    internal static string NormalizeForComparison(string text, SnapshotOptions options, string fileExtension) {
        var result = FormatForStorage(text, options, fileExtension);
        if(options.IgnoreLineEndingDifferencesEnabled) {
            result = NormalizeLineEndings(result);
        }

        return options.IgnoreWhiteSpaceDifferencesEnabled ? CollapseWhitespace(result) : result;
    }

    /// <summary>Applies root-type and root-member scrubbers to serialized object content.</summary>
    /// <remarks>Type membership is checked with <see cref="Type.IsInstanceOfType"/>; member activation checks
    /// public instance/static members. Both scrubbers operate on the serialized text, not individual graph nodes.</remarks>
    /// <param name="text">The serialized text.</param>
    /// <param name="value">The root object, when available.</param>
    /// <param name="options">The snapshot options.</param>
    /// <returns>The scrubbed text.</returns>
    internal static string ApplyObjectScrubbers(string text, object? value, SnapshotOptions options) {
        if(value is null) {
            return text;
        }

        var result = text;
        foreach(var (type, scrubber) in options.TypeScrubbers) {
            if(type.IsInstanceOfType(value)) {
                result = scrubber(result);
            }
        }

        var runtimeType = value.GetType();
        foreach(var (member, scrubber) in options.MemberScrubbers) {
            if(runtimeType.GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Length != 0) {
                result = scrubber(result);
            }
        }

        return result;
    }

    /// <summary>Determines whether a file extension matches, case-insensitively.</summary>
    /// <param name="actual">The actual extension.</param>
    /// <param name="expected">The expected extension.</param>
    /// <returns><see langword="true"/> when the extensions match.</returns>
    private static bool IsExtension(string actual, string expected) =>
        string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);

    /// <summary>Normalizes CRLF and CR to LF.</summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The LF-only text.</returns>
    private static string NormalizeLineEndings(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n');

    /// <summary>Collapses every whitespace run to a single space and trims.</summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The whitespace-collapsed text.</returns>
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

    /// <summary>Sorts JSON object keys recursively and re-serializes with two-space indentation.</summary>
    /// <param name="text">The JSON text.</param>
    /// <returns>The sorted JSON, or the original text when parsing fails.</returns>
    private static string SortJson(string text) {
        try {
            var node = JsonNode.Parse(text);
            var sorted = SortKeys(node);
            return sorted is null ? "null" : sorted.ToJsonString(_indentedJson);
        }
        catch(JsonException) {
            return text;
        }
    }

    /// <summary>Recursively rebuilds JSON objects with ordinally sorted keys.</summary>
    /// <param name="node">The node to sort.</param>
    /// <returns>A sorted copy of the node.</returns>
    private static JsonNode? SortKeys(JsonNode? node) {
        switch(node) {
            case JsonObject obj:
                var sorted = new JsonObject();
                foreach(var property in obj.OrderBy(static property => property.Key, StringComparer.Ordinal)) {
                    sorted[property.Key] = SortKeys(property.Value);
                }

                return sorted;
            case JsonArray array:
                var copied = new JsonArray();
                foreach(var item in array) {
                    copied.Add(SortKeys(item));
                }

                return copied;
            default:
                return node?.DeepClone();
        }
    }

    /// <summary>Adjusts leading-space indentation from the serializer's two-space convention.</summary>
    /// <param name="text">The text to reindent.</param>
    /// <param name="spaces">The desired number of spaces per level.</param>
    /// <returns>The reindented text with LF line endings.</returns>
    private static string Reindent(string text, int spaces) {
        var lines = NormalizeLineEndings(text).Split('\n');
        for(var index = 0; index < lines.Length; index++) {
            var line = lines[index];
            var leading = 0;
            while(leading < line.Length && line[leading] == ' ') {
                leading++;
            }

            if(leading != 0) {
                var replacement = new StringBuilder(line.Length);
                replacement.Append(' ', leading / 2 * spaces);
                replacement.Append(line, leading, line.Length - leading);
                lines[index] = replacement.ToString();
            }
        }

        return string.Join("\n", lines);
    }
}
