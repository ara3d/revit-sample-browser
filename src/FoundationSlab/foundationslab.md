# Command

| Field | Value |
|-------|-------|
| **Sample** | FoundationSlab |
| **Class** | `Command` |
| **Source** | `src/FoundationSlab/Command.cs` |
| **SDK ReadMe** | — |
| **MCP rating** | 4/5 |

Creates foundation slabs from user-specified boundary geometry, levels, and slab types through a configuration dialog.

## What it demonstrates

- `SlabData` gathering document context for a WinForms UI
- `FoundationSlabForm` collecting slab creation inputs and driving slab placement on OK
- Transaction-wrapped foundation slab creation delegated to the form/data layer

## Prerequisites

- Project with levels, floor/slab types, and boundary geometry selectable in the UI

## User interaction

- Modal `FoundationSlabForm`; cancel returns `Result.Cancelled`

## MCP notes

- Proposed tool: `revit_create_foundation_slab` with `boundary_curve_ids`, `level_id`, `slab_type_id`, and structural flag. Extract creation logic from the form handlers.

## See also

- MCP descriptor: `src/FoundationSlab/foundationslab.json`
