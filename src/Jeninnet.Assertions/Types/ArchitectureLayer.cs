//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Describes a named namespace layer and the other layers it may depend on.</summary>
/// <remarks>
/// Layer membership is prefix-based: a type belongs to a layer when its namespace equals
/// <see cref="NamespacePrefix"/> or starts with <c>NamespacePrefix + "."</c>. When prefixes overlap,
/// the longest matching prefix wins. A layer with no <see cref="DependsOn"/> calls may depend only on itself.
/// </remarks>
public sealed class ArchitectureLayer {
    /// <summary>Gets the reserved layer name used for framework dependencies when <c>includingFrameworkDependencies</c> is enabled.</summary>
    public static string FrameworkLayerName => "Framework";

    private readonly HashSet<string> _dependencies = new(StringComparer.Ordinal);

    /// <summary>Creates a layer with a name and a namespace prefix.</summary>
    /// <param name="name">The unique layer name used by <see cref="DependsOn"/> and in failure messages.</param>
    /// <param name="namespacePrefix">The namespace prefix that assigns types to this layer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="namespacePrefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> or <paramref name="namespacePrefix"/> is empty or whitespace.</exception>
    public ArchitectureLayer(string name, string namespacePrefix) {
        Name = Validate(name, nameof(name), "The layer name must not be empty.");
        NamespacePrefix = Validate(namespacePrefix, nameof(namespacePrefix), "The namespace prefix must not be empty.");
    }

    /// <summary>Gets the unique layer name.</summary>
    public string Name { get; }

    /// <summary>Gets the namespace prefix that assigns types to this layer.</summary>
    public string NamespacePrefix { get; }

    /// <summary>Gets the layer names this layer may depend on.</summary>
    internal IReadOnlyCollection<string> Dependencies => _dependencies;

    /// <summary>Declares that this layer may depend on the named layers.</summary>
    /// <param name="layerNames">The names of layers this layer may depend on.</param>
    /// <returns>This layer, for fluent composition.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="layerNames"/> or one of its entries is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="layerNames"/> is empty or contains a blank name.</exception>
    public ArchitectureLayer DependsOn(params string[] layerNames) {
        if(layerNames is null) {
            throw new ArgumentNullException(nameof(layerNames));
        }

        if(layerNames.Length == 0) {
            throw new ArgumentException("At least one layer name must be provided.", nameof(layerNames));
        }

        foreach(var layerName in layerNames) {
            _dependencies.Add(Validate(layerName, nameof(layerNames), "The layer name must not be empty."));
        }

        return this;
    }

    /// <summary>Determines whether this layer declares a dependency on <paramref name="layerName"/>.</summary>
    /// <param name="layerName">The target layer name.</param>
    /// <returns><see langword="true"/> when the dependency is allowed.</returns>
    internal bool Allows(string layerName) => _dependencies.Contains(layerName);

    /// <summary>Validates and trims a layer name or prefix.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The caller parameter name.</param>
    /// <param name="message">The exception message for a blank value.</param>
    /// <returns>The trimmed value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is empty or whitespace.</exception>
    private static string Validate(string value, string parameterName, string message) {
        if(value is null) {
            throw new ArgumentNullException(parameterName);
        }

        var trimmed = value.Trim();
        if(trimmed.Length == 0) {
            throw new ArgumentException(message, parameterName);
        }

        return trimmed;
    }
}
