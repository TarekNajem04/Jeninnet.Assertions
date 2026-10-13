//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

public partial class SelfReferenceEquivalencyOptions<TExpectation> {
    /// <summary>
    /// Re-types these element options as whole-collection options, so paths authored against the element stay
    /// element-relative while the comparison runs over the collection.
    /// </summary>
    /// <remarks>
    /// Member paths already match through collection indices (an <c>Orders</c> exclusion applies to every
    /// <c>[i].Orders</c>), so the re-typed instance shares this element's rules as-is. Pair with a
    /// <c>BeEquivalentTo</c> overload whose configuration returns <c>SelfReferenceEquivalencyOptions{IEnumerable{TExpectation}}</c>,
    /// e.g. <c>items.Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Inner).AsCollection())</c>.
    /// </remarks>
    /// <returns>Element options re-typed as collection options, sharing every rule and scalar.</returns>
    public SelfReferenceEquivalencyOptions<IEnumerable<TExpectation>> AsCollection() {
        var target = new SelfReferenceEquivalencyOptions<IEnumerable<TExpectation>>();
        CopyStateTo(target);
        return target;
    }

    /// <summary>Copies every rule, scalar and diagnostics setting into <paramref name="target"/>.</summary>
    /// <typeparam name="T">The re-typed options parameter; private access is per type declaration, so any arity works.</typeparam>
    /// <param name="target">The options instance to populate.</param>
    private void CopyStateTo<T>(SelfReferenceEquivalencyOptions<T> target) {
        target._exclusions.AddRange(_exclusions);
        target._inclusions.AddRange(_inclusions);
        target._byValueTypes.UnionWith(_byValueTypes);
        target._excludedMemberNames.UnionWith(_excludedMemberNames);
        target._excludedMemberTypes.AddRange(_excludedMemberTypes);
        target._excludePredicates.AddRange(_excludePredicates);
        target._includePredicates.AddRange(_includePredicates);
        target._strictForPredicates.AddRange(_strictForPredicates);
        target._strictExceptPredicates.AddRange(_strictExceptPredicates);
        target._conversionForPredicates.AddRange(_conversionForPredicates);
        target._conversionExceptPredicates.AddRange(_conversionExceptPredicates);
        target._byMembersTypes.UnionWith(_byMembersTypes);
        target._orderingRules.AddRange(_orderingRules);
        target._dateTimeToleranceRules.AddRange(_dateTimeToleranceRules);
        target._numericToleranceRules.AddRange(_numericToleranceRules);
        target._mappings.AddRange(_mappings);
        target._selectionRules.AddRange(_selectionRules);
        target._matchingRules.AddRange(_matchingRules);
        target._orderingRuleObjects.AddRange(_orderingRuleObjects);
        target.StrictOrdering = StrictOrdering;
        target.DateTimeTolerance = DateTimeTolerance;
        target.NumericTolerance = NumericTolerance;
        target.StrictTyping = StrictTyping;
        target.AutoConversion = AutoConversion;
        target.MemberTypePreference = MemberTypePreference;
        target.EnumComparison = EnumComparison;
        target.RecordsByValue = RecordsByValue;
        target.NullStringsAsEmpty = NullStringsAsEmpty;
        target.NullCollectionsAsEmpty = NullCollectionsAsEmpty;
        target.AllowInfiniteRecursion = AllowInfiniteRecursion;
        target.IgnoreCyclicReferences = IgnoreCyclicReferences;
        target.ShallowComparison = ShallowComparison;
        target.IgnoreJsonPropertyCasing = IgnoreJsonPropertyCasing;
        target.GraphIgnoreCase = GraphIgnoreCase;
        target.GraphTrimStart = GraphTrimStart;
        target.GraphTrimEnd = GraphTrimEnd;
        target.GraphNormalizeNewlines = GraphNormalizeNewlines;
        target.IncludeFullStringsInDifference = IncludeFullStringsInDifference;
        target.PropertyVisibility = PropertyVisibility;
        target.FieldVisibility = FieldVisibility;
        target.PreferDeclaredMembers = PreferDeclaredMembers;
        target.ExcludeExplicitlyImplemented = ExcludeExplicitlyImplemented;
        target.ThrowOnMissingMembers = ThrowOnMissingMembers;
        target.ExcludeNonBrowsableOnExpectation = ExcludeNonBrowsableOnExpectation;
        target.IgnoreNonBrowsableOnSubject = IgnoreNonBrowsableOnSubject;
        target.FullDump = FullDump;
        target.TraceWriter = TraceWriter;
        target.Plan.CopyFrom(Plan);
    }

