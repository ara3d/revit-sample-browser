using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_MepElementShape sample.</summary>
    internal static partial class Util
    {
        #region MEP Element Shape Version 1

        internal static class MepElementShapeV1
        {
            private static RegexCache _regex_cache = new RegexCache();

            public static string GetElementShape(Element e)
            {
                if (MepElementShapeIsElementOfCategory(e,
                    BuiltInCategory.OST_DuctCurves))
                {
                    Debug.Assert(
                        1 == e.GetParameters("Size").Count,
                        "expected only one parameters named 'Size'");

                    var size = e.LookupParameter("Size")
                        .AsString();

                    if (size.Split('x').Length == 2)
                        return "rectangular";
                    if (size.Split('/').Length == 2)
                        return "oval";
                    return "round";
                }

                if (MepElementShapeIsElementOfCategory(e,
                    BuiltInCategory.OST_DuctFitting))
                    if (e is FamilyInstance {MEPModel: MechanicalFitting fitting})
                    {
                        var p = e.get_Parameter(
                            BuiltInParameter.RBS_CALCULATED_SIZE);

                        var size = p.AsString();

                        var partType = fitting.PartType;

                        if (partType is PartType.Elbow or PartType.Transition)
                        {
                            if (size.Split('x').Length == 3)
                                return "rectangular2rectangular";
                            if (size.Split('/').Length == 3)
                                return "oval2oval";
                            if (_regex_cache.Match(
                                "[0-9]+\"?x[0-9]+\"?-[0-9]+\"?/[0-9]+\"?", size))
                                return "rectangular2oval";
                            if (_regex_cache.Match(
                                "[0-9]+\"?/[0-9]+\"?-[0-9]+\"?x[0-9]+\"?", size))
                                return "oval2rectangular";
                            if (_regex_cache.Match(
                                "[0-9]+\"?[^0-9]-[0-9]+\"?x[0-9]+\"?", size))
                                return "round2rectangular";
                            if (_regex_cache.Match(
                                "[0-9]+\"?x[0-9]+\"?-[0-9]+\"?[^0-9]", size))
                                return "rectangular2round";
                            if (_regex_cache.Match(
                                "[0-9]+\"?[^0-9]-[0-9]+\"?/[0-9]+\"?", size))
                                return "round2oval";
                            if (_regex_cache.Match(
                                "[0-9]+\"?/[0-9]+\"?-[0-9]+\"?[^0-9]", size))
                                return "oval2round";
                            if (_regex_cache.Match(
                                "[0-9]+\"?[^0-9]-[0-9]+\"?[^0-9]", size))
                                return "round2round";
                            return "other case";
                        }
                    }

                return "unknown";
            }

            private class RegexCache : Dictionary<string, Regex>
            {
                public bool Match(string pattern, string input)
                {
                    if (!ContainsKey(pattern)) Add(pattern, new Regex(pattern));
                    return this[pattern].IsMatch(input);
                }
            }
        }

        #endregion // MEP Element Shape Version 1

        #region MEP Element Shape Version 2

        internal static class MepElementShapeVersion2
        {
            public static string GetElementShape(
                Element e,
                Element pe = null,
                Element ne = null)
            {
                if (MepElementShapeIsElementOfCategory(e,
                    BuiltInCategory.OST_DuctCurves))
                {
                    var cm = (e as MEPCurve)
                        .ConnectorManager;

                    foreach (Connector c in cm.Connectors)
                        return $"{c.Shape} 2 {c.Shape}";
                }
                else if (MepElementShapeIsElementOfCategory(e,
                    BuiltInCategory.OST_DuctFitting))
                {
                    var system
                        = MepElementShapeExtractMechanicalOrPipingSystem(e);

                    var fi = e as FamilyInstance;
                    var mm = fi.MEPModel;

                    var connectors
                        = mm.ConnectorManager.Connectors;

                    if (fi != null && mm is MechanicalFitting fitting)
                    {
                        var partType
                            = fitting.PartType;

                        if (PartType.Elbow == partType)
                        {
                            foreach (Connector c in connectors)
                                return $"{c.Shape} 2 {c.Shape}";
                        }
                        else if (PartType.Transition == partType)
                        {
                            var tmp = new string[2];

                            if (system != null)
                            {
                                foreach (Connector c in connectors)
                                {
                                    if (c.Direction == FlowDirectionType.In)
                                        tmp[0] = c.Shape.ToString();

                                    if (c.Direction == FlowDirectionType.Out)
                                        tmp[1] = c.Shape.ToString();
                                }

                                return string.Join(" 2 ", tmp);
                            }

                            var i = 0;

                            foreach (Connector c in connectors)
                            {
                                if (pe != null)
                                {
                                    if (MepElementShapeIsConnectedTo(c, pe))
                                        tmp[0] = c.Shape.ToString();
                                    else
                                        tmp[1] = c.Shape.ToString();
                                }
                                else
                                {
                                    tmp[i] = c.Shape.ToString();
                                }

                                ++i;
                            }

                            if (pe != null)
                                return string.Join(" 2 ", tmp);

                            return string.Join("-", tmp);
                        }
                        else if (partType is PartType.Tee or PartType.Cross or PartType.Pants or PartType.Wye)
                        {
                            string from, to;
                            from = to = null;
                            var unk = new List<string>();

                            if (system != null)
                            {
                                foreach (Connector c in connectors)
                                {
                                    if (c.Direction == FlowDirectionType.In)
                                        from = c.Shape.ToString();
                                    else
                                        unk.Add(c.Shape.ToString());

                                    if (ne != null && MepElementShapeIsConnectedTo(c, ne))
                                        to = c.Shape.ToString();
                                }

                                if (to != null)
                                    return $"{from} 2 {to}";

                                return $"{from} 2 {string.Join("-", unk.ToArray())}";
                            }

                            foreach (Connector c in connectors)
                            {
                                if (ne != null && MepElementShapeIsConnectedTo(
                                    c, ne))
                                {
                                    to = c.Shape.ToString();
                                    continue;
                                }

                                if (pe != null && MepElementShapeIsConnectedTo(
                                    c, pe))
                                {
                                    from = c.Shape.ToString();
                                    continue;
                                }

                                unk.Add(c.Shape.ToString());
                            }

                            if (to != null)
                                return $"{from} 2 {to}";

                            if (from != null)
                                return $"{from} 2 {string.Join("-", unk.ToArray())}";

                            return string.Join("-", unk.ToArray());
                        }
                    }
                }

                return "unknown";
            }

            public static bool MepElementShapeIsConnectedTo(
                Connector c,
                Element e)
            {
                var cm = e is FamilyInstance instance
                    ? instance.MEPModel.ConnectorManager
                    : (e as MEPCurve).ConnectorManager;

                foreach (Connector c2 in cm.Connectors)
                    if (c.IsConnectedTo(c2))
                        return true;
                return false;
            }

            public static MEPSystem MepElementShapeExtractMechanicalOrPipingSystem(
                Element selectedElement)
            {
                MEPSystem system = null;

                if (selectedElement is MEPSystem element)
                {
                    if (element is MechanicalSystem or PipingSystem)
                    {
                        system = element;
                        return system;
                    }
                }
                else
                {
                    if (selectedElement is FamilyInstance fi)
                    {
                        var mepModel = fi.MEPModel;
                        ConnectorSet connectors = null;
                        try
                        {
                            connectors = mepModel.ConnectorManager.Connectors;
                        }
                        catch (Exception)
                        {
                            system = null;
                        }

                        system = MepElementShapeExtractSystemFromConnectors(connectors);
                    }
                    else
                    {
                        if (selectedElement is MEPCurve mepCurve)
                        {
                            ConnectorSet connectors = null;
                            connectors = mepCurve.ConnectorManager.Connectors;
                            system = MepElementShapeExtractSystemFromConnectors(connectors);
                        }
                    }
                }

                return system;
            }

            public static MEPSystem MepElementShapeExtractSystemFromConnectors(ConnectorSet connectors)
            {
                MEPSystem system = null;

                if (connectors == null || connectors.Size == 0) return null;

                var systems = new List<MEPSystem>();
                foreach (Connector connector in connectors)
                {
                    var tmpSystem = connector.MEPSystem;
                    if (tmpSystem == null) continue;

                    if (tmpSystem is MechanicalSystem ms)
                    {
                        if (ms.IsWellConnected) systems.Add(tmpSystem);
                    }
                    else
                    {
                        if (tmpSystem is PipingSystem {IsWellConnected: true}) systems.Add(tmpSystem);
                    }
                }

                var countOfSystem = systems.Count;
                if (countOfSystem != 0)
                {
                    var countOfElements = 0;
                    foreach (var sys in systems)
                        if (sys.Elements.Size > countOfElements)
                        {
                            system = sys;
                            countOfElements = sys.Elements.Size;
                        }
                }

                return system;
            }
        }

        #endregion // MEP Element Shape Version 2

        #region MEP Element Shape Version 3

        internal static class MepElementShapeVersion3
        {
            private static bool HasInvalidElementIdValue(
                Element e,
                BuiltInParameter bip)
            {
                var p = e.get_Parameter(bip);

                return p is {StorageType: StorageType.ElementId} && ElementId.InvalidElementId == p.AsElementId();
            }

            public static string GetElementShape(
                Element e)
            {
                var shape = "unknown";

                var tid = e.GetTypeId();

                if (ElementId.InvalidElementId != tid)
                {
                    var doc = e.Document;

                    if (doc.GetElement(tid) is DuctType)
                    {
                        if (HasInvalidElementIdValue(e, BuiltInParameter
                            .RBS_CURVETYPE_MULTISHAPE_TRANSITION_OVALROUND_PARAM))
                            shape = "rectangular";
                        else if (HasInvalidElementIdValue(e, BuiltInParameter
                            .RBS_CURVETYPE_MULTISHAPE_TRANSITION_RECTOVAL_PARAM))
                            shape = "round";
                        else if (HasInvalidElementIdValue(e, BuiltInParameter
                            .RBS_CURVETYPE_MULTISHAPE_TRANSITION_PARAM))
                            shape = "oval";
                    }
                }

                return shape;
            }
        }

        #endregion // MEP Element Shape Version 3

        #region MEP Element Shape Version 4

        public static bool MepElementShapeIsElementOfCategory(
            Element e,
            BuiltInCategory c)
        {
            return e.Category.Id.Value.Equals(
                (int) c);
        }

        public static string GetElementShape4(
            Element e)
        {
            var shape = "unknown";

            var tid = e.GetTypeId();

            if (ElementId.InvalidElementId != tid)
            {
                var doc = e.Document;

                if (doc.GetElement(tid) is ElementType etyp) shape = etyp.FamilyName;
            }

            return shape;
        }

        public static ConnectorProfileType GetDuctConnectorShape(Duct duct)
        {
            var ductShape
                = ConnectorProfileType.Invalid;

            foreach (Connector c
                in duct.ConnectorManager.Connectors)
                if (c.ConnectorType == ConnectorType.End)
                {
                    ductShape = c.Shape;
                    break;
                }

            return ductShape;
        }

        public static ConnectorProfileType[] GetDuctProfileTypes(
            Duct duct)
        {
            var connectors
                = duct.ConnectorManager.Connectors;

            var n = connectors.Size;

            var profileTypes
                = new ConnectorProfileType[n];

            var i = 0;

            foreach (Connector c in connectors) profileTypes[i++] = c.Shape;
            return profileTypes;
        }

        #endregion // MEP Element Shape Version 4
    }
}
