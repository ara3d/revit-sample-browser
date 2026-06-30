#region Header

//
// CmdElectricalLoad.cs - Retrieve electrical load
//
// Copyright (C) 2019-2020 by Alexander Ignatovich and Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;


#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdElectricalLoad : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;

            var familyInstance
                = Util.SelectFamilyInstanceWithApparentLoad(
                    uidoc);

            if (familyInstance == null)
                return Result.Cancelled;

            Util.ElectricalApparentLoadFactory electricalApparentLoadFactory
                = new();

            var apparentLoads = electricalApparentLoadFactory
                .Create(familyInstance);

            TaskDialog.Show("CmdElectricalLoad",
                string.Join("\n", apparentLoads));

            return Result.Succeeded;
        }
    }
}
