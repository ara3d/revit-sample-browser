#!/usr/bin/env python3
"""Import reusable RevitDBExplorer utilities into src/Common/."""

from __future__ import annotations

import json
import re
import shutil
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
VENDOR = REPO / "_vendor" / "RevitDBExplorer"
UPSTREAM_ROOT = VENDOR / "sources" / "RevitDBExplorer"
MANIFEST_PATH = REPO / "scripts" / "rdbe-import-manifest.json"
PINNED_TAG = "v2.6.1"
PINNED_COMMIT = "6929da81491a7f9ef69ed4c346afa1c582b830b5"

ARA3D_HEADER = """// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ {commit}

"""

NS_QUERY = "Ara3D.RevitSampleBrowser.Common.Documents.Query"
NS_DOCUMENTS = "Ara3D.RevitSampleBrowser.Common.Documents"
NS_PARAMETERS = "Ara3D.RevitSampleBrowser.Common.Parameters"
NS_INFRA = "Ara3D.RevitSampleBrowser.Common.Infrastructure"

TIER1_REVIT = [
    ("Extensions/Autodesk.Revit.DB/GeometryObjectExtensions.cs", "src/Common/Document/GeometryObjectExtensions.cs", NS_DOCUMENTS),
    ("Extensions/Autodesk.Revit.DB/DocumentExtensions.cs", "src/Common/Document/DocumentExtensions.cs", NS_DOCUMENTS),
    ("Extensions/Autodesk.Revit.DB/BoundingBoxXYZExtensions.cs", "src/Common/Document/BoundingBoxXYZExtensions.cs", NS_DOCUMENTS),
    ("Extensions/Autodesk.Revit.DB/XYZExtensions.cs", "src/Common/Document/XYZExtensions.cs", NS_DOCUMENTS),
    ("Extensions/Autodesk.Revit.DB/CurveArrayExtensions.cs", "src/Common/Document/CurveArrayExtensions.cs", NS_DOCUMENTS),
    ("Extensions/Autodesk.Revit.DB/ParameterExtensions.cs", "src/Common/Parameters/ParameterExtensions.cs", NS_PARAMETERS),
]

TIER2_INFRA = [
    ("Extensions/System/StringExtensions.cs", "src/Common/Infrastructure/StringExtensions.cs", NS_INFRA),
    ("Extensions/System/Collections.Generic/IEnumerableExtensions.cs", "src/Common/Infrastructure/IEnumerableExtensions.cs", NS_INFRA),
    ("Extensions/System/DoubleExtensions.cs", "src/Common/Infrastructure/DoubleExtensions.cs", NS_INFRA),
]

# RDQ subtree (relative to Domain/RevitDatabaseQuery); skip WPF autocompletion UI
RDQ_SKIP = {
    "Autocompletion/AutocompleteItemProvider.cs",
    "Autocompletion/FavoritesManager.cs",
}


def ensure_vendor() -> None:
    if UPSTREAM_ROOT.is_dir():
        return
    VENDOR.parent.mkdir(parents=True, exist_ok=True)
    subprocess.run(
        [
            "git", "clone", "--depth", "1", "--branch", PINNED_TAG,
            "https://github.com/NeVeSpl/RevitDBExplorer.git",
            str(VENDOR),
        ],
        check=True,
    )


def strip_upstream_header(text: str) -> str:
    text = re.sub(
        r"^// \(c\) Revit Database Explorer.*\n",
        "",
        text,
        flags=re.MULTILINE,
    )
    return text.lstrip("\ufeff")


def simplify_revit2025_ifdefs(text: str) -> str:
    """Keep Revit 2023+ / 2022+ code paths; drop older API branches."""
    for old in ("R2021_MAX", "R2022_MAX"):
        text = re.sub(rf"#if {old}.*?#endif\s*", "", text, flags=re.DOTALL)
    for keep in ("R2023_MIN", "R2022_MIN"):
        text = re.sub(rf"#if {keep}\s*\n(.*?)#endif\s*", r"\1", text, flags=re.DOTALL)
    text = text.replace(
        "Application.UIApplication?.ActiveUIDocument?.Document?.GetUnits()",
        "RevitDatabaseQueryHost.Document?.GetUnits()",
    )
    text = text.replace(
        "ParameterUtils.GetAllBuiltInParameters()",
        "Autodesk.Revit.DB.ParameterUtils.GetAllBuiltInParameters()",
    )
    text = text.replace(
        "ParameterUtils.GetBuiltInParameter(x)",
        "Autodesk.Revit.DB.ParameterUtils.GetBuiltInParameter(x)",
    )
    return text


