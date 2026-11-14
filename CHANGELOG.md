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

### Fixed

- `BeApproximately`/`NotBeApproximately` no longer round `decimal` or 64-bit integers through
  `double`; `BigInteger` and 128-bit integers use exact arbitrary-precision differences.
  NaN never approximates a value, and equal infinities compare as approximately equal.

### Security

- `TestEnvironment` now rejects absolute paths and relative paths that escape `Root` before any
  file-system operation; `CreateTree` and `CreateFiles` validate every path before creating entries.
