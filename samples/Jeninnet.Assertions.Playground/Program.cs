using Jeninnet.Assertions;

try {
    FluentStyle();
    FailureReport();
    Snapshots();
    DeterministicClock();

    await Console.Out.WriteLineAsync("Playground complete: fluent style, failure report, snapshot, and clock all behaved as documented.");
    return 0;
}
catch(Exception exception) {
    await Console.Error.WriteLineAsync("FAILED: " + exception.Message);
    return 1;
}

static void FluentStyle() {
    Console.WriteLine("--- fluent style: assertions read as sentences ---");
    const int total = 42;
    total.Should().BePositive("the cart total after checkout");
    total.Should().Be(42).And.BeLessThan(100);

    const string name = "Ada Lovelace";
    name.Should().StartWith("Ada").And.Contain("Lovelace").And.HaveLength(12);

    int[] scores = [3, 1, 2];
    scores.Should().HaveCount(3).And.Contain(2).And.NotContain(9);
    Console.WriteLine("ok: values, strings, and collections.");
}

static void FailureReport() {
    Console.WriteLine("--- failure report: one report, not the first throw ---");
    try {
        using(new AssertionScope("Checkout")) {
            const int total = -5;
            total.Should().BePositive();

            string[] lines = [];
            lines.Should().NotBeEmpty();
        }
    }
    catch(AssertionFailedException exception) {
        Console.WriteLine(exception.Message);
        return;
    }

    throw new InvalidOperationException("The scoped failures should have been reported together.");
}

static void Snapshots() {
    Console.WriteLine("--- snapshots: review once, pin the baseline, diffs reveal change ---");
    var invoice = string.Join("\n", "invoice: 42", "total: 84.00 USD", "lines: 2");
    invoice.Should().MatchSnapshot("playground-report");
    Console.WriteLine("ok: output matched __snapshots__/playground-report.verified.txt.");
}

static void DeterministicClock() {
    Console.WriteLine("--- deterministic clock: test time without sleeping ---");
    AssertionClock.Current = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
    try {
        var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);
        value.Should().BeWithin(TimeSpan.FromMinutes(5));
        Console.WriteLine("ok: boundary passed exactly at the edge.");
    }
    finally {
        AssertionClock.Current = SystemClock.Instance;
    }
}
