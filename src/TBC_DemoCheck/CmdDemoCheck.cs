#region Header

//
// CmdDemoCheck.cs - Check whether the running Revit application is a demo version
//
// Copyright (C) 2013-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using UIFrameworkServices;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdDemoCheck : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var revitHandle = Process
                .GetCurrentProcess().MainWindowHandle;

            var s = Util.GetWindowTextUsingWinApi(
                revitHandle);

            // Much simpler direct access:

            s = Util.GetWindowTextUsingNet();

            // My system returns:
            // "Autodesk Revit 2013 - Not For Resale Version 
            // - [Floor Plan: Level 1 - rac_empty.rvt]"

            var isDemo = s.Contains("VIEWER");

            // Language independent serial number check:

            var serial_number = InfoCenterService.ProductSerialNumber;

            isDemo = serial_number.Equals("000-00000000");

            var sDemo = isDemo ? "Demo" : "Production";

            TaskDialog.Show(
                "Serial Number and Demo Version Check",
                $"Serial number: {serial_number} : {sDemo} version.");

            return Result.Succeeded;
        }
    }
}