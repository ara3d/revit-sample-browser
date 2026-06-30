// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    public interface IStairsConfiguration
    {
        int GetNumberOfRuns();

        void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex);

        int GetNumberOfLandings();

        void CreateLanding(Document document, ElementId stairsElementId, int landingIndex);

        void SetRunWidth(double width);
    }
}
