//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task XUnit_Equal_Converts() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.Equal("expected", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Be("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", XUNIT_STUB, "Be", "Equal");
    }

    [TestMethod]
    public async Task XUnit_NotEqual_ConvertsToNotBe() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.NotEqual("expected", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().NotBe("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", XUNIT_STUB, "NotBe", "NotEqual");
    }

    [TestMethod]
    public async Task XUnit_True_ConvertsToBeTrue() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    {|#0:Assert.True(flag)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    flag.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", XUNIT_STUB, "BeTrue", "True");
    }

    [TestMethod]
    public async Task XUnit_NotNull_ConvertsToNotBeNull() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string? present = "x";
                    {|#0:Assert.NotNull(present)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? present = "x";
                    present.Should().NotBeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1002", XUNIT_STUB, "NotBeNull", "NotNull");
    }

    [TestMethod]
    public async Task XUnit_Contains_String_ConvertsToContain() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.Contains("tua", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Contain("tua");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1001", XUNIT_STUB, "Contain", "Contains");
    }
}
