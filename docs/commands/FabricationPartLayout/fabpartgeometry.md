# FabPartGeometry

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `FabPartGeometry` |
| **Source** | `src/FabricationPartLayout/CS/FabPartGeometry.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/CS/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Exports coarse mesh geometry and insulation/lining meshes for selected fabrication parts to CSV triangle files.

## What it demonstrates

- `FabricationPart.get_Geometry` and `GetInsulationLiningGeometry` with `ViewDetailLevel.Coarse`
- Recursive `GeometryInstance` traversal and CSV export of triangle vertices

## Prerequisites

- Pre-selected fabrication parts

## User interaction

- `FileSaveDialog` for base filename; writes multiple `-main-` and `-ins-` CSV files

## MCP notes

- Export path and format are UI-driven; MCP would need explicit output directory parameter
