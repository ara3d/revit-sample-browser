//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

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

using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Revit.SDK.Samples.Site.CS
{
    /// <summary>
    /// A command that normalizes all points in a region to the average elevation.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SiteNormalizeTerrainInRegionCommand : IExternalCommand
    {
        
        /// <summary>
        /// Implementation of the external command.
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
        /// Normalizes all points in the selected subregion to the average elevation of the host surface.
        /// </summary>
        /// <param name="uiDoc">The document.</param>
        private void NormalizeSubregionAndPoints(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            // Pick subregion
            var subregion = SiteUIUtils.PickSubregion(uiDoc);
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
