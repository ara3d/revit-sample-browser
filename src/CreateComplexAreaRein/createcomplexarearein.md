# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateComplexAreaRein |
| **Class** | `Command` |
| **Source** | `src/CreateComplexAreaRein/CreateComplexAreaRein.cs` |
| **MCP rating** | 2/5 |

Creates area reinforcement on a single selected structural rectangular floor slab, then applies user-chosen bar layout parameters through a dialog.

## What it demonstrates

- Selection validation for exactly one `Floor` with rectangular horizontal geometry (`GeomHelper.GetFloorGeom`)
- `AreaReinforcement.Create` with default `AreaReinforcementType`, `RebarBarType`, and `RebarHookType`
- Major direction derived from the first boundary line; `AreaReinData.FillIn` sets curve parameters

## Prerequisites

- Pre-select one structural rectangular horizontal slab

## User interaction

- `CreateComplexAreaReinForm` configures reinforcement parameters; cancel rolls back the transaction

## MCP notes

- Requires slab selection and a modal form for bar spacing and hook settings — poor headless MCP fit without refactoring parameter input.

## See also

- Related: [createsimplearearein.md](../CreateSimpleAreaRein/createsimplearearein.md)
