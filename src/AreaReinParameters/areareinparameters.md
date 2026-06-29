# Command

| Field | Value |
|-------|-------|
| **Sample** | AreaReinParameters |
| **Class** | `Command` |
| **Source** | `src/AreaReinParameters/AreaReinParameters.cs` |
| **SDK ReadMe** | `src/AreaReinParameters/ReadMe_AreaReinParameters.rtf` |
| **MCP rating** | 2/5 |

Displays and edits area reinforcement parameters through a property-grid form bound to wall or floor reinforcement data.

## What it demonstrates

- Loading hook and bar types from the document into static hashtables
- `WallAreaReinData` / `FloorAreaReinData` adapters for `PropertyGrid` binding
- Transaction commit/rollback tied to dialog OK/Cancel

## Prerequisites

- Exactly one `AreaReinforcement` selected; project must contain `RebarHookType` and `RebarBarType` definitions

## User interaction

- `AreaReinParametersForm` modal dialog required for edits

## MCP notes

- Could expose read-only rebar settings via MCP, but the sample is optimized for interactive PropertyGrid editing

## See also

- [`rebarparas.md`](rebarparas.md) — companion command listing rebar parameters
