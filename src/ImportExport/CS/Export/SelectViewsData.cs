// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores views information for export
    /// </summary>
    public class SelectViewsData
    {
        /// <summary>
        ///     Revit command data
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     Whether contain 3D view in selected views
        /// </summary>
        private bool m_contain3DView;

        /// <summary>
        ///     All printable sheets
        /// </summary>
        private ViewSet m_printableSheets;

        /// <summary>
        ///     All printable views(except sheets)
        /// </summary>
        private ViewSet m_printableViews;

        /// <summary>
        ///     All selected views in UI
        /// </summary>
        private ViewSet m_selectedViews;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData"></param>
        public SelectViewsData(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_printableViews = new ViewSet();
            m_printableSheets = new ViewSet();
            m_selectedViews = new ViewSet();

            GetAllPrintableViews();
        }

        /// <summary>
        ///     All printable views(except sheets)
        /// </summary>
        public ViewSet PrintableViews
        {
            get => m_printableViews;
            set => m_printableViews = value;
        }

        /// <summary>
        ///     All printable sheets
        /// </summary>
        public ViewSet PrintableSheets
        {
            get => m_printableSheets;
            set => m_printableSheets = value;
        }

        /// <summary>
        ///     All selected views in UI
        /// </summary>
        public ViewSet SelectedViews
        {
            get => m_selectedViews;
            set => m_selectedViews = value;
        }

        /// <summary>
        ///     Whether contain 3D view in selected views
        /// </summary>
        public bool Contain3DView
        {
            get => m_contain3DView;
            set => m_contain3DView = value;
        }

        /// <summary>
        ///     Get all printable views and sheets
        /// </summary>
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
