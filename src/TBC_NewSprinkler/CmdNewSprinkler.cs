#region Header

//
// CmdNewSprinkler.cs - insert a new sprinkler family instance
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewSprinkler : IExternalCommand
    {
#pragma warning disable IDE1006 // Naming Styles

        private const string _path = "C:/Documents and Settings/All Users/Application Data/Autodesk/RME 2010/Metric Library/Fire Protection/Sprinklers/";
        private const string _name = "M_Sprinkler - Pendent - Hosted";
        private const string _ext = ".rfa";

        private const string _filename = _path + _name + _ext;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            var rc = Result.Failed;

            using var t = new Transaction(doc);
            t.Start("Place a New Sprinkler Instance");


            // retrieve the sprinkler family symbol:

#if _2010
        Filter filter = app.Create.Filter.NewFamilyFilter(
          _name );

        List<Element> families = new List<Element>();
        doc.get_Elements( filter, families );
        Family family = null;

        foreach( Element e in families )
        {
          family = e as Family;
          if( null != family )
            break;
        }
#endif // _2010

            if (Util.GetFirstElementOfTypeNamed(
                doc, typeof(Family), _name) is not Family family)
                if (!doc.LoadFamily(_filename, out family))
                {
                    message = $"Unable to load '{_filename}'.";
                    return rc;
                }

            FamilySymbol sprinklerSymbol = null;

            //foreach( FamilySymbol fs in family.Symbols ) // 2014

            foreach (var id in
                family.GetFamilySymbolIds()) // 2015
            {
                sprinklerSymbol = doc.GetElement(id)
                    as FamilySymbol;

                break;
            }

            Debug.Assert(null != sprinklerSymbol,
                "expected at least one sprinkler symbol"
                + " to be defined in family");

            // pick the host ceiling:

            var ceiling = Util.SelectSingleElement(
                uidoc, "ceiling to host sprinkler");

            if (null == ceiling
                || !ceiling.Category.Id.Value.Equals(
                    (int) BuiltInCategory.OST_Ceilings))
            {
                message = "No ceiling selected.";
                return rc;
            }

            //Level level = ceiling.Level;

            //XYZ p = new XYZ( 40.1432351841559, 30.09700395984548, 8.0000 );

            // these two methods cannot create the sprinkler on the ceiling:

            //FamilyInstance fi = doc.Create.NewFamilyInstance( p, sprinklerSymbol, ceiling, level, StructuralType.NonStructural );
            //FamilyInstance fi = doc.Create.NewFamilyInstance( p, sprinklerSymbol, ceiling, StructuralType.NonStructural );

            // use this overload so get the bottom face of the ceiling instead:

            // FamilyInstance NewFamilyInstance( Face face, XYZ location, XYZ referenceDirection, FamilySymbol symbol )

            // retrieve the bottom face of the ceiling:

            var ceilingBottom
                = Util.GetLargestHorizontalFace(ceiling);

            if (null != ceilingBottom)
            {
                var p = Util.PointOnFace(ceilingBottom);

                // Create the sprinkler family instance:

                var fi = doc.Create.NewFamilyInstance(
                    ceilingBottom, p, XYZ.BasisX, sprinklerSymbol);

                rc = Result.Succeeded;
            }

            t.Commit();

            return rc;
        }
    }
}