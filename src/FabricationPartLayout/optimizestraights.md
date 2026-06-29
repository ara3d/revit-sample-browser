# OptimizeStraights

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `OptimizeStraights` |
| **Source** | `src/FabricationPartLayout/OptimizeStraights.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Optimizes straight fabrication part lengths within the current selection using the built-in length optimizer.

## What it demonstrates

- `FabricationPart.OptimizeLengths` returning affected part ids
- Transaction-wrapped regenerate after optimization

## Prerequisites

- Selection containing optimizable straight fabrication parts

## User interaction

- Uses current selection; cancels if no straights were optimized

## MCP notes

- Could accept `element_ids[]` for MCP; optimizer behavior is Revit-internal
