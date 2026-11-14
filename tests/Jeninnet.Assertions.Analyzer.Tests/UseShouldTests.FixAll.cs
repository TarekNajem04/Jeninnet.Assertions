//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

public partial class UseShouldTests {
    [TestMethod]
    public async Task FixAll_ConvertsEveryOccurrenceInDocument() {
        const string source = """
            using System.Collections.Generic;
            using Xunit;

            public class SampleTests {
                public void Check() {
                    List<string> first = [];
                    List<string> second = [];
                    {|#0:Assert.Empty(first)|};
                    {|#1:Assert.Empty(second)|};
                }
            }
            """;
        const string fixedSource = """
            using System.Collections.Generic;
            using Xunit;
            using Jeninnet.Assertions;

            public class SampleTests {
                public void Check() {
                    List<string> first = [];
                    List<string> second = [];
                    first.Should().BeEmpty();
                    second.Should().BeEmpty();
                }
            }
            """;
        await UseShouldVerifier.VerifyFixAllWithReferencesAsync(source, fixedSource, "JNA1004", XUNIT_STUB, "BeEmpty", "Empty");
    }
}
