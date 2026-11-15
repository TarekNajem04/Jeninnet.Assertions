# Release checklist

This checklist covers the local pre-release verification for Jeninnet.Assertions. Publishing is a separate,
explicitly approved step (`nuget-publish.yml`), and no task may bump `<Version>` in `Directory.Build.props`.

## 1. Pre-flight

- [ ] Work is on the intended branch and `git status` contains only the release changes.
- [ ] `dotnet build src/Jeninnet.Assertions/Jeninnet.Assertions.csproj -c Release` ends with `0 Error(s)` for all four TFMs.
- [ ] `dotnet test tests/Jeninnet.Assertions.Tests/Jeninnet.Assertions.Tests.csproj -c Release` ends with `Failed: 0`.
- [ ] `pwsh scripts/docs/check-doc-parity.ps1`, `check-readme-claims.ps1`, and `check-localization.ps1` are green.
- [ ] The analyzer audit (temporary IDE severities with `EnforceCodeStyleInBuild`, then reverted) reports no IDE/CA/S/RCS diagnostics.

## 2. Public API and documentation

- [ ] `PublicApiSurface_MatchesCommittedBaseline` passes; the baseline changes only when the public surface intentionally changed.
- [ ] `README.md` and `CHANGELOG.md` describe the release's public changes under `[Unreleased]`.
- [ ] The AOT/trimming matrix and the performance table match the current annotations and measurements.
- [ ] No `[Obsolete]` members and no version bump were introduced.

## 3. Benchmarks

- [ ] `dotnet run --project benchmarks/Jeninnet.Assertions.Benchmarks/Jeninnet.Assertions.Benchmarks.csproj -c Release -- --filter '*' --exporters github` completes.
- [ ] The recorded environment (OS, CPU, .NET SDK/runtime, BenchmarkDotNet version) is included with any published numbers.
- [ ] Numbers are treated as order-of-magnitude; compare against the previous report on the same class of hardware, not absolutes.
- [ ] The advisory `Benchmarks` workflow is green for the release commit (or the local run above is attached).

## 4. Package verification

- [ ] `dotnet pack src/Jeninnet.Assertions/Jeninnet.Assertions.csproj -c Release --output artifacts/packages` succeeds.
- [ ] `pwsh scripts/release/verify-package.ps1` succeeds.
- [ ] The package contains `lib/netstandard2.0`, `lib/net6.0`, `lib/net8.0`, `lib/net10.0`, and `lib/net11.0` assemblies with XML docs.
- [ ] `README.md`, `icon.png`, and the MIT license expression are present in the package.
- [ ] The nuspec carries `repository type="git"`, the repository URL, and the current commit hash.
- [ ] The sibling `.snupkg` contains a PDB for every TFM, and no source, test, or local scratch files leak into either archive.
- [ ] No test-framework dependency (MSTest, xUnit, NUnit, TUnit) appears in the dependency groups.

## 5. CI gates

- [ ] `ci.yml` `build-and-test` passes on Windows, Ubuntu, and macOS.
- [ ] `ci.yml` `package-validation` packs with validation and runs the package verification step.
- [ ] `docs-build.yml` and `sonarcloud.yml` are green for the release commit.
- [ ] `Benchmarks` is not part of the blocking PR path; it runs weekly or manually only.

## 6. Publishing (only after explicit approval)

- [ ] `Directory.Build.props` `<Version>` matches the release tag `v<version>` (four-part comparison is allowed).
- [ ] Pushing the tag triggers `nuget-publish.yml`, which re-runs build, tests, package validation,
      `verify-package.ps1`, and uploads the `.nupkg`/`.snupkg` files to nuget.org.
- [ ] Symbols ride along with the package push; do not push `.snupkg` files in a separate step.
- [ ] The GitHub release is created from the same tag and includes the packed artifacts.

## 7. Notes

- Benchmarks are advisory: a noisy runner never fails a release by itself.
- Snapshot `.verified.*` files follow the LF policy in `.gitattributes`; never normalize them as a side effect.
- The `qps-ploc` pseudo-locale is included in the package as a resource today; exclude it only with a deliberate pack change that keeps the localization tests intact.
