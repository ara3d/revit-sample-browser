# Command

| Field | Value |
|-------|-------|
| **Sample** | Events / PrintLog |
| **Class** | `Command` |
| **Source** | `src/Events/PrintLog/Command.cs` |
| **SDK ReadMe** | `src/Events/PrintLog/ReadMe_PrintLog.rtf` |
| **MCP rating** | 1/5 |

Prints all printable views in the active document to a file, triggering `ViewPrint` and `DocumentPrint` events handled by the sample's external application.

## What it demonstrates

- Collecting printable views (non-template, `CanBePrinted`) via `FilteredElementCollector`
- Configuring `PrintManager` for file output and calling `Document.Print`
- Integration with `EventsReactor` logging (log files written beside the add-in assembly)

## User interaction

- Runs immediately with no dialog; output path is fixed relative to the assembly folder

## MCP notes

- Exists to exercise print event handlers; printing is environment-specific and not a useful MCP primitive here
