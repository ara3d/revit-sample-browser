# Command

| Field | Value |
|-------|-------|
| **Sample** | CapitalizeAllTextNotes |
| **Class** | `Command` |
| **Source** | `src/CapitalizeAllTextNotes/Command.cs` |
| **SDK ReadMe** | `src/CapitalizeAllTextNotes/Readme_CapitalizeAllTextNotes.rtf` |
| **MCP rating** | 4/5 |

Sets all-caps formatting on every `TextNote` in the document that is not already fully capitalized.

## What it demonstrates

- Collecting all `TextNote` elements with `document.GetElements<TextNote>()`
- Reading and writing `FormattedText` via `GetFormattedText` / `SetFormattedText`
- Using `GetAllCapsStatus` and `SetAllCapsStatus` on the full text range

## Prerequisites

- Document containing at least one `TextNote` not already all-caps

## User interaction

- Fully automatic; no picks or dialogs
- Fails with a message if no text notes exist or none need updating

## MCP notes

- Proposed tool: `revit_capitalize_text_notes`
- Parameters: optional `view_id` to scope notes, optional `text_note_ids[]`
- Returns: count of updated text notes
- MCP descriptor: `src/CapitalizeAllTextNotes/capitalizealltextnotes.json`

## See also

- MCP descriptor: `src/CapitalizeAllTextNotes/capitalizealltextnotes.json`
