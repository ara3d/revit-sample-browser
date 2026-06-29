# Cut

| Field | Value |
|-------|-------|
| **Sample** | SolidSolidCut |
| **Class** | `Cut` |
| **Source** | `src/SolidSolidCut/Command.cs` |
| **SDK ReadMe** | `src/SolidSolidCut/ReadMe_SolidSolidCut.rtf` |
| **MCP rating** | 2/5 |

Applies a solid-solid cut so one family solid element cuts another in a family document.

## What it demonstrates

- `SolidSolidCutUtils.CanElementCutElement` eligibility check
- `SolidSolidCutUtils.AddCutBetweenSolids` inside a transaction
- Hardcoded element ids for a cube and sphere in `SolidSolidCut.rfa`

## Prerequisites

- Open `SolidSolidCut.rfa` family with the sample cube (30481) and sphere (30809) elements

## User interaction

- No picks; shows a notice if the expected elements are missing

## MCP notes

API is automatable with element ids, but the sample uses fixed ids in a demo family rather than a reusable parameterized command.
