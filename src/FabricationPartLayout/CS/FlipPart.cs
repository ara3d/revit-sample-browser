// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FlipPart : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part to flip.");

                if (!(doc.GetElement(refObj) is FabricationPart part))
                {
                    message = "The selected element is not a fabrication part.";
                    return Result.Failed;
                }

                if (part.CanFlipPart() == false)
                {
                    message = "The selected fabrication part cannot be flipped";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "flip part"))
                {
                    trans.Start();

                    part.Flip();

                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
