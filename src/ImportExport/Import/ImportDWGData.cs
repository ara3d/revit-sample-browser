// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Import
{
    public class ImportDwgData : ImportData
    {
        private List<string> m_colorMode;

        private List<ImportColorMode> m_enumColorMode;

        private List<ImportPlacement> m_enumPlacement;

        private List<ImportUnit> m_enumUnit;

        private List<bool> m_enumVisibleLayersOnly;

        private ImportColorMode m_importColorMode;

        private double m_importCustomScale;

        private bool m_importOrientToView;

        private ImportPlacement m_importPlacement;

        private bool m_importThisViewOnly;

        private ImportUnit m_importUnit;

        private View m_importView;

        private bool m_importVisibleLayersOnly;

        private bool m_is3DView;

        private List<string> m_placement;

        private List<string> m_unit;

        private ViewSet m_views;

        private List<string> m_visibleLayersOnly;

        public ImportDwgData(ExternalCommandData commandData, ImportFormat format)
            : base(commandData, format)
        {
            Initialize();
        }

        public bool ImportThisViewOnly
        {
            get => m_importThisViewOnly;
            set => m_importThisViewOnly = value;
        }

        public ViewSet Views
        {
            get => m_views;
            set => m_views = value;
        }

        public View ImportView
        {
            get => m_importView;
            set => m_importView = value;
        }

        public ReadOnlyCollection<string> ColorMode => new ReadOnlyCollection<string>(m_colorMode);

        public ReadOnlyCollection<ImportColorMode> EnumColorMode =>
            new ReadOnlyCollection<ImportColorMode>(m_enumColorMode);

        public ImportColorMode ImportColorMode
        {
            get => m_importColorMode;
            set => m_importColorMode = value;
        }

        public double ImportCustomScale
        {
            get => m_importCustomScale;
            set => m_importCustomScale = value;
        }

        public bool ImportOrientToView
        {
            get => m_importOrientToView;
            set => m_importOrientToView = value;
        }

        public ReadOnlyCollection<string> Placement => new ReadOnlyCollection<string>(m_placement);

        public ReadOnlyCollection<ImportPlacement> EnumPlacement =>
            new ReadOnlyCollection<ImportPlacement>(m_enumPlacement);

        public ImportPlacement ImportPlacement
        {
            get => m_importPlacement;
            set => m_importPlacement = value;
        }

        public ReadOnlyCollection<string> Unit => new ReadOnlyCollection<string>(m_unit);

        public ReadOnlyCollection<ImportUnit> EnumUnit => new ReadOnlyCollection<ImportUnit>(m_enumUnit);

        public ImportUnit ImportUnit
        {
            get => m_importUnit;
            set => m_importUnit = value;
        }

        public ReadOnlyCollection<string> VisibleLayersOnly => new ReadOnlyCollection<string>(m_visibleLayersOnly);

        public ReadOnlyCollection<bool> EnumVisibleLayersOnly => new ReadOnlyCollection<bool>(m_enumVisibleLayersOnly);

        public bool ImportVisibleLayersOnly
        {
            get => m_importVisibleLayersOnly;
            set => m_importVisibleLayersOnly = value;
        }

        public bool Is3DView
        {
            get => m_is3DView;
            set => m_is3DView = value;
        }

        public override bool Import()
        {
            //parameter: DWGImportOptions
            var dwgImportOption = new DWGImportOptions
            {
                ColorMode = m_importColorMode,
                CustomScale = m_importCustomScale,
                OrientToView = m_importOrientToView,
                Placement = m_importPlacement,
                ThisViewOnly = m_importThisViewOnly
            };
            View view = null;
            if (!m_importThisViewOnly)
                view = m_importView;
            else
                view = ActiveDoc.ActiveView;
            dwgImportOption.Unit = m_importUnit;
            dwgImportOption.VisibleLayersOnly = m_importVisibleLayersOnly;

            //parameter: ElementId

            //Import
            var t = new Transaction(ActiveDoc);
            t.SetName("Import");
            t.Start();
            var imported = ActiveDoc.Import(ImportFileFullName, dwgImportOption, view, out _);
            t.Commit();

            return imported;
        }

        private void Initialize()
        {
            //ColorMode
            m_colorMode = new List<string>();
            m_enumColorMode = new List<ImportColorMode>();
            m_colorMode.Add("Black and white");
            m_enumColorMode.Add(ImportColorMode.BlackAndWhite);
            m_colorMode.Add("Preserve colors");
            m_enumColorMode.Add(ImportColorMode.Preserved);
            m_colorMode.Add("Invert colors");
            m_enumColorMode.Add(ImportColorMode.Inverted);

            //Placement
            m_placement = new List<string>();
            m_enumPlacement = new List<ImportPlacement>();
            m_placement.Add("Center-to-center");
            m_enumPlacement.Add(ImportPlacement.Centered);
            m_placement.Add("Origin-to-origin");
            m_enumPlacement.Add(ImportPlacement.Origin);

            //Unit
            m_unit = new List<string>();
            m_enumUnit = new List<ImportUnit>();
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
            m_visibleLayersOnly = new List<string>();
            m_enumVisibleLayersOnly = new List<bool>();
            m_visibleLayersOnly.Add("All");
            m_enumVisibleLayersOnly.Add(false);
            m_visibleLayersOnly.Add("Visible");
            m_enumVisibleLayersOnly.Add(true);

            //Whether active view is 3D
            m_is3DView = false;
            if (ActiveDoc.ActiveView.ViewType == ViewType.ThreeD) m_is3DView = true;

            //Views
            m_views = new ViewSet();
            GetViews();

            m_importCustomScale = 0.0;
            m_importOrientToView = true;
            m_importUnit = ImportUnit.Default;
            m_importThisViewOnly = false;
            m_importView = ActiveDoc.ActiveView;
            m_importColorMode = ImportColorMode.Inverted;
            m_importPlacement = ImportPlacement.Centered;
            m_importVisibleLayersOnly = false;

            Filter = "DWG Files (*.dwg)|*.dwg";
            Title = "Import DWG";
        }

        private void GetViews()
        {
            var collector = new FilteredElementCollector(ActiveDoc);
            var itor = collector.OfClass(typeof(View)).GetElementIterator();
            itor.Reset();
            var views = new ViewSet();
            var floorPlans = new ViewSet();
            var ceilingPlans = new ViewSet();
            var engineeringPlans = new ViewSet();
            while (itor.MoveNext())
            {
                // skip view templates because they're invalid for import/export
                if (!(itor.Current is View view) || view.IsTemplate)
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
                    m_views.Insert(activeView);
                    foreach (View view in views)
                    {
                        if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                            m_views.Insert(view);
                    }

                    break;
                }
                case ViewType.EngineeringPlan:
                {
                    if (views.Contains(activeView)) m_views.Insert(activeView);
                    foreach (View view in views)
                    {
                        if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                            m_views.Insert(view);
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

                    m_views.Insert(viewLowestElevation);
                    break;
                }
            }
        }
    }
}
