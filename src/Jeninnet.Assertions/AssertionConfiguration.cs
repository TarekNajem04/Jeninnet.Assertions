//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Process-wide assertion configuration. Set once at startup (ideally from an assembly initializer); read on every assertion.</summary>
public static class AssertionConfiguration {
    /// <summary>Gets the global value formatters consulted after scoped formatters.</summary>
    public static ValueFormatterOptions Formatting { get; } = new();

    /// <summary>Gets the global equivalency defaults.</summary>
    public static EquivalencyDefaults Equivalency { get; } = new();
}

/// <summary>Global defaults for equivalency assertions.</summary>
/// <remarks>
/// Ambient overrides set with <see cref="Modify"/> are isolated per async flow (<see cref="AsyncLocal{T}"/>),
/// exactly like <see cref="AssertionScope"/> nesting: parallel tests never see each other's overrides.
/// Per-assertion configuration always wins over the ambient template, which wins over these process-wide defaults.
/// </remarks>
public sealed class EquivalencyDefaults {
    private static readonly AsyncLocal<SelfReferenceEquivalencyOptions?> _ambient = new();

    /// <summary>Gets or sets whether collections compare order-sensitively unless configured otherwise.</summary>
    public bool StrictOrderingDefault { get; set; }

    /// <summary>Gets the process-wide custom steps consulted before the built-in comparison.</summary>
    public EquivalencyPlan Plan { get; } = new();

    /// <summary>Applies <paramref name="action"/> to the ambient equivalency template visible in this async flow.</summary>
    /// <param name="action">The mutation to apply (cumulative with earlier calls in the same flow).</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Modify(Action<SelfReferenceEquivalencyOptions> action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        var template = _ambient.Value ?? CloneDefaults();
        action(template);
        _ambient.Value = template;
    }

    /// <summary>Clones the process-wide defaults into a detached template.</summary>
    /// <returns>The detached template.</returns>
    public SelfReferenceEquivalencyOptions CloneDefaults() {
        var template = new SelfReferenceEquivalencyOptions {
            StrictOrdering = StrictOrderingDefault,
        };
        template.Plan.CopyFrom(Plan);
        return template;
    }

    /// <summary>Clears the ambient overrides set with <see cref="Modify"/> in this async flow.</summary>
    public static void ResetToDefaults() => _ambient.Value = null;

    /// <summary>Gets the effective template: the ambient override, or a clone of the process-wide defaults.</summary>
    /// <returns>The effective template.</returns>
    internal SelfReferenceEquivalencyOptions CurrentTemplate() => _ambient.Value ?? CloneDefaults();

    /// <summary>Captures a detached copy of the ambient template visible in this async flow (used by <see cref="AssertionIsolation"/>).</summary>
    /// <returns>The copy, or <see langword="null"/> when no ambient override is set.</returns>
    internal static SelfReferenceEquivalencyOptions? CaptureAmbient() => _ambient.Value?.Copy();

    /// <summary>Replaces the ambient template visible in this async flow with a previously captured copy (used by <see cref="AssertionIsolation"/>).</summary>
    /// <param name="template">The captured copy, or <see langword="null"/> to clear the ambient override.</param>
    internal static void RestoreAmbient(SelfReferenceEquivalencyOptions? template) => _ambient.Value = template;
}
