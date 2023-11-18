// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Xml.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceAnalysis
{
    /// <summary>
    ///     This class contains a routing preference rule group and list of elementIds that correspond
    ///     to found segments and fittings that meet criteria specified through a routing preference manager.
    /// </summary>
    public class PartIdInfo
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="groupType"></param>
        /// <param name="ids"></param>
        public PartIdInfo(RoutingPreferenceRuleGroupType groupType, IList<ElementId> ids)
        {
            Id = new List<ElementId>();
            GroupType = groupType;
            Id.AddRange(ids);
        }
        // List of part Ids
        // group type

        /// <summary>
        ///     Id
        /// </summary>
        public List<ElementId> Id { get; }

        /// <summary>
        ///     Group type
        /// </summary>
        public RoutingPreferenceRuleGroupType GroupType { get; }

        /// <summary>
        ///     Build XML information of document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public XElement GetXml(Document document)
        {
            var xPartInfo = new XElement(XName.Get("PartInfo"));
            xPartInfo.Add(new XAttribute(XName.Get("groupType"), GroupType.ToString()));
            xPartInfo.Add(new XAttribute(XName.Get("partNames"), GetFittingNames(document)));
            return xPartInfo;
        }

        private string GetFittingName(Document document, ElementId id)
        {
            var fittingName = " None ";

            if (id != ElementId.InvalidElementId)
            {
                var element = document.GetElement(id);
                if (element is Segment)
                {
                    fittingName = element.Name;
                }
                else
                {
                    var familySymbol = element as FamilySymbol;
                    fittingName = $"{familySymbol.Family.Name} {familySymbol.Name}";
                }
            }

            return fittingName;
        }

        /// <summary>
        ///     Fitting name
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private string GetFittingNames(Document document)
        {
            var fittingNames = "";

            if (Id.Count == 0) fittingNames += "None -1";
            foreach (var id in Id)
            {
                fittingNames += $"{GetFittingName(document, id)} {id}, ";
            }

            return fittingNames.Remove(fittingNames.Length - 2, 2);
        }
    }
}
