#region Header

//
// CmdAnalyticalModelGeom.cs - retrieve analytical model geometry
//
// Copyright (C) 2011-2022 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdAnalyticalModelGeom : IExternalCommand
    {
#if USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
        private static readonly IEnumerable<AnalyticalCurveType>
            CurveTypes = Enum.GetValues(typeof(AnalyticalCurveType))
                .Cast<AnalyticalCurveType>();
#endif // USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
        private static readonly XYZ Offset = new(100, 0, 0);
        //static Transform _t = Transform.get_Translation( _offset ); // 2013
        private static readonly Transform T = Transform.CreateTranslation(Offset); // 2014

        public Result Execute(
                    ExternalCommandData commandData,
                    ref string message,
                    ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            List<Element> walls = new();

            //XYZ p;
            //List<XYZ> wall_start_points
            //  = walls.Select<Element, XYZ>( e => {
            //    Util.GetElementLocation( out p, e );
            //      return p; } )
            //        .ToList<XYZ>();

            if (!Util.GetSelectedElementsOrAll(
                walls, uidoc, typeof(Wall)))
            {
                var sel = uidoc.Selection;
                //message = ( 0 < sel.Elements.Size ) // 2014
                message = 0 < sel.GetElementIds().Count // 2015
                    ? "Please select some wall elements."
                    : "No wall elements found.";
                return Result.Failed;
            }


            using Transaction tx = new(doc);
            tx.Start("Create model curve copies of analytical model curves");

            Creator creator = new(doc);

            foreach (Wall wall in walls)
            {
                // The analytical model changed in Revit 2023
                // This approach was possible previously:

#if USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
                var am = wall.GetAnalyticalModel(); // 2022

                //AnalyticalToPhysicalRelationManager.GetCounterpartsIds

                //AnalyticalElement ae = null;

                foreach (var ct in CurveTypes)
                {
                    var curves = am.GetCurves(ct);

                    var n = curves.Count;

                    Debug.Print("{0} {1} curve{2}.",
                        n, ct, Util.PluralSuffix(n));

                    foreach (var curve in curves)
                        //creator.CreateModelCurve( curve.get_Transformed( _t ) ); // 2013

                        creator.CreateModelCurve(curve.CreateTransformed(T)); // 2014
                }
#endif // USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023

                var id = Util.GetAnalyticalElementId(wall);

            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}
//#endif // USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
