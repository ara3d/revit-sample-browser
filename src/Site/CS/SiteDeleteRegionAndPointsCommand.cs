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
    ///     A command that deletes a subregion and all topography surface points it contains.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class SiteDeleteRegionAndPointsCommand : IExternalCommand
    {
        /// <summary>
        ///     Implementation of the command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            DeleteSubregionAndPoints(commandData.Application.ActiveUIDocument);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Deletes a subregion and all topography surface points it contains.
        /// </summary>
        /// <param name="uiDoc">The document.</param>
        public void DeleteSubregionAndPoints(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            // Select subregion
            var subregion = SiteUiUtils.PickSubregion(uiDoc);
            var toposurface = SiteEditingUtils.GetTopographySurfaceHost(subregion);
            var points = SiteEditingUtils.GetNonBoundaryPoints(subregion);

            // All changes are added to one transaction group - will create one undo item
            using (var deleteGroup = new TransactionGroup(doc, "Delete region"))
            {
                deleteGroup.Start();

                // Edit scope to delete points- if there are points in the region
                if (points.Count > 0)
                    using (var editScope = new TopographyEditScope(doc, "Edit TS"))
                    {
                        editScope.Start(toposurface.Id);

                        // Transaction for point deletion
                        using (var t = new Transaction(doc, "Delete points"))
                        {
                            t.Start();
                            toposurface.DeletePoints(points);
                            t.Commit();
                        }

                        editScope.Commit(new TopographyEditFailuresPreprocessor());
                    }

                // Transaction to delete subregion
                using (var t2 = new Transaction(doc, "Delete subregion"))
                {
                    t2.Start();
                    doc.Delete(subregion.Id);
                    t2.Commit();
                }

                deleteGroup.Assimilate();
            }
        }
    }
}
