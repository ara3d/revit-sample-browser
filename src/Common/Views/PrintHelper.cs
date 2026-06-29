// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class PrintHelper
    {
        public const string SampleSettingsName = "sample";

        public static ExportPDFSettings FindSampleSettings(Document doc) =>
                    ExportPDFSettings.FindByName(doc, SampleSettingsName);

        public static void MyMessageBox(string text) =>
                    TaskDialog.Show("View Printer", text);

    }
}