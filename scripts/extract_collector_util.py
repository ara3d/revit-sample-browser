import re
from pathlib import Path

src_path = Path(r'c:\Users\cdigg\git\revit-sample-browser\src\TBC_CollectorPerformance\CmdCollectorPerformance.cs')
text = src_path.read_text(encoding='utf-8')

class_match = re.search(
    r'internal class CmdCollectorPerformance : IExternalCommand\s*\{(.*)\n    \}\n\n    #region YBExporteContext',
    text, re.DOTALL)
if not class_match:
    raise SystemExit('Could not find class body')
body = class_match.group(1)

body = re.sub(r'\s*private Document _doc;\s*\n', '\n', body)

body = re.sub(
    r'\s*public Result Execute\(\s*ExternalCommandData commandData,.*?\n        \}\n',
    '\n',
    body, count=1, flags=re.DOTALL)

body = re.sub(
    r'\s*\[Transaction\(TransactionMode\.ReadOnly\)\]\s*\n\s*public class RevitCommand : IExternalCommand\s*\{.*?\n        \}\n',
    '\n',
    body, count=1, flags=re.DOTALL)

body = body.replace('GetElementsOfType(', 'CollectorGetElementsOfType(')
body = body.replace('GetFirstElementOfType(', 'CollectorGetFirstElementOfType(')
body = body.replace('GetSolid(', 'CollectorGetSolid(')
body = body.replace('_doc', 'doc')

patterns_to_fix = [
    (r'private void F\(\)', 'private static void F(Document doc)'),
    (r'private void F3\(\)', 'private static void F3(Document doc)'),
    (r'private void F2\(\)', 'private static void F2(Document doc)'),
    (r'private Level CreateLevel\(', 'private static Level CreateLevel(Document doc, '),
    (r'private void GetDetailCurves\(\)', 'private static void GetDetailCurves(Document doc)'),
    (r'private void RunBenchmark\(\)', 'private static void RunBenchmark(Document doc)'),
    (r'private void BenchmarkAllLevels\(', 'private static void BenchmarkAllLevels(Document doc, '),
    (r'private void BenchmarkSpecificLevel\(', 'private static void BenchmarkSpecificLevel(Document doc, '),
    (r'private FilteredElementCollector GetNonElementTypeElements\(\)', 'private static FilteredElementCollector GetNonElementTypeElements(Document doc)'),
    (r'private FilteredElementCollector CollectorGetElementsOfType\(\s*\n\s*Type type\)', 'private static FilteredElementCollector CollectorGetElementsOfType(\n            Document doc,\n            Type type)'),
    (r'private Element CollectorGetFirstElementOfType\(\s*\n\s*Type type\)', 'private static Element CollectorGetFirstElementOfType(\n            Document doc,\n            Type type)'),
    (r'private Element GetFirstElementOfTypeWithBipString\(\s*\n\s*Type type,', 'private static Element GetFirstElementOfTypeWithBipString(\n            Document doc,\n            Type type,'),
    (r'private List<Element> GetElementsOfTypeUsingExplicitCode\(\s*\n\s*Type type\)', 'private static List<Element> GetElementsOfTypeUsingExplicitCode(\n            Document doc,\n            Type type)'),
    (r'private IEnumerable<Element> GetElementsOfTypeUsingLinq\(\s*\n\s*Type type\)', 'private static IEnumerable<Element> GetElementsOfTypeUsingLinq(\n            Document doc,\n            Type type)'),
    (r'private Element GetFirstNamedElementOfTypeUsingExplicitCode\(\s*\n\s*Type type,', 'private static Element GetFirstNamedElementOfTypeUsingExplicitCode(\n            Document doc,\n            Type type,'),
    (r'private Element GetFirstNamedElementOfTypeUsingLinq\(\s*\n\s*Type type,', 'private static Element GetFirstNamedElementOfTypeUsingLinq(\n            Document doc,\n            Type type,'),
    (r'private Element GetFirstNamedElementOfTypeUsingAnonymousButNamedMethod\(\s*\n\s*Type type,', 'private static Element GetFirstNamedElementOfTypeUsingAnonymousButNamedMethod(\n            Document doc,\n            Type type,'),
    (r'private Element GetFirstNamedElementOfTypeUsingAnonymousMethod\(\s*\n\s*Type type,', 'private static Element GetFirstNamedElementOfTypeUsingAnonymousMethod(\n            Document doc,\n            Type type,'),
    (r'private void GetInstancesIntersectingElement\(', 'internal static void GetInstancesIntersectingElement('),
    (r'private List<string> ListElementsInAssembly\(', 'internal static List<string> ListElementsInAssembly('),
    (r'private Element EmptyMethod\(\s*\n\s*Type type\)', 'private static Element EmptyMethod(\n            Document doc,\n            Type type)'),
    (r'private Element EmptyMethod\(\s*\n\s*Type type,\s*\n\s*string name\)', 'private static Element EmptyMethod(\n            Document doc,\n            Type type,\n            string name)'),
]
for pat, repl in patterns_to_fix:
    body = re.sub(pat, repl, body)

