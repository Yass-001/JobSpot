# 02-update-project-targets: Change target framework to net10.0

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
