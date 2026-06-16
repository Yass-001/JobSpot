# 03-update-nuget-packages: Upgrade packages to .NET 10 versions

Upgrade 7 NuGet packages that have newer versions supporting .NET 10. Address the deprecated xunit package.

**Scope**: Package updates in both projects

## Research Findings

### Projects Affected
1. **F:\CS_My_Projects\JobSpot\JobSpot\JobSpot.csproj** (net10.0 - Web)
   - 6 packages need upgrade
   - 3 packages already compatible

2. **F:\CS_My_Projects\JobSpot\JobSpot.Repository.Tests\JobSpot.Tests.csproj** (net10.0 - Test)
   - 3 packages need upgrade (with overlap to main project)
   - 4 packages already compatible
   - 1 deprecated package (xunit)

### Package Management Mode
- **Mode**: Standard (non-CPM) — no Directory.Packages.props found
- **Strategy**: Update Version attribute directly in each project file

### Packages to Update

#### JobSpot.csproj (Main Project)

| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| Microsoft.AspNetCore.Authentication.Google | 9.0.14 | 10.0.9 | Upgrade required |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.10 | 10.0.9 | Upgrade required |
| Microsoft.AspNetCore.Identity.UI | 9.0.10 | 10.0.9 | Upgrade required |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.10 | 10.0.9 | Upgrade required |
| Microsoft.EntityFrameworkCore.Tools | 9.0.10 | 10.0.9 | Upgrade required (PrivateAssets) |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 9.0.0 | 10.0.2 | Upgrade required |
| Serilog | 4.3.0 | 4.3.0 | ✅ Compatible |
| Serilog.AspNetCore | 10.0.0 | 10.0.0 | ✅ Compatible |
| Serilog.Sinks.File | 7.0.0 | 7.0.0 | ✅ Compatible |

#### JobSpot.Tests.csproj (Test Project)

| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| Microsoft.AspNetCore.Authentication.Google | 9.0.14 | 10.0.9 | Upgrade required |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.11 | 10.0.9 | Upgrade required |
| Microsoft.EntityFrameworkCore.Tools | 9.0.10 | 10.0.9 | Upgrade required (PrivateAssets) |
| xunit | 2.9.2 | 2.9.2 | ⚠️ Deprecated — keep v2 (no breaking changes in v2.9.2) |
| coverlet.collector | 6.0.2 | 6.0.2 | ✅ Compatible |
| Microsoft.NET.Test.Sdk | 17.12.0 | 17.12.0 | ✅ Compatible |
| Moq | 4.20.72 | 4.20.72 | ✅ Compatible |
| xunit.runner.visualstudio | 2.8.2 | 2.8.2 | ✅ Compatible |

### Decisions Made
- **xunit**: Keeping v2.9.2 (no migration to v3 needed at this stage). xunit 2.9.2 is compatible with net10.0.
- **EntityFrameworkCore.Tools**: PrivateAssets attributes preserved during version update
- **Approach**: Sequential replacement in both project files, then build validation

### Dependencies & Risks
- ✅ No CPM used — straightforward Version attribute updates
- ✅ No transitive dependency conflicts identified
- ✅ All target packages have net10.0 versions available
- ⚠️ EntityFrameworkCore tools may trigger migration scaffolding checks (expected, safe)

**Done when**:
- All 7 packages upgraded to recommended versions in both project files
- Solution restores without package resolution errors
- Build succeeds after package updates
