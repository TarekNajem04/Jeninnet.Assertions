# Jeninnet.Assertions.MSTest

Optional [MSTest](https://github.com/microsoft/testfx) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as MSTest `AssertFailedException` so reporters classify them as test failures rather than
errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.MSTest
```

Current release: NuGet package `1.2.0`; assembly version `1.2.0.0`; tag `v1.2.0.0`.

The adapter targets `net6.0`, `net8.0`, `net10.0`, and `net11.0`, and pulls the core `Jeninnet.Assertions`
package automatically. The repository builds it with the pinned .NET 11 SDK (`11.0.100`) and C# 15;
consumers can use any adapter target. Nothing is detected or rewritten: you opt in by wrapping the
assertion. The core package stays framework-agnostic, so wrapping keeps the integration visible at the
call site.

See the [English book](https://tareknajem04.github.io/Jeninnet.Assertions/), the
[API reference](https://tareknajem04.github.io/Jeninnet.Assertions/api/toc.html), or
[الدليل العربي](https://tareknajem04.github.io/Jeninnet.Assertions/ar/00-index.html).

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
