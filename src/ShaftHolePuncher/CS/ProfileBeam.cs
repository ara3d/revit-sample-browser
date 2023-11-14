// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     ProfileBeam class contains the information about profile of beam,
    ///     and contains method used to create opening on a beam.
    /// </summary>
    public class ProfileBeam : Profile
    {
        //store the transform used to change points in beam coordinate system to Revit coordinate system
        private Transform m_beamTransform;

        private readonly FamilyInstance m_data; //beam

        //if m_haveOpening is true means beam has already had opening on it
        //then the points get from get_Geometry(Option) do not need to be transformed 
        //by the Transform get from Instance object anymore.
        private bool m_haveOpening;
        private bool m_isZaxis = true; //decide whether to create opening on Zaxis of beam or Yaixs of beam
        private Matrix4 m_matrixYaxis; //transform points to plane whose normal is Yaxis of beam
        private Matrix4 m_matrixZaxis; //transform points to plane whose normal is Zaxis of beam

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="beam">beam to create opening on</param>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        public ProfileBeam(FamilyInstance beam, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = beam;
            var faces = GetFaces(m_data);
            Points = GetNeedPoints(faces);
            To2DMatrix = GetTo2DMatrix();
            MoveToCenterMatrix = ToCenterMatrix();
        }

        /// <summary>
        ///     Get points of the first face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        /// <returns>points of first face</returns>
        public override List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            var needPoints = new List<List<XYZ>>();
            foreach (var face in faces)
            foreach (var edge in face)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                if (false == m_haveOpening)
                {
                    var transformedPoints = new List<XYZ>();
                    for (var j = 0; j < edgexyzs.Count; j++)
                    {
                        var xyz = edgexyzs[j];
                        var transformedXyz = m_beamTransform.OfPoint(xyz);
                        transformedPoints.Add(transformedXyz);
                    }

                    edgexyzs = transformedPoints;
                }

                needPoints.Add(edgexyzs);
            }

            return needPoints;
        }

        /// <summary>
        ///     Get the bound of a face
        /// </summary>
        /// <returns>points array stores the bound of the face</returns>
        public override PointF[] GetFaceBounds()
        {
            var matrix = To2DMatrix;
            var inverseMatrix = matrix.Inverse();
            float minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;

            //get the max and min point on the face
            foreach (var points in Points)
            {
                foreach (var point in points)
                {
                    var v = new Vector4(point);
                    var v1 = inverseMatrix.Transform(v);

                    if (bFirstPoint)
                    {
                        minX = maxX = v1.X;
                        minY = maxY = v1.Y;
                        bFirstPoint = false;
                    }
                    else
                    {
                        if (v1.X < minX)
                            minX = v1.X;
                        else if (v1.X > maxX) maxX = v1.X;

                        if (v1.Y < minY)
                            minY = v1.Y;
                        else if (v1.Y > maxY) maxY = v1.Y;
                    }
                }
            }

            //return an array with max and min value of face
            var resultPoints = new PointF[2]
            {
                new PointF(minX, minY), new PointF(maxX, maxY)
            };
            return resultPoints;
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        /// <returns>matrix which can transform points to 2D</returns>
        public override Matrix4 GetTo2DMatrix()
        {
            //get transform used to transform points to plane whose normal is Zaxis of beam
            var xAxis = new Vector4(m_data.HandOrientation);
            xAxis.Normalize();
            //Because Y axis in windows UI is downward, so we should Multiply(-1) here
            var yAxis = new Vector4(m_data.FacingOrientation.Multiply(-1));
            yAxis.Normalize();
            var zAxis = yAxis.CrossProduct(xAxis);
            zAxis.Normalize();

            var vOrigin = new Vector4(Points[0][0]);
            var result = new Matrix4(xAxis, yAxis, zAxis, vOrigin);
            m_matrixZaxis = result;

            //get transform used to transform points to plane whose normal is Yaxis of beam
            xAxis = new Vector4(m_data.HandOrientation);
            xAxis.Normalize();
            zAxis = new Vector4(m_data.FacingOrientation);
            zAxis.Normalize();
            yAxis = xAxis.CrossProduct(zAxis) * -1;
            yAxis.Normalize();
            result = new Matrix4(xAxis, yAxis, zAxis, vOrigin);
            m_matrixYaxis = result;
            return m_matrixZaxis;
        }

        /// <summary>
        ///     Get edges of element's profile
        /// </summary>
        /// <param name="elem">selected element</param>
        /// <returns>all the faces in the selected Element</returns>
        public override List<List<Edge>> GetFaces(Element elem)
        {
            var faceEdges = new List<List<Edge>>();
            var options = AppCreator.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            //make sure references to geometric objects are computed.
            options.ComputeReferences = true;
            var geoElem = elem.get_Geometry(options);
            //GeometryObjectArray gObjects = geoElem.Objects;
            var objects = geoElem.GetEnumerator();
            //get all the edges in the Geometry object
            //foreach (GeometryObject geo in gObjects)
            while (objects.MoveNext())
            {
                var geo = objects.Current;

                switch (geo)
                {
                    //if beam doesn't contain opening on it, then we can get edges from instance
                    //and the points we get should be transformed by instance.Tranceform
                    case GeometryInstance instance:
                    {
                        m_beamTransform = instance.Transform;
                        var elemGeo = instance.SymbolGeometry;
                        //GeometryObjectArray objectsGeo = elemGeo.Objects;
                        var objects1 = elemGeo.GetEnumerator();
                        //foreach (GeometryObject objGeo in objectsGeo)
                        while (objects1.MoveNext())
                        {
                            var objGeo = objects1.Current;

                            var solid = objGeo as Solid;
                            if (null != solid)
                            {
                                var faces = solid.Faces;
                                foreach (Face face in faces)
                                {
                                    var edgeArrarr = face.EdgeLoops;
                                    foreach (EdgeArray edgeArr in edgeArrarr)
                                    {
                                        var edgesList = new List<Edge>();
                                        foreach (Edge edge in edgeArr) edgesList.Add(edge);
                                        faceEdges.Add(edgesList);
                                    }
                                }
                            }
                        }

                        break;
                    }
                    //if beam contains opening on it, then we can get edges from solid
                    //and the points we get do not need transform anymore
                    case Solid solid:
                    {
                        m_haveOpening = true;
                        var faces = solid.Faces;
                        foreach (Face face in faces)
                        {
                            var edgeArrarr = face.EdgeLoops;
                            foreach (EdgeArray edgeArr in edgeArrarr)
                            {
                                var edgesList = new List<Edge>();
                                foreach (Edge edge in edgeArr) edgesList.Add(edge);
                                faceEdges.Add(edgesList);
                            }
                        }

                        break;
                    }
                }
            }

            return faceEdges;
        }

        /// <summary>
        ///     Create Opening on beam
        /// </summary>
        /// <param name="points">points used to create Opening</param>
        /// <returns>newly created Opening</returns>
        public override Opening CreateOpening(List<Vector4> points)
        {
            XYZ p1, p2;
            Line curve;
            var curves = AppCreator.NewCurveArray();
            for (var i = 0; i < points.Count - 1; i++)
            {
                p1 = new XYZ(points[i].X, points[i].Y, points[i].Z);
                p2 = new XYZ(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
                curve = Line.CreateBound(p1, p2);
                curves.Append(curve);
            }

            //close the curve
            p1 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            p2 = new XYZ(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);
            curve = Line.CreateBound(p1, p2);
            curves.Append(curve);

            return DocCreator.NewOpening(m_data, curves, false == m_isZaxis ? eRefFace.CenterY : eRefFace.CenterZ);
        }

        /// <summary>
        ///     Change transform matrix used to transform points to 2d.
        /// </summary>
        /// <param name="isZaxis">
        ///     transform points to which plane.
        ///     true means transform points to plane whose normal is Zaxis of beam.
        ///     false means transform points to plane whose normal is Yaxis of beam
        /// </param>
        public void ChangeTransformMatrix(bool isZaxis)
        {
            m_isZaxis = isZaxis;
            if (isZaxis)
                To2DMatrix = m_matrixZaxis;
            else
                To2DMatrix = m_matrixYaxis;
            //re-calculate matrix used to move points to center
            MoveToCenterMatrix = ToCenterMatrix();
        }
    }
}
