# CreateNegativeBlockCommand

| Field | Value |
|-------|-------|
| **Sample** | FreeFormElement |
| **Class** | `CreateNegativeBlockCommand` |
| **Source** | `src/FreeFormElement/CS/CreateNegativeBlockCommand.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates a generic-model family block that represents the negative volume of a selected solid element within a user-defined XY boundary.

## What it demonstrates

- `ISelectionFilter` implementations for solids (`TargetElementSelectionFilter`) and XY-plane curves (`BoundarySelectionFilter`)
- `FreeFormElementUtils.CreateNegativeBlock` with load options and `GenericModel.rft` template lookup
- Failure conditions: non-contiguous boundary, curves above target, or no intersection

## Prerequisites

- Target element with extractable solids; boundary curves in the XY plane forming a closed loop

## User interaction

- Two pick phases: one target element, then multiple boundary curves

## MCP notes

- Proposed tool: `revit_create_negative_block` with `target_element_id`, `boundary_curve_ids`, and optional `family_template_path`. Replace picks with element ids.

## See also

- MCP descriptor: `src/FreeFormElement/createnegativeblockcommand.json`
