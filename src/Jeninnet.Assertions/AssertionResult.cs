//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// The outcome of running assertions as a validator: the same fluent verbs, executed without throwing, whose
/// failures are collected into a structured result instead of surfacing as an exception.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Create"/> runs the delegate inside a soft collection (<see cref="SoftAssertions"/>), so assertion
/// failures are collected and every verb is still evaluated; the resulting <see cref="AssertionResult"/> reports
/// <see cref="IsSuccess"/> and the collected <see cref="Failures"/> for building production validators. The
/// default throwing path is unchanged — outside a validator, assertions still throw on the first failure.
/// </para>
/// <para>
/// Assertions that must return a value even to fail (<c>Throw*</c>, <c>ContainSingle</c>) throw immediately even
/// inside a validator: they abort the delegate. Use <see cref="ThrowIfFailed"/> to rethrow the collected failures
/// as one <see cref="AssertionFailedException"/> (mirrors <see cref="SoftAssertions.ThenReport"/>), and
/// <see cref="ToReportJson"/> to attach the failures to logs or CI.
/// </para>
/// </remarks>
public sealed class AssertionResult {
    private readonly IReadOnlyList<AssertionFailure> _failures;

    private AssertionResult(IReadOnlyList<AssertionFailure> failures) => _failures = failures;

    /// <summary>
    /// Runs <paramref name="assertions"/> as a validator: verbs are evaluated without throwing and their failures
    /// are collected into the returned result.
    /// </summary>
    /// <param name="assertions">The assertion block to run.</param>
    /// <returns>A result holding <see langword="true"/> <see cref="IsSuccess"/> only when the block produced no failures.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A verb that must return a value failed (e.g. <c>Throw*</c>); it also aborts the block.</exception>
    /// <example>
    /// <code>
    /// var result = AssertionResult.Create(() => {
    ///     order.Total.Should().BeGreaterThan(0);
    ///     order.Customer.Should().NotBeNull();
    /// });
    /// </code>
    /// </example>
    public static AssertionResult Create(Action assertions) {
        _ = assertions ?? throw new ArgumentNullException(nameof(assertions));

        var soft = new SoftAssertions().Collect();
        IReadOnlyList<AssertionFailure> failures;
        try {
            assertions();
        }
        finally {
            failures = soft.FinishSilently();
        }

        return new AssertionResult(failures);
    }

    /// <summary>
    /// Runs <paramref name="assertions"/> as an asynchronous validator: verbs are evaluated without throwing and
    /// their failures are collected into the returned result.
    /// </summary>
    /// <param name="assertions">The asynchronous assertion block to run.</param>
    /// <returns>A result holding <see langword="true"/> <see cref="IsSuccess"/> only when the block produced no failures.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A verb that must return a value failed (e.g. <c>Throw*</c>); it also aborts the block.</exception>
    public static Task<AssertionResult> CreateAsync(Func<Task> assertions) {
        _ = assertions ?? throw new ArgumentNullException(nameof(assertions));
        return CreateCoreAsync(assertions);
    }

    /// <summary>Runs the asynchronous block under a collecting scope and packages the collected failures.</summary>
    /// <param name="assertions">The asynchronous assertion block to run.</param>
    /// <returns>The result holding the failures collected when the block completed or threw.</returns>
    private static async Task<AssertionResult> CreateCoreAsync(Func<Task> assertions) {
        var soft = new SoftAssertions().Collect();
        IReadOnlyList<AssertionFailure> failures;
        try {
            await assertions().ConfigureAwait(false);
        }
        finally {
            failures = soft.FinishSilently();
        }

        return new AssertionResult(failures);
    }

    /// <summary>Gets <see langword="true"/> when the validator block produced no failures.</summary>
    public bool IsSuccess => _failures.Count == 0;

    /// <summary>Gets the structured failures collected by the validator block, in report order.</summary>
    public IReadOnlyList<AssertionFailure> Failures => _failures;

    /// <summary>
    /// Serializes the collected failures into the same human-readable JSON as
    /// <see cref="SoftAssertions.ToReportJson"/>: each entry carries <c>expression</c>, <c>category</c>,
    /// <c>message</c>, and the optional <c>actual</c>/<c>expected</c> values. Never throws.
    /// </summary>
    /// <returns>A compact JSON document with a <c>count</c> and a <c>failures</c> array.</returns>
    public string ToReportJson() => FailureReport.ToJson(_failures);

    /// <summary>
    /// Throws a single <see cref="AssertionFailedException"/> aggregating the collected failures, exactly like
    /// <see cref="SoftAssertions.ThenReport"/>; a no-op on a successful result. Ideal for turning a validator
    /// result back into a test assertion.
    /// </summary>
    /// <exception cref="AssertionFailedException">At least one failure was collected.</exception>
    public void ThrowIfFailed() {
        if(IsSuccess) {
            return;
        }

        if(_failures.Count == 1) {
            throw new AssertionFailedException(_failures[0].Message);
        }

        var messages = new string[_failures.Count];
        for(var i = 0; i < _failures.Count; i++) {
            messages[i] = _failures[i].Message;
        }

        throw new AssertionFailedException(AssertionScope.BuildAggregateMessage(messages, null));
    }
}
