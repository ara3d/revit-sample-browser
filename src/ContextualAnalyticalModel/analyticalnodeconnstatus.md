# AnalyticalNodeConnStatus

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `AnalyticalNodeConnStatus` |
| **Source** | `src/ContextualAnalyticalModel/AnalyticalNodeConnStatus.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 2/5 |

Queries the connection status of a user-selected analytical node using `AnalyticalNodeData`.

## What it demonstrates

- `Selection.PickObject` with `ObjectType.Element` to select an analytical node
- `AnalyticalNodeData.GetAnalyticalNodeData(element)` and `GetConnectionStatus()`
- Read-only inspection of node connectivity (result is not surfaced to the user)

## Prerequisites

- Document containing analytical nodes from members, panels, or other converging elements

## User interaction

- Single element pick prompt; no dialog or TaskDialog output
- Connection status is retrieved but not displayed — suitable as an API snippet only

## MCP notes

- Poor automation candidate: requires interactive pick and discards the status result; a useful tool would accept `node_element_id` and return structured connection status.

## See also

- Related: [moveanalyticalnodeusingelementtransformutils.md](moveanalyticalnodeusingelementtransformutils.md)
