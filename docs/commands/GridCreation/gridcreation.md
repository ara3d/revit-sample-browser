# Command

| Field | Value |
|-------|-------|
| **Sample** | GridCreation |
| **Class** | `Command` |
| **Source** | `src/GridCreation/CS/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates structural grids from selected curves, orthogonal spacing, or radial/arc layouts via a multi-step dialog workflow.

## What it demonstrates

- Three modes: `CreateWithSelectedCurvesData`, `CreateOrthogonalGridsData`, `CreateRadialAndArcGridsData`
- Reading selected model/detail lines and arcs; avoiding duplicate grid labels via `GetAllLabelsOfGrids`
- `Grid` creation inside transactions after option forms return OK

## Prerequisites

- Project document; optional pre-selected lines/arcs for the Select mode

## User interaction

- `GridCreationOptionForm` plus a mode-specific form; cancel at any step aborts creation

## MCP notes

- Proposed tool: `revit_create_grids` with `mode` (`selected`, `orthogonal`, `radial_arc`) and spacing/label parameters. Bypass WinForms and call data classes directly.

## See also

- MCP descriptor: `docs/mcp/GridCreation/gridcreation.json`
