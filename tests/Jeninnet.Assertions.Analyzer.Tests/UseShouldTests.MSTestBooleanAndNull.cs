//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task IsTrue_ConvertsToBeTrue() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    {|#0:Assert.IsTrue(flag)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    flag.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1002", "BeTrue", "IsTrue");
    }

    [TestMethod]
    public async Task IsFalse_WithMessage_ConvertsToBeFalse() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    bool flag = true;
                    {|#0:Assert.IsFalse(flag, "reason")|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool flag = true;
                    flag.Should().BeFalse("reason");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1002", "BeFalse", "IsFalse");
    }

    [TestMethod]
    public async Task IsNull_ConvertsToBeNull() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string? missing = "x";
                    {|#0:Assert.IsNull(missing)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? missing = "x";
                    missing.Should().BeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1002", "BeNull", "IsNull");
    }

    [TestMethod]
    public async Task IsNotNull_ConvertsToNotBeNull() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string? missing = null;
                    {|#0:Assert.IsNotNull(missing)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? missing = null;
                    missing.Should().NotBeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1002", "NotBeNull", "IsNotNull");
    }

    [TestMethod]
    public async Task IsNull_ValueType_SuggestsNothing() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    int count = 5;
                    Assert.IsNull(count);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }
}
