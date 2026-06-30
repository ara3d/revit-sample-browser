# CreateAnalyticalMember

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateAnalyticalMember` |
| **Source** | `src/ContextualAnalyticalModel/CreateAnalyticalMember.cs` |
| **MCP rating** | 4/5 |

Creates a straight `AnalyticalMember` between two XYZ endpoints and exposes static helpers used by other commands in this sample.

## What it demonstrates

- `AnalyticalMember.Create(Document, Line)` from a bounded line curve
- `StructuralRole` (`StructuralRoleBeam`) and `AnalyzeAs` (`Lateral`) properties
- Static `CreateMember` and `CreateConvergentMember` helpers for shared test geometry

## Prerequisites

- Structural document supporting contextual analytical members

## User interaction

- None in the ribbon command — endpoints `(-5,0,0)` to `(0,0,0)` are fixed
- Other commands in the sample call the static helpers directly

## MCP notes

- Proposed tool: `revit_create_analytical_member`
- Parameters: `start_point`, `end_point`, optional `structural_role`, `analyze_as`
- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalmember.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalmember.json`
- Related: [flipanalyticalmember.md](flipanalyticalmember.md), [moveanalyticalmemberusingelementtransformutils.md](moveanalyticalmemberusingelementtransformutils.md)
