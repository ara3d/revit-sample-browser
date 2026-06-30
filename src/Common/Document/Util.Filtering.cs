// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Element Filtering

        public static FilteredElementCollector GetElementsOfType(
            Document doc,
            Type type,
            BuiltInCategory bic)
        {
            var collector
                = new FilteredElementCollector(doc);

            collector.OfCategory(bic);
            collector.OfClass(type);

            return collector;
        }

        public static Element GetFirstElementOfTypeNamed(
            Document doc,
            Type type,
            string name)
        {
            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(type);

#if EXPLICIT_CODE
      // explicit iteration and manual checking of a property:

      Element ret = null;
      foreach( Element e in collector )
      {
        if( e.Name.Equals( name ) )
        {
          ret = e;
          break;
        }
      }
      return ret;
#endif // EXPLICIT_CODE

#if USE_LINQ
      // using LINQ:

      IEnumerable<Element> elementsByName =
        from e in collector
        where e.Name.Equals( name )
        select e;

      return elementsByName.First<Element>();
#endif // USE_LINQ

            // using an anonymous method:

            // if no matching elements exist, First<> throws an exception.

            //return collector.Any<Element>( e => e.Name.Equals( name ) )
            //  ? collector.First<Element>( e => e.Name.Equals( name ) )
            //  : null;

            // using an anonymous method to define a named method:

            Func<Element, bool> nameEquals = e => e.Name.Equals(name);

            return collector.Any(nameEquals)
                ? collector.First(nameEquals)
                : null;
        }

        public static Element GetFirstNonTemplate3dView(Document doc)
        {
            var collector
                = new FilteredElementCollector(doc);

            collector.OfClass(typeof(View3D));

            return collector
                .Cast<View3D>()
                .First(v3 => !v3.IsTemplate);
        }

        public static FamilySymbol FindFamilySymbol(
            Document doc,
            string familyName,
            string symbolName)
        {
            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Family));

            foreach (Family f in collector)
                if (f.Name.Equals(familyName))
                {
                    //foreach( FamilySymbol symbol in f.Symbols ) // 2014

                    var ids = f.GetFamilySymbolIds(); // 2015

                    foreach (var id in ids)
                    {
                        var symbol = doc.GetElement(id)
                            as FamilySymbol;

                        if (symbol.Name == symbolName) return symbol;
                    }
                }

            return null;
        }

        public static ElementType GetElementTypeByName(
            Document doc,
            string name)
        {
            return new FilteredElementCollector(doc)
                    .OfClass(typeof(ElementType))
                    .First(q => q.Name.Equals(name))
                as ElementType;
        }

        public static ElementType GetFamilySymbolByName(
            Document doc,
            string name)
        {
            return new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilySymbol))
                    .First(q => q.Name.Equals(name))
                as FamilySymbol;
        }

        #endregion
    }
}
