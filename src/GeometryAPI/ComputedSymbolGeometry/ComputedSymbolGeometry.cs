// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ComputedSymbolGeometry.CS
{
    public class ComputedSymbolGeometry
    {
        private readonly Options m_options;
        private int m_schemaId = -1;
        private readonly Document m_revitDoc;

        public ComputedSymbolGeometry(Document doc)
        {
            m_revitDoc = doc;
            m_options = new Options();
        }

        public void GetInstanceGeometry()
        {
            FilteredElementCollector instanceCollector = new(m_revitDoc);
            instanceCollector.OfClass(typeof(FamilyInstance));

            var view3DId = ElementId.InvalidElementId;
            var elems = new FilteredElementCollector(m_revitDoc).OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (var e in elems)
            {
                if (e is ViewFamilyType v && v.ViewFamily == ViewFamily.ThreeDimensional)
                {
                    view3DId = e.Id;
                    break;
                }
            }

            var instanceView = View3D.CreateIsometric(m_revitDoc, view3DId);
            ViewOrientation3D instanceViewOrientation3D = new(
                new XYZ(-30.8272352809007, -2.44391067967133, 18.1013736367246),
                new XYZ(0.577350269189626, 0.577350269189626, -0.577350269189626),
                new XYZ(0.408248290463863, 0.408248290463863, 0.816496580927726));
            instanceView.SetOrientation(instanceViewOrientation3D);
            instanceView.SaveOrientation();
            instanceView.Name = "InstanceGeometry";

            var originalView = View3D.CreateIsometric(m_revitDoc, view3DId);
            ViewOrientation3D originalViewOrientation3D = new(
                new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478),
                new XYZ(0, 0.707106781186547, -0.707106781186547), new XYZ(0, 0.707106781186548, 0.707106781186548));
            originalView.SetOrientation(originalViewOrientation3D);
            originalView.SaveOrientation();
            originalView.Name = "OriginalGeometry";

            var transView = View3D.CreateIsometric(m_revitDoc, view3DId);
            ViewOrientation3D transViewOrientation3D =
                new(new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478),
                    new XYZ(0, 0.707106781186547, -0.707106781186547),
                    new XYZ(0, 0.707106781186548, 0.707106781186548));
            transView.SetOrientation(transViewOrientation3D);
            transView.SaveOrientation();
            transView.Name = "TransformedGeometry";

            foreach (FamilyInstance instance in instanceCollector)
            {
                var instanceGeo = instance.get_Geometry(m_options);
                var computedGeo = instance.GetOriginalGeometry(m_options);
                var transformGeo = computedGeo.GetTransformed(instance.GetTransform());

                var objects = instanceGeo.GetEnumerator();
                while (objects.MoveNext())
                {
                    var obj = objects.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, instanceView);
                    }
                }

                var objects1 = computedGeo.GetEnumerator();
                while (objects1.MoveNext())
                {
                    var obj = objects1.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, originalView);
                    }
                }

                var objects2 = transformGeo.GetEnumerator();
                while (objects2.MoveNext())
                {
                    var obj = objects2.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, transView);
                    }
                }
            }

            // Deletes source family instances so only AVF-painted geometry remains visible.
            m_revitDoc.Delete(instanceCollector.ToElementIds());
        }

        private void PaintSolid(Solid solid, View view)
        {
            var viewName = view.Name;
            var sfm = SpatialFieldManager.GetSpatialFieldManager(view)
                      ?? SpatialFieldManager.CreateSpatialFieldManager(view, 1);

            if (m_schemaId != -1)
            {
                var results = sfm.GetRegisteredResults();

                if (!results.Contains(m_schemaId)) m_schemaId = -1;
            }

            if (m_schemaId == -1)
            {
                AnalysisResultSchema resultSchema1 = new($"PaintedSolid {viewName}", "Description");

                var displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(m_revitDoc,
                    $"Real_Color_Surface{viewName}", new AnalysisDisplayColoredSurfaceSettings(),
                    new AnalysisDisplayColorSettings(), new AnalysisDisplayLegendSettings());

                resultSchema1.AnalysisDisplayStyleId = displayStyle.Id;

                m_schemaId = sfm.RegisterResult(resultSchema1);
            }

            var faces = solid.Faces;
            var trf = Transform.Identity;
            foreach (Face face in faces)
            {
                var idx = sfm.AddSpatialFieldPrimitive(face, trf);
                ComputeValueAtPointForFace(face, out var uvPts, out var valList, 1);
                FieldDomainPointsByUV pnts = new(uvPts);
                FieldValues vals = new(valList);
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, m_schemaId);
            }
        }

        private void ComputeValueAtPointForFace(Face face, out IList<UV> uvPts,
            out IList<ValueAtPoint> valList, int measurementNo)
        {
            List<double> doubleList = [];
            uvPts = [];
            valList = [];
            var bb = face.GetBoundingBox();
            for (var u = bb.Min.U; u < bb.Max.U + 0.0000001; u += (bb.Max.U - bb.Min.U) / 1)
                for (var v = bb.Min.V; v < bb.Max.V + 0.0000001; v += (bb.Max.V - bb.Min.V) / 1)
                {
                    UV uvPnt = new(u, v);
                    uvPts.Add(uvPnt);
                    var faceXyz = face.Evaluate(uvPnt);
                    for (var ii = 1; ii <= measurementNo; ii++)
                        doubleList.Add(faceXyz.DistanceTo(XYZ.Zero) * ii);
                    valList.Add(new ValueAtPoint(doubleList));
                    doubleList.Clear();
                }
        }
    }
}
