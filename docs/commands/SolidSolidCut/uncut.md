# Uncut

| Field | Value |
|-------|-------|
| **Sample** | SolidSolidCut |
| **Class** | `Uncut` |
| **Source** | `src/SolidSolidCut/CS/Command.cs` |
| **SDK ReadMe** | `src/SolidSolidCut/CS/ReadMe_SolidSolidCut.rtf` |
| **MCP rating** | 2/5 |

Removes an existing solid-solid cut relationship between two family solids.

## What it demonstrates

- `SolidSolidCutUtils.RemoveCutBetweenSolids` in a transaction
- Same hardcoded cube and sphere element ids as the `Cut` command

## Prerequisites

- Open `SolidSolidCut.rfa` with an active cut between the sample elements

## User interaction

- No picks; prompts to open the demo family if elements are absent

## MCP notes

Uncut by element id is straightforward, but this sample is a fixed-id family demo, not a general tool surface.
