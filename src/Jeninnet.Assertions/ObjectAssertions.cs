//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying objects of type <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The static type of the object under assertion.</typeparam>
/// <param name="value">The object value to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class ObjectAssertions<T>(T? value, string? expression = null) : AssertionsBase(expression, "object") {
    private readonly T? _value = value;

    /// <summary>Gets the value under assertion (for comparer extensions in this assembly).</summary>
    internal T? Subject => _value;

    private static string Format<TValue>(TValue value) => AssertionScope.FormatInvariant(value);

    /// <summary>Asserts that the value equals <paramref name="expected"/> using the default equality comparer.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are not equal.</exception>
    public AndConstraint<ObjectAssertions<T>> Be(T? expected, string? because = null, params object?[] becauseArgs) {
        if(!EqualityComparer<T?>.Default.Equals(_value, expected)) {
            Fail(Strings.Format(StringKeys.OBJECT_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/> using the default equality comparer.</summary>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The values are equal.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBe(T? unexpected, string? because = null, params object?[] becauseArgs) {
        if(EqualityComparer<T?>.Default.Equals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.OBJECT_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> candidates using the default equality comparer.</summary>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value equals none of the candidates.</exception>
    public AndConstraint<ObjectAssertions<T>> BeOneOf(IEnumerable<T> expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!expected.Contains(_value!)) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_ONE_OF, Expression, string.Join(", ", expected.Select(Format)), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the object graph is structurally equivalent to <paramref name="expectation"/>.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="expectation">The expected object graph.</param>
    /// <param name="config">Optional equivalency configuration (exclusions, ordering, by-value types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The graphs differ.</exception>
    public AndConstraint<ObjectAssertions<T>> BeEquivalentTo<TExpectation>(
        TExpectation expectation,
        Func<SelfReferenceEquivalencyOptions<TExpectation>, SelfReferenceEquivalencyOptions<TExpectation>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SelfReferenceEquivalencyOptions<TExpectation>()) ?? new SelfReferenceEquivalencyOptions<TExpectation>();
        var differences = EquivalencyEngine.FindDifferences(_value, expectation, options);
        if(differences.Count == 1) {
            Fail(AppendDiagnostics(Strings.Format(StringKeys.EQUIVALENCE_DETAIL, Expression, differences[0].TrimEnd('.')), options, _value, expectation), because, becauseArgs);
        } else if(differences.Count > 1) {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "Expected {0} to be equivalent, but found {1} difference(s):", Expression, AssertionScope.FormatInvariant(differences.Count));
            for(var index = 0; index < differences.Count; index++) {
                builder.AppendLine().AppendFormat(CultureInfo.InvariantCulture, "{0}. {1}", AssertionScope.FormatInvariant(index + 1), differences[index]);
            }

            Fail(AppendDiagnostics(builder.ToString(), options, _value, expectation), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Appends the configuration summary and, when opted in, full dumps of both graphs.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="message">The failure message so far.</param>
    /// <param name="options">The equivalency configuration.</param>
    /// <param name="subject">The actual object graph.</param>
    /// <param name="expectation">The expected object graph.</param>
    /// <returns><paramref name="message"/> with diagnostics appended when tracing or dumping is on.</returns>
    private static string AppendDiagnostics<TExpectation>(
        string message,
        SelfReferenceEquivalencyOptions<TExpectation> options,
        T? subject,
        TExpectation? expectation) =>
        EquivalencyDiagnostics.Append(message, options, subject, expectation);

    /// <summary>Asserts that the JSON serialization of the value matches the committed snapshot.</summary>
    /// <remarks>Serialized by runtime type with indented, invariant JSON; cyclic graphs throw instead of silently dropping data.</remarks>
    /// <param name="snapshotName">The snapshot name (<c>{name}.verified.json</c>), or <see langword="null"/> to derive it from the subject expression.</param>
    /// <param name="snapshotsDirectory">The snapshots folder, or <see langword="null"/> for <c>__snapshots__</c> next to the calling source file.</param>
    /// <param name="config">Optional snapshot configuration (explicit scrubbers).</param>
    /// <param name="callerFilePath">The calling source file (provided by the compiler).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentException">The snapshot name is empty or unsafe.</exception>
    /// <exception cref="AssertionFailedException">No verified snapshot exists, or the content differs (a <c>.received</c> file is written for review).</exception>
    public AndConstraint<ObjectAssertions<T>> MatchSnapshot(
        string? snapshotName = null,
        string? snapshotsDirectory = null,
        Func<SnapshotOptions, SnapshotOptions>? config = null,
        [CallerFilePath] string? callerFilePath = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SnapshotOptions()) ?? new SnapshotOptions();
        var json = _value is null ? "null" : JsonSerializer.Serialize(_value, _value.GetType(), Snapshot.JsonOptions);
        // System.Text.Json emits Environment.NewLine; normalize so snapshots are byte-identical on every machine.
        // Text snapshots stay exact (see Snapshot docs); only generator output is normalized here.
        Snapshot.Verify(json.Replace("\r\n", "\n").Replace('\r', '\n'), snapshotName, snapshotsDirectory, ".json", options, new(Expression, callerFilePath, because, becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that the value refers to the same instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected instance (compared by reference).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is a different instance.</exception>
    public AndConstraint<ObjectAssertions<T>> BeSameAs(T? expected, string? because = null, params object?[] becauseArgs) {
        if(!ReferenceEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_SAME_INSTANCE, Expression, Format(expected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value does not refer to the same instance as <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The instance the subject must not be.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is the same instance.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBeSameAs(T? unexpected, string? because = null, params object?[] becauseArgs) {
        if(ReferenceEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.OBJECT_NOT_BE_SAME_INSTANCE, Expression, Format(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the run-time type of the value is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not <see langword="null"/>.</exception>
    public AndConstraint<ObjectAssertions<T>> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.OBJECT_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the run-time type of the value exactly matches <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The expected run-time type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or its run-time type differs.</exception>
    public AndConstraint<ObjectAssertions<T>> BeOfType<TExpected>(string? because = null, params object?[] becauseArgs) {
        var actualType = _value as Type ?? _value?.GetType();
        if(actualType != typeof(TExpected)) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_OF_TYPE, Expression, typeof(TExpected).Name, actualType?.Name ?? "null"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the run-time type of the value does not exactly match <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The run-time type the value must not have.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The run-time type exactly matches <typeparamref name="TExpected"/>.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBeOfType<TExpected>(string? because = null, params object?[] becauseArgs) {
        var actualType = _value as Type ?? _value?.GetType();
        if(actualType == typeof(TExpected)) {
            Fail(Strings.Format(StringKeys.OBJECT_NOT_BE_OF_TYPE, Expression, typeof(TExpected).Name, actualType?.Name ?? "null"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is assignable to type <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The type that the value must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or is not assignable to the expected type.</exception>
    public AndConstraint<ObjectAssertions<T>> BeAssignableTo<TExpected>(string? because = null, params object?[] becauseArgs) {
        if(_value is not TExpected) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_ASSIGNABLE_TO, Expression, typeof(TExpected).Name, _value?.GetType().Name ?? "null"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not assignable to type <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The type that the value must not be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is assignable to the unexpected type.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBeAssignableTo<TExpected>(string? because = null, params object?[] becauseArgs) {
        if(_value is TExpected) {
            Fail(Strings.Format(StringKeys.OBJECT_NOT_BE_ASSIGNABLE_TO, Expression, typeof(TExpected).Name, _value?.GetType().Name ?? "null"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the value must satisfy. Exceptions thrown by the predicate propagate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or does not satisfy <paramref name="predicate"/>.</exception>
    public AndConstraint<ObjectAssertions<T>> Match(Expression<Func<T, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value is null || !predicate.Compile().Invoke(_value)) {
            Fail(Strings.Format(StringKeys.OBJECT_MATCH, Expression, predicate), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the object graph is not structurally equivalent to the specified expectation.</summary>
    /// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
    /// <param name="unexpected">The expected object graph that must not match.</param>
    /// <param name="config">Optional equivalency configuration (exclusions, ordering, by-value types).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The graphs are equivalent.</exception>
    public AndConstraint<ObjectAssertions<T>> NotBeEquivalentTo<TExpectation>(
        TExpectation unexpected,
        Func<SelfReferenceEquivalencyOptions<TExpectation>, SelfReferenceEquivalencyOptions<TExpectation>>? config = null,
        string? because = null,
        params object?[] becauseArgs) {
        var options = config?.Invoke(new SelfReferenceEquivalencyOptions<TExpectation>()) ?? new SelfReferenceEquivalencyOptions<TExpectation>();
        var differences = EquivalencyEngine.FindDifferences(_value, unexpected, options);
        if(differences.Count == 0) {
            Fail(AppendDiagnostics(Strings.Format(StringKeys.OBJECT_NOT_BE_EQUIVALENT, Expression, Format(unexpected)), options, _value, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value survives an <see cref="XmlSerializer"/> round-trip unchanged.</summary>
    /// <remarks>Round-trips are slow by design: use this to verify serialization contracts, not for fast comparisons.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Serialization fails, or the round-tripped graph differs.</exception>
    public AndConstraint<ObjectAssertions<T>> BeXmlSerializable(string? because = null, params object?[] becauseArgs) {
        T? roundTripped;
        try {
            var runtimeType = _value?.GetType() ?? typeof(T);
            var serializer = new XmlSerializer(runtimeType);
            using var writer = new StringWriter(CultureInfo.InvariantCulture);
            serializer.Serialize(writer, _value);
            var settings = new XmlReaderSettings {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using var stringReader = new StringReader(writer.ToString());
            using var reader = XmlReader.Create(stringReader, settings);
            roundTripped = (T?)serializer.Deserialize(reader);
        }
        catch(Exception ex) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_XML_SERIALIZABLE, Expression, "serialization failed: " + ex.Message.TrimEnd('.')), because, becauseArgs, ex);
            return new(this);
        }

        AssertRoundTripped(roundTripped, StringKeys.OBJECT_BE_XML_SERIALIZABLE, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the value survives a <see cref="JsonSerializer"/> round-trip unchanged.</summary>
    /// <remarks>Round-trips are slow by design: use this to verify serialization contracts, not for fast comparisons.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Serialization fails, or the round-tripped graph differs.</exception>
    public AndConstraint<ObjectAssertions<T>> BeJsonSerializable(string? because = null, params object?[] becauseArgs) {
        T? roundTripped;
        try {
            var runtimeType = _value?.GetType() ?? typeof(T);
            roundTripped = (T?)JsonSerializer.Deserialize(JsonSerializer.Serialize(_value, runtimeType), runtimeType);
        }
        catch(Exception ex) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_JSON_SERIALIZABLE, Expression, "serialization failed: " + ex.Message.TrimEnd('.')), because, becauseArgs, ex);
            return new(this);
        }

        AssertRoundTripped(roundTripped, StringKeys.OBJECT_BE_JSON_SERIALIZABLE, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that the value survives a <see cref="DataContractSerializer"/> round-trip unchanged.</summary>
    /// <remarks>Round-trips are slow by design: use this to verify serialization contracts, not for fast comparisons.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Serialization fails, or the round-tripped graph differs.</exception>
    public AndConstraint<ObjectAssertions<T>> BeDataContractSerializable(string? because = null, params object?[] becauseArgs) {
        T? roundTripped;
        try {
            var runtimeType = _value?.GetType() ?? typeof(T);
            var serializer = new DataContractSerializer(runtimeType);
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, _value);
            stream.Position = 0;
            roundTripped = (T?)serializer.ReadObject(stream);
        }
        catch(Exception ex) {
            Fail(Strings.Format(StringKeys.OBJECT_BE_DATA_CONTRACT_SERIALIZABLE, Expression, "serialization failed: " + ex.Message.TrimEnd('.')), because, becauseArgs, ex);
            return new(this);
        }

        AssertRoundTripped(roundTripped, StringKeys.OBJECT_BE_DATA_CONTRACT_SERIALIZABLE, because, becauseArgs);
        return new(this);
    }

    /// <summary>Compares a round-tripped value against the original and reports the first difference.</summary>
    /// <param name="roundTripped">The value produced by the serializer.</param>
    /// <param name="key">The localized message template key for the serialization kind.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    private void AssertRoundTripped(T? roundTripped, string key, string? because, object?[]? becauseArgs) {
        List<string> differences;
        try {
            differences = EquivalencyEngine.FindDifferences(roundTripped, _value, new SelfReferenceEquivalencyOptions<T>());
        }
        catch(Exception ex) {
            Fail(Strings.Format(key, Expression, "the round-trip could not be compared: " + ex.Message.TrimEnd('.')), because, becauseArgs, ex);
            return;
        }

        if(differences.Count != 0) {
            Fail(
                AssertionScope.AppendBecause(Strings.Format(key, Expression, "the round-trip differed: " + differences[0].TrimEnd('.')), because, becauseArgs));
        }
    }
}
