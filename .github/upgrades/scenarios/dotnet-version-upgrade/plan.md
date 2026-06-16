# Upgrade Plan — .NET 9 to .NET 10

**Strategy**: All-at-Once

**Target Framework**: net10.0 (.NET 10 LTS)

**Projects**: 2
- JobSpot.csproj (ASP.NET Core)
- JobSpot.Tests.csproj (xUnit Tests)

---

## Task Overview

All projects will be upgraded simultaneously in a single atomic pass. The solution will be validated comprehensively after all upgrades are complete.

| # | Task | Description |
|----|------|-------------|
| 1 | Prerequisites & Validation | Verify SDK installation, validate .NET SDK compatibility |
| 2 | Update Project Targets | Change target framework from net9.0 to net10.0 in both projects |
| 3 | Update NuGet Packages | Upgrade 7 packages to .NET 10 compatible versions; address deprecated xunit package |
| 4 | Fix API Incompatibilities | Resolve 6 source-incompatible APIs and 1 behavioral change in Program.cs |
| 5 | Build & Test Validation | Build solution, fix warnings, run all tests |

---

## Tasks

### 01-prerequisites-and-validation: Verify upgrade readiness

The solution requires a .NET 10 SDK. Before making any changes, verify that the required SDK is installed and available in global.json (if present). Validate that all projects can load with the new framework version.

**Scope**: Solution-level preparation

**Assessment context**:
- Target: net10.0 (.NET 10 LTS)
- Current: net9.0 for both projects
- SDK requirement: .NET 10 SDK must be installed

**Research starting points**:
- Check if global.json exists in the repo and whether it pins a specific SDK version
- Verify .NET 10 SDK is installed locally
- Confirm all project files are accessible and valid

**Done when**:
- .NET 10 SDK is confirmed installed
- global.json (if present) is compatible or will be updated
- All project files are validated to load

---

### 02-update-project-targets: Change target framework to net10.0

Update the TargetFramework property in both JobSpot.csproj and JobSpot.Tests.csproj from net9.0 to net10.0. This is the core framework version bump.

**Scope**: 
- JobSpot.csproj
- JobSpot.Tests.csproj

**Assessment context**:
- Both projects currently target net9.0
- Both are SDK-style projects (simplified migration path)
- Dependency: JobSpot.csproj is upstream; JobSpot.Tests.csproj depends on it
- Package impact: 7 packages need upgrading after TFM change

**Known risks**:
- API incompatibilities will surface after TFM change (6 source-incompatible APIs + 1 behavioral change)
- NuGet packages will need version updates to align with net10.0

**Research starting points**:
- Inspect both .csproj files for the current TargetFramework declaration
- Note the current framework identifier (net9.0) to confirm it needs changing

**Done when**:
- Both .csproj files have TargetFramework set to net10.0
- Solution reloads in Visual Studio without errors
- Project references are recognized

---

### 03-update-nuget-packages: Upgrade packages to .NET 10 versions

Upgrade 7 NuGet packages that have newer versions supporting .NET 10. Address the deprecated xunit package.

**Scope**: Package updates in both projects

**Assessment context**:
- 7 packages need version upgrades:
  - Microsoft.AspNetCore.Authentication.Google: 9.0.14 → 10.0.9
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore: 9.0.10 → 10.0.9
  - Microsoft.AspNetCore.Identity.UI: 9.0.10 → 10.0.9
  - Microsoft.EntityFrameworkCore.InMemory: 9.0.11 → 10.0.9
  - Microsoft.EntityFrameworkCore.SqlServer: 9.0.10 → 10.0.9
  - Microsoft.EntityFrameworkCore.Tools: 9.0.10 → 10.0.9
  - Microsoft.VisualStudio.Web.CodeGeneration.Design: 9.0.0 → 10.0.2
- 1 deprecated package: xunit 2.9.2 (v3 migration guide available)
- 7 packages are already compatible (no upgrade needed)

