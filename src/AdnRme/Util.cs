#region Header
// Revit MEP API sample application
//
// Copyright (C) 2007-2021 by Jeremy Tammik, Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// for any purpose and without fee is hereby granted, provided
// that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  
// AUTODESK, INC. DOES NOT WARRANT THAT THE OPERATION OF THE 
// PROGRAM WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject
// to restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Header

#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinForms = System.Windows.Forms;
#endregion // Namespaces

namespace AdnRme
{
    class Util
    {
        #region Exceptions
        public class ParameterException : Exception
        {
            public ParameterException(string parameterName, string description, Element elem)
              : base(string.Format("'{0}' parameter not defined for {1} {2}", parameterName, description, elem.Id.Value.ToString()))
            {
            }
        }

        public class SpaceParameterException : Exception
        {
            public SpaceParameterException(string parameterName, Space space)
              : base(string.Format("'{0}' parameter not defined for space {1}", parameterName, space.Number))
            {
            }
        }

        public class TerminalParameterException : ParameterException
        {
            public TerminalParameterException(string parameterName, FamilyInstance terminal)
              : base(parameterName, "terminal", terminal)
            {
            }
        }
        #endregion // Exceptions

        #region Formatting
        public static string IdList(IList<FamilyInstance> elements)
        {
            var s = string.Empty;
            foreach (Element e in elements)
            {
                if (0 < s.Length)
                {
                    s += ", ";
                }
                s += e.Id.Value.ToString();
            }
            return s;
        }

        #endregion // Formatting

        public static string ElementDescription(Element e)
        {
            var description = (null == e.Category)
              ? e.GetType().Name
              : e.Category.Name;
            if (null != e.Name)
            {
                description += " '" + e.Name + "'";
            }
            return description;
        }

        public static string ElementDescriptionAndId(Element e)
        {
            var description = e.GetType().Name;
            if (null != e.Category)
            {
                description += " " + e.Category.Name;
            }
            var identity = e.Id.Value.ToString();
            if (null != e.Name)
            {
                identity = e.Name + " " + identity;
            }
            return string.Format("{0} <{1}>", description, identity);
        }

        public static string BrowserDescription(Element e)
        {
            return (e is not FamilyInstance inst ? e.Category.Name : inst.Symbol.Family.Name) + " " + e.Name;
        }

        #region Message
        const string _caption = "Revit MEP API Sample";

        public static void InfoMsg(string msg)
        {
            WinForms.MessageBox.Show(msg, _caption, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
        }

        public static void ErrorMsg(string msg)
        {
            WinForms.MessageBox.Show(msg, _caption, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
        }

        public static bool QuestionMsg(string msg)
        {
            return WinForms.DialogResult.Yes
              == WinForms.MessageBox.Show(msg, _caption, WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Question);
        }
        #endregion // Message

        #region Parameter Access
        static Parameter GetParameterFromName(Element elem, string name)
        {
            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition.Name == name)
                {
                    return p;
                }
            }
            return null;
        }

        public static Definition GetParameterDefinitionFromName(Element elem, string name)
        {
            var p = GetParameterFromName(elem, name);
            return (null == p) ? null : p.Definition;
        }

        public static double GetParameterValueFromName(Element elem, string name)
        {
            var p = GetParameterFromName(elem, name);
            return null == p ? throw new ParameterException(name, "element", elem) : p.AsDouble();
        }

        public static string GetStringParameterValueFromName(Element elem, string name)
        {
            var p = GetParameterFromName(elem, name);
            return null == p ? throw new ParameterException(name, "element", elem) : p.AsString();
        }

        static void DumpParameters(Element elem)
        {
            foreach (Parameter p in elem.Parameters)
            {
                Debug.WriteLine(p.Definition.GetDataType().TypeId + " " + p.Definition.Name);
            }
        }

        public static double GetSpaceParameterValue(Space space, BuiltInParameter bip, string name)
        {
            var p = space.get_Parameter(bip);
            return null == p ? throw new SpaceParameterException(name, space) : p.AsDouble();
        }

        public static Parameter GetSpaceParameter(Space space, string name)
        {
            var p = GetParameterFromName(space, name);
            return p ?? throw new SpaceParameterException(name, space);
        }

        public static double GetSpaceParameterValue(Space space, string name)
        {
            var p = GetSpaceParameter(space, name);
            return p.AsDouble();
        }

#if NEED_IS_SUPPLY_AIR_METHOD
    public static bool IsSupplyAir( FamilyInstance terminal )
    {
      Parameter p = terminal.get_Parameter( Bip.SystemType );
      if( null == p )
      {
        throw new TerminalParameterException( Bip.SystemType.ToString(), terminal );
      }
      bool rc = p.AsString().Equals( ParameterValue.SupplyAir );

#if DEBUG
      ElementId typeId = terminal.GetTypeId();
      ElementType t = terminal.Document.get_Element( typeId ) as ElementType;
      MEPSystemType t2 = terminal.Document.get_Element( typeId ) as MEPSystemType;
      Debug.Assert( (MEPSystemClassification.SupplyAir == t2.SystemClassification) == rc,
        "expected parameter check to return correct system classification" );
#endif // DEBUG

      return rc;
    }
#endif // NEED_IS_SUPPLY_AIR_METHOD

        public static Parameter GetTerminalFlowParameter(FamilyInstance terminal)
        {
            //
            // the built-in parameter "Flow" is read-only:
            //
            //Parameter p = terminal.get_Parameter( _bipFlow );
            //
            // The parameter we are interested in is not the BuiltInParameter... 
            //
            var d = Util.GetParameterDefinitionFromName(terminal, ParameterName.Flow);
            var p = terminal.get_Parameter(d);
            return p ?? throw new Util.TerminalParameterException(ParameterName.Flow, terminal);
        }
        #endregion // Parameter Access

