// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Point = System.Drawing.Point;

namespace RevitMultiSample.ShaftHolePuncher.CS
{
    /// <summary>
    ///     ProfileNull class contains method to draw a coordinate system,
    ///     and contains method used to create Shaft Opening
    /// </summary>
    public class ProfileNull : Profile
    {
        private Level m_level1; //level 1 used to create Shaft Opening
        private Level m_level2; //level 2 used to create Shaft Opening

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        public ProfileNull(ExternalCommandData commandData)
            : base(commandData)
        {
            GetLevels();
            To2DMatrix = new Matrix4();
            MoveToCenterMatrix = new Matrix4();
        }

        /// <summary>
        ///     Scale property to get/set scale of shaft opening
        /// </summary>
        public float Scale { get; set; } = 1;

        /// <summary>
        ///     get level1 and level2 used to create shaft opening
        /// </summary>
        private void GetLevels()
        {
            var levelList = new FilteredElementCollector(CommandData.Application.ActiveUIDocument.Document)
                .OfClass(typeof(Level)).ToElements();
            var levels = from elem in levelList
                let level = elem as Level
                where level != null && "Level 1" == level.Name
                select level;
            if (levels.Count() > 0) m_level1 = levels.First();

            levels = from elem in levelList
                let level = elem as Level
                where level != null && "Level 2" == level.Name
                select level;
            if (levels.Count() > 0) m_level2 = levels.First();
        }

        /// <summary>
        ///     calculate the matrix for scale
        /// </summary>
        /// <param name="size">pictureBox size</param>
        /// <returns>maxtrix to scale the opening curve</returns>
        public override Matrix4 ComputeScaleMatrix(Size size)
        {
            ScaleMatrix = new Matrix4(Scale);
            return ScaleMatrix;
        }

        /// <summary>
        ///     calculate the matrix used to transform 3D to 2D.
        ///     because profile of shaft opening in Revit is 2d too,
        ///     so we need do nothing but new a matrix
        /// </summary>
        /// <returns>maxtrix is use to transform 3d points to 2d</returns>
        public override Matrix4 Compute3DTo2DMatrix()
        {
            TransformMatrix = new Matrix4();
            return TransformMatrix;
        }

        /// <summary>
        ///     draw the coordinate system
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        /// <param name="matrix4">
        ///     Matrix used to transform 3d to 2d
        ///     and make picture in right scale
        /// </param>
        public override void Draw2D(Graphics graphics, Pen pen, Matrix4 matrix4)
        {
            graphics.Transform = new Matrix(
                1, 0, 0, 1, 0, 0);
            //draw X axis
            graphics.DrawLine(pen, new Point(20, 280), new Point(400, 280));
            graphics.DrawPie(pen, 400, 265, 30, 30, 165, 30);
            //draw Y axis
            graphics.DrawLine(pen, new Point(20, 280), new Point(20, 50));
            graphics.DrawPie(pen, 5, 20, 30, 30, 75, 30);
            //draw scale
            graphics.DrawLine(pen, new Point(120, 275), new Point(120, 285));
            graphics.DrawLine(pen, new Point(220, 275), new Point(220, 285));
            graphics.DrawLine(pen, new Point(320, 275), new Point(320, 285));
            graphics.DrawLine(pen, new Point(15, 80), new Point(25, 80));
            graphics.DrawLine(pen, new Point(15, 180), new Point(25, 180));
            //dimension
            var font = new Font("Verdana", 10, FontStyle.Regular);
            graphics.DrawString("100'", font, Brushes.Blue, new PointF(122, 266));
            graphics.DrawString("200'", font, Brushes.Blue, new PointF(222, 266));
            graphics.DrawString("300'", font, Brushes.Blue, new PointF(322, 266));
            graphics.DrawString("100'", font, Brushes.Blue, new PointF(22, 181));
            graphics.DrawString("200'", font, Brushes.Blue, new PointF(22, 81));
            graphics.DrawString("(0,0)", font, Brushes.Blue, new PointF(10, 280));
        }

        /// <summary>
        ///     move the points to the center and scale as user selected.
        ///     profile of shaft opening in Revit is 2d too, so don't need transform points to 2d
        /// </summary>
        /// <param name="ps">contain the points to be transformed</param>
        /// <returns>Vector list contains points have been transformed</returns>
        public override List<Vector4> Transform2DTo3D(Point[] ps)
        {
            var result = new List<Vector4>();
            foreach (var point in ps)
            {
                //because our coordinate system is different with window UI
                //so we should change what we got from UI coordinate
                var v = new Vector4(point.X - 20, -(point.Y - 280), 0);
                v = ScaleMatrix.Transform(v);
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        ///     Create Shaft Opening
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

            return DocCreator.NewOpening(m_level1, m_level2, curves);
        }
    }
}
