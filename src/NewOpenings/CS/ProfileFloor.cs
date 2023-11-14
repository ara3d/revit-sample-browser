// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     ProfileFloor class contain the information about profile of floor,
    ///     and contain method to create Opening on floor
    /// </summary>
    public class ProfileFloor : Profile
    {
        private readonly Floor m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="floor">Selected floor</param>
        /// <param name="commandData">ExternalCommandData</param>
        public ProfileFloor(Floor floor, ExternalCommandData commandData)
            : base(floor, commandData)
        {
            m_data = floor;
        }

        /// <summary>
        ///     Create Opening on floor
        /// </summary>
        /// <param name="points">Points use to create Opening</param>
        /// <param name="type">Tool type</param>
        public override void DrawOpening(List<Vector4> points, ToolType type)
        {
            switch (type)
            {
                case ToolType.Line:
                case ToolType.Rectangle:
                    DrawPlineOpening(points);
                    break;

                case ToolType.Circle:
                    DrawCircleOpening(points);
                    break;

                case ToolType.Arc:
                    DrawArcOpening(points);
                    break;
            }
        }

        /// <summary>
        ///     Create Opening which make up of line on floor
        /// </summary>
        /// <param name="points">Points use to create Opening</param>
        private void DrawPlineOpening(List<Vector4> points)
        {
            XYZ p1, p2;
            Line curve;
            var curves = m_appCreator.NewCurveArray();
            for (var i = 0; i < points.Count - 1; i++)
            {
                p1 = new XYZ(points[i].X, points[i].Y, points[i].Z);
                p2 = new XYZ(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
                curve = Line.CreateBound(p1, p2);
                curves.Append(curve);
            }

            p1 = new XYZ(points[points.Count - 1].X,
                points[points.Count - 1].Y, points[points.Count - 1].Z);
            p2 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            curve = Line.CreateBound(p1, p2);
            curves.Append(curve);

            m_docCreator.NewOpening(m_data, curves, true);
        }

        /// <summary>
        ///     Create Opening which make up of Circle on floor
        /// </summary>
        /// <param name="points">Points use to create Opening</param>
        private void DrawCircleOpening(List<Vector4> points)
        {
            var curves = m_appCreator.NewCurveArray();
            var p1 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            var p2 = new XYZ(points[1].X, points[1].Y, points[1].Z);
            var p3 = new XYZ(points[2].X, points[2].Y, points[2].Z);
            var p4 = new XYZ(points[3].X, points[3].Y, points[3].Z);
            var arc = Arc.Create(p1, p3, p2);
            var arc2 = Arc.Create(p1, p3, p4);
            curves.Append(arc);
            curves.Append(arc2);
            m_docCreator.NewOpening(m_data, curves, true);
        }

        /// <summary>
        ///     Create Opening which make up of Arc on floor
        /// </summary>
        /// <param name="points">Points use to create Opening</param>
        private void DrawArcOpening(List<Vector4> points)
        {
            var curves = m_appCreator.NewCurveArray();
            var p1 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            var p2 = new XYZ(points[1].X, points[1].Y, points[1].Z);
            var p3 = new XYZ(points[2].X, points[2].Y, points[2].Z);
            var arc = Arc.Create(p1, p2, p3);
            curves.Append(arc);
            for (var i = 1; i < points.Count - 3; i += 2)
            {
                p1 = new XYZ(points[i].X, points[i].Y, points[i].Z);
                p2 = new XYZ(points[i + 2].X, points[i + 2].Y, points[i + 2].Z);
                p3 = new XYZ(points[i + 3].X, points[i + 3].Y, points[i + 3].Z);
                arc = Arc.Create(p1, p2, p3);
                curves.Append(arc);
            }

            m_docCreator.NewOpening(m_data, curves, true);
        }
    }
}
