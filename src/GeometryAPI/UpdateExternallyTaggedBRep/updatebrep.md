# UpdateBRep

| Field | Value |
|-------|-------|
| **Sample** | GeometryAPI/UpdateExternallyTaggedBRep |
| **Class** | `UpdateBRep` |
| **Source** | `src/GeometryAPI/UpdateExternallyTaggedBRep/UpdateBRep.cs` |
| **MCP rating** | 2/5 |

Replaces the externally tagged BRep in the `DirectShape` created by `CreateBRep` with a resized podium geometry.

## What it demonstrates

- `DirectShape.RemoveExternallyTaggedGeometry` and `HasExternalGeometry` validation
- `AddExternallyTaggedGeometry` with a new `HelperMethods.CreateExternallyTaggedPodium` solid
- Auto-running `CreateBRep` when no valid cached `DirectShape` exists

## Prerequisites

- Run `CreateBRep` first (or let this command recreate the shape); same document as the original direct shape

## User interaction

- No dialog; uses fixed resized dimensions (120×20×60 in sample units)

## MCP notes

- Update logic pairs with CreateBRep; a single parameterized MCP tool could create-or-update by external id

## See also

- Related: [createbrep](createbrep.md)
