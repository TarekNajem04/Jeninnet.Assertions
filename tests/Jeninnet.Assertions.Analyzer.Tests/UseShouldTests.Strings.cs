//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task XUnit_StartsWith_ConvertsToStartWith() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.StartsWith("act", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().StartWith("act");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1003", XUNIT_STUB, "StartWith", "StartsWith");
    }

    [TestMethod]
    public async Task XUnit_EndsWith_ConvertsToEndWith() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.EndsWith("ual", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().EndWith("ual");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1003", XUNIT_STUB, "EndWith", "EndsWith");
    }

    [TestMethod]
    public async Task MSTest_StringAssertContains_ConvertsToContain() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:StringAssert.Contains(actual, "tua")|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Contain("tua");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1003", "Contain", "Contains");
    }

    [TestMethod]
    public async Task MSTest_StringAssertContains_MessageConvertsToBecause() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:StringAssert.Contains(actual, "tua", "reason")|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Contain("tua", "reason");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1003", "Contain", "Contains");
    }

    [TestMethod]
    public async Task NUnit_StringAssertEndsWith_ConvertsToEndWith() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:StringAssert.EndsWith(actual, "ual")|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().EndWith("ual");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1003", NUNIT_CLASSIC_STUB, "EndWith", "EndsWith");
    }

    [TestMethod]
    public async Task StringAssert_StringComparisonOverload_SuggestsNothing() {
        const string source = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    StringAssert.Contains(actual, "TUA", StringComparison.OrdinalIgnoreCase);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }
}
