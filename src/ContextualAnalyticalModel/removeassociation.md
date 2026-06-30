# RemoveAssociation

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `RemoveAssociation` |
| **Source** | `src/ContextualAnalyticalModel/RemoveAssociation.cs` |
| **MCP rating** | 2/5 |

Removes the analytical–physical association for a user-selected element via `AnalyticalToPhysicalAssociationManager`.

## What it demonstrates

- `AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc)`
- `RemoveAssociation(ElementId)` for either side of an existing association
- Single element pick with `Selection.PickObject`

## Prerequisites

- Element with an existing analytical–physical association to remove

## User interaction

- One pick prompt: "select the element for which you want to break relation"
- Headless variant would accept `element_id` directly

## MCP notes

- `RemoveAssociation` by element ID is simple to automate, but rating stays 2 because the sample is pick-only with no confirmation or return payload; pair with add-association tools in a future MCP layer.

## See also

- Related: [addassociation.md](addassociation.md), [addcustomassociation.md](addcustomassociation.md)
