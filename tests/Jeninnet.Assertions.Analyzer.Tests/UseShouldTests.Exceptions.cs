//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task XUnit_Throws_ConvertsToThrowExactly() {
        const string source = """
            using System;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    {|#0:Assert.Throws<InvalidOperationException>(() => Throw())|};
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        const string fixedSource = """
            using System;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    new System.Action(() => Throw()).Should().ThrowExactly<InvalidOperationException>();
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1005", XUNIT_STUB, "ThrowExactly", "Throws");
    }

    [TestMethod]
    public async Task MSTest_Throws_ConvertsToThrow() {
        const string source = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    {|#0:Assert.Throws<InvalidOperationException>(() => Throw())|};
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        const string fixedSource = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    new System.Action(() => Throw()).Should().Throw<InvalidOperationException>();
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1005", "Throw", "Throws");
    }

    [TestMethod]
    public async Task MSTest_Throws_MessageConvertsToBecause() {
        const string source = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    {|#0:Assert.Throws<InvalidOperationException>(() => Throw(), "reason")|};
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        const string fixedSource = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    new System.Action(() => Throw()).Should().Throw<InvalidOperationException>("reason");
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1005", "Throw", "Throws");
    }

    [TestMethod]
    public async Task XUnit_ThrowsAsync_ConvertsToAwaitedThrowExactlyAsync() {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using Xunit;

            public class SampleTests {
                public async Task Check() {
                    await {|#0:Assert.ThrowsAsync<InvalidOperationException>(async () => await ThrowAsync())|};
                }

                private static Task ThrowAsync() => Task.CompletedTask;
            }
            """;
        const string fixedSource = """
            using System;
            using System.Threading.Tasks;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public async Task Check() {
                    await new System.Func<System.Threading.Tasks.Task>(async () => await ThrowAsync()).Should().ThrowExactlyAsync<InvalidOperationException>();
                }

                private static Task ThrowAsync() => Task.CompletedTask;
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1005", XUNIT_STUB, "ThrowExactlyAsync", "ThrowsAsync");
    }

    [TestMethod]
    public async Task NUnit_Throws_ConvertsToThrowExactly() {
        const string source = """
            using System;
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    {|#0:Assert.Throws<InvalidOperationException>(() => Throw())|};
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        const string fixedSource = """
            using System;
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    new System.Action(() => Throw()).Should().ThrowExactly<InvalidOperationException>();
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1005", NUNIT_CLASSIC_STUB, "ThrowExactly", "Throws");
    }

    [TestMethod]
    public async Task Throws_ResultUsed_SuggestsNothing() {
        const string source = """
            using System;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    var exception = Assert.Throws<InvalidOperationException>(() => Throw());
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }

    [TestMethod]
    public async Task Throws_PlainException_SuggestsNothing() {
        const string source = """
            using System;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    Assert.Throws<Exception>(() => Throw());
                }

                private static void Throw() => throw new InvalidOperationException();
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }
}
