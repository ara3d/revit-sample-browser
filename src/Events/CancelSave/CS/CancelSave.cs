// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.RevitAddIns;

namespace Ara3D.RevitSampleBrowser.Events.CancelSave.CS
{
    /// <summary>
    ///     This class is an external application which checks whether "Project Status" is updated
    ///     once the project is about to be saved. If updated pass the save else cancel the save and inform user with one
    ///     message.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CancelSave : IExternalApplication
    {
        private const string ThisAddinFileName = "CancelSave.addin";

        // The dictionary contains document hashcode and its original "Project Status" pair.
        private readonly Dictionary<int, string> m_documentOriginalStatusDic = new Dictionary<int, string>();
        private int m_hashcodeofCurrentClosingDoc;

        /// <summary>
        ///     Implement OnStartup method of IExternalApplication interface.
        ///     This method subscribes to DocumentOpened, DocumentCreated, DocumentSaving and DocumentSavingAs events.
        ///     The first two events are used to reserve "Project Status" original value;
        ///     The last two events are used to check whether "Project Status" has been updated, and re-reserve current value as
        ///     new original value for next compare.
        /// </summary>
        /// <param name="application">Controlled application to be loaded to Revit process.</param>
        /// <returns>The status of the external application</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // subscribe to DocumentOpened, DocumentCreated, DocumentSaving and DocumentSavingAs events
            application.ControlledApplication.DocumentOpened += ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentCreated += ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentSaving += CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentSavingAs += CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentClosing += MemClosingDocumentHashCode;
            application.ControlledApplication.DocumentClosed += RemoveStatusofClosedDocument;

            return Result.Succeeded;
        }

        /// <summary>
        ///     Implement OnShutdown method of IExternalApplication interface.
        /// </summary>
        /// <param name="application">Controlled application to be shutdown.</param>
        /// <returns>The status of the external application.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            // unsubscribe to DocumentOpened, DocumentCreated, DocumentSaving and DocumentSavingAs events
            application.ControlledApplication.DocumentOpened -= ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentCreated -= ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentSaving -= CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentSavingAs -= CheckProjectStatusUpdate;

            // finalize the log file.
            LogManager.LogFinalize();

