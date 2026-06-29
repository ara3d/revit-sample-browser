# AddCustomAssociation

| Field | Value |
|-------|-------|
| **Sample** | ContextualAnalyticalModel |
| **Class** | `AddCustomAssociation` |
| **Source** | `src/ContextualAnalyticalModel/CS/AddCustomAssociation.cs` |
| **SDK ReadMe** | `src/ContextualAnalyticalModel/CS/ReadMe_ContextualAnalyticalModel.rtf` |
| **MCP rating** | 4/5 |

Associates multiple analytical elements with multiple physical elements in one operation through `AnalyticalToPhysicalAssociationManager`.

## What it demonstrates

- Multi-select via `Utilities.GetSelectedObjects` returning `ISet<ElementId>`
- `AddAssociation(ISet<ElementId> analyticalElementIds, ISet<ElementId> physicalElementIds)` overload
- Batch relation creation in a single transaction

## Prerequisites

- Active structural document with analytical and physical elements to link
- Elements must be valid association targets for the manager API

## User interaction

- Two multi-pick prompts: analytical elements first, then physical elements
- Replace picks with explicit ID arrays for automation

## MCP notes

- Proposed tool: `revit_add_analytical_physical_associations`
- Parameters: `analytical_element_ids[]`, `physical_element_ids[]`
- MCP descriptor: `docs/mcp/ContextualAnalyticalModel/addcustomassociation.json`

## See also

- MCP descriptor: `docs/mcp/ContextualAnalyticalModel/addcustomassociation.json`
- Related: [addassociation.md](addassociation.md), [removeassociation.md](removeassociation.md)
