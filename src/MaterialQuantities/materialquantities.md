# Command

| Field | Value |
|-------|-------|
| **Sample** | MaterialQuantities |
| **Class** | `Command` |
| **Source** | `src/MaterialQuantities/MaterialQuantities.cs` |
| **SDK ReadMe** | `src/MaterialQuantities/ReadMe_MaterialQuantities.rtf` |
| **MCP rating** | 5/5 |

Calculates net and gross material volumes and areas for all walls, floors, and roofs, writing CSV-style totals to `CalculateMaterialQuantities.txt`.

## What it demonstrates

- `Element.GetMaterialIds`, `GetMaterialVolume`, and `GetMaterialArea` per compound element
- Gross quantities via a temporary transaction that deletes doors, windows, and openings then rolls back
- Specialized calculators (`WallMaterialQuantityCalculator`, `FloorMaterialQuantityCalculator`, `RoofMaterialQuantityCalculator`) over category filters
- Per-element and project-level aggregation with CSV output

## Prerequisites

- Project with walls, floors, and/or roofs containing material assignments

## User interaction

- No dialog; writes `CalculateMaterialQuantities.txt` in the working directory
- Returns `Result.Cancelled` because the model is not modified (gross pass uses rollback)

## MCP notes

- Proposed tool: `revit_calculate_material_quantities`
- Parameters: optional `categories` (`walls`, `floors`, `roofs`), `include_gross`, `output_path`
- Returns: nested totals per material (gross/net volume and area) plus per-element breakdown
- MCP descriptor: `src/MaterialQuantities/materialquantities.json`

## See also

- MCP descriptor: `src/MaterialQuantities/materialquantities.json`
