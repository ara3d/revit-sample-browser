# Command

| Field | Value |
|-------|-------|
| **Sample** | Massing/ManipulateForm |
| **Class** | `Command` |
| **Source** | `src/Massing/ManipulateForm/Command.cs` |
| **MCP rating** | 2/5 |

Builds a loft `Form` from rectangular profiles, then programmatically adds profiles and edges and moves sub-elements to show massing form manipulation APIs.

## What it demonstrates

- `FamilyCreate.NewLoftForm` with `ReferenceArray` profiles built from model curves
- `Form.AddProfile`, `MoveProfile`, `get_CurveLoopReferencesOnProfile`, and `MoveSubElement`
- `Form.AddEdge`, `GetControlPoints`, and geometry traversal via `Solid.Edges`
- Repeated `Document.Regenerate` between manipulation steps

## Prerequisites

- Conceptual mass family document (uses `FamilyCreate`)

## User interaction

- Fully automatic; no picks or dialogs
- Hard-coded dimensions and offsets; not parameterized for external callers

## MCP notes

- Specialized conceptual-massing demo; low value as a general MCP tool without substantial refactoring.
