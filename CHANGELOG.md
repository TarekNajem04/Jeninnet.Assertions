# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- URI assertions through `Uri.Should()`: absolute/relative kind, scheme, host, port, path,
  fragment, query parameters (with `Which` and exact values), and path-boundary `BeBaseOf`.
- Custom comparable values: `Should()` now accepts any `IComparable<T>` and returns
  `ComparableTypeAssertions<T>`, enabling comparisons, ranges, rank equality, membership,
  and predicates for domain value types.
- Numeric assertions now cover `Half`, `Int128`, `UInt128`, `BigInteger`, and native-sized
  integers (`nint`/`nuint`) on target frameworks that expose them, plus `BeZero`/`NotBeZero`.
- Task assertions now cover every terminal state without waiting: `BeCompleted`, `NotBeCompleted`,
  `BeCompletedSuccessfully`, `BeFaulted` (exposing the base exception through `Which`), and
  `BeCanceled`. `Task<T>` adds `HaveResult` (exact, predicate, and `Which`) and a successful
  completion constraint that exposes the value. `BeThreadSafeAsync` is now available on async
  actions and `Func<T>` delegates, sharing one barrier-based stress runner with aggregated failures.
- HTTP response assertions now cover null/status sugar (`BeRedirect`, `BeClientError`, `BeServerError`,
  `BeNoContent`), content metadata (`HaveContentType`, `HaveContentLength`), protocol details
  (`HaveVersion`, `HaveReasonPhrase`), caching/redirect headers (`HaveETag`, `HaveRetryAfter`,
  `HaveLocation`), and `HaveCookie` for multiple `Set-Cookie` headers. Async
  `HaveStringContentAsync`, `HaveJsonContentAsync`, and `BeProblemDetailsAsync` are token-aware and
  read the body exactly once.
- File-system assertions for `FileInfo` and `DirectoryInfo`: existence, names/full paths/extensions,
  file-length bounds, empty checks, attributes, UTC timestamps with tolerance, path-boundary parent
  checks, directory contents, and top-level file/directory counts. Missing subjects fail with a clear
  existence message instead of `FileNotFoundException`/`IOException`.
- JSON assertions now accept `JsonElement` and `JsonDocument` in addition to `JsonNode`, with
  object/array kind checks, scalar values, deep equivalency, properties, paths, and deterministic
  snapshots (`MatchSnapshot` sorts object keys ordinally). Disposed documents fail with a clear message.
- JSON paths now support quoted names (`$['a.b']`), wildcards (`[*]`, `.*`), recursive descent
  (`$..name`), and negative indexes (`$[-1]`). `HaveJsonPath` remains the single-result verb;
  `HaveJsonPathValues` returns every wildcard/recursive match in document order.
- Numeric collections support `EqualApproximately` / `NotEqualApproximately` on net8.0+ via
  `INumber<T>`. Precision follows the type-aware scalar rules: decimal and 64-bit integers stay
  exact, 128-bit integers and `BigInteger` use arbitrary precision, and NaN never approximates.
- Dictionary assertions now accept `IReadOnlyDictionary<TKey,TValue>` implementations (including
  immutable and frozen dictionaries) and any `IEnumerable<KeyValuePair<TKey,TValue>>` sequence.
  Lazy pair sequences are materialized once, and `ContainSingle()` exposes the sole pair through
  `Which`. Count bounds (`HaveCountGreaterThan`, `HaveCountGreaterThanOrEqualTo`,
  `HaveCountLessThan`, `HaveCountLessThanOrEqualTo`) mirror collection assertions.
- Reflection assertions now cover type kinds, names, generic parameters, identity, and
  inheritance shortcuts (`BeClass`, `BeRecord`, `BeImmutable`, `BeSameAs`, `BeAssignableFrom`,
  `BeSubclassOf`, `BeBaseOf`, `HaveName`, `HaveFullName`, `HaveNamespace`, `HaveBaseType`,
  `HaveGenericParameter`), plus member metadata: method abstract/override/final/generic/extension,
  property static/abstract/override/required/init-only/index parameters, constructor
  private/protected/internal/static access, exact `HaveParameters`, and predicate decorators
  with identity checks on `MethodInfo`, `PropertyInfo`, and `ConstructorInfo`.

### Fixed

- `BeApproximately`/`NotBeApproximately` no longer round `decimal` or 64-bit integers through
  `double`; `BigInteger` and 128-bit integers use exact arbitrary-precision differences.
  NaN never approximates a value, and equal infinities compare as approximately equal.

### Security

- `TestEnvironment` now rejects absolute paths and relative paths that escape `Root` before any
  file-system operation; `CreateTree` and `CreateFiles` validate every path before creating entries.
