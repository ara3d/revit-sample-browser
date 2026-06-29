#!/usr/bin/env python3
"""Consolidate per-sample _utils.cs files into src/Common/."""
import hashlib
import os
import re
import shutil
from collections import defaultdict
from pathlib import Path

SRC = Path(r"c:\Users\cdigg\git\revit-sample-browser\src")
COMMON = SRC / "Common"
CSPROJ = SRC / "Ara3D.RevitSampleBrowser.csproj"

# Method name -> (area, class_name)
METHOD_MAP = {
    # XyzMath
    "SubXyz": ("Geometry", "XyzMath"),
    "AddXyz": ("Geometry", "XyzMath"),
    "DotMatrix": ("Geometry", "XyzMath"),
    "CrossMatrix": ("Geometry", "XyzMath"),
    "IsEqual": ("Geometry", "XyzMath"),
    "GetLength": ("Geometry", "XyzMath"),
    "UnitVector": ("Geometry", "XyzMath"),
    "MultiplyVector": ("Geometry", "XyzMath"),
    "TransformPoint": ("Geometry", "XyzMath"),
    "OffsetPoint": ("Geometry", "XyzMath"),
    "IsSameDirection": ("Geometry", "XyzMath"),
    "IsOppositeDirection": ("Geometry", "XyzMath"),
    "XyzToString": ("Geometry", "XyzMath"),
    "MoveXyzToElevation": ("Geometry", "XyzMath"),
    "DoubleEquals": ("Geometry", "XyzMath"),
    # CurveGeometry
    "GetTangentAt": ("Geometry", "CurveGeometry"),
    "ComputeParameter": ("Geometry", "CurveGeometry"),
    "ProjectCurveToElevation": ("Geometry", "CurveGeometry"),
    "ProjectCurvesToElevation": ("Geometry", "CurveGeometry"),
    "TransformCurve": ("Geometry", "CurveGeometry"),
    "TransformCurves": ("Geometry", "CurveGeometry"),
    "FindLongestEndpointConnection": ("Geometry", "CurveGeometry"),
    "HasCommonEndPoint": ("Geometry", "CurveGeometry"),
    "IsVertical": ("Geometry", "CurveGeometry"),
    # Point2D
    "Distance2D": ("Geometry", "Point2DMath"),
    "Sub2D": ("Geometry", "Point2DMath"),
    "Add2D": ("Geometry", "Point2DMath"),
    "Dot2D": ("Geometry", "Point2DMath"),
    "Cross2D": ("Geometry", "Point2DMath"),
    "Normalize2D": ("Geometry", "Point2DMath"),
    # PlaneAndTransform
    "GetTransform": ("Geometry", "PlaneAndTransform"),
    "GetPlane": ("Geometry", "PlaneAndTransform"),
    "GetNormalToWallAt": ("Geometry", "PlaneAndTransform"),
    "GetWallDeltaAt": ("Geometry", "PlaneAndTransform"),
  "CheckOrientation": ("Geometry", "PlaneAndTransform"),
    # FaceAndSolidGeometry
    "TryGetDemoSolids": ("Geometry", "FaceAndSolidGeometry"),
    "GetSolidFromElement": ("Geometry", "FaceAndSolidGeometry"),
    "GetFaces": ("Geometry", "FaceAndSolidGeometry"),
    # Units
    "CovertFromApi": ("Units", "UnitConversion"),
    "CovertToApi": ("Units", "UnitConversion"),
    "GetUnitLabel": ("Units", "UnitConversion"),
    "ConvertValueDocumentUnits": ("Units", "DocumentUnits"),
    "ConvertValueToFeet": ("Units", "DocumentUnits"),
    "FormatNumber": ("Units", "ValueFormatting"),
    "FormatFractionalInches": ("Units", "ValueFormatting"),
    "FormatParameterLine": ("Units", "ValueFormatting"),
    "AngleStringToDouble": ("Units", "ValueFormatting"),
    "DoubleToAngleString": ("Units", "ValueFormatting"),
    "TimeZoneStringToDouble": ("Units", "ValueFormatting"),
    "TimeZoneDoubleToString": ("Units", "ValueFormatting"),
    # Parameters
    "SetParameter": ("Parameters", "ParameterAccess"),
    "FindParameter": ("Parameters", "ParameterAccess"),
    "SetParaNullId": ("Parameters", "ParameterAccess"),
    "SetParaInt": ("Parameters", "ParameterAccess"),
    "FindParaByName": ("Parameters", "ParameterAccess"),
    "FindFamilySymbol": ("Parameters", "ParameterAccess"),
    # SharedParameterHelper
    "AddSharedParameter": ("Parameters", "SharedParameterHelper"),
    "GetOrCreateSharedParameter": ("Parameters", "SharedParameterHelper"),
    # Document
    "GetViewFilters": ("Document", "FilterBuilder"),
    "CreateFilterRuleBuilder": ("Document", "FilterBuilder"),
    "ReflectToInnerRule": ("Document", "FilterBuilder"),
    "CreateElementFilterFromFilterRules": ("Document", "FilterBuilder"),
    "GetConjunctionOfFilterRulesFromElementFilter": ("Document", "FilterBuilder"),
    "Get3DView": ("Document", "ElementQuery"),
    "GetRoomAndSpaceElements": ("Document", "ElementQuery"),
    "FindProperFamilySymbol": ("Document", "ElementQuery"),
    "CollectExteriorWalls": ("Document", "ElementQuery"),
    "CollectWindows": ("Document", "ElementQuery"),
    "MoveElement": ("Document", "SelectionHelper"),
    "GetSelectedBeams": ("Document", "SelectionHelper"),
    "SelectConnection": ("Document", "SelectionHelper"),
    "SelectConnectionElements": ("Document", "SelectionHelper"),
    "GetSelectedModelGroup": ("Document", "SelectionHelper"),
    # Structural
    "GetHookOrient": ("Structural", "RebarGeometry"),
    "IsInRightDir": ("Structural", "RebarGeometry"),
    "IsRectangular": ("Structural", "AreaReinforcementHelper"),
    "FindUsageByName": ("Structural", "AnalyticalModelHelper"),
    "FindLoadCaseByName": ("Structural", "AnalyticalModelHelper"),
    # MEP
    "ExtractSystemFromConnectors": ("Mep", "ConnectorHelper"),
    "GetConnectedConnector": ("Mep", "ConnectorHelper"),
    "ValidateMep": ("Mep", "RoutingPreferenceHelper"),
    "MepWarning": ("Mep", "RoutingPreferenceHelper"),
    "ValidatePipesDefined": ("Mep", "RoutingPreferenceHelper"),
    "PipesDefinedWarning": ("Mep", "RoutingPreferenceHelper"),
    "IsADuct": ("Mep", "FabricationPartHelper"),
    "IsAPipe": ("Mep", "FabricationPartHelper"),
    "VerifyUnusedConnectors": ("Mep", "ConnectorHelper"),
    "IsElementBelongsToCircuit": ("Mep", "ConnectorHelper"),
    # Views
    "ShowAllAttachedDetailGroups": ("Views", "GroupVisibilityHelper"),
    "HideAllAttachedDetailGroups": ("Views", "GroupVisibilityHelper"),
    "CreateAndAddSchedules": ("Views", "ScheduleHelper"),
    "AddDataRow": ("Views", "ScheduleHelper"),
    "CreateTable": ("Views", "ScheduleHelper"),
    "GetNumberOfRowsAndColumns": ("Views", "ScheduleHelper"),
    "ReplaceIllegalCharacters": ("Views", "ScheduleHelper"),
    "MyMessageBox": ("Views", "PrintHelper"),
    "ShowWarningMessageBox": ("Views", "ViewTemplateHelper"),
    "ShowInformationMessageBox": ("Views", "ViewTemplateHelper"),
    "FindSampleSettings": ("Views", "PrintHelper"),
    "DisplayExport": ("Views", "CustomExportHelper"),
    "AddTo": ("Views", "CustomExportHelper"),
    "ChangeSubregionAndPointsElevation": ("Views", "SiteTopographyHelper"),
    "PickSubregion": ("Views", "SiteTopographyHelper"),
    "PickTopographySurface": ("Views", "SiteTopographyHelper"),
    "PickPointNearToposurface": ("Views", "SiteTopographyHelper"),
    "GetAverageElevation": ("Views", "SiteTopographyHelper"),
    "GeneratePondPointsSurrounding": ("Views", "SiteTopographyHelper"),
    "GetCenterOf": ("Views", "SiteTopographyHelper"),
    "GetPointsFromSubregionRough": ("Views", "SiteTopographyHelper"),
    "GetPointsFromSubregionExact": ("Views", "SiteTopographyHelper"),
    "GetTopographySurfaceHost": ("Views", "SiteTopographyHelper"),
    "GetNonBoundaryPoints": ("Views", "SiteTopographyHelper"),
    "SetIconsForPushButtonData": ("Views", "ViewHelper"),
    "CalculateControlPoints": ("Views", "StairsHelper"),
    "CalculateMaxStepsCount": ("Views", "StairsHelper"),
    "FindTargetLevels": ("Views", "StairsHelper"),
    "IsSouthFacing": ("Views", "ViewHelper"),
    "TransformByProjectLocation": ("Views", "ViewHelper"),
    "IsExterior": ("Views", "ViewHelper"),
    "GetExteriorWallDirection": ("Views", "ViewHelper"),
    "GetWindowDirection": ("Views", "ViewHelper"),
    "GetDocumentDisplayName": ("Views", "ViewHelper"),
    "AddTagSymbolByCategory": ("Views", "ViewHelper"),
    "Translate": ("Views", "ViewHelper"),
    "ParseBracketedId": ("Views", "ViewHelper"),
    "CollectPointsFromImportInstance": ("Views", "SiteTopographyHelper"),
    "TryGetFirstToposolidTypeAndLevel": ("Views", "SiteTopographyHelper"),
    # Infrastructure
    "GetRevitDbEventName": ("Infrastructure", "EventLoggingHelper"),
    "GetRevitUiEventName": ("Infrastructure", "EventLoggingHelper"),
    "TitleNoExt": ("Infrastructure", "EventLoggingHelper"),
    "Message": ("Infrastructure", "DialogHelper"),
    "ShowErrorMessage": ("Infrastructure", "DialogHelper"),
    "CenterOnScreen": ("Infrastructure", "DialogHelper"),
    "GetAssemblyPath": ("Infrastructure", "AssemblyPathHelper"),
    "GetAssemblyFullName": ("Infrastructure", "AssemblyPathHelper"),
    "GetApplicationResourcesPath": ("Infrastructure", "AssemblyPathHelper"),
    "ResolveDirectoryPath": ("Infrastructure", "AssemblyPathHelper"),
    "GetSourceFolder": ("Infrastructure", "SampleBrowserUtils"),
    "NormalizeSampleNamespace": ("Infrastructure", "SampleBrowserUtils"),
    "NewGuid": ("Infrastructure", "ExtensibleStorageHelper"),
    "DoesAnyStorageExist": ("Infrastructure", "ExtensibleStorageHelper"),
    "GetElementsWithAllSchemas": ("Infrastructure", "ExtensibleStorageHelper"),
    "GetTargetFolderUrn": ("Infrastructure", "CloudApiHelper"),
    "ConvertFromBitmap": ("Infrastructure", "BitmapHelper"),
    "GetBitmapAsImageSource": ("Infrastructure", "BitmapHelper"),
    "GetStdIcon": ("Infrastructure", "BitmapHelper"),
    "GetSmallIcon": ("Infrastructure", "BitmapHelper"),
    "GetXyz": ("Infrastructure", "SerializationHelper"),
    "GetBoolean": ("Infrastructure", "SerializationHelper"),
    "GetDouble": ("Infrastructure", "SerializationHelper"),
    "GetInteger": ("Infrastructure", "SerializationHelper"),
    "GetColor": ("Infrastructure", "SerializationHelper"),
    "GetXElement": ("Infrastructure", "SerializationHelper"),
    "GetColorXElement": ("Infrastructure", "SerializationHelper"),
    "ExecuteCreateBRepCommand": ("Infrastructure", "SampleBrowserUtils"),
    "CreateExternallyTaggedPodium": ("Infrastructure", "SampleBrowserUtils"),
    "CreateDirectShapeWithExternallyTaggedBRep": ("Infrastructure", "SampleBrowserUtils"),
    "ModifySelectedDoors": ("Infrastructure", "DialogHelper"),
    "FlipHandAndFace": ("Infrastructure", "DialogHelper"),
    "MakeLeft": ("Infrastructure", "DialogHelper"),
    "MakeRight": ("Infrastructure", "DialogHelper"),
    "TurnIn": ("Infrastructure", "DialogHelper"),
    "TurnOut": ("Infrastructure", "DialogHelper"),
    "Write": ("Infrastructure", "EventLoggingHelper"),
    "CloseFile": ("Infrastructure", "EventLoggingHelper"),
    "FindColumnsWithin": ("Document", "ElementQuery"),
    "GetElevationForRay": ("Document", "ElementQuery"),
}

