#!/usr/bin/env python3
"""Update call sites from _Utils to Common classes, delete _utils.cs, update csproj."""
import json
import re
from pathlib import Path

SRC = Path(r"c:\Users\cdigg\git\revit-sample-browser\src")
CSPROJ = SRC / "Ara3D.RevitSampleBrowser.csproj"
MAP = json.loads((SRC.parent / "tools" / "method_class_map.json").read_text())

# Build method -> class info
METHOD_CLASS = {k: v["class"] for k, v in MAP.items()}
CLASS_NS = {}
for v in MAP.values():
    CLASS_NS[v["class"]] = v["ns"]

# Additional type relocations for non-_Utils types
TYPE_CLASS = {
    "Vector4": "GraphicsLinearAlgebra",
    "Matrix4": "GraphicsLinearAlgebra",
    "PointD": "GraphicsLinearAlgebra",
    "WallTypeComparer": "SelectionFilters",
    "ViewComparer": "SelectionFilters",
    "ConversionValue": "UnitConversion",
    "StandardIORouter": "SampleBrowserUtils",
    "SampleBrowserUtils": "SampleBrowserUtils",
    "Line2D": "Point2DMath",
}

for cls, area in [
    ("GraphicsLinearAlgebra", "Geometry"), ("XyzMath", "Geometry"),
    ("CurveGeometry", "Geometry"), ("Point2DMath", "Geometry"),
    ("PlaneAndTransform", "Geometry"), ("FaceAndSolidGeometry", "Geometry"),
    ("UnitConversion", "Units"), ("DocumentUnits", "Units"),
    ("ValueFormatting", "Units"), ("ParameterAccess", "Parameters"),
    ("SharedParameterHelper", "Parameters"), ("SelectionHelper", "Document"),
    ("ElementQuery", "Document"), ("FilterBuilder", "Document"),
    ("SelectionFilters", "Document"), ("RebarGeometry", "Structural"),
    ("AreaReinforcementHelper", "Structural"),
    ("StructuralConnectionHelper", "Structural"),
    ("AnalyticalModelHelper", "Structural"), ("MultiplanarRebarHelper", "Structural"),
    ("ConnectorHelper", "Mep"), ("RoutingPreferenceHelper", "Mep"),
    ("FabricationPartHelper", "Mep"), ("MepObstructionHelper", "Mep"),
    ("SpatialFieldHelper", "Mep"), ("ViewHelper", "Views"),
    ("ViewDuplicationHelper", "Views"), ("ViewTemplateHelper", "Views"),
    ("PrintHelper", "Views"), ("ScheduleHelper", "Views"),
    ("CustomExportHelper", "Views"), ("GroupVisibilityHelper", "Views"),
    ("SiteTopographyHelper", "Views"), ("StairsHelper", "Views"),
    ("DialogHelper", "Infrastructure"), ("WinFormsValidation", "Infrastructure"),
    ("EventLoggingHelper", "Infrastructure"), ("ExtensibleStorageHelper", "Infrastructure"),
    ("AssemblyPathHelper", "Infrastructure"), ("SerializationHelper", "Infrastructure"),
    ("CloudApiHelper", "Infrastructure"), ("ExternalResourceHelper", "Infrastructure"),
    ("BitmapHelper", "Infrastructure"), ("SampleBrowserUtils", "Infrastructure"),
]:
    CLASS_NS.setdefault(cls, f"Ara3D.RevitSampleBrowser.Common.{area}")
    TYPE_CLASS.setdefault(cls, cls)


def update_file(path):
    if path.name == "_utils.cs" or "Common" in path.parts:
        return False
    text = path.read_text(encoding='utf-8-sig')
    orig = text
    usings_needed = set()

    # Replace _Utils.Method with ClassName.Method
    for method, cls in sorted(METHOD_CLASS.items(), key=lambda x: -len(x[0])):
        pattern = rf'\b_Utils\.{re.escape(method)}\b'
        if re.search(pattern, text):
            text = re.sub(pattern, f'{cls}.{method}', text)
            usings_needed.add(CLASS_NS[cls])

    # Replace relocated types - add usings when types from Common are used
    TYPE_USINGS = {
        "Vector4": "Ara3D.RevitSampleBrowser.Common.Geometry",
        "Matrix4": "Ara3D.RevitSampleBrowser.Common.Geometry",
        "PointD": "Ara3D.RevitSampleBrowser.Common.Geometry",
        "WallTypeComparer": "Ara3D.RevitSampleBrowser.Common.Documents",
        "ViewComparer": "Ara3D.RevitSampleBrowser.Common.Documents",
        "ConversionValue": "Ara3D.RevitSampleBrowser.Common.Units",
        "StandardIORouter": "Ara3D.RevitSampleBrowser.Common.Infrastructure",
        "Line2D": "Ara3D.RevitSampleBrowser.CreateBeamSystem.CS",
    }
    for tname, ns in TYPE_USINGS.items():
        if re.search(rf'\b{tname}\b', text) and f'using {ns}' not in text:
            usings_needed.add(ns)

    if text == orig:
        return False

    # Add usings
    for ns in sorted(usings_needed):
        using_line = f"using {ns};"
        if using_line not in text:
            # insert after last using
            m = list(re.finditer(r'^using [^;]+;\s*$', text, re.MULTILINE))
            if m:
                insert_at = m[-1].end()
                text = text[:insert_at] + f"\n{using_line}" + text[insert_at:]
            else:
                # after copyright
                cm = re.search(r'// Copyright.*\n\n', text)
                if cm:
                    text = text[:cm.end()] + using_line + "\n\n" + text[cm.end():]
                else:
                    text = using_line + "\n" + text

    path.write_text(text.replace('\x00', ''), encoding='utf-8', newline='\n')
    return True


def update_csproj():
    text = CSPROJ.read_text(encoding='utf-8')
    # Remove all _utils.cs includes
    text = re.sub(r'\s*<Compile Include="[^"]*_utils\.cs" />\s*\n', '\n', text)
    # Add Common glob if not present
    if 'Common\\**\\*.cs' not in text:
        text = text.replace(
            '<EnableDefaultItems>false</EnableDefaultItems>',
            '<EnableDefaultItems>false</EnableDefaultItems>\n    <Compile Include="Common\\**\\*.cs" />'
        )
    CSPROJ.write_text(text, encoding='utf-8')


def delete_utils():
    count = 0
    for f in SRC.rglob("_utils.cs"):
        f.unlink()
        count += 1
    return count


def main():
    updated = 0
    for cs in SRC.rglob("*.cs"):
        if update_file(cs):
            updated += 1
            print(f"Updated {cs.relative_to(SRC)}")
    deleted = delete_utils()
    update_csproj()
    # Fix SampleData namespace
    sd = SRC / "SampleData.cs"
    t = sd.read_text(encoding='utf-8-sig')
    if "using Ara3D.RevitSampleBrowser.Common.Infrastructure" not in t:
        t = t.replace(
            "using System.IO;",
            "using System.IO;\nusing Ara3D.RevitSampleBrowser.Common.Infrastructure;"
        )
        sd.write_text(t, encoding='utf-8')
    print(f"Updated {updated} files, deleted {deleted} _utils.cs")


if __name__ == '__main__':
    main()