    /// <summary>Gets whether failing comparisons append a full dump of both graphs.</summary>
    internal bool FullDump { get; private set; }

    /// <summary>Gets the diagnostics sink, or <see langword="null"/> when tracing is off.</summary>
    internal ITraceWriter? TraceWriter { get; private set; }

    /// <summary>Describes the active non-default rules for the failure-message configuration block.</summary>
    /// <returns>The one-line configuration summary (invariant diagnostics prose).</returns>
    internal string DescribeConfiguration() {
        var parts = new List<string> {
            $"strict-ordering: {(StrictOrdering ? "on" : "off")}",
        };
        if(StrictTyping) {
            parts.Add("strict-typing: on");
        }

        if(AutoConversion) {
            parts.Add("auto-conversion: on");
        }

        if(MemberTypePreference != MemberTypePreference.Default) {
            parts.Add($"member-types: {MemberTypePreference.ToString().ToLowerInvariant()}");
        }

        if(EnumComparison != EnumComparison.Default) {
            parts.Add($"enums: {EnumComparison.ToString().ToLowerInvariant()}");
        }

        if(RecordsByValue) {
            parts.Add("records: by-value");
        }

        if(NullStringsAsEmpty) {
            parts.Add("null-strings-as-empty");
        }

        if(NullCollectionsAsEmpty) {
            parts.Add("null-collections-as-empty");
        }

        if(AllowInfiniteRecursion) {
            parts.Add("allow-infinite-recursion");
        }

        if(IgnoreCyclicReferences) {
            parts.Add("ignore-cyclic-references");
        }

        if(ShallowComparison) {
            parts.Add("shallow");
        }

        if(IgnoreJsonPropertyCasing) {
            parts.Add("ignore-json-casing");
        }

        if(GraphIgnoreCase) {
            parts.Add("ignore-case");
        }

        if(GraphTrimStart) {
            parts.Add("trim-start");
        }

        if(GraphTrimEnd) {
            parts.Add("trim-end");
        }

        if(GraphNormalizeNewlines) {
            parts.Add("normalize-newlines");
        }

        if(IncludeFullStringsInDifference) {
            parts.Add("include-full-strings-in-difference");
        }

        var stepCount = Plan.Snapshot().Count;
        if(stepCount > 0) {
            parts.Add($"custom-steps: {AssertionScope.FormatInvariant(stepCount)}");
        }

        if(_selectionRules.Count > 0) {
            parts.Add($"selection-rules: {AssertionScope.FormatInvariant(_selectionRules.Count)}");
        }

        if(_matchingRules.Count > 0) {
            parts.Add($"matching-rules: {AssertionScope.FormatInvariant(_matchingRules.Count)}");
        }

        if(_orderingRuleObjects.Count > 0) {
            parts.Add($"ordering-rules: {AssertionScope.FormatInvariant(_orderingRuleObjects.Count)}");
        }

        if(_orderingRules.Count > 0) {
            parts.Add($"ordering-paths: {AssertionScope.FormatInvariant(_orderingRules.Count)}");
        }

        if(DateTimeTolerance is not null) {
            parts.Add($"date-time-tolerance: {AssertionScope.FormatInvariant(DateTimeTolerance.Value)}");
        }

        if(_dateTimeToleranceRules.Count > 0) {
            parts.Add($"date-time-tolerance-paths: {AssertionScope.FormatInvariant(_dateTimeToleranceRules.Count)}");
        }

        if(NumericTolerance is not null) {
            parts.Add($"numeric-tolerance: {AssertionScope.FormatInvariant(NumericTolerance.Value)}");
        }

        if(_numericToleranceRules.Count > 0) {
            parts.Add($"numeric-tolerance-paths: {AssertionScope.FormatInvariant(_numericToleranceRules.Count)}");
        }

        if(_mappings.Count > 0) {
            parts.Add($"mappings: {AssertionScope.FormatInvariant(_mappings.Count)}");
        }

        if(FullDump) {
            parts.Add("full-dump");
        }

        if(TraceWriter is not null) {
            parts.Add("tracing");
        }

        return "With configuration: " + string.Join("; ", parts) + ".";
    }

    /// <summary>Adds an exclusion rule for an already-resolved path (used by the nested builder).</summary>
    /// <param name="path">The root-relative path segments to exclude.</param>
    internal void ExcludePath(string[] path) => _exclusions.Add(path);

