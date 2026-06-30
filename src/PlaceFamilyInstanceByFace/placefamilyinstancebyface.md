# Command

| Field | Value |
|-------|-------|
| **Sample** | PlaceFamilyInstanceByFace |
| **Class** | `Command` |
| **Source** | `src/PlaceFamilyInstanceByFace/Command.cs` |
| **MCP rating** | 4/5 |

Launches a two-step workflow to place face-based, point-based, or line-based family instances on geometry extracted from the current selection.

## What it demonstrates

- `FamilyInstanceCreator` harvesting faces from selected elements and listing loadable `FamilySymbol` types
- `BasedTypeForm` choosing placement mode (face, point, or line)
- `PlaceFamilyInstanceForm` and `PointUserControl` for interactive placement on a chosen face
- `Document.Create.NewFamilyInstance` with face references and host elements

## Prerequisites

- Pre-selected element(s) with face geometry
- Appropriate family symbols loaded (sample expects point-based and line-based types)

## User interaction

- Two modal WinForms dialogs (`BasedTypeForm`, then `PlaceFamilyInstanceForm`)
- Headless use would need element id, face index, family symbol id, and placement point passed as parameters

## MCP notes

- Proposed tool: `revit_place_family_on_face`
- Parameters: `host_element_id`, `family_symbol_id`, `face_reference` or UV/point on face, optional `placement_type`
- Returns: new `FamilyInstance` element id
- MCP descriptor: `src/PlaceFamilyInstanceByFace/placefamilyinstancebyface.json`

## See also

- MCP descriptor: `src/PlaceFamilyInstanceByFace/placefamilyinstancebyface.json`
- Helper: `src/PlaceFamilyInstanceByFace/FamilyInstanceCreator.cs`
