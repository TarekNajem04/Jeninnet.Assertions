# Jeninnet.Assertions.MSTest

Optional [MSTest](https://github.com/microsoft/testfx) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as MSTest `AssertFailedException` so reporters classify them as test failures rather than
errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.MSTest
```

The adapter targets `net6.0`, `net8.0`, and `net10.0`, and pulls the core `Jeninnet.Assertions`
package automatically. Nothing is detected or rewritten: you opt in by wrapping the assertion.

## Usage

```csharp
using Jeninnet.Assertions;

MSTestAssertions.That(() => order.Should().Be("shipped"));
await MSTestAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

- `That` returns normally when the wrapped assertions pass.
- A failed `AssertionFailedException` surfaces as `AssertFailedException` with the message and
  inner exception preserved.
- Every other exception propagates untouched.

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).
