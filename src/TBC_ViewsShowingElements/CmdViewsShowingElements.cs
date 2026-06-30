#region Header

//
// CmdViewsShowingElements.cs - determine all views displaying a given set of elements
//
// By Colin, cshha, 
// http://forums.autodesk.com/t5/user/viewprofilepage/user-id/1162312
// published in 
// http://forums.autodesk.com/t5/Revit-API/Revision-help-which-views-show-this-object/m-p/5029772
//
// Copyright (C) 2014-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdViewsShowingElements : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var uiapp = revit.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var ids
                = uidoc.Selection.GetElementIds();

            if (0 == ids.Count)
            {
                message = "Please pre-select some elements "
                          + "before launching this command to list "
                          + "the views displaying them.";

                return Result.Failed;
            }

            var targets
                = from id in ids
                select doc.GetElement(id);

            var views = targets
                .FindAllViewsWhereAllElementsVisible();

            var names = string.Join(", ",
                from v in views
                select v.Name);

            var nElems = targets.Count();

            var nViews = names.Count(
                c => ',' == c) + 1;

            var dlg = new TaskDialog($"{nElems} element{Util.PluralSuffix(nElems)} are visible in {nViews} view{Util.PluralSuffix(nViews)}");

            dlg.MainInstruction = names;

            dlg.Show();

            return Result.Succeeded;
        }
    }
}
