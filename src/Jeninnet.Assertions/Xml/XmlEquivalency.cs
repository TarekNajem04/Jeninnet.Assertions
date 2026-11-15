//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
// S1075: '/' is the XPath segment separator in diagnostic paths, not a filesystem root.
#pragma warning disable S1075
namespace Jeninnet.Assertions.Xml;

/// <summary>One structural XML difference with a simplified XPath and the expected/actual values.</summary>
/// <param name="Path">The simplified XPath of the differing node, attribute, or text.</param>
/// <param name="Expected">The expected value.</param>
/// <param name="Actual">The actual value.</param>
internal readonly record struct XmlDifference(string Path, string Expected, string Actual);

/// <summary>Walks XML trees and reports the first structural differences instead of dumping the whole markup.</summary>
internal static class XmlEquivalency {
    private const int MAX_DIFFERENCES = 10;

    /// <summary>Compares two element trees under <paramref name="options"/>.</summary>
    /// <param name="actual">The actual element (or <see langword="null"/>).</param>
    /// <param name="expected">The expected element (or <see langword="null"/>).</param>
    /// <param name="options">The equivalency options.</param>
    /// <returns>The first differences in document order, capped at ten.</returns>
    internal static IReadOnlyList<XmlDifference> Compare(XElement? actual, XElement? expected, XmlEquivalencyOptions options) {
        var differences = new List<XmlDifference>();
        CompareElement(actual, expected, differences, options);
        return differences;
    }

    /// <summary>Formats differences as a compact diagnostic list.</summary>
    /// <param name="differences">The differences to format.</param>
    /// <returns>A semicolon-separated, numbered list of paths and values.</returns>
    internal static string Format(IReadOnlyList<XmlDifference> differences) =>
        string.Join("; ", differences.Select(static (difference, index) =>
            AssertionScope.FormatInvariant(index + 1) + ". " + difference.Path
            + ": " + Strings.Get(StringKeys.XML_DIFF_EXPECTED) + " " + difference.Expected + ", " + Strings.Get(StringKeys.XML_DIFF_FOUND) + " " + difference.Actual));

    private static void CompareElement(XElement? actual, XElement? expected, List<XmlDifference> differences, XmlEquivalencyOptions options) {
        if(differences.Count >= MAX_DIFFERENCES) {
            return;
        }

        if(actual is null || expected is null) {
            if(actual is not null || expected is not null) {
                Add(differences, "/", Describe(expected), Describe(actual));
            }

            return;
        }

        var path = PathOf(actual);
        if(actual.Name != expected.Name) {
            Add(differences, path, expected.Name.ToString(), actual.Name.ToString());
            return;
        }

        CompareAttributes(actual, expected, path, differences);
        CompareDirectText(actual, expected, path, differences, options);
        CompareComments(actual, expected, path, differences, options);
        CompareChildren(actual, expected, path, differences, options);
    }

    private static void CompareAttributes(XElement actual, XElement expected, string path, List<XmlDifference> differences) {
        var actualAttributes = actual.Attributes().ToDictionary(static attribute => attribute.Name, static attribute => attribute.Value);
        var expectedAttributes = expected.Attributes().ToDictionary(static attribute => attribute.Name, static attribute => attribute.Value);
        var names = actualAttributes.Keys.Union(expectedAttributes.Keys)
            .OrderBy(static name => name.ToString(), StringComparer.Ordinal);

        foreach(var name in names) {
            if(differences.Count >= MAX_DIFFERENCES) {
                return;
            }

            var attributePath = path + "/@" + name.LocalName;
            var hasActual = actualAttributes.TryGetValue(name, out var actualValue);
            var hasExpected = expectedAttributes.TryGetValue(name, out var expectedValue);
            if(!hasActual) {
                Add(differences, attributePath, Quote(expectedValue ?? ""), Missing());
            } else if(!hasExpected) {
                Add(differences, attributePath, Missing(), Quote(actualValue ?? ""));
            } else if(!string.Equals(actualValue, expectedValue, StringComparison.Ordinal)) {
                Add(differences, attributePath, Quote(expectedValue ?? ""), Quote(actualValue ?? ""));
            }
        }
    }

    private static void CompareDirectText(XElement actual, XElement expected, string path, List<XmlDifference> differences, XmlEquivalencyOptions options) {
        if(differences.Count >= MAX_DIFFERENCES) {
            return;
        }

        var actualText = DirectText(actual, options.IgnoreWhitespace);
        var expectedText = DirectText(expected, options.IgnoreWhitespace);
        if(!string.Equals(actualText, expectedText, StringComparison.Ordinal)) {
            Add(differences, path + "/text()", Quote(expectedText), Quote(actualText));
        }
    }