def write_query_host() -> None:
    dest = REPO / "src/Common/Document/Query/RevitDatabaseQueryHost.cs"
    content = ARA3D_HEADER.format(commit=PINNED_COMMIT) + f"""using Autodesk.Revit.DB;

namespace {NS_QUERY}
{{
    /// <summary>
    /// Holds the active document during query parsing (replaces RevitDBExplorer.Application static state).
    /// </summary>
    public static class RevitDatabaseQueryHost
    {{
        public static Document Document {{ get; set; }}
    }}
}}
"""
    dest.write_text(content, encoding="utf-8")


def rewrite_cs(text: str, target_ns: str | None, is_query: bool = False) -> str:
    text = strip_upstream_header(text)

    if is_query:
        text = text.replace("RevitDBExplorer.Domain.RevitDatabaseQuery", NS_QUERY)
        text = re.sub(r"\binternal\b", "public", text)
    elif target_ns:
        # Replace injected namespaces with explicit Ara3D namespace
        text = re.sub(r"namespace\s+Autodesk\.Revit\.DB\s*\{", f"namespace {target_ns}\n{{", text)
        text = re.sub(r"namespace\s+System\s*\{", f"namespace {target_ns}\n{{", text)
        text = re.sub(r"namespace\s+System\.Collections\.Generic\s*\{", f"namespace {target_ns}\n{{", text)
        text = re.sub(r"\binternal\b", "public", text)

    # Revit 2025: ElementId uses long Value
    text = re.sub(
        r"public static long Value\(this ElementId id\)\s*\{[^}]+\}",
        "public static long Value(this ElementId id) => id.Value;",
        text,
        flags=re.DOTALL,
    )
    text = re.sub(
        r"public static ElementId Create\(long id\)\s*\{[^}]+\}",
        "public static ElementId Create(long id) => new ElementId(id);",
        text,
        flags=re.DOTALL,
    )
    text = re.sub(r"#if R2023_MAX.*?#endif\s*", "", text, flags=re.DOTALL)
    text = re.sub(r"#if R2024_MIN.*?#endif\s*", "", text, flags=re.DOTALL)
    text = simplify_revit2025_ifdefs(text)

    if is_query or target_ns in (NS_DOCUMENTS, NS_PARAMETERS):
        if "using Autodesk.Revit.DB;" not in text:
            text = "using Autodesk.Revit.DB;\n\n" + text

    if target_ns == NS_DOCUMENTS:
        if "using System.Collections.Generic;" not in text and "IEnumerable<" in text:
            text = "using System.Collections.Generic;\n\n" + text
        if "XYZExtensions" in text and "using System;" not in text:
            text = "using System;\n\n" + text

    if target_ns == NS_INFRA:
        if "StringExtensions" in text:
            for u in ("using System;", "using System.Collections.Generic;"):
                if u not in text:
                    text = u + "\n" + text
        if "IEnumerableExtensions" in text:
            for u in ("using System.Collections.Generic;", "using System.Linq;"):
                if u not in text:
                    text = u + "\n" + text
        if "DoubleExtensions" in text and "using Autodesk.Revit.DB;" not in text:
            text = "using Autodesk.Revit.DB;\n\n" + text

    if is_query:
        extras = [
            "using Ara3D.RevitSampleBrowser.Common.Documents;",
            "using Ara3D.RevitSampleBrowser.Common.Infrastructure;",
        ]
        for line in extras:
            if line not in text:
                text = line + "\n" + text

    # DoubleExtensions: avoid conflict with Common.Units namespace
    text = text.replace(
        "this double value, Units units)",
        "this double value, Autodesk.Revit.DB.Units units)",
    )

    return ARA3D_HEADER.format(commit=PINNED_COMMIT) + text


def write_element_id_extensions() -> None:
    dest = REPO / "src/Common/Document/ElementIdExtensions.cs"
    content = ARA3D_HEADER.format(commit=PINNED_COMMIT) + f"""using Autodesk.Revit.DB;

namespace {NS_DOCUMENTS}
{{
    public static class ElementIdExtensions
    {{
        public static long Value(this ElementId id) => id.Value;

        public static bool IsInvalid(this ElementId id) =>
            ElementId.InvalidElementId == id;

        public static bool IsValid(this ElementId id) => !id.IsInvalid();
    }}

    public static class ElementIdFactory
    {{
        public static ElementId Create(long id) => new ElementId(id);
    }}
}}
"""
    dest.write_text(content, encoding="utf-8")


