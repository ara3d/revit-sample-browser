# InstanceViewCreation

| Field | Value |
|-------|-------|
| **Sample** | PanelSchedule |
| **Class** | `InstanceViewCreation` |
| **Source** | `src/PanelSchedule/InstanceViewCreation.cs` |
| **MCP rating** | 4/5 |

Creates a panel schedule instance view for a user-picked electrical equipment panel.

## What it demonstrates

- `Selection.PickObject(ObjectType.Element)` to choose a panel
- `PanelScheduleView.CreateInstanceView(doc, panelElementId)` inside a named transaction
- Failure handling when the picked element is not an electrical panel

## Prerequisites

- Electrical panel family instance in the model

## User interaction

- Single element pick; no form
- Headless execution possible with `panel_element_id` instead of `PickObject`

## MCP notes

- Proposed tool: `revit_create_panel_schedule_view`
- Parameters: `panel_element_id`
- Returns: new `PanelScheduleView` element id
- MCP descriptor: `src/PanelSchedule/instanceviewcreation.json`

## See also

- MCP descriptor: `src/PanelSchedule/instanceviewcreation.json`
