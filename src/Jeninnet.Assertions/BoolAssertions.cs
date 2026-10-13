//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying <see cref="bool"/> values.</summary>
/// <param name="value">The boolean value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class BoolAssertions(bool value, string? expression = null) : AssertionsBase(expression, "value") {
    private readonly bool _value = value;

    /// <summary>Asserts that the boolean value is <see langword="true"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="false"/>.</exception>
    public AndConstraint<BoolAssertions> BeTrue(string? because = null, params object?[] becauseArgs) {
        if(!_value) {
            Fail(Strings.Format(StringKeys.BOOLEAN_BE_TRUE, Expression), because, becauseArgs, Format(_value), Format(true));
        }

        return new(this);
    }

    /// <summary>Asserts that the boolean value is <see langword="false"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="true"/>.</exception>
    public AndConstraint<BoolAssertions> BeFalse(string? because = null, params object?[] becauseArgs) {
        if(_value) {
            Fail(Strings.Format(StringKeys.BOOLEAN_BE_FALSE, Expression), because, becauseArgs, Format(_value), Format(false));
        }

        return new(this);
    }

    /// <summary>Asserts that the boolean value is not <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value equals <paramref name="unexpected"/>.</exception>
    public AndConstraint<BoolAssertions> NotBe(bool unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.BOOLEAN_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts logical implication (<c>value → operand</c>, i.e. <c>!value || operand</c>).</summary>
    /// <param name="operand">The consequent of the implication.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="true"/> while <paramref name="operand"/> is <see langword="false"/>.</exception>
    public AndConstraint<BoolAssertions> Imply(bool operand, string? because = null, params object?[] becauseArgs) {
        if(_value && !operand) {
            Fail(Strings.Format(StringKeys.BOOLEAN_IMPLY, Expression, Format(operand), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    private static string Format(bool value) => value ? "true" : "false";
}
