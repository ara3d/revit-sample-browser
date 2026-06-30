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
        #region Element Selection

        public static Element SelectSingleElement(
            UIDocument uidoc,
            string description)
        {
            if (ViewType.Internal == uidoc.ActiveView.ViewType)
            {
                TaskDialog.Show("Error",
                    $"Cannot pick element in this view: {uidoc.ActiveView.Name}");

                return null;
            }

#if _2010
    sel.Elements.Clear();
    Element e = null;
    sel.StatusbarTip = "Please select " + description;
    if( sel.PickOne() )
    {
      ElementSetIterator elemSetItr
        = sel.Elements.ForwardIterator();
      elemSetItr.MoveNext();
      e = elemSetItr.Current as Element;
    }
    return e;
#endif // _2010

            try
            {
                var r = uidoc.Selection.PickObject(
                    ObjectType.Element,
                    $"Please select {description}");

                // 'Autodesk.Revit.DB.Reference.Element' is
                // obsolete: Property will be removed. Use
                // Document.GetElement(Reference) instead.
                //return null == r ? null : r.Element; // 2011

                return uidoc.Document.GetElement(r); // 2012
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public static Element GetSingleSelectedElement(
            UIDocument uidoc)
        {
            var ids
                = uidoc.Selection.GetElementIds();

            Element e = null;

            if (1 == ids.Count)
                foreach (var id in ids)
                    e = uidoc.Document.GetElement(id);
            return e;
        }

        private static bool HasRequestedType(
            Element e,
            Type t,
            bool acceptDerivedClass)
        {
            var rc = null != e;

            if (rc)
            {
                var t2 = e.GetType();

                rc = t2.Equals(t);

                if (!rc && acceptDerivedClass) rc = t2.IsSubclassOf(t);
            }

            return rc;
        }

        public static Element SelectSingleElementOfType(
            UIDocument uidoc,
            Type t,
            string description,
            bool acceptDerivedClass)
        {
            var e = GetSingleSelectedElement(uidoc);

            if (!HasRequestedType(e, t, acceptDerivedClass))
                e = SelectSingleElement(
                    uidoc, description);
            return HasRequestedType(e, t, acceptDerivedClass)
                ? e
                : null;
        }

        public static bool GetSelectedElementsOrAll(
            List<Element> a,
            UIDocument uidoc,
            Type t)
        {
            var doc = uidoc.Document;

            var ids
                = uidoc.Selection.GetElementIds();

            if (0 < ids.Count)
                a.AddRange(ids
                    .Select(
                        id => doc.GetElement(id))
                    .Where(
                        e => t.IsInstanceOfType(e)));
            else
                a.AddRange(new FilteredElementCollector(doc)
                    .OfClass(t));
            return 0 < a.Count;
        }

        #endregion
    }
}
