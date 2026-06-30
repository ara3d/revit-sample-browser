# MoveAnalyticalPanelUsingElementTransformUtils

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MoveAnalyticalPanelUsingElementTransformUtils` |
| **Source** | `src/ContextualAnalyticalModel/MoveAnalyticalPanelUsingElementTransformUtils.cs` |
| **MCP rating** | 2/5 |

Translates an analytical panel with `ElementTransformUtils` while maintaining its connection to an adjacent analytical member.

## What it demonstrates

- `CreateAnalyticalPanel.CreateAmPanel` plus connected `CreateAnalyticalMember.CreateMember`
- `ElementTransformUtils.MoveElement(document, panelId, new XYZ(5, 5, 0))`
- Rigid panel move preserves panel–member node connectivity (per SDK expected results)

## Prerequisites

- Structural document; command creates panel and member automatically

## User interaction

- None — translation `(5, 5, 0)` is fixed
- Compare with sketch-edit panel move that breaks connectivity

## MCP notes

- Panel translation by element ID is straightforward to automate, but this sample is a fixed demo without caller parameters.

## See also

- Related: [moveanalyticalpanelusingsketcheditscope.md](moveanalyticalpanelusingsketcheditscope.md), [createanalyticalpanel.md](createanalyticalpanel.md)
