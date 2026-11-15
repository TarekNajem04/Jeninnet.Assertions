# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Architecture layer direction: positive `DependOn`/`DependOnAny`, reverse `OnlyBeDependedOnBy`/
  `NotBeDependedOnBy`, and `ArchitectureLayer`/`FollowLayers` with longest-prefix assignment,
  same-layer allowance, optional framework checks via `ArchitectureLayer.FrameworkLayerName`, and
  offender lists capped at ten. Localized in en/ar/es/de/qps-ploc.
- Peer performance benchmarks: `benchmarks/Jeninnet.Assertions.Benchmarks` now measures
  AwesomeAssertions 9.6.0, Shouldly 4.3.0, and NFluent 3.1.0 on identical hot verbs and scope
  scenarios, with published mean/allocation tables for success and failure and explicit notes where
  scope semantics differ. Package references stay in the benchmark project; no core dependency or
  public API changed.
- String parsing assertions: `BeParsableInto<T>` (+`Which`) and `NotBeParsableInto<T>` for types implementing
  `IParsable<T>` (net8.0+), with an `IFormatProvider` overload; the provider-less overload parses with the
  invariant culture. A `null` subject fails positive assertions and passes negative ones, and parser
  exceptions become assertion failures with the target type and a shortened reason. Localized in
  en/ar/es/de/qps-ploc.
- Event monitoring depth: `RaiseOnce` (+`Which`) and `RaiseTimes` with every occurrence constraint,
  `HaveRecordedEvents` (+`Which`) / `HaveNoRecordedEvents`, deterministic `OccurredEvent.Timestamp`
  from `AssertionClock`, and `EventMonitorOptions.IncludingEvents`/`ExcludingEvents` filters applied
  at subscription.
- Classic `XmlElement` assertions (`BeNull`/`NotBeNull`, `HaveName`, `HaveNamespace`,
  `HaveAttribute`/`HaveAttributeWithValue`/`NotHaveAttribute`, `HaveElement` (+occurrence,
  `Which`), `HaveElementWithValue`, `HaveInnerText`, structural `BeEquivalentTo`) and
  `XDocument` root attribute/value/namespace verbs. XML equivalency now reports simplified
  XPath paths (`/root/orders[2]/@id`) and supports `XmlEquivalencyOptions`
  (`IgnoringAttributeOrder`, `IgnoringElementOrder`, `IgnoringWhitespace`, `IgnoringComments`).
- `IAsyncEnumerable<T>` parity: `AllAsync` / `AnyAsync` (early stop), `ContainSingleAsync`
  (+predicate, `Which`), `OnlyHaveUniqueItemsAsync`, `ContainInConsecutiveOrderAsync`,
  `NotContainInOrderAsync`, count bounds (`>`, `>=`, `<`, `<=`), and
  `SatisfyRespectivelyAsync` / `SatisfyAsync`; every verb is cancellation-aware and
  enumerates the source exactly once.
- `char` assertions (`Be` / `NotBe`, Unicode categories, casing, ASCII, with negatives) and
  `ParameterInfo` assertions (name, exact type, optional/required, explicit default values,
  `out`/`ref`/`in`/`params`, position, attributes (+`Which`), null handling), both localized and covered by
  tests; the `ParameterInfo` entry point carries `RequiresUnreferencedCode`.
- Migration guides for Shouldly, NFluent, and AwesomeAssertions/FluentAssertions in the migration
  playbook (English + Arabic), including the exact license differences (Shouldly BSD-3-Clause,
  NFluent/AA Apache-2.0, FA 7.x Apache-2.0 vs FA 8+ Xceed Community/commercial), a table of shapes
  that are never a one-to-one rewrite, and links from the README and analyzer README.
- Performance and release hardening: a BenchmarkDotNet project covering hot success/failure verbs,
  `AssertionScope` aggregation, equivalency on small and 500-object graphs, and snapshot verification;
  expanded allocation invariants (`AssertionScope`, equivalency, `ExecutionTime`); a package-verification
  script for TFMs, XML docs, README/icon, nuspec repository commit, symbols, and dependency hygiene; a
  weekly advisory `Benchmarks` workflow; published measurements in the README and chapter 9; and
  `docs/release-checklist.md`.
- Trimming/NativeAOT readiness: public reflection paths now carry `RequiresUnreferencedCode`,
  `RequiresDynamicCode`, or `DynamicallyAccessedMembers` annotations; always-throwing failure helpers
  use `DoesNotReturn`; the reflection-free verb surface is covered by a NativeAOT/trimmed sample that
  publishes and runs successfully. AOT-safety matrix documented in chapter 9 and the README.
- Localization expansion: complete Spanish (`es`) and German (`de`) satellite resources, a committed
  `qps-ploc` pseudo-locale for detecting hardcoded messages, a `scripts/docs/check-localization.ps1`
  completeness gate (missing/extra/empty keys, with the French reference satellite explicitly
  allowlisted as partial), documented culture fallback (`es-ES` → `es` → English), and a translator
  guide in `CONTRIBUTING.md`. Invariant numeric formatting and translated `because`/scope headers are
  covered by tests.
