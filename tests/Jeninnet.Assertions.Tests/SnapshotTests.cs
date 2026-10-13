//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SnapshotTests {
    [TestMethod]
    public void MatchSnapshot_PassForMatchingBaseline() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "line1\nline2");

        const string order = "line1\nline2";
        order.Should().MatchSnapshot("order", directory);
        Assert.IsFalse(File.Exists(Path.Combine(directory, "order.received.txt")));
    }

    [TestMethod]
    public void MatchSnapshot_MissingBaseline_WritesReceived() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var received = Path.Combine(directory, "order.received.txt");
        var verified = Path.Combine(directory, "order.verified.txt");

        const string order = "line1\nline2";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
        Assert.AreEqual($"Expected order to match snapshot 'order', but no verified snapshot exists. Review '{received}' and rename it to '{verified}' to accept it.\n\nReceived:\nline1\nline2", ex.Message);
        Assert.AreEqual("line1\nline2", File.ReadAllText(received));
    }

    [TestMethod]
    public void MatchSnapshot_Mismatch_OverwritesReceivedWithDiff() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var received = Path.Combine(directory, "order.received.txt");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "line1\nline2");
        File.WriteAllText(received, "stale");

        const string order = "line1\nCHANGED";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
        Assert.AreEqual(
            $"Expected order to match snapshot 'order', but the content differs. Review '{received}'.\n--- expected\n+++ actual\n@@ -1,2 +1,2 @@\n line1\n-line2\n+CHANGED",
            ex.Message);
        Assert.AreEqual("line1\nCHANGED", File.ReadAllText(received));
    }

    [TestMethod]
    public void MatchSnapshot_Match_DeletesStaleReceived() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "same");
        var received = Path.Combine(directory, "order.received.txt");
        File.WriteAllText(received, "stale");

        const string order = "same";
        order.Should().MatchSnapshot("order", directory);
        Assert.IsFalse(File.Exists(received));
    }

    [TestMethod]
    public void MatchSnapshot_InvalidNames_ThrowArgumentException() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");

        const string order = "x";
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("a/b", directory));
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("..", directory));
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("", directory));
    }

    [TestMethod]
    public void MatchSnapshot_DefaultName_UsesSubjectExpression() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "ready");

        const string order = "ready";
        order.Should().MatchSnapshot(snapshotsDirectory: directory);
    }

    [TestMethod]
    public void MatchSnapshot_DefaultDirectory_UsesCallerFile() {
        using var environment = new TestEnvironment();
        var project = environment.CreateDirectory("proj");
        var fakeSource = Path.Combine(project, "OrderTests.cs");
        var directory = Path.Combine(project, "__snapshots__");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "ready");

        const string order = "ready";
        // S3236 waived: the test deliberately exercises caller-path inference with a controlled path.
#pragma warning disable S3236
        order.Should().MatchSnapshot("order", null, null, fakeSource);
