# Jeninnet.Assertions.NUnit

Optional [NUnit](https://nunit.org/) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as `NUnit.Framework.AssertionException` so NUnit reports them as test failures rather than
errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.NUnit
```

The adapter targets `net6.0`, `net8.0`, `net10.0`, and `net11.0`, and pulls the core `Jeninnet.Assertions`
package automatically. The `net6.0` target compiles against NUnit 4.3.2 because NUnit 5 dropped
`net6.0`; `net8.0`, `net10.0`, and `net11.0` compile against the current NUnit 5 line. Both versions expose the
same `AssertionException` type.

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
