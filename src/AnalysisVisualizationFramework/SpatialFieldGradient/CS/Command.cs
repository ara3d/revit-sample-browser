//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

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

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.AnalysisVisualizationFramework.CS
{
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class SpatialFieldGradient : IExternalCommand
   {
      static AddInId m_appId = new AddInId(new Guid("CF099951-E66B-4a35-BF7F-2959CA87A42D"));
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
          var doc = commandData.Application.ActiveUIDocument.Document;
         var uiDoc = commandData.Application.ActiveUIDocument;

         var trans = new Transaction(doc, "Revit.SDK.Samples.AnalysisVisualizationFramework");
         trans.Start();

         var sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);         
         if (sfm == null) sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);

         IList<Reference> refList = new List<Reference>();
         refList = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face);
                 foreach (var reference in refList)
                 {

                         IList<UV> uvPts = new List<UV>();

                         var doubleList = new List<double>();
                         IList<ValueAtPoint> valList = new List<ValueAtPoint>();
                         var face = doc.GetElement(reference).GetGeometryObjectFromReference(reference)as Face;
                         var bb = face.GetBoundingBox();
                         var min = bb.Min;
                         var max = bb.Max;

                         for (var u = min.U; u < max.U; u += (max.U - min.U) / 10)
                         {
                             for (var v = min.V; v < max.V; v += (max.V - min.V) / 10)
                             {
                                 var uv = new UV(u, v);
                                 if (face.IsInside(uv))
                                 {
                                     uvPts.Add(uv);
                                     doubleList.Add(v + DateTime.Now.Second);
                                     valList.Add(new ValueAtPoint(doubleList));
                                     doubleList.Clear();
                                 }
                             }
                         }

                         var pnts = new FieldDomainPointsByUV(uvPts);
                         var vals = new FieldValues(valList);
                         var idx = sfm.AddSpatialFieldPrimitive(reference);
                         var resultSchema = new AnalysisResultSchema("Schema 1", "Schema 1 Description"); 
                         sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, sfm.RegisterResult(resultSchema));
                 }



         trans.Commit();
         return Result.Succeeded;
      }
   }
}
 