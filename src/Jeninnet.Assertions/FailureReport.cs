//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Builds structured CI reports from collected <see cref="AssertionFailure"/> entries.</summary>
/// <remarks>
/// JSON is the original compact shape and remains stable. SARIF, JUnit XML, GitHub annotations, and Markdown
/// are deterministic (no timestamps, LF line endings) and never scrub values: callers are responsible for keeping
/// sensitive data out of <see cref="AssertionFailure.Message"/>/<see cref="AssertionFailure.Actual"/>/
/// <see cref="AssertionFailure.Expected"/>.
/// </remarks>
public static class FailureReport {
    private static readonly JsonSerializerOptions _indentedJson = new() { WriteIndented = true };

    /// <summary>Serializes <paramref name="failures"/> into the compact JSON document used by
    /// <see cref="SoftAssertions.ToReportJson"/> and <see cref="AssertionResult.ToReportJson"/>.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>The JSON report.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    public static string ToJson(IReadOnlyList<AssertionFailure> failures) {
        Validate(failures);
        var array = new JsonArray();
        foreach(var failure in failures) {
            Add(array, new JsonObject {
                ["expression"] = failure.Expression,
                ["category"] = failure.Category,
                ["message"] = failure.Message,
                ["actual"] = failure.Actual,
                ["expected"] = failure.Expected,
            });
        }

        var root = new JsonObject {
            ["count"] = failures.Count,
            ["failures"] = array,
        };

