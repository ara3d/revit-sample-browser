#region Header

//
// CmdWindowHandle.cs - determine Revit
// application main window handle.
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdWindowHandle : IExternalCommand
    {
        private const string _prompt
            = "Please select some elements.";

        private static WindowHandle _hWndRevit;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (null == _hWndRevit)
                _hWndRevit = Util.GetRevitWindowHandle();

            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var sel = uidoc.Selection;

            using var f
                = new CmdWindowHandleForm();
            f.Show(_hWndRevit);
            var go = true;
            while (go)
            {
                var ids = sel.GetElementIds();
                var n = ids.Count;

                var s = $"{n} element{Util.PluralSuffix(n)} selected{(0 == n ? $";\n{_prompt}" : ":")}";

                foreach (var id in ids)
                {
                    s += "\n";
                    s += Util.ElementDescription(
                        doc.GetElement(id));
                }

                f.LabelText = s;

                var r = uidoc.Selection.PickObject(
                    ObjectType.Element, _prompt);

                go = null != r;

                Debug.Print($"go = {go}");
            }

            return Result.Failed;
        }

        // Modified by Tao (Tau) Yang to use WindowSelect.
        public Result Execute2(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (null == _hWndRevit)
                _hWndRevit = Util.GetRevitWindowHandle();

            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var sel = uidoc.Selection;

            using var f
                = new CmdWindowHandleForm();
            f.Show(_hWndRevit);
            var go = true;
            while (go)
            {
                var a = sel.PickElementsByRectangle(_prompt);
                go = 0 < a.Count;

                var ids = sel.GetElementIds();
                var n = ids.Count;

                var s = $"{n} element{Util.PluralSuffix(n)} selected{(0 == n ? $";\n{_prompt}" : ":")}";

                foreach (var id in ids)
                {
                    s += "\n";
                    s += Util.ElementDescription(
                        doc.GetElement(id));
                }

                f.LabelText = s;
                Debug.Print($"go = {go}");
            }

            return Result.Failed;
        }
    }
}
