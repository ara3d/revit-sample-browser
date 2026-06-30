# MakeSweptBlendForm

| Field | Value |
|-------|-------|
| **Sample** | Massing/NewForm |
| **Class** | `MakeSweptBlendForm` |
| **Source** | `src/Massing/NewForm/Command.cs` |
| **MCP rating** | 2/5 |

Creates a swept blend form from two triangular profiles along a vertical path curve.

## What it demonstrates

- `FamilyCreate.NewSweptBlendForm(true, path, profiles)` with `ReferenceArrayArray` for start/end profiles
- Separate path `ReferenceArray` built from a single vertical model line
- `FormUtils.MakeLine` for profile and path construction

## Prerequisites

- Conceptual mass family document

## User interaction

- No UI; all geometry is hard-coded

## MCP notes

- Illustrates swept-blend API; not a general MCP candidate without external profile/path definitions.
