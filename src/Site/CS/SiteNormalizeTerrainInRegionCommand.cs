// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Site.CS
{
    /// <summary>
    ///     A command that normalizes all points in a region to the average elevation.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class SiteNormalizeTerrainInRegionCommand : IExternalCommand
    {
        /// <summary>
        ///     Implementation of the external command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            NormalizeSubregionAndPoints(commandData.Application.ActiveUIDocument);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Normalizes all points in the selected subregion to the average elevation of the host surface.
        /// </summary>
        /// <param name="uiDoc">The document.</param>
        private void NormalizeSubregionAndPoints(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            // Pick subregion
            var subregion = SiteUiUtils.PickSubregion(uiDoc);
            var toposurface = SiteEditingUtils.GetTopographySurfaceHost(subregion);
            var points = SiteEditingUtils.GetPointsFromSubregionExact(subregion);

            // Get elevation of all points on the toposurface
            var allPoints = toposurface.GetPoints();
            var elevation = SiteEditingUtils.GetAverageElevation(allPoints);

            // Edit scope for all changes
            using (var editScope = new TopographyEditScope(doc, "Edit TS"))
            {
                editScope.Start(toposurface.Id);

                using (var t = new Transaction(doc, "Normalize terrain"))
                {
                    t.Start();

                    // Change all points to same elevation
                    toposurface.ChangePointsElevation(points, elevation);
                    t.Commit();
                }

                editScope.Commit(new TopographyEditFailuresPreprocessor());
            }
        }
    }
}
