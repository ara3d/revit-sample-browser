# CommandReadPreferences

| Field | Value |
|-------|-------|
| **Sample** | RoutingPreferenceTools |
| **Class** | `CommandReadPreferences` |
| **Source** | `src/RoutingPreferenceTools/CS/RoutingPreferenceBuilder/CommandReadPreferences.cs` |
| **MCP rating** | 5/5 |

Imports pipe segments, schedules, fittings, and routing preference rules from a RoutingPreference Builder XML file into the active document.

## What it demonstrates

- Open-file dialog loading validated XML (`SchemaValidationHelper.ValidateRoutingPreferenceBuilderXml`)
- `RoutingPreferenceBuilder.ParseAllPipingPoliciesFromXml` creating pipe types, segments, sizes, and rules
- MEP and pipe-type validation before import

## Prerequisites

- MEP document with at least one existing pipe type
- Valid RoutingPreference Builder XML conforming to the sample XSD

## User interaction

- File open dialog to choose XML; success shown in `TaskDialog`
- MCP would accept `file_path` directly

## MCP notes

- Proposed tool: `revit_import_routing_preferences`
- Parameters: `xml_file_path`
- Returns: summary of created or updated pipe types and routing rules
- MCP descriptor: `src/RoutingPreferenceTools/commandreadpreferences.json`

## See also

- MCP descriptor: `src/RoutingPreferenceTools/commandreadpreferences.json`
- Export counterpart: [commandwritepreferences.md](commandwritepreferences.md)
