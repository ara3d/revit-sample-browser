// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS
{
    /// <summary>
    ///     This class implements method Execute as an external command for Revit.
    ///     This external command updates the ExternallyTaggedBRep in the DirectShape created by the CreateBRep command.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class UpdateBRep : IExternalCommand
    {
        /// <summary>
        ///     Removes the ExternallyTaggedBRep created by the CreateBRep command from the DirectShape.
        ///     After that, creates the new ExternallyTaggedBRep with other dimensions and adds it to the DirectShape.
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user canceled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dbDocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // If the UpdateBRep external command is called before the CreateBRep command
                // or the created DirectShape is manually removed or is not valid now,
                // we should call CreateBRep command at first.
                if (null == CreateBRep.CreatedDirectShape ||
                    !CreateBRep.CreatedDirectShape.IsValidObject ||
                    !CreateBRep.CreatedDirectShape.Document.Equals(dbDocument))
                    if (Result.Succeeded != HelperMethods.ExecuteCreateBRepCommand(dbDocument))
                        return Result.Failed;

                using (var transaction = new Transaction(dbDocument, "UpdateExternallyTaggedBRep"))
                {
                    transaction.Start();

                    // Create BRep with other dimensions than CreateBRep command creates and update the geometry in the DirectShape.
                    var resizedTaggedBRep = HelperMethods.CreateExternallyTaggedPodium(120.0, 20.0, 60.0);
                    if (null == resizedTaggedBRep)
                        return Result.Failed;

                    // Remove the old ExternallyTaggedBRep from the DirectShape.
                    CreateBRep.CreatedDirectShape.RemoveExternallyTaggedGeometry(Podium.ExternalId);

                    // Check that the old ExternallyTaggedBRep is removed from the DirectShape.
                    if (CreateBRep.CreatedDirectShape.HasExternalGeometry(Podium.ExternalId))
                        return Result.Failed;

                    // Add the new resized ExternallyTaggedBRep to the DirectShape.
                    CreateBRep.CreatedDirectShape.AddExternallyTaggedGeometry(resizedTaggedBRep);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