TYPE_TO_FILE = {
    "Vector4": ("Geometry", "GraphicsLinearAlgebra"),
    "Matrix4": ("Geometry", "GraphicsLinearAlgebra"),
    "PointD": ("Geometry", "GraphicsLinearAlgebra"),
    "SampleBrowserUtils": ("Infrastructure", "SampleBrowserUtils"),
}

# Types that stay as separate classes in Common
STANDALONE_TYPES = {
    "WallTypeComparer", "ViewComparer", "FilterRuleBuilder", "RuleCriteraNames",
    "ConversionValue", "StandardIORouter",
}

COPYRIGHT = "// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt\n\n"

def norm_body(s):
    return re.sub(r'\s+', '', s)

def find_utils_files():
    files = list(SRC.rglob("_utils.cs"))
    return sorted(files)

def parse_file(path):
    text = path.read_text(encoding='utf-8-sig')
    return text

def extract_usings(text):
    usings = []
    for m in re.finditer(r'^using\s+([^;]+);', text, re.MULTILINE):
        usings.append(m.group(1).strip())
    return usings

def extract_types(text):
    """Extract top-level type definitions from _utils.cs."""
    types = []
    # Remove file header comments
    body = text
    pos = 0
    while pos < len(body):
        # Find next type declaration
        m = re.search(
            r'(?P<attrs>(?:\s*\[.*?\]\s*)*)'
            r'(?P<mods>(?:public|private|internal|protected)\s+(?:static\s+)?(?:partial\s+)?)'
            r'(?P<kind>class|struct|enum|interface)\s+'
            r'(?P<name>\w+)'
            r'(?P<inheritance>(?:\s*:\s*[^{]+)?)'
            r'\s*\{',
            body[pos:], re.DOTALL)
        if not m:
            break
        start = pos + m.start()
        name = m.group('name')
        brace_start = pos + m.end() - 1
        depth = 0
        i = brace_start
        while i < len(body):
            if body[i] == '{':
                depth += 1
            elif body[i] == '}':
                depth -= 1
                if depth == 0:
                    end = i + 1
                    types.append({
                        'name': name,
                        'full_text': body[start:end],
                        'kind': m.group('kind'),
                        'is_static_class': 'static class' in m.group(0),
                    })
                    pos = end
                    break
            i += 1
        else:
            break
    return types

