# SetOuterContourForPanels

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `SetOuterContourForPanels` |
| **Source** | `src/ContextualAnalyticalModel/SetOuterContourForPanels.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 2/5 |

Creates an analytical panel, then replaces its outer boundary with a custom four-sided `CurveLoop` using `AnalyticalPanel.SetOuterContour`.

## What it demonstrates

- Delegating panel creation to `CreateAnalyticalPanel.CreateAmPanel`
- Building a closed profile from `Line.CreateBound` segments
- `AnalyticalPanel.SetOuterContour(CurveLoop)` inside a manual transaction

## Prerequisites

- Document that supports contextual analytical panels

## User interaction

- None — contour vertices are hard-coded in model coordinates (0,0) through (5,5) with one concave edge

## MCP notes

- Contour editing is automatable with explicit vertex lists, but this sample only demonstrates a fixed shape after auto-creating a panel — limited MCP value as written.

## See also

- Related: [createanalyticalpanel.md](createanalyticalpanel.md), [modifypanelcontour.md](modifypanelcontour.md)
