// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     ProfileWall class contain the information about profile of wall,
    ///     and contain method to create Opening on wall
    /// </summary>
    public class ProfileWall : Profile
    {
        private readonly Wall m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="wall">Selected wall</param>
        /// <param name="commandData">ExternalCommandData</param>
        public ProfileWall(Wall wall, ExternalCommandData commandData)
            : base(wall, commandData)
        {
            m_data = wall;
        }

        /// <summary>
        ///     Create opening on wall
        /// </summary>
        /// <param name="points">Points use to create Opening</param>
        /// <param name="type">Tool type</param>
        public override void DrawOpening(List<Vector4> points, ToolType type)
        {
            //get the rectangle two points
            var p1 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            var p2 = new XYZ(points[2].X, points[2].Y, points[2].Z);

            //draw opening on wall
            m_docCreator.NewOpening(m_data, p1, p2);
        }
    }
}
