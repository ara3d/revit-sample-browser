#!/usr/bin/env python3
"""Consolidate _utils.cs into src/Common/ with brace-aware parsing."""
import json
import re
from collections import defaultdict
from pathlib import Path

SRC = Path(r"c:\Users\cdigg\git\revit-sample-browser\src")
COMMON = SRC / "Common"
COPYRIGHT = "// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt\n\n"

METHOD_MAP = {
    "SubXyz": ("Geometry", "XyzMath"), "AddXyz": ("Geometry", "XyzMath"),
    "DotMatrix": ("Geometry", "XyzMath"), "CrossMatrix": ("Geometry", "XyzMath"),
    "IsEqual": ("Geometry", "XyzMath"), "GetLength": ("Geometry", "XyzMath"),
    "UnitVector": ("Geometry", "XyzMath"), "MultiplyVector": ("Geometry", "XyzMath"),
    "TransformPoint": ("Geometry", "XyzMath"), "OffsetPoint": ("Geometry", "XyzMath"),
    "IsSameDirection": ("Geometry", "XyzMath"), "IsOppositeDirection": ("Geometry", "XyzMath"),
    "XyzToString": ("Geometry", "XyzMath"), "MoveXyzToElevation": ("Geometry", "XyzMath"),
    "DoubleEquals": ("Geometry", "XyzMath"),
    "GetTangentAt": ("Geometry", "CurveGeometry"), "ComputeParameter": ("Geometry", "CurveGeometry"),
    "ProjectCurveToElevation": ("Geometry", "CurveGeometry"),
    "ProjectCurvesToElevation": ("Geometry", "CurveGeometry"),
    "TransformCurve": ("Geometry", "CurveGeometry"), "TransformCurves": ("Geometry", "CurveGeometry"),
    "FindLongestEndpointConnection": ("Geometry", "CurveGeometry"),
    "Distance2D": ("Geometry", "Point2DMath"), "Sub2D": ("Geometry", "Point2DMath"),
    "Add2D": ("Geometry", "Point2DMath"), "Dot2D": ("Geometry", "Point2DMath"),
    "Cross2D": ("Geometry", "Point2DMath"), "Normalize2D": ("Geometry", "Point2DMath"),
    "GetNormalToWallAt": ("Geometry", "PlaneAndTransform"),
    "GetWallDeltaAt": ("Geometry", "PlaneAndTransform"),
    "TryGetDemoSolids": ("Geometry", "FaceAndSolidGeometry"),
    "CovertFromApi": ("Units", "UnitConversion"), "CovertToApi": ("Units", "UnitConversion"),
    "UnitDictionary": ("Units", "UnitConversion"),
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
    "SetParameter": ("Parameters", "ParameterAccess"),
    "FindParameter": ("Parameters", "ParameterAccess"),
    "SetParaNullId": ("Parameters", "ParameterAccess"),
    "SetParaInt": ("Parameters", "ParameterAccess"),
    "FindParaByName": ("Parameters", "ParameterAccess"),
    "FindFamilySymbol": ("Parameters", "ParameterAccess"),
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
    "FindColumnsWithin": ("Document", "ElementQuery"),
    "GetElevationForRay": ("Document", "ElementQuery"),
    "MoveElement": ("Document", "SelectionHelper"),
    "GetSelectedBeams": ("Document", "SelectionHelper"),
    "SelectConnection": ("Document", "SelectionHelper"),
    "SelectConnectionElements": ("Document", "SelectionHelper"),
    "GetSelectedModelGroup": ("Document", "SelectionHelper"),
    "GetHookOrient": ("Structural", "RebarGeometry"),
    "IsInRightDir": ("Structural", "RebarGeometry"),
    "IsVertical": ("Structural", "RebarGeometry"),
    "IsRectangular": ("Structural", "AreaReinforcementHelper"),
    "FindUsageByName": ("Structural", "AnalyticalModelHelper"),
    "FindLoadCaseByName": ("Structural", "AnalyticalModelHelper"),
    "ExtractSystemFromConnectors": ("Mep", "ConnectorHelper"),
    "GetConnectedConnector": ("Mep", "ConnectorHelper"),
    "ValidateMep": ("Mep", "RoutingPreferenceHelper"),
    "MepWarning": ("Mep", "RoutingPreferenceHelper"),
    "ValidatePipesDefined": ("Mep", "RoutingPreferenceHelper"),
    "PipesDefinedWarning": ("Mep", "RoutingPreferenceHelper"),
    "IsADuct": ("Mep", "FabricationPartHelper"), "IsAPipe": ("Mep", "FabricationPartHelper"),
    "VerifyUnusedConnectors": ("Mep", "ConnectorHelper"),
    "IsElementBelongsToCircuit": ("Mep", "ConnectorHelper"),
    "ShowAllAttachedDetailGroups": ("Views", "GroupVisibilityHelper"),
    "HideAllAttachedDetailGroups": ("Views", "GroupVisibilityHelper"),
    "CreateAndAddSchedules": ("Views", "ScheduleHelper"),
    "AddDataRow": ("Views", "ScheduleHelper"), "CreateTable": ("Views", "ScheduleHelper"),
    "GetNumberOfRowsAndColumns": ("Views", "ScheduleHelper"),
    "ReplaceIllegalCharacters": ("Views", "ScheduleHelper"),
    "MyMessageBox": ("Views", "PrintHelper"),
    "ShowWarningMessageBox": ("Views", "ViewTemplateHelper"),
    "ShowInformationMessageBox": ("Views", "ViewTemplateHelper"),
    "FindSampleSettings": ("Views", "PrintHelper"),
    "DisplayExport": ("Views", "CustomExportHelper"), "AddTo": ("Views", "CustomExportHelper"),
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
    "Translate": ("Views", "ViewHelper"), "ParseBracketedId": ("Views", "ViewHelper"),
    "CollectPointsFromImportInstance": ("Views", "SiteTopographyHelper"),
    "TryGetFirstToposolidTypeAndLevel": ("Views", "SiteTopographyHelper"),
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
    "Write": ("Infrastructure", "EventLoggingHelper"),
    "CloseFile": ("Infrastructure", "EventLoggingHelper"),
    "ModifySelectedDoors": ("Infrastructure", "DialogHelper"),
    "FlipHandAndFace": ("Infrastructure", "DialogHelper"),
    "MakeLeft": ("Infrastructure", "DialogHelper"),
    "MakeRight": ("Infrastructure", "DialogHelper"),
    "TurnIn": ("Infrastructure", "DialogHelper"),
    "TurnOut": ("Infrastructure", "DialogHelper"),
}

