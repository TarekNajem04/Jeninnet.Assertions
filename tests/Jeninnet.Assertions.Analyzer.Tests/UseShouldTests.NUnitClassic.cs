//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task NUnit_AreEqual_Converts() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.AreEqual("expected", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Be("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", NUNIT_CLASSIC_STUB, "Be", "AreEqual");
    }

    [TestMethod]
    public async Task NUnit_IsTrue_ConvertsToBeTrue() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    {|#0:Assert.IsTrue(flag)|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    flag.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", NUNIT_CLASSIC_STUB, "BeTrue", "IsTrue");
    }
}
