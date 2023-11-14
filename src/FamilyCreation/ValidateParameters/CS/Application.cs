// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ValidateParameters.CS
{
    /// <summary>
    ///     A class inherits IExternalApplication interface to add an event to the document saving,
    ///     which will be called when the document is being saved.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    internal class Application : IExternalApplication
    {
        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the internal reference.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.ControlledApplication.DocumentSaving += application_DocumentSaving;
                application.ControlledApplication.DocumentSavingAs += application_DocumentSavingAs;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                application.ControlledApplication.DocumentSaving -= application_DocumentSaving;
                application.ControlledApplication.DocumentSavingAs -= application_DocumentSavingAs;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Subscribe to the DocumentSaving event to be notified when Revit is just about to save the document.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">The event argument used by DocumentSaving event. </param>
        private void application_DocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            ValidateParameters(e.Document);
        }

        /// <summary>
        ///     Subscribe to the DocumentSavingAs event to be notified when Revit is just about to save the document with a new
        ///     file name.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">The event argument used by DocumentSavingAs event.</param>
        private void application_DocumentSavingAs(object sender, DocumentSavingAsEventArgs e)
        {
            ValidateParameters(e.Document);
        }

        /// <summary>
        ///     The method is to validate parameters via FamilyParameter and FamilyType
        /// </summary>
        /// <param name="doc">the document which need to validate parameters</param>
        private void ValidateParameters(Document doc)
        {
            var errorInfo = new List<string>();
            if (doc.IsFamilyDocument)
            {
                var familyManager = doc.FamilyManager;
                errorInfo = Command.ValidateParameters(familyManager);
            }
            else
            {
                errorInfo.Add(
                    "The current document isn't a family document, so the validation doesn't work correctly!");
            }

            using (var msgForm = new MessageForm(errorInfo.ToArray()))
            {
                msgForm.StartPosition = FormStartPosition.CenterParent;
                msgForm.ShowDialog();
            }
        }
    }
}
