#!/usr/bin/env python3
"""Fix bare method calls inside Common/*.cs files using method_class_map.json."""

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
COMMON = ROOT / "src" / "Common"
MAP_PATH = ROOT / "tools" / "method_class_map.json"

# Methods defined in the same file (class name from filename)
FILE_CLASS = {
    "XyzMath.cs": "XyzMath",
    "CurveGeometry.cs": "CurveGeometry",
    "FaceAndSolidGeometry.cs": "FaceAndSolidGeometry",
    "GraphicsLinearAlgebra.cs": "GraphicsLinearAlgebra",
    "PlaneAndTransform.cs": "PlaneAndTransform",
    "Point2DMath.cs": "Point2DMath",
    "UnitConversion.cs": "UnitConversion",
    "DocumentUnits.cs": "DocumentUnits",
    "ValueFormatting.cs": "ValueFormatting",
    "ParameterAccess.cs": "ParameterAccess",
    "ElementQuery.cs": "ElementQuery",
    "FilterBuilder.cs": "FilterBuilder",
    "SelectionHelper.cs": "SelectionHelper",
    "SelectionFilters.cs": None,  # multiple types
    "RebarGeometry.cs": "RebarGeometry",
    "AreaReinforcementHelper.cs": "AreaReinforcementHelper",
    "AnalyticalModelHelper.cs": "AnalyticalModelHelper",
    "ConnectorHelper.cs": "ConnectorHelper",
    "RoutingPreferenceHelper.cs": "RoutingPreferenceHelper",
    "FabricationPartHelper.cs": "FabricationPartHelper",
    "ViewHelper.cs": "ViewHelper",
    "ViewTemplateHelper.cs": "ViewTemplateHelper",
    "PrintHelper.cs": "PrintHelper",
    "ScheduleHelper.cs": "ScheduleHelper",
    "CustomExportHelper.cs": "CustomExportHelper",
    "GroupVisibilityHelper.cs": "GroupVisibilityHelper",
    "SiteTopographyHelper.cs": "SiteTopographyHelper",
    "StairsHelper.cs": "StairsHelper",
    "SampleBrowserUtils.cs": "SampleBrowserUtils",
    "DialogHelper.cs": "DialogHelper",
    "EventLoggingHelper.cs": "EventLoggingHelper",
    "ExtensibleStorageHelper.cs": "ExtensibleStorageHelper",
    "AssemblyPathHelper.cs": "AssemblyPathHelper",
    "SerializationHelper.cs": "SerializationHelper",
    "CloudApiHelper.cs": "CloudApiHelper",
    "BitmapHelper.cs": "BitmapHelper",
    "ExternalResourceHelper.cs": "ExternalResourceHelper",
}

SKIP_NAMES = {
    "if", "for", "while", "switch", "catch", "return", "new", "typeof", "sizeof",
    "nameof", "throw", "using", "lock", "checked", "unchecked", "default", "base",
    "this", "var", "out", "ref", "in", "is", "as", "not", "and", "or",
}

# Local aliases / fields that should not be prefixed
LOCAL_SKIP = {
    "Precision", "Tolerance", "DoubleTolerance", "Resources", "ResManager",
    "Math", "Convert", "String", "Array", "Enumerable", "File", "Path",
    "TaskDialog", "MessageBox", "Directory", "Console", "Debug", "Trace",
    "JsonSerializer", "Bitmap", "Image", "Color", "Point", "Size",
}


def load_map():
    data = json.loads(MAP_PATH.read_text(encoding="utf-8"))
    return {k: v["class"] for k, v in data.items()}


def fix_file(path: Path, method_map: dict[str, str]) -> bool:
    text = path.read_text(encoding="utf-8")
    original = text
    own_class = FILE_CLASS.get(path.name)

    def replacer(match: re.Match) -> str:
        prefix = match.group(1) or ""
        name = match.group(2)
        if prefix or name in SKIP_NAMES or name in LOCAL_SKIP:
            return match.group(0)
        if name[0].isupper() and name.endswith("Exception"):
            return match.group(0)
        if name not in method_map:
            return match.group(0)
        target = method_map[name]
        if own_class and target == own_class:
            return match.group(0)
        return f"{target}.{name}("

    text = re.sub(r"(?<![\w.])([\w]+\.)?(\w+)\(", replacer, text)

    # Fix ShowWarningMessage -> DialogHelper.ShowWarningMessage in SampleBrowserUtils
    if path.name == "SampleBrowserUtils.cs":
        text = text.replace("ShowWarningMessage(", "DialogHelper.ShowWarningMessage(")
        if "GridCreation.CS.Properties.Resources" not in text:
            text = text.replace(
                "Resources.ResourceManager",
                "GridCreationResources.ResourceManager",
            )
            text = text.replace(
                "private static readonly ResourceManager ResManager = GridCreationResources.ResourceManager;",
                "private static readonly ResourceManager ResManager = GridCreationResources.ResourceManager;",
            )
            insert = "using GridCreationResources = Ara3D.RevitSampleBrowser.GridCreation.CS.Properties.Resources;\n"
            if insert.strip() not in text:
                text = text.replace(
                    "using WinFormsControl = System.Windows.Forms.Control;\n",
                    "using WinFormsControl = System.Windows.Forms.Control;\n" + insert,
                )

    # CompareDouble -> IsEqual within XyzMath
    if path.name == "XyzMath.cs":
        text = text.replace("CompareDouble(", "IsEqual(")

    if text != original:
        path.write_text(text, encoding="utf-8")
        return True
    return False


def add_usings():
    """Add Common namespace usings where ClassName. prefixes appear."""
    ns_by_class = {}
    data = json.loads(MAP_PATH.read_text(encoding="utf-8"))
    for info in data.values():
        ns_by_class[info["class"]] = info["ns"]

    for path in COMMON.rglob("*.cs"):
        text = path.read_text(encoding="utf-8")
        own_class = FILE_CLASS.get(path.name)
        needed = set()
        for cls, ns in ns_by_class.items():
            if cls == own_class:
                continue
            if re.search(rf"(?<![\w.]){cls}\.\w+\(", text):
                needed.add(f"using {ns};")
        if not needed:
            continue
        lines = text.splitlines(keepends=True)
        insert_at = 0
        for i, line in enumerate(lines):
            if line.startswith("namespace "):
                insert_at = i
                break
        existing = set(l.strip() for l in lines if l.strip().startswith("using "))
        to_add = sorted(needed - existing)
        if not to_add:
            continue
        block = "".join(u + "\n" for u in to_add) + "\n"
        lines.insert(insert_at, block)
        path.write_text("".join(lines), encoding="utf-8")


def main():
    method_map = load_map()
    changed = 0
    for path in sorted(COMMON.rglob("*.cs")):
        if fix_file(path, method_map):
            changed += 1
            print(f"fixed {path.relative_to(ROOT)}")
    add_usings()
    print(f"Updated {changed} files")


if __name__ == "__main__":
    main()
