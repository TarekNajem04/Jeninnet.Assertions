//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xml;

/// <summary>Configures structural XML equivalency for <see cref="XDocument"/>, <see cref="XElement"/>, and <see cref="System.Xml.XmlElement"/> assertions.</summary>
/// <remarks>Attributes are compared as an unordered map by default; element order and whitespace are significant until relaxed.</remarks>
public sealed class XmlEquivalencyOptions {
    internal bool IgnoreElementOrder { get; private set; }

    internal bool IgnoreWhitespace { get; private set; }

    internal bool IgnoreComments { get; private set; }

    /// <summary>Keeps attribute order irrelevant (the default behavior; the method exists for readable intent).</summary>
    /// <returns>The same options instance for fluent chaining.</returns>
    public XmlEquivalencyOptions IgnoringAttributeOrder() => this;

    /// <summary>Treats sibling elements with the same name as an unordered group.</summary>
    /// <returns>The same options instance for fluent chaining.</returns>
    public XmlEquivalencyOptions IgnoringElementOrder() {
        IgnoreElementOrder = true;
        return this;
    }

    /// <summary>Collapses insignificant whitespace before comparing text nodes.</summary>
    /// <returns>The same options instance for fluent chaining.</returns>
    public XmlEquivalencyOptions IgnoringWhitespace() {
        IgnoreWhitespace = true;
        return this;
    }

    /// <summary>Skips comment nodes while comparing the tree.</summary>
    /// <returns>The same options instance for fluent chaining.</returns>
    public XmlEquivalencyOptions IgnoringComments() {
        IgnoreComments = true;
        return this;
    }
}
