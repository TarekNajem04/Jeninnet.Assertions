//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task NUnitThat_EqualTo_ConvertsToBe() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.That(actual, Is.EqualTo("expected"))|};
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
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", NUNIT_CONSTRAINT_STUB, "Be", "That");
    }

    [TestMethod]
    public async Task NUnitThat_NotEqualTo_ConvertsToNotBe() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.That(actual, Is.Not.EqualTo("expected"))|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().NotBe("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", NUNIT_CONSTRAINT_STUB, "NotBe", "That");
    }

    [TestMethod]
    public async Task NUnitThat_True_ConvertsToBeTrue() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    {|#0:Assert.That(flag, Is.True)|};
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
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", NUNIT_CONSTRAINT_STUB, "BeTrue", "That");
    }

    [TestMethod]
    public async Task NUnitThat_Null_ConvertsToBeNull() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string? missing = "x";
                    {|#0:Assert.That(missing, Is.Null)|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? missing = "x";
                    missing.Should().BeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", NUNIT_CONSTRAINT_STUB, "BeNull", "That");
    }

    [TestMethod]
    public async Task NUnitThat_NotNull_ConvertsToNotBeNull() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    string? present = "x";
                    {|#0:Assert.That(present, Is.Not.Null)|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? present = "x";
                    present.Should().NotBeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", NUNIT_CONSTRAINT_STUB, "NotBeNull", "That");
    }

    [TestMethod]
    public async Task NUnitThat_GreaterThan_SuggestsNothing() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    int value = 5;
                    Assert.That(value, Is.GreaterThan(3));
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, NUNIT_CONSTRAINT_STUB);
    }
}
