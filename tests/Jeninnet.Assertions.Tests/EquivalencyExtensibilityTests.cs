//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

/// <summary>Covers the equivalency extensibility surface: custom steps, plan ordering, global Modify, diagnostics, rules, Value checks, discovery, and scope visibility.</summary>
[TestClass]
public class EquivalencyExtensibilityTests {
    [TestMethod]
    public void CustomStep_HandlesTargetTypeAndSkipsOthers() {
        var actual = new Order("a", new Money(5, "USD"));
        var expected = new Order("a", new Money(5, "EUR"));

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Total.Currency was 'USD' instead of 'EUR'.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.Using(new MoneyStep()));

        var other = new Order("b", new Money(5, "USD"));
        var skipped = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(other, static o => o.Using(new MoneyStep())));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", skipped.Message);
    }

    [TestMethod]
    public void CustomStep_ValidateOnlyLifecycle() {
        var writer = new StringBuilderTraceWriter();
        var step = new ValidateOnlyStep();
        // Distinct instances: equal interned literals are reference-identical and skip custom steps by design.
        var actual = new Order(new string('a', 1), new Money(5, "USD"));
        actual.Should().BeEquivalentTo(
            new Order(new string('a', 1), new Money(5, "USD")),
            o => o.WithTracing(writer).Using(step));

        Assert.AreEqual(1, step.Validated);
        Assert.Contains("validated-string", writer.ToString());
    }

    [TestMethod]
    public void PlanOrdering_AddBeforeAddAfterInsert() {
        var log = new List<string>();
        var a = new RecordingStep("a", log);
        var b = new RecordingStep("b", log);
        var c = new RecordingStep("c", log);
        var actual = new Order("x", new Money(1, "USD"));
        var expected = new Order("x", new Money(1, "USD"));

        actual.Should().BeEquivalentTo(expected, o => {
            o.Plan.Add(a).Add(b).Add(c);
            return o;
        });
        Assert.AreEqual("Handles:a:<root>,Handles:b:<root>,Handles:c:<root>", string.Join(",", log.Take(3)));

        log.Clear();
        var x = new RecordingStep("x", log);
        var y = new RecordingStep("y", log);
        var z = new RecordingStep("z", log);
        actual.Should().BeEquivalentTo(expected, o => {
            o.Plan.Add(a).Add(b).Add(c).AddBefore(b, x).AddAfter(a, y).Insert(0, z).Remove(x);
            return o;
        });
        Assert.AreEqual("Handles:z:<root>,Handles:a:<root>,Handles:y:<root>,Handles:b:<root>,Handles:c:<root>", string.Join(",", log.Take(5)));
    }

    [TestMethod]
    public void Plan_RemoveByTypeResetClearAndMissingAnchor() {
        var plan = new EquivalencyPlan();
        var first = new RecordingStep("a", []);
        var second = new RecordingStep("a-again", []);
        plan.Add(first).Add(second);
        Assert.HasCount(2, plan.Snapshot());

        plan.Remove(typeof(RecordingStep));
        Assert.IsEmpty(plan.Snapshot());

        plan.Add(first).Add(second).Remove(first);
        Assert.HasCount(1, plan.Snapshot());

        plan.Add(first).Add(second).Reset();
        Assert.IsEmpty(plan.Snapshot());

        plan.Add(first).Clear();
        Assert.IsEmpty(plan.Snapshot());

        var missing = Assert.ThrowsExactly<InvalidOperationException>(() => plan.AddAfter(first, second));
        Assert.Contains("No equivalency step matches", missing.Message);
    }

    [TestMethod]
    public void Using_NullGuards() {
        var options = new SelfReferenceEquivalencyOptions<Order>();
        Assert.ThrowsExactly<ArgumentNullException>(() => options.Using(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.UsingSelectionRule(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.UsingMatchingRule(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.UsingOrderingRule(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.WithTracing(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionConfiguration.Equivalency.Modify(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => CustomAssertionDiscovery.FindCustomAssertions(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => CustomAssertionDiscovery.RunInitializers(null!));
    }

    [TestMethod]
    public async Task GlobalModify_AppliesAndIsolatesAcrossFlows() {
        try {
            AssertionConfiguration.Equivalency.Modify(static o => o.StrictOrdering = true);

            var actual = new Basket(["b", "a"]);
            var expected = new Basket(["a", "b"]);
            var strict = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
            Assert.AreEqual(
                string.Join(
                    Environment.NewLine,
                    "Expected actual to be equivalent, but found 2 difference(s):",
                    "1. Tags[0] was 'b' instead of 'a'.",
                    "2. Tags[1] was 'a' instead of 'b'."),
                strict.Message);

            await Task.Run(() => {
                EquivalencyDefaults.ResetToDefaults();
                actual.Should().BeEquivalentTo(expected);
                AssertionConfiguration.Equivalency.Modify(static o => o.StrictOrdering = true);
            }, TestContext.CancellationToken);

            var stillStrict = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
            Assert.AreEqual(
                string.Join(
                    Environment.NewLine,
                    "Expected actual to be equivalent, but found 2 difference(s):",
                    "1. Tags[0] was 'b' instead of 'a'.",
                    "2. Tags[1] was 'a' instead of 'b'."),
                stillStrict.Message);
        }
        finally {
            EquivalencyDefaults.ResetToDefaults();
        }

        new Basket(["b", "a"]).Should().BeEquivalentTo(new Basket(["a", "b"]));
    }

    /// <summary>
    /// Mutates the process-wide equivalency plan; excluded from parallel execution so the step cannot leak into other tests.
    /// </summary>
    [TestMethod, DoNotParallelize]
    public void GlobalPlan_StepAppliesProcessWide() {
        AssertionConfiguration.Equivalency.Plan.Add(new MoneyStep());
        try {
            // Same amount, different currency: the global MoneyStep owns Money nodes (amount-only),
            // so the built-in comparison never sees the currency difference.
            new Order("a", new Money(5, "USD")).Should().BeEquivalentTo(new Order("a", new Money(5, "EUR")));
        }
        finally {
            AssertionConfiguration.Equivalency.Plan.Remove(typeof(MoneyStep));
        }

        // Without the global step the built-in comparison governs again.
        Assert.ThrowsExactly<AssertionFailedException>(() =>
            new Order("a", new Money(5, "USD")).Should().BeEquivalentTo(new Order("a", new Money(5, "EUR"))));
    }

    [TestMethod]
    public void CloneDefaults_IsDetachedSnapshot() {
        var clone = AssertionConfiguration.Equivalency.CloneDefaults();
        Assert.IsFalse(clone.StrictOrdering);

        clone.StrictOrdering = true;
        var fresh = new SelfReferenceEquivalencyOptions<object>();
        Assert.IsFalse(fresh.StrictOrdering);
    }

    [TestMethod]
    public void FullDump_ShowsWholeTreeAndConfiguration() {
        var actual = new Order("x", new Money(5, "USD"));
        var expected = new Order("y", new Money(5, "USD"));
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithFullDump()));

        Assert.Contains("With configuration:", ex.Message);
        Assert.Contains("Full dump of subject:", ex.Message);
        Assert.Contains("Full dump of expectation:", ex.Message);
        Assert.Contains("Name: 'x'", ex.Message);
        Assert.Contains("Name: 'y'", ex.Message);
        Assert.Contains("Amount: 5", ex.Message);
    }

    [TestMethod]
    public void FullDump_MarksCyclesInsteadOfOverflowing() {
        var actual = new Node("a");
        actual.Next = actual;
        var expected = new Node("b");
        expected.Next = expected;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.WithFullDump()));
        Assert.Contains("cyclic reference", ex.Message);
        Assert.Contains("<cyclic>", ex.Message);
    }

    [TestMethod]
    public void Tracing_ShowsComparisonSteps() {
        var writer = new StringBuilderTraceWriter();
        var actual = new Order("a", new Money(5, "USD"));
        var expected = new Order("a", new Money(6, "USD"));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, o => o.WithTracing(writer).Using(new MoneyStep())));
        Assert.Contains("With configuration:", ex.Message);

        var trace = writer.ToString();
        Assert.Contains("Comparing <root>", trace);
        Assert.Contains(nameof(MoneyStep), trace);
        Assert.Contains("handles", trace);
    }

    [TestMethod]
    public void SelectionRule_ExcludesAndRestores() {
        var actual = new Order("a", new Money(5, "USD"));
        var expected = new Order("b", new Money(5, "USD"));

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.UsingSelectionRule(new SkipNameRule()));
        var restored = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.UsingSelectionRule(new SkipNameRule()).WithoutSelectionRules()));
        Assert.AreEqual("Expected actual to be equivalent, but Name was 'a' instead of 'b'.", restored.Message);
    }

    [TestMethod]
    public void MatchingRule_ResolvesAndRestores() {
        var actual = new Parcel("a");
        var expected = new Shipment("a");

        var plain = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("Town was not found on the subject.", plain.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.UsingMatchingRule(new TownMatchingRule()));
        var restored = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.UsingMatchingRule(new TownMatchingRule()).WithoutMatchingRules()));
        Assert.Contains("Town was not found on the subject.", restored.Message);
    }

    [TestMethod]
    public void OrderingRule_ScopesStrictnessAndRestores() {
        var actual = new Basket(["b", "a"]);
        var expected = new Basket(["a", "b"]);
        actual.Should().BeEquivalentTo(expected);

        var strict = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().BeEquivalentTo(expected, static o => o.UsingOrderingRule(new StrictTagsRule())));
        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "Expected actual to be equivalent, but found 2 difference(s):",
                "1. Tags[0] was 'b' instead of 'a'.",
                "2. Tags[1] was 'a' instead of 'b'."),
            strict.Message);

        actual.Should().BeEquivalentTo(expected, static o => o.UsingOrderingRule(new StrictTagsRule()).WithoutOrderingRules());
    }

    [TestMethod]
    public void Value_ThatMatches_PassAndFail() {
        Value.ThatMatches(5, static x => x > 3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => Value.ThatMatches(5, static x => x > 10));
        Assert.AreEqual("Expected value to match (x > 10), but found 5.", ex.Message);
    }

    [TestMethod]
    public void Value_ThatSatisfies_PassAndFail() {
        Value.ThatSatisfies("a", static s => s.Should().Be("a"));

        var assertion = Assert.ThrowsExactly<AssertionFailedException>(
            () => Value.ThatSatisfies<string?>(null, static s => s!.Should().NotBeNull()));
        Assert.Contains("to not be null", assertion.Message);

        var boom = Assert.ThrowsExactly<AssertionFailedException>(
            () => Value.ThatSatisfies(5, static _ => throw new InvalidOperationException("bang")));
        Assert.AreEqual("Expected value to satisfy the given check, but System.InvalidOperationException was thrown: bang.", boom.Message);
    }

    [TestMethod]
    public void Discovery_FindsMarkersAndRunsInitializers() {
        var methods = CustomAssertionDiscovery.FindCustomAssertions(typeof(ExtensibilityTests).Assembly);
        Assert.Contains("SampleExtension", methods.Select(static m => m.Name));

        var count = CustomAssertionDiscovery.RunInitializers(typeof(ExtensibilityTests).Assembly);
        Assert.IsGreaterThanOrEqualTo(count, 1);
        Assert.IsTrue(TestInitializer.Ran);

        var marked = CustomAssertionDiscovery.FindMarkedAssemblies();
        Assert.IsNotNull(marked);
    }

    [TestMethod]
    public void ScopeVisibility_CurrentNameAndFailures() {
        Assert.IsNull(AssertionScope.Current);

        using(var scope = new AssertionScope("Pricing")) {
            Assert.AreEqual("Pricing", scope.Name);
            Assert.AreSame(scope, AssertionScope.Current);
            Assert.IsFalse(scope.HasFailures());

            true.Should().BeFalse();
            Assert.IsTrue(scope.HasFailures());

            var failures = scope.Discard();
            Assert.HasCount(1, failures);
            Assert.IsFalse(scope.HasFailures());
        }

        Assert.IsNull(AssertionScope.Current);
    }

    private sealed class Money(decimal amount, string currency) {
        public decimal Amount { get; } = amount;

        public string Currency { get; } = currency;
    }

    private sealed class Order(string name, Money total) {
        public string Name { get; } = name;

        public Money Total { get; } = total;
    }

    private sealed class Basket(List<string> tags) {
        public List<string> Tags { get; } = tags;
    }

    private sealed class Parcel(string city) {
        public string City { get; } = city;
    }

    private sealed class Shipment(string town) {
        public string Town { get; } = town;
    }

    private sealed class Node(string label) {
        public string Label { get; } = label;

        public Node? Next { get; set; }
    }

    private sealed class MoneyStep : EquivalencyStep {
        public override bool Handles(Comparands comparands) => comparands.Expectation is Money;

        public override bool TryHandle(Comparands comparands, IObjectGraph objectGraph, IEquivalencyStepContext context) {
            var subject = comparands.Subject as Money;
            var expectation = (Money)comparands.Expectation!;
            if(subject is null || subject.Amount != expectation.Amount) {
                context.Fail($"{comparands.FormattedPath} was {Format(subject)} instead of {Format(expectation)}.");
            }

            return true;
        }

        private static string Format(Money? money) => money is null ? "<null>" : $"{money.Amount} {money.Currency}";
    }

    private sealed class ValidateOnlyStep : EquivalencyStep {
        public int Validated;

        public override bool Handles(Comparands comparands) => comparands.Expectation is string;

        public override void AssertEquivalency(Comparands comparands, IEquivalencyValidationContext context) {
            Validated++;
            context.Trace("validated-string");
        }
    }

    private sealed class RecordingStep(string name, List<string> log) : EquivalencyStep {
        public string Name { get; } = name;

        public override bool Handles(Comparands comparands) {
            log.Add($"Handles:{Name}:{comparands.FormattedPath}");
            return false;
        }
    }

    private sealed class SkipNameRule : IMemberSelectionRule {
        public bool Includes(MemberInfo member) => member.Name != "Name";
    }

    private sealed class TownMatchingRule : IMemberMatchingRule {
        public string? ResolveSubjectMember(IReadOnlyList<string> path, Type expectationType, Type subjectType) =>
            path.Count > 0 && path[path.Count - 1] == "Town" ? "City" : null;
    }

    private sealed class StrictTagsRule : IOrderingRule {
        public bool? IsOrdered(IReadOnlyList<string> path) =>
            path.Count == 1 && path[0] == "Tags" ? true : null;
    }

    // MSTest injects the instance after construction; the null-forgiving initializer only silences CS8618.
    public TestContext TestContext { get; set; } = null!;
}
