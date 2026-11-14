//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeSelectorAssertions {
    private const int MAX_OFFENDERS = 10;
    private static readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Asserts that every selected type is visible outside its assembly.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is not public.</exception>
    public AndConstraint<TypeSelectorAssertions> BePublic(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => !type.IsVisible).Select(Describe).ToList();
        Report(StringKeys.ARCH_BE_PUBLIC, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no selected type is visible outside its assembly.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is public.</exception>
    public AndConstraint<TypeSelectorAssertions> BeInternal(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => type.IsVisible).Select(Describe).ToList();
        Report(StringKeys.ARCH_BE_INTERNAL, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every selected type is nested inside another type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is not nested.</exception>
    public AndConstraint<TypeSelectorAssertions> BeNested(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => !type.IsNested).Select(Describe).ToList();
        Report(StringKeys.ARCH_BE_NESTED, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no selected type is nested inside another type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is nested.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeNested(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => type.IsNested).Select(Describe).ToList();
        Report(StringKeys.ARCH_NOT_BE_NESTED, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every selected type is a record.</summary>
    /// <remarks>Detection uses compiler-generated record markers; hand-written look-alikes can be misclassified.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is not a record.</exception>
    public AndConstraint<TypeSelectorAssertions> BeRecord(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => !ReflectionMetadata.IsRecord(type)).Select(Describe).ToList();
        Report(StringKeys.ARCH_BE_RECORD, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no selected type is a record.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is a record.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeRecord(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(ReflectionMetadata.IsRecord).Select(Describe).ToList();
        Report(StringKeys.ARCH_NOT_BE_RECORD, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every selected type is structurally immutable from its public surface.</summary>
    /// <remarks>Heuristic: public instance fields must be readonly and public setters must be init-only or absent.
    /// Private mutable state is ignored; this is a structural check, not a proof.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type exposes mutable public state.</exception>
    public AndConstraint<TypeSelectorAssertions> BeImmutable(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(static type => !ReflectionMetadata.IsImmutable(type)).Select(Describe).ToList();
        Report(StringKeys.ARCH_BE_IMMUTABLE, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no selected type is structurally immutable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type is immutable.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeImmutable(string? because = null, params object?[] becauseArgs) {
        var offenders = _types.Where(ReflectionMetadata.IsImmutable).Select(Describe).ToList();
        Report(StringKeys.ARCH_NOT_BE_IMMUTABLE, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that every selected type's simple name starts with <paramref name="prefix"/> (ordinal).</summary>
    /// <param name="prefix">The required prefix.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type does not start with the prefix.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveNameStartingWith(string prefix, string? because = null, params object?[] becauseArgs) {
        if(prefix is null) {
            throw new ArgumentNullException(nameof(prefix));
        }

        var offenders = _types.Where(type => !type.Name.StartsWith(prefix, StringComparison.Ordinal)).Select(Describe).ToList();
        Report(StringKeys.ARCH_HAVE_NAME_STARTING_WITH, offenders, because, becauseArgs, prefix);
        return new(this);
    }

    /// <summary>Asserts that every selected type's simple name ends with <paramref name="suffix"/> (ordinal).</summary>
    /// <param name="suffix">The required suffix.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="suffix"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type does not end with the suffix.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveNameEndingWith(string suffix, string? because = null, params object?[] becauseArgs) {
        if(suffix is null) {
            throw new ArgumentNullException(nameof(suffix));
        }

        var offenders = _types.Where(type => !type.Name.EndsWith(suffix, StringComparison.Ordinal)).Select(Describe).ToList();
        Report(StringKeys.ARCH_HAVE_NAME_ENDING_WITH, offenders, because, becauseArgs, suffix);
        return new(this);
    }

    /// <summary>Asserts that every selected type's simple name matches <paramref name="pattern"/>.</summary>
    /// <remarks>The pattern runs with a one-second timeout and culture-invariant semantics.</remarks>
    /// <param name="pattern">The regular expression matched against simple type names.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is not a valid regular expression.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type does not match the pattern.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveNameMatching(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        var regex = new Regex(pattern, RegexOptions.CultureInvariant, _regexTimeout);
        var offenders = _types.Where(type => !regex.IsMatch(type.Name)).Select(Describe).ToList();
        Report(StringKeys.ARCH_HAVE_NAME_MATCHING, offenders, because, becauseArgs, pattern);
        return new(this);
    }

    /// <summary>Asserts that every selected type is declared in <paramref name="namespaceName"/> (exact match).</summary>
    /// <param name="namespaceName">The expected namespace (exact).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceName"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type is in another namespace.</exception>
    public AndConstraint<TypeSelectorAssertions> ResideInNamespace(string namespaceName, string? because = null, params object?[] becauseArgs) =>
        BeInNamespace(namespaceName, because, becauseArgs);

    /// <summary>Asserts that no selected type depends on <paramref name="namespaceOrType"/>.</summary>
    /// <remarks>The dependency graph is reflection-level: base types, interfaces, generic arguments, members, and
    /// attribute references. Method bodies and IL are not inspected.</remarks>
    /// <param name="namespaceOrType">A namespace prefix, full type name, or simple type name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceOrType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceOrType"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a matching dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> NotDependOn(string namespaceOrType, string? because = null, params object?[] becauseArgs) {
        var target = ValidateTarget(namespaceOrType, nameof(namespaceOrType));
        var graph = TypeDependencyGraph.Build(_types);
        var offenders = new List<string>();
        foreach(var type in _types) {
            var dependency = graph.DependenciesOf(type).FirstOrDefault(match => DependencyMatcher.Matches(match, target));
            if(dependency is not null) {
                offenders.Add(Describe(type) + " -> " + DependencyMatcher.Describe(dependency));
            }
        }

        Report(StringKeys.ARCH_NOT_DEPEND_ON, offenders, because, becauseArgs, target);
        return new(this);
    }

    /// <summary>Asserts that no selected type depends on <paramref name="target"/>.</summary>
    /// <param name="target">The forbidden target type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a matching dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> NotDependOn(Type target, string? because = null, params object?[] becauseArgs) {
        if(target is null) {
            throw new ArgumentNullException(nameof(target));
        }

        return NotDependOn(DependencyMatcher.TargetName(target), because, becauseArgs);
    }

    /// <summary>Asserts that every dependency of every selected type matches one of <paramref name="allowedTargets"/>.</summary>
    /// <remarks>There is no implicit allowance for BCL types: include <c>System</c> (or narrower namespaces) explicitly.
    /// The dependency graph is reflection-level; method bodies and IL are not inspected.</remarks>
    /// <param name="allowedTargets">Namespace prefixes, full type names, or simple type names that dependencies may match.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="allowedTargets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="allowedTargets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a disallowed dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> OnlyDependOn(params string[] allowedTargets) => OnlyDependOn(allowedTargets, null, []);

    /// <summary>Asserts that every dependency of every selected type matches one of <paramref name="allowedTargets"/>.</summary>
    /// <param name="allowedTargets">Namespace prefixes, full type names, or simple type names that dependencies may match.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="allowedTargets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="allowedTargets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a disallowed dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> OnlyDependOn(string[] allowedTargets, string? because, params object?[] becauseArgs) {
        if(allowedTargets is null) {
            throw new ArgumentNullException(nameof(allowedTargets));
        }

        if(allowedTargets.Length == 0) {
            throw new ArgumentException("At least one allowed dependency target must be provided.", nameof(allowedTargets));
        }

        var targets = allowedTargets.Select(target => ValidateTarget(target, nameof(allowedTargets))).ToArray();
        var graph = TypeDependencyGraph.Build(_types);
        var offenders = new List<string>();
        foreach(var type in _types) {
            var dependency = graph.DependenciesOf(type).FirstOrDefault(match => !targets.Any(target => DependencyMatcher.Matches(match, target)));
            if(dependency is not null) {
                offenders.Add(Describe(type) + " -> " + DependencyMatcher.Describe(dependency));
            }
        }

        Report(StringKeys.ARCH_ONLY_DEPEND_ON, offenders, because, becauseArgs, string.Join(", ", targets));
        return new(this);
    }

    /// <summary>Asserts that no selected type depends on any of <paramref name="targets"/>.</summary>
    /// <param name="targets">Namespace prefixes, full type names, or simple type names that must not be dependencies.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="targets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a matching dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> NotDependOnAny(params string[] targets) => NotDependOnAny(targets, null, []);

    /// <summary>Asserts that no selected type depends on any of <paramref name="targets"/>.</summary>
    /// <param name="targets">Namespace prefixes, full type names, or simple type names that must not be dependencies.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targets"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="targets"/> is empty or contains a blank entry.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type has a matching dependency.</exception>
    public AndConstraint<TypeSelectorAssertions> NotDependOnAny(string[] targets, string? because, params object?[] becauseArgs) {
        if(targets is null) {
            throw new ArgumentNullException(nameof(targets));
        }

        if(targets.Length == 0) {
            throw new ArgumentException("At least one forbidden dependency target must be provided.", nameof(targets));
        }

        var normalized = targets.Select(target => ValidateTarget(target, nameof(targets))).ToArray();
        var graph = TypeDependencyGraph.Build(_types);
        var offenders = new List<string>();
        foreach(var type in _types) {
            var dependency = graph.DependenciesOf(type).FirstOrDefault(match => normalized.Any(target => DependencyMatcher.Matches(match, target)));
            if(dependency is not null) {
                offenders.Add(Describe(type) + " -> " + DependencyMatcher.Describe(dependency));
            }
        }

        Report(StringKeys.ARCH_NOT_DEPEND_ON_ANY, offenders, because, becauseArgs, string.Join(", ", normalized));
        return new(this);
    }

    /// <summary>Asserts that the selected types have no dependency cycles among themselves.</summary>
    /// <remarks>Cycles are computed from the same reflection-level graph as <see cref="NotDependOn(string, string?, object?[])"/>;
    /// dependencies outside the selection are ignored. Up to ten cycles are listed with the total count.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one dependency cycle exists.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveNoCycles(string? because = null, params object?[] becauseArgs) {
        var cycles = TypeDependencyGraph.Build(_types).FindCycles();
        if(cycles.Count == 0) {
            return new(this);
        }

        var described = cycles.Take(MAX_OFFENDERS)
            .Select(static cycle => string.Join(" -> ", cycle.Select(DependencyMatcher.Describe)))
            .ToList();
        if(cycles.Count > MAX_OFFENDERS) {
            described.Add(Strings.Format(StringKeys.ARCH_MORE_OFFENDERS, cycles.Count - MAX_OFFENDERS));
        }

        AssertionScope.Fail(AssertionScope.AppendBecause(
            Strings.Format(StringKeys.ARCH_HAVE_NO_CYCLES, cycles.Count, string.Join("; ", described)),
            because,
            becauseArgs));
        return new(this);
    }

    /// <summary>Asserts that every selected type exposes only public or private members.</summary>
    /// <remarks>Private members are implementation details and are ignored. Fields, properties, events, and methods
    /// with internal or protected accessibility are reported; constructors are not part of this rule.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">At least one selected type exposes a non-public, non-private member.</exception>
    public AndConstraint<TypeSelectorAssertions> OnlyHavePublicMembers(string? because = null, params object?[] becauseArgs) {
        var offenders = new List<string>();
        foreach(var type in _types) {
            var member = FindNonPublicSurfaceMember(type);
            if(member is not null) {
                offenders.Add($"{Describe(type)} -> {member.Value.Name} ({member.Value.Access})");
            }
        }

        Report(StringKeys.ARCH_ONLY_PUBLIC_MEMBERS, offenders, because, becauseArgs);
        return new(this);
    }

    /// <summary>Asserts that no selected type exposes any of <paramref name="exposedTypes"/> in its public API.</summary>
    /// <remarks>Public fields, properties, events, methods, and constructors are inspected, including array element
    /// types, generic arguments, and generic constraints. Exact type identity is used; derived types are not implied.</remarks>
    /// <param name="exposedTypes">The types that must not appear in a public signature.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exposedTypes"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="exposedTypes"/> is empty.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type exposes one of the forbidden types.</exception>
    public AndConstraint<TypeSelectorAssertions> NotExposeTypes(params Type[] exposedTypes) => NotExposeTypes(exposedTypes, null, []);

    /// <summary>Asserts that no selected type exposes any of <paramref name="exposedTypes"/> in its public API.</summary>
    /// <param name="exposedTypes">The types that must not appear in a public signature.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exposedTypes"/> is <see langword="null"/> or contains <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="exposedTypes"/> is empty.</exception>
    /// <exception cref="AssertionFailedException">At least one selected type exposes one of the forbidden types.</exception>
    public AndConstraint<TypeSelectorAssertions> NotExposeTypes(Type[] exposedTypes, string? because, params object?[] becauseArgs) {
        if(exposedTypes is null) {
            throw new ArgumentNullException(nameof(exposedTypes));
        }

        if(exposedTypes.Length == 0) {
            throw new ArgumentException("At least one exposed type must be provided.", nameof(exposedTypes));
        }

        if(exposedTypes.Any(static type => type is null)) {
            throw new ArgumentNullException(nameof(exposedTypes));
        }

        var offenders = new List<string>();
        foreach(var type in _types) {
            var exposure = FindExposure(type, exposedTypes);
            if(exposure is not null) {
                offenders.Add($"{Describe(type)} -> {exposure.Value.Member} ({DependencyMatcher.Describe(exposure.Value.Exposed)})");
            }
        }

        Report(StringKeys.ARCH_NOT_EXPOSE_TYPES, offenders, because, becauseArgs, string.Join(", ", exposedTypes.Select(DependencyMatcher.Describe)));
        return new(this);
    }

    /// <summary>Reports the first offenders for a selector rule.</summary>
    /// <param name="key">The localization key for the rule message.</param>
    /// <param name="offenders">The offender descriptions.</param>
    /// <param name="because">The optional reason.</param>
    /// <param name="becauseArgs">Optional format arguments for the reason.</param>
    /// <param name="leadingArgs">Arguments that precede the offender count and list in the message template.</param>
    private static void Report(string key, List<string> offenders, string? because, object?[] becauseArgs, params object?[] leadingArgs) {
        if(offenders.Count == 0) {
            return;
        }

        var arguments = new object?[leadingArgs.Length + 2];
        leadingArgs.CopyTo(arguments, 0);
        arguments[leadingArgs.Length] = offenders.Count;
        arguments[leadingArgs.Length + 1] = FormatOffenders(offenders);
        AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(key, arguments), because, becauseArgs));
    }

    /// <summary>Formats up to ten offenders and appends the hidden count.</summary>
    /// <param name="offenders">The offender descriptions.</param>
    /// <returns>The semicolon-separated offender list.</returns>
    private static string FormatOffenders(List<string> offenders) {
        var visible = string.Join("; ", offenders.Take(MAX_OFFENDERS));
        return offenders.Count > MAX_OFFENDERS
            ? visible + "; " + Strings.Format(StringKeys.ARCH_MORE_OFFENDERS, offenders.Count - MAX_OFFENDERS)
            : visible;
    }

    /// <summary>Describes a type for offender lists.</summary>
    /// <param name="type">The type to describe.</param>
    /// <returns>The normalized full name.</returns>
    private static string Describe(Type type) => DependencyMatcher.Describe(type);

    /// <summary>Validates a dependency target string.</summary>
    /// <param name="target">The target to validate.</param>
    /// <param name="parameterName">The caller parameter name.</param>
    /// <returns>The trimmed target.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="target"/> is empty or whitespace.</exception>
    private static string ValidateTarget(string target, string parameterName) {
        if(target is null) {
            throw new ArgumentNullException(parameterName);
        }

        var trimmed = target.Trim();
        if(trimmed.Length == 0) {
            throw new ArgumentException("The dependency target must not be empty.", parameterName);
        }

        return trimmed;
    }

    /// <summary>Finds the first non-public, non-private member on a type's declared surface.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The member name and accessibility, or <see langword="null"/> when the surface is clean.</returns>
    private static (string Name, AccessModifier Access)? FindNonPublicSurfaceMember(Type type) {
        const BindingFlags declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach(var field in type.GetFields(declared)) {
            if(field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
                continue;
            }

            var access = GetAccessModifier(field);
            if(IsSurfaceViolation(access)) {
                return (field.Name, access);
            }
        }

        foreach(var property in type.GetProperties(declared)) {
            var access = MostVisible(property.GetMethod, property.SetMethod);
            if(IsSurfaceViolation(access)) {
                return (property.Name, access);
            }
        }

        foreach(var @event in type.GetEvents(declared)) {
            var access = MostVisible(@event.AddMethod, @event.RemoveMethod);
            if(IsSurfaceViolation(access)) {
                return (@event.Name, access);
            }
        }

        foreach(var method in type.GetMethods(declared)) {
            if(method.IsSpecialName || method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
                continue;
            }

            var access = ReflectionMetadata.GetAccessModifier(method);
            if(IsSurfaceViolation(access)) {
                return (method.Name, access);
            }
        }

        return null;
    }

    /// <summary>Finds the first public member exposing one of the forbidden types.</summary>
    /// <param name="type">The type whose public surface is inspected.</param>
    /// <param name="exposedTypes">The forbidden types.</param>
    /// <returns>The member name and type, or <see langword="null"/> when nothing is exposed.</returns>
    private static (string Member, Type Exposed)? FindExposure(Type type, IReadOnlyList<Type> exposedTypes) {
        const BindingFlags declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach(var field in type.GetFields(declared)) {
            var match = exposedTypes.FirstOrDefault(target => References(field.FieldType, target));
            if(match is not null) {
                return (field.Name, match);
            }
        }

        foreach(var property in type.GetProperties(declared)) {
            if(property.GetMethod is not { IsPublic: true } && property.SetMethod is not { IsPublic: true }) {
                continue;
            }

            var match = exposedTypes.FirstOrDefault(target => References(property.PropertyType, target)
                || property.GetIndexParameters().Any(parameter => References(parameter.ParameterType, target)));
            if(match is not null) {
                return (property.Name, match);
            }
        }

        foreach(var @event in type.GetEvents(declared)) {
            if(@event.AddMethod is not { IsPublic: true } || @event.EventHandlerType is not { } handlerType) {
                continue;
            }

            var match = exposedTypes.FirstOrDefault(target => References(handlerType, target));
            if(match is not null) {
                return (@event.Name, match);
            }
        }

        foreach(var method in type.GetMethods(declared)) {
            if(!method.IsPublic || method.IsSpecialName || method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)) {
                continue;
            }

            var match = exposedTypes.FirstOrDefault(target => References(method.ReturnType, target)
                || method.GetParameters().Any(parameter => References(parameter.ParameterType, target)));
            if(match is not null) {
                return ($"{method.Name}({ReflectionMetadata.FormatParameters(method.GetParameters())})", match);
            }
        }

        foreach(var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            var match = exposedTypes.FirstOrDefault(target => constructor.GetParameters().Any(parameter => References(parameter.ParameterType, target)));
            if(match is not null) {
                return ($"constructor({ReflectionMetadata.FormatParameters(constructor.GetParameters())})", match);
            }
        }

        return null;
    }

    /// <summary>Determines whether a type signature references <paramref name="target"/>.</summary>
    /// <param name="candidate">The signature type to inspect.</param>
    /// <param name="target">The forbidden type.</param>
    /// <returns><see langword="true"/> for an exact match through arrays, generics, or generic constraints.</returns>
    private static bool References(Type candidate, Type target) {
        if(candidate == target) {
            return true;
        }

        if(candidate.HasElementType) {
            return References(candidate.GetElementType()!, target);
        }

        if(candidate.IsGenericParameter) {
            return candidate.GetGenericParameterConstraints().Any(constraint => References(constraint, target));
        }

        return candidate.IsGenericType
            && (References(candidate.GetGenericTypeDefinition(), target)
                || candidate.GetGenericArguments().Any(argument => References(argument, target)));
    }

    /// <summary>Gets the declared accessibility of a field.</summary>
    /// <param name="field">The field to inspect.</param>
    /// <returns>The declared accessibility.</returns>
    private static AccessModifier GetAccessModifier(FieldInfo field) {
        if(field.IsPublic) {
            return AccessModifier.Public;
        }

        if(field.IsPrivate) {
            return AccessModifier.Private;
        }

        if(field.IsFamilyOrAssembly) {
            return AccessModifier.ProtectedInternal;
        }

        if(field.IsFamilyAndAssembly) {
            return AccessModifier.PrivateProtected;
        }

        return field.IsFamily ? AccessModifier.Protected : AccessModifier.Internal;
    }

    /// <summary>Selects the most visible accessibility from two accessors.</summary>
    /// <param name="first">The first accessor (may be <see langword="null"/>).</param>
    /// <param name="second">The second accessor (may be <see langword="null"/>).</param>
    /// <returns>The most visible accessibility, or <see cref="AccessModifier.Private"/> when both are absent.</returns>
    private static AccessModifier MostVisible(MethodBase? first, MethodBase? second) {
        var left = first is null ? AccessModifier.Private : ReflectionMetadata.GetAccessModifier(first);
        var right = second is null ? AccessModifier.Private : ReflectionMetadata.GetAccessModifier(second);
        return VisibilityRank(left) <= VisibilityRank(right) ? left : right;
    }

    /// <summary>Ranks access modifiers from most to least visible.</summary>
    /// <param name="access">The accessibility.</param>
    /// <returns>A rank where lower is more visible.</returns>
    private static int VisibilityRank(AccessModifier access) =>
        access switch {
            AccessModifier.Public => 0,
            AccessModifier.ProtectedInternal => 1,
            AccessModifier.Internal => 2,
            AccessModifier.Protected => 3,
            AccessModifier.PrivateProtected => 4,
            _ => 5,
        };

    /// <summary>Determines whether an accessibility violates the public-members rule.</summary>
    /// <param name="access">The accessibility.</param>
    /// <returns><see langword="true"/> for internal or protected accessibilities.</returns>
    private static bool IsSurfaceViolation(AccessModifier access) =>
        access is not AccessModifier.Public and not AccessModifier.Private;
}
