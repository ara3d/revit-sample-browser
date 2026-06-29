// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.NetworkPressureLossReport.CS
{
    public class NetworkInfo
    {
        private double m_maxFlow; // The maximum flow value of any segment on the entire network.
        private readonly IDictionary<int, SectionInfo> m_sections;

        public NetworkInfo(Document doc)
        {
            Document = doc;
            m_maxFlow = 0.0;
            FlowDisplay = null;
            DomainType = ConnectorDomainType.Undefined;
            m_sections = new SortedDictionary<int, SectionInfo>();
        }

        public int NumberOfSections => m_sections.Count;

        public Document Document { get; }

        private string Name { get; set; }

        public string FlowDisplay { get; private set; }

        public ConnectorDomainType DomainType { get; set; }

        public static IList<NetworkInfo> FindValidNetworks(Document doc)
        {
            IList<NetworkInfo> validNetworks = new List<NetworkInfo>();

            var visitedSegments = new HashSet<MEPNetworkSegmentId>(new CompareNetworkSegmentId());

            // Find all elements that may drive the pipe or duct flow calculations.
            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_MechanicalEquipment,
                BuiltInCategory.OST_PlumbingEquipment,
                BuiltInCategory.OST_DuctTerminal
            };

            var multiCatFilter = new ElementMulticategoryFilter(categories);
            var elemCollector = new FilteredElementCollector(doc).WherePasses(multiCatFilter)
                .WhereElementIsNotElementType();
            foreach (var elem in elemCollector.ToElements())
            {
                var data = MEPAnalyticalModelData.GetMEPAnalyticalModelData(elem);
                if (data == null)
                    continue;

                var nSeg = data.GetNumberOfSegments();
                for (var ii = 0; ii < nSeg; ii++)
                {
                    var seg = data.GetSegmentByIndex(ii);
                    var idSegment = new MEPNetworkSegmentId(elem.Id, seg.Id);
                    if (visitedSegments.Contains(idSegment))
                        continue;

                    // Start from this analytical segment to traverse the entire network.
                    var newNetwork = new NetworkInfo(doc)
                    {
                        DomainType = seg.DomainType
                    };

                    // First start from the side of the start node.
                    var startNode = data.GetNodeById(seg.StartNode);
                    var iter = new MEPNetworkIterator(doc, startNode, seg);
                    for (iter.Start(); !iter.End(); iter.Next())
                    {
                        var currentSegment = iter.GetAnalyticalSegment();
                        if (currentSegment == null)
                            continue;

                        var currentModelData = iter.GetAnalyticalModelData();
                        if (currentModelData == null)
                            continue;

                        // Mark this segment so not to create the duplicate network.
                        var currentSegmentId =
                            new MEPNetworkSegmentId(currentSegment.RevitElementId, currentSegment.Id);
                        visitedSegments.Add(currentSegmentId);

                        var segFlowData = currentModelData.GetSegmentData(currentSegment.Id);
                        // Grow the network information by filling in one segment.
                        newNetwork.AddSegment(currentSegment, segFlowData);

                        // Refine the network name based on the straight segments.
                        if (currentSegment.SegmentType == MEPAnalyticalSegmentType.Segment)
                            newNetwork.RefineName(currentSegment.RevitElementId);
                    }

                    // If this is not a close loop, we must include the other side as well!
                    var endNode = data.GetNodeById(seg.EndNode);
                    iter = new MEPNetworkIterator(doc, endNode, seg);
                    for (iter.Start(); !iter.End(); iter.Next())
                    {
                        var currentSegment = iter.GetAnalyticalSegment();
                        if (currentSegment == null)
                            continue;
                        var currentSegmentId =
                            new MEPNetworkSegmentId(currentSegment.RevitElementId, currentSegment.Id);
                        // Check if the segment was already visited.
                        if (visitedSegments.Contains(currentSegmentId))
                            continue;
                        visitedSegments.Add(currentSegmentId);

                        var currentModelData = iter.GetAnalyticalModelData();
                        if (currentModelData == null)
                            continue;
                        var segFlowData = currentModelData.GetSegmentData(currentSegment.Id);
                        newNetwork.AddSegment(currentSegment, segFlowData);
                        if (currentSegment.SegmentType == MEPAnalyticalSegmentType.Segment)
                            newNetwork.RefineName(currentSegment.RevitElementId);
                    }

                    // Collect this new network
                    if (newNetwork.NumberOfSections > 0)
                    {
                        newNetwork.ResetFlowDisplay();
                        validNetworks.Add(newNetwork);
                    }
                }
            }

            return validNetworks;
        }

        public void AddSegment(MEPAnalyticalSegment segment, MEPNetworkSegmentData segmentData)
        {
            var sectionNumber = segmentData.SectionNumber;
            // The equipment segment may not belong to any section. Skip those?!
            if (sectionNumber < 0)
                return;

            var segmentFlow = Math.Abs(segmentData.Flow);
            if (segmentFlow > m_maxFlow)
                m_maxFlow = segmentFlow;

            if (!m_sections.ContainsKey(sectionNumber)) m_sections.Add(sectionNumber, new SectionInfo());
            m_sections[sectionNumber].AddSegment(Document, segment, segmentData);
        }

        private void RefineName(ElementId idElem)
        {
            var elem = Document.GetElement(idElem);
            switch (elem)
            {
                case MEPCurve aCurve:
                {
                    var sys = aCurve.MEPSystem;
                    if (sys != null) AppendName(sys.Name);
                    break;
                }
                case FabricationPart aPart:
                {
                    var serviceName = aPart.ServiceName;
                    // Get the full name of fabrication service by its id.
                    var fabConfig = FabricationConfiguration.GetFabricationConfiguration(Document);
                    var fabService = fabConfig?.GetService(aPart.ServiceId);
                    if (fabService != null) serviceName = fabService.Name;

                    AppendName(serviceName);
                    break;
                }
            }
        }

        private void AppendName(string name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = name;
            }
            else
            {
                if (!Name.Contains(name)) Name += $" + {name}";
            }
        }

        private void ResetFlowDisplay()
        {
            var specId = SpecTypeId.Flow;
            if (DomainType == ConnectorDomainType.Hvac)
                specId = SpecTypeId.AirFlow;

            // Reset the flow display value based on the maximum number after adding all segments.
            FlowDisplay = UnitFormatUtils.Format(Document.GetUnits(), specId, m_maxFlow, false);
        }

        public void ExportCsv(CsvExporter ex)
        {
            // Export the header line.
            var str = string.Format(
                "Section, Type/No, Element, Flow ({0}), Size/Hydraulic Diameter ({1}), Velocity ({2}), Velocity Pressure ({3}), Length ({4}), Coefficients, Friction ({5}), Pressure Loss ({3}), Section Pressure Loss ({3})",
                ex.GetFlowUnitSymbol(), ex.GetSizeUnitSymbol(), ex.GetVelocityUnitSymbol(), ex.GetPressureUnitSymbol(),
                ex.GetLengthUnitSymbol(), ex.GetFrictionUnitSymbol());
            ex.Writer.WriteLine(str);
            foreach (var item in m_sections)
            {
                item.Value.ExportCsv(ex, item.Key);
            }

            // Critical path if available
            var dCriticalLoss = 0.0;
            string path = null;
            foreach (var item in m_sections)
            {
                if (item.Value.IsCriticalPath)
                {
                    dCriticalLoss += item.Value.TotalPressureLoss;
                    if (string.IsNullOrEmpty(path))
                        path = item.Key.ToString();
                    else
                        path += $@" - {item.Key}";
                }
            }

            ex.Writer.WriteLine("Critical Pressure Loss: {0}, {1}", path, dCriticalLoss);
        }

        public void UpdateView(AvfViewer viewer)
        {
            var points = new List<XYZ>();
            var valList = new List<VectorAtPoint>();

            // Safety check.
            if (m_maxFlow < 0.0000001)
                return;

            foreach (var item in m_sections)
            {
                item.Value.UpdateView(viewer, points, valList, m_maxFlow);
            }

            if (points.Count > 0) viewer.AddData(points, valList);
        }
    }
}
