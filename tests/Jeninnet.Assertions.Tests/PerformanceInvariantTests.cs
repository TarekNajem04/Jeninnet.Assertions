//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class PerformanceInvariantTests {
    private static readonly int[] _numbers = [1, 2, 3];

    [TestMethod]
    public void SuccessPath_Be_DoesNotFormatFailureMessage() {
        WarmUp();

        var success = Measure(() => "abc".Should().Be("abc"));
        var failure = Measure(() => Try(() => "abc".Should().Be("xyz")));

        TestContext.WriteLine($"string.Be success={success}B failure={failure}B");
        // A success path that formatted the message would allocate like the failure path; it must stay far below
        // 2 KB (a single formatted message is several KB) and strictly below the failure path.
        Assert.IsLessThan(2048, success, $"success path allocated {success} B; expected well under 2048 B without message formatting.");
        Assert.IsLessThan(failure, success, $"success path allocated {success} B, more than the failure path's {failure} B.");
        Assert.IsGreaterThanOrEqualTo(512, failure, $"failure path allocated only {failure} B; expected a formatted message.");
    }

    [TestMethod]
    public void HotVerbs_SuccessAllocatesLessThanFailure() {
        WarmUp();

        (string Name, Action Success, Action Failure)[] verbs = [
            ("string.Be", () => "abc".Should().Be("abc"), () => Try(() => "abc".Should().Be("xyz"))),
            ("bool.BeTrue", () => true.Should().BeTrue(), () => Try(() => false.Should().BeTrue())),
            ("string.Contain", () => "hello".Should().Contain("ell"), () => Try(() => "hello".Should().Contain("zzz"))),
            ("int[].HaveCount", () => _numbers.Should().HaveCount(3), () => Try(() => _numbers.Should().HaveCount(5))),
        ];

        foreach(var (name, successAction, failureAction) in verbs) {
            var success = Measure(successAction);
            var failure = Measure(failureAction);

            TestContext.WriteLine($"{name} success={success}B failure={failure}B");
            Assert.IsLessThan(2048, success, $"{name} success path allocated {success} B; expected well under 2048 B without message formatting.");
            Assert.IsLessThan(failure, success, $"{name} success path allocated {success} B, more than the failure path's {failure} B.");
        }
    }

    public TestContext TestContext { get; set; } = null!;

    private static long Measure(Action action) {
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static void Try(Action action) {
        try {
            action();
        }
        catch(AssertionFailedException) {
            // The failure is the measured outcome here; swallowing it is intentional.
        }
    }

    private static void WarmUp() {
        for(var i = 0; i < 1000; i++) {
            _ = "abc".Should().Be("abc");
            _ = true.Should().BeTrue();
            _ = "hello".Should().Contain("ell");
            _ = _numbers.Should().HaveCount(3);
            Try(() => "abc".Should().Be("xyz"));
        }
    }
}
