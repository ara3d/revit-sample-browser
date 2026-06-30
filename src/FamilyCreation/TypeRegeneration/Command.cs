// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.TypeRegeneration.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private FamilyManager m_familyManager;
        private string m_logFileName;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_logFileName = $"{assemblyPath}\\RegenerationLog.txt";
            if (document.IsFamilyDocument)
            {
                m_familyManager = document.FamilyManager;
                var writer = File.CreateText(m_logFileName);
                writer.WriteLine("Family Type     Result");
                writer.WriteLine("-------------------------");
                writer.Close();
                using (var msgForm = new MessageForm())
                {
                    msgForm.StartPosition = FormStartPosition.Manual;
                    CheckTypeRegeneration(msgForm);
                    return Result.Succeeded;
                }
            }

            message = "please make sure you have opened a family document!";
            return Result.Failed;
        }

        // Assigning CurrentType triggers regeneration for that family type.
        public void CheckTypeRegeneration(MessageForm msgForm)
        {
            var errorInfo = new List<string>();
            try
            {
                foreach (FamilyType type in m_familyManager.Types)
                {
                    if (!(type.Name.Trim() == ""))
                    {
                        try
                        {
                            m_familyManager.CurrentType = type;
                            msgForm.AddMessage($"{type.Name} Successful\n", true);
                            WriteLog($"{type.Name}      Successful");
                        }
                        catch
                        {
                            errorInfo.Add(type.Name);
                            msgForm.AddMessage($"{type.Name} Failed \n", true);
                            WriteLog($"{type.Name}      Failed");
                        }

                        msgForm.ShowDialog();
                    }
                }

                string resMsg;
                if (errorInfo.Count > 0)
                {
                    resMsg = $"\nResult: {errorInfo.Count} family types regeneration failed!";
                    foreach (var error in errorInfo)
                    {
                        resMsg += $"\n {error}";
                    }
                }
                else
                {
                    resMsg = "\nResult: All types in the family can regenerate successfully.";
                }

                WriteLog(resMsg);
                resMsg +=
                    $"\nIf you want to know the detail regeneration result please get log file at {m_logFileName}";
                msgForm.AddMessage(resMsg, false);
                msgForm.ShowDialog();
            }
            catch (Exception ex)
            {
                WriteLog($"There is some problem when regeneration:{ex}");
                msgForm.AddMessage($"There is some problem when regeneration:{ex}", true);
                msgForm.ShowDialog();
            }
        }

        private void WriteLog(string logStr)
        {
            var writer = File.AppendText(m_logFileName);
            writer.WriteLine(logStr);
            writer.Close();
        }
    }
}
