// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.Common.Structural
{
    public static class AnalyticalModelHelper
    {
        public static LoadUsage FindUsageByName(IEnumerable<LoadUsage> usages, string name)
        {
            return usages.FirstOrDefault(usage => usage.Name == name);
        }

        public static LoadCase FindLoadCaseByName(IEnumerable<LoadCase> loadCases, string name)
        {
            return loadCases.FirstOrDefault(loadCase => loadCase.Name == name);
        }
    }
}