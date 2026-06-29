# Command Documentation & MCP Exposure Tracker

> Master checklist for command markdown docs and MCP tool descriptor JSON. **Agent instructions:** [`docs/AGENTS.md`](docs/AGENTS.md)

## Progress Summary

| Metric | Value |
|--------|-------|
| Total commands | 283 |
| Command docs created | 283 / 283 |
| MCP descriptors created | 125 / 125 |
| Last updated | 2026-06-28 |

### MCP rating distribution (preliminary)

- **5/5**: 33 commands — MCP JSON required
- **4/5**: 85 commands — MCP JSON required
- **3/5**: 7 commands — MCP JSON required
- **2/5**: 127 commands
- **1/5**: 31 commands

## Goals

1. Create one **concise** markdown file per `IExternalCommand` (see [`docs/AGENTS.md`](docs/AGENTS.md)).
2. For commands with **MCP rating ≥ 3**, create an MCP tool descriptor JSON under `docs/mcp/`.
3. Use each sample's **SDK ReadMe RTF** (`ReadMe_*.rtf` / `Readme_*.rtf` in the sample `CS` folder) as the primary reference.
4. Refine MCP ratings when writing docs; update both the doc and descriptor if the score changes.

## Per-command tasks

| Task | Output | Required |
|------|--------|----------|
| **A — Command doc** | `docs/commands/<Sample>/<slug>.md` | Always |
| **B — MCP descriptor** | `docs/mcp/<Sample>/<slug>.json` | MCP rating ≥ 3 only |

Templates: [`docs/commands/_template.md`](docs/commands/_template.md) · [`docs/mcp/_template.json`](docs/mcp/_template.json)

## Doc conventions

- **Length:** 150–300 words in the body; short paragraphs and bullets.
- **RTF:** consult `src/<Sample>/CS/*.rtf` (or nested `src/<Sample>/<Sub>/CS/*.rtf`); strip RTF markup, do not paste raw RTF.
- **Slug:** kebab-case class name; for `Command`, use leaf subfolder name when nested.
- **Status:** `[ ]` not started · `[~]` in progress · `[x]` done · `—` not applicable

## MCP usefulness scale

| Score | Meaning | MCP JSON? |
|-------|---------|-----------|
| **5** | Excellent — read/query/export, minimal UI, machine-friendly I/O | Yes |
| **4** | Good — useful mutations or workflows that can be parameterized | Yes |
| **3** | Moderate — niche value or needs wrapping | Yes |
| **2** | Low — tutorial/demo, heavy UI, very specialized | No |
| **1** | Poor — infrastructure, UI plumbing, not meaningful headless | No |

Preliminary scores below — confirm or revise when writing each doc.

## Work sessions

| Session | Scope | Doc | MCP JSON | Status |
|---------|-------|-----|----------|--------|
| 1 | `todo.md`, templates, agent runbook | — | — | done |
| 2 | First 20 commands (inventory order, AddSpaceAndZone–CompoundStructure) | done | done | done |
| 3 | MCP 5-rated commands remaining (~30) | pending | pending | pending |
| 4 | MCP 4-rated commands remaining (~75) | pending | pending | pending |
| 5 | MCP 3-rated commands (~7) | pending | pending | pending |
| 6 | MCP 2- and 1-rated commands remaining (~148, doc only) | pending | — | pending |
| 7 | Review ratings; reconcile edge cases | pending | pending | pending |
| 8 | Parallel batch (items 1–200 pending, 5 agents) | done | done | done |
| 9 | Final 43 commands (SelectionDialog–WinderStairs) | done | done | done |

## Command inventory

Columns: **Doc** = Task A · **MCP** = Task B (`—` when rating is 1 or 2)

