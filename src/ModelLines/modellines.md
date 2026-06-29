# Command

| Field | Value |
|-------|-------|
| **Sample** | ModelLines |
| **Class** | `Command` |
| **Source** | `src/ModelLines/Command.cs` |
| **SDK ReadMe** | `src/ModelLines/ReadMe_ModelLines.rtf` |
| **MCP rating** | 4/5 |

Counts existing model curves by type, then creates sample model lines, arcs, ellipses, Hermite splines, and NURBS curves through a dialog-driven workflow.

## What it demonstrates

- `ModelLines` helper enumerating and creating `ModelCurve` variants via `Creation.Document`
- Sketch plane selection and binding curves to planes
- Tracking created elements in typed `ModelCurveArray` collections for UI display

## Prerequisites

- Project or family document where model curves are allowed in the active context

## User interaction

- `ModelLinesForm` and `SketchPlaneForm` drive creation and display counts
- `ModelLines.Run()` contains the creatable geometry logic separable from forms

## MCP notes

- Proposed tool: `revit_create_model_curves`
- Parameters: curve type, plane definition or sketch plane id, control points or radii
- Returns: created model curve element ids and per-type counts
- Refactor: accept explicit geometry instead of form-driven sketch plane picks
- MCP descriptor: `src/ModelLines/modellines.json`

## See also

- MCP descriptor: `src/ModelLines/modellines.json`
