// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;

using System;


namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class XYZExtensions
    {
        public static bool IsParallelTo(this XYZ first, XYZ second)
        {
            return first.CrossProduct(second).IsZeroLength();
        }

        public static bool IsPerpendicularTo(this XYZ first, XYZ second)
        {
            return Math.Abs(first.DotProduct(second)) < 1e-6;
        }

        public static bool IsCodirectionalTo(this XYZ first, XYZ second)
        {
            var dotProduct = first.Normalize().DotProduct(second.Normalize());

            return Math.Abs(dotProduct - 1.0) < 1e-6;
        }       
    }
}