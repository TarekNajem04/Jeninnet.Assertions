using System.Globalization;

namespace Jeninnet.Assertions.FrameworkZoo.Core;

/// <summary>
/// Converts numbers to a configurable FizzBuzz-style representation. The
/// parameterless configuration reproduces the classic 3/"Fizz", 5/"Buzz" rules.
/// </summary>
public sealed class FizzBuzz {
    private readonly int _firstDivisor;
    private readonly string _firstWord;
    private readonly int _secondDivisor;
    private readonly string _secondWord;

    /// <summary>Initializes the classic 3/"Fizz", 5/"Buzz" configuration.</summary>
    public FizzBuzz()
        : this(3, "Fizz", 5, "Buzz") {
    }

    /// <summary>Initializes a custom configuration of divisors and replacement words.</summary>
    /// <param name="firstDivisor">The first divisor; must be positive.</param>
    /// <param name="firstWord">The word used when a number is divisible by the first divisor.</param>
    /// <param name="secondDivisor">The second divisor; must be positive.</param>
    /// <param name="secondWord">The word used when a number is divisible by the second divisor.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a divisor is not positive.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a word is null.</exception>
    public FizzBuzz(int firstDivisor, string firstWord, int secondDivisor, string secondWord) {
        if(firstDivisor <= 0) {
            throw new ArgumentOutOfRangeException(nameof(firstDivisor), "The first divisor must be positive.");
        }

        if(secondDivisor <= 0) {
            throw new ArgumentOutOfRangeException(nameof(secondDivisor), "The second divisor must be positive.");
        }

        _firstDivisor = firstDivisor;
        _firstWord = firstWord ?? throw new ArgumentNullException(nameof(firstWord));
        _secondDivisor = secondDivisor;
        _secondWord = secondWord ?? throw new ArgumentNullException(nameof(secondWord));
    }

    /// <summary>Converts a single number to its representation.</summary>
    /// <param name="value">The number to convert; must be positive.</param>
    /// <returns>The configured word pair, a single word, or the invariant number text.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not positive.</exception>
    public string Convert(int value) {
        if(value <= 0) {
            throw new ArgumentOutOfRangeException(nameof(value), "The value to convert must be positive.");
        }

        if(value % (_firstDivisor * _secondDivisor) == 0) {
            return _firstWord + _secondWord;
        }

        if(value % _firstDivisor == 0) {
            return _firstWord;
        }

        if(value % _secondDivisor == 0) {
            return _secondWord;
        }

        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Converts every number in an inclusive range to its representation.</summary>
    /// <param name="first">The first number of the range; must be positive.</param>
    /// <param name="last">The last number of the range; must not be smaller than the first number.</param>
    /// <returns>One representation per number, in ascending order.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the range is invalid.</exception>
    public IReadOnlyList<string> ConvertRange(int first, int last) {
        if(last < first) {
            throw new ArgumentOutOfRangeException(nameof(last), "The last number must not be smaller than the first number.");
        }

        var results = new List<string>(last - first + 1);
        for(var value = first; value <= last; value++) {
            results.Add(Convert(value));
        }

        return results;
    }
}
