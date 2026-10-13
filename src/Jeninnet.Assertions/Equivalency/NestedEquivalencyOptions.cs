//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>
/// Collects member-selection rules for a nested object graph reached through <see cref="SelfReferenceEquivalencyOptions{TExpectation}.For{TNext}"/>.
/// </summary>
/// <remarks>
/// Every member returned by this builder ends the chain by handing the options instance back, so a
/// configuration reads as a single fluent expression. <see cref="ExcludeAll"/> and <see cref="Exclude{TMember}"/>
/// are the terminal operations; <see cref="For{TNext2}"/> narrows the chain to a nested collection first.
/// </remarks>
/// <typeparam name="TExpectation">The compile-time type of the expectation.</typeparam>
/// <typeparam name="TNext">The element type selected by the preceding <c>For</c> selector.</typeparam>
public sealed class NestedEquivalencyOptions<TExpectation, TNext> {
    private readonly SelfReferenceEquivalencyOptions<TExpectation> _options;
    private readonly string[] _path;

    /// <summary>Creates a builder rooted at <paramref name="path"/> of <paramref name="options"/>.</summary>
    /// <param name="options">The options instance collecting the rules.</param>
    /// <param name="path">The already-selected path prefix.</param>
    internal NestedEquivalencyOptions(SelfReferenceEquivalencyOptions<TExpectation> options, string[] path) {
        _options = options;
        _path = path;
    }

    /// <summary>Excludes a member of the selected collection's elements (and its whole subtree).</summary>
    /// <typeparam name="TMember">The member type selected by <paramref name="selector"/>.</typeparam>
    /// <param name="selector">A member access on the element type.</param>
    /// <returns>The options instance for further chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the element.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> Exclude<TMember>(Expression<Func<TNext, TMember>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        _options.ExcludePath(CombinePath(MemberPath.Parse(selector)));
        return _options;
    }

    /// <summary>Excludes the selected path itself, including every element and every member below it.</summary>
    /// <returns>The options instance for further chaining.</returns>
    public SelfReferenceEquivalencyOptions<TExpectation> ExcludeAll() {
        _options.ExcludePath(_path);
        return _options;
    }

    /// <summary>Narrows the chain to a collection nested inside the selected elements.</summary>
    /// <typeparam name="TNext2">The element type of the nested collection.</typeparam>
    /// <param name="selector">A collection member access on the element type.</param>
    /// <returns>A builder rooted at the nested collection.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the element.</exception>
    public NestedEquivalencyOptions<TExpectation, TNext2> For<TNext2>(Expression<Func<TNext, IEnumerable<TNext2>>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var path = CombinePath(MemberPath.Parse(selector));
        return new NestedEquivalencyOptions<TExpectation, TNext2>(_options, path);
    }

    /// <summary>Prepends the builder's current path to a nested selector's relative path.</summary>
    /// <param name="relative">The relative path segments from the nested selector.</param>
    /// <returns>The combined absolute path segments.</returns>
    private string[] CombinePath(string[] relative) {
        var combined = new string[_path.Length + relative.Length];
        _path.CopyTo(combined, 0);
        relative.CopyTo(combined, _path.Length);
        return combined;
    }
}