TYPE_RELOC = {
    "Vector4": ("Geometry", "GraphicsLinearAlgebra"),
    "Matrix4": ("Geometry", "GraphicsLinearAlgebra"),
    "PointD": ("Geometry", "GraphicsLinearAlgebra"),
    "SampleBrowserUtils": ("Infrastructure", "SampleBrowserUtils"),
    "ConversionValue": ("Units", "UnitConversion"),
    "WallTypeComparer": ("Document", "SelectionFilters"),
    "ViewComparer": ("Document", "SelectionFilters"),
    "StandardIORouter": ("Infrastructure", "SampleBrowserUtils"),
    "Line2D": ("Geometry", "Point2DMath"),
}

SKIP_METHODS = {"ImperialDutRatio", "GetEvaluatorCriteriaName", "AddNewUnit", "DotMatrix"}


def norm(s):
    return re.sub(r'\s+', '', s)


def find_matching_brace(text, open_idx):
    depth = 0
    i = open_idx
    in_str = None
    while i < len(text):
        c = text[i]
        if in_str:
            if c == '\\':
                i += 2
                continue
            if c == in_str:
                in_str = None
        else:
            if c in ('"', "'"):
                in_str = c
            elif c == '{':
                depth += 1
            elif c == '}':
                depth -= 1
                if depth == 0:
                    return i
        i += 1
    return -1


def extract_namespace_body(text):
    m = re.search(r'namespace\s+[\w.]+\s*\{', text)
    if not m:
        return text
    open_b = m.end() - 1
    close_b = find_matching_brace(text, open_b)
    if close_b < 0:
        return text
    return text[open_b + 1:close_b]


def extract_top_level_types(text):
    """Return list of (name, full_text, kind) for namespace-level types only."""
    ns_body = extract_namespace_body(text)
    results = []
    pattern = re.compile(
        r'(?P<prefix>(?:\s*(?://[^\n]*\n|/\*.*?\*/\s*)|(?:\s*\[.*?\]\s*))*'
        r'(?P<mods>(?:public|internal|private)\s+(?:static\s+)?(?:partial\s+)?)'
        r'(?P<kind>class|struct|enum|interface)\s+'
        r'(?P<name>\w+)'
        r'(?P<inherit>[^\{]*)'
        r'\{)',
        re.DOTALL)
    for m in pattern.finditer(ns_body):
        name = m.group('name')
        brace = m.start() + len(m.group(0)) - 1
        end = find_matching_brace(ns_body, brace)
        if end < 0:
            continue
        full = ns_body[m.start():end + 1]
        results.append((name, full, m.group('kind')))
    return results


