# Command

| Field | Value |
|-------|-------|
| **Sample** | MultiplanarRebar |
| **Class** | `Command` |
| **Source** | `src/MultiplanarRebar/Command.cs` |
| **SDK ReadMe** | `src/MultiplanarRebar/ReadMe_MultiplanarRebar.rtf` |
| **MCP rating** | 2/5 |

Adds multi-planar rebar to selected sloped concrete corbel structural connections using user-specified bar types and spacing.

## What it demonstrates

- Filtering selection to `OST_StructConnections` family instances with concrete/precast structural material
- `CorbelFrame.Parse` to derive geometry from sloped corbels
- `CorbelReinforcementOptions` and `CorbelFrame.Reinforce` inside a single transaction

## Prerequisites

- Sloped corbel family instances selected in a structural concrete project
- Rebar types and cover settings available in the options dialog

## User interaction

- `CorbelReinforcementOptionsForm` modal dialog for bar layout parameters
- Requires valid corbel selection; cancels if none qualify

## MCP notes

- Highly specialized rebar placement; would need corbel element ids and reinforcement options as parameters to be automation-friendly.
