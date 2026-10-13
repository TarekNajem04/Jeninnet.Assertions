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
            }
        }
        """;

    private const string NUNIT_CLASSIC_STUB = """
        namespace NUnit.Framework {
            public static class Assert {
                public static void AreEqual<T>(T expected, T actual) { }
                public static void IsTrue(bool condition) { }
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
