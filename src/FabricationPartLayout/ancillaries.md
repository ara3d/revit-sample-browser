# Ancillaries

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `Ancillaries` |
| **Source** | `src/FabricationPartLayout/Ancillaries.cs` |
| **MCP rating** | 2/5 |

Reports ancillary usage on a user-picked fabrication part, grouped and counted by type and name.

## What it demonstrates

- `FabricationPart.GetPartAncillaryUsage` and `FabricationConfiguration.GetAncillaryName`
- Resolving ancillary type and usage type for display

## Prerequisites

- Loaded fabrication configuration in the document

## User interaction

- Single element pick; results shown in `TaskDialog`

## MCP notes

- Read-only inspection tied to interactive pick; could become a query tool with an element id parameter