def classify_method(method_name, sample_folder):
    if method_name in METHOD_MAP:
        return METHOD_MAP[method_name]
    folder = sample_folder.lower()
    # Heuristic fallbacks
    if any(k in method_name.lower() for k in ['rebar', 'hook', 'rein']):
        return ("Structural", "RebarGeometry")
    if any(k in method_name.lower() for k in ['connector', 'mep', 'duct', 'pipe', 'circuit']):
        return ("Mep", "ConnectorHelper")
    if any(k in method_name.lower() for k in ['view', 'sheet', 'schedule', 'print']):
        return ("Views", "ViewHelper")
    if any(k in method_name.lower() for k in ['filter', 'element', 'collect', 'select', 'find']):
        return ("Document", "ElementQuery")
    if any(k in method_name.lower() for k in ['param', 'para']):
        return ("Parameters", "ParameterAccess")
    if any(k in method_name.lower() for k in ['unit', 'convert', 'format']):
        return ("Units", "UnitConversion")
    if any(k in folder for k in ['site', 'topo', 'stair', 'winder']):
        return ("Views", "SiteTopographyHelper")
    return ("Infrastructure", "SampleBrowserUtils")

def rename_static_class(content, old_name, new_name):
    content = re.sub(rf'\bpublic\s+static\s+class\s+{old_name}\b', f'public static class {new_name}', content)
    content = re.sub(rf'\bstatic\s+class\s+{old_name}\b', f'static class {new_name}', content)
    return content

