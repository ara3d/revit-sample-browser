# CmdCableTray3

| Field | Value |
|-------|-------|
| **Sample** | CableTraySample |
| **Class** | `CmdCableTray3` |
| **Source** | `src/CableTraySample/CmdCableTray3.cs` |
| **Origin** | [jeremytammik/CableTraySample](https://github.com/jeremytammik/CableTraySample) (MIT) |
| **MCP rating** | 2/5 |

Creates two cable tray segments meeting at a common point, rotates the second segment 90° about the vertical axis at the junction, then connects and inserts an elbow fitting.

## What it demonstrates

- `LocationCurve.Rotate` on a cable tray before fitting placement
- Elbow insertion after orienting the second segment

## Prerequisites

- MEP-enabled project with a cable tray type named `default` and level `Level 1`

## User interaction

- Non-interactive

## MCP notes

- Proposed tool: `revit_cable_tray_elbow_rotate`
- MCP descriptor: `src/CableTraySample/cable-tray-elbow-rotate.json`

## See also

- MCP descriptor: `src/CableTraySample/cable-tray-elbow-rotate.json`
- `CmdCableTray4` — aligns connector coordinate systems with a computed rotation before inserting the elbow