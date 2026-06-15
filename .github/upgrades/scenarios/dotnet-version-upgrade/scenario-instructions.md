# .NET 10 Upgrade

## Preferences
- **Flow Mode**: Guided
- **Target Framework**: net10.0 (.NET 10 LTS)

## Source Control
- **Source Branch**: master
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Upgrade Options
- **Upgrade Strategy**: All-at-Once

## Strategy
**Selected**: All-at-Once
**Rationale**: Small scope (2 projects, ≤15 projects threshold), simple 2-tier dependency graph, all modern .NET (net9.0 → net10.0), all SDK-style projects. Single atomic upgrade with full solution validation after completion. No multi-targeting overhead needed. Fastest and simplest approach for this scope.

### Execution Constraints
- Upgrade both projects simultaneously (no phased approach)
- All projects must reach net10.0 before validation
- Full solution build must succeed with zero errors and zero warnings after all upgrades
- All xUnit tests must pass after upgrade
- Per commit strategy "After Each Task": commit after each task completes (prerequisites → targets → packages → APIs → validation)
