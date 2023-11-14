// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.GridCreation.CS
{
    /// <summary>
    ///     Data class which stores the information of the way to create grids
    /// </summary>
    public class GridCreationOptionData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="hasSelectedLinesOrArcs">Whether lines or arcs have been selected</param>
        public GridCreationOptionData(bool hasSelectedLinesOrArcs)
        {
            HasSelectedLinesOrArcs = hasSelectedLinesOrArcs;
        }
        // The way to create grids
        // If lines/arcs have been selected

        /// <summary>
        ///     Creating mode
        /// </summary>
        public CreateMode CreateGridsMode { get; set; }

        /// <summary>
        ///     State whether lines/arcs have been selected
        /// </summary>
        public bool HasSelectedLinesOrArcs { get; }
    }
}
