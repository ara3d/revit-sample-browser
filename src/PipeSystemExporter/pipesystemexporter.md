# Command

| Field | Value |
|-------|-------|
| **Sample** | PipeSystemExporter |
| **Class** | `Command` |
| **Source** | `src/PipeSystemExporter/Command.cs` |
| **Origin** | [jeremytammik/PipeSystemExporter](https://github.com/jeremytammik/PipeSystemExporter) (MIT) |
| **MCP rating** | 3/5 |

Enumerates all pipes and pipe fittings in the active document and logs connector geometry to the debug output. Pipes report diameter (feet and millimetres) and two endpoint coordinates; fittings are classified as plug (one connector), elbow (two), or tee (three).

## What it demonstrates

- `FilteredElementCollector` filtered by `OST_PipeCurves` / `OST_PipeFitting`
- Reading `Pipe.Diameter` and connector origins via `MEPCurve.ConnectorManager`
- `FamilyInstance` MEP connector access through `MEPModel.ConnectorManager`
- Helper methods in `Util` for connector enumeration and formatted point output

## Prerequisites

- MEP-enabled project with pipe curves and/or pipe fittings

## User interaction

- Non-interactive; results appear in the Visual Studio / Revit debug output window only

## MCP notes

- Proposed tool: `revit_export_pipe_system`
- Headless use would require replacing `Debug.Print` with a returned JSON structure
- MCP descriptor: `src/PipeSystemExporter/pipesystemexporter.json`

## See also

- MCP descriptor: `src/PipeSystemExporter/pipesystemexporter.json`
- Related: [CableTraySample](../CableTraySample/cable-tray-elbow-fitting.md) (MEP curve fitting creation)
