//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if !NETSTANDARD2_0
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying regex <see cref="Match"/> results, including capture groups.</summary>
/// <remarks>
/// A failed match (<c>Match.Success</c> is <see langword="false"/>) or a <see langword="null"/> match fails
/// the verbs with a dedicated message. A capture group that did not participate (an unmatched optional group)
/// counts as the empty string, requesting a group that does not exist lists the available group names, and
/// group 0 is the whole match. Requires net6.0+; <c>Group.Name</c> is unavailable on .NET Standard 2.0.
/// </remarks>
/// <param name="match">The regex match to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class MatchAssertions(Match? match, string? expression = null) : AssertionsBase(expression, "match") {
    private readonly Match? _match = match;

    /// <summary>Asserts that the match succeeded.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The match is <see langword="null"/> or did not succeed.</exception>
    public AndConstraint<MatchAssertions> Succeed(string? because = null, params object?[] becauseArgs) {
        if(_match is null) {
            Fail(Strings.Format(StringKeys.MATCH_NULL, Expression), because, becauseArgs);
        } else if(!_match.Success) {
            Fail(Strings.Format(StringKeys.MATCH_SUCCEED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the whole match equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected match text.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The match is <see langword="null"/>, failed, or the text differs.</exception>
    public AndConstraint<MatchAssertions> HaveValue(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_match is null) {
            Fail(Strings.Format(StringKeys.MATCH_NULL, Expression), because, becauseArgs);
        } else if(!_match.Success) {
            Fail(Strings.Format(StringKeys.MATCH_HAVE_VALUE_FAILED, Expression, expected), because, becauseArgs);
        } else if(!string.Equals(_match.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.MATCH_HAVE_VALUE, Expression, expected, _match.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the named capture group holds <paramref name="expected"/>.</summary>
    /// <remarks>A group that did not participate counts as the empty string.</remarks>
    /// <param name="name">The capture group name.</param>
    /// <param name="expected">The expected group value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The match is <see langword="null"/>, failed, the group does not exist, or the value differs.</exception>
    public AndConstraint<MatchAssertions> HaveGroup(string name, string expected, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        return CheckGroup(name, index: null, expected, because, becauseArgs);
    }

    /// <summary>Asserts that the capture group at <paramref name="index"/> holds <paramref name="expected"/>.</summary>
    /// <remarks>A group that did not participate counts as the empty string; index 0 is the whole match.</remarks>
    /// <param name="index">The zero-based capture group index.</param>
    /// <param name="expected">The expected group value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The match is <see langword="null"/>, failed, the index is out of range, or the value differs.</exception>
    public AndConstraint<MatchAssertions> HaveGroup(int index, string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        return CheckGroup(AssertionScope.FormatInvariant(index), index, expected, because, becauseArgs);
    }

    /// <summary>Asserts that the match exposes exactly <paramref name="expected"/> capture groups.</summary>
    /// <remarks>The count includes group 0 (the whole match), so a pattern with one unnamed group reports two groups.</remarks>
    /// <param name="expected">The expected group count (including group 0).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The match is <see langword="null"/>, failed, or the count differs.</exception>
    public AndConstraint<MatchAssertions> HaveGroupCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_match is null) {
            Fail(Strings.Format(StringKeys.MATCH_NULL, Expression), because, becauseArgs);
        } else if(!_match.Success) {
            Fail(Strings.Format(StringKeys.MATCH_SUCCEED, Expression), because, becauseArgs);
        } else if(_match.Groups.Count != expected) {
            Fail(Strings.Format(StringKeys.MATCH_HAVE_GROUP_COUNT, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_match.Groups.Count)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Validates one named capture group against the expected value, handling null and failed matches.</summary>
    /// <param name="name">The group name (or formatted index) used in messages.</param>
    /// <param name="index">The group index when the lookup was numeric, else <see langword="null"/>.</param>
    /// <param name="expected">The expected group value.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    private AndConstraint<MatchAssertions> CheckGroup(string name, int? index, string expected, string? because, object?[]? becauseArgs) {
        if(_match is null) {
            Fail(Strings.Format(StringKeys.MATCH_NULL, Expression), because, becauseArgs);
            return new(this);
        }

        if(!_match.Success) {
            Fail(Strings.Format(StringKeys.MATCH_HAVE_GROUP_FAILED, Expression, name, expected), because, becauseArgs);
            return new(this);
        }

        if(index is not null && (index.Value < 0 || index.Value >= _match.Groups.Count)) {
            Fail(MissingGroupMessage(name), because, becauseArgs);
            return new(this);
        }

        var group = _match.Groups.Cast<Group>().FirstOrDefault(candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if(group is null) {
            Fail(MissingGroupMessage(name), because, becauseArgs);
            return new(this);
        }

        if(!string.Equals(group.Value, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.MATCH_HAVE_GROUP_VALUE, Expression, name, expected, group.Value), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Builds the failure message for a missing capture group, listing the available group names.</summary>
    /// <param name="name">The requested group name.</param>
    /// <returns>The formatted message.</returns>
    private string MissingGroupMessage(string name) =>
        Strings.Format(StringKeys.MATCH_HAVE_GROUP_MISSING, Expression, name, string.Join(", ", _match!.Groups.Cast<Group>().Select(group => group.Name)));
}
#endif
