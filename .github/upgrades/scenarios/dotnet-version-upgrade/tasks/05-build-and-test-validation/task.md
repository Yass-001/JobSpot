# 05-build-and-test-validation: Build solution, fix warnings, run tests

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