body = body.replace('CollectorGetElementsOfType(type)', 'CollectorGetElementsOfType(doc, type)')
body = body.replace('GetNonElementTypeElements()', 'GetNonElementTypeElements(doc)')
body = body.replace('CreateLevel(i)', 'CreateLevel(doc, i)')
body = body.replace('EmptyMethod(\n                    t, name)', 'EmptyMethod(\n                    doc, t, name)')
body = body.replace('EmptyMethod(t)', 'EmptyMethod(doc, t)')
body = body.replace('CollectorGetElementsOfType(typeof(Level))', 'CollectorGetElementsOfType(doc, typeof(Level))')
body = body.replace('BenchmarkAllLevels(nLevels)', 'BenchmarkAllLevels(doc, nLevels)')
body = body.replace('BenchmarkSpecificLevel(iLevel)', 'BenchmarkSpecificLevel(doc, iLevel)')
body = body.replace('RunBenchmark()', 'RunBenchmark(doc)')
body = body.replace('CollectorGetFirstElementOfType(t)', 'CollectorGetFirstElementOfType(doc, t)')

body = re.sub(r'\n        private (?!static )', '\n        private static ', body)
body = re.sub(r'\n        public (?!static )', '\n        internal static ', body)
body = re.sub(r'\n        internal static static ', '\n        internal static ', body)
body = re.sub(r'\n        private static static ', '\n        private static ', body)

body = body.replace('IEnumerable<IndependentTag> GetMaterialTags', 'private static IEnumerable<IndependentTag> GetMaterialTags')
body = body.replace('IEnumerable<IndependentTag> RetrieveRectangularFabricationPartTags', 'private static IEnumerable<IndependentTag> RetrieveRectangularFabricationPartTags')
body = body.replace('IEnumerable<Element> GetFittingTypesOfPartType', 'private static IEnumerable<Element> GetFittingTypesOfPartType')
body = body.replace('public class ColumnMarkComparer', 'private class ColumnMarkComparer')

header = '''using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_CollectorPerformance sample.</summary>
    internal static partial class Util
    {
'''

footer = '''
    }
}
'''

util_text = header + body + footer
out_path = Path(r'c:\Users\cdigg\git\revit-sample-browser\src\TBC_CollectorPerformance\Util.cs')
out_path.write_text(util_text, encoding='utf-8')
print(f'Wrote {out_path} ({len(util_text)} chars)')

cmd_header = text[:text.index('internal class CmdCollectorPerformance')]
cmd_footer = text[text.index('#region YBExporteContext'):]
slim_cmd = cmd_header + '''internal class CmdCollectorPerformance : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            Util.ListElementsInAssembly(doc);

            //Util.RunBenchmark(doc);

            var wall = Util.SelectSingleElementOfType(
                uidoc, typeof(Wall), "a wall", true);

            Util.GetInstancesIntersectingElement(wall);

            return Result.Succeeded;
        }
    }

    ''' + cmd_footer
src_path.write_text(slim_cmd, encoding='utf-8')
print('Updated CmdCollectorPerformance.cs')
