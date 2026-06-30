#region Header

//
// CmdIdling.cs - subscribe to the Idling event
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
using Autodesk.Revit.UI.Events;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdIdling : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Util.LogIdlingMessage("Execute begin");

            var uiapp = commandData.Application;

            uiapp.Idling += OnIdling;

            Util.LogIdlingMessage("Execute end");

            return Result.Succeeded;
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var uiapp = sender as UIApplication;
            var doc = uiapp.ActiveUIDocument.Document;

            Util.LogIdlingMessage(
                $"OnIdling with active document {doc.Title}");
        }
    }
}
