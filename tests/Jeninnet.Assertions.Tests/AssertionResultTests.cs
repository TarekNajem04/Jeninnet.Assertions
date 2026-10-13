//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AssertionResultTests {
    [TestMethod]
    public void Validator_PassingAssertions_ReturnsSuccessWithoutThrowing() {
        var result = AssertionResult.Create(() => {
            const bool flag = true;
            flag.Should().BeTrue();
            "a".Should().Be("a");
        });

        Assert.IsTrue(result.IsSuccess);
        Assert.IsEmpty(result.Failures);
    }

    [TestMethod]
    public void Validator_FailingAssertions_CollectAllWithoutThrowing() {
        var result = AssertionResult.Create(() => {
            const bool flag = false;
            flag.Should().BeTrue();
            var numbers = new List<int> { 1, 2 };
            numbers.Should().Contain(3);
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.HasCount(2, result.Failures);
        Assert.AreEqual("Expected flag to be true, but found false.", result.Failures[0].Message);
        Assert.IsTrue(result.Failures[1].Message.Contains("to contain '3'", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Validator_Failures_AreStructured() {
        var result = AssertionResult.Create(() => {
            const bool flag = false;
            flag.Should().BeTrue();
        });

        Assert.HasCount(1, result.Failures);
        Assert.AreEqual("flag", result.Failures[0].Expression);
        Assert.AreEqual("BoolAssertions", result.Failures[0].Category);
        Assert.AreEqual("false", result.Failures[0].Actual);
        Assert.AreEqual("true", result.Failures[0].Expected);
    }

    [TestMethod]
    public void Validator_ThrowIfFailed_ThrowsAggregated() {
        var result = AssertionResult.Create(() => {
            const bool flag = false;
            flag.Should().BeTrue();
            const string actual = "Actual";
            actual.Should().Be("Expected");
        });

        var ex = Assert.ThrowsExactly<AssertionFailedException>(result.ThrowIfFailed);
        Assert.Contains("2 assertion failures:", ex.Message);
        Assert.Contains("Expected flag to be true, but found false.", ex.Message);
        Assert.Contains("Expected actual to be 'Expected', but found 'Actual'.", ex.Message);
    }

    [TestMethod]
    public void Validator_ThrowIfFailed_DoesNotThrowOnSuccess() {
        var result = AssertionResult.Create(() => true.Should().BeTrue());

        result.ThrowIfFailed();
    }

    [TestMethod]
    public void Validator_ToReportJson_IsStructured() {
        var result = AssertionResult.Create(() => {
            const bool flag = false;
            flag.Should().BeTrue();
        });

        var root = JsonNode.Parse(result.ToReportJson())!.AsObject();
        Assert.AreEqual(1, root["count"]!.AsValue().GetValue<int>());
        var entry = root["failures"]![0]!.AsObject();
        Assert.AreEqual("flag", entry["expression"]!.GetValue<string>());
        Assert.AreEqual("BoolAssertions", entry["category"]!.GetValue<string>());
        Assert.AreEqual("false", entry["actual"]!.GetValue<string>());
        Assert.AreEqual("true", entry["expected"]!.GetValue<string>());
    }

    [TestMethod]
    public void Validator_CreateAsync_CollectsFailures() {
        var result = GetAsyncResult().GetAwaiter().GetResult();

        Assert.IsFalse(result.IsSuccess);
        Assert.HasCount(1, result.Failures);
        Assert.AreEqual("Expected ok to be true, but found false.", result.Failures[0].Message);
    }

    [TestMethod]
    public void Validator_AmbientScopeRestoredAfterCreate() {
        var result = AssertionResult.Create(() => {
            const bool flag = false;
            flag.Should().BeTrue();
        });
        Assert.IsFalse(result.IsSuccess);

        const bool other = true;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => other.Should().BeFalse());
        Assert.AreEqual("Expected other to be false, but found true.", ex.Message);
    }

    [TestMethod]
    public void Validator_NullDelegate_ThrowsArgumentNull() {
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionResult.Create(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionResult.CreateAsync(null!));
    }

    private static async Task<AssertionResult> GetAsyncResult() =>
        await AssertionResult.CreateAsync(async () => {
            var ok = await ComputeAsync();
            ok.Should().BeTrue();
        });

    private static Task<bool> ComputeAsync() => Task.FromResult(false);
}
