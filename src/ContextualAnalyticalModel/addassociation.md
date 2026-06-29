# AddAssociation

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `AddAssociation` |
| **Source** | `src/ContextualAnalyticalModel/CS/AddAssociation.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/CS/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Creates a one-to-one association between a selected analytical element and a selected physical element using `AnalyticalToPhysicalAssociationManager`.

## What it demonstrates

- Obtaining the document association manager via `AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager`
- `AddAssociation(ElementId analyticalElementId, ElementId physicalElementId)` for a single pair
- Manual transaction wrapping association changes

## Prerequisites

- Active structural document with contextual analytical modeling enabled
- One analytical element and one physical element available for selection

## User interaction

- Prompts twice via `Utilities.GetSelectedObject` (analytical element, then physical element)
- Headless use would pass element IDs directly instead of interactive picks

## MCP notes

- Proposed tool: `revit_add_analytical_physical_association`
- Parameters: `analytical_element_id`, `physical_element_id`
- Returns: success flag and association confirmation
- MCP descriptor: `src/ContextualAnalyticalModel/addassociation.json`

## See also

- MCP descriptor: `src/ContextualAnalyticalModel/addassociation.json`
- Related: [addcustomassociation.md](addcustomassociation.md), [removeassociation.md](removeassociation.md)
