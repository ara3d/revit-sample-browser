#region Header

//
// CmdDuctResize.cs - Ensure that branch ducts are no larger than the main duct they are tapping into
//
// Copyright (C) 2019-2020 by Jared Wilson and Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     Based on code by Jared Wilson shared in case 14918470 [Find all ducts that have been tapped into]
    ///     https://forums.autodesk.com/t5/revit-api-forum/find-all-ducts-that-have-been-tapped-into/m-p/8485269
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdDuctResize : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            Util.DuctResize(doc);

            return Result.Succeeded;
        }
    }
}
