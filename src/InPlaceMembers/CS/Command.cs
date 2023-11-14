// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.InPlaceMembers.CS
{
    /// <summary>
    ///     This command shows how to get In-place Family instance properties and
    ///     paint it AnalyticalModel profile on a PictureBox.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private static ExternalCommandData m_commandData;

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            m_commandData = commandData;
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");

            FamilyInstance inPlace = null;

            AnalyticalElement model = null;

            try
            {
                transaction.Start();
                if (!PrepareData(ref inPlace, ref model))
                {
                    message = "You should select only one in place member which have analytical model.";
                    return Result.Failed;
                }

                var graphicsData = GraphicsDataFactory.CreateGraphicsData(model);
                var instanceProperties = new Properties(inPlace);
                var form = new InPlaceMembersForm(instanceProperties, graphicsData);
                return form.ShowDialog() == DialogResult.Abort ? Result.Failed : Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            finally
            {
                transaction.Commit();
            }
        }

        /// <summary>
        ///     Search for the In-Place family instance's properties data to be listed
        ///     and graphics data to be drawn.
        /// </summary>
        /// <param name="inPlaceMember">properties data to be listed</param>
        /// <param name="model">graphics data to be draw</param>
        /// <returns>Returns true if retrieved this data</returns>
        private bool PrepareData(ref FamilyInstance inPlaceMember, ref AnalyticalElement model)
        {
            var selected = new ElementSet();
            foreach (var elementId in m_commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                selected.Insert(m_commandData.Application.ActiveUIDocument.Document.GetElement(elementId));

            if (selected.Size != 1) return false;

            foreach (var o in selected)
            {
                inPlaceMember = o as FamilyInstance;
                if (null == inPlaceMember) return false;
            }

            var document = inPlaceMember.Document;
            var relManager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            if (relManager != null)
            {
                var associatedElementId = relManager.GetAssociatedElementId(inPlaceMember.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalElement element)
                        model = element;
                }
            }

            return null != model;
        }
    }
}
