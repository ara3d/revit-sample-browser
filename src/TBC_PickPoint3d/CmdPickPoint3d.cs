#region Header

//
// CmdPickPoint3d.cs - set active work plane to pick a point in 3d
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

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdPickPoint3d : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            //Util.PickPointsForArea( uidoc );

            XYZ point_in_3d;

            if (Util.PickFaceSetWorkPlaneAndPickPoint(
                uidoc, out point_in_3d))
            {
                TaskDialog.Show("3D Point Selected",
                    $"3D point picked on the plane defined by the selected face: {Util.PointString(point_in_3d)}");

                return Result.Succeeded;
            }

            message = "3D point selection cancelled or failed";
            return Result.Failed;
        }
    }
}
