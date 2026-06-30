// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportPdfData : ExportDataWithViews
    {
        public ExportPdfData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Filter = "PDF Files |*.pdf";
            Title = "Export PDF";

            Combine = true;
        }

        public bool Combine { get; set; }

        public override bool Export()
        {
            base.Export();

            // Parameter: The list of view/sheet id to export
            IList<ElementId> views = new List<ElementId>();
            if (CurrentViewOnly)
            {
                views.Add(ActiveDocument.ActiveView.Id);
            }
            else
            {
                var viewSet = SelectViewsData.SelectedViews;
                foreach (View v in viewSet)
                {
                    views.Add(v.Id);
                }
            }

            // Parameter: The exporting options, including paper size, orientation, file name or naming rule and etc.
            var options = new PDFExportOptions
            {
                FileName = ExportFileName,
                Combine =
                Combine // If not combined, PDFs will be exported with default naming rule "Type-ViewName"
            };
            var exported = ActiveDocument.Export(ExportFolder, views, options);

            return exported;
        }
    }
}
