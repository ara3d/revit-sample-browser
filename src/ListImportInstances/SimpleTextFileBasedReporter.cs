// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ListImportInstances by Nikolay Shulga and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/ListImportInstances

using System;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ListImportInstances.CS
{
    internal class SimpleTextFileBasedReporter : IReportImportData
    {
        string _projectFileName;
        string _logFileName;
        StreamWriter _outputFile;
        string _currentSection;
        bool _warnUser;

        public bool Init(string projectFileName)
        {
            _currentSection = null;
            _warnUser = false;
            _projectFileName = string.IsNullOrEmpty(projectFileName)
                ? "Default"
                : projectFileName;

            _logFileName = Path.Combine(
                Path.GetDirectoryName(_projectFileName) ?? ".",
                Path.GetFileNameWithoutExtension(_projectFileName) + "-ListOfImportedData.txt");

            try
            {
                _outputFile = new StreamWriter(_logFileName);
                _outputFile.WriteLine("List of imported CAD data in " + projectFileName);
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                TaskDialog.Show("FindImports",
                    "You are not authorized to create " + _logFileName);
            }
            catch (System.ArgumentNullException)
            {
                TaskDialog.Show("FindImports",
                    "That's just not fair. Null argument for StreamWriter()");
            }
            catch (System.ArgumentException)
            {
                TaskDialog.Show("FindImports",
                    "Failed to create " + _logFileName);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                TaskDialog.Show("FindImports",
                    "That's not supposed to happen: directory not found: "
                    + Path.GetDirectoryName(_projectFileName));
            }
            catch (System.IO.PathTooLongException)
            {
                TaskDialog.Show("FindImports",
                    "The OS thinks the file name " + _logFileName + " is too long");
            }
            catch (System.IO.IOException)
            {
                TaskDialog.Show("FindImports",
                    "An IO error has occurred while writing to " + _logFileName);
            }
            catch (System.Security.SecurityException)
            {
                TaskDialog.Show("FindImports",
                    "The OS thinks your access rights to "
                    + Path.GetDirectoryName(_projectFileName)
                    + " are insufficient");
            }

            return false;
        }

        public void StartReportSection(string sectionName)
        {
            EndReportSection();
            _outputFile.WriteLine();
            _outputFile.WriteLine(sectionName);
            _outputFile.WriteLine();
            _currentSection = sectionName;
        }

        public void LogItem(string item) => _outputFile.WriteLine(item);

        public void SetWarning() => _warnUser = true;

        public void Done()
        {
            EndReportSection();
            _outputFile.WriteLine();
            _outputFile.WriteLine("The End");
            _outputFile.WriteLine();
            _outputFile.Close();

            var doneMsg = _warnUser
                ? new TaskDialog("Potential issues found. Please review the log file")
                : new TaskDialog("FindImports completed successfully");

            doneMsg.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink1,
                "Review " + _logFileName);

            if (doneMsg.Show() == TaskDialogResult.CommandLink1)
                Process.Start("notepad.exe", _logFileName);
        }

        public string GetLogFileName() => _logFileName;

        void EndReportSection()
        {
            if (_currentSection == null)
                return;

            _outputFile.WriteLine();
            _outputFile.WriteLine("End of " + _currentSection);
            _outputFile.WriteLine();
        }
    }
}
