# CmdCableTray2

| Field | Value |
|-------|-------|
| **Sample** | CableTraySample |
| **Class** | `CmdCableTray2` |
| **Source** | `src/CableTraySample/CmdCableTray2.cs` |
| **Origin** | [jeremytammik/CableTraySample](https://github.com/jeremytammik/CableTraySample) (MIT) |
| **MCP rating** | 2/5 |

Creates two cable tray segments at fixed coordinates and attempts to connect them with an elbow fitting. Simplified variant from the upstream elbow-fitting investigation.

## What it demonstrates

- Two-segment `CableTray.Create` workflow
- `Connector.ConnectTo` and `NewElbowFitting` without segment rotation

## Prerequisites

- MEP-enabled project with cable tray type `Default` and level `Level 1`

## User interaction

- Non-interactive

## MCP notes

- Proposed tool: `revit_cable_tray_elbow_two_segment`
- MCP descriptor: `src/CableTraySample/cable-tray-elbow-two-segment.json`

## See also

- MCP descriptor: `src/CableTraySample/cable-tray-elbow-two-segment.json`
- `CmdCableTray3` — rotates the second segment before inserting the elbow
