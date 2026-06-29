# Command

| Field | Value |
|-------|-------|
| **Sample** | GenerateFloor |
| **Class** | `Command` |
| **Source** | `src/GenerateFloor/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates a floor from a profile, floor type, and level chosen in a dialog.

## What it demonstrates

- `Data.ObtainData` reading document context; `GenerateFloorForm` for user choices
- Static `CreateFloor` building a `CurveLoop` and calling `Floor.Create`
- Transaction commit/rollback tied to dialog OK/Cancel

## Prerequisites

- Project with at least one floor type, level, and definable boundary profile (from UI)

## User interaction

- Modal `GenerateFloorForm`; floor is only created on OK

## MCP notes

- Proposed tool: `revit_create_floor` with `profile_curve_loop`, `floor_type_id`, `level_id`, and `structural` boolean. `CreateFloor` is already a clean static entry point.

## See also

- MCP descriptor: `src/GenerateFloor/generatefloor.json`
