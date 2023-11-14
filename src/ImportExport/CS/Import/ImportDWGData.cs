// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for importing dwg format
    /// </summary>
    public class ImportDWGData : ImportData
    {
        /// <summary>
        ///     ColorMode for import
        /// </summary>
        private List<string> m_colorMode;

        /// <summary>
        ///     All available import color modes
        /// </summary>
        private List<ImportColorMode> m_enumColorMode;

        /// <summary>
        ///     All placement for layers to be imported
        /// </summary>
        private List<ImportPlacement> m_enumPlacement;

        /// <summary>
        ///     All import unit for import layers
        /// </summary>
        private List<ImportUnit> m_enumUnit;

        /// <summary>
        ///     All boolean values for available visible layers
        /// </summary>
        private List<bool> m_enumVisibleLayersOnly;

        /// <summary>
        ///     Import color mode
        /// </summary>
        private ImportColorMode m_importColorMode;

        /// <summary>
        ///     Custom scale for import
        /// </summary>
        private double m_importCustomScale;

        /// <summary>
        ///     OrientToView
        /// </summary>
        private bool m_importOrientToView;

        /// <summary>
        ///     Placement for import
        /// </summary>
        private ImportPlacement m_importPlacement;

        /// <summary>
        ///     ThisViewOnly
        /// </summary>
        private bool m_importThisViewOnly;

        /// <summary>
        ///     Import unit
        /// </summary>
        private ImportUnit m_importUnit;

        /// <summary>
        ///     Import view
        /// </summary>
        private View m_importView;

        /// <summary>
        ///     Whether import visible layer only
        /// </summary>
        private bool m_importVisibleLayersOnly;

        /// <summary>
        ///     Whether active view is 3D
        /// </summary>
        private bool m_is3DView;

        /// <summary>
        ///     Placement
        /// </summary>
        private List<string> m_placement;

        /// <summary>
        ///     All units for layer to be imported
        /// </summary>
        private List<string> m_unit;

        /// <summary>
        ///     All views
        /// </summary>
        private ViewSet m_views;

        /// <summary>
        ///     All available layers only
        /// </summary>
        private List<string> m_visibleLayersOnly;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="format">Format to import</param>
        public ImportDWGData(ExternalCommandData commandData, ImportFormat format)
            : base(commandData, format)
        {
            Initialize();
        }

        /// <summary>
        ///     Get or set whether import this view only
        /// </summary>
        public bool ImportThisViewOnly
        {
            get => m_importThisViewOnly;
            set => m_importThisViewOnly = value;
        }

        /// <summary>
        ///     all views for import
        /// </summary>
        public ViewSet Views
        {
            get => m_views;
            set => m_views = value;
        }

        /// <summary>
        ///     Import view
        /// </summary>
        public View ImportView
        {
            get => m_importView;
            set => m_importView = value;
        }

        /// <summary>
        ///     All available color modes for import
        /// </summary>
        public ReadOnlyCollection<string> ColorMode => new ReadOnlyCollection<string>(m_colorMode);

        /// <summary>
        ///     All available import color modes
        /// </summary>
        public ReadOnlyCollection<ImportColorMode> EnumColorMode =>
            new ReadOnlyCollection<ImportColorMode>(m_enumColorMode);

        /// <summary>
        ///     Import color mode
        /// </summary>
        public ImportColorMode ImportColorMode
        {
            get => m_importColorMode;
            set => m_importColorMode = value;
        }

        /// <summary>
        ///     Custom scale for import
        /// </summary>
        public double ImportCustomScale
        {
            get => m_importCustomScale;
            set => m_importCustomScale = value;
        }

        /// <summary>
        ///     Whether import orient to view
        /// </summary>
        public bool ImportOrientToView
        {
            get => m_importOrientToView;
            set => m_importOrientToView = value;
        }

        /// <summary>
        ///     All placement for layers to be imported
        /// </summary>
        public ReadOnlyCollection<string> Placement => new ReadOnlyCollection<string>(m_placement);

        /// <summary>
        ///     All ImportPlacements for all layers to be imported
        /// </summary>
        public ReadOnlyCollection<ImportPlacement> EnumPlacement =>
            new ReadOnlyCollection<ImportPlacement>(m_enumPlacement);

        /// <summary>
        ///     Import placement for import
        /// </summary>
        public ImportPlacement ImportPlacement
        {
            get => m_importPlacement;
            set => m_importPlacement = value;
        }

        /// <summary>
        ///     All units for layer to be imported
        /// </summary>
        public ReadOnlyCollection<string> Unit => new ReadOnlyCollection<string>(m_unit);

        /// <summary>
        ///     All import unit for import layers
        /// </summary>
        public ReadOnlyCollection<ImportUnit> EnumUnit => new ReadOnlyCollection<ImportUnit>(m_enumUnit);

        /// <summary>
        ///     Get or set import unit
        /// </summary>
        public ImportUnit ImportUnit
        {
            get => m_importUnit;
            set => m_importUnit = value;
        }

        /// <summary>
        ///     All available layers only
        /// </summary>
        public ReadOnlyCollection<string> VisibleLayersOnly => new ReadOnlyCollection<string>(m_visibleLayersOnly);

        /// <summary>
        ///     All boolean values for available visible layers
        /// </summary>
        public ReadOnlyCollection<bool> EnumVisibleLayersOnly => new ReadOnlyCollection<bool>(m_enumVisibleLayersOnly);

        /// <summary>
        ///     Whether import visible layer only
        /// </summary>
        public bool ImportVisibleLayersOnly
        {
            get => m_importVisibleLayersOnly;
            set => m_importVisibleLayersOnly = value;
        }

        /// <summary>
        ///     Whether active view is 3D
        /// </summary>
        public bool Is3DView
        {
            get => m_is3DView;
            set => m_is3DView = value;
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Import()
        {
            //parameter: DWGImportOptions
            var dwgImportOption = new DWGImportOptions();
            dwgImportOption.ColorMode = m_importColorMode;
            dwgImportOption.CustomScale = m_importCustomScale;
            dwgImportOption.OrientToView = m_importOrientToView;
            dwgImportOption.Placement = m_importPlacement;
            dwgImportOption.ThisViewOnly = m_importThisViewOnly;
            View view = null;
            if (!m_importThisViewOnly)
                view = m_importView;
            else
                view = m_activeDoc.ActiveView;
            dwgImportOption.Unit = m_importUnit;
            dwgImportOption.VisibleLayersOnly = m_importVisibleLayersOnly;

            //parameter: ElementId

            //Import
            var t = new Transaction(m_activeDoc);
            t.SetName("Import");
            t.Start();
            var imported = m_activeDoc.Import(m_importFileFullName, dwgImportOption, view, out _);
            t.Commit();

            return imported;
        }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
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
            if (m_activeDoc.ActiveView.ViewType == ViewType.ThreeD) m_is3DView = true;

            //Views
            m_views = new ViewSet();
            GetViews();

            m_importCustomScale = 0.0;
            m_importOrientToView = true;
            m_importUnit = ImportUnit.Default;
            m_importThisViewOnly = false;
            m_importView = m_activeDoc.ActiveView;
            m_importColorMode = ImportColorMode.Inverted;
            m_importPlacement = ImportPlacement.Centered;
            m_importVisibleLayersOnly = false;

            m_filter = "DWG Files (*.dwg)|*.dwg";
            m_title = "Import DWG";
        }

        /// <summary>
        ///     Get all the views to be displayed
        /// </summary>
        private void GetViews()
        {
            var collector = new FilteredElementCollector(m_activeDoc);
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
            foreach (View ceilingPlan in ceilingPlans)
                if (floorPlan.Name == ceilingPlan.Name)
                    views.Insert(floorPlan);

            foreach (View engineeringPlan in engineeringPlans)
                if (engineeringPlan.Name == engineeringPlan.GenLevel.Name)
                    views.Insert(engineeringPlan);

            var activeView = m_activeDoc.ActiveView;
            var viewType = activeView.ViewType;
            switch (viewType)
            {
                case ViewType.FloorPlan:
                case ViewType.CeilingPlan:
                {
                    m_views.Insert(activeView);
                    foreach (View view in views)
                        if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                            m_views.Insert(view);
                    break;
                }
                case ViewType.EngineeringPlan:
                {
                    if (views.Contains(activeView)) m_views.Insert(activeView);
                    foreach (View view in views)
                        if (view.GenLevel.Elevation < activeView.GenLevel.Elevation)
                            m_views.Insert(view);
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
