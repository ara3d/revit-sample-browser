// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public class GridCreationOptionData
    {
        public GridCreationOptionData(bool hasSelectedLinesOrArcs)
        {
            HasSelectedLinesOrArcs = hasSelectedLinesOrArcs;
        }
        // The way to create grids
        // If lines/arcs have been selected

        public CreateMode CreateGridsMode { get; set; }

        public bool HasSelectedLinesOrArcs { get; }
    }
}