def extract_members(class_body):
    """Extract direct child members from class/struct body (inside outer braces)."""
    inner = class_body.strip()
    open_b = inner.find('{')
    if open_b < 0:
        return []
    close_b = find_matching_brace(inner, open_b)
    if close_b < 0:
        return []
    body = inner[open_b + 1:close_b]
    members = []
    i = 0
    n = len(body)
    while i < n:
        while i < n and body[i].isspace():
            i += 1
        if i >= n:
            break
        # skip comments
        if body.startswith('//', i):
            eol = body.find('\n', i)
            i = eol + 1 if eol >= 0 else n
            continue
        if body.startswith('/*', i):
            endc = body.find('*/', i)
            i = endc + 2 if endc >= 0 else n
            continue
        start = i
        # find member end: either ; or { ... }
        if body[i] == '{':
            end = find_matching_brace(body, i)
            if end < 0:
                break
            i = end + 1
            members.append(body[start:i].strip())
            continue
        # scan to ; or {
        j = i
        paren = 0
        while j < n:
            c = body[j]
            if c == '(':
                paren += 1
            elif c == ')':
                paren -= 1
            elif c == '{' and paren == 0:
                end = find_matching_brace(body, j)
                members.append(body[start:end + 1].strip())
                i = end + 1
                break
            elif c == ';' and paren == 0:
                members.append(body[start:j + 1].strip())
                i = j + 1
                break
            j += 1
        else:
            break
    return [m for m in members if m and not m.startswith('//')]


def member_name(member):
    m = re.search(
        r'(?:public|private|protected|internal)\s+(?:static\s+)?(?:const\s+)?'
        r'(?:[\w<>\[\],\s\.]+\s+)+(\w+)\s*(?:\(|\{|=)',
        member)
    return m.group(1) if m else None


def classify(name, sample):
    if name in METHOD_MAP:
        return METHOD_MAP[name]
    nl = name.lower()
    if any(k in nl for k in ('rebar', 'hook', 'rein', 'structural')):
        return ("Structural", "RebarGeometry")
    if any(k in nl for k in ('connector', 'mep', 'duct', 'pipe', 'circuit', 'routing')):
        return ("Mep", "ConnectorHelper")
    if any(k in nl for k in ('view', 'sheet', 'schedule', 'print', 'template')):
        return ("Views", "ViewHelper")
    if any(k in nl for k in ('filter', 'element', 'collect', 'select', 'find', 'query')):
        return ("Document", "ElementQuery")
    if any(k in nl for k in ('param', 'para')):
        return ("Parameters", "ParameterAccess")
    if any(k in nl for k in ('unit', 'convert', 'format', 'covert')):
        return ("Units", "UnitConversion")
    if any(k in nl for k in ('bitmap', 'icon', 'image')):
        return ("Infrastructure", "BitmapHelper")
    if any(k in nl for k in ('storage', 'schema', 'guid')):
        return ("Infrastructure", "ExtensibleStorageHelper")
    if any(k in nl for k in ('log', 'event', 'trace')):
        return ("Infrastructure", "EventLoggingHelper")
    if any(k in nl for k in ('dialog', 'message', 'show', 'form')):
        return ("Infrastructure", "DialogHelper")
    if any(k in nl for k in ('xyz', 'curve', 'line', 'plane', 'transform', 'vector', 'matrix', 'point')):
        return ("Geometry", "XyzMath")
    return ("Infrastructure", "SampleBrowserUtils")


def strip_precision_const(members):
    return [m for m in members if not re.match(r'private\s+const\s+double\s+Precision\s*=', m)]


def fix_member_visibility(member, public_names):
    mn = member_name(member)
    if mn and mn in public_names and member.strip().startswith('private'):
        return re.sub(r'\bprivate\b', 'public', member, count=1)
    return member


