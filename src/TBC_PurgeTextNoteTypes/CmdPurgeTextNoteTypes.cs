#region Header

//
// CmdPurgeTextNoteTypes.cs - purge TextNote types, i.e. delete all unused TextNote type instances
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
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

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdPurgeTextNoteTypes : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var unusedTextNoteTypes
                = Util.GetUnusedTextNoteTypes(doc);

            var n = unusedTextNoteTypes.Count;

            var nLoop = 100;

            var sw = new Stopwatch();

            sw.Reset();
            sw.Start();

            for (var i = 0; i < nLoop; ++i)
            {
                unusedTextNoteTypes
                    = Util.GetUnusedTextNoteTypes(doc);

                Debug.Assert(unusedTextNoteTypes.Count == n,
                    "expected same number of unused text note types");
            }

            sw.Stop();
            var ms = sw.ElapsedMilliseconds
                     / (double)nLoop;

            sw.Reset();
            sw.Start();

            for (var i = 0; i < nLoop; ++i)
            {
                unusedTextNoteTypes
                    = Util.GetUnusedTextNoteTypesExcluding(doc);

                Debug.Assert(unusedTextNoteTypes.Count == n,
                    "expected same number of unused texct note types");
            }

            sw.Stop();
            var msExcluding
                = sw.ElapsedMilliseconds
                  / (double)nLoop;

            var t = new Transaction(doc,
                "Purging unused text note types");

            t.Start();

            sw.Reset();
            sw.Start();

            doc.Delete(unusedTextNoteTypes);

            sw.Stop();
            var msDeleting
                = sw.ElapsedMilliseconds
                  / (double)nLoop;

            t.Commit();

            Util.InfoMsg(string.Format(
                "{0} text note type{1} purged. "
                + "{2} ms to collect, {3} ms to collect "
                + "excluding, {4} ms to delete.",
                n, Util.PluralSuffix(n),
                ms, msExcluding, msDeleting));

            return Result.Succeeded;
        }
    }
}