// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    public class AnalysisVisualizationFramework
    {
        private static AnalysisVisualizationFramework _instance;
        private static int _schemaId = -1;
        private readonly Document m_doc;
        private readonly List<string> m_viewNameList = [];

        private AnalysisVisualizationFramework(Document doc)
        {
            m_doc = doc;
        }

        public static AnalysisVisualizationFramework GetInstance(Document doc)
        {
            return _instance ??= new AnalysisVisualizationFramework(doc);
        }

        public void PaintSolid(Solid s, string viewName)
        {
            View view;
            if (!m_viewNameList.Contains(viewName))
            {
                var viewFamilyTypes = new FilteredElementCollector(m_doc).OfClass(typeof(ViewFamilyType)).ToElements();
                var view3DId = ElementId.InvalidElementId;
                foreach (var e in viewFamilyTypes)
                {
                    if (e.Name == "3D View")
                        view3DId = e.Id;
                }

                view = View3D.CreateIsometric(m_doc, view3DId);
                ViewOrientation3D viewOrientation3D = new(new XYZ(1, -1, -1), new XYZ(1, 1, 1), new XYZ(1, 1, -2));
                (view as View3D).SetOrientation(viewOrientation3D);
                (view as View3D).SaveOrientation();
                view.Name = viewName;
                m_viewNameList.Add(viewName);
            }
            else
            {
                view = new FilteredElementCollector(m_doc).OfClass(typeof(View))
                    .Cast<View>().First(e => e.Name == viewName);
            }

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view) ?? SpatialFieldManager.CreateSpatialFieldManager(view, 1);

            if (_schemaId != -1)
            {
                var results = sfm.GetRegisteredResults();

                if (!results.Contains(_schemaId)) _schemaId = -1;
            }

            if (_schemaId == -1)
            {
                AnalysisResultSchema resultSchema1 = new($"PaintedSolid{viewName}", "Description");

                var displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
                    m_doc,
                    $"Real_Color_Surface{viewName}",
                    new AnalysisDisplayColoredSurfaceSettings(),
                    new AnalysisDisplayColorSettings(),
                    new AnalysisDisplayLegendSettings());

                resultSchema1.AnalysisDisplayStyleId = displayStyle.Id;

                _schemaId = sfm.RegisterResult(resultSchema1);
            }

            var faces = s.Faces;
            var trf = Transform.Identity;

            foreach (Face face in faces)
            {
                var idx = sfm.AddSpatialFieldPrimitive(face, trf);

                ComputeValueAtPointForFace(face, out var uvPts, out var valList, 1);

                FieldDomainPointsByUV pnts = new(uvPts);

                FieldValues vals = new(valList);

                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, _schemaId);
            }
        }

        private static void ComputeValueAtPointForFace(Face face, out IList<UV> uvPts, out IList<ValueAtPoint> valList,
            int measurementNo)
        {
            List<double> doubleList = new();
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
