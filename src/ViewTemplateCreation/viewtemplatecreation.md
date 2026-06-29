# Command

| Field | Value |
|-------|-------|
| **Sample** | ViewTemplateCreation |
| **Class** | `Command` |
| **Source** | `src/ViewTemplateCreation/Command.cs` |
| **SDK ReadMe** | `src/ViewTemplateCreation/Readme_ViewTemplateCreation.rtf` |
| **MCP rating** | 4/5 |

Creates a view template from a selected view and configures parts visibility, detail level, and category graphic overrides.

## What it demonstrates

- `View.CreateViewTemplate` from a source view where `IsViewValidForTemplateCreation()` is true
- `SetNonControlledTemplateParameterIds` to exclude parts visibility from template control
- `View.DetailLevel` assignment and `SetCategoryOverrides` with cut fill patterns
- Assigning `ViewTemplateId` back to the source view

## Prerequisites

- At least one non-template view eligible for template creation

## User interaction

- `ViewTemplateCreationForm` modal dialog for view, parts visibility, detail level, and apply
- Headless use needs source view id and template settings as parameters

## MCP notes

- Proposed tool: `revit_create_view_template`
- Parameters: `source_view_id`, `parts_visibility_controlled`, `detail_level`, optional category override map
- Returns: new view template element id
- MCP descriptor: `src/ViewTemplateCreation/viewtemplatecreation.json`

## See also

- MCP descriptor: `src/ViewTemplateCreation/viewtemplatecreation.json`
