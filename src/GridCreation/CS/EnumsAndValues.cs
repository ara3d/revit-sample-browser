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

using System;
using System.Collections.Generic;
using System.Text;

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    /// An enumerate type listing the ways to create grids.
    /// </summary>
    public enum CreateMode
    {
        /// <summary>
        /// Create grids with selected lines/arcs
        /// </summary>
        Select,
        /// <summary>
        /// Create orthogonal grids
        /// </summary>
        Orthogonal,
        /// <summary>
        /// Create radial and arc grids
        /// </summary>
        RadialAndArc
    }

    /// <summary>
    /// An enumerate type listing bubble locations of grids.
    /// </summary>
    public enum BubbleLocation
    {
        /// <summary>
        /// Place bubble at the start point
        /// </summary>
        StartPoint,
        /// <summary>
        /// Place bubble at the end point
        /// </summary>
        EndPoint
    }

    /// <summary>
    /// Class contains common const values
    /// </summary>
    static class Values
    {
        public const double PI = 3.1415926535897900;
        // ratio from degree to radian
        public const double DEGTORAD = PI / 180;
    }
}
