# Upgrade Options — JobSpot

Assessment: 2 projects (both net9.0 → net10.0), 15 packages (7 need upgrade, 1 deprecated), 6 source-incompatible APIs, 1 behavioral change, 2-tier dependency graph, minimal LOC impact (7+)

## Strategy

### Upgrade Strategy
Both projects are on modern .NET with a simple 2-tier dependency graph and small scope (≤15 projects). All-at-Once is the fastest and simplest approach — upgrade both projects simultaneously in a single pass.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously. No multi-targeting, solution-wide validation after upgrade. Fastest approach for small scopes. |
| Top-Down | Upgrade applications first, multi-target libraries temporarily, consolidate in phase 2. Use when incremental buildability is critical or for larger solutions. |