        return root.ToJsonString();
    }

    /// <summary>Serializes <paramref name="failures"/> as SARIF 2.1.0.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>Indented SARIF JSON with LF line endings.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    public static string ToSarif(IReadOnlyList<AssertionFailure> failures) {
        Validate(failures);

        var rules = new JsonArray();
        var seenRules = new HashSet<string>(StringComparer.Ordinal);
        foreach(var failure in failures) {
            if(!seenRules.Add(failure.RuleId)) {
                continue;
            }

            Add(rules, new JsonObject {
                ["id"] = failure.RuleId,
                ["name"] = failure.Category,
                ["shortDescription"] = new JsonObject { ["text"] = failure.Category },
            });
        }

        var results = new JsonArray();
        foreach(var failure in failures) {
            var properties = new JsonObject {
                ["expression"] = failure.Expression,
                ["actual"] = failure.Actual,
                ["expected"] = failure.Expected,
            };
            var result = new JsonObject {
                ["ruleId"] = failure.RuleId,
                ["level"] = SarifLevel(failure.Severity),
                ["message"] = new JsonObject { ["text"] = failure.Message },
                ["properties"] = properties,
            };

            if(Located(failure, includeLine: false) is { } location) {
                var locations = new JsonArray();
                Add(locations, location);
                result["locations"] = locations;
            }

            Add(results, result);
        }

        var runs = new JsonArray();
        Add(runs, new JsonObject {
            ["tool"] = new JsonObject {
                ["driver"] = new JsonObject {
                    ["name"] = "Jeninnet.Assertions",
                    ["informationUri"] = "https://github.com/TarekNajem04/Jeninnet.Assertions",
                    ["rules"] = rules,
                },
            },
            ["results"] = results,
        });

        var root = new JsonObject {
            ["$schema"] = "https://json.schemastore.org/sarif-2.1.0.json",
            ["version"] = "2.1.0",
            ["runs"] = runs,
        };

        return ReportEscaping.NormalizeLineEndings(root.ToJsonString(_indentedJson));
    }

    /// <summary>Serializes <paramref name="failures"/> as JUnit XML.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>JUnit XML with LF line endings and no timestamp.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    public static string ToJUnitXml(IReadOnlyList<AssertionFailure> failures) {
        Validate(failures);

        var suite = new XElement(
            "testsuite",
            new XAttribute("name", "Jeninnet.Assertions"),
            new XAttribute("tests", failures.Count),
            new XAttribute("failures", failures.Count),
            new XAttribute("errors", 0),
            new XAttribute("skipped", 0));
        foreach(var failure in failures) {
            var failureElement = new XElement(
                "failure",
                new XAttribute("message", failure.Message),
                new XAttribute("type", failure.RuleId),
                failure.Message);
            suite.Add(new XElement(
                "testcase",
                new XAttribute("name", failure.Expression),
                new XAttribute("classname", failure.Category),
                failureElement));
        }

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "testsuites",
                new XAttribute("tests", failures.Count),
                new XAttribute("failures", failures.Count),
                new XAttribute("errors", 0),
                new XAttribute("skipped", 0),
                suite));

        return ReportEscaping.NormalizeLineEndings(document.ToString());
    }

    /// <summary>Serializes <paramref name="failures"/> as GitHub Actions workflow commands.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>One escaped annotation per failure, joined with LF; empty when no failures exist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    public static string ToGitHubAnnotations(IReadOnlyList<AssertionFailure> failures) {
        Validate(failures);

        var lines = new List<string>();
        foreach(var failure in failures) {
            var message = ReportEscaping.GitHubData(failure.Message);
            var level = GitHubLevel(failure.Severity);
            var file = failure.File;
            if(file is null || file.Trim().Length == 0) {
                lines.Add($"::{level}::{message}");
                continue;
            }

            var properties = " file=" + ReportEscaping.GitHubProperty(file.Trim().Replace('\\', '/'));
            if(failure.Line is > 0) {
                properties += ",line=" + AssertionScope.FormatInvariant(failure.Line.Value);
            }

            if(failure.Column is > 0) {
                properties += ",col=" + AssertionScope.FormatInvariant(failure.Column.Value);
            }

            lines.Add($"::{level}{properties}::{message}");
        }

        return string.Join("\n", lines);
    }

    /// <summary>Serializes <paramref name="failures"/> as a Markdown table.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <returns>A Markdown table with LF line endings; empty failures still produce the header.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    public static string ToMarkdown(IReadOnlyList<AssertionFailure> failures) {
        Validate(failures);

        var lines = new List<string> {
            "| # | Expression | Category | Message |",
            "|---:|---|---|---|",
        };
        for(var index = 0; index < failures.Count; index++) {
            var failure = failures[index];
            lines.Add("| " + AssertionScope.FormatInvariant(index + 1)
                + " | " + ReportEscaping.MarkdownCell(failure.Expression)
                + " | " + ReportEscaping.MarkdownCell(failure.Category)
                + " | " + ReportEscaping.MarkdownCell(failure.Message) + " |");
        }

        return string.Join("\n", lines);
    }

    /// <summary>Serializes <paramref name="failures"/> in the requested format.</summary>
    /// <param name="failures">The structured failures to serialize.</param>
    /// <param name="format">The target report format.</param>
    /// <returns>The report text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not a defined report format.</exception>
    public static string ToReport(IReadOnlyList<AssertionFailure> failures, AssertionReportFormat format) {
        Validate(failures);
        return format switch {
            AssertionReportFormat.Json => ToJson(failures),
            AssertionReportFormat.Sarif => ToSarif(failures),
            AssertionReportFormat.JUnitXml => ToJUnitXml(failures),
            AssertionReportFormat.GitHubAnnotations => ToGitHubAnnotations(failures),
            AssertionReportFormat.Markdown => ToMarkdown(failures),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown assertion report format."),
        };
    }

    /// <summary>Adds a node through the non-generic <see cref="JsonArray"/> contract (AOT-safe).</summary>
    /// <param name="array">The target array.</param>
    /// <param name="node">The node to add.</param>
    private static void Add(JsonArray array, JsonNode node) => ((IList<JsonNode?>)array).Add(node);

    /// <summary>Validates the failure list argument.</summary>
    /// <param name="failures">The failures to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <see langword="null"/>.</exception>
    private static void Validate(IReadOnlyList<AssertionFailure> failures) {
        if(failures is null) {
            throw new ArgumentNullException(nameof(failures));
        }
    }

    /// <summary>Builds a SARIF physical location when a source file is available.</summary>
    /// <param name="failure">The failure whose location is requested.</param>
    /// <param name="includeLine">Whether to include a line region even without a line number.</param>
    /// <returns>The SARIF location, or <see langword="null"/> when no file is known.</returns>
    private static JsonObject? Located(AssertionFailure failure, bool includeLine) {
        var file = failure.File;
        if(file is null || file.Trim().Length == 0) {
            return null;
        }

        var physical = new JsonObject {
            ["artifactLocation"] = new JsonObject { ["uri"] = file.Trim().Replace('\\', '/') },
        };
        if(failure.Line is > 0 || includeLine) {
            var region = new JsonObject { ["startLine"] = failure.Line is > 0 ? failure.Line.Value : 1 };
            if(failure.Column is > 0) {
                region["startColumn"] = failure.Column.Value;
            }

            physical["region"] = region;
        }

        return new JsonObject { ["physicalLocation"] = physical };
    }

    /// <summary>Normalizes a severity to a SARIF level.</summary>
    /// <param name="severity">The failure severity.</param>
    /// <returns><c>error</c>, <c>warning</c>, <c>note</c>, or <c>none</c>.</returns>
    private static string SarifLevel(string severity) =>
        severity.Trim().ToLowerInvariant() switch {
            "warning" => "warning",
            "note" or "info" => "note",
            "none" => "none",
            _ => "error",
        };

    /// <summary>Normalizes a severity to a GitHub annotation level.</summary>
    /// <param name="severity">The failure severity.</param>
    /// <returns><c>error</c>, <c>warning</c>, or <c>notice</c>.</returns>
    private static string GitHubLevel(string severity) =>
        severity.Trim().ToLowerInvariant() switch {
            "warning" => "warning",
            "note" or "notice" or "info" => "notice",
            _ => "error",
        };
}
