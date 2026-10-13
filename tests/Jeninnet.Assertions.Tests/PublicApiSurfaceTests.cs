//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

/// <summary>
/// Public API surface verification for the standalone Jeninnet.Assertions package.
/// The committed baseline under <c>ApiBaselines</c> is compared, per packable assembly,
/// against the built assembly's exported public surface. Any accidental public API
/// addition, removal, or signature change fails the test with an actionable diff.
/// The mechanism is deterministic, offline, and CI-safe (runs inside the existing
/// <c>dotnet test</c> verification model) and uses no third-party tooling.
/// </summary>
[TestClass]
public class PublicApiSurfaceTests {
    /// <summary>The packable library assemblies. Identity: project == assembly == namespace == package.</summary>
    private static readonly string[] _packableAssemblies = [
        "Jeninnet.Assertions",
        "Jeninnet.Assertions.MSTest",
    ];

    [TestMethod]
    public void PublicApiSurface_MatchesCommittedBaseline() {
        var failures = new List<string>();

        foreach(var assemblyName in _packableAssemblies) {
            var baselinePath = Path.Combine(BaselineDirectory, assemblyName + ".Api.txt");
            if(!File.Exists(baselinePath)) {
                failures.Add($"{assemblyName}: missing committed baseline file (a packable project must participate in API validation)");
                continue;
            }

            var expected = File.ReadAllLines(baselinePath)
                               .Where(static line => !line.StartsWith("//", StringComparison.Ordinal))
                               .ToArray();
            var actual = DumpAssembly(assemblyName);

            var differences = DiffLines(expected, actual);
            if(differences.Count > 0) {
                failures.Add($"{assemblyName}: {differences.Count} public API difference(s)" + Environment.NewLine + string.Join(Environment.NewLine, differences.Take(40)));
            }
        }

        failures.Should().BeEmpty(string.Join(Environment.NewLine + Environment.NewLine, failures));
    }

    [TestMethod]
    public void PublicApiBaseline_RegenerateWhenRequested() {
        if(Environment.GetEnvironmentVariable("JENINNET_API_REGEN") != "1") {
            Assert.Inconclusive("set JENINNET_API_REGEN=1 to regenerate the API baseline file");
            return;
        }

        foreach(var assemblyName in _packableAssemblies) {
            var lines = new[] { $"// Public API baseline for {assemblyName} (net10.0)." }.Concat(DumpAssembly(assemblyName));
            Directory.CreateDirectory(BaselineDirectory);
            File.WriteAllLines(Path.Combine(BaselineDirectory, assemblyName + ".Api.txt"), lines);
        }
    }

    private static string BaselineDirectory {
        get {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while(directory is not null && !File.Exists(Path.Combine(directory.FullName, "Directory.Packages.props"))) {
                directory = directory.Parent;
            }

            if(directory is null) {
                throw new InvalidOperationException("repository root could not be located from the test output directory");
            }

            return Path.Combine(directory.FullName, "tests", "Jeninnet.Assertions.Tests", "ApiBaselines");
        }
    }

    private static string[] DumpAssembly(string assemblyName) {
        var assembly = Assembly.Load(new AssemblyName(assemblyName));
        var lines = new List<string>();

        foreach(var type in assembly.GetExportedTypes()
                     .OrderBy(static type => type.FullName, StringComparer.Ordinal)) {
            lines.AddRange(DumpType(type));
        }

        return [.. lines];
    }

    private static IEnumerable<string> DumpType(Type type) {
        var header = "type " + FormatType(type, includeGenericParameters: true);
        var members = DumpConstructors(type)
            .Concat(DumpMethods(type))
            .Concat(DumpProperties(type))
            .Concat(DumpEvents(type))
            .Concat(DumpFields(type))
            .OrderBy(static line => line, StringComparer.Ordinal);

        return [header, .. members];
    }

    private static IEnumerable<string> DumpConstructors(Type type) {
        foreach(var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
            yield return "  ctor(" + FormatParameters(constructor.GetParameters()) + ")";
        }
    }

