// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.NetworkPressureLossReport.CS
{
    public class AvfViewer
    {
        private const string DisplayStyleName = "NetworkFlowDisplayStyle";
        private const string SchemaName = "NetworkFlowSchema";
        private readonly bool? m_isItemzied;
        private XYZ m_maxCorner;
        private int m_schemaIdx;
        private readonly SpatialFieldManager m_sfm;
        private readonly View m_view;

        public AvfViewer(View view, bool? isItemized)
        {
            m_view = view;
            m_isItemzied = isItemized;

            m_sfm = SpatialFieldManager.GetSpatialFieldManager(view) ?? SpatialFieldManager.CreateSpatialFieldManager(view, 1);
            m_sfm.Clear();
            m_maxCorner = new XYZ(-double.MaxValue, -double.MaxValue, -double.MaxValue);
        }

        public Document Document => m_view.Document;

        public double Scale => m_view.Scale;

        public bool IsItemized => m_isItemzied == true;

        public void InitAvf()
        {
            m_view.EnableTemporaryViewPropertiesMode(m_view.Id);
            m_view.TemporaryViewModes.RemoveCustomization();
            m_view.TemporaryViewModes.CustomTitle = "Network Flow Analysis";

            var resultSchema = new AnalysisResultSchema(SchemaName, "");
            m_sfm.SetMeasurementNames(new List<string> { SchemaName });
            m_schemaIdx = m_sfm.RegisterResult(resultSchema);
        }

        public int AddData(List<XYZ> points, List<VectorAtPoint> valList)
        {
            var idx = m_sfm.AddSpatialFieldPrimitive();
            var pnts = new FieldDomainPointsByXYZ(points);
            var vals = new FieldValues(valList);
            m_sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, m_schemaIdx);
            return idx;
        }

        public void AddCorner(double maxX, double maxY, double maxZ)
        {
            var xx = Math.Max(m_maxCorner.X, maxX);
            var yy = Math.Max(m_maxCorner.Y, maxY);
            var zz = Math.Max(m_maxCorner.Z, maxZ);
            m_maxCorner = new XYZ(xx, yy, zz);
        }

        private AnalysisDisplayStyle GetStyleByName(string name)
        {
            var collector = new FilteredElementCollector(Document);
            ICollection<Element> collection = collector.OfClass(typeof(AnalysisDisplayStyle)).ToElements();
            var displayStyle = from element in collection
                where element.Name == name
                select element;
            AnalysisDisplayStyle analysisDisplayStyle = null;
            if (displayStyle.Count() != 0)
                analysisDisplayStyle = displayStyle.Cast<AnalysisDisplayStyle>().ElementAt(0);
            return analysisDisplayStyle;
        }

        public void FinishDisplayStyle()
        {
            // set the legend to the top right corner so it is close to the AVF display
            m_sfm.LegendPosition = m_maxCorner;

            var analysisDisplayStyle = GetStyleByName(DisplayStyleName);

            // If display style does not already exist in the document, create it
            var colorSettings = new AnalysisDisplayColorSettings();
            if (analysisDisplayStyle == null)
            {
                colorSettings.MaxColor = new Color(255, 0, 0);
                colorSettings.MinColor = new Color(0, 255, 0);

                var vectorSettings = new AnalysisDisplayVectorSettings();
                vectorSettings.VectorOrientation = AnalysisDisplayStyleVectorOrientation.Linear;
                vectorSettings.ArrowheadScale = AnalysisDisplayStyleVectorArrowheadScale.NoScaling;
                vectorSettings.ArrowLineWeight = 4;
                vectorSettings.VectorTextType = AnalysisDisplayStyleVectorTextType.ShowNone;

                var legendSettings = new AnalysisDisplayLegendSettings();
                legendSettings.ShowLegend = false;

                analysisDisplayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(Document, DisplayStyleName,
                    vectorSettings, colorSettings, legendSettings);
            }

            m_view.AnalysisDisplayStyleId = analysisDisplayStyle.Id;

            m_view.TemporaryViewModes.CustomColor = analysisDisplayStyle.GetColorSettings().MaxColor;

            // Transparent everything so we can see the flow vector.
            var rasterId = new ElementId(BuiltInCategory.OST_RasterImages);
            foreach (Category c in m_view.Document.Settings.Categories)
                if (!m_view.GetCategoryHidden(c.Id))
                    if (c.Id != rasterId && m_view.IsCategoryOverridable(c.Id))
                    {
                        var ogs = m_view.GetCategoryOverrides(c.Id);
                        if (!ogs.Halftone)
                        {
                            m_view.SetCategoryOverrides(c.Id, ogs.SetHalftone(true));
                            m_view.SetCategoryOverrides(c.Id, ogs.SetSurfaceTransparency(50));
                        }
                    }
        }
    }
}
