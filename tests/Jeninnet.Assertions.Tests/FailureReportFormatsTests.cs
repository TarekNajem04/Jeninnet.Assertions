//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class FailureReportFormatsTests {
    [TestMethod]
    public void Sarif_ProducesValidDocumentWithRulesAndLocation() {
        var failure = new AssertionFailure(
            "order.Total",
            "NumericAssertions",
            "Expected order.Total to be 10, but found 9.",
            "9",
            "10",
            "JNA-ORDER",
            "warning",
            "src/OrdersTests.cs",
            42,
            7);

        var sarif = FailureReport.ToSarif([failure]);

        using var document = JsonDocument.Parse(sarif);
        var root = document.RootElement;
        Assert.AreEqual("2.1.0", root.GetProperty("version").GetString());
        Assert.AreEqual("https://json.schemastore.org/sarif-2.1.0.json", root.GetProperty("$schema").GetString());
        Assert.DoesNotContain("\r", sarif);

        var run = root.GetProperty("runs")[0];
        Assert.AreEqual("Jeninnet.Assertions", run.GetProperty("tool").GetProperty("driver").GetProperty("name").GetString());

        var rule = run.GetProperty("tool").GetProperty("driver").GetProperty("rules")[0];
        Assert.AreEqual("JNA-ORDER", rule.GetProperty("id").GetString());
        Assert.AreEqual("NumericAssertions", rule.GetProperty("name").GetString());

        var result = run.GetProperty("results")[0];
        Assert.AreEqual("JNA-ORDER", result.GetProperty("ruleId").GetString());
        Assert.AreEqual("warning", result.GetProperty("level").GetString());
        Assert.AreEqual("Expected order.Total to be 10, but found 9.", result.GetProperty("message").GetProperty("text").GetString());

        var properties = result.GetProperty("properties");
        Assert.AreEqual("order.Total", properties.GetProperty("expression").GetString());
        Assert.AreEqual("9", properties.GetProperty("actual").GetString());
        Assert.AreEqual("10", properties.GetProperty("expected").GetString());

        var location = result.GetProperty("locations")[0].GetProperty("physicalLocation");
        Assert.AreEqual("src/OrdersTests.cs", location.GetProperty("artifactLocation").GetProperty("uri").GetString());
        Assert.AreEqual(42, location.GetProperty("region").GetProperty("startLine").GetInt32());
        Assert.AreEqual(7, location.GetProperty("region").GetProperty("startColumn").GetInt32());
    }

    [TestMethod]
    public void Sarif_OmitsLocationsWithoutFileAndDeduplicatesRules() {
        var first = new AssertionFailure("first", "BoolAssertions", "first message");
        var second = new AssertionFailure("second", "BoolAssertions", "second message");

        using var document = JsonDocument.Parse(FailureReport.ToSarif([first, second]));
        var run = document.RootElement.GetProperty("runs")[0];

        Assert.AreEqual(2, run.GetProperty("results").GetArrayLength());
        Assert.AreEqual(1, run.GetProperty("tool").GetProperty("driver").GetProperty("rules").GetArrayLength());
        Assert.IsFalse(run.GetProperty("results")[0].TryGetProperty("locations", out _));
    }

    [TestMethod]
    public void JUnitXml_ProducesValidXmlAndEscapesCharacters() {
        const string message = "Expected <actual> & \"expected\" 'quoted'\nsecond line";
        var failure = new AssertionFailure("order.Total", "NumericAssertions", message, "0", "1");

        var xml = FailureReport.ToJUnitXml([failure]);

        var document = XDocument.Parse(xml);
        Assert.AreEqual("1", document.Root!.Attribute("tests")!.Value);
        Assert.AreEqual("1", document.Root.Attribute("failures")!.Value);

        var suite = document.Root.Element("testsuite")!;
        Assert.AreEqual("Jeninnet.Assertions", suite.Attribute("name")!.Value);
        Assert.AreEqual("1", suite.Attribute("tests")!.Value);

        var testcase = suite.Element("testcase")!;
        Assert.AreEqual("order.Total", testcase.Attribute("name")!.Value);
        Assert.AreEqual("NumericAssertions", testcase.Attribute("classname")!.Value);

        var failureElement = testcase.Element("failure")!;
        Assert.AreEqual(message, failureElement.Attribute("message")!.Value);
        Assert.AreEqual("NumericAssertions", failureElement.Attribute("type")!.Value);
        Assert.AreEqual(message, failureElement.Value);
    }

    [TestMethod]
    public void JUnitXml_EmptyFailures_IsStillValid() {
        var document = XDocument.Parse(FailureReport.ToJUnitXml([]));
        Assert.AreEqual("0", document.Root!.Attribute("tests")!.Value);
        Assert.IsNull(document.Root.Element("testsuite")!.Element("testcase"));
    }

    [TestMethod]
    public void GitHubAnnotations_EscapesMessageAndProperties() {
        var failure = new AssertionFailure(
            "order.Total",
            "NumericAssertions",
            "100%\nnext\rline",
            file: "src/a,b:c.cs",
            line: 3,
            column: 4);

        var annotations = FailureReport.ToGitHubAnnotations([failure]);

        Assert.AreEqual("::error file=src/a%2Cb%3Ac.cs,line=3,col=4::100%25%0Anext%0Dline", annotations);
        Assert.DoesNotContain("\r", annotations);
        Assert.DoesNotContain("\n", annotations);
    }

    [TestMethod]
    public void SeverityNames_NormalizeToSarifAndGitHubLevels() {
        var note = new AssertionFailure("value", "BoolAssertions", "message", severity: "note");
        var none = new AssertionFailure("value", "BoolAssertions", "message", severity: "none");
        var info = new AssertionFailure("value", "BoolAssertions", "message", severity: "info");

        using var sarifNote = JsonDocument.Parse(FailureReport.ToSarif([note]));
        Assert.AreEqual("note", sarifNote.RootElement.GetProperty("runs")[0].GetProperty("results")[0].GetProperty("level").GetString());

        using var sarifNone = JsonDocument.Parse(FailureReport.ToSarif([none]));
        Assert.AreEqual("none", sarifNone.RootElement.GetProperty("runs")[0].GetProperty("results")[0].GetProperty("level").GetString());

        Assert.AreEqual("::notice::message", FailureReport.ToGitHubAnnotations([info]));
    }

    [TestMethod]
    public void GitHubAnnotations_UsesWarningLevelForWarningSeverity() {
        var failure = new AssertionFailure("value", "BoolAssertions", "message", severity: "warning");

        Assert.AreEqual("::warning::message", FailureReport.ToGitHubAnnotations([failure]));
    }

    [TestMethod]
    public void GitHubAnnotations_WithoutFile_UsesPlainErrorAndJoinsWithoutTrailingNewline() {
        var first = new AssertionFailure("first", "BoolAssertions", "first", line: 5);
        var second = new AssertionFailure("second", "BoolAssertions", "second");

        var annotations = FailureReport.ToGitHubAnnotations([first, second]);

        Assert.AreEqual("::error::first\n::error::second", annotations);
    }

    [TestMethod]
    public void Markdown_EscapesPipesAndLineBreaks() {
        var failure = new AssertionFailure("order|Total", "Numeric|Assertions", "a|b\nc");

        var markdown = FailureReport.ToMarkdown([failure]);

        Assert.Contains("| 1 | order\\|Total | Numeric\\|Assertions | a\\|b<br>c |", markdown);
        Assert.DoesNotContain("a|b\nc", markdown);
    }

    [TestMethod]
    public void Formats_EmptyFailures_AreValidAndDeterministic() {
        var json = FailureReport.ToJson([]);
        using var jsonDocument = JsonDocument.Parse(json);
        Assert.AreEqual(0, jsonDocument.RootElement.GetProperty("count").GetInt32());

        using var sarifDocument = JsonDocument.Parse(FailureReport.ToSarif([]));
        Assert.AreEqual(0, sarifDocument.RootElement.GetProperty("runs")[0].GetProperty("results").GetArrayLength());
        Assert.AreEqual(0, sarifDocument.RootElement.GetProperty("runs")[0].GetProperty("tool").GetProperty("driver").GetProperty("rules").GetArrayLength());

        Assert.AreEqual("0", XDocument.Parse(FailureReport.ToJUnitXml([])).Root!.Attribute("tests")!.Value);
        Assert.AreEqual(string.Empty, FailureReport.ToGitHubAnnotations([]));
        Assert.AreEqual("| # | Expression | Category | Message |\n|---:|---|---|---|", FailureReport.ToMarkdown([]));
    }

    [TestMethod]
    public void Facades_SupportEveryFormat() {
        var result = AssertionResult.Create(() => false.Should().BeTrue());
        var soft = new SoftAssertions().Collect();
        false.Should().BeTrue();

        foreach(var format in Enum.GetValues<AssertionReportFormat>()) {
            Assert.IsFalse(string.IsNullOrEmpty(result.ToReport(format)), format.ToString());
            Assert.IsFalse(string.IsNullOrEmpty(soft.ToReport(format)), format.ToString());
        }

        Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);
        Assert.AreEqual(result.ToReportJson(), result.ToReport(AssertionReportFormat.Json));
        Assert.AreEqual(FailureReport.ToSarif(result.Failures), result.ToReport(AssertionReportFormat.Sarif));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => result.ToReport((AssertionReportFormat)99));
    }

    [TestMethod]
    public void AssertionFailure_MetadataDefaultsToCategoryAndError() {
        var failure = new AssertionFailure("value", "BoolAssertions", "message");

        Assert.AreEqual("BoolAssertions", failure.RuleId);
        Assert.AreEqual("error", failure.Severity);
        Assert.IsNull(failure.File);
        Assert.IsNull(failure.Line);
        Assert.IsNull(failure.Column);
    }
}
