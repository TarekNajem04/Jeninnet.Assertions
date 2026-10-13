//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>Maps an expectation member path to a differently named subject member path.</summary>
    /// <param name="expectationMemberPath">The dotted expectation path, e.g. <c>Address.City</c>.</param>
    /// <param name="subjectMemberPath">The dotted subject path the member is read from, e.g. <c>Address.Town</c>.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentException">Either path is empty.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithMapping(string expectationMemberPath, string subjectMemberPath) {
        AddMapping(ParseMappingPath(expectationMemberPath, nameof(expectationMemberPath)),
            ParseMappingPath(subjectMemberPath, nameof(subjectMemberPath)),
            null,
            null);
        return this;
    }

    /// <summary>Registers a mapping from pre-parsed path segments (internal fast path).</summary>
    /// <param name="expectationPath">The parsed expectation path segments.</param>
    /// <param name="subjectPath">The parsed subject path segments.</param>
    /// <returns>This options instance for chaining.</returns>
    private SelfReferenceEquivalencyOptions<TExpectation> WithMapping(string[] expectationPath, string[] subjectPath) {
        AddMapping(expectationPath, subjectPath, null, null);
        return this;
    }

    /// <summary>Maps an expectation member to a differently named subject member using expressions.</summary>
    /// <typeparam name="TSubject">The subject type declaring the mapped member.</typeparam>
    /// <param name="expectationMemberPath">A member access on the expectation.</param>
    /// <param name="subjectMemberPath">A member access on the subject.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">An expression is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An expression is not a member access on its parameter.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithMapping<TSubject>(
        Expression<Func<TExpectation, object>> expectationMemberPath,
        Expression<Func<TSubject, object>> subjectMemberPath) {
        if(expectationMemberPath is null) {
            throw new ArgumentNullException(nameof(expectationMemberPath));
        }

        if(subjectMemberPath is null) {
            throw new ArgumentNullException(nameof(subjectMemberPath));
        }

        return WithMapping(MemberPath.Parse(expectationMemberPath), MemberPath.Parse(subjectMemberPath));
    }

    /// <summary>Maps a direct member of <typeparamref name="TNestedExpectation"/> to a direct member of <typeparamref name="TNestedSubject"/>.</summary>
    /// <typeparam name="TNestedExpectation">The expectation type declaring the mapped member.</typeparam>
    /// <typeparam name="TNestedSubject">The subject type declaring the mapped member.</typeparam>
    /// <param name="expectationMemberName">The direct member name on the expectation (no <c>.</c> or <c>[]</c>).</param>
    /// <param name="subjectMemberName">The direct member name on the subject (no <c>.</c> or <c>[]</c>).</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentException">A name is empty or is not a direct member name.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithMapping<TNestedExpectation, TNestedSubject>(
        string expectationMemberName,
        string subjectMemberName) {
        AddMapping(ParseDirectName(expectationMemberName, nameof(expectationMemberName)),
            ParseDirectName(subjectMemberName, nameof(subjectMemberName)),
            typeof(TNestedExpectation),
            typeof(TNestedSubject));
        return this;
    }

    /// <summary>Maps a direct member of <typeparamref name="TNestedExpectation"/> to a direct member of <typeparamref name="TNestedSubject"/> using expressions.</summary>
    /// <typeparam name="TNestedExpectation">The expectation type declaring the mapped member.</typeparam>
    /// <typeparam name="TNestedSubject">The subject type declaring the mapped member.</typeparam>
    /// <param name="expectationMember">A direct member access on the expectation.</param>
    /// <param name="subjectMember">A direct member access on the subject.</param>
    /// <returns>This options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">An expression is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An expression is not a direct member access on its parameter.</exception>
    public SelfReferenceEquivalencyOptions<TExpectation> WithMapping<TNestedExpectation, TNestedSubject>(
        Expression<Func<TNestedExpectation, object>> expectationMember,
        Expression<Func<TNestedSubject, object>> subjectMember) {
        if(expectationMember is null) {
            throw new ArgumentNullException(nameof(expectationMember));
        }

        if(subjectMember is null) {
            throw new ArgumentNullException(nameof(subjectMember));
        }

        var expectationSegments = MemberPath.Parse(expectationMember);
        var subjectSegments = MemberPath.Parse(subjectMember);
        if(expectationSegments.Length != 1 || subjectSegments.Length != 1) {
            throw new ArgumentException("Only direct members can be mapped to each other.", nameof(expectationMember));
        }

        AddMapping(expectationSegments, subjectSegments, typeof(TNestedExpectation), typeof(TNestedSubject));
        return this;
    }

    /// <summary>Selects a nested collection whose elements take further exclusion rules.</summary>
    /// <typeparam name="TNext">The element type of the selected collection.</typeparam>
    /// <param name="selector">A collection member access on the expectation.</param>
    /// <returns>A builder rooted at the selected collection.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> is not a member access on the expectation.</exception>
    public NestedEquivalencyOptions<TExpectation, TNext> For<TNext>(Expression<Func<TExpectation, IEnumerable<TNext>>> selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        return new NestedEquivalencyOptions<TExpectation, TNext>(this, MemberPath.Parse(selector));
    }
}
