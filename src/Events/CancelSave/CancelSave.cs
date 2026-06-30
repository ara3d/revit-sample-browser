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
    /// <summary>Cancels save when Project Status on Project Info was not updated since open/create.</summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CancelSave : IExternalApplication
    {
        private const string ThisAddinFileName = "CancelSave.addin";

        private readonly Dictionary<int, string> m_documentOriginalStatusDic = new Dictionary<int, string>();
        private int m_hashCodeOfCurrentClosingDoc;

        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened += ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentCreated += ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentSaving += CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentSavingAs += CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentClosing += MemClosingDocumentHashCode;
            application.ControlledApplication.DocumentClosed += RemoveStatusofClosedDocument;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentCreated -= ReservePojectOriginalStatus;
            application.ControlledApplication.DocumentSaving -= CheckProjectStatusUpdate;
            application.ControlledApplication.DocumentSavingAs -= CheckProjectStatusUpdate;

            LogManager.LogFinalize();

            return Result.Succeeded;
        }

        private void ReservePojectOriginalStatus(object sender, RevitAPIPostDocEventArgs args)
        {
            var doc = args.Document;

            // ProjectInformation is unavailable for family documents.
            if (doc.IsFamilyDocument) return;

            LogManager.WriteLog(args, doc);

            var docHashCode = doc.GetHashCode();

            var currentProjectStatus = RetrieveProjectCurrentStatus(doc);
            m_documentOriginalStatusDic.Add(docHashCode, currentProjectStatus);

            LogManager.WriteLog($"   Current Project Status: {currentProjectStatus}");
        }

        private void CheckProjectStatusUpdate(object sender, RevitAPIPreDocEventArgs args)
        {
            var doc = args.Document;

            if (doc.IsFamilyDocument) return;

            LogManager.WriteLog(args, doc);

            var currentProjectStatus = RetrieveProjectCurrentStatus(args.Document);

            var originalProjectStatus = m_documentOriginalStatusDic[doc.GetHashCode()];

            LogManager.WriteLog(
                $"   Current Project Status: {currentProjectStatus}; Original Project Status: {originalProjectStatus}");

            if ((string.IsNullOrEmpty(currentProjectStatus) && string.IsNullOrEmpty(originalProjectStatus)) ||
                0 == string.Compare(currentProjectStatus, originalProjectStatus, true))
            {
                DealNotUpdate(args);
                return;
            }

            m_documentOriginalStatusDic.Remove(doc.GetHashCode());
            m_documentOriginalStatusDic.Add(doc.GetHashCode(), currentProjectStatus);
        }

        private void MemClosingDocumentHashCode(object sender, DocumentClosingEventArgs args)
        {
            m_hashCodeOfCurrentClosingDoc = args.Document.GetHashCode();
        }

        private void RemoveStatusofClosedDocument(object sender, DocumentClosedEventArgs args)
        {
            if (args.Status.Equals(RevitAPIEventStatus.Succeeded) &&
                m_documentOriginalStatusDic.ContainsKey(m_hashCodeOfCurrentClosingDoc))
                m_documentOriginalStatusDic.Remove(m_hashCodeOfCurrentClosingDoc);
        }

        private static void DealNotUpdate(RevitAPIPreDocEventArgs args)
        {
            string mainMessage;
            var taskDialog = new TaskDialog("CancelSave Sample");

            if (args.Cancellable)
            {
                args.Cancel();

                mainMessage =
                    "CancelSave sample detected that the Project Status parameter on Project Info has not been updated. The file will not be saved.";
            }
            else
            {
                mainMessage =
                    "The file is about to save. But CancelSave sample detected that the Project Status parameter on Project Info has not been updated.";
            }

            // Skip UI during regression tests.
            if (!LogManager.RegressionTestNow)
            {
                var additionalText =
                    "You can disable this permanently by uninstaling the CancelSave sample from Revit. Remove or rename CancelSave.addin from the addins directory.";

                taskDialog.MainInstruction = mainMessage;
                taskDialog.MainContent = additionalText;
                taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open the addins directory");
                taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
                taskDialog.DefaultButton = TaskDialogResult.Close;
                var tResult = taskDialog.Show();
                if (TaskDialogResult.CommandLink1 == tResult)
                    Process.Start("explorer.exe", DetectAddinFileLocation(args.Document.Application));
            }

            LogManager.WriteLog($"   Project Status is not updated, taskDialog informs user: {mainMessage}");
        }

        private static string RetrieveProjectCurrentStatus(Document doc)
        {
            return doc.IsFamilyDocument ? null :
                doc.ProjectInformation.Status;
        }

        private static string DetectAddinFileLocation(Application applictaion)
        {
            string addinFileFolderLocation = null;
            IList<RevitProduct> installedRevitList = RevitProductUtility.GetAllInstalledRevitProducts();

            foreach (var revit in installedRevitList)
            {
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
            }

            return addinFileFolderLocation;
        }
    }
}
