# 04-fix-api-incompatibilities: Resolve API changes

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
