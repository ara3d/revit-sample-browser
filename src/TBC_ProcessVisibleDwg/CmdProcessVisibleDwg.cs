#region Header

//
// CmdProcessVisibleDwg.cs - extract geometry from visible import instance layers in the current view
//
// Copyright (C) 2018-2020 by Ryan Goertzen, Goertzen Enterprises, and Jeremy Tammik, Autodesk Inc. All rights reserved.
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
    internal class CmdProcessVisibleDwg : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            Util.ProcessVisible(uidoc);

            return Result.Succeeded;
        }
    }
}
