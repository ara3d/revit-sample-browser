# Command

| Field | Value |
|-------|-------|
| **Sample** | SheetToView3D |
| **Class** | `Command` |
| **Source** | `src/SheetToView3D/CS/SheetToView3D.cs` |
| **SDK ReadMe** | `src/SheetToView3D/CS/ReadMe_SheetToView3D.rtf` |
| **MCP rating** | 5/5 |

Creates a perspective 3D view whose camera is placed at the sheet click mapped into model space through viewport transforms.

## What it demonstrates

- `Viewport.GetBoxOutline`, `GetProjectionToSheetTransform`, and `View.GetModelToProjectionTransforms`
- Inverting sheet-to-model transforms for multi-crop plan views
- Ray-in-crop test with `CurveLoop` point-in-polygon logic
- `ViewPlan` cut-plane projection and `View3D.CreatePerspective` with `SetOrientation`

## Prerequisites

- Active sheet view with a plan viewport that reports view and viewport transforms

## User interaction

- Requires `PickPoint` on a viewport while a sheet is active; automation can pass sheet UV and viewport id instead

## MCP notes

- Proposed tool: `revit_create_3d_view_from_sheet_click`
- Parameters: `viewport_id`, `sheet_point` (x, y in sheet space) or model `xyz`
- Returns: new `View3D` element id
- MCP descriptor: `docs/mcp/SheetToView3D/sheettoview3d.json`

## See also

- MCP descriptor: `docs/mcp/SheetToView3D/sheettoview3d.json`