### AddSpaceAndZone (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AddSpaceAndZone/addspaceandzone.md](docs/commands/AddSpaceAndZone/addspaceandzone.md) | [AddSpaceAndZone/addspaceandzone.json](docs/mcp/AddSpaceAndZone/addspaceandzone.json) | `AddSpaceAndZone/CS/Command.cs` | 4 |
### AllViews (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AllViews/allviews.md](docs/commands/AllViews/allviews.md) | [AllViews/allviews.json](docs/mcp/AllViews/allviews.json) | `AllViews/CS/AllViews.cs` | 5 |
### AnalysisVisualizationFramework (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `MultithreadedCalculation` | [AnalysisVisualizationFramework/MultithreadedCalculation/multithreadedcalculation.md](docs/commands/AnalysisVisualizationFramework/MultithreadedCalculation/multithreadedcalculation.md) | — | `AnalysisVisualizationFramework/MultithreadedCalculation/CS/MultithreadedCalculation.cs` | 2 |
| [x] | — | `SpatialFieldGradient` | [AnalysisVisualizationFramework/SpatialFieldGradient/spatialfieldgradient.md](docs/commands/AnalysisVisualizationFramework/SpatialFieldGradient/spatialfieldgradient.md) | — | `AnalysisVisualizationFramework/SpatialFieldGradient/CS/Command.cs` | 2 |
### AnalyticalSupportData_Info (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AnalyticalSupportData_Info/analyticalsupportdata-info.md](docs/commands/AnalyticalSupportData_Info/analyticalsupportdata-info.md) | [AnalyticalSupportData_Info/analyticalsupportdata-info.json](docs/mcp/AnalyticalSupportData_Info/analyticalsupportdata-info.json) | `AnalyticalSupportData_Info/CS/AnalyticalSupportData_Info.cs` | 5 |
### AppearanceAssetEditing (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AppearanceAssetEditing/appearanceassetediting.md](docs/commands/AppearanceAssetEditing/appearanceassetediting.md) | — | `AppearanceAssetEditing/CS/Command.cs` | 2 |
### AreaReinCurve (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AreaReinCurve/areareincurve.md](docs/commands/AreaReinCurve/areareincurve.md) | — | `AreaReinCurve/CS/AreaReinCurve.cs` | 2 |
### AreaReinParameters (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AreaReinParameters/areareinparameters.md](docs/commands/AreaReinParameters/areareinparameters.md) | — | `AreaReinParameters/CS/AreaReinParameters.cs` | 2 |
| [x] | — | `RebarParas` | [AreaReinParameters/rebarparas.md](docs/commands/AreaReinParameters/rebarparas.md) | — | `AreaReinParameters/CS/AreaReinParameters.cs` | 2 |
### AttachedDetailGroup (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AttachedDetailGroupHideAllCommand` | [AttachedDetailGroup/attacheddetailgrouphideallcommand.md](docs/commands/AttachedDetailGroup/attacheddetailgrouphideallcommand.md) | [AttachedDetailGroup/attacheddetailgrouphideallcommand.json](docs/mcp/AttachedDetailGroup/attacheddetailgrouphideallcommand.json) | `AttachedDetailGroup/CS/AttachedDetailGroupHideAllCommand.cs` | 4 |
| [x] | [x] | `AttachedDetailGroupShowAllCommand` | [AttachedDetailGroup/attacheddetailgroupshowallcommand.md](docs/commands/AttachedDetailGroup/attacheddetailgroupshowallcommand.md) | [AttachedDetailGroup/attacheddetailgroupshowallcommand.json](docs/mcp/AttachedDetailGroup/attacheddetailgroupshowallcommand.json) | `AttachedDetailGroup/CS/AttachedDetailGroupShowAllCommand.cs` | 4 |
### AutoRoute (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AutoRoute/autoroute.md](docs/commands/AutoRoute/autoroute.md) | — | `AutoRoute/CS/Command.cs` | 2 |
### AutoTagRooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AutoTagRooms/autotagrooms.md](docs/commands/AutoTagRooms/autotagrooms.md) | [AutoTagRooms/autotagrooms.json](docs/mcp/AutoTagRooms/autotagrooms.json) | `AutoTagRooms/CS/Command.cs` | 4 |
### AvoidObstruction (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AvoidObstruction/avoidobstruction.md](docs/commands/AvoidObstruction/avoidobstruction.md) | — | `AvoidObstruction/CS/Command.cs` | 2 |
### BeamAndSlabNewParameter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [BeamAndSlabNewParameter/beamandslabnewparameter.md](docs/commands/BeamAndSlabNewParameter/beamandslabnewparameter.md) | — | `BeamAndSlabNewParameter/CS/BeamAndSlabNewParameter.cs` | 2 |
### BoundaryConditions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [BoundaryConditions/boundaryconditions.md](docs/commands/BoundaryConditions/boundaryconditions.md) | [BoundaryConditions/boundaryconditions.json](docs/mcp/BoundaryConditions/boundaryconditions.json) | `BoundaryConditions/CS/Command.cs` | 4 |
### CapitalizeAllTextNotes (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CapitalizeAllTextNotes/capitalizealltextnotes.md](docs/commands/CapitalizeAllTextNotes/capitalizealltextnotes.md) | [CapitalizeAllTextNotes/capitalizealltextnotes.json](docs/mcp/CapitalizeAllTextNotes/capitalizealltextnotes.json) | `CapitalizeAllTextNotes/CS/Command.cs` | 4 |
### CloudAPISample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `RunSampleCommand` | [CloudAPISample/runsamplecommand.md](docs/commands/CloudAPISample/runsamplecommand.md) | — | `CloudAPISample/CS/Application.cs` | 1 |
### ColorFill (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ColorFill/colorfill.md](docs/commands/ColorFill/colorfill.md) | [ColorFill/colorfill.json](docs/mcp/ColorFill/colorfill.json) | `ColorFill/CS/Command.cs` | 4 |
### CompoundStructure (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `WallCompoundStructure` | [CompoundStructure/wallcompoundstructure.md](docs/commands/CompoundStructure/wallcompoundstructure.md) | [CompoundStructure/wallcompoundstructure.json](docs/mcp/CompoundStructure/wallcompoundstructure.json) | `CompoundStructure/CS/Command.cs` | 4 |
### ContextualAnalyticalModel (21)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddAssociation` | [ContextualAnalyticalModel/addassociation.md](docs/commands/ContextualAnalyticalModel/addassociation.md) | [ContextualAnalyticalModel/addassociation.json](docs/mcp/ContextualAnalyticalModel/addassociation.json) | `ContextualAnalyticalModel/CS/AddAssociation.cs` | 4 |
| [x] | [x] | `AddCustomAssociation` | [ContextualAnalyticalModel/addcustomassociation.md](docs/commands/ContextualAnalyticalModel/addcustomassociation.md) | [ContextualAnalyticalModel/addcustomassociation.json](docs/mcp/ContextualAnalyticalModel/addcustomassociation.json) | `ContextualAnalyticalModel/CS/AddCustomAssociation.cs` | 4 |
| [x] | — | `AnalyticalNodeConnStatus` | [ContextualAnalyticalModel/analyticalnodeconnstatus.md](docs/commands/ContextualAnalyticalModel/analyticalnodeconnstatus.md) | — | `ContextualAnalyticalModel/CS/AnalyticalNodeConnStatus.cs` | 2 |
| [x] | [x] | `CreateAnalyticalCurvedPanel` | [ContextualAnalyticalModel/createanalyticalcurvedpanel.md](docs/commands/ContextualAnalyticalModel/createanalyticalcurvedpanel.md) | [ContextualAnalyticalModel/createanalyticalcurvedpanel.json](docs/mcp/ContextualAnalyticalModel/createanalyticalcurvedpanel.json) | `ContextualAnalyticalModel/CS/CreateAnalyticalCurvedPanel.cs` | 4 |
| [x] | [x] | `CreateAnalyticalMember` | [ContextualAnalyticalModel/createanalyticalmember.md](docs/commands/ContextualAnalyticalModel/createanalyticalmember.md) | [ContextualAnalyticalModel/createanalyticalmember.json](docs/mcp/ContextualAnalyticalModel/createanalyticalmember.json) | `ContextualAnalyticalModel/CS/CreateAnalyticalMember.cs` | 4 |
| [x] | [x] | `CreateAnalyticalPanel` | [ContextualAnalyticalModel/createanalyticalpanel.md](docs/commands/ContextualAnalyticalModel/createanalyticalpanel.md) | [ContextualAnalyticalModel/createanalyticalpanel.json](docs/mcp/ContextualAnalyticalModel/createanalyticalpanel.json) | `ContextualAnalyticalModel/CS/CreateAnalytcalPanel.cs` | 4 |
| [x] | [x] | `CreateAreaLoadWithRefPoint` | [ContextualAnalyticalModel/createarealoadwithrefpoint.md](docs/commands/ContextualAnalyticalModel/createarealoadwithrefpoint.md) | [ContextualAnalyticalModel/createarealoadwithrefpoint.json](docs/mcp/ContextualAnalyticalModel/createarealoadwithrefpoint.json) | `ContextualAnalyticalModel/CS/CreateAreaLoadWithRefPoint.cs` | 4 |
| [x] | [x] | `CreateCustomAreaLoad` | [ContextualAnalyticalModel/createcustomareaload.md](docs/commands/ContextualAnalyticalModel/createcustomareaload.md) | [ContextualAnalyticalModel/createcustomareaload.json](docs/mcp/ContextualAnalyticalModel/createcustomareaload.json) | `ContextualAnalyticalModel/CS/CustomAreaLoad.cs` | 4 |
| [x] | [x] | `CreateCustomLineLoad` | [ContextualAnalyticalModel/createcustomlineload.md](docs/commands/ContextualAnalyticalModel/createcustomlineload.md) | [ContextualAnalyticalModel/createcustomlineload.json](docs/mcp/ContextualAnalyticalModel/createcustomlineload.json) | `ContextualAnalyticalModel/CS/CustomLineLoad.cs` | 4 |
| [x] | [x] | `CreateCustomPointLoad` | [ContextualAnalyticalModel/createcustompointload.md](docs/commands/ContextualAnalyticalModel/createcustompointload.md) | [ContextualAnalyticalModel/createcustompointload.json](docs/mcp/ContextualAnalyticalModel/createcustompointload.json) | `ContextualAnalyticalModel/CS/CustomPointLoad.cs` | 4 |
| [x] | — | `FlipAnalyticalMember` | [ContextualAnalyticalModel/flipanalyticalmember.md](docs/commands/ContextualAnalyticalModel/flipanalyticalmember.md) | — | `ContextualAnalyticalModel/CS/FlipAnalyticalMember.cs` | 2 |
| [x] | — | `MemberForcesAnalyticalMember` | [ContextualAnalyticalModel/memberforcesanalyticalmember.md](docs/commands/ContextualAnalyticalModel/memberforcesanalyticalmember.md) | — | `ContextualAnalyticalModel/CS/MemberForcesAnalyticalMember.cs` | 2 |
| [x] | — | `ModifyPanelContour` | [ContextualAnalyticalModel/modifypanelcontour.md](docs/commands/ContextualAnalyticalModel/modifypanelcontour.md) | — | `ContextualAnalyticalModel/CS/ModifyPanelContour.cs` | 2 |
| [x] | — | `MoveAnalyticalMemberUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalmemberusingelementtransformutils.md](docs/commands/ContextualAnalyticalModel/moveanalyticalmemberusingelementtransformutils.md) | — | `ContextualAnalyticalModel/CS/MoveAnalyticalMemberUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalMemberUsingSetCurve` | [ContextualAnalyticalModel/moveanalyticalmemberusingsetcurve.md](docs/commands/ContextualAnalyticalModel/moveanalyticalmemberusingsetcurve.md) | — | `ContextualAnalyticalModel/CS/MoveAnalyticalMemberUsingSetCurve.cs` | 2 |
| [x] | — | `MoveAnalyticalNodeUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalnodeusingelementtransformutils.md](docs/commands/ContextualAnalyticalModel/moveanalyticalnodeusingelementtransformutils.md) | — | `ContextualAnalyticalModel/CS/MoveAnalyticalNodeUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalPanelUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalpanelusingelementtransformutils.md](docs/commands/ContextualAnalyticalModel/moveanalyticalpanelusingelementtransformutils.md) | — | `ContextualAnalyticalModel/CS/MoveAnalyticalPanelUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalPanelUsingSketchEditScope` | [ContextualAnalyticalModel/moveanalyticalpanelusingsketcheditscope.md](docs/commands/ContextualAnalyticalModel/moveanalyticalpanelusingsketcheditscope.md) | — | `ContextualAnalyticalModel/CS/MoveAnalyticalPanelUsingSketchEditScope.cs` | 2 |
| [x] | — | `ReleaseConditionsAnalyticalMember` | [ContextualAnalyticalModel/releaseconditionsanalyticalmember.md](docs/commands/ContextualAnalyticalModel/releaseconditionsanalyticalmember.md) | — | `ContextualAnalyticalModel/CS/ReleaseConditionsAnalyticalMember.cs` | 2 |
| [x] | — | `RemoveAssociation` | [ContextualAnalyticalModel/removeassociation.md](docs/commands/ContextualAnalyticalModel/removeassociation.md) | — | `ContextualAnalyticalModel/CS/RemoveAssociation.cs` | 2 |
| [x] | — | `SetOuterContourForPanels` | [ContextualAnalyticalModel/setoutercontourforpanels.md](docs/commands/ContextualAnalyticalModel/setoutercontourforpanels.md) | — | `ContextualAnalyticalModel/CS/SetOuterContourForPanels.cs` | 2 |
### CreateBeamsColumnsBraces (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateBeamsColumnsBraces/createbeamscolumnsbraces.md](docs/commands/CreateBeamsColumnsBraces/createbeamscolumnsbraces.md) | [CreateBeamsColumnsBraces/createbeamscolumnsbraces.json](docs/mcp/CreateBeamsColumnsBraces/createbeamscolumnsbraces.json) | `CreateBeamsColumnsBraces/CS/CreateBeamsColumnsBraces.cs` | 4 |
### CreateBeamSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateBeamSystem/createbeamsystem.md](docs/commands/CreateBeamSystem/createbeamsystem.md) | [CreateBeamSystem/createbeamsystem.json](docs/mcp/CreateBeamSystem/createbeamsystem.json) | `CreateBeamSystem/CS/Command.cs` | 4 |
### CreateComplexAreaRein (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [CreateComplexAreaRein/createcomplexarearein.md](docs/commands/CreateComplexAreaRein/createcomplexarearein.md) | — | `CreateComplexAreaRein/CS/CreateComplexAreaRein.cs` | 2 |
### CreateDimensions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateDimensions/createdimensions.md](docs/commands/CreateDimensions/createdimensions.md) | [CreateDimensions/createdimensions.json](docs/mcp/CreateDimensions/createdimensions.json) | `CreateDimensions/CS/Command.cs` | 4 |
### CreateDuctworkStiffener (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateDuctworkStiffener/createductworkstiffener.md](docs/commands/CreateDuctworkStiffener/createductworkstiffener.md) | [CreateDuctworkStiffener/createductworkstiffener.json](docs/mcp/CreateDuctworkStiffener/createductworkstiffener.json) | `CreateDuctworkStiffener/CS/Command.cs` | 3 |
### CreateFillPattern (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateFillPattern/createfillpattern.md](docs/commands/CreateFillPattern/createfillpattern.md) | [CreateFillPattern/createfillpattern.json](docs/mcp/CreateFillPattern/createfillpattern.json) | `CreateFillPattern/CS/Command.cs` | 4 |
### CreateSimpleAreaRein (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [CreateSimpleAreaRein/createsimplearearein.md](docs/commands/CreateSimpleAreaRein/createsimplearearein.md) | — | `CreateSimpleAreaRein/CS/CreateSimpleAreaRein.cs` | 2 |
### CreateTrianglesTopography (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateTrianglesTopography/createtrianglestopography.md](docs/commands/CreateTrianglesTopography/createtrianglestopography.md) | [CreateTrianglesTopography/createtrianglestopography.json](docs/mcp/CreateTrianglesTopography/createtrianglestopography.json) | `CreateTrianglesTopography/CS/Command.cs` | 3 |
### CreateViewSection (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateViewSection/createviewsection.md](docs/commands/CreateViewSection/createviewsection.md) | [CreateViewSection/createviewsection.json](docs/mcp/CreateViewSection/createviewsection.json) | `CreateViewSection/CS/Command.cs` | 3 |
| [x] | [x] | `CreateDraftingView` | [CreateViewSection/createdraftingview.md](docs/commands/CreateViewSection/createdraftingview.md) | [CreateViewSection/createdraftingview.json](docs/mcp/CreateViewSection/createdraftingview.json) | `CreateViewSection/CS/Command.cs` | 4 |
### CreateWallinBeamProfile (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateWallinBeamProfile` | [CreateWallinBeamProfile/createwallinbeamprofile.md](docs/commands/CreateWallinBeamProfile/createwallinbeamprofile.md) | [CreateWallinBeamProfile/createwallinbeamprofile.json](docs/mcp/CreateWallinBeamProfile/createwallinbeamprofile.json) | `CreateWallinBeamProfile/CS/CreateWallinBeamProfile.cs` | 4 |
### CreateWallsUnderBeams (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateWallsUnderBeams` | [CreateWallsUnderBeams/createwallsunderbeams.md](docs/commands/CreateWallsUnderBeams/createwallsunderbeams.md) | [CreateWallsUnderBeams/createwallsunderbeams.json](docs/mcp/CreateWallsUnderBeams/createwallsunderbeams.json) | `CreateWallsUnderBeams/CS/CreateWallsUnderBeams.cs` | 4 |
### CurtainSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurtainSystem/curtainsystem.md](docs/commands/CurtainSystem/curtainsystem.md) | [CurtainSystem/curtainsystem.json](docs/mcp/CurtainSystem/curtainsystem.json) | `CurtainSystem/CS/Command.cs` | 4 |
### CurtainWallGrid (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurtainWallGrid/curtainwallgrid.md](docs/commands/CurtainWallGrid/curtainwallgrid.md) | [CurtainWallGrid/curtainwallgrid.json](docs/mcp/CurtainWallGrid/curtainwallgrid.json) | `CurtainWallGrid/CS/Command.cs` | 4 |
### CurvedBeam (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurvedBeam/curvedbeam.md](docs/commands/CurvedBeam/curvedbeam.md) | [CurvedBeam/curvedbeam.json](docs/mcp/CurvedBeam/curvedbeam.json) | `CurvedBeam/CS/CurvedBeam.cs` | 4 |
### CustomExporter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CustomExporter/Custom2DExporter/custom2dexporter.md](docs/commands/CustomExporter/Custom2DExporter/custom2dexporter.md) | [CustomExporter/Custom2DExporter/custom2dexporter.json](docs/mcp/CustomExporter/Custom2DExporter/custom2dexporter.json) | `CustomExporter/Custom2DExporter/CS/Command.cs` | 5 |
### DatumsModification (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `DatumAlignment` | [DatumsModification/datumalignment.md](docs/commands/DatumsModification/datumalignment.md) | — | `DatumsModification/CS/DatumsModificationCmd.cs` | 2 |
| [x] | — | `DatumPropagation` | [DatumsModification/datumpropagation.md](docs/commands/DatumsModification/datumpropagation.md) | — | `DatumsModification/CS/DatumsModificationCmd.cs` | 2 |
| [x] | — | `DatumStyleModification` | [DatumsModification/datumstylemodification.md](docs/commands/DatumsModification/datumstylemodification.md) | — | `DatumsModification/CS/DatumsModificationCmd.cs` | 2 |
### DeckProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeckProperties/deckproperties.md](docs/commands/DeckProperties/deckproperties.md) | [DeckProperties/deckproperties.json](docs/mcp/DeckProperties/deckproperties.json) | `DeckProperties/CS/Command.cs` | 4 |
### DeleteDimensions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeleteDimensions/deletedimensions.md](docs/commands/DeleteDimensions/deletedimensions.md) | [DeleteDimensions/deletedimensions.json](docs/mcp/DeleteDimensions/deletedimensions.json) | `DeleteDimensions/CS/DeleteDimesions.cs` | 4 |
### DeleteObject (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeleteObject/deleteobject.md](docs/commands/DeleteObject/deleteobject.md) | [DeleteObject/deleteobject.json](docs/mcp/DeleteObject/deleteobject.json) | `DeleteObject/CS/DeleteObject.cs` | 4 |
### DimensionLeaderEnd (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `MoveHorizontally` | [DimensionLeaderEnd/movehorizontally.md](docs/commands/DimensionLeaderEnd/movehorizontally.md) | [DimensionLeaderEnd/movehorizontally.json](docs/mcp/DimensionLeaderEnd/movehorizontally.json) | `DimensionLeaderEnd/CS/Command.cs` | 3 |
| [x] | [x] | `MoveToPickedPoint` | [DimensionLeaderEnd/movetopickedpoint.md](docs/commands/DimensionLeaderEnd/movetopickedpoint.md) | [DimensionLeaderEnd/movetopickedpoint.json](docs/mcp/DimensionLeaderEnd/movetopickedpoint.json) | `DimensionLeaderEnd/CS/Command.cs` | 3 |
### DirectionCalculation (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `FindSouthFacingWallsWithoutProjectLocation` | [DirectionCalculation/findsouthfacingwallswithoutprojectlocation.md](docs/commands/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.md) | [DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json](docs/mcp/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json) | `DirectionCalculation/CS/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWallsWithProjectLocation` | [DirectionCalculation/findsouthfacingwallswithprojectlocation.md](docs/commands/DirectionCalculation/findsouthfacingwallswithprojectlocation.md) | [DirectionCalculation/findsouthfacingwallswithprojectlocation.json](docs/mcp/DirectionCalculation/findsouthfacingwallswithprojectlocation.json) | `DirectionCalculation/CS/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWindowsWithoutProjectLocation` | [DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.md](docs/commands/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.md) | [DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json](docs/mcp/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json) | `DirectionCalculation/CS/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWindowsWithProjectLocation` | [DirectionCalculation/findsouthfacingwindowswithprojectlocation.md](docs/commands/DirectionCalculation/findsouthfacingwindowswithprojectlocation.md) | [DirectionCalculation/findsouthfacingwindowswithprojectlocation.json](docs/mcp/DirectionCalculation/findsouthfacingwindowswithprojectlocation.json) | `DirectionCalculation/CS/Commands.cs` | 5 |
### DisplacementElementAnimation (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `DisplacementStructureModelAnimatorCommand` | [DisplacementElementAnimation/displacementstructuremodelanimatorcommand.md](docs/commands/DisplacementElementAnimation/displacementstructuremodelanimatorcommand.md) | — | `DisplacementElementAnimation/CS/DisplacementStructureModelAnimatorCommand.cs` | 1 |
| [x] | — | `DisplacementStructureModelAnimatorCommandStepByStep` | [DisplacementElementAnimation/displacementstructuremodelanimatorcommandstepbystep.md](docs/commands/DisplacementElementAnimation/displacementstructuremodelanimatorcommandstepbystep.md) | — | `DisplacementElementAnimation/CS/DisplacementStructureModelAnimatorCommand.cs` | 1 |
### DockableDialogs (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ExternalCommandHidePage` | [DockableDialogs/externalcommandhidepage.md](docs/commands/DockableDialogs/externalcommandhidepage.md) | — | `DockableDialogs/CS/TopLevelCommands/ExternalCommandHidePage.cs` | 1 |
| [x] | — | `ExternalCommandRegisterPage` | [DockableDialogs/externalcommandregisterpage.md](docs/commands/DockableDialogs/externalcommandregisterpage.md) | — | `DockableDialogs/CS/TopLevelCommands/ExternalCommandRegisterPage.cs` | 1 |
| [x] | — | `ExternalCommandShowPage` | [DockableDialogs/externalcommandshowpage.md](docs/commands/DockableDialogs/externalcommandshowpage.md) | — | `DockableDialogs/CS/TopLevelCommands/ExternalCommandShowPage.cs` | 1 |
### DocumentChanged (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [DocumentChanged/documentchanged.md](docs/commands/DocumentChanged/documentchanged.md) | — | `DocumentChanged/CS/ChangesMonitor.cs` | 1 |
### DoorSwing (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `InitializeCommand` | [DoorSwing/initializecommand.md](docs/commands/DoorSwing/initializecommand.md) | — | `DoorSwing/CS/Command.cs` | 2 |
| [x] | — | `UpdateGeometryCommand` | [DoorSwing/updategeometrycommand.md](docs/commands/DoorSwing/updategeometrycommand.md) | — | `DoorSwing/CS/Command.cs` | 2 |
| [x] | — | `UpdateParamsCommand` | [DoorSwing/updateparamscommand.md](docs/commands/DoorSwing/updateparamscommand.md) | — | `DoorSwing/CS/Command.cs` | 2 |
### DuplicateGraphics (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CommandClearExternalGraphics` | [DuplicateGraphics/commandclearexternalgraphics.md](docs/commands/DuplicateGraphics/commandclearexternalgraphics.md) | — | `DuplicateGraphics/CS/Command.cs` | 2 |
| [x] | [x] | `CommandDuplicateGraphics` | [DuplicateGraphics/commandduplicategraphics.md](docs/commands/DuplicateGraphics/commandduplicategraphics.md) | [DuplicateGraphics/commandduplicategraphics.json](docs/mcp/DuplicateGraphics/commandduplicategraphics.json) | `DuplicateGraphics/CS/Command.cs` | 4 |
### DuplicateViews (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DuplicateAcrossDocumentsCommand` | [DuplicateViews/duplicateacrossdocumentscommand.md](docs/commands/DuplicateViews/duplicateacrossdocumentscommand.md) | [DuplicateViews/duplicateacrossdocumentscommand.json](docs/mcp/DuplicateViews/duplicateacrossdocumentscommand.json) | `DuplicateViews/CS/DuplicateAcrossDocumentsCommand.cs` | 4 |
### DynamicModelUpdate (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `AssociativeSectionUpdater` | [DynamicModelUpdate/associativesectionupdater.md](docs/commands/DynamicModelUpdate/associativesectionupdater.md) | — | `DynamicModelUpdate/CS/Application.cs` | 2 |
### ElementFilterSample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ElementFilterSample/elementfiltersample.md](docs/commands/ElementFilterSample/elementfiltersample.md) | [ElementFilterSample/elementfiltersample.json](docs/mcp/ElementFilterSample/elementfiltersample.json) | `ElementFilterSample/CS/Command.cs` | 5 |
### ErrorHandling (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ErrorHandling/errorhandling.md](docs/commands/ErrorHandling/errorhandling.md) | — | `ErrorHandling/CS/Command.cs` | 1 |
### Events (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Events/EventsMonitor/eventsmonitor.md](docs/commands/Events/EventsMonitor/eventsmonitor.md) | — | `Events/EventsMonitor/CS/Command.cs` | 1 |
| [x] | — | `Command` | [Events/PrintLog/printlog.md](docs/commands/Events/PrintLog/printlog.md) | — | `Events/PrintLog/CS/Command.cs` | 1 |
| [x] | — | `Command` | [Events/ProgressNotifier/progressnotifier.md](docs/commands/Events/ProgressNotifier/progressnotifier.md) | — | `Events/ProgressNotifier/CS/Command.cs` | 1 |
| [x] | — | `Command` | [Events/SelectionChanged/selectionchanged.md](docs/commands/Events/SelectionChanged/selectionchanged.md) | — | `Events/SelectionChanged/CS/Command.cs` | 1 |
### ExportPDFSettingsSample (5)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddNamingRuleCommand` | [ExportPDFSettingsSample/addnamingrulecommand.md](docs/commands/ExportPDFSettingsSample/addnamingrulecommand.md) | [ExportPDFSettingsSample/addnamingrulecommand.json](docs/mcp/ExportPDFSettingsSample/addnamingrulecommand.json) | `ExportPDFSettingsSample/CS/Application.cs` | 4 |
| [x] | [x] | `CreateExportPdfSettingsCommand` | [ExportPDFSettingsSample/createexportpdfsettingscommand.md](docs/commands/ExportPDFSettingsSample/createexportpdfsettingscommand.md) | [ExportPDFSettingsSample/createexportpdfsettingscommand.json](docs/mcp/ExportPDFSettingsSample/createexportpdfsettingscommand.json) | `ExportPDFSettingsSample/CS/Application.cs` | 5 |
| [x] | [x] | `DeleteNamingRuleCommand` | [ExportPDFSettingsSample/deletenamingrulecommand.md](docs/commands/ExportPDFSettingsSample/deletenamingrulecommand.md) | [ExportPDFSettingsSample/deletenamingrulecommand.json](docs/mcp/ExportPDFSettingsSample/deletenamingrulecommand.json) | `ExportPDFSettingsSample/CS/Application.cs` | 4 |
| [x] | [x] | `ModifyExportPdfSettingsCommand` | [ExportPDFSettingsSample/modifyexportpdfsettingscommand.md](docs/commands/ExportPDFSettingsSample/modifyexportpdfsettingscommand.md) | [ExportPDFSettingsSample/modifyexportpdfsettingscommand.json](docs/mcp/ExportPDFSettingsSample/modifyexportpdfsettingscommand.json) | `ExportPDFSettingsSample/CS/Application.cs` | 5 |
| [x] | — | `MofidyNamingRuleCommand` | [ExportPDFSettingsSample/mofidynamingrulecommand.md](docs/commands/ExportPDFSettingsSample/mofidynamingrulecommand.md) | — | `ExportPDFSettingsSample/CS/Application.cs` | 2 |
### ExtensibleStorageManager (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ExtensibleStorageManager/ExtensibleStorageManager/extensiblestoragemanager.md](docs/commands/ExtensibleStorageManager/ExtensibleStorageManager/extensiblestoragemanager.md) | — | `ExtensibleStorageManager/ExtensibleStorageManager/CS/Application/Command.cs` | 2 |
### ExtensibleStorageUtility (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DeleteStorage` | [ExtensibleStorageUtility/deletestorage.md](docs/commands/ExtensibleStorageUtility/deletestorage.md) | [ExtensibleStorageUtility/deletestorage.json](docs/mcp/ExtensibleStorageUtility/deletestorage.json) | `ExtensibleStorageUtility/CS/DeleteStorage.cs` | 4 |
| [x] | [x] | `QueryStorage` | [ExtensibleStorageUtility/querystorage.md](docs/commands/ExtensibleStorageUtility/querystorage.md) | [ExtensibleStorageUtility/querystorage.json](docs/mcp/ExtensibleStorageUtility/querystorage.json) | `ExtensibleStorageUtility/CS/QueryStorage.cs` | 5 |
### ExternalCommand (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ExternalCommand3DView` | [ExternalCommand/externalcommand3dview.md](docs/commands/ExternalCommand/externalcommand3dview.md) | — | `ExternalCommand/CS/ExternalCommandRegistration/ExternalCommandClass.cs` | 1 |
| [x] | — | `ExternalCommandCreateWall` | [ExternalCommand/externalcommandcreatewall.md](docs/commands/ExternalCommand/externalcommandcreatewall.md) | — | `ExternalCommand/CS/ExternalCommandRegistration/ExternalCommandClass.cs` | 1 |
### FabricationPartLayout (25)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Ancillaries` | [FabricationPartLayout/ancillaries.md](docs/commands/FabricationPartLayout/ancillaries.md) | — | `FabricationPartLayout/CS/Ancillaries.cs` | 2 |
| [x] | — | `ApplyChange` | [FabricationPartLayout/applychange.md](docs/commands/FabricationPartLayout/applychange.md) | — | `FabricationPartLayout/CS/ChangeService.cs` | 2 |
| [x] | — | `ButtonPaletteExclusions` | [FabricationPartLayout/buttonpaletteexclusions.md](docs/commands/FabricationPartLayout/buttonpaletteexclusions.md) | — | `FabricationPartLayout/CS/ButtonPaletteExclusions.cs` | 2 |
| [x] | — | `ChangeService` | [FabricationPartLayout/changeservice.md](docs/commands/FabricationPartLayout/changeservice.md) | — | `FabricationPartLayout/CS/ChangeService.cs` | 2 |
| [x] | — | `ChangeSize` | [FabricationPartLayout/changesize.md](docs/commands/FabricationPartLayout/changesize.md) | — | `FabricationPartLayout/CS/ChangeService.cs` | 2 |
| [x] | — | `ConvertToFabrication` | [FabricationPartLayout/converttofabrication.md](docs/commands/FabricationPartLayout/converttofabrication.md) | — | `FabricationPartLayout/CS/ConvertToFabrication.cs` | 2 |
| [x] | — | `DecreaseRodStructureExtension` | [FabricationPartLayout/decreaserodstructureextension.md](docs/commands/FabricationPartLayout/decreaserodstructureextension.md) | — | `FabricationPartLayout/CS/HangerRods.cs` | 2 |
| [x] | — | `DetachRods` | [FabricationPartLayout/detachrods.md](docs/commands/FabricationPartLayout/detachrods.md) | — | `FabricationPartLayout/CS/HangerRods.cs` | 2 |
| [x] | — | `DoubleRodLength` | [FabricationPartLayout/doublerodlength.md](docs/commands/FabricationPartLayout/doublerodlength.md) | — | `FabricationPartLayout/CS/HangerRods.cs` | 2 |
| [x] | [x] | `ExportToMaj` | [FabricationPartLayout/exporttomaj.md](docs/commands/FabricationPartLayout/exporttomaj.md) | [FabricationPartLayout/exporttomaj.json](docs/mcp/FabricationPartLayout/exporttomaj.json) | `FabricationPartLayout/CS/ExportToMAJ.cs` | 5 |
| [x] | [x] | `ExportToPcf` | [FabricationPartLayout/exporttopcf.md](docs/commands/FabricationPartLayout/exporttopcf.md) | [FabricationPartLayout/exporttopcf.json](docs/mcp/FabricationPartLayout/exporttopcf.json) | `FabricationPartLayout/CS/ExportToPCF.cs` | 5 |
| [x] | — | `FabPartGeometry` | [FabricationPartLayout/fabpartgeometry.md](docs/commands/FabricationPartLayout/fabpartgeometry.md) | — | `FabricationPartLayout/CS/FabPartGeometry.cs` | 2 |
| [x] | — | `FabricationPartLayout` | [FabricationPartLayout/fabricationpartlayout.md](docs/commands/FabricationPartLayout/fabricationpartlayout.md) | — | `FabricationPartLayout/CS/FabricationPartLayout.cs` | 2 |
| [x] | — | `FlipPart` | [FabricationPartLayout/flippart.md](docs/commands/FabricationPartLayout/flippart.md) | — | `FabricationPartLayout/CS/FlipPart.cs` | 2 |
| [x] | — | `GetCustomData` | [FabricationPartLayout/getcustomdata.md](docs/commands/FabricationPartLayout/getcustomdata.md) | — | `FabricationPartLayout/CS/CustomData.cs` | 2 |
| [x] | — | `HalveRodLength` | [FabricationPartLayout/halverodlength.md](docs/commands/FabricationPartLayout/halverodlength.md) | — | `FabricationPartLayout/CS/HangerRods.cs` | 2 |
| [x] | — | `IncreaseRodStructureExtension` | [FabricationPartLayout/increaserodstructureextension.md](docs/commands/FabricationPartLayout/increaserodstructureextension.md) | — | `FabricationPartLayout/CS/HangerRods.cs` | 2 |
| [x] | — | `LoadAndPlaceNextItemFile` | [FabricationPartLayout/loadandplacenextitemfile.md](docs/commands/FabricationPartLayout/loadandplacenextitemfile.md) | — | `FabricationPartLayout/CS/ItemFile.cs` | 2 |
| [x] | — | `OptimizeStraights` | [FabricationPartLayout/optimizestraights.md](docs/commands/FabricationPartLayout/optimizestraights.md) | — | `FabricationPartLayout/CS/OptimizeStraights.cs` | 2 |
| [x] | [x] | `PartInfo` | [FabricationPartLayout/partinfo.md](docs/commands/FabricationPartLayout/partinfo.md) | [FabricationPartLayout/partinfo.json](docs/mcp/FabricationPartLayout/partinfo.json) | `FabricationPartLayout/CS/PartInfo.cs` | 5 |
| [x] | — | `PartRenumber` | [FabricationPartLayout/partrenumber.md](docs/commands/FabricationPartLayout/partrenumber.md) | — | `FabricationPartLayout/CS/PartRenumber.cs` | 2 |
| [x] | — | `SetCustomData` | [FabricationPartLayout/setcustomdata.md](docs/commands/FabricationPartLayout/setcustomdata.md) | — | `FabricationPartLayout/CS/CustomData.cs` | 2 |
| [x] | — | `SplitStraight` | [FabricationPartLayout/splitstraight.md](docs/commands/FabricationPartLayout/splitstraight.md) | — | `FabricationPartLayout/CS/SplitStraight.cs` | 2 |
| [x] | — | `StretchAndFit` | [FabricationPartLayout/stretchandfit.md](docs/commands/FabricationPartLayout/stretchandfit.md) | — | `FabricationPartLayout/CS/StretchAndFit.cs` | 2 |
| [x] | — | `UnloadUnusedItemFiles` | [FabricationPartLayout/unloadunuseditemfiles.md](docs/commands/FabricationPartLayout/unloadunuseditemfiles.md) | — | `FabricationPartLayout/CS/ItemFile.cs` | 2 |
### FamilyCreation (10)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [FamilyCreation/AutoJoin/autojoin.md](docs/commands/FamilyCreation/AutoJoin/autojoin.md) | — | `FamilyCreation/AutoJoin/CS/Command.cs` | 2 |
| [x] | [x] | `AddParameterToFamilies` | [FamilyCreation/AutoParameter/addparametertofamilies.md](docs/commands/FamilyCreation/AutoParameter/addparametertofamilies.md) | [FamilyCreation/AutoParameter/addparametertofamilies.json](docs/mcp/FamilyCreation/AutoParameter/addparametertofamilies.json) | `FamilyCreation/AutoParameter/CS/Command.cs` | 4 |
| [x] | [x] | `AddParameterToFamily` | [FamilyCreation/AutoParameter/addparametertofamily.md](docs/commands/FamilyCreation/AutoParameter/addparametertofamily.md) | [FamilyCreation/AutoParameter/addparametertofamily.json](docs/mcp/FamilyCreation/AutoParameter/addparametertofamily.json) | `FamilyCreation/AutoParameter/CS/Command.cs` | 4 |
| [x] | — | `Command` | [FamilyCreation/CreateAirHandler/createairhandler.md](docs/commands/FamilyCreation/CreateAirHandler/createairhandler.md) | — | `FamilyCreation/CreateAirHandler/CS/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/CreateTruss/createtruss.md](docs/commands/FamilyCreation/CreateTruss/createtruss.md) | — | `FamilyCreation/CreateTruss/CS/CreateTruss.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/DWGFamilyCreation/dwgfamilycreation.md](docs/commands/FamilyCreation/DWGFamilyCreation/dwgfamilycreation.md) | — | `FamilyCreation/DWGFamilyCreation/CS/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/GenericModelCreation/genericmodelcreation.md](docs/commands/FamilyCreation/GenericModelCreation/genericmodelcreation.md) | — | `FamilyCreation/GenericModelCreation/CS/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/TypeRegeneration/typeregeneration.md](docs/commands/FamilyCreation/TypeRegeneration/typeregeneration.md) | — | `FamilyCreation/TypeRegeneration/CS/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/ValidateParameters/validateparameters.md](docs/commands/FamilyCreation/ValidateParameters/validateparameters.md) | — | `FamilyCreation/ValidateParameters/CS/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/WindowWizard/windowwizard.md](docs/commands/FamilyCreation/WindowWizard/windowwizard.md) | — | `FamilyCreation/WindowWizard/CS/Command.cs` | 2 |
### FamilyParametersOrder (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [FamilyParametersOrder/familyparametersorder.md](docs/commands/FamilyParametersOrder/familyparametersorder.md) | — | `FamilyParametersOrder/CS/Command.cs` | 2 |
### FindReferencesByDirection (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FindReferencesByDirection/FindColumns/findcolumns.md](docs/commands/FindReferencesByDirection/FindColumns/findcolumns.md) | [FindReferencesByDirection/FindColumns/findcolumns.json](docs/mcp/FindReferencesByDirection/FindColumns/findcolumns.json) | `FindReferencesByDirection/FindColumns/CS/FindColumns.cs` | 5 |
| [x] | [x] | `Command` | [FindReferencesByDirection/MeasureHeight/measureheight.md](docs/commands/FindReferencesByDirection/MeasureHeight/measureheight.md) | [FindReferencesByDirection/MeasureHeight/measureheight.json](docs/mcp/FindReferencesByDirection/MeasureHeight/measureheight.json) | `FindReferencesByDirection/MeasureHeight/CS/MeasureHeight.cs` | 5 |
| [x] | [x] | `Command` | [FindReferencesByDirection/RaytraceBounce/raytracebounce.md](docs/commands/FindReferencesByDirection/RaytraceBounce/raytracebounce.md) | [FindReferencesByDirection/RaytraceBounce/raytracebounce.json](docs/mcp/FindReferencesByDirection/RaytraceBounce/raytracebounce.json) | `FindReferencesByDirection/RaytraceBounce/CS/RayTraceBounce.cs` | 5 |
### FoundationSlab (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FoundationSlab/foundationslab.md](docs/commands/FoundationSlab/foundationslab.md) | [FoundationSlab/foundationslab.json](docs/mcp/FoundationSlab/foundationslab.json) | `FoundationSlab/CS/Command.cs` | 4 |
### FrameBuilder (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FrameBuilder/framebuilder.md](docs/commands/FrameBuilder/framebuilder.md) | [FrameBuilder/framebuilder.json](docs/mcp/FrameBuilder/framebuilder.json) | `FrameBuilder/CS/Command.cs` | 4 |
### FreeFormElement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateNegativeBlockCommand` | [FreeFormElement/createnegativeblockcommand.md](docs/commands/FreeFormElement/createnegativeblockcommand.md) | [FreeFormElement/createnegativeblockcommand.json](docs/mcp/FreeFormElement/createnegativeblockcommand.json) | `FreeFormElement/CS/CreateNegativeBlockCommand.cs` | 4 |
### GenerateFloor (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [GenerateFloor/generatefloor.md](docs/commands/GenerateFloor/generatefloor.md) | [GenerateFloor/generatefloor.json](docs/mcp/GenerateFloor/generatefloor.json) | `GenerateFloor/CS/Command.cs` | 4 |
### GenericStructuralConnection (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [GenericStructuralConnection/genericstructuralconnection.md](docs/commands/GenericStructuralConnection/genericstructuralconnection.md) | — | `GenericStructuralConnection/CS/Command.cs` | 2 |
### GeometryAPI (9)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateCube` | [GeometryAPI/BRepBuilderExample/createcube.md](docs/commands/GeometryAPI/BRepBuilderExample/createcube.md) | [GeometryAPI/BRepBuilderExample/createcube.json](docs/mcp/GeometryAPI/BRepBuilderExample/createcube.json) | `GeometryAPI/BRepBuilderExample/CS/CreateCube.cs` | 4 |
| [x] | [x] | `CreateNurbs` | [GeometryAPI/BRepBuilderExample/createnurbs.md](docs/commands/GeometryAPI/BRepBuilderExample/createnurbs.md) | [GeometryAPI/BRepBuilderExample/createnurbs.json](docs/mcp/GeometryAPI/BRepBuilderExample/createnurbs.json) | `GeometryAPI/BRepBuilderExample/CS/CreateNURBS.cs` | 4 |
| [x] | [x] | `CreatePeriodic` | [GeometryAPI/BRepBuilderExample/createperiodic.md](docs/commands/GeometryAPI/BRepBuilderExample/createperiodic.md) | [GeometryAPI/BRepBuilderExample/createperiodic.json](docs/mcp/GeometryAPI/BRepBuilderExample/createperiodic.json) | `GeometryAPI/BRepBuilderExample/CS/CreatePeriodic.cs` | 4 |
| [x] | — | `Command` | [GeometryAPI/ComputedSymbolGeometry/computedsymbolgeometry.md](docs/commands/GeometryAPI/ComputedSymbolGeometry/computedsymbolgeometry.md) | — | `GeometryAPI/ComputedSymbolGeometry/CS/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/EnergyAnalysisModel/energyanalysismodel.md](docs/commands/GeometryAPI/EnergyAnalysisModel/energyanalysismodel.md) | — | `GeometryAPI/EnergyAnalysisModel/CS/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/GeometryCreation_BooleanOperation/geometrycreation-booleanoperation.md](docs/commands/GeometryAPI/GeometryCreation_BooleanOperation/geometrycreation-booleanoperation.md) | — | `GeometryAPI/GeometryCreation_BooleanOperation/CS/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/ProximityDetection_WallJoinControl/proximitydetection-walljoincontrol.md](docs/commands/GeometryAPI/ProximityDetection_WallJoinControl/proximitydetection-walljoincontrol.md) | — | `GeometryAPI/ProximityDetection_WallJoinControl/CS/Command.cs` | 2 |
| [x] | [x] | `CreateBRep` | [GeometryAPI/UpdateExternallyTaggedBRep/createbrep.md](docs/commands/GeometryAPI/UpdateExternallyTaggedBRep/createbrep.md) | [GeometryAPI/UpdateExternallyTaggedBRep/createbrep.json](docs/mcp/GeometryAPI/UpdateExternallyTaggedBRep/createbrep.json) | `GeometryAPI/UpdateExternallyTaggedBRep/CS/CreateBRep.cs` | 4 |
| [x] | — | `UpdateBRep` | [GeometryAPI/UpdateExternallyTaggedBRep/updatebrep.md](docs/commands/GeometryAPI/UpdateExternallyTaggedBRep/updatebrep.md) | — | `GeometryAPI/UpdateExternallyTaggedBRep/CS/UpdateBRep.cs` | 2 |
### GetSetDefaultTypes (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ThisCommand` | [GetSetDefaultTypes/thiscommand.md](docs/commands/GetSetDefaultTypes/thiscommand.md) | [GetSetDefaultTypes/thiscommand.json](docs/mcp/GetSetDefaultTypes/thiscommand.json) | `GetSetDefaultTypes/CS/ThisCommand.cs` | 4 |
### GridCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [GridCreation/gridcreation.md](docs/commands/GridCreation/gridcreation.md) | [GridCreation/gridcreation.json](docs/mcp/GridCreation/gridcreation.json) | `GridCreation/CS/Command.cs` | 4 |
### HelloRevit (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [HelloRevit/hellorevit.md](docs/commands/HelloRevit/hellorevit.md) | — | `HelloRevit/CS/Command.cs` | 2 |
### Host (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `SampleBrowserCommand` | [Host/samplebrowsercommand.md](docs/commands/Host/samplebrowsercommand.md) | — | `SampleBrowserCommand.cs` | 1 |
### ImportExport (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ImportExport/importexport.md](docs/commands/ImportExport/importexport.md) | [ImportExport/importexport.json](docs/mcp/ImportExport/importexport.json) | `ImportExport/CS/Command.cs` | 5 |
### InCanvasControlAPI (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InCanvasControlAPI/incanvascontrolapi.md](docs/commands/InCanvasControlAPI/incanvascontrolapi.md) | — | `InCanvasControlAPI/CS/Command.cs` | 1 |
### InPlaceMembers (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InPlaceMembers/inplacemembers.md](docs/commands/InPlaceMembers/inplacemembers.md) | — | `InPlaceMembers/CS/Command.cs` | 2 |
### InvisibleParam (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InvisibleParam/invisibleparam.md](docs/commands/InvisibleParam/invisibleparam.md) | — | `InvisibleParam/CS/Command.cs` | 2 |
### Journaling (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Journaling/journaling.md](docs/commands/Journaling/journaling.md) | — | `Journaling/CS/Command.cs` | 1 |
### LevelsProperty (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [LevelsProperty/levelsproperty.md](docs/commands/LevelsProperty/levelsproperty.md) | [LevelsProperty/levelsproperty.json](docs/mcp/LevelsProperty/levelsproperty.json) | `LevelsProperty/CS/Command.cs` | 5 |
### Loads (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Loads` | [Loads/loads.md](docs/commands/Loads/loads.md) | [Loads/loads.json](docs/mcp/Loads/loads.json) | `Loads/CS/Loads.cs` | 4 |
### Massing (17)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `SetDistanceParam` | [Massing/DistanceToPanels/setdistanceparam.md](docs/commands/Massing/DistanceToPanels/setdistanceparam.md) | — | `Massing/DistanceToPanels/CS/Command.cs` | 2 |
| [x] | — | `Command` | [Massing/DividedSurfaceByIntersects/dividedsurfacebyintersects.md](docs/commands/Massing/DividedSurfaceByIntersects/dividedsurfacebyintersects.md) | — | `Massing/DividedSurfaceByIntersects/CS/Command.cs` | 2 |
| [x] | — | `Command` | [Massing/ManipulateForm/manipulateform.md](docs/commands/Massing/ManipulateForm/manipulateform.md) | — | `Massing/ManipulateForm/CS/Command.cs` | 2 |
| [x] | [x] | `MeasurePanelArea` | [Massing/MeasurePanelArea/measurepanelarea.md](docs/commands/Massing/MeasurePanelArea/measurepanelarea.md) | [Massing/MeasurePanelArea/measurepanelarea.json](docs/mcp/Massing/MeasurePanelArea/measurepanelarea.json) | `Massing/MeasurePanelArea/CS/Command.cs` | 5 |
| [x] | — | `MakeCapForm` | [Massing/NewForm/makecapform.md](docs/commands/Massing/NewForm/makecapform.md) | — | `Massing/NewForm/CS/Command.cs` | 2 |
| [x] | — | `MakeExtrusionForm` | [Massing/NewForm/makeextrusionform.md](docs/commands/Massing/NewForm/makeextrusionform.md) | — | `Massing/NewForm/CS/Command.cs` | 2 |
| [x] | — | `MakeLoftForm` | [Massing/NewForm/makeloftform.md](docs/commands/Massing/NewForm/makeloftform.md) | — | `Massing/NewForm/CS/Command.cs` | 2 |
| [x] | — | `MakeRevolveForm` | [Massing/NewForm/makerevolveform.md](docs/commands/Massing/NewForm/makerevolveform.md) | — | `Massing/NewForm/CS/Command.cs` | 2 |
| [x] | — | `MakeSweptBlendForm` | [Massing/NewForm/makesweptblendform.md](docs/commands/Massing/NewForm/makesweptblendform.md) | — | `Massing/NewForm/CS/Command.cs` | 2 |
| [x] | — | `SetLengthAngleParams` | [Massing/PanelEdgeLengthAngle/setlengthangleparams.md](docs/commands/Massing/PanelEdgeLengthAngle/setlengthangleparams.md) | — | `Massing/PanelEdgeLengthAngle/CS/Command.cs` | 2 |
| [x] | — | `SetParameterValueWithImageData` | [Massing/ParameterValuesFromImage/setparametervaluewithimagedata.md](docs/commands/Massing/ParameterValuesFromImage/setparametervaluewithimagedata.md) | — | `Massing/ParameterValuesFromImage/CS/Command.cs` | 2 |
| [x] | — | `CatenaryCurve` | [Massing/PointCurveCreation/catenarycurve.md](docs/commands/Massing/PointCurveCreation/catenarycurve.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
| [x] | — | `CyclicSurface` | [Massing/PointCurveCreation/cyclicsurface.md](docs/commands/Massing/PointCurveCreation/cyclicsurface.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
| [x] | — | `PointsFromTextFile` | [Massing/PointCurveCreation/pointsfromtextfile.md](docs/commands/Massing/PointCurveCreation/pointsfromtextfile.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
| [x] | — | `PointsOnCurve` | [Massing/PointCurveCreation/pointsoncurve.md](docs/commands/Massing/PointCurveCreation/pointsoncurve.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
| [x] | — | `PointsParabola` | [Massing/PointCurveCreation/pointsparabola.md](docs/commands/Massing/PointCurveCreation/pointsparabola.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
| [x] | — | `SineCurve` | [Massing/PointCurveCreation/sinecurve.md](docs/commands/Massing/PointCurveCreation/sinecurve.md) | — | `Massing/PointCurveCreation/CS/Command.cs` | 2 |
### MaterialProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `MaterialProperties` | [MaterialProperties/materialproperties.md](docs/commands/MaterialProperties/materialproperties.md) | [MaterialProperties/materialproperties.json](docs/mcp/MaterialProperties/materialproperties.json) | `MaterialProperties/CS/MaterialProperties.cs` | 5 |
### MaterialQuantities (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [MaterialQuantities/materialquantities.md](docs/commands/MaterialQuantities/materialquantities.md) | [MaterialQuantities/materialquantities.json](docs/mcp/MaterialQuantities/materialquantities.json) | `MaterialQuantities/CS/MaterialQuantities.cs` | 5 |
### ModelessDialog (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ModelessDialog/ModelessForm_ExternalEvent/modelessform-externalevent.md](docs/commands/ModelessDialog/ModelessForm_ExternalEvent/modelessform-externalevent.md) | — | `ModelessDialog/ModelessForm_ExternalEvent/CS/Command.cs` | 1 |
| [x] | — | `Command` | [ModelessDialog/ModelessForm_IdlingEvent/modelessform-idlingevent.md](docs/commands/ModelessDialog/ModelessForm_IdlingEvent/modelessform-idlingevent.md) | — | `ModelessDialog/ModelessForm_IdlingEvent/CS/Command.cs` | 1 |
### ModelLines (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ModelLines/modellines.md](docs/commands/ModelLines/modellines.md) | [ModelLines/modellines.json](docs/mcp/ModelLines/modellines.json) | `ModelLines/CS/Command.cs` | 4 |
### MoveLinear (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [MoveLinear/movelinear.md](docs/commands/MoveLinear/movelinear.md) | [MoveLinear/movelinear.json](docs/mcp/MoveLinear/movelinear.json) | `MoveLinear/CS/Command.cs` | 4 |
### MultiplanarRebar (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [MultiplanarRebar/multiplanarrebar.md](docs/commands/MultiplanarRebar/multiplanarrebar.md) | — | `MultiplanarRebar/CS/Command.cs` | 2 |
### MultiThreading (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [MultiThreading/WorkThread/workthread.md](docs/commands/MultiThreading/WorkThread/workthread.md) | — | `MultiThreading/WorkThread/CS/Command.cs` | 2 |
### NetworkPressureLossReport (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NetworkPressureLossReport/networkpressurelossreport.md](docs/commands/NetworkPressureLossReport/networkpressurelossreport.md) | [NetworkPressureLossReport/networkpressurelossreport.json](docs/mcp/NetworkPressureLossReport/networkpressurelossreport.json) | `NetworkPressureLossReport/CS/Command.cs` | 5 |
### NewHostedSweep (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewHostedSweep/newhostedsweep.md](docs/commands/NewHostedSweep/newhostedsweep.md) | — | `NewHostedSweep/CS/Command.cs` | 2 |
### NewOpenings (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NewOpenings/newopenings.md](docs/commands/NewOpenings/newopenings.md) | [NewOpenings/newopenings.json](docs/mcp/NewOpenings/newopenings.json) | `NewOpenings/CS/command.cs` | 4 |
### NewPathReinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewPathReinforcement/newpathreinforcement.md](docs/commands/NewPathReinforcement/newpathreinforcement.md) | — | `NewPathReinforcement/CS/Command.cs` | 2 |
### NewRebar (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewRebar/newrebar.md](docs/commands/NewRebar/newrebar.md) | — | `NewRebar/CS/Command.cs` | 2 |
### NewRoof (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NewRoof/newroof.md](docs/commands/NewRoof/newroof.md) | [NewRoof/newroof.json](docs/mcp/NewRoof/newroof.json) | `NewRoof/CS/Command.cs` | 4 |
### Openings (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Openings/openings.md](docs/commands/Openings/openings.md) | [Openings/openings.json](docs/mcp/Openings/openings.json) | `Openings/CS/Command.cs` | 4 |
### PanelSchedule (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `InstanceViewCreation` | [PanelSchedule/instanceviewcreation.md](docs/commands/PanelSchedule/instanceviewcreation.md) | [PanelSchedule/instanceviewcreation.json](docs/mcp/PanelSchedule/instanceviewcreation.json) | `PanelSchedule/CS/InstanceViewCreation.cs` | 4 |
| [x] | [x] | `PanelScheduleExport` | [PanelSchedule/panelscheduleexport.md](docs/commands/PanelSchedule/panelscheduleexport.md) | [PanelSchedule/panelscheduleexport.json](docs/mcp/PanelSchedule/panelscheduleexport.json) | `PanelSchedule/CS/PanelScheduleExport.cs` | 5 |
| [x] | [x] | `SheetImport` | [PanelSchedule/sheetimport.md](docs/commands/PanelSchedule/sheetimport.md) | [PanelSchedule/sheetimport.json](docs/mcp/PanelSchedule/sheetimport.json) | `PanelSchedule/CS/SheetImport.cs` | 4 |
### ParameterUtils (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ParameterUtils/parameterutils.md](docs/commands/ParameterUtils/parameterutils.md) | [ParameterUtils/parameterutils.json](docs/mcp/ParameterUtils/parameterutils.json) | `ParameterUtils/CS/Command.cs` | 5 |
### PathOfTravel (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PathOfTravel/pathoftravel.md](docs/commands/PathOfTravel/pathoftravel.md) | [PathOfTravel/pathoftravel.json](docs/mcp/PathOfTravel/pathoftravel.json) | `PathOfTravel/CS/Command.cs` | 3 |
### PathReinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [PathReinforcement/pathreinforcement.md](docs/commands/PathReinforcement/pathreinforcement.md) | — | `PathReinforcement/CS/Command.cs` | 2 |
### PerformanceAdviserControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `UiCommand` | [PerformanceAdviserControl/uicommand.md](docs/commands/PerformanceAdviserControl/uicommand.md) | — | `PerformanceAdviserControl/CS/UICommand.cs` | 1 |
### PhysicalProp (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DumpMaterialPhysicalParameters` | [PhysicalProp/dumpmaterialphysicalparameters.md](docs/commands/PhysicalProp/dumpmaterialphysicalparameters.md) | [PhysicalProp/dumpmaterialphysicalparameters.json](docs/mcp/PhysicalProp/dumpmaterialphysicalparameters.json) | `PhysicalProp/CS/Command.cs` | 5 |
### PlaceFamilyInstanceByFace (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PlaceFamilyInstanceByFace/placefamilyinstancebyface.md](docs/commands/PlaceFamilyInstanceByFace/placefamilyinstancebyface.md) | [PlaceFamilyInstanceByFace/placefamilyinstancebyface.json](docs/mcp/PlaceFamilyInstanceByFace/placefamilyinstancebyface.json) | `PlaceFamilyInstanceByFace/CS/Command.cs` | 4 |
### PlacementOptions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PlacementOptions/placementoptions.md](docs/commands/PlacementOptions/placementoptions.md) | [PlacementOptions/placementoptions.json](docs/mcp/PlacementOptions/placementoptions.json) | `PlacementOptions/CS/Command.cs` | 4 |
### PointCloudEngine (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddPredefinedInstanceCommand` | [PointCloudEngine/addpredefinedinstancecommand.md](docs/commands/PointCloudEngine/addpredefinedinstancecommand.md) | [PointCloudEngine/addpredefinedinstancecommand.json](docs/mcp/PointCloudEngine/addpredefinedinstancecommand.json) | `PointCloudEngine/CS/PointCloudEngineSample.cs` | 4 |
| [x] | [x] | `AddRandomizedInstanceCommand` | [PointCloudEngine/addrandomizedinstancecommand.md](docs/commands/PointCloudEngine/addrandomizedinstancecommand.md) | [PointCloudEngine/addrandomizedinstancecommand.json](docs/mcp/PointCloudEngine/addrandomizedinstancecommand.json) | `PointCloudEngine/CS/PointCloudEngineSample.cs` | 4 |
| [x] | [x] | `AddTransformedInstanceCommand` | [PointCloudEngine/addtransformedinstancecommand.md](docs/commands/PointCloudEngine/addtransformedinstancecommand.md) | [PointCloudEngine/addtransformedinstancecommand.json](docs/mcp/PointCloudEngine/addtransformedinstancecommand.json) | `PointCloudEngine/CS/PointCloudEngineSample.cs` | 4 |
| [x] | — | `SerializePredefinedPointCloud` | [PointCloudEngine/serializepredefinedpointcloud.md](docs/commands/PointCloudEngine/serializepredefinedpointcloud.md) | — | `PointCloudEngine/CS/PointCloudEngineSample.cs` | 2 |
### PostCommandWorkflow (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `PostCommandRevisionMonitorCommand` | [PostCommandWorkflow/postcommandrevisionmonitorcommand.md](docs/commands/PostCommandWorkflow/postcommandrevisionmonitorcommand.md) | — | `PostCommandWorkflow/CS/PostCommandRevisionMonitorCommand.cs` | 1 |
### PowerCircuit (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [PowerCircuit/powercircuit.md](docs/commands/PowerCircuit/powercircuit.md) | — | `PowerCircuit/CS/Command.cs` | 2 |
### ProjectInfo (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ProjectInfo/projectinfo.md](docs/commands/ProjectInfo/projectinfo.md) | [ProjectInfo/projectinfo.json](docs/mcp/ProjectInfo/projectinfo.json) | `ProjectInfo/CS/Command.cs` | 5 |
### ReadonlySharedParameters (5)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `BindNewReadonlySharedParametersToDocument` | [ReadonlySharedParameters/bindnewreadonlysharedparameterstodocument.md](docs/commands/ReadonlySharedParameters/bindnewreadonlysharedparameterstodocument.md) | — | `ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyCost1` | [ReadonlySharedParameters/setreadonlycost1.md](docs/commands/ReadonlySharedParameters/setreadonlycost1.md) | — | `ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyCost2` | [ReadonlySharedParameters/setreadonlycost2.md](docs/commands/ReadonlySharedParameters/setreadonlycost2.md) | — | `ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyId1` | [ReadonlySharedParameters/setreadonlyid1.md](docs/commands/ReadonlySharedParameters/setreadonlyid1.md) | — | `ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyId2` | [ReadonlySharedParameters/setreadonlyid2.md](docs/commands/ReadonlySharedParameters/setreadonlyid2.md) | — | `ReadonlySharedParameters/CS/ReadonlySharedParametersCommands.cs` | 2 |
### RebarContainerAnyShapeType (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [RebarContainerAnyShapeType/rebarcontaineranyshapetype.md](docs/commands/RebarContainerAnyShapeType/rebarcontaineranyshapetype.md) | — | `RebarContainerAnyShapeType/CS/Command.cs` | 2 |
### RebarFreeForm (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddSharedParams` | [RebarFreeForm/addsharedparams.md](docs/commands/RebarFreeForm/addsharedparams.md) | [RebarFreeForm/addsharedparams.json](docs/mcp/RebarFreeForm/addsharedparams.json) | `RebarFreeForm/CS/AddSharedParams.cs` | 4 |
| [x] | — | `Command` | [RebarFreeForm/rebarfreeform.md](docs/commands/RebarFreeForm/rebarfreeform.md) | — | `RebarFreeForm/CS/Command.cs` | 2 |
### ReferencePlane (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ReferencePlane/referenceplane.md](docs/commands/ReferencePlane/referenceplane.md) | [ReferencePlane/referenceplane.json](docs/mcp/ReferencePlane/referenceplane.json) | `ReferencePlane/CS/Command.cs` | 4 |
### Reinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Reinforcement/reinforcement.md](docs/commands/Reinforcement/reinforcement.md) | — | `Reinforcement/CS/Command.cs` | 2 |
### Ribbon (6)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CreateWall` | [Ribbon/createwall.md](docs/commands/Ribbon/createwall.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
| [x] | — | `DeleteWalls` | [Ribbon/deletewalls.md](docs/commands/Ribbon/deletewalls.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
| [x] | — | `Dummy` | [Ribbon/dummy.md](docs/commands/Ribbon/dummy.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
| [x] | — | `ResetSetting` | [Ribbon/resetsetting.md](docs/commands/Ribbon/resetsetting.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
| [x] | — | `XMoveWalls` | [Ribbon/xmovewalls.md](docs/commands/Ribbon/xmovewalls.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
| [x] | — | `YMoveWalls` | [Ribbon/ymovewalls.md](docs/commands/Ribbon/ymovewalls.md) | — | `Ribbon/CS/AddInCommand.cs` | 1 |
### RoofsRooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [RoofsRooms/roofsrooms.md](docs/commands/RoofsRooms/roofsrooms.md) | [RoofsRooms/roofsrooms.json](docs/mcp/RoofsRooms/roofsrooms.json) | `RoofsRooms/CS/Command.cs` | 4 |
### Rooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Rooms/rooms.md](docs/commands/Rooms/rooms.md) | [Rooms/rooms.json](docs/mcp/Rooms/rooms.json) | `Rooms/CS/Command.cs` | 4 |
### RoomSchedule (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [RoomSchedule/roomschedule.md](docs/commands/RoomSchedule/roomschedule.md) | [RoomSchedule/roomschedule.json](docs/mcp/RoomSchedule/roomschedule.json) | `RoomSchedule/CS/Command.cs` | 4 |
### RotateFramingObjects (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `RotateFramingObjects` | [RotateFramingObjects/rotateframingobjects.md](docs/commands/RotateFramingObjects/rotateframingobjects.md) | [RotateFramingObjects/rotateframingobjects.json](docs/mcp/RotateFramingObjects/rotateframingobjects.json) | `RotateFramingObjects/CS/RotateFramingObjects.cs` | 4 |
### RoutingPreferenceTools (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [RoutingPreferenceTools/routingpreferencetools.md](docs/commands/RoutingPreferenceTools/routingpreferencetools.md) | — | `RoutingPreferenceTools/CS/RoutingPreferenceAnalysis/Command.cs` | 2 |
| [x] | [x] | `CommandReadPreferences` | [RoutingPreferenceTools/commandreadpreferences.md](docs/commands/RoutingPreferenceTools/commandreadpreferences.md) | [RoutingPreferenceTools/commandreadpreferences.json](docs/mcp/RoutingPreferenceTools/commandreadpreferences.json) | `RoutingPreferenceTools/CS/RoutingPreferenceBuilder/CommandReadPreferences.cs` | 5 |
| [x] | — | `CommandWritePreferences` | [RoutingPreferenceTools/commandwritepreferences.md](docs/commands/RoutingPreferenceTools/commandwritepreferences.md) | — | `RoutingPreferenceTools/CS/RoutingPreferenceBuilder/CommandWritePreferences.cs` | 2 |
### ScheduleAutomaticFormatter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ScheduleFormatterCommand` | [ScheduleAutomaticFormatter/scheduleformattercommand.md](docs/commands/ScheduleAutomaticFormatter/scheduleformattercommand.md) | [ScheduleAutomaticFormatter/scheduleformattercommand.json](docs/mcp/ScheduleAutomaticFormatter/scheduleformattercommand.json) | `ScheduleAutomaticFormatter/CS/ScheduleFormatterCommand.cs` | 4 |
### ScheduleCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ScheduleCreation/schedulecreation.md](docs/commands/ScheduleCreation/schedulecreation.md) | [ScheduleCreation/schedulecreation.json](docs/mcp/ScheduleCreation/schedulecreation.json) | `ScheduleCreation/CS/Command.cs` | 4 |
### ScheduleToHTML (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ScheduleHtmlExportCommand` | [ScheduleToHTML/schedulehtmlexportcommand.md](docs/commands/ScheduleToHTML/schedulehtmlexportcommand.md) | [ScheduleToHTML/schedulehtmlexportcommand.json](docs/mcp/ScheduleToHTML/schedulehtmlexportcommand.json) | `ScheduleToHTML/CS/ScheduleHTMLExportCommand.cs` | 5 |
### Selections (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `PickforDeletion` | [Selections/pickfordeletion.md](docs/commands/Selections/pickfordeletion.md) | — | `Selections/CS/Command.cs` | 2 |
| [x] | — | `PlaceAtPickedFaceWorkplane` | [Selections/placeatpickedfaceworkplane.md](docs/commands/Selections/placeatpickedfaceworkplane.md) | — | `Selections/CS/Command.cs` | 2 |
| [x] | — | `PlaceAtPointOnWallFace` | [Selections/placeatpointonwallface.md](docs/commands/Selections/placeatpointonwallface.md) | — | `Selections/CS/Command.cs` | 2 |
| [x] | — | `SelectionDialog` | [Selections/selectiondialog.md](docs/commands/Selections/selectiondialog.md) | — | `Selections/CS/Command.cs` | 2 |
### ShaftHolePuncher (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ShaftHolePuncher/shaftholepuncher.md](docs/commands/ShaftHolePuncher/shaftholepuncher.md) | [ShaftHolePuncher/shaftholepuncher.json](docs/mcp/ShaftHolePuncher/shaftholepuncher.json) | `ShaftHolePuncher/CS/Command.cs` | 4 |
### SharedCoordinateSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SharedCoordinateSystem/sharedcoordinatesystem.md](docs/commands/SharedCoordinateSystem/sharedcoordinatesystem.md) | [SharedCoordinateSystem/sharedcoordinatesystem.json](docs/mcp/SharedCoordinateSystem/sharedcoordinatesystem.json) | `SharedCoordinateSystem/CS/Command.cs` | 5 |
### SheetToView3D (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SheetToView3D/sheettoview3d.md](docs/commands/SheetToView3D/sheettoview3d.md) | [SheetToView3D/sheettoview3d.json](docs/mcp/SheetToView3D/sheettoview3d.json) | `SheetToView3D/CS/SheetToView3D.cs` | 5 |
### SinePlotter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [SinePlotter/sineplotter.md](docs/commands/SinePlotter/sineplotter.md) | — | `SinePlotter/CS/Command.cs` | 1 |
### Site (6)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `SiteAddRetainingPondCommand` | [Site/siteaddretainingpondcommand.md](docs/commands/Site/siteaddretainingpondcommand.md) | [Site/siteaddretainingpondcommand.json](docs/mcp/Site/siteaddretainingpondcommand.json) | `Site/CS/SiteAddRetainingPondCommand.cs` | 4 |
| [x] | [x] | `SiteDeleteRegionAndPointsCommand` | [Site/sitedeleteregionandpointscommand.md](docs/commands/Site/sitedeleteregionandpointscommand.md) | [Site/sitedeleteregionandpointscommand.json](docs/mcp/Site/sitedeleteregionandpointscommand.json) | `Site/CS/SiteDeleteRegionAndPointsCommand.cs` | 4 |
| [x] | — | `SiteLowerTerrainInRegionCommand` | [Site/sitelowerterraininregioncommand.md](docs/commands/Site/sitelowerterraininregioncommand.md) | — | `Site/CS/SiteLowerTerrainInRegionCommand.cs` | 2 |
| [x] | — | `SiteMoveRegionAndPointsCommand` | [Site/sitemoveregionandpointscommand.md](docs/commands/Site/sitemoveregionandpointscommand.md) | — | `Site/CS/SiteMoveRegionAndPointsCommand.cs` | 2 |
| [x] | — | `SiteNormalizeTerrainInRegionCommand` | [Site/sitenormalizeterraininregioncommand.md](docs/commands/Site/sitenormalizeterraininregioncommand.md) | — | `Site/CS/SiteNormalizeTerrainInRegionCommand.cs` | 2 |
| [x] | — | `SiteRaiseTerrainInRegionCommand` | [Site/siteraiseterraininregioncommand.md](docs/commands/Site/siteraiseterraininregioncommand.md) | — | `Site/CS/SiteRaiseTerrainInRegionCommand.cs` | 2 |
### SlabProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SlabProperties/slabproperties.md](docs/commands/SlabProperties/slabproperties.md) | [SlabProperties/slabproperties.json](docs/mcp/SlabProperties/slabproperties.json) | `SlabProperties/CS/Command.cs` | 4 |
### SlabShapeEditing (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SlabShapeEditing/slabshapeediting.md](docs/commands/SlabShapeEditing/slabshapeediting.md) | [SlabShapeEditing/slabshapeediting.json](docs/mcp/SlabShapeEditing/slabshapeediting.json) | `SlabShapeEditing/CS/Command.cs` | 4 |
### SolidSolidCut (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Cut` | [SolidSolidCut/cut.md](docs/commands/SolidSolidCut/cut.md) | — | `SolidSolidCut/CS/Command.cs` | 2 |
| [x] | — | `Uncut` | [SolidSolidCut/uncut.md](docs/commands/SolidSolidCut/uncut.md) | — | `SolidSolidCut/CS/Command.cs` | 2 |
### SpanDirection (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SpanDirection/spandirection.md](docs/commands/SpanDirection/spandirection.md) | [SpanDirection/spandirection.json](docs/mcp/SpanDirection/spandirection.json) | `SpanDirection/CS/Command.cs` | 4 |
### SpotDimension (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SpotDimension/spotdimension.md](docs/commands/SpotDimension/spotdimension.md) | [SpotDimension/spotdimension.json](docs/mcp/SpotDimension/spotdimension.json) | `SpotDimension/CS/Command.cs` | 4 |
### StairsAutomation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StairsAutomation/stairsautomation.md](docs/commands/StairsAutomation/stairsautomation.md) | [StairsAutomation/stairsautomation.json](docs/mcp/StairsAutomation/stairsautomation.json) | `StairsAutomation/CS/Command.cs` | 4 |
### StructSample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StructSample/structsample.md](docs/commands/StructSample/structsample.md) | [StructSample/structsample.json](docs/mcp/StructSample/structsample.json) | `StructSample/CS/Command.cs` | 4 |
### StructuralLayerFunction (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StructuralLayerFunction/structurallayerfunction.md](docs/commands/StructuralLayerFunction/structurallayerfunction.md) | [StructuralLayerFunction/structurallayerfunction.json](docs/mcp/StructuralLayerFunction/structurallayerfunction.json) | `StructuralLayerFunction/CS/Command.cs` | 3 |
### TagBeam (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [TagBeam/tagbeam.md](docs/commands/TagBeam/tagbeam.md) | [TagBeam/tagbeam.json](docs/mcp/TagBeam/tagbeam.json) | `TagBeam/CS/Command.cs` | 4 |
| [x] | [x] | `CreateText` | [TagBeam/createtext.md](docs/commands/TagBeam/createtext.md) | [TagBeam/createtext.json](docs/mcp/TagBeam/createtext.json) | `TagBeam/CS/Command.cs` | 4 |
| [x] | [x] | `TagRebar` | [TagBeam/tagrebar.md](docs/commands/TagBeam/tagrebar.md) | [TagBeam/tagrebar.json](docs/mcp/TagBeam/tagrebar.json) | `TagBeam/CS/Command.cs` | 4 |
### Toposolid (8)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ContourSettingCreation` | [Toposolid/contoursettingcreation.md](docs/commands/Toposolid/contoursettingcreation.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `ContourSettingModification` | [Toposolid/contoursettingmodification.md](docs/commands/Toposolid/contoursettingmodification.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `SimplifyToposolid` | [Toposolid/simplifytoposolid.md](docs/commands/Toposolid/simplifytoposolid.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `SplitToposolid` | [Toposolid/splittoposolid.md](docs/commands/Toposolid/splittoposolid.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `SsePointVisibility` | [Toposolid/ssepointvisibility.md](docs/commands/Toposolid/ssepointvisibility.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `ToposolidCreation` | [Toposolid/toposolidcreation.md](docs/commands/Toposolid/toposolidcreation.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `ToposolidFromDwg` | [Toposolid/toposolidfromdwg.md](docs/commands/Toposolid/toposolidfromdwg.md) | — | `Toposolid/CS/Command.cs` | 2 |
| [x] | — | `ToposolidFromSurface` | [Toposolid/toposolidfromsurface.md](docs/commands/Toposolid/toposolidfromsurface.md) | — | `Toposolid/CS/Command.cs` | 2 |
### TransactionControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [TransactionControl/transactioncontrol.md](docs/commands/TransactionControl/transactioncontrol.md) | — | `TransactionControl/CS/Command.cs` | 2 |
### TraverseSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [TraverseSystem/traversesystem.md](docs/commands/TraverseSystem/traversesystem.md) | — | `TraverseSystem/CS/Command.cs` | 2 |
### Truss (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Truss/truss.md](docs/commands/Truss/truss.md) | [Truss/truss.json](docs/mcp/Truss/truss.json) | `Truss/CS/Command.cs` | 4 |
### UIAPI (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CalcCommand` | [UIAPI/calccommand.md](docs/commands/UIAPI/calccommand.md) | — | `UIAPI/CS/ExternalApplication.cs` | 1 |
| [x] | — | `DragAndDropCommand` | [UIAPI/draganddropcommand.md](docs/commands/UIAPI/draganddropcommand.md) | — | `UIAPI/CS/DragAndDrop/DragAndDropCommand.cs` | 1 |
| [x] | — | `PreviewCommand` | [UIAPI/previewcommand.md](docs/commands/UIAPI/previewcommand.md) | — | `UIAPI/CS/PreviewControl/PreviewCommand.cs` | 1 |
### Units (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Units/units.md](docs/commands/Units/units.md) | [Units/units.json](docs/mcp/Units/units.json) | `Units/CS/Command.cs` | 5 |
### VersionChecking (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [VersionChecking/versionchecking.md](docs/commands/VersionChecking/versionchecking.md) | [VersionChecking/versionchecking.json](docs/mcp/VersionChecking/versionchecking.json) | `VersionChecking/CS/VersionChecking.cs` | 5 |
### ViewPrinter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ViewPrinter/viewprinter.md](docs/commands/ViewPrinter/viewprinter.md) | [ViewPrinter/viewprinter.json](docs/mcp/ViewPrinter/viewprinter.json) | `ViewPrinter/CS/Command.cs` | 4 |
### ViewTemplateCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ViewTemplateCreation/viewtemplatecreation.md](docs/commands/ViewTemplateCreation/viewtemplatecreation.md) | [ViewTemplateCreation/viewtemplatecreation.json](docs/mcp/ViewTemplateCreation/viewtemplatecreation.json) | `ViewTemplateCreation/CS/Command.cs` | 4 |
### VisibilityControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [VisibilityControl/visibilitycontrol.md](docs/commands/VisibilityControl/visibilitycontrol.md) | [VisibilityControl/visibilitycontrol.json](docs/mcp/VisibilityControl/visibilitycontrol.json) | `VisibilityControl/CS/Command.cs` | 4 |
### WinderStairs (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [WinderStairs/winderstairs.md](docs/commands/WinderStairs/winderstairs.md) | [WinderStairs/winderstairs.json](docs/mcp/WinderStairs/winderstairs.json) | `WinderStairs/CS/Command.cs` | 4 |

## Notes

- **Agent runbook:** full step-by-step instructions in [`docs/AGENTS.md`](docs/AGENTS.md).
- **SDK ReadMe:** `SampleData` resolves the first `*.rtf` in the command's sample folder; ~167 RTF files exist under `src/`.
- `CreateStructureWall` inherits `CreateWall` and is discovered by reflection but not listed separately.
- `IExternalCommandAvailability` classes (`WallSelection`, `View3D`, etc.) are excluded — not commands.
- MCP JSON files in `docs/mcp/` are **specifications** for future Bowerbird Revit tools, not live MCP server entries.
- Refresh progress counts after each session by recounting `[x]` in the inventory or re-running the inventory generator.

