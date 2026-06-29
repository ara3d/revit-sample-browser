# ToposolidFromDwg

| Field | Value |
|-------|-------|
| **Sample** | Toposolid |
| **Class** | `ToposolidFromDwg` |
| **Source** | `src/Toposolid/Command.cs` |
| **SDK ReadMe** | `src/Toposolid/ReadMe_Toposolid.rtf` |
| **MCP rating** | 2/5 |

Builds a toposolid from polyline and line geometry extracted from a picked imported DWG.

## What it demonstrates

- Walking `ImportInstance` symbol geometry (`PolyLine`, `Line`) via `GeometryInstance`
- `Toposolid.Create` from a point list with Z offset via `TOPOSOLID_HEIGHTABOVELEVEL_PARAM`
- `ImportInstanceFilter` for DWG selection

## Prerequisites

- Imported DWG (`ImportInstance`) with usable linework
- `ToposolidType` and `Level` in the project

## User interaction

- User must pick one imported DWG instance
- Headless use would need the import element id and equivalent point extraction

## MCP notes

- DWG parsing is fragile and selection-dependent; better handled by a dedicated import-and-convert workflow.

## See also

- [ToposolidCreation](toposolidcreation.md)
- [ToposolidFromSurface](toposolidfromsurface.md)
