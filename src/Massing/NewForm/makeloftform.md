# MakeLoftForm

| Field | Value |
|-------|-------|
| **Sample** | Massing/NewForm |
| **Class** | `MakeLoftForm` |
| **Source** | `src/Massing/NewForm/Command.cs` |
| **MCP rating** | 2/5 |

Lofts four arc-based profiles (plus an empty fifth profile slot) into a single mass form.

## What it demonstrates

- `ReferenceArrayArray` of profiles for `FamilyCreate.NewLoftForm`
- `FormUtils.MakeArc` to create arc model curves on computed sketch planes
- Stacking multiple profiles at different elevations for a lofted shape

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; coordinates are fixed in code

## MCP notes

- Demonstrates loft form creation only; not a practical MCP surface without dynamic profile definitions.
