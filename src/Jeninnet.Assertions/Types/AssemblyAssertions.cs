//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="Assembly"/> metadata.</summary>
/// <param name="value">The assembly to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class AssemblyAssertions(Assembly? value, string? expression = null) : AssertionsBase(expression, "assembly") {
    private readonly Assembly? _value = value;

    /// <summary>Asserts that the assembly is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The assembly is not <see langword="null"/>.</exception>
    public AndConstraint<AssemblyAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.TYPE_BE_NULL, Expression, _value.FullName), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/>.</exception>
    public AndConstraint<AssemblyAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly defines a type named <paramref name="fullName"/> and exposes it for further assertions.</summary>
    /// <param name="fullName">The full name of the type (namespace + name, '+' for nested types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the defined type.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fullName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/> or defines no such type.</exception>
    public WhichConstraint<Type> DefineType(string fullName, string? because = null, params object?[] becauseArgs) {
        if(fullName is null) {
            throw new ArgumentNullException(nameof(fullName));
        }

        var type = (_value?.GetType(fullName)) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.ASSEMBLY_DEFINE_TYPE, Expression, fullName), because, becauseArgs));
        return new WhichConstraint<Type>(type);
    }

    /// <summary>Asserts that the assembly defines no type named <paramref name="fullName"/>.</summary>
    /// <param name="fullName">The full name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fullName"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The assembly defines such a type.</exception>
    public AndConstraint<AssemblyAssertions> NotDefineType(string fullName, string? because = null, params object?[] becauseArgs) {
        if(fullName is null) {
            throw new ArgumentNullException(nameof(fullName));
        }

        if(_value?.GetType(fullName) is not null) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_NOT_DEFINE_TYPE, Expression, fullName), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly references <paramref name="assembly"/>.</summary>
    /// <param name="assembly">An assembly from the referenced assembly set (only its name is compared).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/> or does not reference it.</exception>
    public AndConstraint<AssemblyAssertions> Reference(Assembly assembly, string? because = null, params object?[] becauseArgs) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_REFERENCE_NULL, Expression, assembly.GetName().Name), because, becauseArgs);
            return new(this);
        }

        var referenced = _value.GetReferencedAssemblies().Select(static name => name.Name);
        if(!referenced.Contains(assembly.GetName().Name)) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_REFERENCE_MISSING, Expression, assembly.GetName().Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly does not reference <paramref name="assembly"/>.</summary>
    /// <param name="assembly">An assembly from the referenced assembly set (only its name is compared).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/> or references it.</exception>
    public AndConstraint<AssemblyAssertions> NotReference(Assembly assembly, string? because = null, params object?[] becauseArgs) {
        if(assembly is null) {
            throw new ArgumentNullException(nameof(assembly));
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_NOT_REFERENCE_NULL, Expression, assembly.GetName().Name), because, becauseArgs);
            return new(this);
        }

        var referenced = _value.GetReferencedAssemblies().Select(static name => name.Name);
        if(referenced.Contains(assembly.GetName().Name)) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_NOT_REFERENCE_FOUND, Expression, assembly.GetName().Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly is unsigned (carries no public key).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/> or signed.</exception>
    public AndConstraint<AssemblyAssertions> BeUnsigned(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_BE_UNSIGNED_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        var key = _value.GetName().GetPublicKey();
        if(key is not null && key.Length != 0) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_BE_UNSIGNED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the assembly is signed with the given public key (hexadecimal, case-insensitive).</summary>
    /// <param name="hexKey">The expected public key as hex digits (dashes and whitespace are ignored).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="hexKey"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="hexKey"/> is not valid hexadecimal.</exception>
    /// <exception cref="AssertionFailedException">The assembly is <see langword="null"/>, unsigned, or signed with another key.</exception>
    public AndConstraint<AssemblyAssertions> BeSignedWithPublicKey(string hexKey, string? because = null, params object?[] becauseArgs) {
        if(hexKey is null) {
            throw new ArgumentNullException(nameof(hexKey));
        }

        var expected = ParseHexKey(hexKey);
        if(_value is null) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_BE_SIGNED_WITH_KEY_NULL, Expression, FormatHex(expected)), because, becauseArgs);
            return new(this);
        }

        var actual = _value.GetName().GetPublicKey();
        if(actual is null || actual.Length == 0) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_BE_SIGNED_WITH_KEY, Expression, FormatHex(expected), "<unsigned>"), because, becauseArgs);
            return new(this);
        }

        if(!actual.SequenceEqual(expected)) {
            Fail(Strings.Format(StringKeys.ASSEMBLY_BE_SIGNED_WITH_KEY, Expression, FormatHex(expected), FormatHex(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Parses a hexadecimal public key, ignoring dashes and whitespace.</summary>
    /// <param name="hexKey">The hexadecimal key text.</param>
    /// <returns>The decoded key bytes.</returns>
    /// <exception cref="ArgumentException"><paramref name="hexKey"/> is empty, has odd length, or contains non-hexadecimal characters.</exception>
    private static byte[] ParseHexKey(string hexKey) {
        var cleaned = new StringBuilder(hexKey.Length);
        foreach(var character in hexKey) {
            if(character == '-' || char.IsWhiteSpace(character)) {
                continue;
            }

            cleaned.Append(character);
        }

        if(cleaned.Length == 0 || cleaned.Length % 2 != 0) {
            throw new ArgumentException("Public key must be a non-empty even-length hexadecimal string.", nameof(hexKey));
        }

        var bytes = new byte[cleaned.Length / 2];
        for(var index = 0; index < bytes.Length; index++) {
            var high = HexValue(cleaned[index * 2]);
            var low = HexValue(cleaned[(index * 2) + 1]);
            if(high < 0 || low < 0) {
                throw new ArgumentException("Public key must be a non-empty even-length hexadecimal string.", nameof(hexKey));
            }

            bytes[index] = (byte)((high << 4) | low);
        }

        return bytes;
    }

    /// <summary>Converts one hexadecimal digit to its value.</summary>
    /// <param name="character">The digit character.</param>
    /// <returns>The digit value, or <c>-1</c> when the character is not hexadecimal.</returns>
    private static int HexValue(char character) {
        if(character is >= '0' and <= '9') {
            return character - '0';
        }

        if(character is >= 'a' and <= 'f') {
            return character - 'a' + 10;
        }

        if(character is >= 'A' and <= 'F') {
            return character - 'A' + 10;
        }

        return -1;
    }

    /// <summary>Formats key bytes as uppercase hexadecimal text for failure messages.</summary>
    /// <param name="bytes">The key bytes.</param>
    /// <returns>The uppercase hexadecimal representation.</returns>
    private static string FormatHex(byte[] bytes) {
        var builder = new StringBuilder(bytes.Length * 2);
        foreach(var value in bytes) {
            builder.Append(value.ToString("X2", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }
}
