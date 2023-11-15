// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateAnalyticalPanel : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                //create analytical panel
                var analyticalPanel = CreateAmPanel(document);
                if (analyticalPanel != null)
                    //create analytical opening on the panel we've just created
                    CreateAmOpening(document, analyticalPanel.Id);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Creates an Analytiocal Panel
        /// </summary>
        /// <param name="revitDoc">Revit documenr</param>
        /// <returns></returns>
        public static AnalyticalPanel CreateAmPanel(Document revitDoc)
        {
            AnalyticalPanel analyticalPanel = null;
            using (var transaction = new Transaction(revitDoc, "Create Analytical Panel"))
            {
                transaction.Start();

                //create curveloop which will be assigned to the analytical panel
                var profileloop = new CurveLoop();
                profileloop.Append(Line.CreateBound(
                    new XYZ(0, 0, 0), new XYZ(5, 0, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(5, 0, 0), new XYZ(5, 5, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(5, 5, 0), new XYZ(0, 5, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(0, 5, 0), new XYZ(0, 0, 0)));

                //create the AnalyticalPanel
                analyticalPanel = AnalyticalPanel.Create(revitDoc, profileloop);

                analyticalPanel.StructuralRole = AnalyticalStructuralRole.StructuralRoleFloor;
                analyticalPanel.AnalyzeAs = AnalyzeAs.SlabOneWay;

                transaction.Commit();
            }

            return analyticalPanel;
        }

        /// <summary>
        ///     creates an AnalyticalOpening element which will be placed on the AnalyticalPanel
        ///     with id = panelId
        /// </summary>
        public static AnalyticalOpening CreateAmOpening(Document revitDoc, ElementId panelId)
        {
            if (panelId == ElementId.InvalidElementId)
                return null;

            AnalyticalOpening opening = null;

            using (var transaction = new Transaction(revitDoc, "Create Analytical Opening"))
            {
                transaction.Start();

                //create the curveLoop for the AnalyticalOpening element
                var profileloop = new CurveLoop();
                profileloop.Append(Line.CreateBound(
                    new XYZ(1, 1, 0), new XYZ(2, 1, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(2, 1, 0), new XYZ(2, 2, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(2, 2, 0), new XYZ(-1, 2, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(-1, 2, 0), new XYZ(1, 1, 0)));

                if (AnalyticalOpening.IsCurveLoopValidForAnalyticalOpening(profileloop, revitDoc, panelId))
                    //create the AnalyticalOpening
                    opening = AnalyticalOpening.Create(revitDoc, profileloop, panelId);

                transaction.Commit();
            }

            return opening;
        }
    }
}
