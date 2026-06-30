#region Copyright
// (C) Copyright 2011-2014 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
#endregion // Copyright

#region Namespaces
using Autodesk.Revit.DB;
#endregion // Namespaces

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    /// <summary>
    /// Manage data for a Revit element.
    /// This is also used in the loose connector navigator.
    /// </summary>
    public class ElementData
    {
        public string Class { get; set; }
        public string Category { get; set; }
        public string Family { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }

        public ElementData(
          Element e)
        {
            Class = e.GetType().Name;

            Category = (null == e.Category)
              ? string.Empty
              : e.Category.Name;

            var typeId = e.GetTypeId();

            var elementType = (null == typeId)
              ? null
              : e.Document.GetElement(typeId) as ElementType;

            var fi = e as FamilyInstance;

            //Duct duct = e as Duct;
            //if( null != duct )
            //{
            //  string s = duct.DuctType.Name;
            //}

            Family = (null != fi)
              ? fi.Symbol.Family.Name
              : string.Empty;

            Symbol = (null != fi) ? fi.Symbol.Name
              : ((null != elementType) ? elementType.Name
              : string.Empty);

            Name = e.Name;

            // This is not valid for electrical panels, which 
            // may have something like "EP-2" versus "400 A":
            //
            //Debug.Assert( Name.Equals( Symbol ), 
            //  "expected element name to equal symbol name" );

            Id = e.Id.Value;
        }

        public override string ToString()
        {
            var c = (0 == Category.Length)
              ? Class
              : Category;

            var fam = (0 == Family.Length)
              ? string.Empty
              : "'" + Family + "' ";

            return string.Format(
              "{0} {1}<{2} '{3}'>",
              c, fam, Id, Name);
        }
    }

    public class SearchHit : ElementData
    {
        /// <summary>
        /// Built-in parameter name, if applicable.
        /// </summary>
        string BipName { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        //public string ParameterString { get; set; }

        public int Index { get; set; }

        public SearchHit(
          Element e,
          string bipName,
          string parameterName,
          string parameterValue,
          //string parameterString,
          int index)
          : base(e)
        {
            BipName = bipName;
            ParameterName = parameterName;
            ParameterValue = parameterValue;
            //ParameterString = parameterString;
            Index = index;
        }

        public override string ToString()
        {
            return string.Format(
              "Index {0} in '{1}' in parameter '{2}' (bip '{3}') on element {4}",
              Index, ParameterValue, ParameterName,
              BipName, base.ToString());
        }
    }
}
