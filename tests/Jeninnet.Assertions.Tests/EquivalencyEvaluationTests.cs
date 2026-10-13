//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class EquivalencyEvaluationTests {
    private static readonly DateTime _epoch = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [TestMethod]
    public void Equivalent_ReturnsVerdictWithoutMessageOrDifferences() {
        var expected = new Shipment { Amount = 10.00m, CreatedAt = _epoch };
        var actual = new Shipment { Amount = 10.00m, CreatedAt = _epoch };

        var result = EquivalencyEvaluation.Evaluate(actual, expected);

        Assert.IsTrue(result.IsEquivalent);
        Assert.IsEmpty(result.Differences);
        Assert.AreEqual(string.Empty, result.Message);

        var json = JsonNode.Parse(result.ToReportJson())!.AsObject();
        Assert.IsTrue(json["isEquivalent"]!.GetValue<bool>());
        Assert.AreEqual(0, json["count"]!.GetValue<int>());
        Assert.AreEqual(0, json["differences"]!.AsArray().Count);
    }

    [TestMethod]
    public void Differences_AreStructured_AndOrderedOrdinally() {
        var expected = new Shipment { CreatedAt = _epoch, UpdatedAt = _epoch };
        var actual = new Shipment { CreatedAt = _epoch.AddSeconds(2), UpdatedAt = _epoch.AddSeconds(5) };

        var result = EquivalencyEvaluation.Evaluate(actual, expected);

        Assert.IsFalse(result.IsEquivalent);
        Assert.HasCount(2, result.Differences);

        var createdAt = result.Differences[0];
        Assert.AreEqual("CreatedAt", createdAt.Path);
        Assert.AreEqual("CreatedAt was 01/01/2026 00:00:02 instead of 01/01/2026 00:00:00.", createdAt.Message);
        Assert.AreEqual("01/01/2026 00:00:00", createdAt.Expected);
        Assert.AreEqual("01/01/2026 00:00:02", createdAt.Actual);

        var updatedAt = result.Differences[1];
        Assert.AreEqual("UpdatedAt", updatedAt.Path);
        Assert.AreEqual("01/01/2026 00:00:05", updatedAt.Actual);

        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "Expected the graphs to be equivalent, but found 2 difference(s):",
                "1. CreatedAt was 01/01/2026 00:00:02 instead of 01/01/2026 00:00:00.",
                "2. UpdatedAt was 01/01/2026 00:00:05 instead of 01/01/2026 00:00:00."),
            result.Message);
    }

    [TestMethod]
    public void NestedMembers_AnnotateThePath() {
        var expected = new Parcel { Address = new Address { City = "Berlin" } };
        var actual = new Parcel { Address = new Address { City = "Paris" } };

        var result = EquivalencyEvaluation.Evaluate(actual, expected);

        Assert.HasCount(1, result.Differences);
        var difference = result.Differences[0];
        Assert.AreEqual("Address.City", difference.Path);
        Assert.AreEqual("Address.City was 'Paris' instead of 'Berlin'.", difference.Message);
        Assert.AreEqual("'Berlin'", difference.Expected);
        Assert.AreEqual("'Paris'", difference.Actual);
        Assert.AreEqual("Expected the graphs to be equivalent, but Address.City was 'Paris' instead of 'Berlin'.", result.Message);
    }

    [TestMethod]
    public void CollectionCountDifference_CarriesCounts() {
        var expected = new Parcel { Tags = [1, 2, 3] };
        var actual = new Parcel { Tags = [1, 2] };

        var result = EquivalencyEvaluation.Evaluate(actual, expected);

        Assert.HasCount(1, result.Differences);
        var difference = result.Differences[0];
        Assert.AreEqual("Tags", difference.Path);
        Assert.AreEqual("Tags contains 2 item(s) instead of 3.", difference.Message);
        Assert.AreEqual("3", difference.Expected);
        Assert.AreEqual("2", difference.Actual);
    }

    [TestMethod]
    public void TypeMismatchAtTheRoot_IsReportedWithoutThrowing() {
        var result = EquivalencyEvaluation.Evaluate<object>("text", 5);

        Assert.HasCount(1, result.Differences);
        var difference = result.Differences[0];
        Assert.AreEqual("<root>", difference.Path);
        Assert.AreEqual("found an instance of type System.String instead of System.Int32.", difference.Message);
        Assert.AreEqual("System.Int32", difference.Expected);
        Assert.AreEqual("System.String", difference.Actual);
        Assert.AreEqual("Expected the graphs to be equivalent, but found an instance of type System.String instead of System.Int32.", result.Message);
    }

    [TestMethod]
    public void Configure_FlowsThrough() {
        var expected = new Parcel { Id = 1, Address = new Address { City = "Berlin" } };
        var actual = new Parcel { Id = 99, Address = new Address { City = "Berlin" } };

        var result = EquivalencyEvaluation.Evaluate(actual, expected, o => o.Excluding(x => x.Id));

        Assert.IsTrue(result.IsEquivalent);
    }

    [TestMethod]
    public void ToleranceOptions_FlowThrough() {
        var expected = new Shipment { Amount = 10.00m };
        var actual = new Shipment { Amount = 10.005m };

        var within = EquivalencyEvaluation.Evaluate(actual, expected, o => o.WithNumericTolerance(0.01));
        Assert.IsTrue(within.IsEquivalent);

        var exceeded = EquivalencyEvaluation.Evaluate(new Shipment { Amount = 10.02m }, expected, o => o.WithNumericTolerance(0.01));
        Assert.HasCount(1, exceeded.Differences);
        Assert.AreEqual("Amount was 10.02 instead of 10.00. The difference 0.02 exceeds the allowed tolerance 0.01.", exceeded.Differences[0].Message);
        Assert.AreEqual("10.00", exceeded.Differences[0].Expected);
        Assert.AreEqual("10.02", exceeded.Differences[0].Actual);
    }

    [TestMethod]
    public void Json_MatchesTheT11Conventions() {
        var expected = new Dictionary<string, int> { ["a"] = 1 };
        var actual = new Dictionary<string, int> { ["b"] = 1 };

        var result = EquivalencyEvaluation.Evaluate(actual, expected);
        var json = JsonNode.Parse(result.ToReportJson())!.AsObject();

        Assert.IsFalse(json["isEquivalent"]!.GetValue<bool>());
        Assert.AreEqual(1, json["count"]!.GetValue<int>());

        var entry = json["differences"]!.AsArray()[0]!.AsObject();
        Assert.AreEqual("[\"a\"]", entry["path"]!.GetValue<string>());
        Assert.AreEqual("[\"a\"] was not found in the subject dictionary.", entry["message"]!.GetValue<string>());
        Assert.IsNull(entry["expected"]);
        Assert.IsNull(entry["actual"]);
    }

    private sealed class Address {
        public string City { get; set; } = string.Empty;
    }

    private sealed class Parcel {
        public Address? Address { get; set; }

        public int Id { get; set; }

        public List<int>? Tags { get; set; }
    }

    private sealed class Shipment {
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
