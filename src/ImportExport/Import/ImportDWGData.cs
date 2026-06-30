// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Import
{
    public class ImportDwgData : ImportData
    {
        private List<string> m_colorMode;

        private List<ImportColorMode> m_enumColorMode;

        private List<ImportPlacement> m_enumPlacement;

        private List<ImportUnit> m_enumUnit;

        private List<bool> m_enumVisibleLayersOnly;
        private List<string> m_placement;

        private List<string> m_unit;
        private List<string> m_visibleLayersOnly;

        public ImportDwgData(ExternalCommandData commandData, ImportFormat format)
            : base(commandData, format)
        {
            Initialize();
        }

        public bool ImportThisViewOnly { get; set; }

        public ViewSet Views { get; set; }

        public View ImportView { get; set; }

        public ReadOnlyCollection<string> ColorMode => new(m_colorMode);

        public ReadOnlyCollection<ImportColorMode> EnumColorMode =>
            new(m_enumColorMode);

        public ImportColorMode ImportColorMode { get; set; }

        public double ImportCustomScale { get; set; }

        public bool ImportOrientToView { get; set; }

        public ReadOnlyCollection<string> Placement => new(m_placement);

        public ReadOnlyCollection<ImportPlacement> EnumPlacement =>
            new(m_enumPlacement);

        public ImportPlacement ImportPlacement { get; set; }

        public ReadOnlyCollection<string> Unit => new(m_unit);

        public ReadOnlyCollection<ImportUnit> EnumUnit => new(m_enumUnit);

        public ImportUnit ImportUnit { get; set; }

        public ReadOnlyCollection<string> VisibleLayersOnly => new(m_visibleLayersOnly);

        public ReadOnlyCollection<bool> EnumVisibleLayersOnly => new(m_enumVisibleLayersOnly);

        public bool ImportVisibleLayersOnly { get; set; }

        public bool Is3DView { get; set; }

        public override bool Import()
        {
            //parameter: DWGImportOptions
            DWGImportOptions dwgImportOption = new()
            {
                ColorMode = ImportColorMode,
                CustomScale = ImportCustomScale,
                OrientToView = ImportOrientToView,
                Placement = ImportPlacement,
                ThisViewOnly = ImportThisViewOnly
            };
            var view = !ImportThisViewOnly ? ImportView : ActiveDoc.ActiveView;
            dwgImportOption.Unit = ImportUnit;
            dwgImportOption.VisibleLayersOnly = ImportVisibleLayersOnly;

            //parameter: ElementId

            //Import
            Transaction t = new(ActiveDoc);
            t.SetName("Import");
            t.Start();
            var imported = ActiveDoc.Import(ImportFileFullName, dwgImportOption, view, out _);
            t.Commit();

            return imported;
        }

        private void Initialize()
        {
            //ColorMode
            m_colorMode = [];
            m_enumColorMode = [];
            m_colorMode.Add("Black and white");
            m_enumColorMode.Add(ImportColorMode.BlackAndWhite);
            m_colorMode.Add("Preserve colors");
            m_enumColorMode.Add(ImportColorMode.Preserved);
            m_colorMode.Add("Invert colors");
            m_enumColorMode.Add(ImportColorMode.Inverted);

            //Placement
            m_placement = [];
            m_enumPlacement = [];
            m_placement.Add("Center-to-center");
            m_enumPlacement.Add(ImportPlacement.Centered);
            m_placement.Add("Origin-to-origin");
            m_enumPlacement.Add(ImportPlacement.Origin);

            //Unit
            m_unit = [];
            m_enumUnit = [];
            m_unit.Add("Auto-Detect");
            m_enumUnit.Add(ImportUnit.Default);
            m_unit.Add(ImportUnit.Foot.ToString());
            m_enumUnit.Add(ImportUnit.Foot);
            m_unit.Add(ImportUnit.Inch.ToString());
            m_enumUnit.Add(ImportUnit.Inch);
            m_unit.Add(ImportUnit.Meter.ToString());
            m_enumUnit.Add(ImportUnit.Meter);
            m_unit.Add(ImportUnit.Decimeter.ToString());
            m_enumUnit.Add(ImportUnit.Decimeter);
            m_unit.Add(ImportUnit.Centimeter.ToString());
            m_enumUnit.Add(ImportUnit.Centimeter);
            m_unit.Add(ImportUnit.Millimeter.ToString());
            m_enumUnit.Add(ImportUnit.Millimeter);
            m_unit.Add("Custom");
            m_enumUnit.Add(ImportUnit.Default);

            //VisibleLayersOnly
            m_visibleLayersOnly = [];
            m_enumVisibleLayersOnly = [];
            m_visibleLayersOnly.Add("All");
            m_enumVisibleLayersOnly.Add(false);
            m_visibleLayersOnly.Add("Visible");
            m_enumVisibleLayersOnly.Add(true);

            //Whether active view is 3D
            Is3DView = false;
            if (ActiveDoc.ActiveView.ViewType == ViewType.ThreeD) Is3DView = true;

            //Views
            Views = new ViewSet();
            GetViews();

            ImportCustomScale = 0.0;
            ImportOrientToView = true;
            ImportUnit = ImportUnit.Default;
            ImportThisViewOnly = false;
            ImportView = ActiveDoc.ActiveView;
            ImportColorMode = ImportColorMode.Inverted;
            ImportPlacement = ImportPlacement.Centered;
            ImportVisibleLayersOnly = false;

            Filter = "DWG Files (*.dwg)|*.dwg";
            Title = "Import DWG";
        }

        private void GetViews()
        {
            FilteredElementCollector collector = new(ActiveDoc);
            var itor = collector.OfClass(typeof(View)).GetElementIterator();
            itor.Reset();
            ViewSet views = new();
            ViewSet floorPlans = new();
            ViewSet ceilingPlans = new();
            ViewSet engineeringPlans = new();
            while (itor.MoveNext())
            {
                // skip view templates because they're invalid for import/export
                if (itor.Current is not View view || view.IsTemplate)
                    continue;
                switch (view.ViewType)
                {
                    case ViewType.FloorPlan:
                        floorPlans.Insert(view);
                        break;
                    case ViewType.CeilingPlan:
                        ceilingPlans.Insert(view);
                        break;
                    case ViewType.EngineeringPlan:
                        engineeringPlans.Insert(view);
                        break;
                }
            }

            foreach (View floorPlan in floorPlans)
            {
                foreach (View ceilingPlan in ceilingPlans)
                {
                    if (floorPlan.Name == ceilingPlan.Name)
                        views.Insert(floorPlan);
                }
            }

            foreach (View engineeringPlan in engineeringPlans)
            {
                if (engineeringPlan.Name == engineeringPlan.GenLevel.Name)
                    views.Insert(engineeringPlan);
            }

            var activeView = ActiveDoc.ActiveView;
            var viewType = activeView.ViewType;
            switch (viewType)
            {
                case ViewType.FloorPlan:
                case ViewType.CeilingPlan:
                    {
                        Views.Insert(activeView);
                        foreach (View view in views)
                        {
                            if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                                Views.Insert(view);
                        }

                        break;
                    }
                case ViewType.EngineeringPlan:
                    {
                        if (views.Contains(activeView)) Views.Insert(activeView);
                        foreach (View view in views)
                        {
                            if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                                Views.Insert(view);
                        }

                        break;
                    }
                //Get view of the lowest elevation
                default:
                    {
                        var i = 0;
                        double elevation = 0;
                        View viewLowestElevation = null;
                        foreach (View view in views)
                        {
                            if (i == 0)
                            {
                                elevation = view.GenLevel.Elevation;
                                viewLowestElevation = view;
                            }
                            else
                            {
                                if (view.GenLevel.Elevation <= elevation)
                                {
                                    elevation = view.GenLevel.Elevation;
                                    viewLowestElevation = view;
                                }
                            }

                            i++;
                        }

                        Views.Insert(viewLowestElevation);
                        break;
                    }
            }
        }
    }
}
