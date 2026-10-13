using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;
using Xunit;

namespace Jeninnet.Assertions.FrameworkZoo.Xunit.Tests;

public class FizzBuzzTests {
    private readonly FizzBuzz _fizzBuzz = new();
    private readonly FizzBuzz _customFizzBuzz = new(2, "Even", 7, "Lucky");

    [Theory]
    [InlineData(1, "1")]
    [InlineData(3, "Fizz")]
    [InlineData(5, "Buzz")]
    [InlineData(15, "FizzBuzz")]
    public void Numbers_convert_to_their_representation(int value, string expected) =>
        _fizzBuzz.Convert(value).Should().Be(expected);

    [Fact]
    public void A_range_contains_one_entry_per_number() =>
        _fizzBuzz.ConvertRange(1, 15)
            .Should().HaveCount(15)
            .And.Contain("FizzBuzz")
            .And.NotContain("16");

    [Fact]
    public void Custom_words_are_configurable() =>
        _customFizzBuzz.Convert(14).Should().Be("EvenLucky", "14 is divisible by both custom divisors");
}
