# StretchAndFit

| Field | Value |
|-------|-------|
| **Sample** | FabricationPartLayout |
| **Class** | `StretchAndFit` |
| **Source** | `src/FabricationPartLayout/StretchAndFit.cs` |
| **SDK ReadMe** | `src/FabricationPartLayout/Readme_FabricationPartLayout.rtf` |
| **MCP rating** | 2/5 |

Stretches a fabrication fitting from one open connector to connect with a second selected element.

## What it demonstrates

- `FabricationPart.StretchAndFit` with `FabricationPartRouteEnd.CreateFromConnector`
- Selection rules: source must be a non-straight, non-tap, non-hanger part with one occupied and one free connector
- Handling `FabricationPartFitResult` and calling `Document.Regenerate` after a successful fit

## Prerequisites

- Exactly two pre-selected elements: a valid fabrication fitting and a target part with an unused connector

## User interaction

- Uses the current selection (no dialog); fails if fewer than two elements are selected or connector rules are not met

## MCP notes

- Stretch-and-fit logic is automatable with element ids, but validation rules and regeneration make it a specialized MEP workflow tool
