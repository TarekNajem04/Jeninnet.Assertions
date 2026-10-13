//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

internal static partial class EquivalencyEngine {
    /// <summary>Whether strict typing and conversion apply to one compared pair.</summary>
    /// <remarks>Members below an edge reuse the ambient rules; only the edge itself carries scoped overrides.</remarks>
    /// <param name="Strict">Whether the pair must share the exact runtime type.</param>
    /// <param name="Convert">Whether the pair may convert between differing runtime types.</param>
    private readonly record struct EdgeScope(bool Strict, bool Convert);

    private sealed partial class Run<TExp> {
        private readonly SelfReferenceEquivalencyOptions<TExp> _options;
        private readonly IReadOnlyList<IEquivalencyStep> _steps;
        private readonly ITraceWriter? _trace;
        private readonly HashSet<(object Subject, object Expectation)> _ancestors = [];
        internal readonly List<string> Failures = [];
        internal readonly List<EquivalencyDifference> Entries = [];

        internal Run(SelfReferenceEquivalencyOptions<TExp> options, IReadOnlyList<IEquivalencyStep> steps, ITraceWriter? trace) {
            _options = options;
            _steps = steps;
            _trace = trace;
        }

        internal void Compare(object? subject, object? expectation, List<string> path) =>
            Compare(subject, expectation, path, new EdgeScope(_options.StrictTyping, _options.AutoConversion));

