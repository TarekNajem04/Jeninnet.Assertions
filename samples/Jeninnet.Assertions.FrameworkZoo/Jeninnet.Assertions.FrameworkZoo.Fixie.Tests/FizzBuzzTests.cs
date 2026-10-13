using Jeninnet.Assertions;
using Jeninnet.Assertions.FrameworkZoo.Core;

namespace Jeninnet.Assertions.FrameworkZoo.Fixie.Tests;

public class FizzBuzzTests {
    private readonly FizzBuzz _fizzBuzz = new();
    private readonly FizzBuzz _customFizzBuzz = new(2, "Even", 7, "Lucky");

    public void Numbers_convert_to_their_representation() {
        _fizzBuzz.Convert(1).Should().Be("1");
        _fizzBuzz.Convert(3).Should().Be("Fizz");
        _fizzBuzz.Convert(5).Should().Be("Buzz");
        _fizzBuzz.Convert(15).Should().Be("FizzBuzz");
    }

    public void A_range_contains_one_entry_per_number() =>
        _fizzBuzz.ConvertRange(1, 15)
            .Should().HaveCount(15)
            .And.Contain("FizzBuzz")
            .And.NotContain("16");

    public void Custom_words_are_configurable() =>
        _customFizzBuzz.Convert(14).Should().Be("EvenLucky", "14 is divisible by both custom divisors");
}
