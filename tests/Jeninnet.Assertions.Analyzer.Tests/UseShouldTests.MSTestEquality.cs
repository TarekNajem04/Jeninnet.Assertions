//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task AreEqual_String_Converts() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.AreEqual("expected", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Be("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "Be", "AreEqual");
    }

    [TestMethod]
    public async Task AreEqual_Message_ConvertsToBecause() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.AreEqual("expected", actual, "reason")|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Be("expected", "reason");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "Be", "AreEqual");
    }

    [TestMethod]
    public async Task AreNotEqual_ConvertsToNotBe() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.AreNotEqual("expected", actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().NotBe("expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "NotBe", "AreNotEqual");
    }

    [TestMethod]
    public async Task AreEqual_NamedArguments_Preserved() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    {|#0:Assert.AreEqual(expected: "expected", actual: actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    actual.Should().Be(expected: "expected");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "Be", "AreEqual");
    }

    [TestMethod]
    public async Task AreEqual_BoolLiteral_ConvertsToBeTrue() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    bool flag = false;
                    {|#0:Assert.AreEqual(true, flag)|};
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
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "BeTrue", "AreEqual");
    }

    [TestMethod]
    public async Task AreEqual_Object_Converts() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object first = "a";
                    object second = "b";
                    {|#0:Assert.AreEqual(first, second)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object first = "a";
                    object second = "b";
                    second.Should().Be(first);
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1001", "Be", "AreEqual");
    }

    [TestMethod]
    public async Task AreEqual_Numeric_SuggestsNothing() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    int count = 5;
                    Assert.AreEqual(5, count);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }

    [TestMethod]
    public async Task AreEqual_Delta_SuggestsNothing() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    double value = 5.0;
                    Assert.AreEqual(5.0, value, 0.1);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }

    [TestMethod]
    public async Task AreEqual_IgnoreCase_SuggestsNothing() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    string actual = "actual";
                    Assert.AreEqual("EXPECTED", actual, true);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }

    [TestMethod]
    public async Task AreEqual_CustomAssert_SuggestsNothing() {
        const string source = """
            public static class Assert {
                public static void AreEqual(object expected, object actual) { }
            }

            public class SampleTests {
                public void Check() {
                    Assert.AreEqual("a", "b");
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticAsync(source);
    }
}
