// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.MultiplanarRebar.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            List<CorbelFrame> corbelsToReinforce = [];

            ElementSet elems = new();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                elems.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            }

            foreach (Element elem in elems)
            {
                if (elem is FamilyInstance corbel && IsCorbel(corbel))
                    try
                    {
                        // CorbelFrame.Parse throws when the corbel is not sloped.
                        var frame = CorbelFrame.Parse(corbel);
                        corbelsToReinforce.Add(frame);
                    }
                    catch (Exception ex)
                    {
                        message += ex.ToString();
                    }
            }

            if (corbelsToReinforce.Count == 0)
            {
                if (string.IsNullOrEmpty(message))
                    message += "Please select sloped corbels.";

                return Result.Cancelled;
            }

            var revitDoc = commandData.Application.ActiveUIDocument.Document;
            CorbelReinforcementOptions reinforcementOptions = new(revitDoc);
            using (CorbelReinforcementOptionsForm reinforcementOptionsForm =
                   new(reinforcementOptions))
            {
                if (reinforcementOptionsForm.ShowDialog() == DialogResult.Cancel)
                    return Result.Cancelled;
            }

            Transaction reinforceTransaction = new(revitDoc);
            try
            {
                reinforceTransaction.Start("Reinforce Corbels");

                foreach (var corbel in corbelsToReinforce)
                {
                    corbel.Reinforce(reinforcementOptions);
                }

                reinforceTransaction.Commit();
            }
            catch (Exception ex)
            {
                reinforceTransaction.RollBack();
                message += ex.ToString();

                return Result.Failed;
            }

            return Result.Succeeded;
        }

        // Structural Connection families with concrete/precast material type are treated as corbels.
        private bool IsCorbel(FamilyInstance corbel)
        {
            return corbel.Category.BuiltInCategory == BuiltInCategory.OST_StructConnections &&
                   (corbel.StructuralMaterialType == StructuralMaterialType.Concrete ||
                    corbel.StructuralMaterialType == StructuralMaterialType.PrecastConcrete);
        }
    }
}
