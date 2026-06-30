// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    public class DoorGeometry
    {
        // boundingBox of the geometry. 
        private readonly BoundingBoxXYZ m_bbox;

        // curves can represent the wireFrame of the door's geometry.
        private readonly List<List<XYZ>> m_curve3Ds = new List<List<XYZ>>();

        // User preferences for parsing of geometry.
        private readonly Options m_options;

        public DoorGeometry(Element door)
        {
            m_options = new Options
            {
                View = GetPlanform2DView(door),
                ComputeReferences = false
            };
            var geoEle = door.get_Geometry(m_options);
            AddGeometryElement(geoEle);

            m_bbox = door.get_BoundingBox(m_options.View);
        }

        public RectangleF Bbox2D =>
            new RectangleF((float)m_bbox.Min.X, (float)m_bbox.Min.Y,
                (float)(m_bbox.Max.X - m_bbox.Min.X), (float)(m_bbox.Max.Y - m_bbox.Min.Y));

        public void DrawGraphics(Graphics graphics, Pen drawPen)
        {
            foreach (var points in m_curve3Ds)
            {
                for (var j = 0; j < points.Count - 1; j++)
                {
                    // ignore xyz.Z value, drawn as top view.
                    var startPoint = new PointF((float)points[j].X, (float)points[j].Y);
                    var endPoint = new PointF((float)points[j + 1].X, (float)points[j + 1].Y);
                    graphics.DrawLine(drawPen, startPoint, endPoint);
                }
            }
        }

        private static ViewPlan GetPlanform2DView(Element door)
        {
            var viewPlans = from elem in
                    new FilteredElementCollector(door.Document).OfClass(typeof(ViewPlan)).ToElements()
                let viewPlan = elem as ViewPlan
                where viewPlan != null && !viewPlan.IsTemplate && viewPlan.GenLevel.Id == door.LevelId
                select viewPlan;
            return viewPlans.Count() > 0 ? viewPlans.First() : null;
        }

        private void AddGeometryElement(GeometryElement geoEle)
        {
            // get all geometric primitives contained in the Geometry Element
            //GeometryObjectArray geoObjArray = geoEle.Objects;
            var objects = geoEle.GetEnumerator();

            // iterate each Geometry Object and generate data accordingly.
            //foreach (GeometryObject geoObj in geoObjArray)
            while (objects.MoveNext())
            {
                var geoObj = objects.Current;

                switch (geoObj)
                {
                    case Curve _:
                        AddCurve(geoObj);
                        break;
                    case Edge _:
                        AddEdge(geoObj);
                        break;
                    case GeometryElement _:
                        AddElement(geoObj);
                        break;
                    case Face _:
                        AddFace(geoObj);
                        break;
                    case GeometryInstance _:
                        AddInstance(geoObj);
                        break;
                    case Mesh _:
                        AddMesh(geoObj);
                        break;
                    case Profile _:
                        AddProfile(geoObj);
                        break;
                    case Solid _:
                        AddSolid(geoObj);
                        break;
                }
            }
        }

        private void AddCurve(GeometryObject obj)
        {
            var curve = obj as Curve;

            if (!curve.IsBound) return;

            // get a polyline approximation to the curve.
            var points = curve.Tessellate() as List<XYZ>;

            m_curve3Ds.Add(points);
        }

        private void AddEdge(GeometryObject obj)
        {
            var edge = obj as Edge;

            // get a polyline approximation to the edge.
            var points = edge.Tessellate() as List<XYZ>;

            m_curve3Ds.Add(points);
        }

        private void AddElement(GeometryObject obj)
        {
            var geoEle = obj as GeometryElement;
            AddGeometryElement(geoEle);
        }

        private void AddFace(GeometryObject obj)
        {
            var face = obj as Face;

            // get a triangular mesh approximation to the face.
            var mesh = face.Triangulate();
            if (null != mesh) AddMesh(mesh);
        }

        private void AddInstance(GeometryObject obj)
        {
            var instance = obj as GeometryInstance;
            var geoElement = instance.SymbolGeometry;

            AddGeometryElement(geoElement);
        }

        private void AddMesh(GeometryObject obj)
        {
            var mesh = obj as Mesh;
            var points = new List<XYZ>();

            // get all triangles of the mesh.
            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var trigangle = mesh.get_Triangle(i);

                for (var j = 0; j < 3; j++)
                {
                    // A vertex of the triangle.
                    var point = trigangle.get_Vertex(j);

                    points.Add(point);
                }

                var iniPoint = points[0];
                points.Add(iniPoint);

                m_curve3Ds.Add(points);
            }
        }

        private void AddProfile(GeometryObject obj)
        {
            var profile = obj as Profile;

            var curves = profile.Curves;

            foreach (Curve curve in curves)
            {
                AddCurve(curve);
            }
        }

        private void AddSolid(GeometryObject obj)
        {
            var solid = obj as Solid;

            var faces = solid.Faces;

            foreach (Face face in faces)
            {
                AddFace(face);
            }
        }
    }
}
