# CreateBRep

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/UpdateExternallyTaggedBRep |
| **Class** | `CreateBRep` |
| **Source** | `src/GeometryAPI/UpdateExternallyTaggedBRep/CS/CreateBRep.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates an `ExternallyTaggedBRep` (“Podium”), adds it to a new `DirectShape`, and verifies retrieval by external geometry ids.

## What it demonstrates

- `HelperMethods.ExecuteCreateBRepCommand` building tagged BRep geometry
- `DirectShape.AddExternallyTaggedGeometry` and lookup by `ExternalGeometryId`
- Static `CreatedDirectShape` reference shared with the update command

## Prerequisites

- Project document

## User interaction

- No picks; fixed podium dimensions from helper methods

## MCP notes

- Proposed tool: `revit_create_externally_tagged_brep` with dimension parameters and external id strings. Returns direct shape element id.

## See also

- MCP descriptor: `src/GeometryAPI/UpdateExternallyTaggedBRep/createbrep.json`
- Related: [updatebrep](updatebrep.md)
