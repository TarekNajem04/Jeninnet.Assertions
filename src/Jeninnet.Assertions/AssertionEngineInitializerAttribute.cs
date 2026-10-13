//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Runs a setup method once before the first assertion (e.g. to register formatters or defaults).</summary>
/// <example>
/// <code>
/// [assembly: AssertionEngineInitializer(typeof(Initializer), nameof(Initializer.Setup))]
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class AssertionEngineInitializerAttribute : Attribute {
    /// <summary>Creates the initializer reference.</summary>
    /// <param name="initializerType">The type declaring the setup method.</param>
    /// <param name="methodName">The name of the static parameterless void setup method.</param>
    /// <exception cref="ArgumentNullException"><paramref name="initializerType"/> or <paramref name="methodName"/> is <see langword="null"/>.</exception>
    public AssertionEngineInitializerAttribute(Type initializerType, string methodName) {
        if(initializerType is null) {
            throw new ArgumentNullException(nameof(initializerType));
        }

        if(methodName is null) {
            throw new ArgumentNullException(nameof(methodName));
        }

        InitializerType = initializerType;
        MethodName = methodName;
    }

    /// <summary>Gets the type declaring the setup method.</summary>
    public Type InitializerType { get; }

    /// <summary>Gets the name of the static parameterless void setup method.</summary>
    public string MethodName { get; }
}
