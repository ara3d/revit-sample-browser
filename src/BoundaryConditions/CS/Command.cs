// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.BoundaryConditions.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                //Retrieves the currently active project.
                var doc = commandData.Application.ActiveUIDocument;

                // must select a element first
                var elementSet = new ElementSet();
                foreach (var elementId in doc.Selection.GetElementIds())
                    elementSet.Insert(doc.Document.GetElement(elementId));
                if (1 != elementSet.Size)
                {
                    message = "Please select one structural element which is listed as follows: \r\n" +
                              "Columns/braces/Beams/Walls/Wall Foundations/Slabs/Foundation Slabs";
                    return Result.Cancelled;
                }


                var tran = new Transaction(doc.Document, "BoundaryConditions");
                tran.Start();

                // deal with the selected element
                foreach (Element element in elementSet)
                {
                    // the selected element must be a structural element
                    if (!IsExpectedElement(element))
                    {
                        message = "Please select one structural element which is listed as follows: \r\n" +
                                  "Columns/braces/Beams/Walls/Wall Foundations/ \r\n" +
                                  "Slabs/Foundation Slabs";
                        return Result.Cancelled;
                    }

                    // prepare the relative data
                    var dataBuffer = new BoundaryConditionsData(element);

                    // show UI
                    using (var displayForm = new BoundaryConditionsForm(dataBuffer))
                    {
                        var result = displayForm.ShowDialog();
                        if (DialogResult.OK == result)
                        {
                            tran.Commit();
                            return Result.Succeeded;
                        }

                        if (DialogResult.Retry == result)
                        {
                            message = "failed to create BoundaryConditions.";
                            tran.RollBack();
                            return Result.Failed;
                        }
                    }
                }

                tran.RollBack();
                // user cancel the operation
                return Result.Cancelled;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     the selected element must be a structural Column/brace/Beam/Wall/Wall Foundation/Slab/Foundation Slab.
        /// </summary>
        /// <returns></returns>
        private bool IsExpectedElement(Element element)
        {
            // judge the element's type. If it is any type of FamilyInstance, Wall, Floor or 
            // WallFoundation, then get judge if it has a AnalyticalModel.
            var assocManager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(element.Document);
            AnalyticalElement elemAnalytical = null;
            if (assocManager != null)
            {
                var associatedElementId = assocManager.GetAssociatedElementId(element.Id);
                if (associatedElementId != ElementId.InvalidElementId)
                {
                    var associatedElement = element.Document.GetElement(associatedElementId);
                    if (associatedElement != null && associatedElement is AnalyticalElement)
                        elemAnalytical = associatedElement as AnalyticalElement;
                }
            }

            if (null == elemAnalytical) return false;
            var familyInstance = element as FamilyInstance;
            if (null != familyInstance &&
                StructuralType.Footing ==
                familyInstance.StructuralType) return false; // if selected a isolated foundation not create BC

            if (element is FamilyInstance || element is Wall || element is Floor || element is WallFoundation)
                return true;

            return false;
        }
    }
}
