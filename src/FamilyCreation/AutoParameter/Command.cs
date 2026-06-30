// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoParameter.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class AddParameterToFamily : IExternalCommand
    {
        private UIApplication m_app;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            m_app = commandData.Application;
            MessageManager.MessageBuff = new StringBuilder();

            try
            {
                var succeeded = AddParameters();

                if (succeeded)
                {
                    return Result.Succeeded;
                }

                message = MessageManager.MessageBuff.ToString();
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        private bool AddParameters()
        {
            var doc = m_app.ActiveUIDocument.Document;
            if (null == doc)
            {
                MessageManager.MessageBuff.Append("There's no available document. \n");
                return false;
            }

            if (!doc.IsFamilyDocument)
            {
                MessageManager.MessageBuff.Append("The active document is not a family document. \n");
                return false;
            }

            var assigner = new FamilyParameterAssigner(m_app.Application, doc);
            var succeeded = assigner.LoadParametersFromFile();
            if (!succeeded) return false;

            var t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            succeeded = assigner.AddParameters();
            if (succeeded)
            {
                t.Commit();
                return true;
            }

            t.RollBack();
            return false;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class AddParameterToFamilies : IExternalCommand
    {
        private Application m_app;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            m_app = commandData.Application.Application;
            MessageManager.MessageBuff = new StringBuilder();

            try
            {
                var succeeded = LoadFamiliesAndAddParameters();

                if (succeeded)
                {
                    return Result.Succeeded;
                }

                message = MessageManager.MessageBuff.ToString();
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        private bool LoadFamiliesAndAddParameters()
        {
            var succeeded = true;

            new List<string>();

            var myDocumentsFolder = Environment.SpecialFolder.MyDocuments;
            var myDocs = Environment.GetFolderPath(myDocumentsFolder);
            var families = $"{myDocs}\\AutoParameter_Families";
            if (!Directory.Exists(families))
                MessageManager.MessageBuff.Append(
                    "The folder [AutoParameter_Families] doesn't exist in [MyDocuments] folder.\n");
            var familiesDir = new DirectoryInfo(families);
            var files = familiesDir.GetFiles("*.rfa");
            if (0 == files.Length)
                MessageManager.MessageBuff.Append("No family file exists in [AutoParameter_Families] folder.\n");
            foreach (var info in files)
            {
                if (info.IsReadOnly)
                {
                    MessageManager.MessageBuff.Append(
                        $"Family file: \"{info.FullName}\" is read only. Can not add parameters to it.\n");
                    continue;
                }

                var famFilePath = info.FullName;
                var doc = m_app.OpenDocumentFile(famFilePath);

                if (!doc.IsFamilyDocument)
                {
                    succeeded = false;
                    MessageManager.MessageBuff.Append($"Document: \"{famFilePath}\" is not a family document.\n");
                    continue;
                }

                if (!succeeded) return false;

                var assigner = new FamilyParameterAssigner(m_app, doc);
                succeeded = assigner.LoadParametersFromFile();
                if (!succeeded)
                {
                    MessageManager.MessageBuff.Append("Failed to load parameters from parameter files.\n");
                    return false;
                }

                var t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString());
                t.Start();
                succeeded = assigner.AddParameters();
                if (succeeded)
                {
                    t.Commit();
                    doc.Save();
                    doc.Close();
                }
                else
                {
                    t.RollBack();
                    doc.Close();
                    MessageManager.MessageBuff.Append($"Failed to add parameters to {famFilePath}.\n");
                    return false;
                }
            }

            return true;
        }
    }

    public static class MessageManager
    {
        public static StringBuilder MessageBuff { get; set; } = new StringBuilder();
    }
}
