using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;
using Jeninnet.Assertions.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.FrameworkZoo.MSTest.Tests;

[TestClass]
public class FizzBuzzTests {
    private readonly FizzBuzz _fizzBuzz = new();
    private readonly FizzBuzz _customFizzBuzz = new(2, "Even", 7, "Lucky");

    [TestMethod]
    [DataRow(1, "1")]
    [DataRow(3, "Fizz")]
    [DataRow(5, "Buzz")]
    [DataRow(15, "FizzBuzz")]
    public void Numbers_convert_to_their_representation(int value, string expected) =>
        MSTestAssertions.That(() => _fizzBuzz.Convert(value).Should().Be(expected));

    [TestMethod]
    public void A_range_contains_one_entry_per_number() =>
        MSTestAssertions.That(() => _fizzBuzz.ConvertRange(1, 15)
            .Should().HaveCount(15)
            .And.Contain("FizzBuzz")
            .And.NotContain("16"));

    [TestMethod]
    public void Custom_words_are_configurable() =>
        MSTestAssertions.That(() => _customFizzBuzz.Convert(14).Should().Be("EvenLucky", "14 is divisible by both custom divisors"));
}
