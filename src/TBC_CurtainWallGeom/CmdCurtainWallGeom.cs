#region Header

//
// CmdCurtainWallGeom.cs - retrieve curtain wall geometry
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdCurtainWallGeom : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            if (Util.SelectSingleElementOfType(
                uidoc, typeof(Wall), "a curtain wall", false) is not Wall wall)
            {
                message = "Please select a single "
                          + "curtain wall element.";

                return Result.Failed;
            }

            var locationcurve
                = wall.Location as LocationCurve;

            var curve = locationcurve.Curve;

            // move whole geometry over by length of wall:

            var p = curve.GetEndPoint(0);
            var q = curve.GetEndPoint(1);
            var v = q - p;

            var tv = Transform.CreateTranslation(v);

            //curve = curve.get_Transformed( tv ); // 2013
            curve = curve.CreateTransformed(tv); // 2014

            Creator creator = new(doc);
            creator.CreateModelCurve(curve);

            var opt = app.Create.NewGeometryOptions();
            opt.IncludeNonVisibleObjects = true;

            var e = wall.get_Geometry(opt);

            using Transaction t = new(doc);
            t.Start("Create Model Curves");

            foreach (var obj in e)
            {
                curve = obj as Curve;

                if (null != curve)
                {
                    //curve = curve.get_Transformed( tv ); // 2013
                    curve = curve.CreateTransformed(tv); // 2014
                    creator.CreateModelCurve(curve);
                }
            }

            t.Commit();

            return Result.Succeeded;
        }
    }
}

// C:\a\j\adn\case\bsd\1259898\attach\curtain_wall.rvt