// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace RevitMultiSample.RoutingPreferenceTools.CS
{
    /// <summary>
    ///     Class to read and write XML and routing preference data
    /// </summary>
    public class RoutingPreferenceBuilder
    {
        private readonly Document m_document;
        private IEnumerable<FamilySymbol> m_fittings;
        private IEnumerable<Material> m_materials;
        private IEnumerable<PipeScheduleType> m_pipeSchedules;
        private IEnumerable<PipeType> m_pipeTypes;
        private IEnumerable<Segment> m_segments;

        /// <summary>
        ///     Create an instance of the class and initialize lists of all segments, fittings, materials, schedules, and pipe
        ///     types in the document.
        /// </summary>
        public RoutingPreferenceBuilder(Document document)
        {
            m_document = document;
            m_segments = GetAllPipeSegments(m_document);
            m_fittings = GetAllFittings(m_document);
            m_materials = GetAllMaterials(m_document);
            m_pipeSchedules = GetAllPipeScheduleTypes(m_document);
            m_pipeTypes = GetAllPipeTypes(m_document);
        }

        /// <summary>
        ///     Reads data from an Xml source and loads pipe fitting families, creates segments, sizes, schedules, and routing
        ///     preference rules from the xml data.
        /// </summary>
        /// <param name="xDoc">The Xml data source to read from</param>
        public void ParseAllPipingPoliciesFromXml(XDocument xDoc)
        {
            if (!m_pipeTypes.Any())
                throw new RoutingPreferenceDataException(
                    "No pipe pipes defined in this project.  At least one must be defined.");

            var formatOptionPipeSize = m_document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize);

            var docPipeSizeUnit = formatOptionPipeSize.GetUnitTypeId().TypeId;
            var xmlPipeSizeUnit = xDoc.Root.Attribute("pipeSizeUnits").Value;
            if (docPipeSizeUnit != xmlPipeSizeUnit)
                throw new RoutingPreferenceDataException("Units from XML do not match current pipe size units.");

            var formatOptionRoughness = m_document.GetUnits().GetFormatOptions(SpecTypeId.PipingRoughness);

            var docRoughnessUnit = formatOptionRoughness.GetUnitTypeId().TypeId;
            var xmlRoughnessUnit = xDoc.Root.Attribute("pipeRoughnessUnits").Value;
            if (docRoughnessUnit != xmlRoughnessUnit)
                throw new RoutingPreferenceDataException("Units from XML do not match current pipe roughness units.");

            var loadFamilies = new Transaction(m_document, "Load Families");
            loadFamilies.Start();
            var families = xDoc.Root.Elements("Family");
            var findFolderUtility = new FindFolderUtility(m_document.Application);

            foreach (var xfamily in families)
                try
                {
                    ParseFamilyFromXml(xfamily, findFolderUtility); //Load families.
                }
                catch (Exception ex)
                {
                    loadFamilies.RollBack();
                    throw ex;
                }

            loadFamilies.Commit();

            var addPipeTypes = new Transaction(m_document, "Add PipeTypes");
            addPipeTypes.Start();
            var pipeTypes = xDoc.Root.Elements("PipeType");
            foreach (var xpipeType in pipeTypes)
                try
                {
                    ParsePipeTypeFromXml(xpipeType); //Define new pipe types.
                }
                catch (Exception ex)
                {
                    addPipeTypes.RollBack();
                    throw ex;
                }

            addPipeTypes.Commit();

            var addPipeSchedules = new Transaction(m_document, "Add Pipe Schedule Types");
            addPipeSchedules.Start();
            var pipeScheduleTypes = xDoc.Root.Elements("PipeScheduleType");
            foreach (var xpipeScheduleType in pipeScheduleTypes)
                try
                {
                    ParsePipeScheduleTypeFromXml(xpipeScheduleType); //Define new pipe schedule types.
                }
                catch (Exception ex)
                {
                    addPipeSchedules.RollBack();
                    throw ex;
                }

            addPipeSchedules.Commit();

            //The code above have added some new pipe types, schedules, or fittings, so update the lists of all of these.
            UpdatePipeTypesList();
            UpdatePipeTypeSchedulesList();
            UpdateFittingsList();

            new Transaction(m_document, "Add Pipe Segments");
            addPipeSchedules.Start();
            var pipeSegments = xDoc.Root.Elements("PipeSegment"); //Define new segments.
            foreach (var xpipeSegment in pipeSegments)
                try
                {
                    ParsePipeSegmentFromXml(xpipeSegment);
                }
                catch (Exception ex)
                {
                    addPipeSchedules.RollBack();
                    throw ex;
                }

            addPipeSchedules.Commit();

            UpdateSegmentsList(); //More segments may have been created, so update the segment list.

            //Now that all of the various types that routing preferences use have been created or loaded, add all the routing preferences.
            var addRoutingPreferences = new Transaction(m_document, "Add Routing Preferences");
            addRoutingPreferences.Start();
            var routingPreferenceManagers = xDoc.Root.Elements("RoutingPreferenceManager");
            foreach (var xroutingPreferenceManager in routingPreferenceManagers)
                try
                {
                    ParseRoutingPreferenceManagerFromXml(xroutingPreferenceManager);
                }
                catch (Exception ex)
                {
                    addRoutingPreferences.RollBack();
                    throw ex;
                }

            addRoutingPreferences.Commit();
        }

        /// <summary>
        ///     Reads pipe fitting family, segment, size, schedule, and routing preference data from a document and summarizes it
        ///     in Xml.
        /// </summary>
        /// <returns>An XDocument containing an Xml summary of routing preference information</returns>
        public XDocument CreateXmlFromAllPipingPolicies(ref bool pathsNotFound)
        {
            //To export the full path name of all .rfa family files, use the FindFolderUtility class.
            var findFolderUtility = new FindFolderUtility(m_document.Application);

            var routingPreferenceBuilderDoc = new XDocument();
            var xroot = new XElement(XName.Get("RoutingPreferenceBuilder"));

            var formatOptionPipeSize = m_document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize);
            var unitStringPipeSize = formatOptionPipeSize.GetUnitTypeId().TypeId;
            xroot.Add(new XAttribute(XName.Get("pipeSizeUnits"), unitStringPipeSize));

            var formatOptionRoughness = m_document.GetUnits().GetFormatOptions(SpecTypeId.PipingRoughness);
            var unitStringRoughness = formatOptionRoughness.GetUnitTypeId().TypeId;
            xroot.Add(new XAttribute(XName.Get("pipeRoughnessUnits"), unitStringRoughness));

            foreach (var familySymbol in m_fittings)
                xroot.Add(CreateXmlFromFamily(familySymbol, findFolderUtility, ref pathsNotFound));

            foreach (var pipeType in m_pipeTypes) xroot.Add(CreateXmlFromPipeType(pipeType));

            foreach (var pipeScheduleType in m_pipeSchedules)
                xroot.Add(CreateXmlFromPipeScheduleType(pipeScheduleType));

            foreach (PipeSegment pipeSegment in m_segments) xroot.Add(CreateXmlFromPipeSegment(pipeSegment));

            foreach (var pipeType in m_pipeTypes)
                xroot.Add(CreateXmlFromRoutingPreferenceManager(pipeType.RoutingPreferenceManager));

            routingPreferenceBuilderDoc.Add(xroot);
            return routingPreferenceBuilderDoc;
        }

        /// <summary>
        ///     Load a family from xml
        /// </summary>
        /// <param name="familyXElement"></param>
        /// <param name="findFolderUtility"></param>
        private void ParseFamilyFromXml(XElement familyXElement, FindFolderUtility findFolderUtility)
        {
            var xafilename = familyXElement.Attribute(XName.Get("filename"));
            var familyPath = xafilename.Value;
            if (!File.Exists(familyPath))
            {
                var filename = Path.GetFileName(familyPath);
                familyPath = findFolderUtility.FindFileFolder(filename);
                if (!File.Exists(familyPath))
                    throw new RoutingPreferenceDataException("Cannot find family file: " + xafilename.Value);
            }

            if (string.Compare(Path.GetExtension(familyPath), ".rfa", true) != 0)
                throw new RoutingPreferenceDataException(familyPath + " is not a family file.");

            try
            {
                if (!m_document.LoadFamily(familyPath))
                    return; //returns false if already loaded.
            }
            catch (Exception ex)
            {
                throw new RoutingPreferenceDataException("Cannot load family: " + xafilename.Value + ": " + ex);
            }
        }

        /// <summary>
        ///     Create xml from a family
        /// </summary>
        /// <param name="pipeFitting"></param>
        /// <param name="findFolderUtility"></param>
        /// <param name="pathNotFound"></param>
        /// <returns></returns>
        private static XElement CreateXmlFromFamily(FamilySymbol pipeFitting, FindFolderUtility findFolderUtility,
            ref bool pathNotFound)
        {
            //Try to find the path of the .rfa file.
            var path = findFolderUtility.FindFileFolder(pipeFitting.Family.Name + ".rfa");
            string pathToWrite;
            if (path == "")
            {
                pathNotFound = true;
                pathToWrite = pipeFitting.Family.Name + ".rfa";
            }
            else
            {
                pathToWrite = path;
            }

            var xFamilySymbol = new XElement(XName.Get("Family"));
            xFamilySymbol.Add(new XAttribute(XName.Get("filename"), pathToWrite));
            return xFamilySymbol;
        }

        /// <summary>
        ///     Greate a PipeType from xml
        /// </summary>
        /// <param name="pipetypeXElement"></param>
        private void ParsePipeTypeFromXml(XElement pipetypeXElement)
        {
            var xaName = pipetypeXElement.Attribute(XName.Get("name"));

            var pipeTypeId = GetPipeTypeByName(xaName.Value);

            if (pipeTypeId == ElementId.InvalidElementId) //If the pipe type does not exist, create it.
            {
                var newPipeType = m_pipeTypes.First().Duplicate(xaName.Value) as PipeType;
                ClearRoutingPreferenceRules(newPipeType);
            }
        }

        /// <summary>
        ///     Clear all routing preferences in a PipeType
        /// </summary>
        /// <param name="pipeType"></param>
        private static void ClearRoutingPreferenceRules(PipeType pipeType)
        {
            foreach (RoutingPreferenceRuleGroupType group in Enum.GetValues(typeof(RoutingPreferenceRuleGroupType)))
            {
                var ruleCount = pipeType.RoutingPreferenceManager.GetNumberOfRules(group);
                for (var index = 0; index != ruleCount; ++index) pipeType.RoutingPreferenceManager.RemoveRule(group, 0);
            }
        }

        /// <summary>
        ///     Create Xml from a PipeType
        /// </summary>
        /// <param name="pipeType"></param>
        /// <returns></returns>
        private static XElement CreateXmlFromPipeType(PipeType pipeType)
        {
            var xPipeType = new XElement(XName.Get("PipeType"));
            xPipeType.Add(new XAttribute(XName.Get("name"), pipeType.Name));
            return xPipeType;
        }

        private void ParsePipeScheduleTypeFromXml(XElement pipeScheduleTypeXElement)
        {
            var xaName = pipeScheduleTypeXElement.Attribute(XName.Get("name"));
            var pipeScheduleTypeId = GetPipeScheduleTypeByName(xaName.Value);
            if (pipeScheduleTypeId == ElementId.InvalidElementId) //If the pipe schedule type does not exist, create it.
                m_pipeSchedules.First().Duplicate(xaName.Value);
        }

        /// <summary>
        ///     Create Xml from a PipeScheduleType
        /// </summary>
        /// <param name="pipeScheduleType"></param>
        /// <returns></returns>
        private static XElement CreateXmlFromPipeScheduleType(PipeScheduleType pipeScheduleType)
        {
            var xPipeSchedule = new XElement(XName.Get("PipeScheduleType"));
            xPipeSchedule.Add(new XAttribute(XName.Get("name"), pipeScheduleType.Name));
            return xPipeSchedule;
        }

        /// <summary>
        ///     Create a PipeSegment from XML
        /// </summary>
        /// <param name="segmentXElement"></param>
        private void ParsePipeSegmentFromXml(XElement segmentXElement)
        {
            var xaMaterial = segmentXElement.Attribute(XName.Get("materialName"));
            var xaSchedule = segmentXElement.Attribute(XName.Get("pipeScheduleTypeName"));
            var xaRoughness = segmentXElement.Attribute(XName.Get("roughness"));

            var materialId =
                GetMaterialByName(xaMaterial
                    .Value); //There is nothing in the xml schema for creating new materials -- any material specified must already exist in the document.
            if (materialId == ElementId.InvalidElementId)
                throw new RoutingPreferenceDataException("Cannot find Material: " + xaMaterial.Value + " in: " +
                                                         segmentXElement);
            var scheduleId = GetPipeScheduleTypeByName(xaSchedule.Value);

            double roughness;
            var r1 = double.TryParse(xaRoughness.Value, out roughness);

            if (!r1)
                throw new RoutingPreferenceDataException("Invalid roughness value: " + xaRoughness.Value + " in: " +
                                                         segmentXElement);

            if (roughness <= 0)
                throw new RoutingPreferenceDataException("Invalid roughness value: " + xaRoughness.Value + " in: " +
                                                         segmentXElement);

            if (scheduleId == ElementId.InvalidElementId)
                throw new RoutingPreferenceDataException("Cannot find Schedule: " + xaSchedule.Value + " in: " +
                                                         segmentXElement); //we will not create new schedules.

            var existingPipeSegmentId = GetSegmentByIds(materialId, scheduleId);
            if (existingPipeSegmentId != ElementId.InvalidElementId)
                return; //Segment found, no need to create.

            ICollection<MEPSize> sizes = new List<MEPSize>();
            foreach (var sizeNode in segmentXElement.Nodes())
                if (sizeNode is XElement node)
                {
                    var newSize = ParseMepSizeFromXml(node, m_document);
                    sizes.Add(newSize);
                }

            var pipeSegment = PipeSegment.Create(m_document, materialId, scheduleId, sizes);
            pipeSegment.Roughness = Convert.ConvertValueToFeet(roughness, m_document);
        }

        /// <summary>
        ///     Create Xml from a PipeSegment
        /// </summary>
        /// <param name="pipeSegment"></param>
        /// <returns></returns>
        private XElement CreateXmlFromPipeSegment(PipeSegment pipeSegment)
        {
            var xPipeSegment = new XElement(XName.Get("PipeSegment"));

            xPipeSegment.Add(new XAttribute(XName.Get("pipeScheduleTypeName"),
                GetPipeScheduleTypeNamebyId(pipeSegment.ScheduleTypeId)));
            xPipeSegment.Add(new XAttribute(XName.Get("materialName"), GetMaterialNameById(pipeSegment.MaterialId)));

            var roughnessInDocumentUnits = Convert.ConvertValueDocumentUnits(pipeSegment.Roughness, m_document);
            xPipeSegment.Add(new XAttribute(XName.Get("roughness"), roughnessInDocumentUnits.ToString("r")));

            foreach (var size in pipeSegment.GetSizes())
                xPipeSegment.Add(CreateXmlFromMepSize(size, m_document));

            return xPipeSegment;
        }

        /// <summary>
        ///     Create an MEPSize from Xml
        /// </summary>
        /// <param name="sizeXElement"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private static MEPSize ParseMepSizeFromXml(XElement sizeXElement, Document document)
        {
            var xaNominal = sizeXElement.Attribute(XName.Get("nominalDiameter"));
            var xaInner = sizeXElement.Attribute(XName.Get("innerDiameter"));
            var xaOuter = sizeXElement.Attribute(XName.Get("outerDiameter"));
            var xaUsedInSizeLists = sizeXElement.Attribute(XName.Get("usedInSizeLists"));
            var xaUsedInSizing = sizeXElement.Attribute(XName.Get("usedInSizing"));

            double nominal, inner, outer;
            bool usedInSizeLists, usedInSizing;
            var r1 = double.TryParse(xaNominal.Value, out nominal);
            var r2 = double.TryParse(xaInner.Value, out inner);
            var r3 = double.TryParse(xaOuter.Value, out outer);
            var r4 = bool.TryParse(xaUsedInSizeLists.Value, out usedInSizeLists);
            var r5 = bool.TryParse(xaUsedInSizing.Value, out usedInSizing);

            if (!r1 || !r2 || !r3 || !r4 || !r5)
                throw new RoutingPreferenceDataException("Cannot parse MEPSize attributes:" + xaNominal.Value + ", " +
                                                         xaInner.Value + ", " + xaOuter.Value + ", " +
                                                         xaUsedInSizeLists.Value + ", " + xaUsedInSizing.Value);

            MEPSize newSize = null;

            try
            {
                newSize = new MEPSize(Convert.ConvertValueToFeet(nominal, document),
                    Convert.ConvertValueToFeet(inner, document), Convert.ConvertValueToFeet(outer, document),
                    usedInSizeLists, usedInSizing);
            }

            catch (Exception)
            {
                throw new RoutingPreferenceDataException("Invalid MEPSize values: " + nominal + ", " + inner + ", " +
                                                         outer);
            }

            return newSize;
        }

        /// <summary>
        ///     Create Xml from an MEPSize
        /// </summary>
        /// <param name="size"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private static XElement CreateXmlFromMepSize(MEPSize size, Document document)
        {
            var xMepSize = new XElement(XName.Get("MEPSize"));

            xMepSize.Add(new XAttribute(XName.Get("innerDiameter"),
                Convert.ConvertValueDocumentUnits(size.InnerDiameter, document).ToString()));
            xMepSize.Add(new XAttribute(XName.Get("nominalDiameter"),
                Convert.ConvertValueDocumentUnits(size.NominalDiameter, document).ToString()));
            xMepSize.Add(new XAttribute(XName.Get("outerDiameter"),
                Convert.ConvertValueDocumentUnits(size.OuterDiameter, document).ToString()));
            xMepSize.Add(new XAttribute(XName.Get("usedInSizeLists"), size.UsedInSizeLists));
            xMepSize.Add(new XAttribute(XName.Get("usedInSizing"), size.UsedInSizing));
            return xMepSize;
        }

        /// <summary>
        ///     Populate a routing preference manager from Xml
        /// </summary>
        /// <param name="routingPreferenceManagerXElement"></param>
        private void ParseRoutingPreferenceManagerFromXml(XElement routingPreferenceManagerXElement)
        {
            var xaPipeTypeName = routingPreferenceManagerXElement.Attribute(XName.Get("pipeTypeName"));
            var xaPreferredJunctionType =
                routingPreferenceManagerXElement.Attribute(XName.Get("preferredJunctionType"));

            PreferredJunctionType preferredJunctionType;
            var r1 = Enum.TryParse(xaPreferredJunctionType.Value, out preferredJunctionType);

            if (!r1)
                throw new RoutingPreferenceDataException("Invalid Preferred Junction Type in: " +
                                                         routingPreferenceManagerXElement);

            var pipeTypeId = GetPipeTypeByName(xaPipeTypeName.Value);
            if (pipeTypeId == ElementId.InvalidElementId)
                throw new RoutingPreferenceDataException("Could not find pipe type element in: " +
                                                         routingPreferenceManagerXElement);

            var pipeType = m_document.GetElement(pipeTypeId) as PipeType;

            var routingPreferenceManager = pipeType.RoutingPreferenceManager;
            routingPreferenceManager.PreferredJunctionType = preferredJunctionType;

            foreach (var xRule in routingPreferenceManagerXElement.Nodes())
                if (xRule is XElement element)
                {
                    RoutingPreferenceRuleGroupType groupType;
                    var rule = ParseRoutingPreferenceRuleFromXml(element, out groupType);
                    routingPreferenceManager.AddRule(groupType, rule);
                }
        }

        /// <summary>
        ///     Create Xml from a RoutingPreferenceManager
        /// </summary>
        /// <param name="routingPreferenceManager"></param>
        /// <returns></returns>
        private XElement CreateXmlFromRoutingPreferenceManager(RoutingPreferenceManager routingPreferenceManager)
        {
            var xRoutingPreferenceManager = new XElement(XName.Get("RoutingPreferenceManager"));

            xRoutingPreferenceManager.Add(new XAttribute(XName.Get("pipeTypeName"),
                GetPipeTypeNameById(routingPreferenceManager.OwnerId)));

            xRoutingPreferenceManager.Add(new XAttribute(XName.Get("preferredJunctionType"),
                routingPreferenceManager.PreferredJunctionType.ToString()));

            for (var indexCrosses = 0;
                 indexCrosses != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Crosses);
                 indexCrosses++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Crosses, indexCrosses),
                    RoutingPreferenceRuleGroupType.Crosses));

            for (var indexElbows = 0;
                 indexElbows != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Elbows);
                 indexElbows++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Elbows, indexElbows),
                    RoutingPreferenceRuleGroupType.Elbows));

            for (var indexSegments = 0;
                 indexSegments != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Segments);
                 indexSegments++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Segments, indexSegments),
                    RoutingPreferenceRuleGroupType.Segments));

            for (var indexJunctions = 0;
                 indexJunctions != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                 indexJunctions++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, indexJunctions),
                    RoutingPreferenceRuleGroupType.Junctions));

            for (var indexTransitions = 0;
                 indexTransitions !=
                 routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Transitions);
                 indexTransitions++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Transitions, indexTransitions),
                    RoutingPreferenceRuleGroupType.Transitions));

            for (var indexUnions = 0;
                 indexUnions != routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Unions);
                 indexUnions++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Unions, indexUnions),
                    RoutingPreferenceRuleGroupType.Unions));

            for (var indexMechanicalJoints = 0;
                 indexMechanicalJoints !=
                 routingPreferenceManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.MechanicalJoints);
                 indexMechanicalJoints++)
                xRoutingPreferenceManager.Add(CreateXmlFromRoutingPreferenceRule(
                    routingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.MechanicalJoints,
                        indexMechanicalJoints), RoutingPreferenceRuleGroupType.MechanicalJoints));

            return xRoutingPreferenceManager;
        }

        /// <summary>
        ///     Create a RoutingPreferenceRule from Xml
        /// </summary>
        /// <param name="ruleXElement"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        private RoutingPreferenceRule ParseRoutingPreferenceRuleFromXml(XElement ruleXElement,
            out RoutingPreferenceRuleGroupType groupType)
        {
            XAttribute xaMaxSize = null;

            var xaDescription = ruleXElement.Attribute(XName.Get("description"));
            var xaPartName = ruleXElement.Attribute(XName.Get("partName"));
            var xaGroup = ruleXElement.Attribute(XName.Get("ruleGroup"));
            var xaMinSize = ruleXElement.Attribute(XName.Get("minimumSize"));

            ElementId partId;

            var r3 = Enum.TryParse(xaGroup.Value, out groupType);
            if (!r3)
                throw new RoutingPreferenceDataException("Could not parse rule group type: " + xaGroup.Value);

            var description = xaDescription.Value;

            if (groupType == RoutingPreferenceRuleGroupType.Segments)
                partId = GetSegmentByName(xaPartName.Value);
            else
                partId = GetFittingByName(xaPartName.Value);

            if (partId == ElementId.InvalidElementId)
                throw new RoutingPreferenceDataException("Could not find MEP Part: " + xaPartName.Value +
                                                         ".  Is this the correct family name, and is the correct family loaded?");

            var rule = new RoutingPreferenceRule(partId, description);

            PrimarySizeCriterion sizeCriterion;
            if (string.Compare(xaMinSize.Value, "All", true) ==
                0) //If "All" or "None" are specified, set min and max values to documented "Max" values.
            {
                sizeCriterion = PrimarySizeCriterion.All();
            }
            else if (string.Compare(xaMinSize.Value, "None", true) == 0)
            {
                sizeCriterion = PrimarySizeCriterion.None();
            }
            else // "maximumSize" attribute is only needed if not specifying "All" or "None."
            {
                try
                {
                    xaMaxSize = ruleXElement.Attribute(XName.Get("maximumSize"));
                }
                catch (Exception)
                {
                    throw new RoutingPreferenceDataException("Cannot get maximumSize attribute in: " + ruleXElement);
                }

                double min, max;
                var r1 = double.TryParse(xaMinSize.Value, out min);
                var r2 = double.TryParse(xaMaxSize.Value, out max);
                if (!r1 || !r2)
                    throw new RoutingPreferenceDataException("Could not parse size values: " + xaMinSize.Value + ", " +
                                                             xaMaxSize.Value);
                if (min > max)
                    throw new RoutingPreferenceDataException("Invalid size range.");

                min = Convert.ConvertValueToFeet(min, m_document);
                max = Convert.ConvertValueToFeet(max, m_document);
                sizeCriterion = new PrimarySizeCriterion(min, max);
            }

            rule.AddCriterion(sizeCriterion);

            return rule;
        }

        /// <summary>
        ///     Create Xml from a RoutingPreferenceRule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        private XElement CreateXmlFromRoutingPreferenceRule(RoutingPreferenceRule rule,
            RoutingPreferenceRuleGroupType groupType)
        {
            var xRoutingPreferenceRule = new XElement(XName.Get("RoutingPreferenceRule"));
            xRoutingPreferenceRule.Add(new XAttribute(XName.Get("description"), rule.Description));
            xRoutingPreferenceRule.Add(new XAttribute(XName.Get("ruleGroup"), groupType.ToString()));
            if (rule.NumberOfCriteria >= 1)
            {
                var psc = rule.GetCriterion(0) as PrimarySizeCriterion;

                if (psc.IsEqual(PrimarySizeCriterion.All()))
                {
                    xRoutingPreferenceRule.Add(new XAttribute(XName.Get("minimumSize"), "All"));
                }
                else if (psc.IsEqual(PrimarySizeCriterion.None()))
                {
                    xRoutingPreferenceRule.Add(new XAttribute(XName.Get("minimumSize"), "None"));
                }
                else //Only specify "maximumSize" if not specifying "All" or "None" for minimum size, just like in the UI.
                {
                    xRoutingPreferenceRule.Add(new XAttribute(XName.Get("minimumSize"),
                        Convert.ConvertValueDocumentUnits(psc.MinimumSize, m_document).ToString()));
                    xRoutingPreferenceRule.Add(new XAttribute(XName.Get("maximumSize"),
                        Convert.ConvertValueDocumentUnits(psc.MaximumSize, m_document).ToString()));
                }
            }
            else
            {
                xRoutingPreferenceRule.Add(new XAttribute(XName.Get("minimumSize"), "All"));
            }

            if (groupType == RoutingPreferenceRuleGroupType.Segments)
                xRoutingPreferenceRule.Add(new XAttribute(XName.Get("partName"), GetSegmentNameById(rule.MEPPartId)));
            else
                xRoutingPreferenceRule.Add(new XAttribute(XName.Get("partName"), GetFittingNameById(rule.MEPPartId)));

            return xRoutingPreferenceRule;
        }

        /// <summary>
        ///     Get PipeScheduleTypeName by Id
        /// </summary>
        /// <param name="pipescheduleTypeId"></param>
        /// <returns></returns>
        private string GetPipeScheduleTypeNamebyId(ElementId pipescheduleTypeId)
        {
            return m_document.GetElement(pipescheduleTypeId).Name;
        }

        /// <summary>
        ///     Get material name by Id
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        private string GetMaterialNameById(ElementId materialId)
        {
            return m_document.GetElement(materialId).Name;
        }

        /// <summary>
        ///     Get segment name by Id
        /// </summary>
        /// <param name="segmentId"></param>
        /// <returns></returns>
        private string GetSegmentNameById(ElementId segmentId)
        {
            return m_document.GetElement(segmentId).Name;
        }

        /// <summary>
        ///     Get fitting name by Id
        /// </summary>
        /// <param name="fittingId"></param>
        /// <returns></returns>
        private string GetFittingNameById(ElementId fittingId)
        {
            var fs = m_document.GetElement(fittingId) as FamilySymbol;
            return fs.Family.Name + " " + fs.Name;
        }

        /// <summary>
        ///     Get segment by Ids
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="pipeScheduleTypeId"></param>
        /// <returns></returns>
        private ElementId GetSegmentByIds(ElementId materialId, ElementId pipeScheduleTypeId)
        {
            if (materialId == ElementId.InvalidElementId || pipeScheduleTypeId == ElementId.InvalidElementId)
                return ElementId.InvalidElementId;

            var material = m_document.GetElement(materialId);
            var pipeScheduleType = m_document.GetElement(pipeScheduleTypeId);
            var segmentName = material.Name + " - " + pipeScheduleType.Name;
            return GetSegmentByName(segmentName);
        }

        /// <summary>
        ///     Get pipe type name by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPipeTypeNameById(ElementId id)
        {
            return m_document.GetElement(id).Name;
        }

        /// <summary>
        ///     Get segment by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementId GetSegmentByName(string name)
        {
            foreach (var segment in m_segments)
                if (segment.Name == name)
                    return segment.Id;
            return ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Get fitting by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementId GetFittingByName(string name)
        {
            foreach (var fitting in m_fittings)
                if (fitting.Family.Name + " " + fitting.Name == name)

                    return fitting.Id;
            return ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Get material by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementId GetMaterialByName(string name)
        {
            foreach (var material in m_materials)
                if (material.Name == name)
                    return material.Id;
            return ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Get pipe schedule type by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementId GetPipeScheduleTypeByName(string name)
        {
            foreach (var pipeScheduleType in m_pipeSchedules)
                if (pipeScheduleType.Name == name)
                    return pipeScheduleType.Id;
            return ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Get pipe type by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElementId GetPipeTypeByName(string name)
        {
            foreach (var pipeType in m_pipeTypes)
                if (pipeType.Name == name)
                    return pipeType.Id;
            return ElementId.InvalidElementId;
        }

        /// <summary>
        ///     Update fittings list
        /// </summary>
        private void UpdateFittingsList()
        {
            m_fittings = GetAllFittings(m_document);
        }

        /// <summary>
        ///     Update segments list
        /// </summary>
        private void UpdateSegmentsList()
        {
            m_segments = GetAllPipeSegments(m_document);
        }

        /// <summary>
        ///     Update pipe types list
        /// </summary>
        private void UpdatePipeTypesList()
        {
            m_pipeTypes = GetAllPipeTypes(m_document);
        }

        /// <summary>
        ///     Update pipe type schedules list
        /// </summary>
        private void UpdatePipeTypeSchedulesList()
        {
            m_pipeSchedules = GetAllPipeScheduleTypes(m_document);
        }

        /// <summary>
        ///     Update materials list
        /// </summary>
        private void UpdateMaterialsList()
        {
            m_materials = GetAllMaterials(m_document);
        }

        /// <summary>
        ///     Get all pipe segments
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<PipeSegment> GetAllPipeSegments(Document document)
        {
            var fec = new FilteredElementCollector(document);
            fec.OfClass(typeof(PipeSegment));
            var segments = fec.ToElements().Cast<PipeSegment>();
            return segments;
        }

        /// <summary>
        ///     Get all fittings
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<FamilySymbol> GetAllFittings(Document document)
        {
            var fec = new FilteredElementCollector(document);
            fec.OfClass(typeof(FamilySymbol));
            fec.OfCategory(BuiltInCategory.OST_PipeFitting);
            var fittings = fec.ToElements().Cast<FamilySymbol>();
            return fittings;
        }

        /// <summary>
        ///     Get all materials
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private IEnumerable<Material> GetAllMaterials(Document document)
        {
            var fec = new FilteredElementCollector(document);
            fec.OfClass(typeof(Material));
            var materials = fec.ToElements().Cast<Material>();
            return materials;
        }

        /// <summary>
        ///     Get all pipe schedule types
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<PipeScheduleType> GetAllPipeScheduleTypes(Document document)
        {
            var fec = new FilteredElementCollector(document);
            fec.OfClass(typeof(PipeScheduleType));
            var pipeScheduleTypes = fec.ToElements().Cast<PipeScheduleType>();
            return pipeScheduleTypes;
        }

        /// <summary>
        ///     Get all pipe types
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<PipeType> GetAllPipeTypes(Document document)
        {
            var ecf = new ElementClassFilter(typeof(PipeType));

            var fec = new FilteredElementCollector(document);
            fec.WherePasses(ecf);
            var pipeTypes = fec.ToElements().Cast<PipeType>();
            return pipeTypes;
        }
    }
}