    private static IEnumerable<string> DumpMethods(Type type) {
        foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Where(static method => !method.IsSpecialName)) {
            var modifiers = string.Join(' ', MethodModifiers(method));
            var name = FormatTypeName(method.Name, method.GetGenericArguments());
            yield return $"  method {modifiers}{(modifiers.Length > 0 ? " " : "")}{FormatType(method.ReturnType)} {name}({FormatParameters(method.GetParameters())})";
        }
    }

    private static IEnumerable<string> DumpProperties(Type type) {
        foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
            var accessors = string.Join("; ", new[] {
                property.GetMethod is { IsPublic: true } ? "get" : null,
                property.SetMethod is { IsPublic: true } ? "set" : null,
            }.Where(static accessor => accessor is not null));
            var modifiers = property.GetMethod is { IsStatic: true } || property.SetMethod is { IsStatic: true } ? "static " : string.Empty;
            var indexer = property.GetIndexParameters() is { Length: > 0 } indexParameters
                ? "[" + FormatParameters(indexParameters) + "]"
                : string.Empty;
            yield return $"  property {modifiers}{FormatType(property.PropertyType)} {property.Name}{indexer} {{ {accessors}; }}";
        }
    }

    private static IEnumerable<string> DumpEvents(Type type) {
        foreach(var @event in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
            yield return $"  event {FormatType(@event.EventHandlerType!)} {@event.Name}";
        }
    }

    private static IEnumerable<string> DumpFields(Type type) {
        foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
            var modifiers = field.IsStatic ? "static " : string.Empty;
            if(field.IsLiteral) {
                modifiers = "const ";
            } else if(field.IsInitOnly) {
                modifiers += "readonly ";
            }

            yield return $"  field {modifiers}{FormatType(field.FieldType)} {field.Name}";
        }
    }

    private static IEnumerable<string> MethodModifiers(MethodInfo method) {
        if(method.IsStatic) {
            yield return "static";
        }

        if(method.IsAbstract) {
            yield return "abstract";
        } else if(method.IsVirtual && !method.IsFinal) {
            yield return "virtual";
        } else if(method.IsVirtual && method.IsFinal) {
            yield return "override";
        }
    }

    private static string FormatParameters(IReadOnlyList<ParameterInfo> parameters) =>
        string.Join(", ", parameters.Select(static parameter => FormatType(parameter.ParameterType)));

    private static string FormatTypeName(string name, Type[] genericArguments) {
        if(genericArguments.Length == 0) {
            return name;
        }

        return name + "<" + string.Join(",", genericArguments.Select(static argument => argument.Name)) + ">";
    }

    private static string FormatType(Type type, bool includeGenericParameters = false) {
        if(type.IsGenericParameter) {
            return type.Name;
        }

        if(type.IsArray) {
            return FormatType(type.GetElementType()!) + "[]";
        }

        if(type.IsByRef) {
            return FormatType(type.GetElementType()!) + "&";
        }

        if(type.IsGenericType) {
            var fullName = type.GetGenericTypeDefinition().FullName ?? type.GetGenericTypeDefinition().Name;
            var backtick = fullName.IndexOf('`');
            var baseName = backtick >= 0 ? fullName[..backtick] : fullName;
            var arguments = includeGenericParameters
                ? type.GetGenericArguments().Select(static argument => argument.Name)
                : type.GetGenericArguments().Select(static argument => FormatType(argument));
            return baseName + "<" + string.Join(",", arguments) + ">";
        }

        return type.FullName ?? type.Name;
    }

    private static List<string> DiffLines(string[] expected, string[] actual) {
        var expectedCounts = CountByLine(expected);
        var actualCounts = CountByLine(actual);
        var added = new List<string>();
        var removed = new List<string>();

        foreach(var (line, count) in actualCounts) {
            var expectedCount = expectedCounts.TryGetValue(line, out var existing) ? existing : 0;
            for(var index = 0; index < Math.Max(0, count - expectedCount); index++) {
                added.Add(line);
            }
        }

        foreach(var (line, count) in expectedCounts) {
            var actualCount = actualCounts.TryGetValue(line, out var existing) ? existing : 0;
            for(var index = 0; index < Math.Max(0, count - actualCount); index++) {
                removed.Add(line);
            }
        }

        var differences = added.Select(static line => "+ " + line)
            .Concat(removed.Select(static line => "- " + line))
            .OrderBy(static line => line, StringComparer.Ordinal)
            .ToList();
        if(differences.Count > 0) {
            differences.Insert(0, "(+ = in built assembly, not in baseline; - = in baseline, missing from built assembly)");
        }

        return differences;
    }

    private static Dictionary<string, int> CountByLine(IEnumerable<string> lines) {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach(var line in lines) {
            counts.TryGetValue(line, out var count);
            counts[line] = count + 1;
        }

        return counts;
    }
}
