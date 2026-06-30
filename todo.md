# Command Documentation & MCP Exposure Tracker

> Master checklist for command markdown docs and MCP tool descriptor JSON. **Agent instructions:** [`docs/AGENTS.md`](docs/AGENTS.md)

## Progress Summary

| Metric | Value |
|--------|-------|
| Total commands | 441 |
| Command docs created | 441 / 441 |
| MCP descriptors created | 283 / 283 |
| Last updated | 2026-06-30

### MCP rating distribution (preliminary)

- **5/5**: 34 commands — MCP JSON required
- **4/5**: 90 commands — MCP JSON required
- **3/5**: 7 commands — MCP JSON required
- **2/5**: 127 commands
- **1/5**: 31 commands

## Goals

1. Create one **concise** markdown file per `IExternalCommand` (see [`docs/AGENTS.md`](docs/AGENTS.md)).
2. For commands with **MCP rating = 3**, create an MCP tool descriptor JSON under `src/`.
3. Use each sample's **SDK ReadMe RTF** (`ReadMe_*.rtf` / `Readme_*.rtf` in the sample `CS` folder) as the primary reference.
4. Refine MCP ratings when writing docs; update both the doc and descriptor if the score changes.

## Per-command tasks

| Task | Output | Required |
|------|--------|----------|
| **A — Command doc** | `src/<Sample>/<slug>.md` | Always |
| **B — MCP descriptor** | `src/<Sample>/<slug>.json` | MCP rating = 3 only |

Templates: [`src/_template.md`](src/_template.md) · [`src/_template.json`](src/_template.json)

## Doc conventions

