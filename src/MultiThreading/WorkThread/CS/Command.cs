// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Revit.SDK.Samples.WorkThread.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uidoc = commandData.Application.ActiveUIDocument;

                if (uidoc == null)
                    // we can continue only if there is a document open
                    return Result.Cancelled;

                // we ask the end-user to pick a face

                string sref;
                var result = PickWallFace(uidoc, out sref);

                switch (result)
                {
                    // Start the analysis for the picked wall surface
                    case Result.Succeeded:
                        Application.thisApp.RunAnalyzer(commandData.Application, sref);
                        break;
                    case Result.Failed:
                        message = "Did not picked a face on a Wall or FaceWall element!";
                        break;
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Prompting the user to pick a wall face
        /// </summary>
        /// <returns>
        ///     Returns 'Cancelled' if the end-user escapes from face picking.
        ///     Otherwise returns Succeeded or Failed depending on whether
        ///     a face on a wall or face-wall was picked as expected
        /// </returns>
        private Result PickWallFace(UIDocument uidoc, out string sref)
        {
            sref = null;
            Reference faceref = null;

            try
            {
                faceref = uidoc.Selection.PickObject(ObjectType.Face, "Pick a face on a wall or face-wall element.");
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            // we make sure we have the expected kind of face reference

            if (faceref != null)
            {
                var pickedelem = uidoc.Document.GetElement(faceref.ElementId);
                var asfacewall = pickedelem as FaceWall;
                var aswall = pickedelem as Wall;

                // in this particular example, we accepts faces on wall elements only

                if (aswall == null && asfacewall == null) return Result.Failed;
            }

            // we convert the reference object to a more stable string
            // representation that is more traceable  across transactions

            sref = faceref.ConvertToStableRepresentation(uidoc.Document);

            return Result.Succeeded;
        }
    }
}
