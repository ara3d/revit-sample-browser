# Command

| Field | Value |
|-------|-------|
| **Sample** | CreateDuctworkStiffener |
| **Class** | `Command` |
| **Source** | `src/CreateDuctworkStiffener/CS/Command.cs` |
| **SDK ReadMe** | `src/CreateDuctworkStiffener/CS/ReadMe_CreateDuctworkStiffener.rtf` |
| **MCP rating** | 3/5 |

Places a fabrication duct stiffener on the first fabrication duct in the project at the midpoint of its centerline.

## What it demonstrates

- `FilteredElementCollector` for `FabricationPart` ductwork and stiffener `FamilySymbol` types
- `MEPSupportUtils.CreateDuctworkStiffener(document, stiffenerTypeId, ductId, distanceFromStart)`
- Family symbol activation and document regeneration before placement

## Prerequisites

- Project document (not family) with fabrication ductwork and a stiffener family loaded
- Stiffener type named `Duct Stiffener - External Rectangular Angle Iron: L Angle` (hard-coded lookup)

## User interaction

- None — picks first duct and fixed stiffener type automatically

## MCP notes

- Proposed tool: `revit_create_ductwork_stiffener`
- Parameters: `duct_id`, `stiffener_type_id`, `distance_from_start`
- Returns: stiffener element id
- MCP descriptor: `src/CreateDuctworkStiffener/createductworkstiffener.json`

## See also

- MCP descriptor: `src/CreateDuctworkStiffener/createductworkstiffener.json`
