# MakeCapForm

| Field | Value |
|-------|-------|
| **Sample** | Massing/NewForm |
| **Class** | `MakeCapForm` |
| **Source** | `src/Massing/NewForm/CS/Command.cs` |
| **SDK ReadMe** | `src/Massing/NewForm/CS/ReadMe_NewForm.rtf` |
| **MCP rating** | 2/5 |

Creates a triangular cap form from three model-line profile references using `FamilyCreate.NewFormByCap`.

## What it demonstrates

- Building a closed `ReferenceArray` profile with `FormUtils.MakeLine`
- `Document.FamilyCreate.NewFormByCap(true, refAr)` in a mass family

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; fixed XYZ coordinates for profile vertices

## MCP notes

- One of five form-creation demos in the NewForm sample; not suitable as a standalone MCP tool without generalizing profile input.
