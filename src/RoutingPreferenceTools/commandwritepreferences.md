# CommandWritePreferences

| Field | Value |
|-------|-------|
| **Sample** | RoutingPreferenceTools |
| **Class** | `CommandWritePreferences` |
| **Source** | `src/RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs` |
| **MCP rating** | 2/5 |

Exports all piping routing preference policies from the active document to a RoutingPreference Builder XML file.

## What it demonstrates

- `RoutingPreferenceBuilder.CreateXmlFromAllPipingPolicies` serializing pipe types, segments, and rules
- Save-file dialog with default name derived from document path
- Warning when `.rfa` paths referenced in XML cannot be resolved

## Prerequisites

- MEP document with pipe types and routing preferences defined

## User interaction

- Save-file dialog to choose output path

## MCP notes

Useful export workflow but file-dialog bound; MCP would need explicit `output_path` parameter. Lower priority than import for automation agents.

## See also

- Import counterpart: [commandreadpreferences.md](commandreadpreferences.md)