def process_file(path, store, method_map_out):
    text = path.read_text(encoding='utf-8-sig')
    usings = re.findall(r'^using\s+([^;]+);', text, re.MULTILINE)
    sample = path.parent.name
    ns_match = re.search(r'namespace\s+([\w.]+)', text)
    old_ns = ns_match.group(1) if ns_match else ''

    for tname, ttext, kind in extract_top_level_types(text):
        if tname in TYPE_RELOC:
            area, cls = TYPE_RELOC[tname]
            key = norm(ttext)
            if key not in store[area][cls]['seen']:
                store[area][cls]['seen'].add(key)
                fixed = re.sub(r'namespace\s+[\w.]+\s*\{', '', ttext)
                fixed = fixed.strip().rstrip('}')
                store[area][cls]['types'].append(fixed.strip())
                store[area][cls]['usings'].update(usings)
            continue

        if tname == 'SampleBrowserUtils':
            area, cls = "Infrastructure", "SampleBrowserUtils"
            key = norm(ttext)
            if key not in store[area][cls]['seen']:
                store[area][cls]['seen'].add(key)
                body = ttext
                body = re.sub(r'namespace\s+[\w.]+\s*\{', '', body).strip().rstrip('}')
                store[area][cls]['types'].append(body)
                store[area][cls]['usings'].update(usings)
            continue

        if tname == '_Utils':
            members = strip_precision_const(extract_members(ttext))
            for member in members:
                mn = member_name(member)
                if not mn or mn in SKIP_METHODS:
                    continue
                area, cls = classify(mn, sample)
                key = norm(member)
                if key in store[area][cls]['seen']:
                    method_map_out[mn] = (area, cls)
                    continue
                store[area][cls]['seen'].add(key)
                pub = not member.strip().startswith('private')
                if pub:
                    store[area][cls]['public_methods'].add(mn)
                store[area][cls]['members'].append(member)
                store[area][cls]['usings'].update(usings)
                method_map_out[mn] = (area, cls)
            continue

        # Other types (comparers, filters, routers, etc.)
        if 'ISelectionFilter' in ttext or 'IComparer' in ttext:
            area, cls = "Document", "SelectionFilters"
        elif 'IFailuresPreprocessor' in ttext:
            area, cls = "Infrastructure", "DialogHelper"
        else:
            area, cls = classify(tname, sample)
        key = norm(ttext)
        if key not in store[area][cls]['seen']:
            store[area][cls]['seen'].add(key)
            fixed = re.sub(r'namespace\s+[\w.]+\s*\{', '', ttext).strip().rstrip('}')
            store[area][cls]['types'].append(fixed.strip())
            store[area][cls]['usings'].update(usings)


def write_common(store):
    COMMON.mkdir(parents=True, exist_ok=True)
    created = []
    for area in sorted(store.keys()):
        (COMMON / area).mkdir(parents=True, exist_ok=True)
        for cls_name, data in sorted(store[area].items()):
            if not data['members'] and not data['types']:
                continue
            ns = f"Ara3D.RevitSampleBrowser.Common.{area}"
            lines = [COPYRIGHT]
            skip_usings = {'Ara3D.RevitSampleBrowser'}
            for u in sorted(data['usings']):
                if u in skip_usings or (u.startswith('Ara3D.RevitSampleBrowser.') and not u.startswith('Ara3D.RevitSampleBrowser.Common.')):
                    continue
                lines.append(f"using {u};")
            lines.append(f"\nnamespace {ns}\n{{")

            for t in data['types']:
                lines.append('    ' + t.replace('\n', '\n    ').strip())
                lines.append('')

            static_classes = {cls_name}
            if data['members']:
                lines.append(f"    public static class {cls_name}\n    {{")
                if cls_name == 'XyzMath':
                    lines.append("        public const double Precision = 1e-5;\n")
                seen_names = set()
                for m in data['members']:
                    mn = member_name(m)
                    if mn and mn in seen_names:
                        continue
                    if mn:
                        seen_names.add(mn)
                    fixed = fix_member_visibility(m, data['public_methods'])
                    indented = '\n'.join(
                        ('        ' + l if l.strip() else '') for l in fixed.split('\n'))
                    lines.append(indented)
                    lines.append('')
                lines.append("    }")

            lines.append("}")
            out = COMMON / area / f"{cls_name}.cs"
            out.write_text('\n'.join(lines), encoding='utf-8')
            created.append(out)
            print(f"Wrote {out.relative_to(SRC)}: {len(data['members'])} methods, {len(data['types'])} types")
    return created


def new_bucket():
    return {'usings': set(), 'members': [], 'types': [], 'seen': set(), 'public_methods': set()}


def main():
    store = defaultdict(lambda: defaultdict(new_bucket))
    method_map = {}
    files = sorted(SRC.rglob('_utils.cs'))
    print(f"Processing {len(files)} files...")
    for f in files:
        process_file(f, store, method_map)

    for m, t in METHOD_MAP.items():
        method_map[m] = t

    created = write_common(store)
    map_path = SRC.parent / "tools" / "method_class_map.json"
    map_path.write_text(json.dumps(
        {k: {"area": v[0], "class": v[1], "ns": f"Ara3D.RevitSampleBrowser.Common.{v[0]}"}
         for k, v in sorted(method_map.items())}, indent=2))
    print(f"Created {len(created)} Common files, mapped {len(method_map)} methods")
    return method_map


if __name__ == '__main__':
    main()
