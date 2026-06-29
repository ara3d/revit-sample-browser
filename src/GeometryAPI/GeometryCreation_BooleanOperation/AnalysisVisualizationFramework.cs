// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    public class AnalysisVisualizationFramework
    {
        /// <summary>
        ///     The singleton instance of AnalysisVisualizationFramework
        /// </summary>
        private static AnalysisVisualizationFramework _instance;

        /// <summary>
        ///     The ID of schema which SpatialFieldManager register
        /// </summary>
        private static int _schemaId = -1;

        /// <summary>
        ///     revit document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     The created view list
        /// </summary>
        private readonly List<string> m_viewNameList = new List<string>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc">Revit document</param>
        private AnalysisVisualizationFramework(Document doc)
        {
            m_doc = doc;
        }

        /// <summary>
        ///     Get the singleton instance of AnalysisVisualizationFramework
        /// </summary>
        /// <param name="doc">Revit document</param>
        /// <returns>The singleton instance of AnalysisVisualizationFramework</returns>
        public static AnalysisVisualizationFramework GetInstance(Document doc)
        {
            return _instance ?? (_instance = new AnalysisVisualizationFramework(doc));
        }

        /// <summary>
        ///     Paint a solid in a new named view
        /// </summary>
        /// <param name="s">solid</param>
        /// <param name="viewName">Given the name of view</param>
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

                //view = m_doc.Create.NewView3D(new XYZ(1, 1, 1));
                view = View3D.CreateIsometric(m_doc, view3DId);
                var viewOrientation3D = new ViewOrientation3D(new XYZ(1, -1, -1), new XYZ(1, 1, 1), new XYZ(1, 1, -2));
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
                var resultSchema1 = new AnalysisResultSchema($"PaintedSolid{viewName}", "Description");

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

                var pnts = new FieldDomainPointsByUV(uvPts);

                var vals = new FieldValues(valList);

                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, _schemaId);
            }
        }

        /// <summary>
        ///     Compute the value of face on specific point
        /// </summary>
        /// <param name="face"></param>
        /// <param name="uvPts"></param>
        /// <param name="valList"></param>
        /// <param name="measurementNo"></param>
        private static void ComputeValueAtPointForFace(Face face, out IList<UV> uvPts, out IList<ValueAtPoint> valList,
            int measurementNo)
        {
            var doubleList = new List<double>();
            uvPts = new List<UV>();
            valList = new List<ValueAtPoint>();
            var bb = face.GetBoundingBox();
            for (var u = bb.Min.U; u < bb.Max.U + 0.0000001; u += (bb.Max.U - bb.Min.U) / 1)
            for (var v = bb.Min.V; v < bb.Max.V + 0.0000001; v += (bb.Max.V - bb.Min.V) / 1)
            {
                var uvPnt = new UV(u, v);
                uvPts.Add(uvPnt);
                var faceXyz = face.Evaluate(uvPnt);
                // Specify three values for each point
                for (var ii = 1; ii <= measurementNo; ii++)
                    doubleList.Add(faceXyz.DistanceTo(XYZ.Zero) * ii);
                valList.Add(new ValueAtPoint(doubleList));
                doubleList.Clear();
            }
        }
    }
}
