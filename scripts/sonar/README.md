# Sonar preview tooling

`preview.ps1` runs the same SonarScanner analysis as `.github/workflows/sonarcloud.yml`
locally, then prints the open issues and the Quality Gate status.

Two targets are supported:

- **SonarCloud** (default) — needs a User Token; the Free plan only accepts main-branch analyses.
- **Local SonarQube** (`-Local`) — no cloud account needed; ideal before the repository is on GitHub.

## Prerequisites

- .NET SDK 10 and `dotnet-sonarscanner` (`dotnet tool install --global dotnet-sonarscanner`).
- Java 21 (the script resolves a JDK automatically; `SONAR_JAVA_PATH` overrides).
- Local mode: SonarQube Community Build (zip) running at <http://localhost:9000>.

## Local SonarQube (Community Build)

1. Download and unzip SonarQube Community Build (Java 21 required):
   <https://www.sonarsource.com/products/sonarqube/downloads/>
2. Start the server (set `SONAR_JAVA_PATH` when Java 21 is not on `PATH`):

   ```powershell
   $env:SONAR_JAVA_PATH = "C:\Program Files\Android\openjdk\jdk-21.0.8\bin\java.exe"
   & "$env:USERPROFILE\Tools\SonarQube\bin\windows-x86-64\StartSonar.bat"
   ```

3. Open <http://localhost:9000> (first login `admin`/`admin`), change the password, then
   create a token: **My Account > Security > Generate Tokens**.
4. Stop the server:

   ```powershell
   Get-CimInstance Win32_Process -Filter "Name='java.exe'" |
       Where-Object { $_.CommandLine -match 'sonar-application' } |
       ForEach-Object { Stop-Process -Id $_.ProcessId }
   ```

## Usage

```powershell
$env:SONAR_TOKEN = "<token>"

# Local SonarQube (no cloud account)
.\scripts\sonar\preview.ps1 -Local

# Local with tests and coverage (full CI simulation)
.\scripts\sonar\preview.ps1 -Local -WithCoverage

# SonarCloud (after the repository is on GitHub)
.\scripts\sonar\preview.ps1

# Only print the issues and Quality Gate currently stored on the server
.\scripts\sonar\preview.ps1 -FetchIssues

# Fail (exit 1) when the Quality Gate is not OK
.\scripts\sonar\preview.ps1 -Local -FailOnQualityGate
```

`-ServerUrl` and `-ProjectKey` override the defaults
(SonarCloud / `jeninnet-assertions`).

## Notes

- Coverage requires `coverlet.collector` (referenced by the test projects) and the
  `tests/Jeninnet.Assertions.Tests/.runsettings` file. The test step must not use
  `--no-build`, or VSTest does not discover the collector.
- The scanner honors `.editorconfig` severities, so rules pinned there stay silent both
  locally and in CI.
