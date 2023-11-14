// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Revit.SDK.Samples.GetSetDefaultTypes.CS
{
    /// <summary>
    ///     Interaction logic for DefaultElementTypes.xaml
    /// </summary>
    public partial class DefaultElementTypes : Page, IDockablePaneProvider
    {
        public static DockablePaneId PaneId = new DockablePaneId(new Guid("{B6579F42-2F4A-4552-92EF-24B3A897757D}"));
        private Document _document;


        private readonly ExternalEvent _event;
        private readonly IList<ElementTypeGroup> _finishedTypeGroup = new List<ElementTypeGroup>();
        private readonly DefaultElementTypeCommandHandler _handler;

        public DefaultElementTypes()
        {
            InitializeComponent();

            _handler = new DefaultElementTypeCommandHandler();
            _event = ExternalEvent.Create(_handler);

            //TODO: add enums that validators have been correctly set up to the array
            //this will be removed when all enums have been done
            _finishedTypeGroup.Add(ElementTypeGroup.RadialDimensionType);
            _finishedTypeGroup.Add(ElementTypeGroup.LinearDimensionType);
            _finishedTypeGroup.Add(ElementTypeGroup.AngularDimensionType);
            _finishedTypeGroup.Add(ElementTypeGroup.ArcLengthDimensionType);
            _finishedTypeGroup.Add(ElementTypeGroup.DiameterDimensionType);
            _finishedTypeGroup.Add(ElementTypeGroup.SpotElevationType);
            _finishedTypeGroup.Add(ElementTypeGroup.SpotCoordinateType);
            _finishedTypeGroup.Add(ElementTypeGroup.SpotSlopeType);
            _finishedTypeGroup.Add(ElementTypeGroup.LevelType);
            _finishedTypeGroup.Add(ElementTypeGroup.GridType);
            _finishedTypeGroup.Add(ElementTypeGroup.FasciaType);
            _finishedTypeGroup.Add(ElementTypeGroup.GutterType);
            _finishedTypeGroup.Add(ElementTypeGroup.EdgeSlabType);
            _finishedTypeGroup.Add(ElementTypeGroup.WallType);
            _finishedTypeGroup.Add(ElementTypeGroup.RoofType);
            _finishedTypeGroup.Add(ElementTypeGroup.RoofSoffitType);
            _finishedTypeGroup.Add(ElementTypeGroup.TagNoteType);
            _finishedTypeGroup.Add(ElementTypeGroup.TextNoteType);
            _finishedTypeGroup.Add(ElementTypeGroup.ModelTextType);
            _finishedTypeGroup.Add(ElementTypeGroup.MultiReferenceAnnotationType);
            _finishedTypeGroup.Add(ElementTypeGroup.FilledRegionType);
            _finishedTypeGroup.Add(ElementTypeGroup.ColorFillType);
            _finishedTypeGroup.Add(ElementTypeGroup.DetailGroupType);
            _finishedTypeGroup.Add(ElementTypeGroup.AttachedDetailGroupType);
            _finishedTypeGroup.Add(ElementTypeGroup.LineLoadType);
            _finishedTypeGroup.Add(ElementTypeGroup.AreaLoadType);
            _finishedTypeGroup.Add(ElementTypeGroup.PointLoadType);
            _finishedTypeGroup.Add(ElementTypeGroup.StairsBySketchType);
            _finishedTypeGroup.Add(ElementTypeGroup.RailingsTypeForStairs);
            _finishedTypeGroup.Add(ElementTypeGroup.RailingsTypeForRamps);
            _finishedTypeGroup.Add(ElementTypeGroup.RampType);
            _finishedTypeGroup.Add(ElementTypeGroup.StairsRailingType);
            _finishedTypeGroup.Add(ElementTypeGroup.StairsType);
            _finishedTypeGroup.Add(ElementTypeGroup.PipeType);
            _finishedTypeGroup.Add(ElementTypeGroup.FlexPipeType);
            _finishedTypeGroup.Add(ElementTypeGroup.PipeInsulationType);
            _finishedTypeGroup.Add(ElementTypeGroup.WireType);
            _finishedTypeGroup.Add(ElementTypeGroup.RebarBarType);
            _finishedTypeGroup.Add(ElementTypeGroup.AreaReinforcementType);
            _finishedTypeGroup.Add(ElementTypeGroup.PathReinforcementType);
            _finishedTypeGroup.Add(ElementTypeGroup.FabricAreaType);
            _finishedTypeGroup.Add(ElementTypeGroup.FabricSheetType);
            _finishedTypeGroup.Add(ElementTypeGroup.DuctType);
            _finishedTypeGroup.Add(ElementTypeGroup.FlexDuctType);
            _finishedTypeGroup.Add(ElementTypeGroup.DuctInsulationType);
            _finishedTypeGroup.Add(ElementTypeGroup.DuctLiningType);
            _finishedTypeGroup.Add(ElementTypeGroup.CableTrayType);
            _finishedTypeGroup.Add(ElementTypeGroup.ConduitType);
            _finishedTypeGroup.Add(ElementTypeGroup.CeilingType);
            _finishedTypeGroup.Add(ElementTypeGroup.CorniceType);
            _finishedTypeGroup.Add(ElementTypeGroup.RevealType);
            _finishedTypeGroup.Add(ElementTypeGroup.CurtainSystemType);
            _finishedTypeGroup.Add(ElementTypeGroup.AnalyticalLinkType);
            _finishedTypeGroup.Add(ElementTypeGroup.FloorType);
            _finishedTypeGroup.Add(ElementTypeGroup.FootingSlabType);
            //_finishedTypeGroup.Add(ElementTypeGroup.ContFootingType);
            _finishedTypeGroup.Add(ElementTypeGroup.ModelGroupType);
            _finishedTypeGroup.Add(ElementTypeGroup.BuildingPadType);
            _finishedTypeGroup.Add(ElementTypeGroup.ContourLabelingType);
            _finishedTypeGroup.Add(ElementTypeGroup.ReferenceViewerType);
            _finishedTypeGroup.Add(ElementTypeGroup.RepeatingDetailType);
            _finishedTypeGroup.Add(ElementTypeGroup.DecalType);
            _finishedTypeGroup.Add(ElementTypeGroup.BeamSystemType);
            _finishedTypeGroup.Add(ElementTypeGroup.ViewportType);
        }


        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;

            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Top;
        }

        /// <summary>
        ///     Sets document to the default element type pane.
        ///     It will get all valid default types for the enum and fill the data grid.
        /// </summary>
        public void SetDocument(Document document)
        {
            if (_document == document)
                return;

            _document = document;

            _dataGrid_DefaultElementTypes.Items.Clear();

            foreach (ElementTypeGroup etg in Enum.GetValues(typeof(ElementTypeGroup)))
            {
                if (_finishedTypeGroup.IndexOf(etg) == -1)
                    continue;

                var record = new ElementTypeRecord();
                record.ElementTypeGroupName = Enum.GetName(typeof(ElementTypeGroup), etg);

                var collector = new FilteredElementCollector(_document);
                collector = collector.OfClass(typeof(ElementType));
                var query = from ElementType et in collector
                    where _document.IsDefaultElementTypeIdValid(etg, et.Id)
                    select et; // Linq query  

                var defaultElementTypeId = _document.GetDefaultElementTypeId(etg);

                var defaultElementTypeCandidates = new List<DefaultElementTypeCandidate>();
                foreach (var t in query)
                {
                    var item = new DefaultElementTypeCandidate
                    {
                        Name = t.FamilyName + " - " + t.Name,
                        Id = t.Id,
                        ElementTypeGroup = etg
                    };
                    defaultElementTypeCandidates.Add(item);
                    if (t.Id == defaultElementTypeId)
                        record.DefaultElementType = item;
                }

                record.DefaultElementTypeCandidates = defaultElementTypeCandidates;


                _dataGrid_DefaultElementTypes.Items.Add(record);
            }
        }

        /// <summary>
        ///     Responses to the type selection changed.
        ///     It will set the selected type as default type.
        /// </summary>
        private void DefaultElementTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                var cb = sender as ComboBox;
                if (cb == null)
                    return;

                var item = e.AddedItems[0] as DefaultElementTypeCandidate;
                if (item == null)
                    return;

                _handler.SetData(item.ElementTypeGroup, item.Id);
                _event.Raise();
            }
        }
    }


    /// <summary>
    ///     The default element type candidate.
    /// </summary>
    public class DefaultElementTypeCandidate
    {
        /// <summary>
        ///     The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The type group.
        /// </summary>
        public ElementTypeGroup ElementTypeGroup { get; set; }

        /// <summary>
        ///     The element id.
        /// </summary>
        public ElementId Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     The element type record for the data grid.
    /// </summary>
    public class ElementTypeRecord
    {
        /// <summary>
        ///     The type group name.
        /// </summary>
        public string ElementTypeGroupName { get; set; }

        /// <summary>
        ///     List of default element type candidates.
        /// </summary>
        public List<DefaultElementTypeCandidate> DefaultElementTypeCandidates { get; set; }

        /// <summary>
        ///     The current default element type.
        /// </summary>
        public DefaultElementTypeCandidate DefaultElementType { get; set; }
    }

    /// <summary>
    ///     The command handler to set current selection as default element type.
    /// </summary>
    public class DefaultElementTypeCommandHandler : IExternalEventHandler
    {
        private ElementId _defaultTypeId;
        private ElementTypeGroup _elementTypeGroup;

        public string GetName()
        {
            return "Reset Default family type";
        }


        public void Execute(UIApplication revitApp)
        {
            using (var tran = new Transaction(revitApp.ActiveUIDocument.Document,
                       "Set Default element type to " + _defaultTypeId))
            {
                tran.Start();
                revitApp.ActiveUIDocument.Document.SetDefaultElementTypeId(_elementTypeGroup, _defaultTypeId);
                tran.Commit();
            }
        }

        public void SetData(ElementTypeGroup elementTypeGroup, ElementId typeId)
        {
            _elementTypeGroup = elementTypeGroup;
            _defaultTypeId = typeId;
        }
    } // class CommandHandler
}
