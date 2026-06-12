# 03-update-nuget-packages: Upgrade packages to .NET 10 versions

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
