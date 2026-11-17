# Jeninnet.Assertions.NUnit

Optional [NUnit](https://nunit.org/) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as `NUnit.Framework.AssertionException` so NUnit reports them as test failures rather than
errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.NUnit
```

Current release: NuGet package `1.2.0`; assembly version `1.2.0.0`; tag `v1.2.0.0`.

The adapter targets `net6.0`, `net8.0`, `net10.0`, and `net11.0`, and pulls the core `Jeninnet.Assertions`
package automatically. The `net6.0` target compiles against NUnit 4.3.2 because NUnit 5 dropped
`net6.0`; `net8.0`, `net10.0`, and `net11.0` compile against the centrally pinned NUnit 5 line. Both
versions expose the same `AssertionException` type. The repository builds the adapter with the pinned
.NET 11 SDK (`11.0.100`) and C# 15; consumers can use any adapter target.

See the [English book](https://tareknajem04.github.io/Jeninnet.Assertions/), the
[API reference](https://tareknajem04.github.io/Jeninnet.Assertions/api/toc.html), or
[الدليل العربي](https://tareknajem04.github.io/Jeninnet.Assertions/ar/00-index.html).

## Usage

```csharp
using Jeninnet.Assertions;
using Jeninnet.Assertions.NUnit;

NUnitAssertions.That(() => order.Should().Be("shipped"));
await NUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

- `That` returns normally when the wrapped assertions pass.
- A failed `AssertionFailedException` surfaces as `AssertionException` with the message and inner
  exception preserved.
- Every other exception propagates untouched.

**Why explicit?** The core package stays framework-agnostic; nothing is detected or rewritten.
Wrapping makes the integration visible at the call site and keeps NUnit out of projects that do not
use it.

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).
