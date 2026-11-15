//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    private const string SHOULDLY_STUB = """
        namespace Shouldly {
            public static class ShouldlyExtensions {
                public static void ShouldBe<T>(this T actual, T expected) { }
                public static void ShouldNotBe<T>(this T actual, T expected) { }
                public static void ShouldBeNull<T>(this T actual) { }
                public static void ShouldNotBeNull<T>(this T actual) { }
                public static void ShouldBeTrue(this bool actual) { }
                public static void ShouldBeFalse(this bool actual) { }
                public static void ShouldContain(this string actual, string expected) { }
                public static void ShouldNotContain(this string actual, string expected) { }
                public static void ShouldBeEmpty<T>(this System.Collections.Generic.IEnumerable<T> actual) { }
                public static void ShouldNotBeEmpty<T>(this System.Collections.Generic.IEnumerable<T> actual) { }
                public static T ShouldThrow<T>(this System.Action action) where T : System.Exception => default!;
                public static void ShouldNotThrow(this System.Action action) { }
            }
        }
        """;

    private const string NFLUENT_STUB = """
        namespace NFluent {
            public interface ICheck<T> { }

            public static class Check {
                public static ICheck<T> That<T>(T value) => default!;
                public static ICheck<System.Collections.Generic.IEnumerable<T>> That<T>(
                    System.Collections.Generic.IEnumerable<T> value) => default!;
                public static ICheck<System.Action> ThatCode(System.Action value) => default!;
            }

            public static class ObjectCheckExtensions {
                public static ICheck<T> IsEqualTo<T>(this ICheck<T> check, T expected) => check;
                public static ICheck<T> IsNotEqualTo<T>(this ICheck<T> check, T expected) => check;
                public static ICheck<T> IsNull<T>(this ICheck<T> check) => check;
                public static ICheck<T> IsNotNull<T>(this ICheck<T> check) => check;
            }

            public static class BooleanCheckExtensions {
                public static ICheck<bool> IsTrue(this ICheck<bool> check) => check;
                public static ICheck<bool> IsFalse(this ICheck<bool> check) => check;
            }

            public static class StringCheckExtensions {
                public static ICheck<string> Contains(this ICheck<string> check, string expected) => check;
            }

            public static class EnumerableCheckExtensions {
                public static ICheck<System.Collections.Generic.IEnumerable<T>> HasSize<T>(
                    this ICheck<System.Collections.Generic.IEnumerable<T>> check, long size) => check;
                public static ICheck<System.Collections.Generic.IEnumerable<T>> IsEmpty<T>(
                    this ICheck<System.Collections.Generic.IEnumerable<T>> check) => check;
                public static ICheck<System.Collections.Generic.IEnumerable<T>> IsNotEmpty<T>(
                    this ICheck<System.Collections.Generic.IEnumerable<T>> check) => check;
            }

            public static class CodeCheckExtensions {
                public static ICheck<System.Action> Throws<T>(this ICheck<System.Action> check) where T : System.Exception => check;
            }
        }
        """;

    [TestMethod]
    public async Task Shouldly_ShouldBe_ConvertsToBe() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    int actual = 1;
                    {|#0:actual.ShouldBe(1)|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    int actual = 1;
                    actual.Should().Be(1);
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "Be", "ShouldBe");
    }

    [TestMethod]
    public async Task Shouldly_ShouldBeNull_ConvertsToBeNull() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    string? actual = null;
                    {|#0:actual.ShouldBeNull()|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string? actual = null;
                    actual.Should().BeNull();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "BeNull", "ShouldBeNull");
    }

    [TestMethod]
    public async Task Shouldly_ShouldBeTrue_ConvertsToBeTrue() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    bool actual = true;
                    {|#0:actual.ShouldBeTrue()|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool actual = true;
                    actual.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "BeTrue", "ShouldBeTrue");
    }

    [TestMethod]
    public async Task Shouldly_ShouldBeEmpty_ConvertsToBeEmpty() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    int[] actual = [];
                    {|#0:actual.ShouldBeEmpty()|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    int[] actual = [];
                    actual.Should().BeEmpty();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "BeEmpty", "ShouldBeEmpty");
    }

    [TestMethod]
    public async Task Shouldly_ShouldContain_ConvertsToContain() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    string actual = "abc";
                    {|#0:actual.ShouldContain("b")|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    string actual = "abc";
                    actual.Should().Contain("b");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "Contain", "ShouldContain");
    }

    [TestMethod]
    public async Task Shouldly_ShouldThrow_ConvertsToThrow() {
        const string source = """
            using Shouldly;
            using System;

            public class SampleTests {
                public void Check() {
                    Action action = () => throw new InvalidOperationException();
                    {|#0:action.ShouldThrow<InvalidOperationException>()|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using System;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    Action action = () => throw new InvalidOperationException();
                    action.Should().Throw<InvalidOperationException>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "Throw", "ShouldThrow");
    }

    [TestMethod]
    public async Task Shouldly_ConsumedExceptionResult_IsNotConverted() {
        const string source = """
            using Shouldly;
            using System;

            public class SampleTests {
                public void Check() {
                    Action action = () => throw new InvalidOperationException();
                    var exception = action.ShouldThrow<InvalidOperationException>();
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, SHOULDLY_STUB);
    }

    [TestMethod]
    public async Task Shouldly_FixAll_ConvertsBothCalls() {
        const string source = """
            using Shouldly;

            public class SampleTests {
                public void Check() {
                    int first = 1;
                    int second = 2;
                    {|#0:first.ShouldBe(1)|};
                    {|#1:second.ShouldBe(2)|};
                }
            }
            """;
        const string fixedSource = """
            using Shouldly;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    int first = 1;
                    int second = 2;
                    first.Should().Be(1);
                    second.Should().Be(2);
                }
            }
            """;
        await UseShouldVerifier.VerifyFixAllWithReferencesAsync(source, fixedSource, "JNA2001", SHOULDLY_STUB, "Be", "ShouldBe");
    }

    [TestMethod]
    public async Task Nfluent_IsEqualTo_ConvertsToBe() {
        const string source = """
            using NFluent;

            public class SampleTests {
                public void Check() {
                    int actual = 1;
                    {|#0:NFluent.Check.That(actual).IsEqualTo(1)|};
                }
            }
            """;
        const string fixedSource = """
            using NFluent;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    int actual = 1;
                    actual.Should().Be(1);
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2002", NFLUENT_STUB, "Be", "IsEqualTo");
    }

    [TestMethod]
    public async Task Nfluent_IsTrue_ConvertsToBeTrue() {
        const string source = """
            using NFluent;

            public class SampleTests {
                public void Check() {
                    bool actual = true;
                    {|#0:NFluent.Check.That(actual).IsTrue()|};
                }
            }
            """;
        const string fixedSource = """
            using NFluent;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    bool actual = true;
                    actual.Should().BeTrue();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2002", NFLUENT_STUB, "BeTrue", "IsTrue");
    }

    [TestMethod]
    public async Task Nfluent_HasSize_ConvertsToHaveCount() {
        const string source = """
            using NFluent;

            public class SampleTests {
                public void Check() {
                    System.Collections.Generic.IEnumerable<int> actual = [1, 2, 3];
                    {|#0:NFluent.Check.That(actual).HasSize(3)|};
                }
            }
            """;
        const string fixedSource = """
            using NFluent;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    System.Collections.Generic.IEnumerable<int> actual = [1, 2, 3];
                    actual.Should().HaveCount(3);
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2002", NFLUENT_STUB, "HaveCount", "HasSize");
    }

    [TestMethod]
    public async Task Nfluent_Throws_ConvertsToThrow() {
        const string source = """
            using NFluent;
            using System;

            public class SampleTests {
                public void Check() {
                    Action action = () => throw new InvalidOperationException();
                    {|#0:NFluent.Check.ThatCode(action).Throws<InvalidOperationException>()|};
                }
            }
            """;
        const string fixedSource = """
            using NFluent;
            using System;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    Action action = () => throw new InvalidOperationException();
                    action.Should().Throw<InvalidOperationException>();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA2002", NFLUENT_STUB, "Throw", "Throws");
    }

    [TestMethod]
    public async Task Nfluent_ConsumedCheckResult_IsNotConverted() {
        const string source = """
            using NFluent;

            public class SampleTests {
                public void Check() {
                    var check = NFluent.Check.That(1).IsEqualTo(1);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, NFLUENT_STUB);
    }
}
