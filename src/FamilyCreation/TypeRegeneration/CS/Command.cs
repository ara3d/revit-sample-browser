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
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     this class controls the class which subscribes handle events and the events' information UI.
    ///     like a bridge between them.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     store family manager
        /// </summary>
        private FamilyManager m_familyManager;

        /// <summary>
        ///     store the log file name
        /// </summary>
        private string m_logFileName;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_logFileName = $"{assemblyPath}\\RegenerationLog.txt";
            //only a family document  can retrieve family manager
            if (document.IsFamilyDocument)
            {
                m_familyManager = document.FamilyManager;
                //create regeneration log file
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

        /// <summary>
        ///     After setting CurrentType property, the CurrentType has changed to the new one,the Revit model will change along
        ///     with the current type
        /// </summary>
        /// <param name="msgForm">the form is used to show the regeneration result</param>
        public void CheckTypeRegeneration(MessageForm msgForm)
        {
            //the list to record the error messages           
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

                //add a conclusion regeneration result
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

        /// <summary>
        ///     The method to write line to log file
        /// </summary>
        /// <param name="logStr">the log string</param>
        private void WriteLog(string logStr)
        {
            var writer = File.AppendText(m_logFileName);
            writer.WriteLine(logStr);
            writer.Close();
        }
    }
}
