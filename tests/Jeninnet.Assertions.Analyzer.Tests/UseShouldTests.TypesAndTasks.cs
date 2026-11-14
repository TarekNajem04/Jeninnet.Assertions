//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task XUnit_IsType_ConvertsToBeOfType() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    {|#0:Assert.IsType<string>(actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    actual.Should().BeOfType<string>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1006", XUNIT_STUB, "BeOfType", "IsType");
    }

    [TestMethod]
    public async Task MSTest_IsInstanceOfType_ConvertsToBeAssignableTo() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    {|#0:Assert.IsInstanceOfType(actual, typeof(string))|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    actual.Should().BeAssignableTo<string>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1006", "BeAssignableTo", "IsInstanceOfType");
    }

    [TestMethod]
    public async Task MSTest_IsExactInstanceOfType_ConvertsToBeOfType() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    {|#0:Assert.IsExactInstanceOfType(actual, typeof(string))|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    actual.Should().BeOfType<string>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1006", "BeOfType", "IsExactInstanceOfType");
    }

    [TestMethod]
    public async Task XUnit_IsAssignableFrom_ConvertsToBeAssignableTo() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    {|#0:Assert.IsAssignableFrom<string>(actual)|};
                }
            }
            """;
        const string fixedSource = """
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    actual.Should().BeAssignableTo<string>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1006", XUNIT_STUB, "BeAssignableTo", "IsAssignableFrom");
    }

    [TestMethod]
    public async Task NUnit_IsAssignableFrom_ConvertsToBeAssignableTo() {
        const string source = """
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    {|#0:Assert.IsAssignableFrom<string>(actual)|};
                }
            }
            """;
        const string fixedSource = """
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    actual.Should().BeAssignableTo<string>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1006", NUNIT_CLASSIC_STUB, "BeAssignableTo", "IsAssignableFrom");
    }

    [TestMethod]
    public async Task IsType_ResultUsed_SuggestsNothing() {
        const string source = """
            using Xunit;

            public class SampleTests {
                public void Check() {
                    object actual = "text";
                    var text = Assert.IsType<string>(actual);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }

    [TestMethod]
    public async Task XUnit_TrueTaskIsCompleted_ConvertsToBeCompleted() {
        const string source = """
            using System.Threading.Tasks;
            using Xunit;

            public class SampleTests {
                public void Check(Task task) {
                    {|#0:Assert.True(task.IsCompleted)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Threading.Tasks;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check(Task task) {
                    task.Should().BeCompleted();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1007", XUNIT_STUB, "BeCompleted", "True");
    }

    [TestMethod]
    public async Task MSTest_FalseTaskIsCompleted_ConvertsToNotBeCompleted() {
        const string source = """
            using System.Threading.Tasks;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check(Task<int> task) {
                    {|#0:Assert.IsFalse(task.IsCompleted)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Threading.Tasks;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check(Task<int> task) {
                    task.Should().NotBeCompleted();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1007", "NotBeCompleted", "IsFalse");
    }

    [TestMethod]
    public async Task Condition_NonTaskProperty_StillUsesConditionRule() {
        const string source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check(bool flag) {
                    {|#0:Assert.IsTrue(flag)|};
                }
            }
            """;
        const string fixedSource = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check(bool flag) {
                    flag.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1002", "BeTrue", "IsTrue");
    }
}
