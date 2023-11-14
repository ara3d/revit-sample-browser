// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

namespace Revit.SDK.Samples.Site.CS
{
    /// <summary>
    ///     A command that moves a subregion and the points it contains to a new location on the host surface.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class SiteMoveRegionAndPointsCommand : IExternalCommand
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
            MoveSubregionAndPoints(commandData.Application.ActiveUIDocument);
            return Result.Succeeded;
        }


        /// <summary>
        ///     Moves a subregion and the associated topography to a new user-selected location.
        /// </summary>
        /// <param name="uiDoc">The document.</param>
        private void MoveSubregionAndPoints(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            // Pick subregion
            var subregion = SiteUIUtils.PickSubregion(uiDoc);
            var toposurface = SiteEditingUtils.GetTopographySurfaceHost(subregion);
            var points = SiteEditingUtils.GetPointsFromSubregionExact(subregion);
            var sourceLocation = SiteEditingUtils.GetCenterOf(subregion);

            // Pick target location
            var targetPoint = SiteUIUtils.PickPointNearToposurface(uiDoc, toposurface, "Pick point to move to");

            // Delta for the move
            var delta = targetPoint - sourceLocation;

            // All changes are added to one transaction group - will create one undo item
            using (var moveGroup = new TransactionGroup(doc, "Move subregion and points"))
            {
                moveGroup.Start();

                // Get elevation of region in current location
                var existingPointsInCurrentLocation = subregion.GetPoints();

                var existingElevation = SiteEditingUtils.GetAverageElevation(existingPointsInCurrentLocation);

                // Move subregion first - allows the command delete existing points and adjust elevation to surroundings
                using (var t2 = new Transaction(doc, "Move subregion"))
                {
                    t2.Start();
                    ElementTransformUtils.MoveElement(doc, subregion.Id, delta);
                    t2.Commit();
                }

                // The boundary points for the subregion cannot be deleted, since they are generated
                // to represent the subregion boundary rather than representing real points in the host.
                // Get non-boundary points only to be deleted.
                var existingPointsInNewLocation = SiteEditingUtils.GetNonBoundaryPoints(subregion);

                // Average elevation of all points in the subregion.
                var newElevation = SiteEditingUtils.GetAverageElevation(subregion.GetPoints());

                // Adjust delta for elevation based on calculated values
                delta = SiteEditingUtils.MoveXYZToElevation(delta, newElevation - existingElevation);

                // Edit scope for points changes
                using (var editScope = new TopographyEditScope(doc, "Edit TS"))
                {
                    editScope.Start(toposurface.Id);

                    using (var t = new Transaction(doc, "Move points"))
                    {
                        t.Start();
                        // Delete existing points from target region
                        if (existingPointsInNewLocation.Count > 0)
                            toposurface.DeletePoints(existingPointsInNewLocation);

                        // Move points from source region
                        toposurface.MovePoints(points, delta);
                        t.Commit();
                    }

                    editScope.Commit(new TopographyEditFailuresPreprocessor());
                }

                moveGroup.Assimilate();
            }
        }
    }
}
