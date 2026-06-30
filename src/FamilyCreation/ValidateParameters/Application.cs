// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.ValidateParameters.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Application : IExternalApplication
    {
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

        private void application_DocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            ValidateParameters(e.Document);
        }

        private void application_DocumentSavingAs(object sender, DocumentSavingAsEventArgs e)
        {
            ValidateParameters(e.Document);
        }

        private void ValidateParameters(Document doc)
        {
            List<string> errorInfo = [];
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

            using MessageForm msgForm = new(errorInfo.ToArray());
            msgForm.StartPosition = FormStartPosition.CenterParent;
            msgForm.ShowDialog();
        }
    }
}
