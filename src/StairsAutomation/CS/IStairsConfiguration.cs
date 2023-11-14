// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;

namespace RevitMultiSample.StairsAutomation.CS
{
    /// <summary>
    ///     This interface represents a configuration of stairs runs and landings to be created.
    /// </summary>
    public interface IStairsConfiguration
    {
        /// <summary>
        ///     Returns the number of stairs runs in the stairs assembly.
        /// </summary>
        /// <returns>The number of runs.</returns>
        int GetNumberOfRuns();

        /// <summary>
        ///     Creates the stairs run with the given index.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="stairsElementId">The id of the stairs element.</param>
        /// <param name="runIndex">The run index.</param>
        void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex);

        /// <summary>
        ///     Returns the number of landings in the stairs assembly.
        /// </summary>
        /// <returns>The number of landings.</returns>
        int GetNumberOfLandings();

        /// <summary>
        ///     Creates the stairs landing with the given index.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="stairsElementId">The id of the stairs element.</param>
        /// <param name="landingIndex">The landing index.</param>
        void CreateLanding(Document document, ElementId stairsElementId, int landingIndex);

        /// <summary>
        ///     Assigns the width of the stairs runs.
        /// </summary>
        /// <param name="width">The width.</param>
        void SetRunWidth(double width);
    }
}
