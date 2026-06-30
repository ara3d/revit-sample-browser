// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class IEnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }
            return !source.Any();
        }
    }
}