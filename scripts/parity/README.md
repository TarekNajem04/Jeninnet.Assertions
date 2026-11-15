# Peer parity harness

Measures the public `Should()` surface of Jeninnet.Assertions against pinned peer
packages, so gaps are evidence rather than memory. It is a local measurement tool,
not a build or test dependency.

## Requirements

- PowerShell 7 (`pwsh`).
- Network access on the first run to download the pinned NuGet packages.
- No .NET SDK is required in baseline mode; pass `-OurAssembly` only when you want
  to reflect a freshly built assembly.

## Usage

```powershell
pwsh scripts/parity/compare.ps1
```

The first run downloads packages into `artifacts/parity/packages/`, extracts them into
`artifacts/parity/extracted/`, and writes:

- `artifacts/parity/report.md` — human-readable report.
- `artifacts/parity/report.json` — machine-readable report.

Useful switches:

```powershell
# Re-download and re-extract the pinned packages.
pwsh scripts/parity/compare.ps1 -Refresh

# Reflect a built assembly instead of the committed API baseline.
pwsh scripts/parity/compare.ps1 -OurAssembly src/Jeninnet.Assertions/bin/Release/net11.0/Jeninnet.Assertions.dll

# Suppress progress output.
pwsh scripts/parity/compare.ps1 -Quiet
```

All paths are resolved from the repository root unless an absolute path is supplied.

## What it measures

- Exported type count per package.
- Static extension `Should` overloads and the subject type of their first parameter.
- Public instance verbs on the assertion type returned by `Should`.
- A curated probe list (`Uri`, `IComparable<T>`, `IReadOnlyDictionary<,>`,
  `KeyValuePair<,>`, `JsonElement`, `JsonDocument`, `ParameterInfo`, `char`) with
  an interpretation note per probe.
- License and dependency metadata read from each package's `.nuspec`.
- Global verb-name differences: missing in Jeninnet, extra in Jeninnet, shared.

## Interpretation rules

- **A missing subject is a lead, not a verdict.** Confirm the peer version and the
  actual API shape before opening a task.
- **Baseline mode is declared-only.** Inherited members are not expanded; reflection
  mode (`-OurAssembly`) includes them. Use baseline mode for a stable checkout-level
  comparison and reflection mode for local investigation.
- **Subject keys are type names, not full signatures.** `Nullable<T>` collapses all
  nullable value types into one key; that is intentional for a high-level matrix.
- **Shouldly and NFluent use different entry points.** Their `Should()` rows are
  empty by design because they expose `ShouldBe...` and `Check.That`.
- **Generic catch-all subjects** such as `Should<T>(this T)` are grouped under `<T>`
  and are not a parity signal by themselves.
- **Dependency lists** cover every nuspec group, not only the reflected TFM.
- **Counts are directional.** Property accessors, compiler-generated members, and
  inherited methods differ between libraries; do not treat them as a score.

## Adding a probe

Edit `peer-versions.json`:

```json
{
  "name": "Uri",
  "fullName": "System.Uri",
  "note": "Why this probe matters and how to read a missing result."
}
```

The key is derived from `fullName` (the short type name plus generic arity). Keep
the note factual: a probe is a question, not a marketing claim.

## Version policy

`peer-versions.json` pins exact versions; never use `latest`. Re-pin deliberately,
re-run the report, and review the diff before writing tasks. The comparison is only
as honest as the versions it names.

## Safety and boundaries

- Packages are reflected, never executed.
- No package is added to `src/`; the harness does not affect the shipped library.
- Outputs live under `artifacts/` (gitignored) and are not published.
- The script is not part of CI. If it ever becomes a gate, it should run in an
  isolated, network-enabled job with pinned packages and a reviewed report.
