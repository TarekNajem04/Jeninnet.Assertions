# Jeninnet.Assertions.TUnit

Optional [TUnit](https://github.com/thomhurst/TUnit) integration for
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions): wraps fluent assertion
failures as `TUnit.Assertions.Exceptions.AssertionException` so TUnit reports them as test failures
rather than errors.

## Installation

```shell
dotnet add package Jeninnet.Assertions.TUnit
```

The adapter targets `net8.0`, `net10.0`, and `net11.0`, and pulls the core `Jeninnet.Assertions` package and
the TUnit assertion types automatically. It does not reference the TUnit test host or
Microsoft.Testing.Platform runtime.

## Usage

```csharp
using Jeninnet.Assertions;
using Jeninnet.Assertions.TUnit;

TUnitAssertions.That(() => order.Should().Be("shipped"));
await TUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

- `That` returns normally when the wrapped assertions pass.
- A failed `AssertionFailedException` surfaces as TUnit's `AssertionException` with the message and
  inner exception preserved.
- Every other exception propagates untouched.

**Why explicit?** The core package stays framework-agnostic; nothing is detected or rewritten.
Wrapping makes the integration visible at the call site and keeps TUnit out of projects that do not
use it.

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).