def update_jt_element_id() -> None:
    path = REPO / "src/Common/Document/JtElementIdExtensionMethods.cs"
    text = path.read_text(encoding="utf-8")
    replacement = """            public static bool IsInvalid(this ElementId id)
            {
                return ElementIdExtensions.IsInvalid(id);
            }
    
            public static bool IsValid(this ElementId id)
            {
                return ElementIdExtensions.IsValid(id);
            }"""
    text = re.sub(
        r"public static bool IsInvalid\(this ElementId id\)\s*\{[^}]+\}\s*"
        r"public static bool IsValid\(this ElementId id\)\s*\{[^}]+\}",
        replacement,
        text,
        flags=re.DOTALL,
    )
    if "using Ara3D.RevitSampleBrowser.Common.Documents;" not in text:
        text = text.replace(
            "using Autodesk.Revit.DB;",
            "using Autodesk.Revit.DB;\nusing Ara3D.RevitSampleBrowser.Common.Documents;",
        )
    path.write_text(text, encoding="utf-8")


def copy_tier_file(rel_src: str, rel_dest: str, ns: str) -> None:
    src = UPSTREAM_ROOT / rel_src.replace("/", "\\") if False else UPSTREAM_ROOT / rel_src
    dest = REPO / rel_dest
    dest.parent.mkdir(parents=True, exist_ok=True)
    text = src.read_text(encoding="utf-8")
    dest.write_text(rewrite_cs(text, ns), encoding="utf-8")


def copy_rdq_tree() -> list[str]:
    imported: list[str] = []
    rdq_root = UPSTREAM_ROOT / "Domain" / "RevitDatabaseQuery"
    dest_root = REPO / "src/Common/Document/Query"
    for src in rdq_root.rglob("*.cs"):
        rel = src.relative_to(rdq_root).as_posix()
        if rel in RDQ_SKIP:
            continue
        dest = dest_root / rel
        dest.parent.mkdir(parents=True, exist_ok=True)
        text = rewrite_cs(src.read_text(encoding="utf-8"), None, is_query=True)
        dest.write_text(text, encoding="utf-8")
        imported.append(f"src/Common/Document/Query/{rel}")
    return imported


