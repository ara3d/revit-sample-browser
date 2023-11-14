// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.MultiplanarRebar.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // A List to store the Corbels which are suitable to be reinforced.
            var corbelsToReinforce = new List<CorbelFrame>();

            // Filter out the Corbels which can be reinforced by this sample
            // from the selected elements.
            var elems = new ElementSet();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            foreach (Element elem in elems)
            {
                var corbel = elem as FamilyInstance;

                // Make sure it's a Corbel firstly.
                if (corbel != null && IsCorbel(corbel))
                    try
                    {
                        // If the Corbel is sloped, this should return a non-null object.
                        var frame = CorbelFrame.Parse(corbel);
                        corbelsToReinforce.Add(frame);
                    }
                    // If the Corbel is not sloped, it will throw exception.
                    catch (Exception ex)
                    {
                        // Collect the error message, in case there is no any suitable corbel to be reinforced,
                        // Let user know what's happened.
                        message += ex.ToString();
                    }
            }

            // Check to see if there is any Corbel to be reinforced.            
            if (corbelsToReinforce.Count == 0)
            {
                // If there is no suitable Corbel to be reinforced, prompt a message.
                if (string.IsNullOrEmpty(message))
                    message += "Please select sloped corbels.";

                // Return cancelled for invalid selection.
                return Result.Cancelled;
            }

            // Show a model dialog to get Rebar creation options.
            var revitDoc = commandData.Application.ActiveUIDocument.Document;
            var reinforcementOptions = new CorbelReinforcementOptions(revitDoc);
            using (var reinforcementOptionsForm =
                   new CorbelReinforcementOptionsForm(reinforcementOptions))
            {
                if (reinforcementOptionsForm.ShowDialog() == DialogResult.Cancel)
                    // Cancelled by user.
                    return Result.Cancelled;
            }


            // Encapsulate operation "Reinforce Corbels" into one transaction. 
            var reinforceTransaction = new Transaction(revitDoc);
            try
            {
                // Start the transaction.
                reinforceTransaction.Start("Reinforce Corbels");

                // Reinforce all the corbels in list.
                foreach (var corbel in corbelsToReinforce)
                    // Reinforce the sloped Corbel.
                    corbel.Reinforce(reinforcementOptions);

                // Submit the transaction
                reinforceTransaction.Commit();
            }
            catch (Exception ex)
            {
                // Rollback the transaction for any exception.
                reinforceTransaction.RollBack();
                message += ex.ToString();

                // Return failed for any exception.
                return Result.Failed;
            }

            // No any error, return succeeded.
            return Result.Succeeded;
        }

        /// <summary>
        ///     Test to see if the given family instance is a Corbel.
        /// </summary>
        /// <param name="corbel">Given Family instance</param>
        /// <returns>True if the given family instance is Corbel, otherwise, false.</returns>
        private bool IsCorbel(FamilyInstance corbel)
        {
            // Families of category "Structural Connection" support the Structural Material Type parameter. 
            // Structural Connection families of type Concrete or Precast Concrete are considered corbels. 
            // Corbels support the following features: 
            // �Hosting Rebar.
            // �Autojoining to columns and walls.
            // �Manual joining to other concrete elements.
            return corbel.Category.BuiltInCategory == BuiltInCategory.OST_StructConnections &&
                   (corbel.StructuralMaterialType == StructuralMaterialType.Concrete ||
                    corbel.StructuralMaterialType == StructuralMaterialType.PrecastConcrete);
        }
    }
}
