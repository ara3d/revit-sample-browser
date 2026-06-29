# CreateAnalyticalPanel

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateAnalyticalPanel` |
| **Source** | `src/ContextualAnalyticalModel/CreateAnalytcalPanel.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Creates a rectangular analytical floor panel from a `CurveLoop` profile, then adds an `AnalyticalOpening` cutout on that panel.

## What it demonstrates

- `AnalyticalPanel.Create(Document, CurveLoop)` with a four-segment rectangular loop
- `AnalyticalOpening.IsCurveLoopValidForAnalyticalOpening` validation before creation
- `AnalyticalOpening.Create(Document, CurveLoop, ElementId panelId)` hosted opening
- Static `CreateAmPanel` and `CreateAmOpening` helpers reused by move/modify commands

## Prerequisites

- Structural document; opening loop must lie inside panel boundaries

## User interaction

- None — panel and opening vertices are hard-coded
- Headless-friendly; geometry is parameterized only in code

## MCP notes

- Proposed tool: `revit_create_analytical_panel`
- Parameters: `boundary_points[]`, optional `opening_loops[]`, `structural_role`, `analyze_as`
- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalpanel.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalpanel.json`
- Related: [createanalyticalcurvedpanel.md](createanalyticalcurvedpanel.md), [modifypanelcontour.md](modifypanelcontour.md)
