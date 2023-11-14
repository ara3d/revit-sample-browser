// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PathReinforcement.CS
{
    /// <summary>
    ///     This class stores the geometry information of path reinforcement.
    /// </summary>
    internal class Profile
    {
        /// <summary>
        ///     field used to store the bound of the curves of path reinforcement.
        ///     2d data.
        /// </summary>
        private readonly BoundingBoxUV m_box = new BoundingBoxUV();

        /// <summary>
        ///     field used to store external command data.
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     field used to store the geometry curves of path reinforcement.
        ///     3d data.
        /// </summary>
        private readonly List<List<XYZ>> m_curves = new List<List<XYZ>>();

        /// <summary>
        ///     store path 3D.
        /// </summary>
        private readonly List<List<XYZ>> m_path = new List<List<XYZ>>();

        /// <summary>
        ///     store path 2D.
        /// </summary>
        private readonly List<List<UV>> m_path2d = new List<List<UV>>();

        /// <summary>
        ///     field used to store path reinforcement.
        /// </summary>
        private readonly Autodesk.Revit.DB.Structure.PathReinforcement m_pathRein;

        /// <summary>
        ///     field used to store the geometry data of curves of path reinforcement.
        ///     2d data.
        /// </summary>
        private readonly List<List<UV>> m_point2d = new List<List<UV>>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="pathRein">selected path reinforcement element.</param>
        /// <param name="commandData">External command data</param>
        public Profile(Autodesk.Revit.DB.Structure.PathReinforcement pathRein, ExternalCommandData commandData)
        {
            m_pathRein = pathRein;
            m_commandData = commandData;
            Tessellate();
            ComputeBound();
            ComputePathTo2D();
        }

        /// <summary>
        ///     Draw the curves of path reinforcement.
        /// </summary>
        /// <param name="graphics">Gdi object, used to draw curves of path reinforcement.</param>
        /// <param name="size">Bound to limit the size of the whole picture</param>
        /// <param name="pen">Gdi object,determine the color of the line.</param>
        public void Draw(Graphics graphics, Size size, Pen pen)
        {
            var delta = m_box.Max - m_box.Min;
            var scaleX = size.Width / (float)delta.U;
            var scaleY = size.Width / (float)delta.V;
            var scale = scaleY > scaleX ? scaleX : scaleY;
            scale *= 0.90f;

            var contain = graphics.BeginContainer();
            {
                //set graphics coordinate system to picture center
                //and  flip the yAxis.
                graphics.Transform = new Matrix(1, 0, 0, -1, size.Width / 2, size.Height / 2);

                //construct a matrix to transform the origin point to Bound center.
                var center = (m_box.Min + m_box.Max) / 2;
                var toCenter = new Matrix(1, 0, 0, 1, -(float)center.U, -(float)center.V);

                var isDrawFinished = false;
                var point2d = m_point2d;
                var tmpPen = pen;

                while (!isDrawFinished)
                {
                    foreach (var arr in point2d)
                        for (var i = 0; i < arr.Count - 1; i++)
                        {
                            //get the two connection points to draw a line between them.
                            var uv1 = arr[i];
                            var uv2 = arr[i + 1];
                            var points = new[]
                            {
                                new PointF((float)uv1.U, (float)uv1.V),
                                new PointF((float)uv2.U, (float)uv2.V)
                            };

                            //transform points to bound center.
                            toCenter.TransformPoints(points);

                            //Zoom(Scale) the points to fit the picture box.
                            var pf1 = new PointF(points[0].X * scale, points[0].Y * scale);
                            var pf2 = new PointF(points[1].X * scale, points[1].Y * scale);

                            //draw a line between pf1 and pf2.
                            graphics.DrawLine(tmpPen, pf1, pf2);
                        }

                    if (point2d == m_path2d) isDrawFinished = true;
                    point2d = m_path2d;
                    tmpPen = Pens.Blue;
                }
            }
            graphics.EndContainer(contain);
        }

        /// <summary>
        ///     Transform 3d path to 2d path.
        /// </summary>
        /// <returns></returns>
        private void ComputePathTo2D()
        {
            var transform = GetActiveViewMatrix().Inverse();
            foreach (var arr in m_path)
            {
                var uvarr = new List<UV>();
                foreach (var xyz in arr)
                {
                    var tmpVector = transform.Transform(new Vector4(xyz));
                    var tmpUv = new UV(
                        tmpVector.X,
                        tmpVector.Y);
                    uvarr.Add(tmpUv);
                }

                m_path2d.Add(uvarr);
            }
        }

        /// <summary>
        ///     Compute the bound of the curves of path reinforcement.
        /// </summary>
        private void ComputeBound()
        {
            //make the bound
            var min = m_box.get_Bounds(0);
            var max = m_box.get_Bounds(1);

            var transform = GetActiveViewMatrix().Inverse();

            var isFirst = true;
            foreach (var arr in m_curves)
            {
                var uvarr = new List<UV>();
                foreach (var xyz in arr)
                {
                    var tmpVector = transform.Transform(new Vector4(xyz));
                    var tmpUv = new UV(
                        tmpVector.X,
                        tmpVector.Y);
                    uvarr.Add(tmpUv);

                    if (isFirst)
                    {
                        isFirst = false;
                        min = new UV(tmpUv.U, tmpUv.V);
                        max = new UV(tmpUv.U, tmpUv.V);
                    }

                    if (tmpUv.U < min.U)
                        min = new UV(tmpUv.U, min.V);
                    else if (tmpUv.U > max.U) max = new UV(tmpUv.U, max.V);
                    if (tmpUv.V < min.V)
                        min = new UV(min.U, tmpUv.V);
                    else if (tmpUv.V > max.V) max = new UV(max.U, tmpUv.V);
                }

                m_point2d.Add(uvarr);
            }

            m_box.Min = min;
            m_box.Max = max;
        }

        /// <summary>
        ///     Tessellate the curves of path reinforcement.
        /// </summary>
        private void Tessellate()
        {
            var option = new Options();
            option.DetailLevel = ViewDetailLevel.Fine;
            var geoElem = m_pathRein.get_Geometry(option);
            //GeometryObjectArray geoArray = geoElem.Objects;
            var Objects = geoElem.GetEnumerator();
            //foreach (GeometryObject geo in geoArray)
            while (Objects.MoveNext())
            {
                var geo = Objects.Current;

                if (geo is Curve curve)
                {
                    m_curves.Add(curve.Tessellate() as List<XYZ>);
                }
            }

            var curveIds = m_pathRein.GetCurveElementIds();

            foreach (var id in curveIds)
            {
                var modelCurve = m_commandData.Application.ActiveUIDocument.Document.GetElement(id) as ModelCurve;
                m_path.Add(modelCurve.GeometryCurve.Tessellate() as List<XYZ>);
            }
        }

        /// <summary>
        ///     Get view matrix from active view.
        /// </summary>
        /// <returns>view matrix</returns>
        private Matrix4 GetActiveViewMatrix()
        {
            var activeView = m_commandData.Application.ActiveUIDocument.Document.ActiveView;
            var vZAxis = activeView.ViewDirection;
            var vXAxis = activeView.RightDirection;
            var vYAxis = activeView.UpDirection;

            return new Matrix4(new Vector4(vXAxis), new Vector4(vYAxis), new Vector4(vZAxis));
        }
    }
}
