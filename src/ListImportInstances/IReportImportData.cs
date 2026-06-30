// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ListImportInstances by Nikolay Shulga and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/ListImportInstances

namespace Ara3D.RevitSampleBrowser.ListImportInstances.CS
{
    /// <summary>
    /// Reports imported data found in a specific project.
    /// </summary>
    internal interface IReportImportData
    {
        bool Init(string projectName);
        void StartReportSection(string sectionName);
        void LogItem(string item);
        void SetWarning();
        void Done();
        string GetLogFileName();
    }
}
