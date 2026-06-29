# CreateAnalyticalCurvedPanel

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `CreateAnalyticalCurvedPanel` |
| **Source** | `src/ContextualAnalyticalModel/CreateAnalyticalCurvedPanel.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Creates a curved analytical floor panel from a hard-coded `Arc` and sets its structural role and analysis type.

## What it demonstrates

- `AnalyticalPanel.Create(Document, Arc, XYZ normal)` for non-planar rectangular profiles
- `AnalyticalStructuralRole.StructuralRoleFloor` and `AnalyzeAs.SlabOneWay` assignment
- Transaction-scoped panel creation without user input

## Prerequisites

- Structural document; arc geometry must be valid for panel creation

## User interaction

- None — arc endpoints and normal are hard-coded in the command
- Fully headless as written

## MCP notes

- Proposed tool: `revit_create_analytical_curved_panel`
- Parameters: arc geometry (three points or center/radius), `normal`, optional `structural_role`, `analyze_as`
- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalcurvedpanel.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/createanalyticalcurvedpanel.json`
- Related: [createanalyticalpanel.md](createanalyticalpanel.md)
