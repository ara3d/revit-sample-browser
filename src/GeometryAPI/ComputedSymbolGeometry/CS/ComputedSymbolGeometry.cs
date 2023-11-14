// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Revit.SDK.Samples.ComputedSymbolGeometry.CS
{
    internal class ComputedSymbolGeometry
    {
        /// <summary>
        ///     Options for geometry
        /// </summary>
        private readonly Options m_options;

        /// <summary>
        ///     Schema Id
        /// </summary>
        private int m_schemaId = -1;

        /// <summary>
        ///     Revit document
        /// </summary>
        private readonly Document RevitDoc;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc">Revit Document</param>
        public ComputedSymbolGeometry(Document doc)
        {
            RevitDoc = doc;
            m_options = new Options();
        }

        /// <summary>
        ///     Get geometry of family instances and show them in Revit views
        /// </summary>
        public void GetInstanceGeometry()
        {
            var instanceCollector = new FilteredElementCollector(RevitDoc);
            instanceCollector.OfClass(typeof(FamilyInstance));

            // create views by different names
            var View3DId = ElementId.InvalidElementId;
            var elems = new FilteredElementCollector(RevitDoc).OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (var e in elems)
            {
                if (e is ViewFamilyType v && v.ViewFamily == ViewFamily.ThreeDimensional)
                {
                    View3DId = e.Id;
                    break;
                }
            }

            //View instanceView = RevitDoc.Create.NewView3D(new XYZ(1, 1, -1));
            var instanceView = View3D.CreateIsometric(RevitDoc, View3DId);
            var instanceViewOrientation3D = new ViewOrientation3D(
                new XYZ(-30.8272352809007, -2.44391067967133, 18.1013736367246),
                new XYZ(0.577350269189626, 0.577350269189626, -0.577350269189626),
                new XYZ(0.408248290463863, 0.408248290463863, 0.816496580927726));
            instanceView.SetOrientation(instanceViewOrientation3D);
            instanceView.SaveOrientation();
            instanceView.Name = "InstanceGeometry";

            //View originalView = RevitDoc.Create.NewView3D(new XYZ(0, 1, -1));
            var originalView = View3D.CreateIsometric(RevitDoc, View3DId);
            var originalViewOrientation3D = new ViewOrientation3D(
                new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478),
                new XYZ(0, 0.707106781186547, -0.707106781186547), new XYZ(0, 0.707106781186548, 0.707106781186548));
            originalView.SetOrientation(originalViewOrientation3D);
            originalView.SaveOrientation();
            originalView.Name = "OriginalGeometry";

            //View transView = RevitDoc.Create.NewView3D(new XYZ(-1, 1, -1));
            var transView = View3D.CreateIsometric(RevitDoc, View3DId);
            //ViewOrientation3D transViewOrientation3D = new ViewOrientation3D(new XYZ(-7.22273804467383, -2.44391067967133, 18.1013736367246), new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626), new XYZ(-0.408248290463863, 0.408248290463863, 0.816496580927726));
            var transViewOrientation3D =
                new ViewOrientation3D(new XYZ(-19.0249866627872, -5.09536632799455, 20.7528292850478),
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

                // show family instance geometry
                //foreach (GeometryObject obj in instanceGeo.Objects)
                var Objects = instanceGeo.GetEnumerator();
                while (Objects.MoveNext())
                {
                    var obj = Objects.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, instanceView);
                    }
                }

                // show geometry that is original geometry
                //foreach (GeometryObject obj in computedGeo.Objects)
                var Objects1 = computedGeo.GetEnumerator();
                while (Objects1.MoveNext())
                {
                    var obj = Objects1.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, originalView);
                    }
                }

                // show geometry that was transformed
                //foreach (GeometryObject obj in transformGeo.Objects)
                var Objects2 = transformGeo.GetEnumerator();
                while (Objects2.MoveNext())
                {
                    var obj = Objects2.Current;

                    if (obj is Solid solid)
                    {
                        PaintSolid(solid, transView);
                    }
                }
            }

            // remove original instances to view point results.
            RevitDoc.Delete(instanceCollector.ToElementIds());
        }

        /// <summary>
        ///     Paint solid by AVF
        /// </summary>
        /// <param name="solid">Solid to be painted</param>
        /// <param name="view">The view that shows solid</param>
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

            // set up the display style
            if (m_schemaId == -1)
            {
                var resultSchema1 = new AnalysisResultSchema("PaintedSolid " + viewName, "Description");

                var displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(RevitDoc,
                    "Real_Color_Surface" + viewName, new AnalysisDisplayColoredSurfaceSettings(),
                    new AnalysisDisplayColorSettings(), new AnalysisDisplayLegendSettings());

                resultSchema1.AnalysisDisplayStyleId = displayStyle.Id;

                m_schemaId = sfm.RegisterResult(resultSchema1);
            }

            // get points of all faces in the solid
            var faces = solid.Faces;
            var trf = Transform.Identity;
            foreach (Face face in faces)
            {
                var idx = sfm.AddSpatialFieldPrimitive(face, trf);
                IList<UV> uvPts = null;
                IList<ValueAtPoint> valList = null;
                ComputeValueAtPointForFace(face, out uvPts, out valList, 1);

                var pnts = new FieldDomainPointsByUV(uvPts);
                var vals = new FieldValues(valList);
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, m_schemaId);
            }
        }

        /// <summary>
        ///     Compute values at point for face
        /// </summary>
        /// <param name="face">Give face</param>
        /// <param name="uvPts">UV points</param>
        /// <param name="valList">Values at point</param>
        /// <param name="measurementNo"></param>
        private void ComputeValueAtPointForFace(Face face, out IList<UV> uvPts,
            out IList<ValueAtPoint> valList, int measurementNo)
        {
            var doubleList = new List<double>();
            uvPts = new List<UV>();
            valList = new List<ValueAtPoint>();
            var bb = face.GetBoundingBox();
            for (var u = bb.Min.U; u < bb.Max.U + 0.0000001; u = u + (bb.Max.U - bb.Min.U) / 1)
            for (var v = bb.Min.V; v < bb.Max.V + 0.0000001; v = v + (bb.Max.V - bb.Min.V) / 1)
            {
                var uvPnt = new UV(u, v);
                uvPts.Add(uvPnt);
                var faceXYZ = face.Evaluate(uvPnt);
                // Specify three values for each point
                for (var ii = 1; ii <= measurementNo; ii++)
                    doubleList.Add(faceXYZ.DistanceTo(XYZ.Zero) * ii);
                valList.Add(new ValueAtPoint(doubleList));
                doubleList.Clear();
            }
        }
    }
}
