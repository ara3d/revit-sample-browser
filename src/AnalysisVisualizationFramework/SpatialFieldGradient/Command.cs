// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.AnalysisVisualizationFramework.SpatialFieldGradient.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SpatialFieldGradient : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("CF099951-E66B-4a35-BF7F-2959CA87A42D"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var uiDoc = commandData.Application.ActiveUIDocument;

            Transaction trans = new(doc, "Ara3D.RevitSampleBrowser.AnalysisVisualizationFramework");
            trans.Start();

            var sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView) ?? SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);

            var refList = uiDoc.Selection.PickObjects(ObjectType.Face);
            foreach (var reference in refList)
            {
                IList<UV> uvPts = [];

                List<double> doubleList = new();
                IList<ValueAtPoint> valList = [];
                var face = doc.GetElement(reference).GetGeometryObjectFromReference(reference) as Face;
                var bb = face.GetBoundingBox();
                var min = bb.Min;
                var max = bb.Max;

                for (var u = min.U; u < max.U; u += (max.U - min.U) / 10)
                    for (var v = min.V; v < max.V; v += (max.V - min.V) / 10)
                    {
                        UV uv = new(u, v);
                        if (face.IsInside(uv))
                        {
                            uvPts.Add(uv);
                            doubleList.Add(v + DateTime.Now.Second);
                            valList.Add(new ValueAtPoint(doubleList));
                            doubleList.Clear();
                        }
                    }

                FieldDomainPointsByUV pnts = new(uvPts);
                FieldValues vals = new(valList);
                var idx = sfm.AddSpatialFieldPrimitive(reference);
                AnalysisResultSchema resultSchema = new("Schema 1", "Schema 1 Description");
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, sfm.RegisterResult(resultSchema));
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