- CI report formats for `AssertionResult` and `SoftAssertions`: `ToReport(AssertionReportFormat)` emits
  SARIF 2.1.0, JUnit XML, GitHub Actions annotations, or Markdown alongside the existing JSON.
  `AssertionFailure` now accepts optional `RuleId`, `Severity`, `File`, `Line`, and `Column` metadata
  (defaults: category/error/no location) for rule IDs and annotations; reports are deterministic,
  escape format-specific metacharacters, omit invented locations, and leave value scrubbing to callers.
  The original `ToReportJson()` shape is unchanged.
- Optional framework adapters now cover xUnit v2 (`Jeninnet.Assertions.xUnit`), xUnit v3
  (`Jeninnet.Assertions.xUnit.v3`), NUnit 4/5 (`Jeninnet.Assertions.NUnit`), and TUnit
  (`Jeninnet.Assertions.TUnit`), each preserving the `AssertionFailedException` message and inner
  exception while surfacing the runner's native assertion exception. The adapters follow the
  existing explicit `That`/`ThatAsync` pattern, keep the core framework-agnostic, and are exercised
  by dedicated tests and `FrameworkZoo` samples.
- Roslyn analyzer expansion: new `JNA1003`–`JNA1007` rules cover string assertions, collection
  emptiness/single/containment, exception assertions (`Throw`/`ThrowExactly` with exact-throw
  semantics preserved), type assertions (`BeOfType`/`BeAssignableTo`), and `task.IsCompleted`
  state checks. Conversions stay in statement position when results would change type, preserve
  `message:` as `because:`, skip `StringComparison`/dictionary/consumed-result cases, and support
  Fix All across document/project/solution.
- Architecture assertions v2: reflection-level dependency rules (`NotDependOn`, `OnlyDependOn`,
  `NotDependOnAny`), `HaveNoCycles` with concrete cycle paths, public-surface rules
  (`OnlyHavePublicMembers`, `NotExposeTypes`), namespace/name/access/record/immutability assertions,
  and offender lists capped at ten with the remaining count. Selectors now filter by assembly,
  visibility, nesting, generics, records, immutability, name patterns, assignability, and dependencies.
  The graph covers base types, interfaces, generic arguments, members, constructors, events, and
  attribute references; method bodies and IL remain out of scope for the core package.
- Formatting limits on `ValueFormatterOptions`: `MaxDepth`, `MaxLines`, `MaxEnumerables`,
  `MaxStringLength`, `UseLineBreaks`, and opt-in `RespectRuntimeTypes`. Limits truncate with an
  explicit `…` marker and hidden count, preserve line endings by default, and guard structural
  collection display against depth, item-count, and reference cycles. `Copy`/`RestoreFrom` carry
  the limits alongside formatters.
- Custom assertion SDK: `AssertionsBase.Expression` and `FormatValue` are now accessible to
  derived assertion classes, `AssertionScope.WithExpectation`/`ClearExpectation`/`Expectation`
  replace the leading subject phrase per scope (inherited by nested scopes, never mutating the
  parent), and `AssertionIsolation.Isolate()` restores formatting options plus the active scope's
  expectation.
- URI assertions through `Uri.Should()`: absolute/relative kind, scheme, host, port, path,
  fragment, query parameters (with `Which` and exact values), and path-boundary `BeBaseOf`.
- Custom comparable values: `Should()` now accepts any `IComparable<T>` and returns
  `ComparableTypeAssertions<T>`, enabling comparisons, ranges, rank equality, membership,
  and predicates for domain value types.
- Numeric assertions now cover `Half`, `Int128`, `UInt128`, `BigInteger`, and native-sized
  integers (`nint`/`nuint`) on target frameworks that expose them, plus `BeZero`/`NotBeZero`.
- Snapshot engine v2 adds opt-in naming (`UseFileName`, `UseDirectory`, `UseMethodName`, `UseTypeName`,
  `UseParameters`/`UseTextForParameters`), stable `UniqueFor*` suffixes, line-ending/whitespace tolerance,
  JSON `SortProperties`/`IndentSize`, type/member scrubbers, and per-call `AutoVerify`, `DiffTool`, and
  `DisableDiff`. `MatchSnapshot` now also covers `JsonNode`, `XDocument`, and `XElement`; JSON/XML
  snapshots are LF-only, and repository attributes pin verified files to UTF-8/LF while ignoring received
  files. Defaults remain exact and never auto-accept.
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

### Changed

- `OccurredEvent.Timestamp` now comes from `AssertionClock.Current.UtcNow` instead of
  `DateTime.UtcNow`; pin a `ManualClock` when event timestamps must be deterministic.
- XML equivalence for `XDocument`, `XElement`, and classic `XmlElement` is structural: attributes
  are unordered, while element order, whitespace, and comments remain significant unless
  `XmlEquivalencyOptions` relaxes them. Failures report simplified XPath paths instead of dumping
  both documents.

### Fixed

- `BeApproximately`/`NotBeApproximately` no longer round `decimal` or 64-bit integers through
  `double`; `BigInteger` and 128-bit integers use exact arbitrary-precision differences.
  NaN never approximates a value, and equal infinities compare as approximately equal.

### Security

- `TestEnvironment` no longer interpolates file-system paths into a `chmod` command line. It uses
  `File.SetUnixFileMode` on .NET 7+, `ProcessStartInfo.ArgumentList` on .NET 6, and a safely escaped
  argument only on the `netstandard2.0` fallback, preventing argument injection through hostile paths.
- `TestEnvironment` now rejects absolute paths and relative paths that escape `Root` before any
  file-system operation; `CreateTree` and `CreateFiles` validate every path before creating entries.
