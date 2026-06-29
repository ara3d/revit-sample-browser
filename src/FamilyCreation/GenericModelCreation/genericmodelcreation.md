# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/GenericModelCreation |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/GenericModelCreation/CS/Command.cs` |
| **SDK ReadMe** | `src/FamilyCreation/GenericModelCreation/CS/ReadMe_GenericModelCreation.rtf` |
| **MCP rating** | 2/5 |

Creates a generic model family (or uses the active family) and adds sample extrusion, blend, revolution, sweep, and swept-blend forms.

## What it demonstrates

- `Application.NewFamilyDocument` when the active document is not a family
- `FamilyItemFactory` methods: `NewExtrusion`, `NewBlend`, `NewRevolution`, `NewSweep`, `NewSweptBlend`
- `SketchPlane.Create`, profile construction, and `ElementTransformUtils.MoveElement` for layout

## Prerequisites

- Generic Model template available if no family document is open

## User interaction

- No dialog; runs all five form-creation routines in one transaction

## MCP notes

- Geometry API walkthrough with fixed coordinates; not suitable as a general MCP geometry tool without parameterization
