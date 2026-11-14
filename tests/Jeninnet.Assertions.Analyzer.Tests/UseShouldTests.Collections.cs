//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task XUnit_Empty_ConvertsToBeEmpty() {
        const string source = """
            using System.Collections.Generic;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    List<string> items = [];
                    {|#0:Assert.Empty(items)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = [];
                    items.Should().BeEmpty();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1004", XUNIT_STUB, "BeEmpty", "Empty");
    }

    [TestMethod]
    public async Task XUnit_Single_ConvertsToContainSingle() {
        const string source = """
            using System.Collections.Generic;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    {|#0:Assert.Single(items)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    items.Should().ContainSingle();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1004", XUNIT_STUB, "ContainSingle", "Single");
    }

    [TestMethod]
    public async Task XUnit_CollectionContains_ConvertsToContain() {
        const string source = """
            using System.Collections.Generic;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    {|#0:Assert.Contains("a", items)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    items.Should().Contain("a");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1004", XUNIT_STUB, "Contain", "Contains");
    }

    [TestMethod]
    public async Task MSTest_CollectionAssertContains_ConvertsToContain() {
        const string source = """
            using System.Collections.Generic;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    {|#0:CollectionAssert.Contains(items, "a")|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    items.Should().Contain("a");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1004", "Contain", "Contains");
    }

    [TestMethod]
    public async Task MSTest_CollectionAssertContains_MessageConvertsToBecause() {
        const string source = """
            using System.Collections.Generic;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    {|#0:CollectionAssert.Contains(items, "a", "reason")|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    items.Should().Contain("a", "reason");
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixAsync(source, fixedSource, "JNA1004", "Contain", "Contains");
    }

    [TestMethod]
    public async Task CollectionContains_Dictionary_SuggestsNothing() {
        const string source = """
            using System.Collections.Generic;
            using Jeninnet.Assertions;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    Dictionary<string, int> items = [];
                    Assert.Contains(new KeyValuePair<string, int>("a", 1), items);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }

    [TestMethod]
    public async Task NUnit_IsEmpty_ConvertsToBeEmpty() {
        const string source = """
            using System.Collections.Generic;
            using NUnit.Framework;

            public class SampleTests {
                public void Check() {
                    List<string> items = [];
                    {|#0:Assert.IsEmpty(items)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using NUnit.Framework;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> items = [];
                    items.Should().BeEmpty();
                }
            }
            """;
        await UseShouldVerifier.VerifyCodeFixWithReferencesAsync(source, fixedSource, "JNA1004", NUNIT_CLASSIC_STUB, "BeEmpty", "IsEmpty");
    }

    [TestMethod]
    public async Task Single_ResultUsed_SuggestsNothing() {
        const string source = """
            using System.Collections.Generic;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    List<string> items = ["a"];
                    var item = Assert.Single(items);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }

    [TestMethod]
    public async Task Empty_NonGenericEnumerable_SuggestsNothing() {
        const string source = """
            using System.Collections;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    var items = new ArrayList();
                    Assert.Empty(items);
                }
            }
            """;
        await UseShouldVerifier.VerifyNoDiagnosticWithReferencesAsync(source, XUNIT_STUB);
    }
}