    /// <summary>Determines whether <paramref name="path"/> is excluded (longest matching rule wins; exclusions win ties).</summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns><see langword="true"/> when the path is excluded; otherwise <see langword="false"/>.</returns>
    private bool IsExcluded(IReadOnlyList<string> path) {
        if(path.Count == 0) {
            return false;
        }

        var excluded = LongestMatch(_exclusions, path);
        var included = LongestMatch(_inclusions, path);
        if(excluded < 0 && included < 0) {
            return false;
        }

        return excluded >= included;
    }

    /// <summary>
    /// Determines whether a subtree can be skipped entirely: the path is excluded and no inclusion
    /// re-includes the path itself or anything beneath it (so longer <c>Including</c> paths are still visited).
    /// </summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns><see langword="true"/> when nothing under <paramref name="path"/> can contribute a failure.</returns>
    internal bool CanSkipSubtree(IReadOnlyList<string> path) {
        if(!IsExcluded(path)) {
            return false;
        }

        return !_inclusions.Any(inclusion => MemberPath.IsUnder(inclusion, path));
    }

    /// <summary>Determines whether <paramref name="type"/> must be compared by value.</summary>
    /// <param name="type">The runtime type to check.</param>
    /// <returns><see langword="true"/> when registered via <see cref="ComparingByValue{T}"/>.</returns>
    internal bool IsByValue(Type type) => _byValueTypes.Contains(type);

    /// <summary>Determines whether the member edge requires exact runtime-type matches (strict beats conversion here).</summary>
    /// <param name="member">The expectation member owning the edge.</param>
    /// <returns><see langword="true"/> when global strict typing applies and is not exempted, or the member is scoped strict.</returns>
    internal bool IsStrictEdge(MemberInfo member) =>
        (StrictTyping && !_strictExceptPredicates.Any(predicate => predicate(member)))
        || _strictForPredicates.Any(predicate => predicate(member));

    /// <summary>Determines whether the member edge may convert between differing runtime types.</summary>
    /// <param name="member">The expectation member owning the edge.</param>
    /// <returns><see langword="true"/> when global conversion applies and is not exempted, or the member is scoped for conversion.</returns>
    internal bool IsConversionEdge(MemberInfo member) =>
        (AutoConversion && !_conversionExceptPredicates.Any(predicate => predicate(member)))
        || _conversionForPredicates.Any(predicate => predicate(member));

    /// <summary>Determines whether <paramref name="type"/> must be compared memberwise.</summary>
    /// <param name="type">The runtime type to check.</param>
    /// <returns><see langword="true"/> when registered via <see cref="ComparingByMembers{T}"/>.</returns>
    internal bool IsByMembers(Type type) => _byMembersTypes.Contains(type);

    /// <summary>Finds the effective ordering for <paramref name="path"/>: rule objects first (last registered wins), then path rules, then the global ordering.</summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns>The scoped ordering, or <see langword="null"/> when no rule matches.</returns>
    internal bool? OrderingForPath(IReadOnlyList<string> path) {
        for(var index = _orderingRuleObjects.Count - 1; index >= 0; index--) {
            var ordered = _orderingRuleObjects[index].IsOrdered(path);
            if(ordered.HasValue) {
                return ordered.Value;
            }
        }

        for(var index = _orderingRules.Count - 1; index >= 0; index--) {
            var rule = _orderingRules[index];
            if(rule.Path.Length == path.Count && MemberPath.IsPrefix(rule.Path, path)) {
                return rule.Strict;
            }
        }

        return null;
    }

    /// <summary>Finds the effective temporal tolerance for <paramref name="path"/>: a scoped rule (last registered wins) overrides the graph-wide tolerance.</summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns>The tolerance in effect, or <see langword="null"/> when instants must match exactly.</returns>
    internal TimeSpan? DateTimeToleranceForPath(IReadOnlyList<string> path) {
        for(var index = _dateTimeToleranceRules.Count - 1; index >= 0; index--) {
            var rule = _dateTimeToleranceRules[index];
            if(rule.Path.Length == path.Count && MemberPath.IsPrefix(rule.Path, path)) {
                return rule.Tolerance;
            }
        }

        return DateTimeTolerance;
    }

    /// <summary>Finds the effective numeric tolerance for <paramref name="path"/>: a scoped rule (last registered wins) overrides the graph-wide tolerance.</summary>
    /// <param name="path">The member path segments from the root.</param>
    /// <returns>The epsilon in effect, or <see langword="null"/> when numerics must match exactly.</returns>
    internal double? NumericToleranceForPath(IReadOnlyList<string> path) {
        for(var index = _numericToleranceRules.Count - 1; index >= 0; index--) {
            var rule = _numericToleranceRules[index];
            if(rule.Path.Length == path.Count && MemberPath.IsPrefix(rule.Path, path)) {
                return rule.Tolerance;
            }
        }

        return NumericTolerance;
    }

