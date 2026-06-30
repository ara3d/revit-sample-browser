# CmdRenumber

| Field | Value |
|-------|-------|
| **Sample** | SetoutPoints |
| **Class** | `CmdRenumber` |
| **Source** | `src/SetoutPoints/CmdRenumber.cs` |
| **Origin** | [jeremytammik/SetoutPoints](https://github.com/jeremytammik/SetoutPoints) (MIT) |
| **MCP rating** | 4/5 |

Renumbers all major (key) setout point family instances with an `SOP ` prefix, starting at one.

## What it demonstrates

- Finding setout points by `FamilyInstanceFilter` on the major symbol type
- Updating the `Point_Number` shared parameter on each instance inside a transaction

## Prerequisites

- Setout point family loaded in the project (typically after running `CmdGeomVertices`)
- At least one major setout point instance, or the command completes with an informational dialog

## User interaction

- Non-interactive; shows a `TaskDialog` only when the setout point family is not loaded

## MCP notes

- Proposed tool: `revit_renumber_setout_points`
- Parameters: optional `prefix` (default `SOP `), optional filter by host element ids
- Returns: count of renumbered instances and the assigned labels
- MCP descriptor: `src/SetoutPoints/renumber.json`

## See also

- MCP descriptor: `src/SetoutPoints/renumber.json`
- Companion command: [CmdGeomVertices](geom-vertices.md)
- Upstream: [jeremytammik/SetoutPoints](https://github.com/jeremytammik/SetoutPoints)
