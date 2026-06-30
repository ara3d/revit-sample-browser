#!/usr/bin/env python3
"""Move TBC_Shared helper code into src/Common with domain-based organization."""
from __future__ import annotations

import re
import shutil
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SRC = ROOT / "src"
SHARED = SRC / "TBC_Shared"
COMMON = SRC / "Common"

COPYRIGHT = "// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt\n\n"

STANDALONE_MOVES: dict[str, str] = {
    "IntPoint2d.cs": "Geometry/IntPoint2d.cs",
    "IntPoint3d.cs": "Geometry/IntPoint3d.cs",
    "Creator.cs": "Geometry/Creator.cs",
    "FailuresPreprocessor.cs": "Document/BuildingCoderFailuresPreprocessors.cs",
    "ElementInLinkSelectionFilter.cs": "Document/ElementInLinkSelectionFilter.cs",
    "JtElementsOfClassSelectionFilter.cs": "Document/JtElementsOfClassSelectionFilter.cs",
    "JtPairPicker.cs": "Document/JtPairPicker.cs",
    "JtSelectorMulti.cs": "Document/JtSelectorMulti.cs",
    "JtTimer.cs": "Infrastructure/JtTimer.cs",
    "JtWindowHandle.cs": "Infrastructure/JtWindowHandle.cs",
    "MepSystemSearch.cs": "Mep/MepSystemSearch.cs",
    "JtRevision.cs": "Parameters/JtRevision.cs",
    "JtNamedGuidStorage.cs": "Parameters/JtNamedGuidStorage.cs",
    "JtParamValuesForCats.cs": "Parameters/JtParamValuesForCats.cs",
    "ParameterUnitConverter.cs": "Parameters/ParameterUnitConverter.cs",
}

UTIL_REGION_TARGETS: dict[str, str] = {
    "Geometrical Comparison": "Geometry/Util.GeometryComparison.cs",
    "Geometrical Calculation": "Geometry/Util.GeometryCalculation.cs",
    "Colour Conversion": "Geometry/Util.ColourConversion.cs",
    "Create Various Solids": "Geometry/Util.Solids.cs",
    "Convex Hull": "Geometry/Util.ConvexHull.cs",
    "Consolidated geometry helpers": "Geometry/Util.GeometryHelpers.cs",
    "Consolidated XYZ comparers": "Geometry/Util.XyzComparers.cs",
    "Unit Handling": "Units/Util.UnitHandling.cs",
    "Formatting": "Units/Util.Formatting.cs",
    "Element Selection": "Document/Util.Selection.cs",
    "Element Filtering": "Document/Util.Filtering.cs",
    "Default Workset Names": "Document/Util.Worksets.cs",
    "Consolidated room and view helpers": "Document/Util.RoomAndView.cs",
    "Consolidated material helpers": "Document/Util.Materials.cs",
    "MEP utilities": "Mep/Util.Mep.cs",
    "Display a message": "Infrastructure/Util.Messages.cs",
    "Generate add-in manifest on the fly": "Infrastructure/Util.Manifest.cs",
    "Consolidated export helpers": "Infrastructure/Util.Export.cs",
    "Consolidated misc sample helpers": "Infrastructure/Util.Misc.cs",
}

