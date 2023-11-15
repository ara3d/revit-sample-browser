// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     An interface that represents a landing.
    /// </summary>
    public interface IStairsLandingComponent
    {
        /// <summary>
        ///     Obtains the curves that bound the landing.
        /// </summary>
        /// <returns>The boundary curves.</returns>
        CurveLoop GetLandingBoundary();

        /// <summary>
        ///     Obtains the elevation of the landing.
        /// </summary>
        /// <returns>The elevation.</returns>
        double GetLandingBaseElevation();

        /// <summary>
        ///     Creates the landing component represented by this configuration.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="stairsElementId">The stairs element id.</param>
        /// <returns>The new landing.</returns>
        StairsLanding CreateLanding(Document document, ElementId stairsElementId);
    }
}
