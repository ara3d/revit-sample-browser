// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class SelectViewsData
    {
        private readonly ExternalCommandData m_commandData;

        public SelectViewsData(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            PrintableViews = new ViewSet();
            PrintableSheets = new ViewSet();
            SelectedViews = new ViewSet();

            GetAllPrintableViews();
        }

        public ViewSet PrintableViews { get; set; }

        public ViewSet PrintableSheets { get; set; }

        public ViewSet SelectedViews { get; set; }

        public bool Contain3DView { get; set; }

        private void GetAllPrintableViews()
        {
            FilteredElementCollector collector = new(m_commandData.Application.ActiveUIDocument.Document);
            var itor = collector.OfClass(typeof(View)).GetElementIterator();
            itor.Reset();
            PrintableViews.Clear();
            PrintableSheets.Clear();

            while (itor.MoveNext())
            {
                // skip view templates because they're invisible in project browser, invalid for print
                if (itor.Current is not View view || view.IsTemplate || !view.CanBePrinted)
                    continue;
                if (view.ViewType == ViewType.DrawingSheet)
                    PrintableSheets.Insert(view);
                else
                    PrintableViews.Insert(view);
            }
        }
    }
}
