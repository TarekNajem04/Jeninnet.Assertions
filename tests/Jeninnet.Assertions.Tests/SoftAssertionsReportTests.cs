//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SoftAssertionsReportTests {
    [TestMethod]
    public void Report_StructuredFailure_CapturesExpressionCategoryAndValues() {
        var soft = new SoftAssertions().Collect();
        const bool flag = false;
        flag.Should().BeTrue();

        Assert.HasCount(1, soft.Failures);
        var failure = soft.Failures[0];
        Assert.AreEqual("flag", failure.Expression);
        Assert.AreEqual("BoolAssertions", failure.Category);
        Assert.AreEqual("Expected flag to be true, but found false.", failure.Message);
        Assert.AreEqual("false", failure.Actual);
        Assert.AreEqual("true", failure.Expected);
    }

    [TestMethod]
    public void Report_Json_IncludesStructuredFields() {
        var soft = new SoftAssertions().Collect();
        const bool flag = false;
        flag.Should().BeTrue();
        "abc".Should().Be("xyz");

        var json = soft.ToReportJson();
        var root = JsonNode.Parse(json)!.AsObject();
        Assert.AreEqual(2, root["count"]!.AsValue().GetValue<int>());

        var failures = root["failures"]!.AsArray();
        var boolEntry = failures[0]!.AsObject();
        Assert.AreEqual("flag", boolEntry["expression"]!.GetValue<string>());
        Assert.AreEqual("BoolAssertions", boolEntry["category"]!.GetValue<string>());
        Assert.AreEqual("false", boolEntry["actual"]!.GetValue<string>());
        Assert.AreEqual("true", boolEntry["expected"]!.GetValue<string>());

        var stringEntry = failures[1]!.AsObject();
        Assert.AreEqual("StringAssertions", stringEntry["category"]!.GetValue<string>());
        Assert.IsNull(stringEntry["actual"]);
        Assert.IsNull(stringEntry["expected"]);
        Assert.AreEqual("\"abc\"", stringEntry["expression"]!.GetValue<string>());
    }

    [TestMethod]
    public void Report_StringCollectionFailure_CapturesExpressionAndCategory() {
        var soft = new SoftAssertions().Collect();
        IEnumerable<string> words = ["a", "b"];
        words.Should().AllBe("a");

        Assert.HasCount(1, soft.Failures);
        var failure = soft.Failures[0];
        Assert.AreEqual("words", failure.Expression);
        Assert.AreEqual("StringCollectionAssertions", failure.Category);
    }

    [TestMethod]
    public void Report_Failures_EmptyBeforeCollectAndAfterReport() {
        var soft = new SoftAssertions();
        Assert.IsEmpty(soft.Failures);

        soft.Collect();
        const bool flag = false;
        flag.Should().BeTrue();
        Assert.HasCount(1, soft.Failures);

        Assert.ThrowsExactly<AssertionFailedException>(soft.ThenReport);
        Assert.IsEmpty(soft.Failures);
    }

    [TestMethod]
    public void Report_Json_EmptyWhenClean() {
        var soft = new SoftAssertions().Collect();
        true.Should().BeTrue();
        "a".Should().Be("a");
        soft.ThenReport();

        var root = JsonNode.Parse(soft.ToReportJson())!.AsObject();
        Assert.AreEqual(0, root["count"]!.AsValue().GetValue<int>());
        Assert.AreEqual(0, root["failures"]!.AsArray().Count);
    }

    [TestMethod]
    public void Report_Json_EmptyBeforeCollectWithoutThrowing() {
        var soft = new SoftAssertions();

        var root = JsonNode.Parse(soft.ToReportJson())!.AsObject();
        Assert.AreEqual(0, root["count"]!.AsValue().GetValue<int>());
    }

    [TestMethod]
    public void Report_Json_EscapesSpecialCharacters() {
        var soft = new SoftAssertions().Collect();
        const bool flag = false;
        flag.Should().BeTrue("because \"quoted\"\nline");

        var json = soft.ToReportJson();
        var root = JsonNode.Parse(json)!.AsObject();
        var message = root["failures"]![0]!["message"]!.GetValue<string>();
        Assert.AreEqual("Expected flag to be true, but found false because \"quoted\"\nline.", message);
    }
}
