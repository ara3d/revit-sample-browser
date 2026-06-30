#region Header

//
// CmdNewColumnTypeInstance.cs - create a new
// column type and insert an instance of it
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewColumnTypeInstance : IExternalCommand
    {
        private const string _family_name
            = "M_Rectangular Column";

        private const string _extension
            = ".rfa";

        //const string _directory
        //  = "C:/Documents and Settings/All Users"
        //  + "/Application Data/Autodesk/RAC 2009"
        //  + "/Metric Library/Columns/";

        private const string _directory
            = "C:/ProgramData/Autodesk/RAC 2012"
              + "/Libraries/US Metric/Columns/";

        private const string _path
            = _directory + _family_name + _extension;

        private readonly StructuralType nonStructural
            = StructuralType.NonStructural;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var rc = Result.Failed;

            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            // interested in is loaded:

#if _2010
      List<Element> symbols = new List<Element>();

      Filter filter = app.Create.Filter.NewFamilyFilter(
        _family_name );

      doc.get_Elements( filter, symbols );

      // the family filter returns both the

      Family f = null;
      foreach( Element e in symbols )
      {
        if( e is Family )
        {
          f = e as Family;
        }
        else if( e is FamilySymbol )
        {
          FamilySymbol s = e as FamilySymbol;
          Debug.Print(
            "Family name={0}, symbol name={1}",
            s.Family.Name, s.Name );
        }
      }
#endif


            using Transaction t = new(doc);
            t.Start("Create New Column Type and Instance");


            if (Util.GetFirstElementOfTypeNamed(
                doc, typeof(Family), _family_name) is not Family f)
                if (!doc.LoadFamily(_path, out f))
                    message = $"Unable to load '{_path}'.";

            if (null != f)
            {
                Debug.Print("Family name={0}", f.Name);

                // will do, we select the first:

                FamilySymbol s = null;

                //foreach( FamilySymbol s2 in f.Symbols ) // 2014

                foreach (var id in f.GetFamilySymbolIds()) // 2015
                {
                    s = doc.GetElement(id) as FamilySymbol;
                    break;
                }

                Debug.Assert(null != s,
                    "expected at least one symbol"
                    + " to be defined in family");


                var s1 = s.Duplicate("Nuovo simbolo");
                s = s1 as FamilySymbol;


                foreach (Parameter param in s.Parameters)
                    Debug.Print(
                        "Parameter name={0}, value={1}",
                        param.Definition.Name,
                        param.AsValueString());

                // the specified parameter name is case sensitive:


                s.LookupParameter("Width").Set(Util.MmToFoot(500)); // 2015
                s.LookupParameter("Depth").Set(Util.MmToFoot(1000)); // 2015


                s.Name = "Nuovo simbolo due";


                var p = XYZ.Zero;
                doc.Create.NewFamilyInstance(
                    p, s, nonStructural);


                //XYZ normal = new XYZ( 1, 2, 3 );
                //  p, s, normal, null, nonStructural );

                rc = Result.Succeeded;
            }

            t.Commit();

            return rc;
        }
    }
}