        /// <summary>Compares one node recursively: reference equality, subtree skips, custom steps, null handling,
        /// shallow comparison, strict typing, then leaves and references.</summary>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to this pair.</param>
        private void Compare(object? subject, object? expectation, List<string> path, EdgeScope scope) {
            if(ReferenceEquals(subject, expectation)) {
                return;
            }

            if(_options.CanSkipSubtree(path)) {
                return;
            }

            TraceNode(path, subject, expectation);
            if(TryCustomSteps(subject, expectation, path)) {
                return;
            }

            if(subject is null || expectation is null) {
                if(!PassesNullAsEmpty(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return;
            }

            if(_options.ShallowComparison && path.Count > 0) {
                if(!Equals(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return;
            }

            var subjectType = subject.GetType();
            var expectationType = expectation.GetType();
            if(scope.Strict && subjectType != expectationType) {
                Fail(TypeDetail(path, subjectType, expectationType), path, expectationType.FullName ?? expectationType.Name, subjectType.FullName ?? subjectType.Name);
                return;
            }

            if(CompareLeaf(subject, expectation, subjectType, expectationType, path, scope)) {
                return;
            }

            CompareReference(subject, expectation, subjectType, MemberSelectionType(path, expectationType), path, scope);
        }

        /// <summary>Runs custom steps in plan order; the first handling step owns the node.</summary>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="path">The member path from the root.</param>
        /// <returns><see langword="true"/> when a step owned the node.</returns>
        private bool TryCustomSteps(object? subject, object? expectation, List<string> path) {
            if(_steps.Count == 0) {
                return false;
            }

            var comparands = new Comparands(subject, expectation, path);
            var graph = new Graph(this, subject, expectation, path);
            var context = new StepContext(this);
            foreach(var step in _steps) {
                var name = step.GetType().FullName ?? step.GetType().Name;
                Trace($"Evaluating step {name} at {FormatPath(path)}.");
                if(!step.Handles(comparands)) {
                    continue;
                }

                Trace($"Step {name} handles {FormatPath(path)}.");
                if(step.TryHandle(comparands, graph, context)) {
                    Trace($"Step {name} handled {FormatPath(path)}.");
                    return true;
                }

                step.AssertEquivalency(comparands, context);
                Trace($"Step {name} validated {FormatPath(path)}.");
                return true;
            }

            return false;
        }

        /// <summary>Records a trace line for the node currently being compared.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        private void TraceNode(List<string> path, object? subject, object? expectation) {
            if(_trace is null) {
                return;
            }

            _trace.AddSingle($"Comparing {FormatPath(path)} ({TypeName(subject)} vs {TypeName(expectation)}).");
        }

        private void Trace(string message) => _trace?.AddSingle(message);

        private static string FormatPath(List<string> path) => path.Count == 0 ? "<root>" : MemberPath.Format(path);

        private static string TypeName(object? value) => value is null ? "<null>" : value.GetType().Name;

        /// <summary>The current node handed to custom steps.</summary>
        /// <param name="run">The owning run.</param>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="path">The member path from the root.</param>
        private sealed class Graph(Run<TExp> run, object? subject, object? expectation, List<string> path) : IObjectGraph {
            /// <inheritdoc/>
            public object? Subject { get; } = subject;

            /// <inheritdoc/>
            public object? Expectation { get; } = expectation;

            /// <inheritdoc/>
            public IReadOnlyList<string> Path { get; } = path;

            /// <inheritdoc/>
            public void Compare(object? subject, object? expectation, IReadOnlyList<string> path) {
                if(path is null) {
                    throw new ArgumentNullException(nameof(path));
                }

                run.Compare(subject, expectation, [.. path]);
            }
        }

        /// <summary>Tracing and failure services handed to custom steps.</summary>
        /// <param name="run">The owning run.</param>
        private sealed class StepContext(Run<TExp> run) : IEquivalencyStepContext, IEquivalencyValidationContext {
            /// <inheritdoc/>
            public void Trace(string message) {
                if(message is null) {
                    throw new ArgumentNullException(nameof(message));
                }

                run.Trace(message);
            }

            /// <inheritdoc/>
            public void Fail(string detail) {
                if(detail is null) {
                    throw new ArgumentNullException(nameof(detail));
                }

                run.Fail(detail);
            }
        }

        /// <summary>Selects the type contributing members: the declared expectation type at the root when preferred, else the runtime type.</summary>
        /// <param name="path">The member path from the root.</param>
        /// <param name="expectationType">The runtime type of the expectation.</param>
        /// <returns>The type whose members are compared.</returns>
        private Type MemberSelectionType(List<string> path, Type expectationType) =>
            path.Count == 0 && _options.MemberTypePreference == MemberTypePreference.Declared
                ? typeof(TExp)
                : expectationType;

        /// <summary>Compares by-value and simple types.</summary>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="subjectType">The runtime type of the subject.</param>
        /// <param name="expectationType">The runtime type of the expectation.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to this pair.</param>
        /// <returns><see langword="true"/> when the values were handled as leaves.</returns>
        private bool CompareLeaf(object subject, object expectation, Type subjectType, Type expectationType, List<string> path, EdgeScope scope) {
            if(_options.IsByValue(subjectType) || _options.IsByValue(expectationType)) {
                if(!Equals(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return true;
            }

            if(_options.RecordsByValue && (IsRecord(subjectType) || IsRecord(expectationType))) {
                if(!Equals(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return true;
            }

            if(_options.IsByMembers(subjectType) || _options.IsByMembers(expectationType)) {
                return false;
            }

            if(subjectType.IsEnum && expectationType.IsEnum && _options.EnumComparison != EnumComparison.Default) {
                if(!EnumsMatch(subject, expectation, subjectType, expectationType, _options.EnumComparison)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return true;
            }

            if(!IsSimpleType(subjectType) && !IsSimpleType(expectationType)) {
                return false;
            }

            if(subjectType == expectationType) {
                if(subject is string subjectText && expectation is string expectationText) {
                    if(!GraphStringsEqual(subjectText, expectationText)) {
                        Fail(StringValueDetail(path, subjectText, expectationText), path, FormatStringValue(expectationText), FormatStringValue(subjectText));
                    }
                } else if(TryTolerance(subject, expectation, path, out var withinTolerance, out var toleranceNote)) {
                    if(!withinTolerance) {
                        Fail(ValueDetail(path, subject, expectation) + toleranceNote, path, FormatValue(expectation), FormatValue(subject));
                    }
                } else if(!Equals(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return true;
            }

            if(scope.Convert) {
                var converted = TryConvertEquality(subject, expectation, subjectType, expectationType);
                if(converted is not null) {
                    if(!converted.Value) {
                        Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                    }

                    return true;
                }
            }

            Fail(TypeDetail(path, subjectType, expectationType), path, expectationType.FullName ?? expectationType.Name, subjectType.FullName ?? subjectType.Name);
            return true;
        }

        /// <summary>Attempts conversion in both directions, reporting equality only when a conversion succeeds.</summary>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <param name="subjectType">The runtime type of the subject.</param>
        /// <param name="expectationType">The runtime type of the expectation.</param>
        /// <returns>Whether the converted values are equal, or <see langword="null"/> when neither direction converts.</returns>
        private static bool? TryConvertEquality(object subject, object expectation, Type subjectType, Type expectationType) {
            if(TryConvert(subject, expectationType, out var towardExpectation)) {
                return Equals(towardExpectation, expectation);
            }

            if(TryConvert(expectation, subjectType, out var towardSubject)) {
                return Equals(subject, towardSubject);
            }

            return null;
        }

        /// <summary>Converts <paramref name="value"/> toward <paramref name="target"/> with invariant culture.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type.</param>
        /// <param name="result">The converted value, or <see langword="null"/> when conversion fails.</param>
        /// <returns><see langword="true"/> when conversion succeeds.</returns>
        private static bool TryConvert(object value, Type target, out object? result) {
            try {
                result = Convert.ChangeType(value, Nullable.GetUnderlyingType(target) ?? target, CultureInfo.InvariantCulture);
                return true;
            }
            catch(Exception) {
                result = null;
                return false;
            }
        }

        /// <summary>Compares complex types memberwise using the expectation's runtime-type members
        /// (duck-typed: a derived or unrelated subject is checked member by member, with
        /// missing members reported as failures and extra subject members ignored).</summary>
        /// <param name="subject">The actual object.</param>
        /// <param name="expectation">The expected object.</param>
        /// <param name="subjectType">The runtime type of the subject.</param>
        /// <param name="expectationType">The runtime type of the expectation.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to this pair.</param>
        private void CompareReference(object subject, object expectation, Type subjectType, Type expectationType, List<string> path, EdgeScope scope) {
            if(subjectType.IsValueType) {
                CompareComposite(subject, expectation, expectationType, path, scope);
                return;
            }

            var pair = (Subject: subject, Expectation: expectation);
            if(!_ancestors.Add(pair)) {
                if(!_options.AllowInfiniteRecursion && !_options.IgnoreCyclicReferences) {
                    Fail($"a cyclic reference was detected at {MemberPath.Format(path)}.", path);
                }

                return;
            }

            try {
                CompareComposite(subject, expectation, expectationType, path, scope);
            }
            finally {
                _ancestors.Remove(pair);
            }
        }

        /// <summary>Routes a composite pair to member, dictionary, or collection comparison.</summary>
        /// <param name="subject">The actual object.</param>
        /// <param name="expectation">The expected object.</param>
        /// <param name="expectationType">The type whose members are compared.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to this pair.</param>
        private void CompareComposite(object subject, object expectation, Type expectationType, List<string> path, EdgeScope scope) {
            if(_options.IsByMembers(subject.GetType()) || _options.IsByMembers(expectationType)) {
                CompareMembers(subject, expectation, expectationType, path);
                return;
            }

            if(TryGetDictionaryEntries(subject, out var subjectEntries)
                && TryGetDictionaryEntries(expectation, out var expectationEntries)) {
                CompareDictionaries(subjectEntries, expectationEntries, path, scope);
                return;
            }

            if(subject is System.Collections.IEnumerable subjectItems && expectation is System.Collections.IEnumerable expectationItems) {
                CompareCollections(subjectItems, expectationItems, path, scope);
                return;
            }

            CompareMembers(subject, expectation, expectationType, path);
        }

        /// <summary>Compares dictionary entries key by key; missing keys fail, extra subject keys are ignored.</summary>
        /// <param name="subjectEntries">The subject's key/value pairs.</param>
        /// <param name="expectationEntries">The expectation's key/value pairs.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to child pairs.</param>
        private void CompareDictionaries(
            List<(object? Key, object? Value)> subjectEntries,
            List<(object? Key, object? Value)> expectationEntries,
            List<string> path,
            EdgeScope scope) {
            foreach(var (key, expectedValue) in expectationEntries) {
                var keyPath = ChildPath(path, FormatKey(key));
                var (_, Value) = subjectEntries.FirstOrDefault(entry => KeysEqual(entry.Key, key));
                var found = subjectEntries.Any(entry => KeysEqual(entry.Key, key));
                if(!found) {
                    Fail($"{MemberPath.Format(keyPath)} was not found in the subject dictionary.", keyPath);
                    continue;
                }

                Compare(Value, expectedValue, keyPath, scope);
            }
        }

        /// <summary>Compares two sequences: counts first, then index-wise under strict ordering or as a
        /// multiset by speculative matching (silent trials, never traced).</summary>
        /// <param name="subjectItems">The actual sequence.</param>
        /// <param name="expectationItems">The expected sequence.</param>
        /// <param name="path">The member path from the root.</param>
        /// <param name="scope">Whether strict typing and conversion apply to child pairs.</param>
        private void CompareCollections(System.Collections.IEnumerable subjectItems, System.Collections.IEnumerable expectationItems, List<string> path, EdgeScope scope) {
            var subjectList = subjectItems.Cast<object?>().ToList();
            var expectationList = expectationItems.Cast<object?>().ToList();

            if(subjectList.Count != expectationList.Count) {
                Fail(CountDetail(path, subjectList.Count, expectationList.Count), path, AssertionScope.FormatInvariant(expectationList.Count), AssertionScope.FormatInvariant(subjectList.Count));
                return;
            }

            if(_options.OrderingForPath(path) ?? _options.StrictOrdering) {
                for(var index = 0; index < expectationList.Count; index++) {
                    Compare(subjectList[index], expectationList[index], ChildPath(path, $"[{AssertionScope.FormatInvariant(index)}]"), scope);
                }

                return;
            }

            var unmatched = subjectList.ToList();
            foreach(var expected in expectationList) {
                // Speculative matching must stay silent: sharing the trace writer would log failures that backtracking discards.
                var trial = new Run<TExp>(_options, _steps, null);
                trial._ancestors.UnionWith(_ancestors);
                var matched = false;
                foreach(var candidate in unmatched.ToList()) {
                    trial.Failures.Clear();
                    trial.Compare(candidate, expected, path, scope);
                    if(trial.Failures.Count == 0) {
                        unmatched.Remove(candidate);
                        matched = true;
                        break;
                    }
                }

                if(!matched) {
                    Fail(NoEquivalentDetail(path, expected), path, FormatValue(expected));
                    return;
                }
            }
        }

        /// <summary>Compares member by member over the selected members; extra subject members are ignored and
        /// missing ones fail only when <c>ThrowingOnMissingMembers</c> is set.</summary>
        /// <param name="subject">The actual object.</param>
        /// <param name="expectation">The expected object.</param>
        /// <param name="expectationType">The type whose members drive the comparison.</param>
        /// <param name="path">The member path from the root.</param>
        private void CompareMembers(object subject, object expectation, Type expectationType, List<string> path) {
            var members = _options.GetComparableMembers(expectationType);
            if(members.Count == 0) {
                if(_options.UsesDefaultVisibility && !Equals(subject, expectation)) {
                    Fail(ValueDetail(path, subject, expectation), path, FormatValue(expectation), FormatValue(subject));
                }

                return;
            }

            var subjectType = subject.GetType();
            foreach(var member in members) {
                var childPath = ChildPath(path, member.Name);
                if(_options.CanSkipSubtree(childPath)) {
                    continue;
                }

                var expectedValue = member.GetValue(expectation);
                var subjectMember = ResolveSubjectMember(subjectType, childPath, expectationType, member.Name);
                if(subjectMember is null) {
                    if(_options.ThrowOnMissingMembers) {
                        Fail($"{MemberPath.Format(childPath)} was not found on the subject.", childPath);
                    }

                    continue;
                }

                var subjectValue = subjectMember.GetValue(subject);
                var edge = new EdgeScope(_options.IsStrictEdge(member.Member), _options.IsConversionEdge(member.Member));
                Compare(subjectValue, expectedValue, childPath, edge);
            }
        }

        /// <summary>Resolves the subject-side member for an expectation member, honoring path-scoped mappings.</summary>
        /// <param name="subjectType">The runtime type of the subject.</param>
        /// <param name="path">The child path used for path-scoped mappings.</param>
        /// <param name="expectationType">The type whose member is being mapped.</param>
        /// <param name="memberName">The expectation member name.</param>
        /// <returns>The matching subject accessor, or <see langword="null"/> when the member is missing.</returns>
        private MemberAccessor? ResolveSubjectMember(Type subjectType, List<string> path, Type expectationType, string memberName) {
            var mappedName = _options.ResolveSubjectMemberName(path, expectationType, subjectType);
            return _options.FindMember(subjectType, mappedName ?? memberName);
        }

        /// <summary>Determines whether a null side is absorbed by a null-as-empty option.</summary>
        /// <param name="subject">The actual value.</param>
        /// <param name="expectation">The expected value.</param>
        /// <returns><see langword="true"/> when both sides are null-or-empty of the same kind.</returns>
        private bool PassesNullAsEmpty(object? subject, object? expectation) =>
            (_options.NullStringsAsEmpty && IsNullOrEmptyString(subject) && IsNullOrEmptyString(expectation))
            || (_options.NullCollectionsAsEmpty && IsNullOrEmptyCollection(subject) && IsNullOrEmptyCollection(expectation));

        /// <summary>Compares graph strings with the configured casing, whitespace, and newline rules.</summary>
        /// <param name="subject">The actual text.</param>
        /// <param name="expectation">The expected text.</param>
        /// <returns><see langword="true"/> when the texts match under the configured rules.</returns>
        private bool GraphStringsEqual(string subject, string expectation) {
            if(_options.GraphTrimStart) {
                subject = subject.TrimStart();
                expectation = expectation.TrimStart();
            }

            if(_options.GraphTrimEnd) {
                subject = subject.TrimEnd();
                expectation = expectation.TrimEnd();
            }

            if(_options.GraphNormalizeNewlines) {
                subject = NormalizeNewlines(subject);
                expectation = NormalizeNewlines(expectation);
            }

            return _options.GraphIgnoreCase
                ? string.Equals(subject, expectation, StringComparison.OrdinalIgnoreCase)
                : string.Equals(subject, expectation, StringComparison.Ordinal);
        }

        /// <summary>Determines whether two dictionary keys match, optionally ignoring key casing.</summary>
        /// <param name="left">The subject key.</param>
        /// <param name="right">The expectation key.</param>
        /// <returns><see langword="true"/> when the keys match.</returns>
        private bool KeysEqual(object? left, object? right) =>
            left is string leftText && right is string rightText && _options.IgnoreJsonPropertyCasing
                ? string.Equals(leftText, rightText, StringComparison.OrdinalIgnoreCase)
                : Equals(left, right);

        /// <summary>Determines whether a value is <see langword="null"/> or an empty string.</summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns><see langword="true"/> for null or <see cref="string.Empty"/>.</returns>
        private static bool IsNullOrEmptyString(object? value) => value is null || (value is string text && text.Length == 0);

        /// <summary>Determines whether a value is <see langword="null"/> or an empty sequence (strings excluded).</summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns><see langword="true"/> for null or an enumerable with no items.</returns>
        private static bool IsNullOrEmptyCollection(object? value) {
            if(value is null) {
                return true;
            }

            return value is not string && value is System.Collections.IEnumerable items && !items.Cast<object?>().Any();
        }

        /// <summary>Normalizes CRLF and CR line endings to LF for graph string comparison.</summary>
        /// <param name="value">The text to normalize.</param>
        /// <returns>The text with LF-only line endings.</returns>
        private static string NormalizeNewlines(string value) => value.Replace("\r\n", "\n").Replace('\r', '\n');

        /// <summary>Compares enum values by number or by identifier name.</summary>
        /// <param name="subject">The actual enum value.</param>
        /// <param name="expectation">The expected enum value.</param>
        /// <param name="subjectType">The runtime enum type of the subject.</param>
        /// <param name="expectationType">The runtime enum type of the expectation.</param>
        /// <param name="mode">The configured enum comparison.</param>
        /// <returns><see langword="true"/> when the values match under <paramref name="mode"/>.</returns>
        private static bool EnumsMatch(object subject, object expectation, Type subjectType, Type expectationType, EnumComparison mode) {
            if(mode == EnumComparison.ByValue) {
                return Convert.ToInt64(subject, CultureInfo.InvariantCulture) == Convert.ToInt64(expectation, CultureInfo.InvariantCulture);
            }

            var subjectName = Enum.GetName(subjectType, subject);
            var expectationName = Enum.GetName(expectationType, expectation);
            if(!string.Equals(subjectName, expectationName, StringComparison.Ordinal)) {
                return false;
            }

            return subjectName is not null
                || Convert.ToInt64(subject, CultureInfo.InvariantCulture) == Convert.ToInt64(expectation, CultureInfo.InvariantCulture);
        }

        /// <summary>Determines whether <paramref name="type"/> is a record (class or struct).</summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns><see langword="true"/> when the compiler generated a clone method for the type.</returns>
        private static bool IsRecord(Type type) =>
            type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) is not null;
    }
}
