# Command

| Field | Value |
|-------|-------|
| **Sample** | NetworkPressureLossReport |
| **Class** | `Command` |
| **Source** | `src/NetworkPressureLossReport/CS/Command.cs` |
| **SDK ReadMe** | `src/NetworkPressureLossReport/CS/ReadMe_NetworkPressureLossReport.rtf` |
| **MCP rating** | 5/5 |

Lists mechanical piping networks in the document and lets the user visualize pressure loss in the Analysis Visualization Framework or export a CSV report.

## What it demonstrates

- `NetworkInfo.FindValidNetworks` to discover connected piping networks
- WPF `NetworkDialog` for network selection, AVF display (`AvfViewer`), and CSV export
- Transaction-wrapped AVF spatial field display on the active view

## Prerequisites

- MEP project with analyzable piping networks

## User interaction

- Modal WPF dialog with list selection, View (AVF), and Report (save CSV) actions
- `NetworkInfo` and `AvfViewer` classes contain export/visualization logic extractable from UI

## MCP notes

- Proposed tools: `revit_list_piping_networks` and `revit_export_pressure_loss_report`
- Parameters: optional `network_ids[]`, `output_path`, `itemized` flag for AVF
- Returns: network metadata, segment pressure data, or path to exported CSV
- MCP descriptor: `src/NetworkPressureLossReport/networkpressurelossreport.json`

## See also

- MCP descriptor: `src/NetworkPressureLossReport/networkpressurelossreport.json`
