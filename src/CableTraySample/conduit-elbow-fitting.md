# CmdConduit

| Field | Value |
|-------|-------|
| **Sample** | CableTraySample |
| **Class** | `CmdConduit` |
| **Source** | `src/CableTraySample/CmdConduit.cs` |
| **Origin** | [jeremytammik/CableTraySample](https://github.com/jeremytammik/CableTraySample) (MIT) |
| **MCP rating** | 2/5 |

Creates two conduit runs at fixed coordinates and inserts an elbow fitting between them. Included in the upstream sample as a simpler MEP elbow-fitting baseline.

## What it demonstrates

- `Conduit.Create` with the first available `ConduitType`
- Connector lookup and `NewElbowFitting` for conduit

## Prerequisites

- MEP-enabled project with at least one conduit type
- Level named `Level 1`

## User interaction

- Non-interactive; creates geometry at fixed model coordinates

## MCP notes

- Proposed tool: `revit_conduit_elbow_fitting`
- MCP descriptor: `src/CableTraySample/conduit-elbow-fitting.json`

## See also

- MCP descriptor: `src/CableTraySample/conduit-elbow-fitting.json`
- `CmdCableTray2` — same elbow workflow for cable tray
