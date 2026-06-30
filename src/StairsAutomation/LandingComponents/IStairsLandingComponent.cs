// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents
{
    public interface IStairsLandingComponent
    {
        CurveLoop GetLandingBoundary();

        double GetLandingBaseElevation();

        StairsLanding CreateLanding(Document document, ElementId stairsElementId);
    }
}
