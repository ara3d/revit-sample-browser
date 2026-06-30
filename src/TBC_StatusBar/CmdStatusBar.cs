#region Header

//
// CmdStatusBar.cs - set the status bar text using Windows API
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdStatusBar : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var revitHandle = Process
                .GetCurrentProcess().MainWindowHandle;

            Util.SetStatusText(revitHandle, "Kilroy was here.");

            return Result.Succeeded;
        }
    }
}