EXTENSION_TARGETS: dict[str, str] = {
    "IEnumerableExtensions": "Infrastructure/IEnumerableExtensions.cs",
    "JtElementExtensionMethods": "Document/JtElementExtensionMethods.cs",
    "JtElementIdExtensionMethods": "Document/JtElementIdExtensionMethods.cs",
    "JtLineExtensionMethods": "Geometry/JtLineExtensionMethods.cs",
    "JtBoundingBoxXyzExtensionMethods": "Geometry/JtBoundingBoxXyzExtensionMethods.cs",
    "JtPlaneExtensionMethods": "Geometry/JtPlaneExtensionMethods.cs",
    "JtEdgeArrayExtensionMethods": "Geometry/JtEdgeArrayExtensionMethods.cs",
    "JtFamilyParameterExtensionMethods": "Parameters/JtFamilyParameterExtensionMethods.cs",
    "JtFilteredElementCollectorExtensions": "Document/JtFilteredElementCollectorExtensions.cs",
    "JtBuiltInCategoryExtensionMethods": "Document/JtBuiltInCategoryExtensionMethods.cs",
    "JtFamilyInstanceExtensionMethods": "Document/JtFamilyInstanceExtensionMethods.cs",
}


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def write_text(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8", newline="\n")


def extract_usings(text: str) -> str:
    m = re.search(r"#region Namespaces\s*\n(.*?)#endregion", text, re.DOTALL)
    if not m:
        return ""
    return m.group(1).strip() + "\n"


def extract_util_body(text: str) -> str:
    m = re.search(
        r"internal static partial class Util\s*\{(.*)\}\s*\n\s*#region Extension Method Classes",
        text,
        re.DOTALL,
    )
    if not m:
        raise RuntimeError("Could not locate Util partial class body")
    return m.group(1)


def extract_top_level_region(body: str, name: str) -> str:
    start_marker = f"#region {name}"
    start = body.find(start_marker)
    if start < 0:
        raise RuntimeError(f"Region not found: {name}")
    start = body.rfind("\n", 0, start) + 1

    depth = 0
    pos = start
    region_re = re.compile(r"^\s+#(region|endregion)\b", re.MULTILINE)
    for match in region_re.finditer(body, start):
        kind = match.group(1)
        if kind == "region":
            depth += 1
        else:
            depth -= 1
            if depth == 0:
                end = match.end()
                return body[start:end].rstrip()
    raise RuntimeError(f"Unclosed region: {name}")


def extract_util_regions(body: str) -> dict[str, str]:
    regions: dict[str, str] = {}
    for name in UTIL_REGION_TARGETS:
        chunk = extract_top_level_region(body, name)
        regions[name] = chunk
    return regions


def wrap_partial_util(usings: str, regions_text: str) -> str:
    return (
        COPYRIGHT
        + usings
        + "\n\nnamespace BuildingCoder\n{\n"
        + "    internal static partial class Util\n    {\n"
        + regions_text
        + "\n    }\n}\n"
    )


def wrap_namespace(usings: str, body: str) -> str:
    return COPYRIGHT + usings + "\n\nnamespace BuildingCoder\n{\n" + body + "\n}\n"


def split_util(usings: str, util_text: str) -> dict[str, list[str]]:
    body = extract_util_body(util_text)
    grouped: dict[str, list[str]] = {}
    for name, chunk in extract_util_regions(body).items():
        target = UTIL_REGION_TARGETS[name]
        grouped.setdefault(target, []).append(chunk)
    return grouped


def extract_extension_classes(util_text: str) -> dict[str, str]:
    m = re.search(
        r"#region Extension Method Classes\s*\n(.*?)#region Compatibility Methods by Magson Leone",
        util_text,
        re.DOTALL,
    )
    if not m:
        raise RuntimeError("Could not locate extension method classes")
    block = m.group(1)

    classes: dict[str, str] = {}
    pattern = re.compile(
        r"(public static class (\w+)[\s\S]*?\n    \})",
        re.MULTILINE,
    )
    for match in pattern.finditer(block):
        class_name = match.group(2)
        classes[class_name] = match.group(1).rstrip()
    return classes


def extract_compatibility_methods(util_text: str) -> str:
    m = re.search(
        r"(#region Compatibility Methods by Magson Leone[\s\S]*?)#endregion // Compatibility Methods by Magson Leone",
        util_text,
    )
    if not m:
        raise RuntimeError("Could not locate CompatibilityMethods")
    return m.group(1).rstrip() + "\n\n    #endregion // Compatibility Methods by Magson Leone\n"


def move_standalone_files() -> None:
    for src_name, dest_rel in STANDALONE_MOVES.items():
        src = SHARED / src_name
        dest = COMMON / dest_rel
        if not src.exists():
            raise FileNotFoundError(src)
        text = read_text(src)
        if not text.startswith("// Copyright"):
            text = COPYRIGHT + text.lstrip()
        write_text(dest, text)


def split_and_write_util() -> None:
    util_text = read_text(SHARED / "Util.cs")
    usings = extract_usings(util_text)

    grouped = split_util(usings, util_text)
    for dest_rel, chunks in grouped.items():
        body = "\n\n".join(chunks)
        write_text(COMMON / dest_rel, wrap_partial_util(usings, body))

    extensions = extract_extension_classes(util_text)
    for class_name, body in extensions.items():
        dest_rel = EXTENSION_TARGETS.get(class_name)
        if not dest_rel:
            raise RuntimeError(f"No target mapping for extension class: {class_name}")
        write_text(COMMON / dest_rel, wrap_namespace(usings, "    " + body.replace("\n", "\n    ")))

    compatibility = extract_compatibility_methods(util_text)
    write_text(
        COMMON / "Infrastructure/CompatibilityMethods.cs",
        wrap_namespace(usings, "    " + compatibility.replace("\n", "\n    ")),
    )


def remove_shared_folder() -> None:
    if SHARED.exists():
        shutil.rmtree(SHARED)


def update_csproj() -> None:
    csproj = SRC / "Ara3D.RevitSampleBrowser.csproj"
    text = read_text(csproj)
    text = text.replace('    <Compile Include="TBC_Shared\\**\\*.cs" />\n', "")
    write_text(csproj, text)


def main() -> None:
    if not SHARED.exists():
        raise SystemExit(f"Missing source folder: {SHARED}")

    move_standalone_files()
    split_and_write_util()
    remove_shared_folder()
    update_csproj()
    print("TBC_Shared migrated to Common successfully.")


if __name__ == "__main__":
    main()