        #region HVAC Element Access
        public static FilteredElementCollector GetSupplyAirTerminals(Document doc)
        {
            FilteredElementCollector collector = new(doc);
            collector.OfCategory(BuiltInCategory.OST_DuctTerminal);
            collector.OfClass(typeof(FamilyInstance));

            //int n1 = collector.ToElements().Count; // 61 in sample model

            // ensure that system type equals supply air:
            //
            // in Revit 2009 and 2010 API, this did it:
            //
            //ParameterFilter parameterFilter = a.Filter.NewParameterFilter( 
            //  Bip.SystemType, CriteriaFilterType.Equal, ParameterValue.SupplyAir );

            // in Revit 2011, create an ElementParameter filter.
            // Create filter by provider and evaluator:

            ParameterValueProvider provider = new(new ElementId(Bip.SystemType));
            FilterStringRuleEvaluator evaluator = new FilterStringEquals();
            var ruleString = ParameterValue.SupplyAir;
            FilterRule rule = new FilterStringRule(provider, evaluator, ruleString);
            ElementParameterFilter filter = new(rule);

            collector.WherePasses(filter);

            //int n2 = collector.ToElements().Count; // 51 in sample model

            return collector;
        }

        public static List<Space> GetSpaces(Document doc)
        {
            FilteredElementCollector collector
              = new(doc);

            // trying to collect all spaces directly causes 
            // the following error:
            //
            // Input type is of an element type that exists 
            // in the API, but not in Revit's native object 
            // model. Try using Autodesk.Revit.DB.Enclosure 
            // instead, and then postprocessing the results 
            // to find the elements of interest.
            //
            //collector.OfClass( typeof( Space ) );

            collector.OfClass(typeof(SpatialElement));

            //return ( from e in collector.ToElements() // 2011
            //         where e is Space
            //         select e as Space )
            //  .ToList<Space>();

            return collector.ToElements().OfType<Space>().ToList<Space>(); // 2012
        }
        #endregion // HVAC Element Access

        #region Electrical Element Access
        public static FilteredElementCollector GetElementsOfType(
          Document doc,
          Type type,
          BuiltInCategory bic)
        {
            FilteredElementCollector collector
              = new(doc);

            collector.OfCategory(bic);
            collector.OfClass(type);

            return collector;
        }

        public static List<Element> GetElectricalEquipment(
          Document doc)
        {
            var collector
              = GetElementsOfType(doc, typeof(FamilyInstance),
                BuiltInCategory.OST_ElectricalEquipment);

            // return a List instead of IList, because we need the method Exists() on it:

            return [.. collector.ToElements()];
        }

        public static IList<Element> GetElectricalSystems(Document doc)
        {
            FilteredElementCollector collector = new(doc);
            collector.OfClass(typeof(ElectricalSystem));
            return collector.ToElements();
        }

        /// <summary>
        /// Circuit elements: family instances or electrical systems with a non-empty circuit number.
        /// Prepends class filters because parameter access is slow.
        /// </summary>
        public static IList<Element> GetCircuitElements(Document doc)
        {
            //
            // prepend as many 'fast' filters as possible, because parameter access is 'slow':
            //
            ElementClassFilter f1 = new(typeof(FamilyInstance));
            ElementClassFilter f2 = new(typeof(ElectricalSystem));
            LogicalOrFilter f3 = new(f1, f2);
            var collector = new FilteredElementCollector(doc).WherePasses(f3);

            var bip = BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER;

#if DEBUG
            var n1 = collector.ToElements().Count;

            List<Element> a = [];
            foreach (var e in collector)
            {
                var p = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER);
                if (null != p && 0 < p.AsString().Length)
                {
                    a.Add(e);
                }
            }
            var n2 = a.Count;
            Debug.Assert(n1 > n2, "expected filter to eliminate something");

            var b = (
              from e in collector.ToElements()
              where (null != e.get_Parameter(bip)) && (0 < e.get_Parameter(bip).AsString().Length)
              select e).ToList<Element>();

            var n3 = b.Count;
            Debug.Assert(n2 == n3, "expected to reproduce same result");
#endif // DEBUG

            //
            // this is unclear ... negating the rule that says the parameter 
            // exists and is empty could mean that elements pass if the parameter 
            // is non-empty, but also if the parameter does not exist ...
            // so maybe returning the collection b instead of c would be a safer bet?
            //
            ParameterValueProvider provider = new(new ElementId(bip));
            FilterStringRuleEvaluator evaluator = new FilterStringEquals();
            FilterRule rule = new FilterStringRule(provider, evaluator, string.Empty);
            ElementParameterFilter filter = new(rule, true);

            collector.WherePasses(filter);
            var c = collector.ToElements();
#if DEBUG
            var n4 = c.Count;
            Debug.Assert(n2 == n4, "expected to reproduce same result");
#endif

            return c;
        }

        public static Element GetProjectInfoElem(Document doc)
        {
            //Filter filterCategory = app.Create.Filter.NewCategoryFilter( BuiltInCategory.OST_ProjectInformation );
            //ElementIterator i = app.ActiveDocument.get_Elements( filterCategory );
            //i.MoveNext();
            //Element e = i.Current as Element;

            FilteredElementCollector collector = new(doc);
            collector.OfCategory(BuiltInCategory.OST_ProjectInformation);
            Debug.Assert(1 == collector.ToElements().Count, "expected one single element to be returned");
            var e = collector.FirstElement();
            Debug.Assert(null != e, "expected valid project information element");
            return e;
        }
        #endregion // Electrical Element Access
    }
}