    /// <summary>Enumerates the expectation members that participate in the comparison.</summary>
    /// <param name="type">The runtime type of the expectation.</param>
    /// <returns>The selected members, ordered by ordinal member name.</returns>
    internal List<MemberAccessor> GetComparableMembers(Type type) {
        var members = new List<MemberAccessor>();
        if(PropertyVisibility != MemberVisibility.None) {
            foreach(var property in type.GetProperties(ToBindingFlags(PropertyVisibility))) {
                if(!property.CanRead || property.GetIndexParameters().Length != 0 || IsCompilerGenerated(property)) {
                    continue;
                }

                if(ExcludeExplicitlyImplemented && MemberAccessor.IsExplicitlyImplemented(property)) {
                    continue;
                }

                var accessor = MemberAccessor.ForProperty(property);
                if(IsSelected(accessor)) {
                    members.Add(accessor);
                }
            }
        }

        if(FieldVisibility != MemberVisibility.None) {
            foreach(var field in type.GetFields(ToBindingFlags(FieldVisibility))) {
                if(IsCompilerGenerated(field)) {
                    continue;
                }

                var accessor = MemberAccessor.ForField(field);
                if(IsSelected(accessor)) {
                    members.Add(accessor);
                }
            }
        }

        members.Sort(static (left, right) => string.CompareOrdinal(left.Name, right.Name));
        return members;
    }

    /// <summary>Resolves the subject member an expectation member is read from, honoring mapping rules.</summary>
    /// <param name="path">The full path of the expectation member.</param>
    /// <param name="expectationType">The runtime type declaring the expectation member.</param>
    /// <param name="subjectType">The runtime type declaring the subject member.</param>
    /// <returns>The subject member name, or <see langword="null"/> when no mapping applies.</returns>
    internal string? ResolveSubjectMemberName(IReadOnlyList<string> path, Type expectationType, Type subjectType) {
        for(var index = _mappings.Count - 1; index >= 0; index--) {
            if(_mappings[index].Matches(path, expectationType, subjectType)) {
                return _mappings[index].SubjectMemberName;
            }
        }

        foreach(var rule in _matchingRules) {
            var resolved = rule.ResolveSubjectMember(path, expectationType, subjectType);
            if(resolved is not null) {
                return resolved;
            }
        }

        return null;
    }

    /// <summary>Finds a member on the subject, optionally hiding non-browsable members.</summary>
    /// <param name="type">The runtime type of the subject.</param>
    /// <param name="name">The member name to look up.</param>
    /// <returns>The accessor, or <see langword="null"/> when the subject has no such member.</returns>
    internal MemberAccessor? FindMember(Type type, string name) {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var property = type.GetProperty(name, flags);
        if(property is not null && property.CanRead && property.GetIndexParameters().Length == 0) {
            var accessor = MemberAccessor.ForProperty(property);
            if(!IgnoreNonBrowsableOnSubject || accessor.IsBrowsable) {
                return accessor;
            }
        }

        var field = type.GetField(name, flags);
        if(field is not null) {
            var accessor = MemberAccessor.ForField(field);
            if(!IgnoreNonBrowsableOnSubject || accessor.IsBrowsable) {
                return accessor;
            }
        }

        return null;
    }

