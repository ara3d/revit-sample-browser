// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class SelectViewsData
    {
        private readonly ExternalCommandData m_commandData;

        private bool m_contain3DView;

        private ViewSet m_printableSheets;

        private ViewSet m_printableViews;

        private ViewSet m_selectedViews;

        public SelectViewsData(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_printableViews = new ViewSet();
            m_printableSheets = new ViewSet();
            m_selectedViews = new ViewSet();

            GetAllPrintableViews();
        }

        public ViewSet PrintableViews
        {
            get => m_printableViews;
            set => m_printableViews = value;
        }

        public ViewSet PrintableSheets
        {
            get => m_printableSheets;
            set => m_printableSheets = value;
        }

        public ViewSet SelectedViews
        {
            get => m_selectedViews;
            set => m_selectedViews = value;
        }

        public bool Contain3DView
        {
            get => m_contain3DView;
            set => m_contain3DView = value;
        }

        private void GetAllPrintableViews()
        {
            var collector = new FilteredElementCollector(m_commandData.Application.ActiveUIDocument.Document);
            var itor = collector.OfClass(typeof(View)).GetElementIterator();
            itor.Reset();
            m_printableViews.Clear();
            m_printableSheets.Clear();

            while (itor.MoveNext())
            {
                // skip view templates because they're invisible in project browser, invalid for print
                if (!(itor.Current is View view) || view.IsTemplate || !view.CanBePrinted)
                    continue;
                if (view.ViewType == ViewType.DrawingSheet)
                    m_printableSheets.Insert(view);
                else
                    m_printableViews.Insert(view);
            }
        }
    }
}
