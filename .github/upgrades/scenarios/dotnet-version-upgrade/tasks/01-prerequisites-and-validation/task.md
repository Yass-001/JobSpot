# 01-prerequisites-and-validation: Verify upgrade readiness

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

## Research Findings

### SDK Status
- ✅ .NET 10 SDK is installed and available
- ✅ No global.json file present in repository (no version pinning constraints)
- No SDK version conflicts detected

### Projects Loaded Successfully
1. F:\CS_My_Projects\JobSpot\JobSpot\JobSpot.csproj (ASP.NET Core)
2. F:\CS_My_Projects\JobSpot\JobSpot.Repository.Tests\JobSpot.Tests.csproj (xUnit Test Project)

Both project files are accessible and parse correctly.

**Done when**:
- ✅ .NET 10 SDK is confirmed installed
- ✅ global.json (if present) is compatible or will be updated
- ✅ All project files are validated to load