def process():
    utils_files = find_utils_files()
    print(f"Found {len(utils_files)} _utils.cs files")

    # area -> class -> {usings, members}
    classes = defaultdict(lambda: defaultdict(lambda: {'usings': set(), 'members': [], 'types': [], 'seen': set()}))
    method_to_class = {}  # method_name -> (area, class)
    type_locations = {}   # type_name -> (area, class)

    for fpath in utils_files:
        text = parse_file(fpath)
        usings = set(extract_usings(text))
        sample = fpath.parent.name
        types = extract_types(text)

        for t in types:
            tname = t['name']
            ttext = t['full_text']

            if tname in TYPE_TO_FILE:
                area, cls = TYPE_TO_FILE[tname]
                key = norm_body(ttext)
                if key not in classes[area][cls]['seen']:
                    classes[area][cls]['seen'].add(key)
                    classes[area][cls]['types'].append(ttext)
                    classes[area][cls]['usings'].update(usings)
                type_locations[tname] = (area, cls)
                continue

            if tname in STANDALONE_TYPES or (not t['is_static_class'] and tname not in ('_Utils',)):
                # Standalone helper types -> Document or appropriate
                if 'ISelectionFilter' in ttext or 'IComparer' in ttext:
                    area, cls = "Document", "SelectionFilters"
                elif 'IFailuresPreprocessor' in ttext:
                    area, cls = "Infrastructure", "DialogHelper"
                else:
                    area, cls = classify_method(tname, sample)
                key = norm_body(ttext)
                if key not in classes[area][cls]['seen']:
                    classes[area][cls]['seen'].add(key)
                    classes[area][cls]['types'].append(ttext)
                    classes[area][cls]['usings'].update(usings)
                type_locations[tname] = (area, cls)
                continue

            if tname == 'SampleBrowserUtils':
                area, cls = "Infrastructure", "SampleBrowserUtils"
                content = ttext.replace('namespace Ara3D.RevitSampleBrowser',
                    'namespace Ara3D.RevitSampleBrowser.Common.Infrastructure')
                key = norm_body(content)
                if key not in classes[area][cls]['seen']:
                    classes[area][cls]['seen'].add(key)
                    classes[area][cls]['types'].append(content)
                    classes[area][cls]['usings'].update(usings)
                type_locations[tname] = (area, cls)
                continue

            if tname == '_Utils':
                # Extract methods from static class body
                inner = re.search(r'\{', ttext)
                if not inner:
                    continue
                body = ttext[inner.start():]
                # Split members by top-level declarations
                members = split_members(body)
                for member in members:
                    member = member.strip()
                    if not member:
                        continue
                    mm = re.search(r'(?:public|private|protected|internal)\s+(?:static\s+)?[\w<>,\[\]\s]+\s+(\w+)\s*[\(<]', member)
                    if not mm:
                        # fields/constants
                        fm = re.search(r'public\s+(?:const\s+|static\s+)?[\w<>,\[\]]+\s+(\w+)\s*=', member)
                        if fm:
                            mname = fm.group(1)
                        else:
                            continue
                    else:
                        mname = mm.group(1)
                    area, cls = classify_method(mname, sample)
                    key = norm_body(member)
                    if key not in classes[area][cls]['seen']:
                        classes[area][cls]['seen'].add(key)
                        classes[area][cls]['members'].append(member)
                        classes[area][cls]['usings'].update(usings)
                    method_to_class[mname] = (area, cls)

    # Write Common files
    COMMON.mkdir(parents=True, exist_ok=True)
    created_files = []
    class_ns_map = {}

    for area in sorted(classes.keys()):
        area_dir = COMMON / area
        area_dir.mkdir(parents=True, exist_ok=True)
        for cls_name in sorted(classes[area].keys()):
            data = classes[area][cls_name]
            if not data['members'] and not data['types']:
                continue
            ns = f"Ara3D.RevitSampleBrowser.Common.{area}"
            class_ns_map[cls_name] = ns

            lines = [COPYRIGHT]
            for u in sorted(data['usings']):
                if not u.startswith('Ara3D.RevitSampleBrowser.') or u.startswith('Ara3D.RevitSampleBrowser.Common.'):
                    lines.append(f"using {u};")
            lines.append(f"\nnamespace {ns}\n{{")

            # Non-static types first
            for t in data['types']:
                t_fixed = re.sub(r'namespace\s+[\w.]+\s*\{', '', t)
                t_fixed = t_fixed.rstrip().rstrip('}')
                # Fix namespace for moved types
                t_fixed = rename_static_class(t_fixed, 'SampleBrowserUtils', 'SampleBrowserUtils')
                lines.append(t_fixed)
                lines.append('')

            if data['members']:
                if cls_name == 'SampleBrowserUtils' and any('class SampleBrowserUtils' in t for t in data['types']):
                    pass  # already have the class
                elif cls_name == 'SampleBrowserUtils':
                    lines.append("    public static class SampleBrowserUtils\n    {")
                else:
                    lines.append(f"    public static class {cls_name}\n    {{")
                for m in data['members']:
                    indented = '\n'.join('        ' + l if l.strip() else l for l in m.split('\n'))
                    lines.append(indented)
                lines.append("    }")

            lines.append("}")
            outpath = area_dir / f"{cls_name}.cs"
            content = '\n'.join(lines)
            # Fix Precision in XyzMath
            if cls_name == 'XyzMath':
                content = re.sub(r'private const double Precision = [\d.eE+-]+;',
                    'public const double Precision = 1e-5;', content)
                if 'public const double Precision' not in content:
                    content = content.replace(
                        'public static class XyzMath\n    {',
                        'public static class XyzMath\n    {\n        public const double Precision = 1e-5;\n')
            outpath.write_text(content, encoding='utf-8')
            created_files.append(str(outpath.relative_to(SRC)))
            print(f"Created {outpath.name} ({len(data['members'])} members, {len(data['types'])} types)")

    # Save mapping for reference update
    mapping_path = SRC.parent / "tools" / "method_class_map.json"
    import json
    mapping_path.write_text(json.dumps({k: list(v) for k, v in method_to_class.items()}, indent=2))

    return method_to_class, type_locations, class_ns_map, utils_files

def split_members(body):
    """Rough split of class body into member strings."""
    members = []
    depth = 0
    current = []
    in_member = False
    i = 0
    while i < len(body):
        c = body[i]
        if c == '{':
            depth += 1
            in_member = True
        elif c == '}':
            depth -= 1
            if depth == 0:
                if current:
                    members.append(''.join(current))
                break
        if depth == 1 and c == '\n':
            # check for member start at depth 1
            rest = body[i+1:]
            m = re.match(r'\s*((?:public|private|protected|internal)\s+)', rest)
            if m and current:
                members.append(''.join(current))
                current = []
        current.append(c)
        i += 1
    if current and depth > 0:
        members.append(''.join(current))
    return members

if __name__ == '__main__':
    process()
