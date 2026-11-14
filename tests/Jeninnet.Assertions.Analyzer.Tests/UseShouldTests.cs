//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

[TestClass]
public partial class UseShouldTests {
    private const string XUNIT_STUB = """
        namespace Xunit {
            public static class Assert {
                public static void Equal<T>(T expected, T actual) { }
                public static void NotEqual<T>(T expected, T actual) { }
                public static void True(bool condition) { }
                public static void False(bool condition) { }
                public static void Null(object? value) { }
                public static void NotNull(object? value) { }
                public static void Contains(string expectedSubstring, string actualString) { }
                public static void Contains<T>(T expected, System.Collections.Generic.IEnumerable<T> collection) { }
                public static void DoesNotContain<T>(T expected, System.Collections.Generic.IEnumerable<T> collection) { }
                public static void StartsWith(string expectedStartString, string actualString) { }
                public static void EndsWith(string expectedEndString, string actualString) { }
                public static void Empty(System.Collections.IEnumerable collection) { }
                public static void NotEmpty(System.Collections.IEnumerable collection) { }
                public static object? Single(System.Collections.IEnumerable collection) => null;
                public static T IsType<T>(object? value) => default!;
                public static T IsAssignableFrom<T>(object? value) => default!;
                public static T Throws<T>(System.Action action) where T : System.Exception => default!;
                public static System.Threading.Tasks.Task<T> ThrowsAsync<T>(System.Func<System.Threading.Tasks.Task> action) where T : System.Exception => null!;
            }
        }
        """;

    private const string NUNIT_CLASSIC_STUB = """
        namespace NUnit.Framework {
            public delegate void TestDelegate();
            public delegate System.Threading.Tasks.Task AsyncTestDelegate();
            public static class Assert {
                public static void AreEqual<T>(T expected, T actual) { }
                public static void IsTrue(bool condition) { }
                public static void IsEmpty(System.Collections.IEnumerable collection) { }
                public static void IsNotEmpty(System.Collections.IEnumerable collection) { }
                public static void Contains(object expected, System.Collections.ICollection collection) { }
                public static void DoesNotContain(object expected, System.Collections.ICollection collection) { }
                public static T IsInstanceOf<T>(object? value) => default!;
                public static T IsAssignableFrom<T>(object? value) => default!;
                public static T Throws<T>(TestDelegate code) where T : System.Exception => default!;
                public static System.Threading.Tasks.Task<T> ThrowsAsync<T>(AsyncTestDelegate code) where T : System.Exception => null!;
            }
            public static class StringAssert {
                public static void Contains(string actual, string expected) { }
                public static void StartsWith(string actual, string expected) { }
                public static void EndsWith(string actual, string expected) { }
            }
        }
        """;

    private const string NUNIT_CONSTRAINT_STUB = """
        namespace NUnit.Framework {
            public static class Assert {
                public static void That<T>(T actual, object constraint) { }
                public static void That<T>(T actual, object constraint, string message) { }
            }
            public static class Is {
                public static object EqualTo(object expected) => null!;
                public static object GreaterThan(int expected) => null!;
                public static object True => null!;
                public static object False => null!;
                public static object Null => null!;
                public static IsNot Not => null!;
            }
            public sealed class IsNot {
                public object EqualTo(object expected) => null!;
                public object Null => null!;
            }
        }
        """;
}