#pragma warning restore S3236
    }

    [TestMethod]
    public void MatchSnapshot_Object_SerializesDeterministicJson() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.json"), "{\n  \"Name\": \"a\",\n  \"Value\": 1\n}");

        var order = new { Name = "a", Value = 1 };
        order.Should().MatchSnapshot("order", directory);
    }

    [TestMethod]
    public void MatchSnapshot_ObjectMismatch_ShowsJsonDiff() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.json"), "{\n  \"Name\": \"a\",\n  \"Value\": 1\n}");

        var order = new { Name = "a", Value = 2 };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
        Assert.Contains("\"Value\": 1", ex.Message);
        Assert.Contains("\"Value\": 2", ex.Message);
        Assert.IsTrue(File.Exists(Path.Combine(directory, "order.received.json")));
    }

    [TestMethod]
    public void MatchSnapshot_Scrubber_AppliesBeforeCompare() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "order <n> shipped");

        const string order = "order 42 shipped";
        order.Should().MatchSnapshot("order", directory, static options => options.Scrub(static text => text.Replace("42", "<n>", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void MatchSnapshot_NullSubject_SnapshotsNullMarker() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "missing.verified.txt"), "<null>");

        const string? missing = null;
        missing.Should().MatchSnapshot("missing", directory);

        object? nothing = null;
        File.WriteAllText(Path.Combine(directory, "nothing.verified.json"), "null");
        nothing.Should().MatchSnapshot("nothing", directory);
    }

    [TestMethod]
    public void MatchSnapshot_WithBecause_AppendsReason() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var received = Path.Combine(directory, "order.received.txt");
        File.WriteAllText(Path.Combine(directory, "order.verified.txt"), "line1\nline2");

        const string order = "line1\nCHANGED";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory, because: "stale data"));
        Assert.AreEqual(
            $"Expected order to match snapshot 'order', but the content differs. Review '{received}' because stale data.\n--- expected\n+++ actual\n@@ -1,2 +1,2 @@\n line1\n-line2\n+CHANGED",
            ex.Message);
    }

    [TestMethod]
    public void MatchSnapshot_ArabicMessage() {
        var previous = AssertionCulture.Current;
        AssertionCulture.Current = new CultureInfo("ar");
        try {
            using var environment = new TestEnvironment();
            var directory = environment.CreateDirectory("snaps");
            var received = Path.Combine(directory, "order.received.txt");
            var verified = Path.Combine(directory, "order.verified.txt");

            const string order = "ready";
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
            Assert.AreEqual($"المتوقع من order أن يطابق اللقطة 'order'، لكن لا توجد لقطة معتمدة. راجع '{received}' وأعد تسميته إلى '{verified}' لقبوله.\n\nالمحتوى المستلم:\nready", ex.Message);
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void MatchSnapshot_CleansUpOnFailure() {
        string root;
        string received;
        using(var environment = new TestEnvironment()) {
            root = environment.Root;
            var directory = environment.CreateDirectory("snaps");
            received = Path.Combine(directory, "order.received.txt");

            const string order = "ready";
            Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
            Assert.IsTrue(File.Exists(received));
        }

        Assert.IsFalse(Directory.Exists(root));
    }

    [TestMethod, DoNotParallelize]
    public void MatchSnapshot_UpdateMode_PromotesMissingBaselineToVerified() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var received = Path.Combine(directory, "order.received.txt");
        var verified = Path.Combine(directory, "order.verified.txt");
        File.WriteAllText(received, "stale");

        WithUpdateMode(() => {
            const string order = "line1\nline2";
            order.Should().MatchSnapshot("order", directory);
        });

        Assert.AreEqual("line1\nline2", File.ReadAllText(verified));
        Assert.IsFalse(File.Exists(received));
    }

    [TestMethod, DoNotParallelize]
    public void MatchSnapshot_UpdateMode_PromotesMismatchToVerified() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var verified = Path.Combine(directory, "order.verified.txt");
        File.WriteAllText(verified, "line1\nline2");
        var received = Path.Combine(directory, "order.received.txt");
        File.WriteAllText(received, "stale");

        WithUpdateMode(() => {
            const string order = "line1\nCHANGED";
            order.Should().MatchSnapshot("order", directory);
        });

        Assert.AreEqual("line1\nCHANGED", File.ReadAllText(verified));
        Assert.IsFalse(File.Exists(received));
    }

    [TestMethod, DoNotParallelize]
    public void MatchSnapshot_UpdateMode_RequiresExplicitValue() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");

        WithUpdateMode("0", () => {
            const string order = "ready";
            Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("order", directory));
        });

        Assert.IsFalse(File.Exists(Path.Combine(directory, "order.verified.txt")));
    }

    private static void WithUpdateMode(Action action) => WithUpdateMode("1", action);

    private static void WithUpdateMode(string value, Action action) {
        var previous = Environment.GetEnvironmentVariable("JENINNET_SNAPSHOT_UPDATE");
        Environment.SetEnvironmentVariable("JENINNET_SNAPSHOT_UPDATE", value);
        try {
            action();
        }
        finally {
            Environment.SetEnvironmentVariable("JENINNET_SNAPSHOT_UPDATE", previous);
        }
    }
}
