// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDataWithViews : ExportData
    {
        public ExportDataWithViews(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            SelectViewsData = new SelectViewsData(commandData);
        }

        public SelectViewsData SelectViewsData { get; }

        public bool CurrentViewOnly { get; set; }
    }
}