            return Result.Succeeded;
        }

        /// <summary>
        ///     Event handler method for DocumentOpened and DocumentCreated events.
        ///     This method will reserve "Project Status" value after document has been opened or created.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event arguments that contains the event data.</param>
        private void ReservePojectOriginalStatus(object sender, RevitAPIPostDocEventArgs args)
        {
            // The document associated with the event. Here means which document has been created or opened.
            var doc = args.Document;

            // Project information is unavailable for Family document.
            if (doc.IsFamilyDocument) return;

            // write log file. 
            LogManager.WriteLog(args, doc);

            // get the hashCode of this document.
            var docHashCode = doc.GetHashCode();

            // retrieve the current value of "Project Status". 
            var currentProjectStatus = RetrieveProjectCurrentStatus(doc);
            // reserve "Project Status" current value in one dictionary, and use this project's hashCode as key.
            m_documentOriginalStatusDic.Add(docHashCode, currentProjectStatus);

            // write log file. 
            LogManager.WriteLog("   Current Project Status: " + currentProjectStatus);
        }

        /// <summary>
        ///     Event handler method for DocumentSaving and DocumentSavingAs events.
        ///     This method will check whether "Project Status" has been updated, and reserve current value as original value.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event arguments that contains the event data.</param>
        private void CheckProjectStatusUpdate(object sender, RevitAPIPreDocEventArgs args)
        {
            // The document associated with the event. Here means which document is about to save / save as.
            var doc = args.Document;

            // Project information is unavailable for Family document.
            if (doc.IsFamilyDocument) return;

            // write log file.
            LogManager.WriteLog(args, doc);

            // retrieve the current value of "Project Status". 
            var currentProjectStatus = RetrieveProjectCurrentStatus(args.Document);

            // get the old value of "Project Status" for one dictionary.
            var originalProjectStatus = m_documentOriginalStatusDic[doc.GetHashCode()];

            // write log file.
            LogManager.WriteLog("   Current Project Status: " + currentProjectStatus + "; Original Project Status: " +
                                originalProjectStatus);

            // project status has not been updated.
            if ((string.IsNullOrEmpty(currentProjectStatus) && string.IsNullOrEmpty(originalProjectStatus)) ||
                0 == string.Compare(currentProjectStatus, originalProjectStatus, true))
            {
                DealNotUpdate(args);
                return;
            }

            // update "Project Status" value reserved in the dictionary.
            m_documentOriginalStatusDic.Remove(doc.GetHashCode());
            m_documentOriginalStatusDic.Add(doc.GetHashCode(), currentProjectStatus);
        }

        private void MemClosingDocumentHashCode(object sender, DocumentClosingEventArgs args)
        {
            m_hashcodeofCurrentClosingDoc = args.Document.GetHashCode();
        }

        private void RemoveStatusofClosedDocument(object sender, DocumentClosedEventArgs args)
        {
            if (args.Status.Equals(RevitAPIEventStatus.Succeeded) &&
                m_documentOriginalStatusDic.ContainsKey(m_hashcodeofCurrentClosingDoc))
                m_documentOriginalStatusDic.Remove(m_hashcodeofCurrentClosingDoc);
        }

        /// <summary>
        ///     Deal with the case that the project status wasn't updated.
        ///     If the event is Cancellable, cancel it and inform user else just inform user the status.
        /// </summary>
        /// <param name="args">Event arguments that contains the event data.</param>
        private static void DealNotUpdate(RevitAPIPreDocEventArgs args)
        {
            string mainMessage;
            var taskDialog = new TaskDialog("CancelSave Sample");

            if (args.Cancellable)
            {
                args.Cancel(); // cancel this event if it is cancellable. 

                mainMessage =
                    "CancelSave sample detected that the Project Status parameter on Project Info has not been updated. The file will not be saved."; // prompt to user.              
            }
            else
            {
                // will not cancel this event since it isn't cancellable. 
                mainMessage =
                    "The file is about to save. But CancelSave sample detected that the Project Status parameter on Project Info has not been updated."; // prompt to user.              
            }

            // taskDialog will not show when do regression test.
            if (!LogManager.RegressionTestNow)
            {
                var additionalText =
                    "You can disable this permanently by uninstaling the CancelSave sample from Revit. Remove or rename CancelSave.addin from the addins directory.";

                // use one taskDialog to inform user current situation.     
                taskDialog.MainInstruction = mainMessage;
                taskDialog.MainContent = additionalText;
                taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open the addins directory");
                taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
                taskDialog.DefaultButton = TaskDialogResult.Close;
                var tResult = taskDialog.Show();
                if (TaskDialogResult.CommandLink1 == tResult)
                    Process.Start("explorer.exe", DetectAddinFileLocation(args.Document.Application));
            }

            // write log file.
            LogManager.WriteLog("   Project Status is not updated, taskDialog informs user: " + mainMessage);
        }

        /// <summary>
        ///     Retrieve current value of Project Status.
        /// </summary>
        /// <param name="doc">Document of which the Project Status will be retrieved.</param>
        /// <returns>Current value of Project Status.</returns>
        private static string RetrieveProjectCurrentStatus(Document doc)
        {
            // Project information is unavailable for Family document.
            return doc.IsFamilyDocument ? null :
                // get project status stored in project information object and return it.
                doc.ProjectInformation.Status;
        }

        private static string DetectAddinFileLocation(Application applictaion)
        {
            string addinFileFolderLocation = null;
            IList<RevitProduct> installedRevitList = RevitProductUtility.GetAllInstalledRevitProducts();

            foreach (var revit in installedRevitList)
                if (revit.Version.ToString().Contains(applictaion.VersionNumber))
                {
                    var allUsersAddInFolder = revit.AllUsersAddInFolder;
                    var currentUserAddInFolder = revit.CurrentUserAddInFolder;

                    if (File.Exists(Path.Combine(allUsersAddInFolder, ThisAddinFileName)))
                        addinFileFolderLocation = allUsersAddInFolder;
                    else if (File.Exists(Path.Combine(currentUserAddInFolder, ThisAddinFileName)))
                        addinFileFolderLocation = currentUserAddInFolder;

                    break;
                }

            return addinFileFolderLocation;
        }
    }
}