    /// <summary>Backing fields and other compiler-generated members are implementation detail, not part of the contract.</summary>
    /// <param name="member">The member to inspect.</param>
    /// <returns><see langword="true"/> when the compiler generated the member.</returns>
    private static bool IsCompilerGenerated(MemberInfo member) => member.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);

    /// <summary>Translates a visibility mask into reflection binding flags, honoring declared-only preference.</summary>
    /// <param name="visibility">The requested member visibility.</param>
    /// <returns>The binding flags used to enumerate members.</returns>
    private BindingFlags ToBindingFlags(MemberVisibility visibility) {
        var flags = BindingFlags.Instance;
        if((visibility & MemberVisibility.Public) != 0) {
            flags |= BindingFlags.Public;
        }

        if((visibility & MemberVisibility.Internal) != 0) {
            flags |= BindingFlags.NonPublic;
        }

        if(PreferDeclaredMembers) {
            flags |= BindingFlags.DeclaredOnly;
        }

        return flags;
    }

    /// <summary>Determines whether a member passes the exclusion and inclusion rules.</summary>
    /// <param name="member">The member accessor to test.</param>
    /// <returns><see langword="true"/> when the member participates in the comparison.</returns>
    private bool IsSelected(MemberAccessor member) {
        if(ExcludeNonBrowsableOnExpectation && !member.IsBrowsable) {
            return false;
        }

        if(_excludedMemberNames.Contains(member.Name)) {
            return false;
        }

        if(_excludedMemberTypes.Any(type => ExcludesType(type, member.MemberType))) {
            return false;
        }

        if(_excludePredicates.Any(predicate => predicate(member.Member))) {
            return false;
        }

        if(_selectionRules.Count > 0 && !_selectionRules.All(rule => rule.Includes(member.Member))) {
            return false;
        }

        return _includePredicates.Count == 0 || _includePredicates.Any(predicate => predicate(member.Member));
    }

    /// <summary>Open generic definitions match closed generics, otherwise assignability decides.</summary>
    /// <param name="excluded">The excluded type.</param>
    /// <param name="memberType">The member value type.</param>
    /// <returns><see langword="true"/> when the member is excluded.</returns>
    private static bool ExcludesType(Type excluded, Type memberType) {
        if(!excluded.IsGenericTypeDefinition) {
            return excluded.IsAssignableFrom(memberType);
        }

        for(var current = memberType; current is not null; current = current.BaseType) {
            if(current.IsGenericType && current.GetGenericTypeDefinition() == excluded) {
                return true;
            }
        }

        return false;
    }

    /// <summary>Clears every exclusion and inclusion rule so all members are selected again.</summary>
    private void ClearSelectionRules() {
        _exclusions.Clear();
        _inclusions.Clear();
        _excludedMemberNames.Clear();
        _excludedMemberTypes.Clear();
        _excludePredicates.Clear();
        _includePredicates.Clear();
    }

    /// <summary>Registers a member mapping from an expectation path to a subject path, optionally type-scoped.</summary>
    /// <param name="expectationPath">The expectation path segments.</param>
    /// <param name="subjectPath">The subject path segments.</param>
    /// <param name="expectationScope">The type scope on the expectation side, or <see langword="null"/>.</param>
    /// <param name="subjectScope">The type scope on the subject side, or <see langword="null"/>.</param>
    private void AddMapping(string[] expectationPath, string[] subjectPath, Type? expectationScope, Type? subjectScope) =>
        _mappings.Add(new MemberMapping(expectationPath, subjectPath, expectationScope, subjectScope));

    /// <summary>Validates and splits a dotted mapping path into segments.</summary>
    /// <param name="path">The dotted path (e.g. <c>Order.Customer.Name</c>).</param>
    /// <param name="parameterName">The public parameter name used in exceptions.</param>
    /// <returns>The non-empty trimmed segments.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    private static string[] ParseMappingPath(string path, string parameterName) {
        if(string.IsNullOrWhiteSpace(path)) {
            throw new ArgumentException("The member path cannot be empty.", parameterName);
        }

        var nonEmpty = path.Split('.')
            .Select(static segment => segment.Trim())
            .Where(static segment => segment.Length > 0)
            .ToArray();
        if(nonEmpty.Length == 0) {
            throw new ArgumentException("The member path cannot be empty.", parameterName);
        }

        return nonEmpty;
    }

    /// <summary>Validates a direct (non-nested) member name used for name-based mapping.</summary>
    /// <param name="name">The member name.</param>
    /// <param name="parameterName">The public parameter name used in exceptions.</param>
    /// <returns>The name as a single-segment path.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty, whitespace, or contains path syntax.</exception>
    private static string[] ParseDirectName(string name, string parameterName) {
        if(string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("The member name cannot be empty.", parameterName);
        }

        if(name.IndexOfAny(['.', '[', ']']) >= 0) {
            throw new ArgumentException("Only direct member names can be mapped; nested paths are not allowed.", parameterName);
        }

        return [name];
    }

    /// <summary>Finds the length of the longest rule path that prefixes the given path.</summary>
    /// <param name="rules">The candidate rule paths.</param>
    /// <param name="path">The member path to test.</param>
    /// <returns>The matched prefix length, or <c>-1</c> when no rule applies.</returns>
    private static int LongestMatch(List<string[]> rules, IReadOnlyList<string> path) {
        var best = -1;
        foreach(var rule in rules) {
            if(rule.Length <= path.Count && rule.Length > best && MemberPath.IsPrefix(rule, path)) {
                best = rule.Length;
            }
        }

        return best;
    }
}
