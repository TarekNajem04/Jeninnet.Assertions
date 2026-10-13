//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>The current node of the object graph handed to a custom <see cref="IEquivalencyStep"/>.</summary>
public interface IObjectGraph {
    /// <summary>Gets the actual value at this node.</summary>
    object? Subject { get; }

    /// <summary>Gets the expected value at this node.</summary>
    object? Expectation { get; }

    /// <summary>Gets the member path segments from the root (empty at the root).</summary>
    IReadOnlyList<string> Path { get; }

    /// <summary>Compares a nested pair with the ambient options, recording any differences.</summary>
    /// <param name="subject">The actual nested value.</param>
    /// <param name="expectation">The expected nested value.</param>
    /// <param name="path">The member path segments from the root.</param>
    void Compare(object? subject, object? expectation, IReadOnlyList<string> path);
}

/// <summary>Services available while a custom <see cref="IEquivalencyStep"/> handles a node.</summary>
public interface IEquivalencyStepContext {
    /// <summary>Writes a diagnostics line to the configured <see cref="ITraceWriter"/>, when tracing is on.</summary>
    /// <param name="message">The message to trace.</param>
    void Trace(string message);

    /// <summary>Records a failure detail for this node (without the trailing subject prefix).</summary>
    /// <param name="detail">The failure detail.</param>
    void Fail(string detail);
}

/// <summary>Services available while a custom <see cref="IEquivalencyStep"/> validates a node.</summary>
public interface IEquivalencyValidationContext {
    /// <summary>Writes a diagnostics line to the configured <see cref="ITraceWriter"/>, when tracing is on.</summary>
    /// <param name="message">The message to trace.</param>
    void Trace(string message);

    /// <summary>Records a failure detail for this node (without the trailing subject prefix).</summary>
    /// <param name="detail">The failure detail.</param>
    void Fail(string detail);
}

/// <summary>A custom equivalency rule consulted before the built-in comparison at every graph node.</summary>
/// <remarks>
/// The engine walks <see cref="EquivalencyPlan"/> in order for each node: the first step whose
/// <see cref="Handles"/> returns <see langword="true"/> owns the node. <see cref="TryHandle"/> performs the
/// comparison; returning <see langword="true"/> means handled, returning <see langword="false"/> falls through
/// to <see cref="AssertEquivalency"/> for validation-style steps. Derive from <see cref="EquivalencyStep"/>
/// to override only the lifecycle you need.
/// </remarks>
public interface IEquivalencyStep {
    /// <summary>Determines whether this step owns <paramref name="comparands"/>.</summary>
    /// <param name="comparands">The pair under comparison.</param>
    /// <returns><see langword="true"/> when this step handles the pair.</returns>
    bool Handles(Comparands comparands);

    /// <summary>Compares an owned pair, recording failures through <paramref name="context"/>.</summary>
    /// <param name="comparands">The pair under comparison.</param>
    /// <param name="objectGraph">The current node; use <see cref="IObjectGraph.Compare"/> to recurse.</param>
    /// <param name="context">Tracing and failure services.</param>
    /// <returns><see langword="true"/> when handled; <see langword="false"/> to fall through to <see cref="AssertEquivalency"/>.</returns>
    bool TryHandle(Comparands comparands, IObjectGraph objectGraph, IEquivalencyStepContext context);

    /// <summary>Validates an owned pair a second time, recording failures through <paramref name="context"/>.</summary>
    /// <param name="comparands">The pair under comparison.</param>
    /// <param name="context">Tracing and failure services.</param>
    void AssertEquivalency(Comparands comparands, IEquivalencyValidationContext context);
}

/// <summary>Base step with no-op lifecycles; override only the phase you need.</summary>
public abstract class EquivalencyStep : IEquivalencyStep {
    /// <inheritdoc/>
    public virtual bool Handles(Comparands comparands) => false;

    /// <inheritdoc/>
    public virtual bool TryHandle(Comparands comparands, IObjectGraph objectGraph, IEquivalencyStepContext context) => false;

    /// <inheritdoc/>
    public virtual void AssertEquivalency(Comparands comparands, IEquivalencyValidationContext context) {
    }
}
