//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>An ordered list of custom <see cref="IEquivalencyStep"/> rules consulted before the built-in comparison.</summary>
/// <remarks>
/// The built-in comparison is implicit and always runs last, so an empty plan is the default behavior;
/// <see cref="Reset"/> restores that default and <see cref="Clear"/> empties the list outright.
/// Register per assertion with <c>Using</c>, or process-wide with <see cref="EquivalencyDefaults.Plan"/>.
/// </remarks>
public sealed class EquivalencyPlan {
    private readonly List<IEquivalencyStep> _steps = [];
    private readonly object _sync = new();

    /// <summary>Gets a snapshot of the steps in evaluation order.</summary>
    /// <returns>The steps in evaluation order.</returns>
    public IReadOnlyList<IEquivalencyStep> Snapshot() {
        lock(_sync) {
            return [.. _steps];
        }
    }

    /// <summary>Appends steps to the end of the plan.</summary>
    /// <param name="steps">The steps to append.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains a null step.</exception>
    public EquivalencyPlan Add(params IEquivalencyStep[] steps) {
        lock(_sync) {
            _steps.AddRange(Validate(steps));
        }

        return this;
    }

    /// <summary>Inserts steps at <paramref name="index"/>, shifting later steps right.</summary>
    /// <param name="index">The zero-based position of the first inserted step.</param>
    /// <param name="steps">The steps to insert.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains a null step.</exception>
    public EquivalencyPlan Insert(int index, params IEquivalencyStep[] steps) {
        var validated = Validate(steps);
        lock(_sync) {
            if(index < 0 || index > _steps.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _steps.InsertRange(index, validated);
        }

        return this;
    }

    /// <summary>Inserts steps immediately before the first step anchored at <paramref name="reference"/>.</summary>
    /// <remarks>An instance anchors at itself when present, otherwise at the first step of its runtime type.</remarks>
    /// <typeparam name="TReference">The anchor type; a <see cref="Type"/> reference matches steps of exactly that type.</typeparam>
    /// <param name="reference">The anchor: a step instance or a <see cref="Type"/>.</param>
    /// <param name="steps">The steps to insert.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> or <paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains a null step.</exception>
    /// <exception cref="InvalidOperationException">No step matches <paramref name="reference"/>.</exception>
    public EquivalencyPlan AddBefore<TReference>(TReference reference, params IEquivalencyStep[] steps) {
        var validated = Validate(steps);
        lock(_sync) {
            var index = IndexOfAnchor(reference);
            if(index < 0) {
                throw new InvalidOperationException($"No equivalency step matches '{FormatAnchor(reference)}'.");
            }

            _steps.InsertRange(index, validated);
        }

        return this;
    }

    /// <summary>Inserts steps immediately after the first step anchored at <paramref name="reference"/>.</summary>
    /// <remarks>An instance anchors at itself when present, otherwise at the first step of its runtime type.</remarks>
    /// <typeparam name="TReference">The anchor type; a <see cref="Type"/> reference matches steps of exactly that type.</typeparam>
    /// <param name="reference">The anchor: a step instance or a <see cref="Type"/>.</param>
    /// <param name="steps">The steps to insert.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> or <paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains a null step.</exception>
    /// <exception cref="InvalidOperationException">No step matches <paramref name="reference"/>.</exception>
    public EquivalencyPlan AddAfter<TReference>(TReference reference, params IEquivalencyStep[] steps) {
        var validated = Validate(steps);
        lock(_sync) {
            var index = IndexOfAnchor(reference);
            if(index < 0) {
                throw new InvalidOperationException($"No equivalency step matches '{FormatAnchor(reference)}'.");
            }

            _steps.InsertRange(index + 1, validated);
        }

        return this;
    }

    /// <summary>Removes steps anchored at <paramref name="reference"/>.</summary>
    /// <remarks>A step instance removes only itself; a <see cref="Type"/> reference removes every step of exactly that type.</remarks>
    /// <typeparam name="TReference">The anchor type.</typeparam>
    /// <param name="reference">The anchor: a step instance or a <see cref="Type"/>.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> is <see langword="null"/>.</exception>
    public EquivalencyPlan Remove<TReference>(TReference reference) {
        if(reference is null) {
            throw new ArgumentNullException(nameof(reference));
        }

        lock(_sync) {
            if(reference is Type type) {
                _steps.RemoveAll(step => step.GetType() == type);
            } else {
                _steps.RemoveAll(step => ReferenceEquals(step, reference));
            }
        }

        return this;
    }

    /// <summary>Restores the default plan (no custom steps; the built-in comparison is implicit).</summary>
    /// <returns>This plan for chaining.</returns>
    public EquivalencyPlan Reset() {
        lock(_sync) {
            _steps.Clear();
        }

        return this;
    }

    /// <summary>Removes every step from the plan.</summary>
    /// <remarks>Currently identical to <see cref="Reset"/> because the default plan is empty; kept as a separate verb for intent.</remarks>
    /// <returns>This plan for chaining.</returns>
    public EquivalencyPlan Clear() => Reset();

    /// <summary>Copies another plan's steps into this plan.</summary>
    /// <param name="other">The plan to copy from.</param>
    /// <returns>This plan for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    internal EquivalencyPlan CopyFrom(EquivalencyPlan other) {
        if(other is null) {
            throw new ArgumentNullException(nameof(other));
        }

        lock(_sync) {
            _steps.AddRange(other.Snapshot());
        }

        return this;
    }

