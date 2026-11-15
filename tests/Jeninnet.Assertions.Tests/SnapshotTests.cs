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
    public void Options_RejectNullArguments() {
        var options = new SnapshotOptions();
        string[] namesWithNull = [null!];

        Assert.ThrowsExactly<ArgumentNullException>(() => options.Scrub(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddScrubber(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddScrubberByType(null!, static text => text));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddScrubberByType(typeof(object), null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddScrubberByMember(null!, static text => text));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.AddScrubberByMember("member", null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.IgnoreMembersNamed(namesWithNull));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => options.IndentSize(-1));
    }

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

    [TestMethod]
    public void MatchSnapshot_NamingOptions_Compose() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var expected = Path.Combine(directory, "SnapshotTests_MatchSnapshot_NamingOptions_Compose_order_with_discount.verified.txt");

        const string order = "ready";
        order.Should().MatchSnapshot("order", directory,
            static options => options.UseTypeName().UseMethodName().UseParameters().UseTextForParameters("with discount").AutoVerify());

        Assert.IsTrue(File.Exists(expected));
    }

    [TestMethod]
    public void MatchSnapshot_UseFileNameAndDirectory_StayContained() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var expected = Path.Combine(directory, "nested", "custom.received.txt");

        const string order = "ready";
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("ignored", directory,
            static options => options.UseDirectory("nested").UseFileName("custom")));

        Assert.IsTrue(File.Exists(expected));

        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("order", directory, static options => options.UseDirectory("../escape")));
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("order", directory, static options => options.UseDirectory(Path.GetTempPath())));
        Assert.ThrowsExactly<ArgumentException>(() => order.Should().MatchSnapshot("order", directory, static options => options.UseFileName("a/b")));
    }

    [TestMethod]
    public void MatchSnapshot_UniqueSuffixes_AreStable() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");

        const string order = "ready";
        var osSuffix = "other";
        if(OperatingSystem.IsWindows()) {
            osSuffix = "win";
        } else if(OperatingSystem.IsLinux()) {
            osSuffix = "linux";
        } else if(OperatingSystem.IsMacOS()) {
            osSuffix = "macos";
        }

        var expectedOs = Path.Combine(directory, $"unique_{osSuffix}.received.txt");
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("unique", directory, static options => options.UniqueForOSPlatform()));
        Assert.IsTrue(File.Exists(expectedOs));

        var expectedTfm = Path.Combine(directory, "unique_net11.0.received.txt");
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("unique", directory, static options => options.UniqueForTargetFramework()));
        Assert.IsTrue(File.Exists(expectedTfm));

        var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture switch {
            System.Runtime.InteropServices.Architecture.X64 => "x64",
            System.Runtime.InteropServices.Architecture.X86 => "x86",
            System.Runtime.InteropServices.Architecture.Arm64 => "arm64",
            System.Runtime.InteropServices.Architecture.Arm => "arm",
            _ => "other",
        };
        var expectedArchitecture = Path.Combine(directory, $"unique_{architecture}.received.txt");
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("unique", directory, static options => options.UniqueForArchitecture()));
        Assert.IsTrue(File.Exists(expectedArchitecture));

        var assembly = typeof(SnapshotTests).Assembly.GetName().Name!.ToLowerInvariant();
        var expectedAssembly = Path.Combine(directory, $"unique_{assembly}.received.txt");
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("unique", directory, static options => options.UniqueForAssembly()));
        Assert.IsTrue(File.Exists(expectedAssembly));

        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("unique", directory, static options => options.UniqueForRuntime()));
        Assert.IsNotEmpty(Directory.GetFiles(directory, "unique_net*.received.txt"));
    }

    [TestMethod]
    public void MatchSnapshot_LineEndingAndWhitespaceTolerance() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "line.verified.txt"), "line1\nline2");
        File.WriteAllText(Path.Combine(directory, "space.verified.txt"), "a b");

        const string line = "line1\r\nline2";
        line.Should().MatchSnapshot("line", directory, static options => options.IgnoreLineEndingDifferences());

        const string space = "a   b";
        space.Should().MatchSnapshot("space", directory, static options => options.IgnoreWhiteSpaceDifferences());
    }

    [TestMethod]
    public void MatchSnapshot_SortPropertiesAndIndentSize() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "sorted.verified.json"), "{\n    \"a\": 1,\n    \"b\": 2\n}");

        var value = new { b = 2, a = 1 };
        value.Should().MatchSnapshot("sorted", directory, static options => options.SortProperties().IndentSize(4));
    }

    [TestMethod]
    public void MatchSnapshot_ObjectScrubbersByTypeAndMember() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "type.verified.json"), "{\n  \"Name\": \"<masked>\"\n}");
        File.WriteAllText(Path.Combine(directory, "member.verified.json"), "{\n  \"Name\": \"<masked>\"\n}");

        var value = new ScrubTarget("secret");
        value.Should().MatchSnapshot("type", directory, static options => options.AddScrubberByType(typeof(ScrubTarget), static text => text.Replace("secret", "<masked>", StringComparison.Ordinal)));
        value.Should().MatchSnapshot("member", directory, static options => options.AddScrubberByMember(nameof(ScrubTarget.Name), static text => text.Replace("secret", "<masked>", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void MatchSnapshot_AutoVerifyAndDisableDiff() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");

        const string order = "ready";
        order.Should().MatchSnapshot("auto", directory, static options => options.AutoVerify());
        Assert.AreEqual("ready", File.ReadAllText(Path.Combine(directory, "auto.verified.txt")));
        Assert.IsFalse(File.Exists(Path.Combine(directory, "auto.received.txt")));

        File.WriteAllText(Path.Combine(directory, "diff.verified.txt"), "before");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("diff", directory, static options => options.DisableDiff()));
        Assert.DoesNotContain("--- expected", ex.Message);
        Assert.IsTrue(File.Exists(Path.Combine(directory, "diff.received.txt")));
    }

    [TestMethod]
    public void MatchSnapshot_ConfiguredDiffToolFailureDoesNotHideAssertion() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "tool.verified.txt"), "before");
        var missingTool = Path.Combine(directory, "missing-diff.exe");

        const string order = "after";
        Assert.ThrowsExactly<AssertionFailedException>(() => order.Should().MatchSnapshot("tool", directory, options => options.DiffTool(missingTool)));
    }

    [TestMethod]
    public void MatchSnapshot_JsonNodeAndXml_WriteLfSnapshots() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");

        var node = JsonNode.Parse("""{"b":2,"a":1}""");
        Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().MatchSnapshot("node", directory, static options => options.SortProperties()));
        var nodeReceived = Path.Combine(directory, "node.received.json");
        Assert.AreEqual("{\n  \"a\": 1,\n  \"b\": 2\n}", File.ReadAllText(nodeReceived));
        Assert.DoesNotContain("\r", File.ReadAllText(nodeReceived));
        File.Move(nodeReceived, Path.Combine(directory, "node.verified.json"));
        node.Should().MatchSnapshot("node", directory, static options => options.SortProperties());

        var element = XElement.Parse("<root><a>1</a></root>");
        Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().MatchSnapshot("xml", directory));
        var xmlReceived = Path.Combine(directory, "xml.received.xml");
        Assert.Contains("<root>", File.ReadAllText(xmlReceived));
        Assert.DoesNotContain("\r", File.ReadAllText(xmlReceived));
        File.Move(xmlReceived, Path.Combine(directory, "xml.verified.xml"));
        element.Should().MatchSnapshot("xml", directory);

        var document = XDocument.Parse("<root><a>1</a></root>");
        Assert.ThrowsExactly<AssertionFailedException>(() => document.Should().MatchSnapshot("document", directory));
        var documentReceived = Path.Combine(directory, "document.received.xml");
        Assert.DoesNotContain("\r", File.ReadAllText(documentReceived));
        File.Move(documentReceived, Path.Combine(directory, "document.verified.xml"));
        document.Should().MatchSnapshot("document", directory);
    }

    [TestMethod]
    public void MatchSnapshot_ConvenienceScrubbers_MaskMachineUserAndGuid() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var guid = Guid.NewGuid();
        var subject = $"machine={Environment.MachineName};user={Environment.UserName};id={guid:D};upper={guid.ToString("D").ToUpperInvariant()}";

        subject.Should().MatchSnapshot("scrub", directory,
            static options => options.ScrubMachineName().ScrubUserName().ScrubGuid().AutoVerify());

        var verified = File.ReadAllText(Path.Combine(directory, "scrub.verified.txt"));
        Assert.AreEqual("machine=<machine>;user=<user>;id=<guid>;upper=<guid>", verified);
    }

    [TestMethod]
    public void MatchSnapshot_ConvenienceScrubbers_RunAfterExplicitScrubbers() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        var subject = $"host={Environment.MachineName}";

        subject.Should().MatchSnapshot("order", directory,
            static options => options
                .Scrub(text => text.Replace(Environment.MachineName, "custom"))
                .ScrubMachineName()
                .AutoVerify());

        Assert.AreEqual("host=custom", File.ReadAllText(Path.Combine(directory, "order.verified.txt")));
    }

    [TestMethod]
    public void MatchSnapshot_IgnoresJsonPropertyPaths() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "ignored.verified.json"),
            "{\n  \"metadata\": {\n    \"id\": 1\n  },\n  \"name\": \"a\"\n}");

        var value = new { metadata = new { timestamp = "new", id = 1 }, name = "a" };
        value.Should().MatchSnapshot("ignored", directory, static options => options.IgnoreJsonProperty("$.metadata.timestamp"));

        Assert.IsFalse(File.Exists(Path.Combine(directory, "ignored.received.json")));
    }

    [TestMethod]
    public void MatchSnapshot_IgnoresMemberNamesAtEveryDepth() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "names.verified.json"),
            "{\n  \"id\": \"new\",\n  \"nested\": {\n    \"id\": 2\n  }\n}");

        var value = new { id = "new", nested = new { timestamp = "volatile", id = 2 } };
        value.Should().MatchSnapshot("names", directory, static options => options.IgnoreMembersNamed("timestamp"));

        Assert.IsFalse(File.Exists(Path.Combine(directory, "names.received.json")));
    }

    [TestMethod]
    public void MatchSnapshot_MissingIgnoredPath_IsANoOp() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("snaps");
        File.WriteAllText(Path.Combine(directory, "missing.verified.json"), "{\n  \"value\": 1\n}");

        var value = new { value = 2 };
        Assert.ThrowsExactly<AssertionFailedException>(
            () => value.Should().MatchSnapshot("missing", directory, static options => options.IgnoreJsonProperty("$.absent.path")));
        Assert.IsTrue(File.Exists(Path.Combine(directory, "missing.received.json")));
    }

    [TestMethod]
    public void SnapshotOptions_ValidateJsonProjectionArguments() {
        var options = new SnapshotOptions();

        Assert.ThrowsExactly<ArgumentNullException>(() => options.IgnoreJsonProperty(null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreJsonProperty("  "));
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreJsonProperty("$"));
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreJsonProperty("$.a..b"));
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreJsonProperty("$.a[0]"));
        Assert.ThrowsExactly<ArgumentNullException>(() => options.IgnoreMembersNamed(null!));
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreMembersNamed());
        Assert.ThrowsExactly<ArgumentException>(() => options.IgnoreMembersNamed("timestamp", " "));
    }

    [TestMethod]
    public void MatchSnapshot_UseProjectRelativeDirectory_FindsProjectAndFallsBack() {
        using var environment = new TestEnvironment();
        var project = environment.CreateDirectory("project");
        File.WriteAllText(Path.Combine(project, "sample.csproj"), "<Project />");
        var sourceDirectory = Path.Combine(project, "src");
        Directory.CreateDirectory(sourceDirectory);
        var fakeSource = Path.Combine(sourceDirectory, "SampleTests.cs");
        const string subject = "ready";

        // S3236 waived: the test deliberately exercises caller-path inference with a controlled path.
#pragma warning disable S3236
        subject.Should().MatchSnapshot("relative", null,
            static options => options.UseProjectRelativeDirectory().AutoVerify(), fakeSource);
        Assert.IsTrue(File.Exists(Path.Combine(project, "__snapshots__", "relative.verified.txt")));
        Assert.IsFalse(File.Exists(Path.Combine(sourceDirectory, "__snapshots__", "relative.verified.txt")));

        var fallback = environment.CreateDirectory("fallback");
        var fallbackSourceDirectory = Path.Combine(fallback, "src");
        Directory.CreateDirectory(fallbackSourceDirectory);
        var fallbackSource = Path.Combine(fallbackSourceDirectory, "FallbackTests.cs");
        subject.Should().MatchSnapshot("fallback", null,
            static options => options.UseProjectRelativeDirectory().AutoVerify(), fallbackSource);
        Assert.IsTrue(File.Exists(Path.Combine(fallbackSourceDirectory, "__snapshots__", "fallback.verified.txt")));

        var explicitDirectory = environment.CreateDirectory("explicit");
        subject.Should().MatchSnapshot("explicit", explicitDirectory,
            static options => options.UseProjectRelativeDirectory().AutoVerify(), fakeSource);
        Assert.IsTrue(File.Exists(Path.Combine(explicitDirectory, "explicit.verified.txt")));
#pragma warning restore S3236
    }

    private sealed record ScrubTarget(string Name);

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
