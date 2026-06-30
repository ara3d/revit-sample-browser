#region Header

//
// CmdSheetToModel.cs - Convert sheet to model coordinates and convert DWF markup to model elements
//
// Copyright (C) 2015-2020 by Paolo Serra and Jeremy Tammik,
// Autodesk Inc. All rights reserved.
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
    [Transaction(TransactionMode.Manual)]
    internal class CmdSheetToModel : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var doc = uiapp.ActiveUIDocument.Document;

            Util.QTO_2_PlaceHoldersFromDWFMarkups(
                doc, "DWF Markup");

            return Result.Succeeded;
        }
    }
}
