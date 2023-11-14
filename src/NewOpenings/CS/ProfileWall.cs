//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


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