    /// <summary>Finds the position of a plan step, matching instances by reference and types by exact type.</summary>
    /// <typeparam name="TReference">The anchor kind: an <see cref="IEquivalencyStep"/> instance or a step <see cref="Type"/>.</typeparam>
    /// <param name="reference">The step instance or type to locate.</param>
    /// <returns>The zero-based index, or <c>-1</c> when the anchor is not in the plan.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> is <see langword="null"/>.</exception>
    private int IndexOfAnchor<TReference>(TReference reference) {
        if(reference is null) {
            throw new ArgumentNullException(nameof(reference));
        }

        if(reference is not Type) {
            for(var index = 0; index < _steps.Count; index++) {
                if(ReferenceEquals(_steps[index], reference)) {
                    return index;
                }
            }
        }

        for(var index = 0; index < _steps.Count; index++) {
            if(IsAnchor(_steps[index], reference)) {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Determines whether a step matches an anchor by exact type (types) or by instance and type (steps).</summary>
    /// <typeparam name="TReference">The anchor kind.</typeparam>
    /// <param name="step">The candidate step.</param>
    /// <param name="reference">The anchor.</param>
    /// <returns><see langword="true"/> when the step matches the anchor.</returns>
    private static bool IsAnchor<TReference>(IEquivalencyStep step, TReference reference) {
        if(reference is Type type) {
            return step.GetType() == type;
        }

        return ReferenceEquals(step, reference) || step.GetType() == reference!.GetType();
    }

    /// <summary>Formats an anchor for failure messages.</summary>
    /// <typeparam name="TReference">The anchor kind: a step instance or a step type.</typeparam>
    /// <param name="reference">The anchor to format.</param>
    /// <returns>The full type name (falling back to the simple name) of the anchor.</returns>
    private static string FormatAnchor<TReference>(TReference reference) =>
        reference is Type type ? type.FullName ?? type.Name : reference!.GetType().FullName ?? reference.GetType().Name;

    /// <summary>Copies and validates a step list, rejecting empty arrays and null entries.</summary>
    /// <param name="steps">The steps to validate.</param>
    /// <returns>A defensive copy of the steps.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="steps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="steps"/> is empty or contains <see langword="null"/>.</exception>
    private static List<IEquivalencyStep> Validate(IEquivalencyStep[] steps) {
        if(steps is null) {
            throw new ArgumentNullException(nameof(steps));
        }

        if(steps.Length == 0) {
            throw new ArgumentException("At least one step must be specified.", nameof(steps));
        }

        if(steps.Any(static step => step is null)) {
            throw new ArgumentException("Steps cannot contain null.", nameof(steps));
        }

        return [.. steps];
    }
}
