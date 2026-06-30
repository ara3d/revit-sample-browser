// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.BoundaryConditions.CS
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
                ElementSet elementSet = new();
                foreach (var elementId in doc.Selection.GetElementIds())
                {
                    elementSet.Insert(doc.Document.GetElement(elementId));
                }

                if (1 != elementSet.Size)
                {
                    message = "Please select one structural element which is listed as follows: \r\n" +
                              "Columns/braces/Beams/Walls/Wall Foundations/Slabs/Foundation Slabs";
                    return Result.Cancelled;
                }

                Transaction tran = new(doc.Document, "BoundaryConditions");
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
                    BoundaryConditionsData dataBuffer = new(element);

                    // show UI
                    using BoundaryConditionsForm displayForm = new(dataBuffer);
                    var result = displayForm.ShowDialog();
                    switch (result)
                    {
                        case DialogResult.OK:
                            tran.Commit();
                            return Result.Succeeded;
                        case DialogResult.Retry:
                            message = "failed to create BoundaryConditions.";
                            tran.RollBack();
                            return Result.Failed;
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
                    if (associatedElement is not null and AnalyticalElement analyticalElement)
                        elemAnalytical = analyticalElement;
                }
            }

            return null != elemAnalytical && element switch
            {
                FamilyInstance familyInstance when StructuralType.Footing ==
                                                                        familyInstance.StructuralType => false,// if selected a isolated foundation not create BC
                FamilyInstance _ or Wall _ or Floor _ or WallFoundation _ => true,
                _ => false,
            };
        }
    }
}
