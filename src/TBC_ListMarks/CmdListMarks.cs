#region Header

//
// CmdListMarks.cs - list all door marks
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdListMarks : IExternalCommand
    {
        private const string _the_answer = "42";
        private static readonly bool _modify_existing_marks = true;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            //Autodesk.Revit.Creation.Application creApp = app.Application.Create;
            //Autodesk.Revit.Creation.Document creDoc = doc.Create;

            var doors
                = Util.GetElementsOfType(doc,
                    typeof(FamilyInstance),
                    BuiltInCategory.OST_Doors);

            var n = doors.Count();

            Debug.Print("{0} door{1} found.",
                n, Util.PluralSuffix(n));

            if (0 < n)
            {
                var marks
                    = new Dictionary<string, List<Element>>();

                foreach (FamilyInstance door in doors)
                {
                    var mark = door.get_Parameter(
                            BuiltInParameter.ALL_MODEL_MARK)
                        .AsString();

                    if (!marks.ContainsKey(mark)) marks.Add(mark, []);
                    marks[mark].Add(door);
                }

                var keys = new List<string>(
                    marks.Keys);

                keys.Sort();

                n = keys.Count;

                Debug.Print("{0} door mark{1} found{2}",
                    n, Util.PluralSuffix(n),
                    Util.DotOrColon(n));

                foreach (var mark in keys)
                {
                    n = marks[mark].Count;

                    Debug.Print("  {0}: {1} door{2}",
                        mark, n, Util.PluralSuffix(n));
                }
            }

            n = 0; // count how many elements are modified

            if (_modify_existing_marks)
            {
                using var tx = new Transaction(doc);
                tx.Start("Modify Existing Door Marks");

                //ElementSet els = uidoc.Selection.Elements; // 2014

                var ids = uidoc.Selection
                    .GetElementIds(); // 2015

                //foreach( Element e in els ) // 2014

                foreach (var id in ids) // 2015
                {
                    var e = doc.GetElement(id); // 2015

                    if (e is FamilyInstance
                        && null != e.Category
                        && (int)BuiltInCategory.OST_Doors
                        == e.Category.Id.Value)
                    {
                        e.get_Parameter(
                                BuiltInParameter.ALL_MODEL_MARK)
                            .Set(_the_answer);

                        ++n;
                    }
                }

                tx.Commit();
            }

            // the transaction to modify the database:
            //
            //  ? Result.Succeeded
            //  : Result.Failed;
            //
            // That was only useful before the introduction
            // of the manual and read-only transaction modes.

            return Result.Succeeded;
        }
    }
}