- **Length:** 150–300 words in the body; short paragraphs and bullets.
- **RTF:** consult `src/<Sample>/*.rtf` (or nested `src/<Sample>/<Sub>/*.rtf`); strip RTF markup, do not paste raw RTF.
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
| [x] | [x] | `Command` | [AddSpaceAndZone/addspaceandzone.md](src/AddSpaceAndZone/addspaceandzone.md) | [AddSpaceAndZone/addspaceandzone.json](src/AddSpaceAndZone/addspaceandzone.json) | `AddSpaceAndZone/Command.cs` | 4 |
### AllViews (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AllViews/allviews.md](src/AllViews/allviews.md) | [AllViews/allviews.json](src/AllViews/allviews.json) | `AllViews/AllViews.cs` | 5 |
### AnalysisVisualizationFramework (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `MultithreadedCalculation` | [AnalysisVisualizationFramework/MultithreadedCalculation/multithreadedcalculation.md](src/AnalysisVisualizationFramework/MultithreadedCalculation/multithreadedcalculation.md) | — | `AnalysisVisualizationFramework/MultithreadedCalculation/MultithreadedCalculation.cs` | 2 |
| [x] | — | `SpatialFieldGradient` | [AnalysisVisualizationFramework/SpatialFieldGradient/spatialfieldgradient.md](src/AnalysisVisualizationFramework/SpatialFieldGradient/spatialfieldgradient.md) | — | `AnalysisVisualizationFramework/SpatialFieldGradient/Command.cs` | 2 |
### AnalyticalSupportData_Info (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AnalyticalSupportData_Info/analyticalsupportdata-info.md](src/AnalyticalSupportData_Info/analyticalsupportdata-info.md) | [AnalyticalSupportData_Info/analyticalsupportdata-info.json](src/AnalyticalSupportData_Info/analyticalsupportdata-info.json) | `AnalyticalSupportData_Info/AnalyticalSupportData_Info.cs` | 5 |
### AppearanceAssetEditing (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AppearanceAssetEditing/appearanceassetediting.md](src/AppearanceAssetEditing/appearanceassetediting.md) | — | `AppearanceAssetEditing/Command.cs` | 2 |
### AreaReinCurve (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AreaReinCurve/areareincurve.md](src/AreaReinCurve/areareincurve.md) | — | `AreaReinCurve/AreaReinCurve.cs` | 2 |
### AreaReinParameters (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AreaReinParameters/areareinparameters.md](src/AreaReinParameters/areareinparameters.md) | — | `AreaReinParameters/AreaReinParameters.cs` | 2 |
| [x] | — | `RebarParas` | [AreaReinParameters/rebarparas.md](src/AreaReinParameters/rebarparas.md) | — | `AreaReinParameters/AreaReinParameters.cs` | 2 |
### AttachedDetailGroup (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AttachedDetailGroupHideAllCommand` | [AttachedDetailGroup/attacheddetailgrouphideallcommand.md](src/AttachedDetailGroup/attacheddetailgrouphideallcommand.md) | [AttachedDetailGroup/attacheddetailgrouphideallcommand.json](src/AttachedDetailGroup/attacheddetailgrouphideallcommand.json) | `AttachedDetailGroup/AttachedDetailGroupHideAllCommand.cs` | 4 |
| [x] | [x] | `AttachedDetailGroupShowAllCommand` | [AttachedDetailGroup/attacheddetailgroupshowallcommand.md](src/AttachedDetailGroup/attacheddetailgroupshowallcommand.md) | [AttachedDetailGroup/attacheddetailgroupshowallcommand.json](src/AttachedDetailGroup/attacheddetailgroupshowallcommand.json) | `AttachedDetailGroup/AttachedDetailGroupShowAllCommand.cs` | 4 |
### AutoRoute (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AutoRoute/autoroute.md](src/AutoRoute/autoroute.md) | — | `AutoRoute/Command.cs` | 2 |
### AutoTagRooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [AutoTagRooms/autotagrooms.md](src/AutoTagRooms/autotagrooms.md) | [AutoTagRooms/autotagrooms.json](src/AutoTagRooms/autotagrooms.json) | `AutoTagRooms/Command.cs` | 4 |
### AvoidObstruction (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [AvoidObstruction/avoidobstruction.md](src/AvoidObstruction/avoidobstruction.md) | — | `AvoidObstruction/Command.cs` | 2 |
### BeamAndSlabNewParameter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [BeamAndSlabNewParameter/beamandslabnewparameter.md](src/BeamAndSlabNewParameter/beamandslabnewparameter.md) | — | `BeamAndSlabNewParameter/BeamAndSlabNewParameter.cs` | 2 |
### BoundaryConditions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [BoundaryConditions/boundaryconditions.md](src/BoundaryConditions/boundaryconditions.md) | [BoundaryConditions/boundaryconditions.json](src/BoundaryConditions/boundaryconditions.json) | `BoundaryConditions/Command.cs` | 4 |
### CapitalizeAllTextNotes (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CapitalizeAllTextNotes/capitalizealltextnotes.md](src/CapitalizeAllTextNotes/capitalizealltextnotes.md) | [CapitalizeAllTextNotes/capitalizealltextnotes.json](src/CapitalizeAllTextNotes/capitalizealltextnotes.json) | `CapitalizeAllTextNotes/Command.cs` | 4 |
### CloudAPISample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `RunSampleCommand` | [CloudAPISample/runsamplecommand.md](src/CloudAPISample/runsamplecommand.md) | — | `CloudAPISample/Application.cs` | 1 |
### ColorFill (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ColorFill/colorfill.md](src/ColorFill/colorfill.md) | [ColorFill/colorfill.json](src/ColorFill/colorfill.json) | `ColorFill/Command.cs` | 4 |
### CompoundStructure (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `WallCompoundStructure` | [CompoundStructure/wallcompoundstructure.md](src/CompoundStructure/wallcompoundstructure.md) | [CompoundStructure/wallcompoundstructure.json](src/CompoundStructure/wallcompoundstructure.json) | `CompoundStructure/Command.cs` | 4 |
### ContextualAnalyticalModel (21)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddAssociation` | [ContextualAnalyticalModel/addassociation.md](src/ContextualAnalyticalModel/addassociation.md) | [ContextualAnalyticalModel/addassociation.json](src/ContextualAnalyticalModel/addassociation.json) | `ContextualAnalyticalModel/AddAssociation.cs` | 4 |
| [x] | [x] | `AddCustomAssociation` | [ContextualAnalyticalModel/addcustomassociation.md](src/ContextualAnalyticalModel/addcustomassociation.md) | [ContextualAnalyticalModel/addcustomassociation.json](src/ContextualAnalyticalModel/addcustomassociation.json) | `ContextualAnalyticalModel/AddCustomAssociation.cs` | 4 |
| [x] | — | `AnalyticalNodeConnStatus` | [ContextualAnalyticalModel/analyticalnodeconnstatus.md](src/ContextualAnalyticalModel/analyticalnodeconnstatus.md) | — | `ContextualAnalyticalModel/AnalyticalNodeConnStatus.cs` | 2 |
| [x] | [x] | `CreateAnalyticalCurvedPanel` | [ContextualAnalyticalModel/createanalyticalcurvedpanel.md](src/ContextualAnalyticalModel/createanalyticalcurvedpanel.md) | [ContextualAnalyticalModel/createanalyticalcurvedpanel.json](src/ContextualAnalyticalModel/createanalyticalcurvedpanel.json) | `ContextualAnalyticalModel/CreateAnalyticalCurvedPanel.cs` | 4 |
| [x] | [x] | `CreateAnalyticalMember` | [ContextualAnalyticalModel/createanalyticalmember.md](src/ContextualAnalyticalModel/createanalyticalmember.md) | [ContextualAnalyticalModel/createanalyticalmember.json](src/ContextualAnalyticalModel/createanalyticalmember.json) | `ContextualAnalyticalModel/CreateAnalyticalMember.cs` | 4 |
| [x] | [x] | `CreateAnalyticalPanel` | [ContextualAnalyticalModel/createanalyticalpanel.md](src/ContextualAnalyticalModel/createanalyticalpanel.md) | [ContextualAnalyticalModel/createanalyticalpanel.json](src/ContextualAnalyticalModel/createanalyticalpanel.json) | `ContextualAnalyticalModel/CreateAnalytcalPanel.cs` | 4 |
| [x] | [x] | `CreateAreaLoadWithRefPoint` | [ContextualAnalyticalModel/createarealoadwithrefpoint.md](src/ContextualAnalyticalModel/createarealoadwithrefpoint.md) | [ContextualAnalyticalModel/createarealoadwithrefpoint.json](src/ContextualAnalyticalModel/createarealoadwithrefpoint.json) | `ContextualAnalyticalModel/CreateAreaLoadWithRefPoint.cs` | 4 |
| [x] | [x] | `CreateCustomAreaLoad` | [ContextualAnalyticalModel/createcustomareaload.md](src/ContextualAnalyticalModel/createcustomareaload.md) | [ContextualAnalyticalModel/createcustomareaload.json](src/ContextualAnalyticalModel/createcustomareaload.json) | `ContextualAnalyticalModel/CustomAreaLoad.cs` | 4 |
| [x] | [x] | `CreateCustomLineLoad` | [ContextualAnalyticalModel/createcustomlineload.md](src/ContextualAnalyticalModel/createcustomlineload.md) | [ContextualAnalyticalModel/createcustomlineload.json](src/ContextualAnalyticalModel/createcustomlineload.json) | `ContextualAnalyticalModel/CustomLineLoad.cs` | 4 |
| [x] | [x] | `CreateCustomPointLoad` | [ContextualAnalyticalModel/createcustompointload.md](src/ContextualAnalyticalModel/createcustompointload.md) | [ContextualAnalyticalModel/createcustompointload.json](src/ContextualAnalyticalModel/createcustompointload.json) | `ContextualAnalyticalModel/CustomPointLoad.cs` | 4 |
| [x] | — | `FlipAnalyticalMember` | [ContextualAnalyticalModel/flipanalyticalmember.md](src/ContextualAnalyticalModel/flipanalyticalmember.md) | — | `ContextualAnalyticalModel/FlipAnalyticalMember.cs` | 2 |
| [x] | — | `MemberForcesAnalyticalMember` | [ContextualAnalyticalModel/memberforcesanalyticalmember.md](src/ContextualAnalyticalModel/memberforcesanalyticalmember.md) | — | `ContextualAnalyticalModel/MemberForcesAnalyticalMember.cs` | 2 |
| [x] | — | `ModifyPanelContour` | [ContextualAnalyticalModel/modifypanelcontour.md](src/ContextualAnalyticalModel/modifypanelcontour.md) | — | `ContextualAnalyticalModel/ModifyPanelContour.cs` | 2 |
| [x] | — | `MoveAnalyticalMemberUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalmemberusingelementtransformutils.md](src/ContextualAnalyticalModel/moveanalyticalmemberusingelementtransformutils.md) | — | `ContextualAnalyticalModel/MoveAnalyticalMemberUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalMemberUsingSetCurve` | [ContextualAnalyticalModel/moveanalyticalmemberusingsetcurve.md](src/ContextualAnalyticalModel/moveanalyticalmemberusingsetcurve.md) | — | `ContextualAnalyticalModel/MoveAnalyticalMemberUsingSetCurve.cs` | 2 |
| [x] | — | `MoveAnalyticalNodeUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalnodeusingelementtransformutils.md](src/ContextualAnalyticalModel/moveanalyticalnodeusingelementtransformutils.md) | — | `ContextualAnalyticalModel/MoveAnalyticalNodeUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalPanelUsingElementTransformUtils` | [ContextualAnalyticalModel/moveanalyticalpanelusingelementtransformutils.md](src/ContextualAnalyticalModel/moveanalyticalpanelusingelementtransformutils.md) | — | `ContextualAnalyticalModel/MoveAnalyticalPanelUsingElementTransformUtils.cs` | 2 |
| [x] | — | `MoveAnalyticalPanelUsingSketchEditScope` | [ContextualAnalyticalModel/moveanalyticalpanelusingsketcheditscope.md](src/ContextualAnalyticalModel/moveanalyticalpanelusingsketcheditscope.md) | — | `ContextualAnalyticalModel/MoveAnalyticalPanelUsingSketchEditScope.cs` | 2 |
| [x] | — | `ReleaseConditionsAnalyticalMember` | [ContextualAnalyticalModel/releaseconditionsanalyticalmember.md](src/ContextualAnalyticalModel/releaseconditionsanalyticalmember.md) | — | `ContextualAnalyticalModel/ReleaseConditionsAnalyticalMember.cs` | 2 |
| [x] | — | `RemoveAssociation` | [ContextualAnalyticalModel/removeassociation.md](src/ContextualAnalyticalModel/removeassociation.md) | — | `ContextualAnalyticalModel/RemoveAssociation.cs` | 2 |
| [x] | — | `SetOuterContourForPanels` | [ContextualAnalyticalModel/setoutercontourforpanels.md](src/ContextualAnalyticalModel/setoutercontourforpanels.md) | — | `ContextualAnalyticalModel/SetOuterContourForPanels.cs` | 2 |
### CreateBeamsColumnsBraces (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateBeamsColumnsBraces/createbeamscolumnsbraces.md](src/CreateBeamsColumnsBraces/createbeamscolumnsbraces.md) | [CreateBeamsColumnsBraces/createbeamscolumnsbraces.json](src/CreateBeamsColumnsBraces/createbeamscolumnsbraces.json) | `CreateBeamsColumnsBraces/CreateBeamsColumnsBraces.cs` | 4 |
### CreateBeamSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateBeamSystem/createbeamsystem.md](src/CreateBeamSystem/createbeamsystem.md) | [CreateBeamSystem/createbeamsystem.json](src/CreateBeamSystem/createbeamsystem.json) | `CreateBeamSystem/Command.cs` | 4 |
### CreateComplexAreaRein (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [CreateComplexAreaRein/createcomplexarearein.md](src/CreateComplexAreaRein/createcomplexarearein.md) | — | `CreateComplexAreaRein/CreateComplexAreaRein.cs` | 2 |
### CreateDimensions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateDimensions/createdimensions.md](src/CreateDimensions/createdimensions.md) | [CreateDimensions/createdimensions.json](src/CreateDimensions/createdimensions.json) | `CreateDimensions/Command.cs` | 4 |
### CreateDuctworkStiffener (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateDuctworkStiffener/createductworkstiffener.md](src/CreateDuctworkStiffener/createductworkstiffener.md) | [CreateDuctworkStiffener/createductworkstiffener.json](src/CreateDuctworkStiffener/createductworkstiffener.json) | `CreateDuctworkStiffener/Command.cs` | 3 |
### CreateFillPattern (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateFillPattern/createfillpattern.md](src/CreateFillPattern/createfillpattern.md) | [CreateFillPattern/createfillpattern.json](src/CreateFillPattern/createfillpattern.json) | `CreateFillPattern/Command.cs` | 4 |
### CreateSimpleAreaRein (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [CreateSimpleAreaRein/createsimplearearein.md](src/CreateSimpleAreaRein/createsimplearearein.md) | — | `CreateSimpleAreaRein/CreateSimpleAreaRein.cs` | 2 |
### CreateTrianglesTopography (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateTrianglesTopography/createtrianglestopography.md](src/CreateTrianglesTopography/createtrianglestopography.md) | [CreateTrianglesTopography/createtrianglestopography.json](src/CreateTrianglesTopography/createtrianglestopography.json) | `CreateTrianglesTopography/Command.cs` | 3 |
### CreateViewSection (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CreateViewSection/createviewsection.md](src/CreateViewSection/createviewsection.md) | [CreateViewSection/createviewsection.json](src/CreateViewSection/createviewsection.json) | `CreateViewSection/Command.cs` | 3 |
| [x] | [x] | `CreateDraftingView` | [CreateViewSection/createdraftingview.md](src/CreateViewSection/createdraftingview.md) | [CreateViewSection/createdraftingview.json](src/CreateViewSection/createdraftingview.json) | `CreateViewSection/Command.cs` | 4 |
### CreateWallinBeamProfile (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateWallinBeamProfile` | [CreateWallinBeamProfile/createwallinbeamprofile.md](src/CreateWallinBeamProfile/createwallinbeamprofile.md) | [CreateWallinBeamProfile/createwallinbeamprofile.json](src/CreateWallinBeamProfile/createwallinbeamprofile.json) | `CreateWallinBeamProfile/CreateWallinBeamProfile.cs` | 4 |
### CreateWallsUnderBeams (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateWallsUnderBeams` | [CreateWallsUnderBeams/createwallsunderbeams.md](src/CreateWallsUnderBeams/createwallsunderbeams.md) | [CreateWallsUnderBeams/createwallsunderbeams.json](src/CreateWallsUnderBeams/createwallsunderbeams.json) | `CreateWallsUnderBeams/CreateWallsUnderBeams.cs` | 4 |
### CurtainSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurtainSystem/curtainsystem.md](src/CurtainSystem/curtainsystem.md) | [CurtainSystem/curtainsystem.json](src/CurtainSystem/curtainsystem.json) | `CurtainSystem/Command.cs` | 4 |
### CurtainWallGrid (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurtainWallGrid/curtainwallgrid.md](src/CurtainWallGrid/curtainwallgrid.md) | [CurtainWallGrid/curtainwallgrid.json](src/CurtainWallGrid/curtainwallgrid.json) | `CurtainWallGrid/Command.cs` | 4 |
### CurvedBeam (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CurvedBeam/curvedbeam.md](src/CurvedBeam/curvedbeam.md) | [CurvedBeam/curvedbeam.json](src/CurvedBeam/curvedbeam.json) | `CurvedBeam/CurvedBeam.cs` | 4 |
### CustomExporter (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.md](src/CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.md) | [CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.json](src/CustomExporter/AdnMeshJsonExporter/adnmeshjsonexporter.json) | `CustomExporter/AdnMeshJsonExporter/Command.cs` | 5 |
| [x] | [x] | `Command` | [CustomExporter/Custom2DExporter/custom2dexporter.md](src/CustomExporter/Custom2DExporter/custom2dexporter.md) | [CustomExporter/Custom2DExporter/custom2dexporter.json](src/CustomExporter/Custom2DExporter/custom2dexporter.json) | `CustomExporter/Custom2DExporter/Command.cs` | 5 |
### DatumsModification (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `DatumAlignment` | [DatumsModification/datumalignment.md](src/DatumsModification/datumalignment.md) | — | `DatumsModification/DatumsModificationCmd.cs` | 2 |
| [x] | — | `DatumPropagation` | [DatumsModification/datumpropagation.md](src/DatumsModification/datumpropagation.md) | — | `DatumsModification/DatumsModificationCmd.cs` | 2 |
| [x] | — | `DatumStyleModification` | [DatumsModification/datumstylemodification.md](src/DatumsModification/datumstylemodification.md) | — | `DatumsModification/DatumsModificationCmd.cs` | 2 |
### DeckProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeckProperties/deckproperties.md](src/DeckProperties/deckproperties.md) | [DeckProperties/deckproperties.json](src/DeckProperties/deckproperties.json) | `DeckProperties/Command.cs` | 4 |
### DeleteDimensions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeleteDimensions/deletedimensions.md](src/DeleteDimensions/deletedimensions.md) | [DeleteDimensions/deletedimensions.json](src/DeleteDimensions/deletedimensions.json) | `DeleteDimensions/DeleteDimesions.cs` | 4 |
### DeleteObject (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [DeleteObject/deleteobject.md](src/DeleteObject/deleteobject.md) | [DeleteObject/deleteobject.json](src/DeleteObject/deleteobject.json) | `DeleteObject/DeleteObject.cs` | 4 |
### DimensionLeaderEnd (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `MoveHorizontally` | [DimensionLeaderEnd/movehorizontally.md](src/DimensionLeaderEnd/movehorizontally.md) | [DimensionLeaderEnd/movehorizontally.json](src/DimensionLeaderEnd/movehorizontally.json) | `DimensionLeaderEnd/Command.cs` | 3 |
| [x] | [x] | `MoveToPickedPoint` | [DimensionLeaderEnd/movetopickedpoint.md](src/DimensionLeaderEnd/movetopickedpoint.md) | [DimensionLeaderEnd/movetopickedpoint.json](src/DimensionLeaderEnd/movetopickedpoint.json) | `DimensionLeaderEnd/Command.cs` | 3 |
### DirectionCalculation (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `FindSouthFacingWallsWithoutProjectLocation` | [DirectionCalculation/findsouthfacingwallswithoutprojectlocation.md](src/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.md) | [DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json](src/DirectionCalculation/findsouthfacingwallswithoutprojectlocation.json) | `DirectionCalculation/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWallsWithProjectLocation` | [DirectionCalculation/findsouthfacingwallswithprojectlocation.md](src/DirectionCalculation/findsouthfacingwallswithprojectlocation.md) | [DirectionCalculation/findsouthfacingwallswithprojectlocation.json](src/DirectionCalculation/findsouthfacingwallswithprojectlocation.json) | `DirectionCalculation/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWindowsWithoutProjectLocation` | [DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.md](src/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.md) | [DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json](src/DirectionCalculation/findsouthfacingwindowswithoutprojectlocation.json) | `DirectionCalculation/Commands.cs` | 5 |
| [x] | [x] | `FindSouthFacingWindowsWithProjectLocation` | [DirectionCalculation/findsouthfacingwindowswithprojectlocation.md](src/DirectionCalculation/findsouthfacingwindowswithprojectlocation.md) | [DirectionCalculation/findsouthfacingwindowswithprojectlocation.json](src/DirectionCalculation/findsouthfacingwindowswithprojectlocation.json) | `DirectionCalculation/Commands.cs` | 5 |
### DisplacementElementAnimation (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `DisplacementStructureModelAnimatorCommand` | [DisplacementElementAnimation/displacementstructuremodelanimatorcommand.md](src/DisplacementElementAnimation/displacementstructuremodelanimatorcommand.md) | — | `DisplacementElementAnimation/DisplacementStructureModelAnimatorCommand.cs` | 1 |
| [x] | — | `DisplacementStructureModelAnimatorCommandStepByStep` | [DisplacementElementAnimation/displacementstructuremodelanimatorcommandstepbystep.md](src/DisplacementElementAnimation/displacementstructuremodelanimatorcommandstepbystep.md) | — | `DisplacementElementAnimation/DisplacementStructureModelAnimatorCommand.cs` | 1 |
### DockableDialogs (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ExternalCommandHidePage` | [DockableDialogs/externalcommandhidepage.md](src/DockableDialogs/externalcommandhidepage.md) | — | `DockableDialogs/TopLevelCommands/ExternalCommandHidePage.cs` | 1 |
| [x] | — | `ExternalCommandRegisterPage` | [DockableDialogs/externalcommandregisterpage.md](src/DockableDialogs/externalcommandregisterpage.md) | — | `DockableDialogs/TopLevelCommands/ExternalCommandRegisterPage.cs` | 1 |
| [x] | — | `ExternalCommandShowPage` | [DockableDialogs/externalcommandshowpage.md](src/DockableDialogs/externalcommandshowpage.md) | — | `DockableDialogs/TopLevelCommands/ExternalCommandShowPage.cs` | 1 |
### DocumentChanged (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [DocumentChanged/documentchanged.md](src/DocumentChanged/documentchanged.md) | — | `DocumentChanged/ChangesMonitor.cs` | 1 |
### DoorSwing (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `InitializeCommand` | [DoorSwing/initializecommand.md](src/DoorSwing/initializecommand.md) | — | `DoorSwing/Command.cs` | 2 |
| [x] | — | `UpdateGeometryCommand` | [DoorSwing/updategeometrycommand.md](src/DoorSwing/updategeometrycommand.md) | — | `DoorSwing/Command.cs` | 2 |
| [x] | — | `UpdateParamsCommand` | [DoorSwing/updateparamscommand.md](src/DoorSwing/updateparamscommand.md) | — | `DoorSwing/Command.cs` | 2 |
### DuplicateGraphics (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CommandClearExternalGraphics` | [DuplicateGraphics/commandclearexternalgraphics.md](src/DuplicateGraphics/commandclearexternalgraphics.md) | — | `DuplicateGraphics/Command.cs` | 2 |
| [x] | [x] | `CommandDuplicateGraphics` | [DuplicateGraphics/commandduplicategraphics.md](src/DuplicateGraphics/commandduplicategraphics.md) | [DuplicateGraphics/commandduplicategraphics.json](src/DuplicateGraphics/commandduplicategraphics.json) | `DuplicateGraphics/Command.cs` | 4 |
### DuplicateViews (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DuplicateAcrossDocumentsCommand` | [DuplicateViews/duplicateacrossdocumentscommand.md](src/DuplicateViews/duplicateacrossdocumentscommand.md) | [DuplicateViews/duplicateacrossdocumentscommand.json](src/DuplicateViews/duplicateacrossdocumentscommand.json) | `DuplicateViews/DuplicateAcrossDocumentsCommand.cs` | 4 |
### DynamicModelUpdate (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `AssociativeSectionUpdater` | [DynamicModelUpdate/associativesectionupdater.md](src/DynamicModelUpdate/associativesectionupdater.md) | — | `DynamicModelUpdate/Application.cs` | 2 |
### ElementFilterSample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ElementFilterSample/elementfiltersample.md](src/ElementFilterSample/elementfiltersample.md) | [ElementFilterSample/elementfiltersample.json](src/ElementFilterSample/elementfiltersample.json) | `ElementFilterSample/Command.cs` | 5 |
### ErrorHandling (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ErrorHandling/errorhandling.md](src/ErrorHandling/errorhandling.md) | — | `ErrorHandling/Command.cs` | 1 |
### Events (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Events/EventsMonitor/eventsmonitor.md](src/Events/EventsMonitor/eventsmonitor.md) | — | `Events/EventsMonitor/Command.cs` | 1 |
| [x] | — | `Command` | [Events/PrintLog/printlog.md](src/Events/PrintLog/printlog.md) | — | `Events/PrintLog/Command.cs` | 1 |
| [x] | — | `Command` | [Events/ProgressNotifier/progressnotifier.md](src/Events/ProgressNotifier/progressnotifier.md) | — | `Events/ProgressNotifier/Command.cs` | 1 |
| [x] | — | `Command` | [Events/SelectionChanged/selectionchanged.md](src/Events/SelectionChanged/selectionchanged.md) | — | `Events/SelectionChanged/Command.cs` | 1 |
### ExportPDFSettingsSample (5)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddNamingRuleCommand` | [ExportPDFSettingsSample/addnamingrulecommand.md](src/ExportPDFSettingsSample/addnamingrulecommand.md) | [ExportPDFSettingsSample/addnamingrulecommand.json](src/ExportPDFSettingsSample/addnamingrulecommand.json) | `ExportPDFSettingsSample/Application.cs` | 4 |
| [x] | [x] | `CreateExportPdfSettingsCommand` | [ExportPDFSettingsSample/createexportpdfsettingscommand.md](src/ExportPDFSettingsSample/createexportpdfsettingscommand.md) | [ExportPDFSettingsSample/createexportpdfsettingscommand.json](src/ExportPDFSettingsSample/createexportpdfsettingscommand.json) | `ExportPDFSettingsSample/Application.cs` | 5 |
| [x] | [x] | `DeleteNamingRuleCommand` | [ExportPDFSettingsSample/deletenamingrulecommand.md](src/ExportPDFSettingsSample/deletenamingrulecommand.md) | [ExportPDFSettingsSample/deletenamingrulecommand.json](src/ExportPDFSettingsSample/deletenamingrulecommand.json) | `ExportPDFSettingsSample/Application.cs` | 4 |
| [x] | [x] | `ModifyExportPdfSettingsCommand` | [ExportPDFSettingsSample/modifyexportpdfsettingscommand.md](src/ExportPDFSettingsSample/modifyexportpdfsettingscommand.md) | [ExportPDFSettingsSample/modifyexportpdfsettingscommand.json](src/ExportPDFSettingsSample/modifyexportpdfsettingscommand.json) | `ExportPDFSettingsSample/Application.cs` | 5 |
| [x] | — | `MofidyNamingRuleCommand` | [ExportPDFSettingsSample/mofidynamingrulecommand.md](src/ExportPDFSettingsSample/mofidynamingrulecommand.md) | — | `ExportPDFSettingsSample/Application.cs` | 2 |
### ExtensibleStorageManager (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ExtensibleStorageManager/ExtensibleStorageManager/extensiblestoragemanager.md](src/ExtensibleStorageManager/ExtensibleStorageManager/extensiblestoragemanager.md) | — | `ExtensibleStorageManager/ExtensibleStorageManager/Application/Command.cs` | 2 |
### ExtensibleStorageUtility (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DeleteStorage` | [ExtensibleStorageUtility/deletestorage.md](src/ExtensibleStorageUtility/deletestorage.md) | [ExtensibleStorageUtility/deletestorage.json](src/ExtensibleStorageUtility/deletestorage.json) | `ExtensibleStorageUtility/DeleteStorage.cs` | 4 |
| [x] | [x] | `QueryStorage` | [ExtensibleStorageUtility/querystorage.md](src/ExtensibleStorageUtility/querystorage.md) | [ExtensibleStorageUtility/querystorage.json](src/ExtensibleStorageUtility/querystorage.json) | `ExtensibleStorageUtility/QueryStorage.cs` | 5 |
### ExternalCommand (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ExternalCommand3DView` | [ExternalCommand/externalcommand3dview.md](src/ExternalCommand/externalcommand3dview.md) | — | `ExternalCommand/ExternalCommandRegistration/ExternalCommandClass.cs` | 1 |
| [x] | — | `ExternalCommandCreateWall` | [ExternalCommand/externalcommandcreatewall.md](src/ExternalCommand/externalcommandcreatewall.md) | — | `ExternalCommand/ExternalCommandRegistration/ExternalCommandClass.cs` | 1 |
### FabricationPartLayout (25)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Ancillaries` | [FabricationPartLayout/ancillaries.md](src/FabricationPartLayout/ancillaries.md) | — | `FabricationPartLayout/Ancillaries.cs` | 2 |
| [x] | — | `ApplyChange` | [FabricationPartLayout/applychange.md](src/FabricationPartLayout/applychange.md) | — | `FabricationPartLayout/ChangeService.cs` | 2 |
| [x] | — | `ButtonPaletteExclusions` | [FabricationPartLayout/buttonpaletteexclusions.md](src/FabricationPartLayout/buttonpaletteexclusions.md) | — | `FabricationPartLayout/ButtonPaletteExclusions.cs` | 2 |
| [x] | — | `ChangeService` | [FabricationPartLayout/changeservice.md](src/FabricationPartLayout/changeservice.md) | — | `FabricationPartLayout/ChangeService.cs` | 2 |
| [x] | — | `ChangeSize` | [FabricationPartLayout/changesize.md](src/FabricationPartLayout/changesize.md) | — | `FabricationPartLayout/ChangeService.cs` | 2 |
| [x] | — | `ConvertToFabrication` | [FabricationPartLayout/converttofabrication.md](src/FabricationPartLayout/converttofabrication.md) | — | `FabricationPartLayout/ConvertToFabrication.cs` | 2 |
| [x] | — | `DecreaseRodStructureExtension` | [FabricationPartLayout/decreaserodstructureextension.md](src/FabricationPartLayout/decreaserodstructureextension.md) | — | `FabricationPartLayout/HangerRods.cs` | 2 |
| [x] | — | `DetachRods` | [FabricationPartLayout/detachrods.md](src/FabricationPartLayout/detachrods.md) | — | `FabricationPartLayout/HangerRods.cs` | 2 |
| [x] | — | `DoubleRodLength` | [FabricationPartLayout/doublerodlength.md](src/FabricationPartLayout/doublerodlength.md) | — | `FabricationPartLayout/HangerRods.cs` | 2 |
| [x] | [x] | `ExportToMaj` | [FabricationPartLayout/exporttomaj.md](src/FabricationPartLayout/exporttomaj.md) | [FabricationPartLayout/exporttomaj.json](src/FabricationPartLayout/exporttomaj.json) | `FabricationPartLayout/ExportToMAJ.cs` | 5 |
| [x] | [x] | `ExportToPcf` | [FabricationPartLayout/exporttopcf.md](src/FabricationPartLayout/exporttopcf.md) | [FabricationPartLayout/exporttopcf.json](src/FabricationPartLayout/exporttopcf.json) | `FabricationPartLayout/ExportToPCF.cs` | 5 |
| [x] | — | `FabPartGeometry` | [FabricationPartLayout/fabpartgeometry.md](src/FabricationPartLayout/fabpartgeometry.md) | — | `FabricationPartLayout/FabPartGeometry.cs` | 2 |
| [x] | — | `FabricationPartLayout` | [FabricationPartLayout/fabricationpartlayout.md](src/FabricationPartLayout/fabricationpartlayout.md) | — | `FabricationPartLayout/FabricationPartLayout.cs` | 2 |
| [x] | — | `FlipPart` | [FabricationPartLayout/flippart.md](src/FabricationPartLayout/flippart.md) | — | `FabricationPartLayout/FlipPart.cs` | 2 |
| [x] | — | `GetCustomData` | [FabricationPartLayout/getcustomdata.md](src/FabricationPartLayout/getcustomdata.md) | — | `FabricationPartLayout/CustomData.cs` | 2 |
| [x] | — | `HalveRodLength` | [FabricationPartLayout/halverodlength.md](src/FabricationPartLayout/halverodlength.md) | — | `FabricationPartLayout/HangerRods.cs` | 2 |
| [x] | — | `IncreaseRodStructureExtension` | [FabricationPartLayout/increaserodstructureextension.md](src/FabricationPartLayout/increaserodstructureextension.md) | — | `FabricationPartLayout/HangerRods.cs` | 2 |
| [x] | — | `LoadAndPlaceNextItemFile` | [FabricationPartLayout/loadandplacenextitemfile.md](src/FabricationPartLayout/loadandplacenextitemfile.md) | — | `FabricationPartLayout/ItemFile.cs` | 2 |
| [x] | — | `OptimizeStraights` | [FabricationPartLayout/optimizestraights.md](src/FabricationPartLayout/optimizestraights.md) | — | `FabricationPartLayout/OptimizeStraights.cs` | 2 |
| [x] | [x] | `PartInfo` | [FabricationPartLayout/partinfo.md](src/FabricationPartLayout/partinfo.md) | [FabricationPartLayout/partinfo.json](src/FabricationPartLayout/partinfo.json) | `FabricationPartLayout/PartInfo.cs` | 5 |
| [x] | — | `PartRenumber` | [FabricationPartLayout/partrenumber.md](src/FabricationPartLayout/partrenumber.md) | — | `FabricationPartLayout/PartRenumber.cs` | 2 |
| [x] | — | `SetCustomData` | [FabricationPartLayout/setcustomdata.md](src/FabricationPartLayout/setcustomdata.md) | — | `FabricationPartLayout/CustomData.cs` | 2 |
| [x] | — | `SplitStraight` | [FabricationPartLayout/splitstraight.md](src/FabricationPartLayout/splitstraight.md) | — | `FabricationPartLayout/SplitStraight.cs` | 2 |
| [x] | — | `StretchAndFit` | [FabricationPartLayout/stretchandfit.md](src/FabricationPartLayout/stretchandfit.md) | — | `FabricationPartLayout/StretchAndFit.cs` | 2 |
| [x] | — | `UnloadUnusedItemFiles` | [FabricationPartLayout/unloadunuseditemfiles.md](src/FabricationPartLayout/unloadunuseditemfiles.md) | — | `FabricationPartLayout/ItemFile.cs` | 2 |
### FamilyCreation (10)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [FamilyCreation/AutoJoin/autojoin.md](src/FamilyCreation/AutoJoin/autojoin.md) | — | `FamilyCreation/AutoJoin/Command.cs` | 2 |
| [x] | [x] | `AddParameterToFamilies` | [FamilyCreation/AutoParameter/addparametertofamilies.md](src/FamilyCreation/AutoParameter/addparametertofamilies.md) | [FamilyCreation/AutoParameter/addparametertofamilies.json](src/FamilyCreation/AutoParameter/addparametertofamilies.json) | `FamilyCreation/AutoParameter/Command.cs` | 4 |
| [x] | [x] | `AddParameterToFamily` | [FamilyCreation/AutoParameter/addparametertofamily.md](src/FamilyCreation/AutoParameter/addparametertofamily.md) | [FamilyCreation/AutoParameter/addparametertofamily.json](src/FamilyCreation/AutoParameter/addparametertofamily.json) | `FamilyCreation/AutoParameter/Command.cs` | 4 |
| [x] | — | `Command` | [FamilyCreation/CreateAirHandler/createairhandler.md](src/FamilyCreation/CreateAirHandler/createairhandler.md) | — | `FamilyCreation/CreateAirHandler/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/CreateTruss/createtruss.md](src/FamilyCreation/CreateTruss/createtruss.md) | — | `FamilyCreation/CreateTruss/CreateTruss.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/DWGFamilyCreation/dwgfamilycreation.md](src/FamilyCreation/DWGFamilyCreation/dwgfamilycreation.md) | — | `FamilyCreation/DWGFamilyCreation/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/GenericModelCreation/genericmodelcreation.md](src/FamilyCreation/GenericModelCreation/genericmodelcreation.md) | — | `FamilyCreation/GenericModelCreation/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/TypeRegeneration/typeregeneration.md](src/FamilyCreation/TypeRegeneration/typeregeneration.md) | — | `FamilyCreation/TypeRegeneration/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/ValidateParameters/validateparameters.md](src/FamilyCreation/ValidateParameters/validateparameters.md) | — | `FamilyCreation/ValidateParameters/Command.cs` | 2 |
| [x] | — | `Command` | [FamilyCreation/WindowWizard/windowwizard.md](src/FamilyCreation/WindowWizard/windowwizard.md) | — | `FamilyCreation/WindowWizard/Command.cs` | 2 |
### FamilyParametersOrder (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [FamilyParametersOrder/familyparametersorder.md](src/FamilyParametersOrder/familyparametersorder.md) | — | `FamilyParametersOrder/Command.cs` | 2 |
### FindReferencesByDirection (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FindReferencesByDirection/FindColumns/findcolumns.md](src/FindReferencesByDirection/FindColumns/findcolumns.md) | [FindReferencesByDirection/FindColumns/findcolumns.json](src/FindReferencesByDirection/FindColumns/findcolumns.json) | `FindReferencesByDirection/FindColumns/FindColumns.cs` | 5 |
| [x] | [x] | `Command` | [FindReferencesByDirection/MeasureHeight/measureheight.md](src/FindReferencesByDirection/MeasureHeight/measureheight.md) | [FindReferencesByDirection/MeasureHeight/measureheight.json](src/FindReferencesByDirection/MeasureHeight/measureheight.json) | `FindReferencesByDirection/MeasureHeight/MeasureHeight.cs` | 5 |
| [x] | [x] | `Command` | [FindReferencesByDirection/RaytraceBounce/raytracebounce.md](src/FindReferencesByDirection/RaytraceBounce/raytracebounce.md) | [FindReferencesByDirection/RaytraceBounce/raytracebounce.json](src/FindReferencesByDirection/RaytraceBounce/raytracebounce.json) | `FindReferencesByDirection/RaytraceBounce/RayTraceBounce.cs` | 5 |
### FoundationSlab (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FoundationSlab/foundationslab.md](src/FoundationSlab/foundationslab.md) | [FoundationSlab/foundationslab.json](src/FoundationSlab/foundationslab.json) | `FoundationSlab/Command.cs` | 4 |
### FrameBuilder (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [FrameBuilder/framebuilder.md](src/FrameBuilder/framebuilder.md) | [FrameBuilder/framebuilder.json](src/FrameBuilder/framebuilder.json) | `FrameBuilder/Command.cs` | 4 |
### FreeFormElement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateNegativeBlockCommand` | [FreeFormElement/createnegativeblockcommand.md](src/FreeFormElement/createnegativeblockcommand.md) | [FreeFormElement/createnegativeblockcommand.json](src/FreeFormElement/createnegativeblockcommand.json) | `FreeFormElement/CreateNegativeBlockCommand.cs` | 4 |
### GenerateFloor (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [GenerateFloor/generatefloor.md](src/GenerateFloor/generatefloor.md) | [GenerateFloor/generatefloor.json](src/GenerateFloor/generatefloor.json) | `GenerateFloor/Command.cs` | 4 |
### GenericStructuralConnection (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [GenericStructuralConnection/genericstructuralconnection.md](src/GenericStructuralConnection/genericstructuralconnection.md) | — | `GenericStructuralConnection/Command.cs` | 2 |
### GeometryAPI (9)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CreateCube` | [GeometryAPI/BRepBuilderExample/createcube.md](src/GeometryAPI/BRepBuilderExample/createcube.md) | [GeometryAPI/BRepBuilderExample/createcube.json](src/GeometryAPI/BRepBuilderExample/createcube.json) | `GeometryAPI/BRepBuilderExample/CreateCube.cs` | 4 |
| [x] | [x] | `CreateNurbs` | [GeometryAPI/BRepBuilderExample/createnurbs.md](src/GeometryAPI/BRepBuilderExample/createnurbs.md) | [GeometryAPI/BRepBuilderExample/createnurbs.json](src/GeometryAPI/BRepBuilderExample/createnurbs.json) | `GeometryAPI/BRepBuilderExample/CreateNURBS.cs` | 4 |
| [x] | [x] | `CreatePeriodic` | [GeometryAPI/BRepBuilderExample/createperiodic.md](src/GeometryAPI/BRepBuilderExample/createperiodic.md) | [GeometryAPI/BRepBuilderExample/createperiodic.json](src/GeometryAPI/BRepBuilderExample/createperiodic.json) | `GeometryAPI/BRepBuilderExample/CreatePeriodic.cs` | 4 |
| [x] | — | `Command` | [GeometryAPI/ComputedSymbolGeometry/computedsymbolgeometry.md](src/GeometryAPI/ComputedSymbolGeometry/computedsymbolgeometry.md) | — | `GeometryAPI/ComputedSymbolGeometry/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/EnergyAnalysisModel/energyanalysismodel.md](src/GeometryAPI/EnergyAnalysisModel/energyanalysismodel.md) | — | `GeometryAPI/EnergyAnalysisModel/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/GeometryCreation_BooleanOperation/geometrycreation-booleanoperation.md](src/GeometryAPI/GeometryCreation_BooleanOperation/geometrycreation-booleanoperation.md) | — | `GeometryAPI/GeometryCreation_BooleanOperation/Command.cs` | 2 |
| [x] | — | `Command` | [GeometryAPI/ProximityDetection_WallJoinControl/proximitydetection-walljoincontrol.md](src/GeometryAPI/ProximityDetection_WallJoinControl/proximitydetection-walljoincontrol.md) | — | `GeometryAPI/ProximityDetection_WallJoinControl/Command.cs` | 2 |
| [x] | [x] | `CreateBRep` | [GeometryAPI/UpdateExternallyTaggedBRep/createbrep.md](src/GeometryAPI/UpdateExternallyTaggedBRep/createbrep.md) | [GeometryAPI/UpdateExternallyTaggedBRep/createbrep.json](src/GeometryAPI/UpdateExternallyTaggedBRep/createbrep.json) | `GeometryAPI/UpdateExternallyTaggedBRep/CreateBRep.cs` | 4 |
| [x] | — | `UpdateBRep` | [GeometryAPI/UpdateExternallyTaggedBRep/updatebrep.md](src/GeometryAPI/UpdateExternallyTaggedBRep/updatebrep.md) | — | `GeometryAPI/UpdateExternallyTaggedBRep/UpdateBRep.cs` | 2 |
### GetSetDefaultTypes (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ThisCommand` | [GetSetDefaultTypes/thiscommand.md](src/GetSetDefaultTypes/thiscommand.md) | [GetSetDefaultTypes/thiscommand.json](src/GetSetDefaultTypes/thiscommand.json) | `GetSetDefaultTypes/ThisCommand.cs` | 4 |
### GetElementImage (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [GetElementImage/get-element-image.md](src/GetElementImage/get-element-image.md) | [GetElementImage/get-element-image.json](src/GetElementImage/get-element-image.json) | `GetElementImage/Command.cs` | 4 |
### GridCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [GridCreation/gridcreation.md](src/GridCreation/gridcreation.md) | [GridCreation/gridcreation.json](src/GridCreation/gridcreation.json) | `GridCreation/Command.cs` | 4 |
### HelloRevit (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [HelloRevit/hellorevit.md](src/HelloRevit/hellorevit.md) | — | `HelloRevit/Command.cs` | 2 |
### Host (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `SampleBrowserCommand` | [Host/samplebrowsercommand.md](src/Host/samplebrowsercommand.md) | — | `SampleBrowserCommand.cs` | 1 |
### ImportExport (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ImportExport/importexport.md](src/ImportExport/importexport.md) | [ImportExport/importexport.json](src/ImportExport/importexport.json) | `ImportExport/Command.cs` | 5 |
### InCanvasControlAPI (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InCanvasControlAPI/incanvascontrolapi.md](src/InCanvasControlAPI/incanvascontrolapi.md) | — | `InCanvasControlAPI/Command.cs` | 1 |
### InPlaceMembers (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InPlaceMembers/inplacemembers.md](src/InPlaceMembers/inplacemembers.md) | — | `InPlaceMembers/Command.cs` | 2 |
### InvisibleParam (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [InvisibleParam/invisibleparam.md](src/InvisibleParam/invisibleparam.md) | — | `InvisibleParam/Command.cs` | 2 |
### Journaling (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Journaling/journaling.md](src/Journaling/journaling.md) | — | `Journaling/Command.cs` | 1 |
### LevelsProperty (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [LevelsProperty/levelsproperty.md](src/LevelsProperty/levelsproperty.md) | [LevelsProperty/levelsproperty.json](src/LevelsProperty/levelsproperty.json) | `LevelsProperty/Command.cs` | 5 |
### Loads (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Loads` | [Loads/loads.md](src/Loads/loads.md) | [Loads/loads.json](src/Loads/loads.json) | `Loads/Loads.cs` | 4 |
### Massing (17)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `SetDistanceParam` | [Massing/DistanceToPanels/setdistanceparam.md](src/Massing/DistanceToPanels/setdistanceparam.md) | — | `Massing/DistanceToPanels/Command.cs` | 2 |
| [x] | — | `Command` | [Massing/DividedSurfaceByIntersects/dividedsurfacebyintersects.md](src/Massing/DividedSurfaceByIntersects/dividedsurfacebyintersects.md) | — | `Massing/DividedSurfaceByIntersects/Command.cs` | 2 |
| [x] | — | `Command` | [Massing/ManipulateForm/manipulateform.md](src/Massing/ManipulateForm/manipulateform.md) | — | `Massing/ManipulateForm/Command.cs` | 2 |
| [x] | [x] | `MeasurePanelArea` | [Massing/MeasurePanelArea/measurepanelarea.md](src/Massing/MeasurePanelArea/measurepanelarea.md) | [Massing/MeasurePanelArea/measurepanelarea.json](src/Massing/MeasurePanelArea/measurepanelarea.json) | `Massing/MeasurePanelArea/Command.cs` | 5 |
| [x] | — | `MakeCapForm` | [Massing/NewForm/makecapform.md](src/Massing/NewForm/makecapform.md) | — | `Massing/NewForm/Command.cs` | 2 |
| [x] | — | `MakeExtrusionForm` | [Massing/NewForm/makeextrusionform.md](src/Massing/NewForm/makeextrusionform.md) | — | `Massing/NewForm/Command.cs` | 2 |
| [x] | — | `MakeLoftForm` | [Massing/NewForm/makeloftform.md](src/Massing/NewForm/makeloftform.md) | — | `Massing/NewForm/Command.cs` | 2 |
| [x] | — | `MakeRevolveForm` | [Massing/NewForm/makerevolveform.md](src/Massing/NewForm/makerevolveform.md) | — | `Massing/NewForm/Command.cs` | 2 |
| [x] | — | `MakeSweptBlendForm` | [Massing/NewForm/makesweptblendform.md](src/Massing/NewForm/makesweptblendform.md) | — | `Massing/NewForm/Command.cs` | 2 |
| [x] | — | `SetLengthAngleParams` | [Massing/PanelEdgeLengthAngle/setlengthangleparams.md](src/Massing/PanelEdgeLengthAngle/setlengthangleparams.md) | — | `Massing/PanelEdgeLengthAngle/Command.cs` | 2 |
| [x] | — | `SetParameterValueWithImageData` | [Massing/ParameterValuesFromImage/setparametervaluewithimagedata.md](src/Massing/ParameterValuesFromImage/setparametervaluewithimagedata.md) | — | `Massing/ParameterValuesFromImage/Command.cs` | 2 |
| [x] | — | `CatenaryCurve` | [Massing/PointCurveCreation/catenarycurve.md](src/Massing/PointCurveCreation/catenarycurve.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
| [x] | — | `CyclicSurface` | [Massing/PointCurveCreation/cyclicsurface.md](src/Massing/PointCurveCreation/cyclicsurface.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
| [x] | — | `PointsFromTextFile` | [Massing/PointCurveCreation/pointsfromtextfile.md](src/Massing/PointCurveCreation/pointsfromtextfile.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
| [x] | — | `PointsOnCurve` | [Massing/PointCurveCreation/pointsoncurve.md](src/Massing/PointCurveCreation/pointsoncurve.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
| [x] | — | `PointsParabola` | [Massing/PointCurveCreation/pointsparabola.md](src/Massing/PointCurveCreation/pointsparabola.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
| [x] | — | `SineCurve` | [Massing/PointCurveCreation/sinecurve.md](src/Massing/PointCurveCreation/sinecurve.md) | — | `Massing/PointCurveCreation/Command.cs` | 2 |
### MaterialProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `MaterialProperties` | [MaterialProperties/materialproperties.md](src/MaterialProperties/materialproperties.md) | [MaterialProperties/materialproperties.json](src/MaterialProperties/materialproperties.json) | `MaterialProperties/MaterialProperties.cs` | 5 |
### MaterialQuantities (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [MaterialQuantities/materialquantities.md](src/MaterialQuantities/materialquantities.md) | [MaterialQuantities/materialquantities.json](src/MaterialQuantities/materialquantities.json) | `MaterialQuantities/MaterialQuantities.cs` | 5 |
### ModelessDialog (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [ModelessDialog/ModelessForm_ExternalEvent/modelessform-externalevent.md](src/ModelessDialog/ModelessForm_ExternalEvent/modelessform-externalevent.md) | — | `ModelessDialog/ModelessForm_ExternalEvent/Command.cs` | 1 |
| [x] | — | `Command` | [ModelessDialog/ModelessForm_IdlingEvent/modelessform-idlingevent.md](src/ModelessDialog/ModelessForm_IdlingEvent/modelessform-idlingevent.md) | — | `ModelessDialog/ModelessForm_IdlingEvent/Command.cs` | 1 |
### ModelLines (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ModelLines/modellines.md](src/ModelLines/modellines.md) | [ModelLines/modellines.json](src/ModelLines/modellines.json) | `ModelLines/Command.cs` | 4 |
### MoveLinear (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [MoveLinear/movelinear.md](src/MoveLinear/movelinear.md) | [MoveLinear/movelinear.json](src/MoveLinear/movelinear.json) | `MoveLinear/Command.cs` | 4 |
### MultiplanarRebar (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [MultiplanarRebar/multiplanarrebar.md](src/MultiplanarRebar/multiplanarrebar.md) | — | `MultiplanarRebar/Command.cs` | 2 |
### MultiThreading (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [MultiThreading/WorkThread/workthread.md](src/MultiThreading/WorkThread/workthread.md) | — | `MultiThreading/WorkThread/Command.cs` | 2 |
### NetworkPressureLossReport (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NetworkPressureLossReport/networkpressurelossreport.md](src/NetworkPressureLossReport/networkpressurelossreport.md) | [NetworkPressureLossReport/networkpressurelossreport.json](src/NetworkPressureLossReport/networkpressurelossreport.json) | `NetworkPressureLossReport/Command.cs` | 5 |
### NewHostedSweep (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewHostedSweep/newhostedsweep.md](src/NewHostedSweep/newhostedsweep.md) | — | `NewHostedSweep/Command.cs` | 2 |
### NewOpenings (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NewOpenings/newopenings.md](src/NewOpenings/newopenings.md) | [NewOpenings/newopenings.json](src/NewOpenings/newopenings.json) | `NewOpenings/command.cs` | 4 |
### NewPathReinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewPathReinforcement/newpathreinforcement.md](src/NewPathReinforcement/newpathreinforcement.md) | — | `NewPathReinforcement/Command.cs` | 2 |
### NewRebar (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [NewRebar/newrebar.md](src/NewRebar/newrebar.md) | — | `NewRebar/Command.cs` | 2 |
### NewRoof (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [NewRoof/newroof.md](src/NewRoof/newroof.md) | [NewRoof/newroof.json](src/NewRoof/newroof.json) | `NewRoof/Command.cs` | 4 |
### Openings (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Openings/openings.md](src/Openings/openings.md) | [Openings/openings.json](src/Openings/openings.json) | `Openings/Command.cs` | 4 |
### PanelSchedule (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `InstanceViewCreation` | [PanelSchedule/instanceviewcreation.md](src/PanelSchedule/instanceviewcreation.md) | [PanelSchedule/instanceviewcreation.json](src/PanelSchedule/instanceviewcreation.json) | `PanelSchedule/InstanceViewCreation.cs` | 4 |
| [x] | [x] | `PanelScheduleExport` | [PanelSchedule/panelscheduleexport.md](src/PanelSchedule/panelscheduleexport.md) | [PanelSchedule/panelscheduleexport.json](src/PanelSchedule/panelscheduleexport.json) | `PanelSchedule/PanelScheduleExport.cs` | 5 |
| [x] | [x] | `SheetImport` | [PanelSchedule/sheetimport.md](src/PanelSchedule/sheetimport.md) | [PanelSchedule/sheetimport.json](src/PanelSchedule/sheetimport.json) | `PanelSchedule/SheetImport.cs` | 4 |
### ParameterUtils (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ParameterUtils/parameterutils.md](src/ParameterUtils/parameterutils.md) | [ParameterUtils/parameterutils.json](src/ParameterUtils/parameterutils.json) | `ParameterUtils/Command.cs` | 5 |
### PathOfTravel (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PathOfTravel/pathoftravel.md](src/PathOfTravel/pathoftravel.md) | [PathOfTravel/pathoftravel.json](src/PathOfTravel/pathoftravel.json) | `PathOfTravel/Command.cs` | 3 |
### PathOfTravelDoors (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PathOfTravelDoors/path-of-travel-doors.md](src/PathOfTravelDoors/path-of-travel-doors.md) | [PathOfTravelDoors/path-of-travel-doors.json](src/PathOfTravelDoors/path-of-travel-doors.json) | `PathOfTravelDoors/Command.cs` | 4 |
| [x] | [x] | `WriteDoorMarksCommand` | [PathOfTravelDoors/write-door-marks.md](src/PathOfTravelDoors/write-door-marks.md) | [PathOfTravelDoors/write-door-marks.json](src/PathOfTravelDoors/write-door-marks.json) | `PathOfTravelDoors/WriteDoorMarksCommand.cs` | 4 |
### PathReinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [PathReinforcement/pathreinforcement.md](src/PathReinforcement/pathreinforcement.md) | — | `PathReinforcement/Command.cs` | 2 |
### PerformanceAdviserControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `UiCommand` | [PerformanceAdviserControl/uicommand.md](src/PerformanceAdviserControl/uicommand.md) | — | `PerformanceAdviserControl/UICommand.cs` | 1 |
### PhysicalProp (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `DumpMaterialPhysicalParameters` | [PhysicalProp/dumpmaterialphysicalparameters.md](src/PhysicalProp/dumpmaterialphysicalparameters.md) | [PhysicalProp/dumpmaterialphysicalparameters.json](src/PhysicalProp/dumpmaterialphysicalparameters.json) | `PhysicalProp/Command.cs` | 5 |
### PipeSystemExporter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PipeSystemExporter/pipesystemexporter.md](src/PipeSystemExporter/pipesystemexporter.md) | [PipeSystemExporter/pipesystemexporter.json](src/PipeSystemExporter/pipesystemexporter.json) | `PipeSystemExporter/Command.cs` | 3 |
### PlaceFamilyInstanceByFace (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PlaceFamilyInstanceByFace/placefamilyinstancebyface.md](src/PlaceFamilyInstanceByFace/placefamilyinstancebyface.md) | [PlaceFamilyInstanceByFace/placefamilyinstancebyface.json](src/PlaceFamilyInstanceByFace/placefamilyinstancebyface.json) | `PlaceFamilyInstanceByFace/Command.cs` | 4 |
### PlacementOptions (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [PlacementOptions/placementoptions.md](src/PlacementOptions/placementoptions.md) | [PlacementOptions/placementoptions.json](src/PlacementOptions/placementoptions.json) | `PlacementOptions/Command.cs` | 4 |
### PointCloudEngine (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddPredefinedInstanceCommand` | [PointCloudEngine/addpredefinedinstancecommand.md](src/PointCloudEngine/addpredefinedinstancecommand.md) | [PointCloudEngine/addpredefinedinstancecommand.json](src/PointCloudEngine/addpredefinedinstancecommand.json) | `PointCloudEngine/PointCloudEngineSample.cs` | 4 |
| [x] | [x] | `AddRandomizedInstanceCommand` | [PointCloudEngine/addrandomizedinstancecommand.md](src/PointCloudEngine/addrandomizedinstancecommand.md) | [PointCloudEngine/addrandomizedinstancecommand.json](src/PointCloudEngine/addrandomizedinstancecommand.json) | `PointCloudEngine/PointCloudEngineSample.cs` | 4 |
| [x] | [x] | `AddTransformedInstanceCommand` | [PointCloudEngine/addtransformedinstancecommand.md](src/PointCloudEngine/addtransformedinstancecommand.md) | [PointCloudEngine/addtransformedinstancecommand.json](src/PointCloudEngine/addtransformedinstancecommand.json) | `PointCloudEngine/PointCloudEngineSample.cs` | 4 |
| [x] | — | `SerializePredefinedPointCloud` | [PointCloudEngine/serializepredefinedpointcloud.md](src/PointCloudEngine/serializepredefinedpointcloud.md) | — | `PointCloudEngine/PointCloudEngineSample.cs` | 2 |
### PostCommandWorkflow (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `PostCommandRevisionMonitorCommand` | [PostCommandWorkflow/postcommandrevisionmonitorcommand.md](src/PostCommandWorkflow/postcommandrevisionmonitorcommand.md) | — | `PostCommandWorkflow/PostCommandRevisionMonitorCommand.cs` | 1 |
### PowerCircuit (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [PowerCircuit/powercircuit.md](src/PowerCircuit/powercircuit.md) | — | `PowerCircuit/Command.cs` | 2 |
### ProjectInfo (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ProjectInfo/projectinfo.md](src/ProjectInfo/projectinfo.md) | [ProjectInfo/projectinfo.json](src/ProjectInfo/projectinfo.json) | `ProjectInfo/Command.cs` | 5 |
### ReadonlySharedParameters (5)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `BindNewReadonlySharedParametersToDocument` | [ReadonlySharedParameters/bindnewreadonlysharedparameterstodocument.md](src/ReadonlySharedParameters/bindnewreadonlysharedparameterstodocument.md) | — | `ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyCost1` | [ReadonlySharedParameters/setreadonlycost1.md](src/ReadonlySharedParameters/setreadonlycost1.md) | — | `ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyCost2` | [ReadonlySharedParameters/setreadonlycost2.md](src/ReadonlySharedParameters/setreadonlycost2.md) | — | `ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyId1` | [ReadonlySharedParameters/setreadonlyid1.md](src/ReadonlySharedParameters/setreadonlyid1.md) | — | `ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` | 2 |
| [x] | — | `SetReadonlyId2` | [ReadonlySharedParameters/setreadonlyid2.md](src/ReadonlySharedParameters/setreadonlyid2.md) | — | `ReadonlySharedParameters/ReadonlySharedParametersCommands.cs` | 2 |
### RebarContainerAnyShapeType (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [RebarContainerAnyShapeType/rebarcontaineranyshapetype.md](src/RebarContainerAnyShapeType/rebarcontaineranyshapetype.md) | — | `RebarContainerAnyShapeType/Command.cs` | 2 |
### RebarFreeForm (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `AddSharedParams` | [RebarFreeForm/addsharedparams.md](src/RebarFreeForm/addsharedparams.md) | [RebarFreeForm/addsharedparams.json](src/RebarFreeForm/addsharedparams.json) | `RebarFreeForm/AddSharedParams.cs` | 4 |
| [x] | — | `Command` | [RebarFreeForm/rebarfreeform.md](src/RebarFreeForm/rebarfreeform.md) | — | `RebarFreeForm/Command.cs` | 2 |
### ReferencePlane (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ReferencePlane/referenceplane.md](src/ReferencePlane/referenceplane.md) | [ReferencePlane/referenceplane.json](src/ReferencePlane/referenceplane.json) | `ReferencePlane/Command.cs` | 4 |
### Reinforcement (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [Reinforcement/reinforcement.md](src/Reinforcement/reinforcement.md) | — | `Reinforcement/Command.cs` | 2 |
### Ribbon (6)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CreateWall` | [Ribbon/createwall.md](src/Ribbon/createwall.md) | — | `Ribbon/AddInCommand.cs` | 1 |
| [x] | — | `DeleteWalls` | [Ribbon/deletewalls.md](src/Ribbon/deletewalls.md) | — | `Ribbon/AddInCommand.cs` | 1 |
| [x] | — | `Dummy` | [Ribbon/dummy.md](src/Ribbon/dummy.md) | — | `Ribbon/AddInCommand.cs` | 1 |
| [x] | — | `ResetSetting` | [Ribbon/resetsetting.md](src/Ribbon/resetsetting.md) | — | `Ribbon/AddInCommand.cs` | 1 |
| [x] | — | `XMoveWalls` | [Ribbon/xmovewalls.md](src/Ribbon/xmovewalls.md) | — | `Ribbon/AddInCommand.cs` | 1 |
| [x] | — | `YMoveWalls` | [Ribbon/ymovewalls.md](src/Ribbon/ymovewalls.md) | — | `Ribbon/AddInCommand.cs` | 1 |
### RoofsRooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [RoofsRooms/roofsrooms.md](src/RoofsRooms/roofsrooms.md) | [RoofsRooms/roofsrooms.json](src/RoofsRooms/roofsrooms.json) | `RoofsRooms/Command.cs` | 4 |
### Rooms (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Rooms/rooms.md](src/Rooms/rooms.md) | [Rooms/rooms.json](src/Rooms/rooms.json) | `Rooms/Command.cs` | 4 |
### RoomSchedule (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [RoomSchedule/roomschedule.md](src/RoomSchedule/roomschedule.md) | [RoomSchedule/roomschedule.json](src/RoomSchedule/roomschedule.json) | `RoomSchedule/Command.cs` | 4 |
### RoomVolumeDirectShape (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [RoomVolumeDirectShape/room-volume-direct-shape.md](src/RoomVolumeDirectShape/room-volume-direct-shape.md) | [RoomVolumeDirectShape/room-volume-direct-shape.json](src/RoomVolumeDirectShape/room-volume-direct-shape.json) | `RoomVolumeDirectShape/Command.cs` | 4 |
### CropViewToRoom (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [CropViewToRoom/crop-view-to-room.md](src/CropViewToRoom/crop-view-to-room.md) | [CropViewToRoom/crop-view-to-room.json](src/CropViewToRoom/crop-view-to-room.json) | `CropViewToRoom/Command.cs` | 4 |
### RotateFramingObjects (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `RotateFramingObjects` | [RotateFramingObjects/rotateframingobjects.md](src/RotateFramingObjects/rotateframingobjects.md) | [RotateFramingObjects/rotateframingobjects.json](src/RotateFramingObjects/rotateframingobjects.json) | `RotateFramingObjects/RotateFramingObjects.cs` | 4 |
### RoutingPreferenceTools (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [RoutingPreferenceTools/routingpreferencetools.md](src/RoutingPreferenceTools/routingpreferencetools.md) | — | `RoutingPreferenceTools/RoutingPreferenceAnalysis/Command.cs` | 2 |
| [x] | [x] | `CommandReadPreferences` | [RoutingPreferenceTools/commandreadpreferences.md](src/RoutingPreferenceTools/commandreadpreferences.md) | [RoutingPreferenceTools/commandreadpreferences.json](src/RoutingPreferenceTools/commandreadpreferences.json) | `RoutingPreferenceTools/RoutingPreferenceBuilder/CommandReadPreferences.cs` | 5 |
| [x] | — | `CommandWritePreferences` | [RoutingPreferenceTools/commandwritepreferences.md](src/RoutingPreferenceTools/commandwritepreferences.md) | — | `RoutingPreferenceTools/RoutingPreferenceBuilder/CommandWritePreferences.cs` | 2 |
### ScheduleAutomaticFormatter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ScheduleFormatterCommand` | [ScheduleAutomaticFormatter/scheduleformattercommand.md](src/ScheduleAutomaticFormatter/scheduleformattercommand.md) | [ScheduleAutomaticFormatter/scheduleformattercommand.json](src/ScheduleAutomaticFormatter/scheduleformattercommand.json) | `ScheduleAutomaticFormatter/ScheduleFormatterCommand.cs` | 4 |
### ScheduleCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ScheduleCreation/schedulecreation.md](src/ScheduleCreation/schedulecreation.md) | [ScheduleCreation/schedulecreation.json](src/ScheduleCreation/schedulecreation.json) | `ScheduleCreation/Command.cs` | 4 |
### ScheduleToHTML (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `ScheduleHtmlExportCommand` | [ScheduleToHTML/schedulehtmlexportcommand.md](src/ScheduleToHTML/schedulehtmlexportcommand.md) | [ScheduleToHTML/schedulehtmlexportcommand.json](src/ScheduleToHTML/schedulehtmlexportcommand.json) | `ScheduleToHTML/ScheduleHTMLExportCommand.cs` | 5 |
### Selections (4)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `PickforDeletion` | [Selections/pickfordeletion.md](src/Selections/pickfordeletion.md) | — | `Selections/Command.cs` | 2 |
| [x] | — | `PlaceAtPickedFaceWorkplane` | [Selections/placeatpickedfaceworkplane.md](src/Selections/placeatpickedfaceworkplane.md) | — | `Selections/Command.cs` | 2 |
| [x] | — | `PlaceAtPointOnWallFace` | [Selections/placeatpointonwallface.md](src/Selections/placeatpointonwallface.md) | — | `Selections/Command.cs` | 2 |
| [x] | — | `SelectionDialog` | [Selections/selectiondialog.md](src/Selections/selectiondialog.md) | — | `Selections/Command.cs` | 2 |
### ShaftHolePuncher (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ShaftHolePuncher/shaftholepuncher.md](src/ShaftHolePuncher/shaftholepuncher.md) | [ShaftHolePuncher/shaftholepuncher.json](src/ShaftHolePuncher/shaftholepuncher.json) | `ShaftHolePuncher/Command.cs` | 4 |
### SharedCoordinateSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SharedCoordinateSystem/sharedcoordinatesystem.md](src/SharedCoordinateSystem/sharedcoordinatesystem.md) | [SharedCoordinateSystem/sharedcoordinatesystem.json](src/SharedCoordinateSystem/sharedcoordinatesystem.json) | `SharedCoordinateSystem/Command.cs` | 5 |
### SheetToView3D (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SheetToView3D/sheettoview3d.md](src/SheetToView3D/sheettoview3d.md) | [SheetToView3D/sheettoview3d.json](src/SheetToView3D/sheettoview3d.json) | `SheetToView3D/SheetToView3D.cs` | 5 |
### SinePlotter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [SinePlotter/sineplotter.md](src/SinePlotter/sineplotter.md) | — | `SinePlotter/Command.cs` | 1 |
### Site (6)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `SiteAddRetainingPondCommand` | [Site/siteaddretainingpondcommand.md](src/Site/siteaddretainingpondcommand.md) | [Site/siteaddretainingpondcommand.json](src/Site/siteaddretainingpondcommand.json) | `Site/SiteAddRetainingPondCommand.cs` | 4 |
| [x] | [x] | `SiteDeleteRegionAndPointsCommand` | [Site/sitedeleteregionandpointscommand.md](src/Site/sitedeleteregionandpointscommand.md) | [Site/sitedeleteregionandpointscommand.json](src/Site/sitedeleteregionandpointscommand.json) | `Site/SiteDeleteRegionAndPointsCommand.cs` | 4 |
| [x] | — | `SiteLowerTerrainInRegionCommand` | [Site/sitelowerterraininregioncommand.md](src/Site/sitelowerterraininregioncommand.md) | — | `Site/SiteLowerTerrainInRegionCommand.cs` | 2 |
| [x] | — | `SiteMoveRegionAndPointsCommand` | [Site/sitemoveregionandpointscommand.md](src/Site/sitemoveregionandpointscommand.md) | — | `Site/SiteMoveRegionAndPointsCommand.cs` | 2 |
| [x] | — | `SiteNormalizeTerrainInRegionCommand` | [Site/sitenormalizeterraininregioncommand.md](src/Site/sitenormalizeterraininregioncommand.md) | — | `Site/SiteNormalizeTerrainInRegionCommand.cs` | 2 |
| [x] | — | `SiteRaiseTerrainInRegionCommand` | [Site/siteraiseterraininregioncommand.md](src/Site/siteraiseterraininregioncommand.md) | — | `Site/SiteRaiseTerrainInRegionCommand.cs` | 2 |
### SlabProperties (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SlabProperties/slabproperties.md](src/SlabProperties/slabproperties.md) | [SlabProperties/slabproperties.json](src/SlabProperties/slabproperties.json) | `SlabProperties/Command.cs` | 4 |
### SlabShapeEditing (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SlabShapeEditing/slabshapeediting.md](src/SlabShapeEditing/slabshapeediting.md) | [SlabShapeEditing/slabshapeediting.json](src/SlabShapeEditing/slabshapeediting.json) | `SlabShapeEditing/Command.cs` | 4 |
### SolidSolidCut (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Cut` | [SolidSolidCut/cut.md](src/SolidSolidCut/cut.md) | — | `SolidSolidCut/Command.cs` | 2 |
| [x] | — | `Uncut` | [SolidSolidCut/uncut.md](src/SolidSolidCut/uncut.md) | — | `SolidSolidCut/Command.cs` | 2 |
### SpanDirection (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SpanDirection/spandirection.md](src/SpanDirection/spandirection.md) | [SpanDirection/spandirection.json](src/SpanDirection/spandirection.json) | `SpanDirection/Command.cs` | 4 |
### SpatialElementGeometryCalculator (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SpatialElementGeometryCalculator/spatial-element-geometry-calculator.md](src/SpatialElementGeometryCalculator/spatial-element-geometry-calculator.md) | [SpatialElementGeometryCalculator/spatial-element-geometry-calculator.json](src/SpatialElementGeometryCalculator/spatial-element-geometry-calculator.json) | `SpatialElementGeometryCalculator/Command.cs` | 4 |
### SetoutPoints (2)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CmdGeomVertices` | [SetoutPoints/geom-vertices.md](src/SetoutPoints/geom-vertices.md) | [SetoutPoints/geom-vertices.json](src/SetoutPoints/geom-vertices.json) | `SetoutPoints/CmdGeomVertices.cs` | 4 |
| [x] | [x] | `CmdRenumber` | [SetoutPoints/renumber.md](src/SetoutPoints/renumber.md) | [SetoutPoints/renumber.json](src/SetoutPoints/renumber.json) | `SetoutPoints/CmdRenumber.cs` | 4 |
### SpotDimension (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [SpotDimension/spotdimension.md](src/SpotDimension/spotdimension.md) | [SpotDimension/spotdimension.json](src/SpotDimension/spotdimension.json) | `SpotDimension/Command.cs` | 4 |
### StairsAutomation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StairsAutomation/stairsautomation.md](src/StairsAutomation/stairsautomation.md) | [StairsAutomation/stairsautomation.json](src/StairsAutomation/stairsautomation.json) | `StairsAutomation/Command.cs` | 4 |
### StructSample (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StructSample/structsample.md](src/StructSample/structsample.md) | [StructSample/structsample.json](src/StructSample/structsample.json) | `StructSample/Command.cs` | 4 |
### StructuralLayerFunction (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [StructuralLayerFunction/structurallayerfunction.md](src/StructuralLayerFunction/structurallayerfunction.md) | [StructuralLayerFunction/structurallayerfunction.json](src/StructuralLayerFunction/structurallayerfunction.json) | `StructuralLayerFunction/Command.cs` | 3 |
### TagBeam (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [TagBeam/tagbeam.md](src/TagBeam/tagbeam.md) | [TagBeam/tagbeam.json](src/TagBeam/tagbeam.json) | `TagBeam/Command.cs` | 4 |
| [x] | [x] | `CreateText` | [TagBeam/createtext.md](src/TagBeam/createtext.md) | [TagBeam/createtext.json](src/TagBeam/createtext.json) | `TagBeam/Command.cs` | 4 |
| [x] | [x] | `TagRebar` | [TagBeam/tagrebar.md](src/TagBeam/tagrebar.md) | [TagBeam/tagrebar.json](src/TagBeam/tagrebar.json) | `TagBeam/Command.cs` | 4 |
### Toposolid (8)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `ContourSettingCreation` | [Toposolid/contoursettingcreation.md](src/Toposolid/contoursettingcreation.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `ContourSettingModification` | [Toposolid/contoursettingmodification.md](src/Toposolid/contoursettingmodification.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `SimplifyToposolid` | [Toposolid/simplifytoposolid.md](src/Toposolid/simplifytoposolid.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `SplitToposolid` | [Toposolid/splittoposolid.md](src/Toposolid/splittoposolid.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `SsePointVisibility` | [Toposolid/ssepointvisibility.md](src/Toposolid/ssepointvisibility.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `ToposolidCreation` | [Toposolid/toposolidcreation.md](src/Toposolid/toposolidcreation.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `ToposolidFromDwg` | [Toposolid/toposolidfromdwg.md](src/Toposolid/toposolidfromdwg.md) | — | `Toposolid/Command.cs` | 2 |
| [x] | — | `ToposolidFromSurface` | [Toposolid/toposolidfromsurface.md](src/Toposolid/toposolidfromsurface.md) | — | `Toposolid/Command.cs` | 2 |
### TransactionControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [TransactionControl/transactioncontrol.md](src/TransactionControl/transactioncontrol.md) | — | `TransactionControl/Command.cs` | 2 |
### TraverseSystem (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `Command` | [TraverseSystem/traversesystem.md](src/TraverseSystem/traversesystem.md) | — | `TraverseSystem/Command.cs` | 2 |
### Truss (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Truss/truss.md](src/Truss/truss.md) | [Truss/truss.json](src/Truss/truss.json) | `Truss/Command.cs` | 4 |
### UIAPI (3)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | — | `CalcCommand` | [UIAPI/calccommand.md](src/UIAPI/calccommand.md) | — | `UIAPI/ExternalApplication.cs` | 1 |
| [x] | — | `DragAndDropCommand` | [UIAPI/draganddropcommand.md](src/UIAPI/draganddropcommand.md) | — | `UIAPI/DragAndDrop/DragAndDropCommand.cs` | 1 |
| [x] | — | `PreviewCommand` | [UIAPI/previewcommand.md](src/UIAPI/previewcommand.md) | — | `UIAPI/PreviewControl/PreviewCommand.cs` | 1 |
### Units (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [Units/units.md](src/Units/units.md) | [Units/units.json](src/Units/units.json) | `Units/Command.cs` | 5 |
### VersionChecking (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [VersionChecking/versionchecking.md](src/VersionChecking/versionchecking.md) | [VersionChecking/versionchecking.json](src/VersionChecking/versionchecking.json) | `VersionChecking/VersionChecking.cs` | 5 |
### ViewPrinter (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ViewPrinter/viewprinter.md](src/ViewPrinter/viewprinter.md) | [ViewPrinter/viewprinter.json](src/ViewPrinter/viewprinter.json) | `ViewPrinter/Command.cs` | 4 |
### ViewTemplateCreation (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [ViewTemplateCreation/viewtemplatecreation.md](src/ViewTemplateCreation/viewtemplatecreation.md) | [ViewTemplateCreation/viewtemplatecreation.json](src/ViewTemplateCreation/viewtemplatecreation.json) | `ViewTemplateCreation/Command.cs` | 4 |
### VisibilityControl (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [VisibilityControl/visibilitycontrol.md](src/VisibilityControl/visibilitycontrol.md) | [VisibilityControl/visibilitycontrol.json](src/VisibilityControl/visibilitycontrol.json) | `VisibilityControl/Command.cs` | 4 |
### WinderStairs (1)

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `Command` | [WinderStairs/winderstairs.md](src/WinderStairs/winderstairs.md) | [WinderStairs/winderstairs.json](src/WinderStairs/winderstairs.json) | `WinderStairs/Command.cs` | 4 |

## Notes

- **Agent runbook:** full step-by-step instructions in [`docs/AGENTS.md`](docs/AGENTS.md).
- **SDK ReadMe:** `SampleData` resolves the first `*.rtf` in the command's sample folder; ~167 RTF files exist under `src/`.
- `CreateStructureWall` inherits `CreateWall` and is discovered by reflection but not listed separately.
- `IExternalCommandAvailability` classes (`WallSelection`, `View3D`, etc.) are excluded — not commands.
- MCP JSON files in `src/` are **specifications** for future Bowerbird Revit tools, not live MCP server entries.
- Refresh progress counts after each session by recounting `[x]` in the inventory or re-running the inventory generator.

## The Building Coder (TBC) samples

Imported 170 commands from The Building Coder.

| Doc | MCP | Class | Command doc | MCP descriptor | Source | Rating |
|-----|-----|-------|-------------|----------------|--------|--------|
| [x] | [x] | `CmdAlignTwoViews` | [TBC_AlignTwoViews/align-two-views.md](src/TBC_AlignTwoViews/align-two-views.md) | [TBC_AlignTwoViews/align-two-views.json](src/TBC_AlignTwoViews/align-two-views.json) | `TBC_AlignTwoViews/CmdViewsShowingElements.cs` | 3 |
| [x] | [x] | `CmdAnalyticalModelGeom` | [TBC_AnalyticalModelGeom/analytical-model-geom.md](src/TBC_AnalyticalModelGeom/analytical-model-geom.md) | [TBC_AnalyticalModelGeom/analytical-model-geom.json](src/TBC_AnalyticalModelGeom/analytical-model-geom.json) | `TBC_AnalyticalModelGeom/CmdAnalyticalModelGeom.cs` | 3 |
| [x] | [x] | `CmdAzimuth` | [TBC_Azimuth/azimuth.md](src/TBC_Azimuth/azimuth.md) | [TBC_Azimuth/azimuth.json](src/TBC_Azimuth/azimuth.json) | `TBC_Azimuth/CmdAzimuth.cs` | 3 |
| [x] | [x] | `CmdBim360Links` | [TBC_Bim360Links/bim360-links.md](src/TBC_Bim360Links/bim360-links.md) | [TBC_Bim360Links/bim360-links.json](src/TBC_Bim360Links/bim360-links.json) | `TBC_Bim360Links/CmdBim360Links.cs` | 3 |
| [x] | [x] | `CmdBoundingBox` | [TBC_BoundingBox/bounding-box.md](src/TBC_BoundingBox/bounding-box.md) | [TBC_BoundingBox/bounding-box.json](src/TBC_BoundingBox/bounding-box.json) | `TBC_BoundingBox/CmdBoundingBox.cs` | 3 |
| [x] | [x] | `CmdBrepBuilder` | [TBC_BrepBuilder/brep-builder.md](src/TBC_BrepBuilder/brep-builder.md) | [TBC_BrepBuilder/brep-builder.json](src/TBC_BrepBuilder/brep-builder.json) | `TBC_BrepBuilder/CmdBrepBuilder.cs` | 3 |
| [x] | [x] | `BrokenCommand` | [TBC_BrokenCommand/broken-command.md](src/TBC_BrokenCommand/broken-command.md) | [TBC_BrokenCommand/broken-command.json](src/TBC_BrokenCommand/broken-command.json) | `TBC_BrokenCommand/CmdDeleteUnusedRefPlanes.cs` | 3 |
| [x] | [x] | `CmdCategories` | [TBC_Categories/categories.md](src/TBC_Categories/categories.md) | [TBC_Categories/categories.json](src/TBC_Categories/categories.json) | `TBC_Categories/CmdCategories.cs` | 3 |
| [x] | [x] | `CmdCategorySupportsTypeParameter` | [TBC_CategorySupportsTypeParameter/category-supports-type-parameter.md](src/TBC_CategorySupportsTypeParameter/category-supports-type-parameter.md) | [TBC_CategorySupportsTypeParameter/category-supports-type-parameter.json](src/TBC_CategorySupportsTypeParameter/category-supports-type-parameter.json) | `TBC_CategorySupportsTypeParameter/CmdCategorySupportsTypeParameter.cs` | 3 |
| [x] | [x] | `CmdChangeElementColor` | [TBC_ChangeElementColor/change-element-color.md](src/TBC_ChangeElementColor/change-element-color.md) | [TBC_ChangeElementColor/change-element-color.json](src/TBC_ChangeElementColor/change-element-color.json) | `TBC_ChangeElementColor/CmdChangeElementColor.cs` | 3 |
| [x] | [x] | `CmdChangeFloorSlope` | [TBC_ChangeFloorSlope/change-floor-slope.md](src/TBC_ChangeFloorSlope/change-floor-slope.md) | [TBC_ChangeFloorSlope/change-floor-slope.json](src/TBC_ChangeFloorSlope/change-floor-slope.json) | `TBC_ChangeFloorSlope/CmdCreateSlopedSlab.cs` | 3 |
| [x] | [x] | `CmdChangeLinkedFilePath` | [TBC_ChangeLinkedFilePath/change-linked-file-path.md](src/TBC_ChangeLinkedFilePath/change-linked-file-path.md) | [TBC_ChangeLinkedFilePath/change-linked-file-path.json](src/TBC_ChangeLinkedFilePath/change-linked-file-path.json) | `TBC_ChangeLinkedFilePath/CmdChangeLinkedFilePath.cs` | 3 |
| [x] | [x] | `CmdCloseDocument` | [TBC_CloseDocument/close-document.md](src/TBC_CloseDocument/close-document.md) | [TBC_CloseDocument/close-document.json](src/TBC_CloseDocument/close-document.json) | `TBC_CloseDocument/CmdCloseDocument.cs` | 3 |
| [x] | [x] | `Command` | [TBC_CmdDimensionInstanceOrigin_Command/dimension-instance-origin-command.md](src/TBC_CmdDimensionInstanceOrigin_Command/dimension-instance-origin-command.md) | [TBC_CmdDimensionInstanceOrigin_Command/dimension-instance-origin-command.json](src/TBC_CmdDimensionInstanceOrigin_Command/dimension-instance-origin-command.json) | `TBC_CmdDimensionInstanceOrigin_Command/CmdDimensionInstanceOrigin.cs` | 3 |
| [x] | [x] | `CmdCollectorPerformance` | [TBC_CollectorPerformance/collector-performance.md](src/TBC_CollectorPerformance/collector-performance.md) | [TBC_CollectorPerformance/collector-performance.json](src/TBC_CollectorPerformance/collector-performance.json) | `TBC_CollectorPerformance/CmdCollectorPerformance.cs` | 3 |
| [x] | [x] | `RevitCommand` | [TBC_CollectorPerformance_RevitCommand/revit-command.md](src/TBC_CollectorPerformance_RevitCommand/revit-command.md) | [TBC_CollectorPerformance_RevitCommand/revit-command.json](src/TBC_CollectorPerformance_RevitCommand/revit-command.json) | `TBC_CollectorPerformance_RevitCommand/CmdCollectorPerformance.cs` | 3 |
| [x] | [x] | `CmdColumnRound` | [TBC_ColumnRound/column-round.md](src/TBC_ColumnRound/column-round.md) | [TBC_ColumnRound/column-round.json](src/TBC_ColumnRound/column-round.json) | `TBC_ColumnRound/CmdColumnRound.cs` | 3 |
| [x] | [x] | `Commands` | [TBC_Commands/commands.md](src/TBC_Commands/commands.md) | [TBC_Commands/commands.json](src/TBC_Commands/commands.json) | `TBC_Commands/CmdCollectorPerformance.cs` | 3 |
| [x] | [x] | `CmdCoordsOfViewOnSheet` | [TBC_CoordsOfViewOnSheet/coords-of-view-on-sheet.md](src/TBC_CoordsOfViewOnSheet/coords-of-view-on-sheet.md) | [TBC_CoordsOfViewOnSheet/coords-of-view-on-sheet.json](src/TBC_CoordsOfViewOnSheet/coords-of-view-on-sheet.json) | `TBC_CoordsOfViewOnSheet/CmdCoordsOfViewOnSheet.cs` | 3 |
| [x] | [x] | `CmdCopyWallType` | [TBC_CopyWallType/copy-wall-type.md](src/TBC_CopyWallType/copy-wall-type.md) | [TBC_CopyWallType/copy-wall-type.json](src/TBC_CopyWallType/copy-wall-type.json) | `TBC_CopyWallType/CmdCopyWallType.cs` | 3 |
| [x] | [x] | `CreateFillledRegionDimensionsCommand` | [TBC_CreateFillledRegionDimensionsCommand/create-fillled-region-dimensions-command.md](src/TBC_CreateFillledRegionDimensionsCommand/create-fillled-region-dimensions-command.md) | [TBC_CreateFillledRegionDimensionsCommand/create-fillled-region-dimensions-command.json](src/TBC_CreateFillledRegionDimensionsCommand/create-fillled-region-dimensions-command.json) | `TBC_CreateFillledRegionDimensionsCommand/CmdDimensionWallsIterateFaces.cs` | 3 |
| [x] | [x] | `CmdCreateGableWall` | [TBC_CreateGableWall/create-gable-wall.md](src/TBC_CreateGableWall/create-gable-wall.md) | [TBC_CreateGableWall/create-gable-wall.json](src/TBC_CreateGableWall/create-gable-wall.json) | `TBC_CreateGableWall/CmdCreateGableWall.cs` | 3 |
| [x] | [x] | `CmdCreateLineStyle` | [TBC_CreateLineStyle/create-line-style.md](src/TBC_CreateLineStyle/create-line-style.md) | [TBC_CreateLineStyle/create-line-style.json](src/TBC_CreateLineStyle/create-line-style.json) | `TBC_CreateLineStyle/CmdCreateLineStyle.cs` | 3 |
| [x] | [x] | `CmdCreateSharedParams` | [TBC_CreateSharedParams/create-shared-params.md](src/TBC_CreateSharedParams/create-shared-params.md) | [TBC_CreateSharedParams/create-shared-params.json](src/TBC_CreateSharedParams/create-shared-params.json) | `TBC_CreateSharedParams/CmdCreateSharedParams.cs` | 3 |
| [x] | [x] | `CmdCreateSlopedSlab` | [TBC_CreateSlopedSlab/create-sloped-slab.md](src/TBC_CreateSlopedSlab/create-sloped-slab.md) | [TBC_CreateSlopedSlab/create-sloped-slab.json](src/TBC_CreateSlopedSlab/create-sloped-slab.json) | `TBC_CreateSlopedSlab/CmdCreateSlopedSlab.cs` | 3 |
| [x] | [x] | `CreateWallsAutomaticallyCommand` | [TBC_CreateWallsAutomaticallyCommand/create-walls-automatically-command.md](src/TBC_CreateWallsAutomaticallyCommand/create-walls-automatically-command.md) | [TBC_CreateWallsAutomaticallyCommand/create-walls-automatically-command.json](src/TBC_CreateWallsAutomaticallyCommand/create-walls-automatically-command.json) | `TBC_CreateWallsAutomaticallyCommand/CmdFaceWall.cs` | 3 |
| [x] | [x] | `CmdCropToRoom` | [TBC_CropToRoom/crop-to-room.md](src/TBC_CropToRoom/crop-to-room.md) | [TBC_CropToRoom/crop-to-room.json](src/TBC_CropToRoom/crop-to-room.json) | `TBC_CropToRoom/CmdCropToRoom.cs` | 3 |
| [x] | [x] | `CmdCurtainWallGeom` | [TBC_CurtainWallGeom/curtain-wall-geom.md](src/TBC_CurtainWallGeom/curtain-wall-geom.md) | [TBC_CurtainWallGeom/curtain-wall-geom.json](src/TBC_CurtainWallGeom/curtain-wall-geom.json) | `TBC_CurtainWallGeom/CmdCurtainWallGeom.cs` | 3 |
| [x] | [x] | `CmdDeleteMacros` | [TBC_DeleteMacros/delete-macros.md](src/TBC_DeleteMacros/delete-macros.md) | [TBC_DeleteMacros/delete-macros.json](src/TBC_DeleteMacros/delete-macros.json) | `TBC_DeleteMacros/CmdDeleteMacros.cs` | 3 |
| [x] | [x] | `CmdDeleteUnusedRefPlanes` | [TBC_DeleteUnusedRefPlanes/delete-unused-ref-planes.md](src/TBC_DeleteUnusedRefPlanes/delete-unused-ref-planes.md) | [TBC_DeleteUnusedRefPlanes/delete-unused-ref-planes.json](src/TBC_DeleteUnusedRefPlanes/delete-unused-ref-planes.json) | `TBC_DeleteUnusedRefPlanes/CmdDeleteUnusedRefPlanes.cs` | 3 |
| [x] | [x] | `CmdDeleteUnusedRefPlanes_2014` | [TBC_DeleteUnusedRefPlanes_2014/delete-unused-ref-planes-2014.md](src/TBC_DeleteUnusedRefPlanes_2014/delete-unused-ref-planes-2014.md) | [TBC_DeleteUnusedRefPlanes_2014/delete-unused-ref-planes-2014.json](src/TBC_DeleteUnusedRefPlanes_2014/delete-unused-ref-planes-2014.json) | `TBC_DeleteUnusedRefPlanes_2014/CmdDeleteUnusedRefPlanes.cs` | 3 |
| [x] | [x] | `CmdDemoCheck` | [TBC_DemoCheck/demo-check.md](src/TBC_DemoCheck/demo-check.md) | [TBC_DemoCheck/demo-check.json](src/TBC_DemoCheck/demo-check.json) | `TBC_DemoCheck/CmdDemoCheck.cs` | 3 |
| [x] | [x] | `CmdDetailCurves` | [TBC_DetailCurves/detail-curves.md](src/TBC_DetailCurves/detail-curves.md) | [TBC_DetailCurves/detail-curves.json](src/TBC_DetailCurves/detail-curves.json) | `TBC_DetailCurves/CmdDetailCurves.cs` | 3 |
| [x] | [x] | `CmdDimensionInstanceOrigin` | [TBC_DimensionInstanceOrigin/dimension-instance-origin.md](src/TBC_DimensionInstanceOrigin/dimension-instance-origin.md) | [TBC_DimensionInstanceOrigin/dimension-instance-origin.json](src/TBC_DimensionInstanceOrigin/dimension-instance-origin.json) | `TBC_DimensionInstanceOrigin/CmdDimensionInstanceOrigin.cs` | 3 |
| [x] | [x] | `CmdDimensionWallsFindRefs` | [TBC_DimensionWallsFindRefs/dimension-walls-find-refs.md](src/TBC_DimensionWallsFindRefs/dimension-walls-find-refs.md) | [TBC_DimensionWallsFindRefs/dimension-walls-find-refs.json](src/TBC_DimensionWallsFindRefs/dimension-walls-find-refs.json) | `TBC_DimensionWallsFindRefs/CmdDimensionWallsFindRefs.cs` | 3 |
| [x] | [x] | `CmdDimensionWallsIterateFaces` | [TBC_DimensionWallsIterateFaces/dimension-walls-iterate-faces.md](src/TBC_DimensionWallsIterateFaces/dimension-walls-iterate-faces.md) | [TBC_DimensionWallsIterateFaces/dimension-walls-iterate-faces.json](src/TBC_DimensionWallsIterateFaces/dimension-walls-iterate-faces.json) | `TBC_DimensionWallsIterateFaces/CmdDimensionWallsIterateFaces.cs` | 3 |
| [x] | [x] | `CmdDisallowJoin` | [TBC_DisallowJoin/disallow-join.md](src/TBC_DisallowJoin/disallow-join.md) | [TBC_DisallowJoin/disallow-join.json](src/TBC_DisallowJoin/disallow-join.json) | `TBC_DisallowJoin/CmdDisallowJoin.cs` | 3 |
| [x] | [x] | `CmdDocumentVersion` | [TBC_DocumentVersion/document-version.md](src/TBC_DocumentVersion/document-version.md) | [TBC_DocumentVersion/document-version.json](src/TBC_DocumentVersion/document-version.json) | `TBC_DocumentVersion/CmdDocumentVersion.cs` | 3 |
| [x] | [x] | `CmdDuctResize` | [TBC_DuctResize/duct-resize.md](src/TBC_DuctResize/duct-resize.md) | [TBC_DuctResize/duct-resize.json](src/TBC_DuctResize/duct-resize.json) | `TBC_DuctResize/CmdDuctResize.cs` | 3 |
| [x] | [x] | `CmdDuplicateElements` | [TBC_DuplicateElements/duplicate-elements.md](src/TBC_DuplicateElements/duplicate-elements.md) | [TBC_DuplicateElements/duplicate-elements.json](src/TBC_DuplicateElements/duplicate-elements.json) | `TBC_DuplicateElements/CmdDuplicateElements.cs` | 3 |
| [x] | [x] | `CmdDutAbbreviation` | [TBC_DutAbbreviation/dut-abbreviation.md](src/TBC_DutAbbreviation/dut-abbreviation.md) | [TBC_DutAbbreviation/dut-abbreviation.json](src/TBC_DutAbbreviation/dut-abbreviation.json) | `TBC_DutAbbreviation/CmdDutAbbreviation.cs` | 3 |
| [x] | [x] | `CmdEditFloor` | [TBC_EditFloor/edit-floor.md](src/TBC_EditFloor/edit-floor.md) | [TBC_EditFloor/edit-floor.json](src/TBC_EditFloor/edit-floor.json) | `TBC_EditFloor/CmdEditFloor.cs` | 3 |
| [x] | [x] | `CmdElectricalLoad` | [TBC_ElectricalLoad/electrical-load.md](src/TBC_ElectricalLoad/electrical-load.md) | [TBC_ElectricalLoad/electrical-load.json](src/TBC_ElectricalLoad/electrical-load.json) | `TBC_ElectricalLoad/CmdElectricalLoad.cs` | 3 |
| [x] | [x] | `CmdElevationWatcher` | [TBC_ElevationWatcher/elevation-watcher.md](src/TBC_ElevationWatcher/elevation-watcher.md) | [TBC_ElevationWatcher/elevation-watcher.json](src/TBC_ElevationWatcher/elevation-watcher.json) | `TBC_ElevationWatcher/CmdElevationWatcher.cs` | 3 |
| [x] | [x] | `CmdElevationWatcherUpdater` | [TBC_ElevationWatcherUpdater/elevation-watcher-updater.md](src/TBC_ElevationWatcherUpdater/elevation-watcher-updater.md) | [TBC_ElevationWatcherUpdater/elevation-watcher-updater.json](src/TBC_ElevationWatcherUpdater/elevation-watcher-updater.json) | `TBC_ElevationWatcherUpdater/CmdElevationWatcher.cs` | 3 |
| [x] | [x] | `CmdEllipticalArc` | [TBC_EllipticalArc/elliptical-arc.md](src/TBC_EllipticalArc/elliptical-arc.md) | [TBC_EllipticalArc/elliptical-arc.json](src/TBC_EllipticalArc/elliptical-arc.json) | `TBC_EllipticalArc/CmdEllipticalArc.cs` | 3 |
| [x] | [x] | `CmdExportIfc` | [TBC_ExportIfc/export-ifc.md](src/TBC_ExportIfc/export-ifc.md) | [TBC_ExportIfc/export-ifc.json](src/TBC_ExportIfc/export-ifc.json) | `TBC_ExportIfc/CmdExportIfc.cs` | 3 |
| [x] | [x] | `CmdExportImage` | [TBC_ExportImage/export-image.md](src/TBC_ExportImage/export-image.md) | [TBC_ExportImage/export-image.json](src/TBC_ExportImage/export-image.json) | `TBC_ExportImage/CmdExportImage.cs` | 3 |
| [x] | [x] | `CmdExportSolidToSat` | [TBC_ExportSolidToSat/export-solid-to-sat.md](src/TBC_ExportSolidToSat/export-solid-to-sat.md) | [TBC_ExportSolidToSat/export-solid-to-sat.json](src/TBC_ExportSolidToSat/export-solid-to-sat.json) | `TBC_ExportSolidToSat/CmdExportSolidToSat.cs` | 3 |
| [x] | [x] | `CmdExteriorWalls` | [TBC_ExteriorWalls/exterior-walls.md](src/TBC_ExteriorWalls/exterior-walls.md) | [TBC_ExteriorWalls/exterior-walls.json](src/TBC_ExteriorWalls/exterior-walls.json) | `TBC_ExteriorWalls/CmdExteriorWalls.cs` | 3 |
| [x] | [x] | `CmdFaceWall` | [TBC_FaceWall/face-wall.md](src/TBC_FaceWall/face-wall.md) | [TBC_FaceWall/face-wall.json](src/TBC_FaceWall/face-wall.json) | `TBC_FaceWall/CmdFaceWall.cs` | 3 |
| [x] | [x] | `CmdFailureGatherer` | [TBC_FailureGatherer/failure-gatherer.md](src/TBC_FailureGatherer/failure-gatherer.md) | [TBC_FailureGatherer/failure-gatherer.json](src/TBC_FailureGatherer/failure-gatherer.json) | `TBC_FailureGatherer/CmdFailureGatherer.cs` | 3 |
| [x] | [x] | `CmdFamilyParamGuid` | [TBC_FamilyParamGuid/family-param-guid.md](src/TBC_FamilyParamGuid/family-param-guid.md) | [TBC_FamilyParamGuid/family-param-guid.json](src/TBC_FamilyParamGuid/family-param-guid.json) | `TBC_FamilyParamGuid/CmdFamilyParamGuid.cs` | 3 |
| [x] | [x] | `CmdFamilyParamValue` | [TBC_FamilyParamValue/family-param-value.md](src/TBC_FamilyParamValue/family-param-value.md) | [TBC_FamilyParamValue/family-param-value.json](src/TBC_FamilyParamValue/family-param-value.json) | `TBC_FamilyParamValue/CmdFamilyParamValue.cs` | 3 |
| [x] | [x] | `CmdFilledRegionCoords` | [TBC_FilledRegionCoords/filled-region-coords.md](src/TBC_FilledRegionCoords/filled-region-coords.md) | [TBC_FilledRegionCoords/filled-region-coords.json](src/TBC_FilledRegionCoords/filled-region-coords.json) | `TBC_FilledRegionCoords/CmdFilledRegionCoords.cs` | 3 |
| [x] | [x] | `CmdFlatten` | [TBC_Flatten/flatten.md](src/TBC_Flatten/flatten.md) | [TBC_Flatten/flatten.json](src/TBC_Flatten/flatten.json) | `TBC_Flatten/CmdFlatten.cs` | 3 |
| [x] | [x] | `CmdFlowMismatch` | [TBC_FlowMismatch/flow-mismatch.md](src/TBC_FlowMismatch/flow-mismatch.md) | [TBC_FlowMismatch/flow-mismatch.json](src/TBC_FlowMismatch/flow-mismatch.json) | `TBC_FlowMismatch/CmdFlowMismatch.cs` | 3 |
| [x] | [x] | `CmdGetColumnGeometry1` | [TBC_GetColumnGeometry1/get-column-geometry1.md](src/TBC_GetColumnGeometry1/get-column-geometry1.md) | [TBC_GetColumnGeometry1/get-column-geometry1.json](src/TBC_GetColumnGeometry1/get-column-geometry1.json) | `TBC_GetColumnGeometry1/CmdColumnRound.cs` | 3 |
| [x] | [x] | `CmdGetDimensionPoints` | [TBC_GetDimensionPoints/get-dimension-points.md](src/TBC_GetDimensionPoints/get-dimension-points.md) | [TBC_GetDimensionPoints/get-dimension-points.json](src/TBC_GetDimensionPoints/get-dimension-points.json) | `TBC_GetDimensionPoints/CmdGetDimensionPoints.cs` | 3 |
| [x] | [x] | `CmdGetMaterials` | [TBC_GetMaterials/get-materials.md](src/TBC_GetMaterials/get-materials.md) | [TBC_GetMaterials/get-materials.json](src/TBC_GetMaterials/get-materials.json) | `TBC_GetMaterials/CmdGetMaterials.cs` | 3 |
| [x] | [x] | `CmdGetSketchElements` | [TBC_GetSketchElements/get-sketch-elements.md](src/TBC_GetSketchElements/get-sketch-elements.md) | [TBC_GetSketchElements/get-sketch-elements.json](src/TBC_GetSketchElements/get-sketch-elements.json) | `TBC_GetSketchElements/CmdGetSketchElements.cs` | 3 |
| [x] | [x] | `CmdIdling` | [TBC_Idling/idling.md](src/TBC_Idling/idling.md) | [TBC_Idling/idling.json](src/TBC_Idling/idling.json) | `TBC_Idling/CmdIdling.cs` | 3 |
| [x] | [x] | `CmdImportsInFamilies` | [TBC_ImportsInFamilies/imports-in-families.md](src/TBC_ImportsInFamilies/imports-in-families.md) | [TBC_ImportsInFamilies/imports-in-families.json](src/TBC_ImportsInFamilies/imports-in-families.json) | `TBC_ImportsInFamilies/CmdImportsInFamilies.cs` | 3 |
| [x] | [x] | `CmdInstallLocation` | [TBC_InstallLocation/install-location.md](src/TBC_InstallLocation/install-location.md) | [TBC_InstallLocation/install-location.json](src/TBC_InstallLocation/install-location.json) | `TBC_InstallLocation/CmdInstallLocation.cs` | 3 |
| [x] | [x] | `CmdIntersectJunctionBox` | [TBC_IntersectJunctionBox/intersect-junction-box.md](src/TBC_IntersectJunctionBox/intersect-junction-box.md) | [TBC_IntersectJunctionBox/intersect-junction-box.json](src/TBC_IntersectJunctionBox/intersect-junction-box.json) | `TBC_IntersectJunctionBox/CmdIntersectJunctionBox.cs` | 3 |
| [x] | [x] | `CmdItemExecuted` | [TBC_ItemExecuted/item-executed.md](src/TBC_ItemExecuted/item-executed.md) | [TBC_ItemExecuted/item-executed.json](src/TBC_ItemExecuted/item-executed.json) | `TBC_ItemExecuted/CmdItemExecuted.cs` | 3 |
| [x] | [x] | `CmdLandXml` | [TBC_LandXml/land-xml.md](src/TBC_LandXml/land-xml.md) | [TBC_LandXml/land-xml.json](src/TBC_LandXml/land-xml.json) | `TBC_LandXml/CmdLandXml.cs` | 3 |
| [x] | [x] | `CmdLibraryPaths` | [TBC_LibraryPaths/library-paths.md](src/TBC_LibraryPaths/library-paths.md) | [TBC_LibraryPaths/library-paths.json](src/TBC_LibraryPaths/library-paths.json) | `TBC_LibraryPaths/CmdLibraryPaths.cs` | 3 |
| [x] | [x] | `CmdLinkedFileElements` | [TBC_LinkedFileElements/linked-file-elements.md](src/TBC_LinkedFileElements/linked-file-elements.md) | [TBC_LinkedFileElements/linked-file-elements.json](src/TBC_LinkedFileElements/linked-file-elements.json) | `TBC_LinkedFileElements/CmdLinkedFileElements.cs` | 3 |
| [x] | [x] | `CmdLinkedFiles` | [TBC_LinkedFiles/linked-files.md](src/TBC_LinkedFiles/linked-files.md) | [TBC_LinkedFiles/linked-files.json](src/TBC_LinkedFiles/linked-files.json) | `TBC_LinkedFiles/CmdLinkedFiles.cs` | 3 |
| [x] | [x] | `CmdLinq` | [TBC_Linq/linq.md](src/TBC_Linq/linq.md) | [TBC_Linq/linq.json](src/TBC_Linq/linq.json) | `TBC_Linq/CmdLinq.cs` | 3 |
| [x] | [x] | `CmdListAllRooms` | [TBC_ListAllRooms/list-all-rooms.md](src/TBC_ListAllRooms/list-all-rooms.md) | [TBC_ListAllRooms/list-all-rooms.json](src/TBC_ListAllRooms/list-all-rooms.json) | `TBC_ListAllRooms/CmdListAllRooms.cs` | 3 |
| [x] | [x] | `CmdListMarks` | [TBC_ListMarks/list-marks.md](src/TBC_ListMarks/list-marks.md) | [TBC_ListMarks/list-marks.json](src/TBC_ListMarks/list-marks.json) | `TBC_ListMarks/CmdListMarks.cs` | 3 |
| [x] | [x] | `CmdListPipeSizes` | [TBC_ListPipeSizes/list-pipe-sizes.md](src/TBC_ListPipeSizes/list-pipe-sizes.md) | [TBC_ListPipeSizes/list-pipe-sizes.json](src/TBC_ListPipeSizes/list-pipe-sizes.json) | `TBC_ListPipeSizes/CmdListPipeSizes.cs` | 3 |
| [x] | [x] | `CmdListRailingTypes` | [TBC_ListRailingTypes/list-railing-types.md](src/TBC_ListRailingTypes/list-railing-types.md) | [TBC_ListRailingTypes/list-railing-types.json](src/TBC_ListRailingTypes/list-railing-types.json) | `TBC_ListRailingTypes/CmdListRailingTypes.cs` | 3 |
| [x] | [x] | `CmdListSharedParams` | [TBC_ListSharedParams/list-shared-params.md](src/TBC_ListSharedParams/list-shared-params.md) | [TBC_ListSharedParams/list-shared-params.json](src/TBC_ListSharedParams/list-shared-params.json) | `TBC_ListSharedParams/CmdListSharedParams.cs` | 3 |
| [x] | [x] | `CmdListViews` | [TBC_ListViews/list-views.md](src/TBC_ListViews/list-views.md) | [TBC_ListViews/list-views.json](src/TBC_ListViews/list-views.json) | `TBC_ListViews/CmdListViews.cs` | 3 |
| [x] | [x] | `CmdListWalls` | [TBC_ListWalls/list-walls.md](src/TBC_ListWalls/list-walls.md) | [TBC_ListWalls/list-walls.json](src/TBC_ListWalls/list-walls.json) | `TBC_ListWalls/CmdListWalls.cs` | 3 |
| [x] | [x] | `CmdMepElementShape` | [TBC_MepElementShape/mep-element-shape.md](src/TBC_MepElementShape/mep-element-shape.md) | [TBC_MepElementShape/mep-element-shape.json](src/TBC_MepElementShape/mep-element-shape.json) | `TBC_MepElementShape/CmdMepElementShape.cs` | 3 |
| [x] | [x] | `CmdMidCurve` | [TBC_MidCurve/mid-curve.md](src/TBC_MidCurve/mid-curve.md) | [TBC_MidCurve/mid-curve.json](src/TBC_MidCurve/mid-curve.json) | `TBC_MidCurve/CmdMidCurve.cs` | 3 |
| [x] | [x] | `CmdMiroTest2` | [TBC_MiroTest2/miro-test2.md](src/TBC_MiroTest2/miro-test2.md) | [TBC_MiroTest2/miro-test2.json](src/TBC_MiroTest2/miro-test2.json) | `TBC_MiroTest2/CmdSheetToModel.cs` | 3 |
| [x] | [x] | `CmdMirror` | [TBC_Mirror/mirror.md](src/TBC_Mirror/mirror.md) | [TBC_Mirror/mirror.json](src/TBC_Mirror/mirror.json) | `TBC_Mirror/CmdMirror.cs` | 3 |
| [x] | [x] | `CmdMirrorListAdded` | [TBC_MirrorListAdded/mirror-list-added.md](src/TBC_MirrorListAdded/mirror-list-added.md) | [TBC_MirrorListAdded/mirror-list-added.json](src/TBC_MirrorListAdded/mirror-list-added.json) | `TBC_MirrorListAdded/CmdMirror.cs` | 3 |
| [x] | [x] | `CmdMultistoryStairSubelements` | [TBC_MultistoryStairSubelements/multistory-stair-subelements.md](src/TBC_MultistoryStairSubelements/multistory-stair-subelements.md) | [TBC_MultistoryStairSubelements/multistory-stair-subelements.json](src/TBC_MultistoryStairSubelements/multistory-stair-subelements.json) | `TBC_MultistoryStairSubelements/CmdMultiStoryStairSubelements.cs` | 3 |
| [x] | [x] | `CmdNamedGuidStorage` | [TBC_NamedGuidStorage/named-guid-storage.md](src/TBC_NamedGuidStorage/named-guid-storage.md) | [TBC_NamedGuidStorage/named-guid-storage.json](src/TBC_NamedGuidStorage/named-guid-storage.json) | `TBC_NamedGuidStorage/CmdNamedGuidStorage.cs` | 3 |
| [x] | [x] | `CmdNestedFamilies` | [TBC_NestedFamilies/nested-families.md](src/TBC_NestedFamilies/nested-families.md) | [TBC_NestedFamilies/nested-families.json](src/TBC_NestedFamilies/nested-families.json) | `TBC_NestedFamilies/CmdNestedFamilies.cs` | 3 |
| [x] | [x] | `CmdNestedInstanceGeo` | [TBC_NestedInstanceGeo/nested-instance-geo.md](src/TBC_NestedInstanceGeo/nested-instance-geo.md) | [TBC_NestedInstanceGeo/nested-instance-geo.json](src/TBC_NestedInstanceGeo/nested-instance-geo.json) | `TBC_NestedInstanceGeo/CmdNestedInstanceGeo.cs` | 3 |
| [x] | [x] | `CmdNewArea` | [TBC_NewArea/new-area.md](src/TBC_NewArea/new-area.md) | [TBC_NewArea/new-area.json](src/TBC_NewArea/new-area.json) | `TBC_NewArea/CmdNewArea.cs` | 3 |
| [x] | [x] | `CmdNewBeamTypeInstance` | [TBC_NewBeamTypeInstance/new-beam-type-instance.md](src/TBC_NewBeamTypeInstance/new-beam-type-instance.md) | [TBC_NewBeamTypeInstance/new-beam-type-instance.json](src/TBC_NewBeamTypeInstance/new-beam-type-instance.json) | `TBC_NewBeamTypeInstance/CmdNewBeamTypeInstance.cs` | 3 |
| [x] | [x] | `CmdNewBlend` | [TBC_NewBlend/new-blend.md](src/TBC_NewBlend/new-blend.md) | [TBC_NewBlend/new-blend.json](src/TBC_NewBlend/new-blend.json) | `TBC_NewBlend/CmdNewBlend.cs` | 3 |
| [x] | [x] | `CmdNewColumnTypeInstance` | [TBC_NewColumnTypeInstance/new-column-type-instance.md](src/TBC_NewColumnTypeInstance/new-column-type-instance.md) | [TBC_NewColumnTypeInstance/new-column-type-instance.json](src/TBC_NewColumnTypeInstance/new-column-type-instance.json) | `TBC_NewColumnTypeInstance/CmdNewColumnTypeInstance.cs` | 3 |
| [x] | [x] | `CmdNewCrossFitting` | [TBC_NewCrossFitting/new-cross-fitting.md](src/TBC_NewCrossFitting/new-cross-fitting.md) | [TBC_NewCrossFitting/new-cross-fitting.json](src/TBC_NewCrossFitting/new-cross-fitting.json) | `TBC_NewCrossFitting/CmdNewCrossFitting.cs` | 3 |
| [x] | [x] | `CmdNewDimensionLabel` | [TBC_NewDimensionLabel/new-dimension-label.md](src/TBC_NewDimensionLabel/new-dimension-label.md) | [TBC_NewDimensionLabel/new-dimension-label.json](src/TBC_NewDimensionLabel/new-dimension-label.json) | `TBC_NewDimensionLabel/CmdNewDimensionLabel.cs` | 3 |
| [x] | [x] | `CmdNewDuctSystem` | [TBC_NewDuctSystem/new-duct-system.md](src/TBC_NewDuctSystem/new-duct-system.md) | [TBC_NewDuctSystem/new-duct-system.json](src/TBC_NewDuctSystem/new-duct-system.json) | `TBC_NewDuctSystem/CmdNewDuctSystem.cs` | 3 |
| [x] | [x] | `CmdNewExtrusionRoof` | [TBC_NewExtrusionRoof/new-extrusion-roof.md](src/TBC_NewExtrusionRoof/new-extrusion-roof.md) | [TBC_NewExtrusionRoof/new-extrusion-roof.json](src/TBC_NewExtrusionRoof/new-extrusion-roof.json) | `TBC_NewExtrusionRoof/CmdNewExtrusionRoof.cs` | 3 |
| [x] | [x] | `CmdNewLightingFixture` | [TBC_NewLightingFixture/new-lighting-fixture.md](src/TBC_NewLightingFixture/new-lighting-fixture.md) | [TBC_NewLightingFixture/new-lighting-fixture.json](src/TBC_NewLightingFixture/new-lighting-fixture.json) | `TBC_NewLightingFixture/CmdNewLightingFixture.cs` | 3 |
| [x] | [x] | `CmdNewLineLoad` | [TBC_NewLineLoad/new-line-load.md](src/TBC_NewLineLoad/new-line-load.md) | [TBC_NewLineLoad/new-line-load.json](src/TBC_NewLineLoad/new-line-load.json) | `TBC_NewLineLoad/CmdNewLineLoad.cs` | 3 |
| [x] | [x] | `CmdNewProjectDoc` | [TBC_NewProjectDoc/new-project-doc.md](src/TBC_NewProjectDoc/new-project-doc.md) | [TBC_NewProjectDoc/new-project-doc.json](src/TBC_NewProjectDoc/new-project-doc.json) | `TBC_NewProjectDoc/CmdNewProjectDoc.cs` | 3 |
| [x] | [x] | `CmdNewRailing` | [TBC_NewRailing/new-railing.md](src/TBC_NewRailing/new-railing.md) | [TBC_NewRailing/new-railing.json](src/TBC_NewRailing/new-railing.json) | `TBC_NewRailing/CmdNewRailing.cs` | 3 |
| [x] | [x] | `CmdNewSpotElevation` | [TBC_NewSpotElevation/new-spot-elevation.md](src/TBC_NewSpotElevation/new-spot-elevation.md) | [TBC_NewSpotElevation/new-spot-elevation.json](src/TBC_NewSpotElevation/new-spot-elevation.json) | `TBC_NewSpotElevation/CmdNewSpotElevation.cs` | 3 |
| [x] | [x] | `CmdNewSprinkler` | [TBC_NewSprinkler/new-sprinkler.md](src/TBC_NewSprinkler/new-sprinkler.md) | [TBC_NewSprinkler/new-sprinkler.json](src/TBC_NewSprinkler/new-sprinkler.json) | `TBC_NewSprinkler/CmdNewSprinkler.cs` | 3 |
| [x] | [x] | `CmdNewSweptBlend` | [TBC_NewSweptBlend/new-swept-blend.md](src/TBC_NewSweptBlend/new-swept-blend.md) | [TBC_NewSweptBlend/new-swept-blend.json](src/TBC_NewSweptBlend/new-swept-blend.json) | `TBC_NewSweptBlend/CmdNewSweptBlend.cs` | 3 |
| [x] | [x] | `CmdNewTextNote` | [TBC_NewTextNote/new-text-note.md](src/TBC_NewTextNote/new-text-note.md) | [TBC_NewTextNote/new-text-note.json](src/TBC_NewTextNote/new-text-note.json) | `TBC_NewTextNote/CmdNewTextNote.cs` | 3 |
| [x] | [x] | `CmdNewWallLayer` | [TBC_NewWallLayer/new-wall-layer.md](src/TBC_NewWallLayer/new-wall-layer.md) | [TBC_NewWallLayer/new-wall-layer.json](src/TBC_NewWallLayer/new-wall-layer.json) | `TBC_NewWallLayer/CmdNewWallLayer.cs` | 3 |
| [x] | [x] | `CmdOmniClassParams` | [TBC_OmniClassParams/omni-class-params.md](src/TBC_OmniClassParams/omni-class-params.md) | [TBC_OmniClassParams/omni-class-params.json](src/TBC_OmniClassParams/omni-class-params.json) | `TBC_OmniClassParams/CmdOmniClassParams.cs` | 3 |
| [x] | [x] | `ParamFilterTest` | [TBC_ParamFilterTest/param-filter-test.md](src/TBC_ParamFilterTest/param-filter-test.md) | [TBC_ParamFilterTest/param-filter-test.json](src/TBC_ParamFilterTest/param-filter-test.json) | `TBC_ParamFilterTest/CmdCollectorPerformance.cs` | 3 |
| [x] | [x] | `CmdParamValuesForCats` | [TBC_ParamValuesForCats/param-values-for-cats.md](src/TBC_ParamValuesForCats/param-values-for-cats.md) | [TBC_ParamValuesForCats/param-values-for-cats.json](src/TBC_ParamValuesForCats/param-values-for-cats.json) | `TBC_ParamValuesForCats/CmdParamValuesForCats.cs` | 3 |
| [x] | [x] | `CmdParameterUnitConverter` | [TBC_ParameterUnitConverter/parameter-unit-converter.md](src/TBC_ParameterUnitConverter/parameter-unit-converter.md) | [TBC_ParameterUnitConverter/parameter-unit-converter.json](src/TBC_ParameterUnitConverter/parameter-unit-converter.json) | `TBC_ParameterUnitConverter/CmdParameterUnitConverter.cs` | 3 |
| [x] | [x] | `CmdPartAtom` | [TBC_PartAtom/part-atom.md](src/TBC_PartAtom/part-atom.md) | [TBC_PartAtom/part-atom.json](src/TBC_PartAtom/part-atom.json) | `TBC_PartAtom/CmdPartAtom.cs` | 3 |
| [x] | [x] | `CmdPickPoint3d` | [TBC_PickPoint3d/pick-point3d.md](src/TBC_PickPoint3d/pick-point3d.md) | [TBC_PickPoint3d/pick-point3d.json](src/TBC_PickPoint3d/pick-point3d.json) | `TBC_PickPoint3d/CmdPickPoint3d.cs` | 3 |
| [x] | [x] | `CmdPickRoomInclLinked` | [TBC_PickRoomInclLinked/pick-room-incl-linked.md](src/TBC_PickRoomInclLinked/pick-room-incl-linked.md) | [TBC_PickRoomInclLinked/pick-room-incl-linked.json](src/TBC_PickRoomInclLinked/pick-room-incl-linked.json) | `TBC_PickRoomInclLinked/CmdPickRoomInclLinked.cs` | 3 |
| [x] | [x] | `CmdPlaceFamilyInstance` | [TBC_PlaceFamilyInstance/place-family-instance.md](src/TBC_PlaceFamilyInstance/place-family-instance.md) | [TBC_PlaceFamilyInstance/place-family-instance.json](src/TBC_PlaceFamilyInstance/place-family-instance.json) | `TBC_PlaceFamilyInstance/CmdPlaceFamilyInstance.cs` | 3 |
| [x] | [x] | `CmdPlanTopology` | [TBC_PlanTopology/plan-topology.md](src/TBC_PlanTopology/plan-topology.md) | [TBC_PlanTopology/plan-topology.json](src/TBC_PlanTopology/plan-topology.json) | `TBC_PlanTopology/CmdPlanTopology.cs` | 3 |
| [x] | [x] | `CmdPostRequestInstancePlacement` | [TBC_PostRequestInstancePlacement/post-request-instance-placement.md](src/TBC_PostRequestInstancePlacement/post-request-instance-placement.md) | [TBC_PostRequestInstancePlacement/post-request-instance-placement.json](src/TBC_PostRequestInstancePlacement/post-request-instance-placement.json) | `TBC_PostRequestInstancePlacement/CmdPostRequestInstancePlacement.cs` | 3 |
| [x] | [x] | `CmdPreprocessFailure` | [TBC_PreprocessFailure/preprocess-failure.md](src/TBC_PreprocessFailure/preprocess-failure.md) | [TBC_PreprocessFailure/preprocess-failure.json](src/TBC_PreprocessFailure/preprocess-failure.json) | `TBC_PreprocessFailure/CmdPreprocessFailure.cs` | 3 |
| [x] | [x] | `CmdPressKeys` | [TBC_PressKeys/press-keys.md](src/TBC_PressKeys/press-keys.md) | [TBC_PressKeys/press-keys.json](src/TBC_PressKeys/press-keys.json) | `TBC_PressKeys/CmdPressKeys.cs` | 3 |
| [x] | [x] | `CmdPreviewImage` | [TBC_PreviewImage/preview-image.md](src/TBC_PreviewImage/preview-image.md) | [TBC_PreviewImage/preview-image.json](src/TBC_PreviewImage/preview-image.json) | `TBC_PreviewImage/CmdPreviewImage.cs` | 3 |
| [x] | [x] | `CmdProcessVisibleDwg` | [TBC_ProcessVisibleDwg/process-visible-dwg.md](src/TBC_ProcessVisibleDwg/process-visible-dwg.md) | [TBC_ProcessVisibleDwg/process-visible-dwg.json](src/TBC_ProcessVisibleDwg/process-visible-dwg.json) | `TBC_ProcessVisibleDwg/CmdProcessVisibleDwg.cs` | 3 |
| [x] | [x] | `CmdProjectParameterGuids` | [TBC_ProjectParameterGuids/project-parameter-guids.md](src/TBC_ProjectParameterGuids/project-parameter-guids.md) | [TBC_ProjectParameterGuids/project-parameter-guids.json](src/TBC_ProjectParameterGuids/project-parameter-guids.json) | `TBC_ProjectParameterGuids/CmdProjectParameterGuids.cs` | 3 |
| [x] | [x] | `CmdPurgeLineStyles` | [TBC_PurgeLineStyles/purge-line-styles.md](src/TBC_PurgeLineStyles/purge-line-styles.md) | [TBC_PurgeLineStyles/purge-line-styles.json](src/TBC_PurgeLineStyles/purge-line-styles.json) | `TBC_PurgeLineStyles/CmdPurgeLineStyles.cs` | 3 |
| [x] | [x] | `CmdPurgeTextNoteTypes` | [TBC_PurgeTextNoteTypes/purge-text-note-types.md](src/TBC_PurgeTextNoteTypes/purge-text-note-types.md) | [TBC_PurgeTextNoteTypes/purge-text-note-types.json](src/TBC_PurgeTextNoteTypes/purge-text-note-types.json) | `TBC_PurgeTextNoteTypes/CmdPurgeTextNoteTypes.cs` | 3 |
| [x] | [x] | `CmdRebarCurves` | [TBC_RebarCurves/rebar-curves.md](src/TBC_RebarCurves/rebar-curves.md) | [TBC_RebarCurves/rebar-curves.json](src/TBC_RebarCurves/rebar-curves.json) | `TBC_RebarCurves/CmdRebarCurves.cs` | 3 |
| [x] | [x] | `CmdRectDuctCorners` | [TBC_RectDuctCorners/rect-duct-corners.md](src/TBC_RectDuctCorners/rect-duct-corners.md) | [TBC_RectDuctCorners/rect-duct-corners.json](src/TBC_RectDuctCorners/rect-duct-corners.json) | `TBC_RectDuctCorners/CmdRectDuctCorners.cs` | 3 |
| [x] | [x] | `CmdRelationshipInverter` | [TBC_RelationshipInverter/relationship-inverter.md](src/TBC_RelationshipInverter/relationship-inverter.md) | [TBC_RelationshipInverter/relationship-inverter.json](src/TBC_RelationshipInverter/relationship-inverter.json) | `TBC_RelationshipInverter/CmdRelationshipInverter.cs` | 3 |
| [x] | [x] | `CmdRemoveDwfLinks` | [TBC_RemoveDwfLinks/remove-dwf-links.md](src/TBC_RemoveDwfLinks/remove-dwf-links.md) | [TBC_RemoveDwfLinks/remove-dwf-links.json](src/TBC_RemoveDwfLinks/remove-dwf-links.json) | `TBC_RemoveDwfLinks/CmdRemoveDwfLinks.cs` | 3 |
| [x] | [x] | `CmdRemoveImportedJpgs` | [TBC_RemoveImportedJpgs/remove-imported-jpgs.md](src/TBC_RemoveImportedJpgs/remove-imported-jpgs.md) | [TBC_RemoveImportedJpgs/remove-imported-jpgs.json](src/TBC_RemoveImportedJpgs/remove-imported-jpgs.json) | `TBC_RemoveImportedJpgs/CmdRemoveImportedJpgs.cs` | 3 |
| [x] | [x] | `RetrievingMaterialCommand` | [TBC_RetrievingMaterialCommand/retrieving-material-command.md](src/TBC_RetrievingMaterialCommand/retrieving-material-command.md) | [TBC_RetrievingMaterialCommand/retrieving-material-command.json](src/TBC_RetrievingMaterialCommand/retrieving-material-command.json) | `TBC_RetrievingMaterialCommand/CmdGetMaterials.cs` | 3 |
| [x] | [x] | `RevitCommand` | [TBC_RevitCommand/revit-command.md](src/TBC_RevitCommand/revit-command.md) | [TBC_RevitCommand/revit-command.json](src/TBC_RevitCommand/revit-command.json) | `TBC_RevitCommand/CmdCollectorPerformance.cs` | 3 |
| [x] | [x] | `CmdRollingOffset` | [TBC_RollingOffset/rolling-offset.md](src/TBC_RollingOffset/rolling-offset.md) | [TBC_RollingOffset/rolling-offset.json](src/TBC_RollingOffset/rolling-offset.json) | `TBC_RollingOffset/CmdRollingOffset.cs` | 3 |
| [x] | [x] | `CmdRoomNeighbours` | [TBC_RoomNeighbours/room-neighbours.md](src/TBC_RoomNeighbours/room-neighbours.md) | [TBC_RoomNeighbours/room-neighbours.json](src/TBC_RoomNeighbours/room-neighbours.json) | `TBC_RoomNeighbours/CmdRoomNeighbours.cs` | 3 |
| [x] | [x] | `CmdRoomWallAdjacency` | [TBC_RoomWallAdjacency/room-wall-adjacency.md](src/TBC_RoomWallAdjacency/room-wall-adjacency.md) | [TBC_RoomWallAdjacency/room-wall-adjacency.json](src/TBC_RoomWallAdjacency/room-wall-adjacency.json) | `TBC_RoomWallAdjacency/CmdRoomWallAdjacency.cs` | 3 |
| [x] | [x] | `CmdRotatedBeamLocation` | [TBC_RotatedBeamLocation/rotated-beam-location.md](src/TBC_RotatedBeamLocation/rotated-beam-location.md) | [TBC_RotatedBeamLocation/rotated-beam-location.json](src/TBC_RotatedBeamLocation/rotated-beam-location.json) | `TBC_RotatedBeamLocation/CmdRotatedBeamLocation.cs` | 3 |
| [x] | [x] | `CmdSelectionChanged` | [TBC_SelectionChanged/selection-changed.md](src/TBC_SelectionChanged/selection-changed.md) | [TBC_SelectionChanged/selection-changed.json](src/TBC_SelectionChanged/selection-changed.json) | `TBC_SelectionChanged/CmdSelectionChanged.cs` | 3 |
| [x] | [x] | `CmdSetGridEndpoint` | [TBC_SetGridEndpoint/set-grid-endpoint.md](src/TBC_SetGridEndpoint/set-grid-endpoint.md) | [TBC_SetGridEndpoint/set-grid-endpoint.json](src/TBC_SetGridEndpoint/set-grid-endpoint.json) | `TBC_SetGridEndpoint/CmdSetGridEndpoint.cs` | 3 |
| [x] | [x] | `CmdSetRoomOccupancy` | [TBC_SetRoomOccupancy/set-room-occupancy.md](src/TBC_SetRoomOccupancy/set-room-occupancy.md) | [TBC_SetRoomOccupancy/set-room-occupancy.json](src/TBC_SetRoomOccupancy/set-room-occupancy.json) | `TBC_SetRoomOccupancy/CmdSetRoomOccupancy.cs` | 3 |
| [x] | [x] | `CmdSetTagType` | [TBC_SetTagType/set-tag-type.md](src/TBC_SetTagType/set-tag-type.md) | [TBC_SetTagType/set-tag-type.json](src/TBC_SetTagType/set-tag-type.json) | `TBC_SetTagType/CmdSetTagType.cs` | 3 |
| [x] | [x] | `CmdSetTangentLock` | [TBC_SetTangentLock/set-tangent-lock.md](src/TBC_SetTangentLock/set-tangent-lock.md) | [TBC_SetTangentLock/set-tangent-lock.json](src/TBC_SetTangentLock/set-tangent-lock.json) | `TBC_SetTangentLock/CmdSetTangentLock.cs` | 3 |
| [x] | [x] | `CmdSetWallType` | [TBC_SetWallType/set-wall-type.md](src/TBC_SetWallType/set-wall-type.md) | [TBC_SetWallType/set-wall-type.json](src/TBC_SetWallType/set-wall-type.json) | `TBC_SetWallType/CmdSetWallType.cs` | 3 |
| [x] | [x] | `CmdSharedParamGuids` | [TBC_SharedParamGuids/shared-param-guids.md](src/TBC_SharedParamGuids/shared-param-guids.md) | [TBC_SharedParamGuids/shared-param-guids.json](src/TBC_SharedParamGuids/shared-param-guids.json) | `TBC_SharedParamGuids/CmdSharedParamGuids.cs` | 3 |
| [x] | [x] | `CmdSheetData` | [TBC_SheetData/sheet-data.md](src/TBC_SheetData/sheet-data.md) | [TBC_SheetData/sheet-data.json](src/TBC_SheetData/sheet-data.json) | `TBC_SheetData/CmdSheetData.cs` | 3 |
| [x] | [x] | `CmdSheetSize` | [TBC_SheetSize/sheet-size.md](src/TBC_SheetSize/sheet-size.md) | [TBC_SheetSize/sheet-size.json](src/TBC_SheetSize/sheet-size.json) | `TBC_SheetSize/CmdSheetSize.cs` | 3 |
| [x] | [x] | `CmdSheetToModel` | [TBC_SheetToModel/sheet-to-model.md](src/TBC_SheetToModel/sheet-to-model.md) | [TBC_SheetToModel/sheet-to-model.json](src/TBC_SheetToModel/sheet-to-model.json) | `TBC_SheetToModel/CmdSheetToModel.cs` | 3 |
| [x] | [x] | `CmdSimpleUpdaterExample` | [TBC_SimpleUpdaterExample/simple-updater-example.md](src/TBC_SimpleUpdaterExample/simple-updater-example.md) | [TBC_SimpleUpdaterExample/simple-updater-example.json](src/TBC_SimpleUpdaterExample/simple-updater-example.json) | `TBC_SimpleUpdaterExample/CmdElevationWatcher.cs` | 3 |
| [x] | [x] | `CmdSlabBoundary` | [TBC_SlabBoundary/slab-boundary.md](src/TBC_SlabBoundary/slab-boundary.md) | [TBC_SlabBoundary/slab-boundary.json](src/TBC_SlabBoundary/slab-boundary.json) | `TBC_SlabBoundary/CmdSlabBoundary.cs` | 3 |
| [x] | [x] | `CmdSlabBoundaryArea` | [TBC_SlabBoundaryArea/slab-boundary-area.md](src/TBC_SlabBoundaryArea/slab-boundary-area.md) | [TBC_SlabBoundaryArea/slab-boundary-area.json](src/TBC_SlabBoundaryArea/slab-boundary-area.json) | `TBC_SlabBoundaryArea/CmdSlabBoundaryArea.cs` | 3 |
| [x] | [x] | `CmdSlabSides` | [TBC_SlabSides/slab-sides.md](src/TBC_SlabSides/slab-sides.md) | [TBC_SlabSides/slab-sides.json](src/TBC_SlabSides/slab-sides.json) | `TBC_SlabSides/CmdSlabSides.cs` | 3 |
| [x] | [x] | `CmdSlopedWall` | [TBC_SlopedWall/sloped-wall.md](src/TBC_SlopedWall/sloped-wall.md) | [TBC_SlopedWall/sloped-wall.json](src/TBC_SlopedWall/sloped-wall.json) | `TBC_SlopedWall/CmdSlopedWall.cs` | 3 |
| [x] | [x] | `CmdSortCurveLoops` | [TBC_SortCurveLoops/sort-curve-loops.md](src/TBC_SortCurveLoops/sort-curve-loops.md) | [TBC_SortCurveLoops/sort-curve-loops.json](src/TBC_SortCurveLoops/sort-curve-loops.json) | `TBC_SortCurveLoops/CmdSortCurveLoops.cs` | 3 |
| [x] | [x] | `CmdSpaceAdjacency` | [TBC_SpaceAdjacency/space-adjacency.md](src/TBC_SpaceAdjacency/space-adjacency.md) | [TBC_SpaceAdjacency/space-adjacency.json](src/TBC_SpaceAdjacency/space-adjacency.json) | `TBC_SpaceAdjacency/CmdSpaceAdjacency.cs` | 3 |
| [x] | [x] | `CmdStatusBar` | [TBC_StatusBar/status-bar.md](src/TBC_StatusBar/status-bar.md) | [TBC_StatusBar/status-bar.json](src/TBC_StatusBar/status-bar.json) | `TBC_StatusBar/CmdStatusBar.cs` | 3 |
| [x] | [x] | `CmdSteelStairBeams` | [TBC_SteelStairBeams/steel-stair-beams.md](src/TBC_SteelStairBeams/steel-stair-beams.md) | [TBC_SteelStairBeams/steel-stair-beams.json](src/TBC_SteelStairBeams/steel-stair-beams.json) | `TBC_SteelStairBeams/CmdSteelStairBeams.cs` | 3 |
| [x] | [x] | `CmdSwitchDoc` | [TBC_SwitchDoc/switch-doc.md](src/TBC_SwitchDoc/switch-doc.md) | [TBC_SwitchDoc/switch-doc.json](src/TBC_SwitchDoc/switch-doc.json) | `TBC_SwitchDoc/CmdSwitchDoc.cs` | 3 |
| [x] | [x] | `TestWall` | [TBC_TestWall/test-wall.md](src/TBC_TestWall/test-wall.md) | [TBC_TestWall/test-wall.json](src/TBC_TestWall/test-wall.json) | `TBC_TestWall/CmdSlopedWall.cs` | 3 |
| [x] | [x] | `CmdTransformedCoords` | [TBC_TransformedCoords/transformed-coords.md](src/TBC_TransformedCoords/transformed-coords.md) | [TBC_TransformedCoords/transformed-coords.json](src/TBC_TransformedCoords/transformed-coords.json) | `TBC_TransformedCoords/CmdTransformedCoords.cs` | 3 |
| [x] | [x] | `CmdTriangleCount` | [TBC_TriangleCount/triangle-count.md](src/TBC_TriangleCount/triangle-count.md) | [TBC_TriangleCount/triangle-count.json](src/TBC_TriangleCount/triangle-count.json) | `TBC_TriangleCount/CmdTriangleCount.cs` | 3 |
| [x] | [x] | `CmdUnrotateNorth` | [TBC_UnrotateNorth/unrotate-north.md](src/TBC_UnrotateNorth/unrotate-north.md) | [TBC_UnrotateNorth/unrotate-north.json](src/TBC_UnrotateNorth/unrotate-north.json) | `TBC_UnrotateNorth/CmdUnrotateNorth.cs` | 3 |
| [x] | [x] | `CmdUpdateReferencingSheet` | [TBC_UpdateReferencingSheet/update-referencing-sheet.md](src/TBC_UpdateReferencingSheet/update-referencing-sheet.md) | [TBC_UpdateReferencingSheet/update-referencing-sheet.json](src/TBC_UpdateReferencingSheet/update-referencing-sheet.json) | `TBC_UpdateReferencingSheet/CmdUpdateReferencingSheet.cs` | 3 |
| [x] | [x] | `CmdViewsShowingElements` | [TBC_ViewsShowingElements/views-showing-elements.md](src/TBC_ViewsShowingElements/views-showing-elements.md) | [TBC_ViewsShowingElements/views-showing-elements.json](src/TBC_ViewsShowingElements/views-showing-elements.json) | `TBC_ViewsShowingElements/CmdViewsShowingElements.cs` | 3 |
| [x] | [x] | `CmdWallBottomFace` | [TBC_WallBottomFace/wall-bottom-face.md](src/TBC_WallBottomFace/wall-bottom-face.md) | [TBC_WallBottomFace/wall-bottom-face.json](src/TBC_WallBottomFace/wall-bottom-face.json) | `TBC_WallBottomFace/CmdWallBottomFace.cs` | 3 |
| [x] | [x] | `CmdWallDimensions` | [TBC_WallDimensions/wall-dimensions.md](src/TBC_WallDimensions/wall-dimensions.md) | [TBC_WallDimensions/wall-dimensions.json](src/TBC_WallDimensions/wall-dimensions.json) | `TBC_WallDimensions/CmdWallDimensions.cs` | 3 |
| [x] | [x] | `CmdWallFooting` | [TBC_WallFooting/wall-footing.md](src/TBC_WallFooting/wall-footing.md) | [TBC_WallFooting/wall-footing.json](src/TBC_WallFooting/wall-footing.json) | `TBC_WallFooting/CmdWallFooting.cs` | 3 |
| [x] | [x] | `CmdWallLayerVolumes` | [TBC_WallLayerVolumes/wall-layer-volumes.md](src/TBC_WallLayerVolumes/wall-layer-volumes.md) | [TBC_WallLayerVolumes/wall-layer-volumes.json](src/TBC_WallLayerVolumes/wall-layer-volumes.json) | `TBC_WallLayerVolumes/CmdWallLayerVolumes.cs` | 3 |
| [x] | [x] | `CmdWallLayers` | [TBC_WallLayers/wall-layers.md](src/TBC_WallLayers/wall-layers.md) | [TBC_WallLayers/wall-layers.json](src/TBC_WallLayers/wall-layers.json) | `TBC_WallLayers/CmdWallLayers.cs` | 3 |
| [x] | [x] | `CmdWallNeighbours` | [TBC_WallNeighbours/wall-neighbours.md](src/TBC_WallNeighbours/wall-neighbours.md) | [TBC_WallNeighbours/wall-neighbours.json](src/TBC_WallNeighbours/wall-neighbours.json) | `TBC_WallNeighbours/CmdWallNeighbours.cs` | 3 |
| [x] | [x] | `CmdWallOpeningProfiles` | [TBC_WallOpeningProfiles/wall-opening-profiles.md](src/TBC_WallOpeningProfiles/wall-opening-profiles.md) | [TBC_WallOpeningProfiles/wall-opening-profiles.json](src/TBC_WallOpeningProfiles/wall-opening-profiles.json) | `TBC_WallOpeningProfiles/CmdWallOpeningProfiles.cs` | 3 |
| [x] | [x] | `CmdWallOpenings` | [TBC_WallOpenings/wall-openings.md](src/TBC_WallOpenings/wall-openings.md) | [TBC_WallOpenings/wall-openings.json](src/TBC_WallOpenings/wall-openings.json) | `TBC_WallOpenings/CmdWallOpenings.cs` | 3 |
| [x] | [x] | `CmdWallProfile` | [TBC_WallProfile/wall-profile.md](src/TBC_WallProfile/wall-profile.md) | [TBC_WallProfile/wall-profile.json](src/TBC_WallProfile/wall-profile.json) | `TBC_WallProfile/CmdWallProfile.cs` | 3 |
| [x] | [x] | `CmdWallProfileArea` | [TBC_WallProfileArea/wall-profile-area.md](src/TBC_WallProfileArea/wall-profile-area.md) | [TBC_WallProfileArea/wall-profile-area.json](src/TBC_WallProfileArea/wall-profile-area.json) | `TBC_WallProfileArea/CmdWallProfileArea.cs` | 3 |
| [x] | [x] | `CmdWallTopFaces` | [TBC_WallTopFaces/wall-top-faces.md](src/TBC_WallTopFaces/wall-top-faces.md) | [TBC_WallTopFaces/wall-top-faces.json](src/TBC_WallTopFaces/wall-top-faces.json) | `TBC_WallTopFaces/CmdWallTopFaces.cs` | 3 |
| [x] | [x] | `CmdWindowHandle` | [TBC_WindowHandle/window-handle.md](src/TBC_WindowHandle/window-handle.md) | [TBC_WindowHandle/window-handle.json](src/TBC_WindowHandle/window-handle.json) | `TBC_WindowHandle/CmdWindowHandle.cs` | 3 |