    private static void CompareComments(XElement actual, XElement expected, string path, List<XmlDifference> differences, XmlEquivalencyOptions options) {
        if(options.IgnoreComments || differences.Count >= MAX_DIFFERENCES) {
            return;
        }

        var actualComments = actual.Nodes().OfType<XComment>().Select(static comment => comment.Value).ToList();
        var expectedComments = expected.Nodes().OfType<XComment>().Select(static comment => comment.Value).ToList();
        for(var index = 0; index < Math.Max(actualComments.Count, expectedComments.Count); index++) {
            if(differences.Count >= MAX_DIFFERENCES) {
                return;
            }

            var commentPath = path + "/comment()";
            var actualComment = index < actualComments.Count ? actualComments[index] : null;
            var expectedComment = index < expectedComments.Count ? expectedComments[index] : null;
            if(!string.Equals(actualComment, expectedComment, StringComparison.Ordinal)) {
                Add(differences, commentPath, expectedComment is null ? Missing() : Quote(expectedComment), actualComment is null ? Missing() : Quote(actualComment));
            }
        }
    }

    private static void CompareChildren(XElement actual, XElement expected, string path, List<XmlDifference> differences, XmlEquivalencyOptions options) {
        var actualChildren = actual.Elements().ToList();
        var expectedChildren = expected.Elements().ToList();
        if(options.IgnoreElementOrder) {
            CompareChildrenUnordered(actualChildren, expectedChildren, path, differences, options);
            return;
        }

        for(var index = 0; index < Math.Max(actualChildren.Count, expectedChildren.Count); index++) {
            if(differences.Count >= MAX_DIFFERENCES) {
                return;
            }

            var actualChild = index < actualChildren.Count ? actualChildren[index] : null;
            var expectedChild = index < expectedChildren.Count ? expectedChildren[index] : null;
            if(actualChild is null) {
                Add(differences, path + "/" + expectedChild!.Name.LocalName, Describe(expectedChild), Missing());
            } else if(expectedChild is null) {
                Add(differences, path + "/" + actualChild.Name.LocalName, Missing(), Describe(actualChild));
            } else {
                CompareElement(actualChild, expectedChild, differences, options);
            }
        }
    }

    private static void CompareChildrenUnordered(
        List<XElement> actualChildren,
        List<XElement> expectedChildren,
        string path,
        List<XmlDifference> differences,
        XmlEquivalencyOptions options) {
        var names = actualChildren.Select(static child => child.Name)
            .Union(expectedChildren.Select(static child => child.Name))
            .OrderBy(static name => name.ToString(), StringComparer.Ordinal);

        foreach(var name in names) {
            if(differences.Count >= MAX_DIFFERENCES) {
                return;
            }

            var actualGroup = actualChildren.Where(child => child.Name == name).ToList();
            var expectedGroup = expectedChildren.Where(child => child.Name == name).ToList();
            if(actualGroup.Count != expectedGroup.Count) {
                Add(differences, path + "/" + name.LocalName, DescribeCount(expectedGroup.Count), DescribeCount(actualGroup.Count));
            }

            for(var index = 0; index < Math.Min(actualGroup.Count, expectedGroup.Count); index++) {
                CompareElement(actualGroup[index], expectedGroup[index], differences, options);
            }
        }
    }

    private static string PathOf(XElement element) {
        var segments = new List<string>();
        for(var current = element; current is not null; current = current.Parent) {
            var name = current.Name.LocalName;
            var parent = current.Parent;
            if(parent is not null) {
                var sameName = parent.Elements(current.Name).ToList();
                if(sameName.Count > 1) {
                    name += "[" + AssertionScope.FormatInvariant(sameName.IndexOf(current) + 1) + "]";
                }
            }

            segments.Add(name);
        }

        segments.Reverse();
        return "/" + string.Join("/", segments);
    }

    private static string DirectText(XElement element, bool ignoreWhitespace) {
        var text = string.Concat(element.Nodes().OfType<XText>().Select(static node => node.Value));
        return ignoreWhitespace ? NormalizeWhitespace(text) : text;
    }

    private static string NormalizeWhitespace(string value) =>
        string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static string Describe(XElement? element) => element is null ? "<null>" : "<" + element.Name.LocalName + ">";

    private static string DescribeCount(int count) => AssertionScope.FormatInvariant(count);

    private static string Missing() => Strings.Get(StringKeys.XML_DIFF_MISSING);

    private static string Quote(string value) => "'" + value + "'";

    private static void Add(List<XmlDifference> differences, string path, string expected, string actual) {
        if(differences.Count < MAX_DIFFERENCES) {
            differences.Add(new XmlDifference(path, expected, actual));
        }
    }
}
#pragma warning restore S1075
