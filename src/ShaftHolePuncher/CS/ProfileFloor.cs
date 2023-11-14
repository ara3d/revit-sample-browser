// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ShaftHolePuncher.CS
{
    /// <summary>
    ///     ProfileFloor class contains the information about profile of floor,
    ///     and contains method used to create Opening on floor
    /// </summary>
    public class ProfileFloor : Profile
    {
        private readonly Floor m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="floor">floor to create Opening on</param>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        public ProfileFloor(Floor floor, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = floor;
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
            foreach (var edge in faces[0])
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                needPoints.Add(edgexyzs);
            }

            return needPoints;
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        /// <returns>matrix which can transform points to 2D</returns>
        public override Matrix4 GetTo2DMatrix()
        {
            View viewLevel2 = null;

            // get view which named "Level 2".
            // Skip view templates because they're behind-the-scene and invisible in project browser; also invalid for API.
            var views = from elem in
                    new FilteredElementCollector(CommandData.Application.ActiveUIDocument.Document)
                        .OfClass(typeof(ViewPlan)).ToElements()
                let view = elem as View
                where view != null && !view.IsTemplate && "Level 2" == view.Name
                select view;
            if (views.Count() > 0) viewLevel2 = views.First();

            var xAxis = new Vector4(viewLevel2.RightDirection);
            //Because Y axis in windows UI is downward, so we should Multiply(-1) here
            var yAxis = new Vector4(viewLevel2.UpDirection.Multiply(-1));
            var zAxis = new Vector4(viewLevel2.ViewDirection);

            var result = new Matrix4(xAxis, yAxis, zAxis);
            return result;
        }

        /// <summary>
        ///     Create Opening on floor
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
            p2 = new XYZ(points[points.Count - 1].X,
                points[points.Count - 1].Y, points[points.Count - 1].Z);
            curve = Line.CreateBound(p1, p2);
            curves.Append(curve);

            return DocCreator.NewOpening(m_data, curves, true);
        }
    }
}
