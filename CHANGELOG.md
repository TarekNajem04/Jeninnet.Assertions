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
