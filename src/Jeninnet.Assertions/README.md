# Jeninnet.Assertions

A fluent, framework-agnostic assertion library for .NET tests, providing expressive validation for objects, strings, collections, exceptions, async operations, and IO test infrastructure.

## Design Philosophy

- **Framework-agnostic** — No dependency on MSTest, xUnit, or NUnit. Works with any test framework.
- **Fluent API** — Intuitive `Should().*` chain starting from any value.
- **Discoverable** — Extension methods on core types (`bool`, `string`, `IEnumerable<T>`, `Action`, `Func<Task>`).
- **Descriptive** — Clear failure messages without custom assertion formatters.
- **Extensible** — Add new assertion types by implementing the same pattern.
- **Self-contained** — No external assertion library dependencies.

## Installation

```shell
dotnet add package Jeninnet.Assertions
```

## Supported Frameworks

- .NET 10.0+
- Works with MSTest, xUnit, NUnit, and any other .NET test framework

## Usage

```csharp
using Jeninnet.Assertions;
```

### Boolean Assertions

```csharp
true.Should().BeTrue();
false.Should().BeFalse();
```

### String Assertions

```csharp
"hello".Should().Be("hello");
"hello world".Should().Contain("world");
"readme.md".Should().EndsWith(".md");
string? nullStr = null;
nullStr.Should().BeNull();
"text".Should().NotBeNull();
```

### Collection Assertions

```csharp
var items = new[] { 1, 2, 3 };

items.Should().HaveCount(3);
items.Should().BeEmpty();
items.Should().NotBeEmpty();
items.Should().Contain(2);
items.Should().NotContain(4);
items.Should().Contain(x => x > 2);
items.Should().NotContain(x => x > 5);
items.Should().BeEquivalentTo(new[] { 3, 1, 2 });
items.Should().ContainSubset(new[] { 1, 3 });

var single = items.Should().ContainSingle();
single.Which.Should().Be(1);

var matching = items.Should().ContainSingle(x => x > 2);
matching.Which.Should().Be(3);
```

### Exception Assertions

```csharp
// Synchronous
Action act = () => { throw new InvalidOperationException("test"); };
act.Should().Throw<InvalidOperationException>()
   .Exception.Should().NotBeNull();

// With message check
act.Should().Throw<InvalidOperationException>("Expected InvalidOperationException");

// Completing without throwing
Action ok = () => { };
ok.Should().NotThrow();
ok.Should().NotThrow("Optional failure message");

// Async
Func<Task> asyncAct = async () => { await Task.Delay(1); throw new TimeoutException(); };
await asyncAct.Should().ThrowAsync<TimeoutException>();

// Async completing without throwing
Func<Task> asyncOk = () => Task.CompletedTask;
await asyncOk.Should().NotThrowAsync();
```

### Object Assertions

```csharp
object? obj = "hello";
obj.Should().Be("hello");
obj.Should().NotBeNull();
obj.Should().BeOfType<string>();

object? nullObj = null;
nullObj.Should().BeNull();
```

### Test Environment (IO)

```csharp
using Jeninnet.Assertions.IO;

using var env = new TestEnvironment();

// Create files and directories
env.CreateDirectory("subdir");
env.CreateFile("subdir/file.txt", "content");
env.CreateFiles("a.txt", "b.txt", "c.txt");

// Resolve absolute paths
var path = env.Abs("subdir");
var fullPath = env.Abs("subdir", "file.txt");

// Simulate inaccessible directories
var pseudo = InaccessibleDirectorySimulator.CreatePseudoInaccessibleDir(env, "restricted");
var locked = InaccessibleDirectorySimulator.CreateLockedDir(env, "locked", out var lockStream);
```

## Project Structure

```
Jeninnet.Assertions/
├── ActionAssertions.cs            # Synchronous exception assertions
├── AsyncActionAssertions.cs       # Async exception assertions
├── BoolAssertions.cs              # Boolean value assertions
├── CollectionAssertions.cs        # Collection assertions
├── ObjectAssertions.cs            # Generic object assertions
├── StringAssertions.cs            # String value assertions
├── AssertionExtensions.cs         # Should() entry points
├── AssertionFailedException.cs    # Framework-agnostic assertion exception
├── ExceptionAssertions.cs         # Exception result wrapper
├── WhichConstraint.cs             # Container for single-item assertion results
├── Extensions/
│   ├── PathExtensions.cs          # Path comparison extensions
│   └── TestEnvironmentExtensions.cs  # TestEnvironment extensions
├── IO/
│   ├── InaccessibleDirectorySimulator.cs  # Inaccessible directory simulation
│   └── TestEnvironment.cs         # Isolated temp directory management
├── Utilities/
│   └── PathHelper.cs              # Path join/normalize/compare helpers
├── GlobalUsings.cs
└── README.md
```

## Extending

Add a new assertion type:

1. Create an assertion class (e.g., `NumericAssertions`).
2. Add a `Should()` extension method in `AssertionExtensions.cs`.
3. Optionally add XML documentation.

## Building and Testing

```shell
dotnet build tests/Jeninnet.Assertions.Tests/
dotnet test tests/Jeninnet.Assertions.Tests/
```

## License

MIT