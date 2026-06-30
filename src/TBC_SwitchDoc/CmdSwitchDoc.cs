#region Header

//
// CmdSwitchDoc.cs - switch document or view
//
// Copyright (C) 2018-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdSwitchDoc : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var rc = Result.Succeeded;

            var ids
                = uidoc.Selection.GetElementIds();

            var zoomToPreselectedElements
                = 0 < ids.Count;

            if (zoomToPreselectedElements)
            {
                rc = Util.ZoomToElements(uidoc, ids,
                    ref message, elements);
            }
            else
            {
                var filepath = "C:/test/xyz.rfa";

                Util.ToggleViews(doc.ActiveView, filepath);
            }

            return rc;
        }
    }
}
