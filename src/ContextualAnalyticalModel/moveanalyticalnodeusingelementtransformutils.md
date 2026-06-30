# MoveAnalyticalNodeUsingElementTransformUtils

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `MoveAnalyticalNodeUsingElementTransformUtils` |
| **Source** | `src/ContextualAnalyticalModel/MoveAnalyticalNodeUsingElementTransformUtils.cs` |
| **MCP rating** | 2/5 |

Moves a user-selected analytical node with `ElementTransformUtils` while keeping converging element connectivity intact.

## What it demonstrates

- Setup panel (`CreateAnalyticalPanel.CreateAmPanel`) and connected member (`CreateAnalyticalMember.CreateMember`)
- `PickObject(ObjectType.PointOnElement)` to select the node
- `ElementTransformUtils.MoveElement` with translation `(-5, -5, 0)`

## Prerequisites

- Document with analytical panel and member sharing a node after sample setup

## User interaction

- Requires picking a point on an analytical node after geometry is created
- Translation vector is hard-coded after the pick

## MCP notes

- Node move by ID and offset is automatable, but the command mixes setup geometry, interactive pick, and fixed offset — weak as a direct MCP tool.

## See also

- Related: [analyticalnodeconnstatus.md](analyticalnodeconnstatus.md), [moveanalyticalpanelusingelementtransformutils.md](moveanalyticalpanelusingelementtransformutils.md)
