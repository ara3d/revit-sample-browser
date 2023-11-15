// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.UpdateExternallyTaggedBRep.CS
{
    /// <summary>
    ///     This class implements method Execute as an external command for Revit.
    ///     This external command creates the ExternallyTaggedBRep and adds it to the document.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateBRep : IExternalCommand
    {
        /// <summary>
        ///     Created DirectShape by the CreateBRep external command.
        /// </summary>
        public static DirectShape CreatedDirectShape { set; get; } = null;

        /// <summary>
        ///     Creates the ExternallyTaggedBRep (named Podium), creates the new DirectShape for the open Document
        ///     and adds the created Podium to this DirectShape. Also this external command checks the retrieving of
        ///     the ExternallyTaggedBRep from the DirectShape by its ExternalGeometryId and the retrieving of the face and edge
        ///     from the ExternallyTaggedBRep by their ExternalGeometryIds.
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
                if (Result.Succeeded != HelperMethods.ExecuteCreateBRepCommand(dbDocument))
                    return Result.Failed;
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
