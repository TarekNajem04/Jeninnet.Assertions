# Jeninnet.Assertions.xUnit.v3

Optional [xUnit v3](https://xunit.net/) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as `Xunit.Sdk.XunitException` so xUnit reports them as test failures rather than errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.xUnit.v3
```

The adapter targets `net8.0` and `net10.0`, and pulls the core `Jeninnet.Assertions` package and
`xunit.v3.assert` automatically. xUnit v2 lives in the separate
`Jeninnet.Assertions.xUnit` package.

## Usage

```csharp
using Jeninnet.Assertions;
using Jeninnet.Assertions.Xunit.V3;

XunitV3Assertions.That(() => order.Should().Be("shipped"));
await XunitV3Assertions.ThatAsync(() => action.Should().NotThrowAsync());
```

- `That` returns normally when the wrapped assertions pass.
- A failed `AssertionFailedException` surfaces as `XunitException` with the message and inner
  exception preserved.
- Every other exception propagates untouched.

**Why explicit?** The core package stays framework-agnostic; nothing is detected or rewritten.
The v3 adapter uses its own namespace and class name so v2 and v3 packages can coexist during a
staged migration.

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).