**Known risks**:
- xunit is in maintenance mode; v3 is available but may require code changes
- EntityFrameworkCore tools package may trigger migrations update check

**Research starting points**:
- Verify each package's current version in .csproj files
- Check if xunit is actively used in tests (if so, consider v3 migration path or stay on v2 with v2 latest)
- Confirm no breaking changes in the target versions of identity/auth packages

**Done when**:
- All 7 packages upgraded to recommended versions
- xunit package addressed (decision: stay on v2, migrate to v3, or update to latest v2)
- Solution restores without package resolution errors
- NuGet packages lock or dependency files updated if present

---

### 04-fix-api-incompatibilities: Resolve API changes

Six APIs in Program.cs are source-incompatible with .NET 10. One API has a behavioral change. These need investigation and code updates.

**Scope**: Program.cs in JobSpot.csproj

**Assessment context**:
- 6 source-incompatible APIs (require code changes):
  - Microsoft.Extensions.DependencyInjection.GoogleExtensions (Google auth extension)
  - Microsoft.AspNetCore.Authentication.Google.GoogleOptions (Google auth options)
  - Microsoft.Extensions.DependencyInjection.IdentityServiceCollectionUIExtensions (Identity UI extensions)
  - Microsoft.AspNetCore.Identity.IdentityOptions (Identity options)
  - Microsoft.Extensions.DependencyInjection.IdentityEntityFrameworkBuilderExtensions (Identity EF extensions)
- 1 behavioral change:
  - Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler (exception handling middleware)

**Known risks**:
- Authentication and Identity API changes may require service collection reorganization
- Exception handler behavioral change may affect error handling in middleware pipeline
- Changes affect the dependency injection and middleware configuration in Program.cs

**Research starting points**:
- Open Program.cs and locate GoogleExtensions, IdentityServiceCollectionUIExtensions, IdentityEntityFrameworkBuilderExtensions calls
- Check Microsoft docs for .NET 10 authentication and identity API changes
- Review the exception handler middleware configuration for the behavioral change
- Verify that Identity scaffolded pages (if any) don't depend on removed APIs

**Done when**:
- All 6 source-incompatible API calls updated to use new .NET 10 equivalents
- Behavioral change in exception handler reviewed and adjusted if needed
- Program.cs compiles without errors
- No CS0246 (missing references), CS0103 (undefined names), or CS0117 (type missing member) errors
- Authentication and identity configuration works correctly at runtime

---

### 05-build-and-test-validation: Build solution, fix warnings, run tests

Build the entire solution, eliminate all build warnings, and run all xUnit tests. This is the final validation step.

**Scope**: Full solution (both projects)

**Assessment context**:
- 2 projects to build and test
- 58 total code files (3 with issues identified)
- 6199 lines of code
- xUnit test project with multiple test fixtures
- Serilog logging infrastructure already in place (no logging framework modernization needed)

**Known risks**:
- New compiler warnings may appear after .NET 10 update (common with framework upgrades)
- Tests may fail if they depend on .NET 9 specific behavior or APIs
- Entity Framework migrations may need regeneration

**Research starting points**:
- Run full solution build and capture all warnings and errors
- Review warning categories (e.g., nullable reference types, obsolete APIs)
- Execute all unit tests in JobSpot.Tests.csproj
- Check for any flaky tests that may be intermittent

**Done when**:
- Solution builds with zero errors
- Zero build warnings (fix all, no suppressions)
- All xUnit tests pass
- No failing migrations or database schema issues
- Deployment artifacts build successfully (DLL files, etc.)
- Commit changes to the upgrade working branch

---

## Success Criteria

- ✅ Both projects target net10.0
- ✅ All 7 NuGet packages upgraded to .NET 10 versions
- ✅ xunit package addressed (decision logged)
- ✅ All API incompatibilities resolved in Program.cs
- ✅ Solution builds without errors or warnings
- ✅ All tests pass
- ✅ Changes committed with descriptive message
