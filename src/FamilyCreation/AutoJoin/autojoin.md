# Command

| Field | Value |
|-------|-------|
| **Sample** | FamilyCreation/AutoJoin |
| **Class** | `Command` |
| **Source** | `src/FamilyCreation/AutoJoin/Command.cs` |
| **MCP rating** | 2/5 |

Joins overlapping solid generic forms in a family document, either from the current selection or automatically across the model.

## What it demonstrates

- `Document.CombineElements` on a `CombinableElementArray` of solid `GenericForm` elements
- Fallback `AutoJoin.Join` that finds and joins all overlapping forms when nothing is selected
- Sharing `Application.Create` via a static field for helper geometry routines

## Prerequisites

- Family document with at least two solid generic forms; manual mode needs two or more selected combinable elements

## User interaction

- Uses pre-selection when elements are selected; otherwise runs automatic join with no dialog

## MCP notes

- Family-editor geometry utility; automatable with element ids but limited to family/massing editing scenarios