def patch_query_service() -> None:
    path = REPO / "src/Common/Document/Query/RevitDatabaseQueryService.cs"
    text = path.read_text(encoding="utf-8")
    text = text.replace(
        "using Ara3D.RevitSampleBrowser.Common.Documents.Query.DataModel;\n", ""
    )
    text = re.sub(
        r"using [^\n]*DataModel[^\n]*\n", "", text
    )

    old_result = """        public static Result ParseAndExecute(Document document, string query)
        {
            if (document is null) return new Result(null, new List<ICommand>(), new SourceOfObjects() { Info = new InfoAboutSource("Query") });"""

    new_result = """        public static Result ParseAndExecute(Document document, string query)
        {
            RevitDatabaseQueryHost.Document = document;
            if (document is null) return new Result(null, new List<ICommand>(), null);"""

    text = text.replace(old_result, new_result)

    text = text.replace(
        "return new Result(providerSyntax + collectorSyntax, commands, new SourceOfObjects(queryExecutor) { Info = new InfoAboutSource(\"Query\") });",
        "return new Result(providerSyntax + collectorSyntax, commands, queryExecutor);",
    )

    text = text.replace(
        "public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, SourceOfObjects SourceOfObjects);",
        "public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, IQueryExecutor Executor);",
    )

    executor_block = """        private class QueryPipeExecutor : IAmSourceOfEverything
        {
            private readonly IReadOnlyList<QueryItem> filterPipe;
            private readonly IReadOnlyList<Provider> seedPipe;


            public QueryPipeExecutor(IReadOnlyList<QueryItem> filterPipe, IReadOnlyList<Provider> seedPipe)
            {
                this.filterPipe = filterPipe;
                this.seedPipe = seedPipe;
            }


            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                var document = app.ActiveUIDocument?.Document;
                if (document == null) return null;

                ICollection<ElementId> elementIds = GatherInitialSeed(app.ActiveUIDocument).ToArray();

                if (seedPipe.Any() && elementIds.IsEmpty())
                {
                     return null;
                }

                var collector = BuildCollector(document, elementIds);
                var snoopableObjects = collector.ToElements().Select(x => new SnoopableObject(document, x));

                return snoopableObjects;
            }"""

    new_executor_block = """        private class QueryPipeExecutor : IQueryExecutor
        {
            private readonly IReadOnlyList<QueryItem> filterPipe;
            private readonly IReadOnlyList<Provider> seedPipe;

            public QueryPipeExecutor(IReadOnlyList<QueryItem> filterPipe, IReadOnlyList<Provider> seedPipe)
            {
                this.filterPipe = filterPipe;
                this.seedPipe = seedPipe;
            }

            public IList<Element> Execute(UIApplication app)
            {
                var uiDocument = app.ActiveUIDocument;
                if (uiDocument?.Document == null)
                    return Array.Empty<Element>();
                return Execute(uiDocument.Document, uiDocument);
            }

            public IList<Element> Execute(Document document, UIDocument uiDocument)
            {
                if (document == null)
                    return Array.Empty<Element>();

                ICollection<ElementId> elementIds = uiDocument != null
                    ? GatherInitialSeed(uiDocument).ToArray()
                    : Array.Empty<ElementId>();

                if (seedPipe.Any() && elementIds.IsEmpty())
                    return Array.Empty<Element>();

                return BuildCollector(document, elementIds).ToElements();
            }"""

    text = text.replace(executor_block, new_executor_block)

    iface = f"""// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace {NS_QUERY}
{{
    public interface IQueryExecutor
    {{
        IList<Element> Execute(UIApplication app);
        IList<Element> Execute(Document document, UIDocument uiDocument);
    }}
}}
"""
    iface_path = REPO / "src/Common/Document/Query/IQueryExecutor.cs"
    if not iface_path.exists():
        iface_path.write_text(
            ARA3D_HEADER.format(commit=PINNED_COMMIT)
            + f"""using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace {NS_QUERY}
{{
    public interface IQueryExecutor
    {{
        IList<Element> Execute(UIApplication app);
        IList<Element> Execute(Document document, UIDocument uiDocument);
    }}
}}
""",
            encoding="utf-8",
        )

    path.write_text(text, encoding="utf-8")


def fix_parameter_origin_typo() -> None:
    path = REPO / "src/Common/Parameters/ParameterExtensions.cs"
    text = path.read_text(encoding="utf-8")
    text = text.replace("ParameterOrgin", "ParameterOrigin")
    text = text.replace("GetOrgin", "GetOrigin")
    if "using Ara3D.RevitSampleBrowser.Common.Documents;" not in text:
        text = text.replace(
            "namespace ",
            "using Ara3D.RevitSampleBrowser.Common.Documents;\n\nnamespace ",
            1,
        )
    path.write_text(text, encoding="utf-8")


def main() -> int:
    ensure_vendor()
    manifest: dict = {
        "upstream": "https://github.com/NeVeSpl/RevitDBExplorer",
        "tag": PINNED_TAG,
        "commit": PINNED_COMMIT,
        "imported": [],
    }

    write_element_id_extensions()
    manifest["imported"].append("src/Common/Document/ElementIdExtensions.cs")

    for rel_src, rel_dest, ns in TIER1_REVIT:
        copy_tier_file(rel_src, rel_dest, ns)
        manifest["imported"].append(rel_dest)

    for rel_src, rel_dest, ns in TIER2_INFRA:
        copy_tier_file(rel_src, rel_dest, ns)
        manifest["imported"].append(rel_dest)

    rdq_files = copy_rdq_tree()
    manifest["imported"].extend(rdq_files)

    write_query_host()
    manifest["imported"].append("src/Common/Document/Query/RevitDatabaseQueryHost.cs")

    patch_query_service()
    fix_parameter_origin_typo()
    update_jt_element_id()

    manifest["skipped"] = [
        "Autocompletion/AutocompleteItemProvider.cs (WPF)",
        "Autocompletion/FavoritesManager.cs (WPF)",
    ]

    MANIFEST_PATH.write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(f"Imported {len(manifest['imported'])} files. Manifest: {MANIFEST_PATH}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
