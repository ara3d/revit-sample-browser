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
        public static readonly DockablePaneId PaneId = new DockablePaneId(new Guid("{B6579F42-2F4A-4552-92EF-24B3A897757D}"));
        private Document m_document;

        private readonly ExternalEvent m_event;
        private readonly IList<ElementTypeGroup> m_finishedTypeGroup = new List<ElementTypeGroup>();
        private readonly DefaultElementTypeCommandHandler m_handler;

        public DefaultElementTypes()
        {
            InitializeComponent();

            m_handler = new DefaultElementTypeCommandHandler();
            m_event = ExternalEvent.Create(m_handler);

            //TODO: add enums that validators have been correctly set up to the array
            //this will be removed when all enums have been done
            m_finishedTypeGroup.Add(ElementTypeGroup.RadialDimensionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.LinearDimensionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.AngularDimensionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ArcLengthDimensionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DiameterDimensionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.SpotElevationType);
            m_finishedTypeGroup.Add(ElementTypeGroup.SpotCoordinateType);
            m_finishedTypeGroup.Add(ElementTypeGroup.SpotSlopeType);
            m_finishedTypeGroup.Add(ElementTypeGroup.LevelType);
            m_finishedTypeGroup.Add(ElementTypeGroup.GridType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FasciaType);
            m_finishedTypeGroup.Add(ElementTypeGroup.GutterType);
            m_finishedTypeGroup.Add(ElementTypeGroup.EdgeSlabType);
            m_finishedTypeGroup.Add(ElementTypeGroup.WallType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RoofType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RoofSoffitType);
            m_finishedTypeGroup.Add(ElementTypeGroup.TagNoteType);
            m_finishedTypeGroup.Add(ElementTypeGroup.TextNoteType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ModelTextType);
            m_finishedTypeGroup.Add(ElementTypeGroup.MultiReferenceAnnotationType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FilledRegionType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ColorFillType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DetailGroupType);
            m_finishedTypeGroup.Add(ElementTypeGroup.AttachedDetailGroupType);
            m_finishedTypeGroup.Add(ElementTypeGroup.LineLoadType);
            m_finishedTypeGroup.Add(ElementTypeGroup.AreaLoadType);
            m_finishedTypeGroup.Add(ElementTypeGroup.PointLoadType);
            m_finishedTypeGroup.Add(ElementTypeGroup.StairsBySketchType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RailingsTypeForStairs);
            m_finishedTypeGroup.Add(ElementTypeGroup.RailingsTypeForRamps);
            m_finishedTypeGroup.Add(ElementTypeGroup.RampType);
            m_finishedTypeGroup.Add(ElementTypeGroup.StairsRailingType);
            m_finishedTypeGroup.Add(ElementTypeGroup.StairsType);
            m_finishedTypeGroup.Add(ElementTypeGroup.PipeType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FlexPipeType);
            m_finishedTypeGroup.Add(ElementTypeGroup.PipeInsulationType);
            m_finishedTypeGroup.Add(ElementTypeGroup.WireType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RebarBarType);
            m_finishedTypeGroup.Add(ElementTypeGroup.AreaReinforcementType);
            m_finishedTypeGroup.Add(ElementTypeGroup.PathReinforcementType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FabricAreaType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FabricSheetType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DuctType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FlexDuctType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DuctInsulationType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DuctLiningType);
            m_finishedTypeGroup.Add(ElementTypeGroup.CableTrayType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ConduitType);
            m_finishedTypeGroup.Add(ElementTypeGroup.CeilingType);
            m_finishedTypeGroup.Add(ElementTypeGroup.CorniceType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RevealType);
            m_finishedTypeGroup.Add(ElementTypeGroup.CurtainSystemType);
            m_finishedTypeGroup.Add(ElementTypeGroup.AnalyticalLinkType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FloorType);
            m_finishedTypeGroup.Add(ElementTypeGroup.FootingSlabType);
            //_finishedTypeGroup.Add(ElementTypeGroup.ContFootingType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ModelGroupType);
            m_finishedTypeGroup.Add(ElementTypeGroup.BuildingPadType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ContourLabelingType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ReferenceViewerType);
            m_finishedTypeGroup.Add(ElementTypeGroup.RepeatingDetailType);
            m_finishedTypeGroup.Add(ElementTypeGroup.DecalType);
            m_finishedTypeGroup.Add(ElementTypeGroup.BeamSystemType);
            m_finishedTypeGroup.Add(ElementTypeGroup.ViewportType);
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
            if (m_document == document)
                return;

            m_document = document;

            _dataGrid_DefaultElementTypes.Items.Clear();

            foreach (ElementTypeGroup etg in Enum.GetValues(typeof(ElementTypeGroup)))
            {
                if (m_finishedTypeGroup.IndexOf(etg) == -1)
                    continue;

                var record = new ElementTypeRecord
                {
                    ElementTypeGroupName = Enum.GetName(typeof(ElementTypeGroup), etg)
                };

                var collector = new FilteredElementCollector(m_document);
                collector = collector.OfClass(typeof(ElementType));
                var query = from ElementType et in collector
                    where m_document.IsDefaultElementTypeIdValid(etg, et.Id)
                    select et; // Linq query  

                var defaultElementTypeId = m_document.GetDefaultElementTypeId(etg);

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
                if (!(sender is ComboBox cb))
                    return;

                if (!(e.AddedItems[0] is DefaultElementTypeCandidate item))
                    return;

                m_handler.SetData(item.ElementTypeGroup, item.Id);
                m_event.Raise();
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
        private ElementId m_defaultTypeId;
        private ElementTypeGroup m_elementTypeGroup;

        public string GetName()
        {
            return "Reset Default family type";
        }

        public void Execute(UIApplication revitApp)
        {
            using (var tran = new Transaction(revitApp.ActiveUIDocument.Document,
                       "Set Default element type to " + m_defaultTypeId))
            {
                tran.Start();
                revitApp.ActiveUIDocument.Document.SetDefaultElementTypeId(m_elementTypeGroup, m_defaultTypeId);
                tran.Commit();
            }
        }

        public void SetData(ElementTypeGroup elementTypeGroup, ElementId typeId)
        {
            m_elementTypeGroup = elementTypeGroup;
            m_defaultTypeId = typeId;
        }
    } // class CommandHandler
}
