// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS
{
    /// <summary>
    ///     Queries all routing preferences and reports potential problems in the form of an XDocument.
    /// </summary>
    internal class Analyzer
    {
        private readonly Document m_document;
        private readonly double m_mepSize;
        private readonly RoutingPreferenceManager m_routingPreferenceManager;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="routingPreferenceManager"></param>
        /// <param name="document"></param>
        public Analyzer(RoutingPreferenceManager routingPreferenceManager, Document document)
        {
            m_routingPreferenceManager = routingPreferenceManager;
            m_document = document;
            m_mepSize = 0;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="routingPreferenceManager"></param>
        /// <param name="mepSize"></param>
        /// <param name="document"></param>
        public Analyzer(RoutingPreferenceManager routingPreferenceManager, double mepSize, Document document)
        {
            m_routingPreferenceManager = routingPreferenceManager;
            m_document = document;

            m_mepSize = Convert.ConvertValueToFeet(mepSize, m_document);
        }

        /// <summary>
        ///     Get specific size query
        /// </summary>
        /// <returns></returns>
        public XDocument GetSpecificSizeQuery()
        {
            var xReportDoc = new XDocument();
            var xroot = new XElement(XName.Get("RoutingPreferenceAnalysisSizeQuery"));
            xroot.Add(GetHeaderInformation());

            foreach (var partId in GetPreferredFittingsAndSegments()) xroot.Add(partId.GetXml(m_document));

            xReportDoc.Add(xroot);
            return xReportDoc;
        }

        /// <summary>
        ///     Get all segments from a the currently selected pipe type, get each size from each segment,
        ///     collect, sort, and return.
        /// </summary>
        public static List<double> GetAvailableSegmentSizes(RoutingPreferenceManager routingPreferenceManager,
            Document document)
        {
            var sizes = new HashSet<double>();
            var segmentCount = routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Segments);
            for (var index = 0; index != segmentCount; ++index)
            {
                var segmentRule = routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Segments, index);

                var segment = document.GetElement(segmentRule.MEPPartId) as Segment;
                foreach (var size in segment.GetSizes()) sizes.Add(size.NominalDiameter);
            }

            var sizesSorted = sizes.ToList();
            sizesSorted.Sort();
            return sizesSorted;
        }

        /// <summary>
        ///     Returns XML data for a variety of potential routing-preference problems.
        /// </summary>
        /// <returns></returns>
        public XDocument GetWarnings()
        {
            var xReportDoc = new XDocument();
            var xroot = new XElement(XName.Get("RoutingPreferenceAnalysis"));
            xroot.Add(GetHeaderInformation());
            var xWarnings = new XElement(XName.Get("Warnings"));

            //None-range-warnings         
            foreach (var groupType in Enum.GetValues(typeof(RoutingPreferenceRuleGroupType))
                         .Cast<RoutingPreferenceRuleGroupType>())
                if (IsRuleSetToRangeNone(m_routingPreferenceManager, groupType, 0))
                {
                    var xNoRangeSet = new XElement(XName.Get("NoRangeSet"));
                    xNoRangeSet.Add(new XAttribute(XName.Get("groupType"), groupType.ToString()));

                    if (IsGroupSetToRangeNone(m_routingPreferenceManager, groupType))
                        xNoRangeSet.Add(new XAttribute(XName.Get("rule"), "allRules"));
                    else
                        xNoRangeSet.Add(new XAttribute(XName.Get("rule"), "firstRule"));
                    xWarnings.Add(xNoRangeSet);
                }

            //tee/tap warnings

            if (!IsPreferredJunctionTypeValid(m_routingPreferenceManager))
            {
                var xJunctionFittingsNotDefined = new XElement(XName.Get("FittingsNotDefinedForPreferredJunction"));
                xWarnings.Add(xJunctionFittingsNotDefined);
            }

            //size range warnings for elbow, Junction, and Cross

            var xSegmentElbowWarning =
                GetSegmentRangeNotCoveredWarning(m_routingPreferenceManager, RoutingPreferenceRuleGroupType.Elbows);
            if (xSegmentElbowWarning != null)
                xWarnings.Add(xSegmentElbowWarning);
            var xSegmentTeeWarning =
                GetSegmentRangeNotCoveredWarning(m_routingPreferenceManager, RoutingPreferenceRuleGroupType.Junctions);
            if (xSegmentTeeWarning != null)
                xWarnings.Add(xSegmentTeeWarning);
            var xSegmentCrossWarning =
                GetSegmentRangeNotCoveredWarning(m_routingPreferenceManager, RoutingPreferenceRuleGroupType.Crosses);
            if (xSegmentCrossWarning != null)
                xWarnings.Add(xSegmentCrossWarning);

            xroot.Add(xWarnings);
            xReportDoc.Add(xroot);
            return xReportDoc;
        }

        /// <summary>
        ///     Get basic information about the PipeType
        /// </summary>
        private XElement GetHeaderInformation()
        {
            var xHeader = new XElement(XName.Get("PipeType"));
            var pipeTypeName = m_document.GetElement(m_routingPreferenceManager.OwnerId).Name;

            xHeader.Add(new XAttribute(XName.Get("name"), pipeTypeName));
            xHeader.Add(new XAttribute(XName.Get("elementId"), m_routingPreferenceManager.OwnerId.ToString()));
            return xHeader;
        }

        /// <summary>
        ///     Checks to see if any segments in the routing preference manager have sizes that cannot be fitted with fittings
        ///     defined in a rule group type, such as "Elbow."
        ///     For example, if a segment rule defines a segment be used from sizes 2" to 12", and there are three elbows rules
        ///     defined to be used from ranges
        ///     2"-4", 4"-7", and 9"-14", this method will return warning information specifying the sizes (8", 8.5", etc...) not
        ///     covered by elbow fittings.
        /// </summary>
        private XElement GetSegmentRangeNotCoveredWarning(RoutingPreferenceManager routingPreferenceManager,
            RoutingPreferenceRuleGroupType groupType)
        {
            for (var index = 0;
                 index != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Segments);
                 ++index)
            {
                var rule = routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Segments, index);
                if (rule.MEPPartId == ElementId.InvalidElementId)
                    continue;

                if (rule.NumberOfCriteria == 0) //double check all/none
                    continue;

                var psc = rule.GetCriterion(0) as PrimarySizeCriterion;

                var segment = m_document.GetElement(rule.MEPPartId) as PipeSegment;
                var sizesNotCovered = new List<double>();
                var isCovered = CheckSegmentForValidCoverage(routingPreferenceManager, psc.MinimumSize, psc.MaximumSize,
                    rule.MEPPartId, groupType, sizesNotCovered);
                if (!isCovered)
                {
                    var xSegmentNotCovered = new XElement(XName.Get("SegmentRangeNotCovered"));
                    xSegmentNotCovered.Add(new XAttribute(XName.Get("name"), segment.Name));
                    var sBuilder = new StringBuilder();

                    foreach (var size in sizesNotCovered)
                    {
                        var roundedSize = Convert.ConvertValueDocumentUnits(size, m_document);

                        sBuilder.Append(roundedSize + " ");
                    }

                    sBuilder.Remove(sBuilder.Length - 1, 1);
                    xSegmentNotCovered.Add(new XAttribute(XName.Get("sizes"), sBuilder.ToString()));
                    xSegmentNotCovered.Add(new XAttribute(XName.Get("unit"),
                        m_document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize).GetUnitTypeId().TypeId));
                    xSegmentNotCovered.Add(new XAttribute(XName.Get("groupType"), groupType.ToString()));

                    return xSegmentNotCovered;
                }
            }

            return null;
        }

        private bool CheckSegmentForValidCoverage(RoutingPreferenceManager routingPreferenceManager, double lowerBound,
            double upperBound, ElementId segmentId, RoutingPreferenceRuleGroupType groupType,
            List<double> sizesNotCovered)
        {
            var retval = true;
            if (segmentId == ElementId.InvalidElementId)
                throw new Exception("Invalid segment ElementId");

            var segment = m_document.GetElement(segmentId) as PipeSegment;
            foreach (var size in segment.GetSizes())
            {
                //skip sizes outside of rp bounds
                if (size.NominalDiameter < lowerBound)
                    continue;
                if (size.NominalDiameter > upperBound)
                    break;

                var conditions = new RoutingConditions(RoutingPreferenceErrorLevel.None);
                conditions.AppendCondition(new RoutingCondition(size.NominalDiameter));
                var foundFitting = routingPreferenceManager.GetMEPPartId(groupType, conditions);
                if (foundFitting == ElementId.InvalidElementId)
                {
                    sizesNotCovered.Add(size.NominalDiameter);
                    retval = false;
                }
            }

            return retval;
        }

        private bool IsRuleSetToRangeNone(RoutingPreferenceManager routingPreferenceManager,
            RoutingPreferenceRuleGroupType groupType, int index)
        {
            if (routingPreferenceManager.GetNumberOfRules(groupType) == 0) return false;

            var rule = routingPreferenceManager.GetRule(groupType, index);
            if (rule.NumberOfCriteria == 0) return false;

            var psc = rule.GetCriterion(0) as PrimarySizeCriterion;
            return psc.IsEqual(PrimarySizeCriterion.None());
        }

        private bool IsGroupSetToRangeNone(RoutingPreferenceManager routingPreferenceManager,
            RoutingPreferenceRuleGroupType groupType)
        {
            var retval = true;

            if (routingPreferenceManager.GetNumberOfRules(groupType) == 0) return false;
            for (var index = 0; index != routingPreferenceManager.GetNumberOfRules(groupType); ++index)
                if (!IsRuleSetToRangeNone(routingPreferenceManager, groupType, index))
                    retval = false;

            return retval;
        }

        /// <summary>
        ///     Check to see if the routing preferences specify a preferred junction type but do not have any
        ///     rules with valid fittings for that type (e.g, "Tee" is the preferred junction type, but only "Tap" fittings
        ///     are specified in junction rules.)
        /// </summary>
        /// <param name="routingPreferenceManager"></param>
        /// <returns></returns>
        private bool IsPreferredJunctionTypeValid(RoutingPreferenceManager routingPreferenceManager)
        {
            var preferredJunctionType = routingPreferenceManager.PreferredJunctionType;

            if (routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions) == 0)
                return false;

            var teeDefined = false;
            var tapDefined = false;
            for (var index = 0;
                 index != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                 ++index)
            {
                var rule = routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, index);
                if (rule.MEPPartId == ElementId.InvalidElementId)
                    continue;

                var familySymbol = m_document.GetElement(rule.MEPPartId) as FamilySymbol;

                var paramPartType = familySymbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                if (paramPartType == null)
                    throw new Exception("Null partType parameter.");

                var partType = (PartType)paramPartType.AsInteger();

                switch (partType)
                {
                    case PartType.Tee:
                        teeDefined = true;
                        break;
                    case PartType.TapAdjustable:
                    case PartType.TapPerpendicular:
                    case PartType.SpudPerpendicular:
                    case PartType.SpudAdjustable:
                        tapDefined = true;
                        break;
                }
            }

            switch (preferredJunctionType)
            {
                case PreferredJunctionType.Tap when !tapDefined:
                case PreferredJunctionType.Tee when !teeDefined:
                    return false;
                default:
                    return true;
            }
        }

        private string GetFittingName(ElementId id)
        {
            if (id == ElementId.InvalidElementId)
                throw new Exception("Invalid ElementId");
            var symbol = m_document.GetElement(id) as FamilySymbol;
            return symbol.Family.Name + " " + symbol.Name;
        }

        private string GetSegmentName(ElementId id)
        {
            if (id == ElementId.InvalidElementId)
                throw new Exception("Invalid ElementId");
            var segment = m_document.GetElement(id) as PipeSegment;
            return segment.Name;
        }

        /// <summary>
        ///     Using routing preferences, display found segment and fitting info from the size and pipe type specified in the
        ///     dialog.
        /// </summary>
        private List<PartIdInfo> GetPreferredFittingsAndSegments()
        {
            var partIdInfoList = new List<PartIdInfo>();

            var conditions = new RoutingConditions(RoutingPreferenceErrorLevel.None);

            conditions.AppendCondition(new RoutingCondition(m_mepSize));
            foreach (RoutingPreferenceRuleGroupType groupType in Enum.GetValues(typeof(RoutingPreferenceRuleGroupType)))
            {
                if (groupType == RoutingPreferenceRuleGroupType.Undefined)
                    continue;

                IList<ElementId> preferredTypes = new List<ElementId>();
                var preferredType = m_routingPreferenceManager.GetMEPPartId(groupType, conditions);
                //GetMEPPartId is the main "query" method of the
                //routing preferences API that evaluates conditions and criteria and returns segment and fitting elementIds that meet
                //those criteria.

                if (groupType != RoutingPreferenceRuleGroupType.Segments)
                    preferredTypes.Add(preferredType);
                else //Get all segments that support a given size, not just the first segment.
                    preferredTypes = m_routingPreferenceManager.GetSharedSizes(m_mepSize, ConnectorProfileType.Round);
                partIdInfoList.Add(new PartIdInfo(groupType,
                    preferredTypes)); //Collect a PartIdInfo object for each group type.
            }

            return partIdInfoList;
        }
    }
}
