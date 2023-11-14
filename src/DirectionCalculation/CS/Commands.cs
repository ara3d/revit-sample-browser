// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.Samples.DirectionCalculation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindSouthFacingWallsWithoutProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        private static AddInId m_appId = new AddInId(new Guid("8B29D56B-7B9A-4c79-8A38-B1C13B921877"));

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
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
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            var trans = new Transaction(Document, "FindSouthFacingWallsWithoutProjectLocation");
            trans.Start();
            Execute(false);

            CloseFile();

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWallsWithProjectLocation : FindSouthFacingWalls, IExternalCommand
    {
        private static AddInId m_appId = new AddInId(new Guid("6CADE602-7F32-496c-AA37-CEE4B0EE6087"));

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
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
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            var trans = new Transaction(Document, "FindSouthFacingWallsWithProjectLocation");
            trans.Start();
            Execute(true);

            CloseFile();
            trans.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithoutProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        private static AddInId m_appId = new AddInId(new Guid("AB3588F5-1CD1-4693-9DF0-C0890C811B21"));

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
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
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;
            var trans = new Transaction(Document, "FindSouthFacingWindowsWithoutProjectLocation");
            trans.Start();
            Execute(false);

            CloseFile();

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FindSouthFacingWindowsWithProjectLocation : FindSouthFacingWindows, IExternalCommand
    {
        private static AddInId m_appId = new AddInId(new Guid("BFECDEA2-C384-4bcc-965E-EA302BA309AA"));

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
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
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Document = revit.Application.ActiveUIDocument.Document;

            var trans = new Transaction(Document, "FindSouthFacingWindowsWithProjectLocation");
            trans.Start();

            Execute(true);

            CloseFile();
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
