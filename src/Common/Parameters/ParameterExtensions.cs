// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Common.Parameters
{
    public enum ParameterOrigin { Shared, Project, BuiltIn }

    public static class ParameterExtensions
    {
        public static ParameterOrigin GetOrigin(this Parameter parameter)
        {
            var result = ParameterOrigin.BuiltIn;
            if (parameter.Id.Value() > -1)
            {
                result = ParameterOrigin.Project;
            }
            if (parameter.IsShared)
            {
                result = ParameterOrigin.Shared;
            }
            return result;
        }
    }
}