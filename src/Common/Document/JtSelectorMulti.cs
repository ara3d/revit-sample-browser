// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion

namespace BuildingCoder
{
    internal class JtSelectorMulti<T> where T : Element
    {
#pragma warning disable IDE1006

        public delegate bool IsSelectable(Element e);

        private const string _usage_error = "Please pre-select "
                                            + "only {0}s before launching this command.";

        private readonly string _msg;
        private readonly Result _result;

        private readonly List<T> _selected;

        public JtSelectorMulti(
            UIDocument uidoc,
            BuiltInCategory? bic,
            string description,
            IsSelectable f)
        {
            _selected = null;
            _msg = null;

            var doc = uidoc.Document;

            if (null == doc)
            {
                _msg = "Please run this command in a valid"
                       + " Revit project document.";
                _result = Result.Failed;
            }

            // Check for pre-selected elements

            var sel = uidoc.Selection;
            var ids = sel.GetElementIds();
            var n = ids.Count;

            if (0 < n)
                //if( 1 != n )
                //{
                //  _msg = _usage_error;
                //  _result = Result.Failed;
                //}

                foreach (var id in ids)
                {
                    var e = doc.GetElement(id);

                    if (!f(e))
                    {
                        _msg = string.Format(
                            _usage_error, description);

                        _result = Result.Failed;
                    }

                    if (null == _selected) _selected = new List<T>(n);

                    _selected.Add(e as T);
                }

            // If no elements were pre-selected, 
            // prompt for post-selection

            if (null == _selected
                || 0 == _selected.Count)
            {
                IList<Reference> refs = null;

                try
                {
                    refs = sel.PickObjects(
                        ObjectType.Element,
                        new JtSelectionFilter(typeof(T), bic, f),
                        $"Please select {description}s.");
                }
                catch (OperationCanceledException)
                {
                    _result = Result.Cancelled;
                }

                if (refs is { Count: > 0 })
                    _selected = [.. refs.Select(
                            r => doc.GetElement(r.ElementId)
                                as T)];
            }

            Debug.Assert(
                null == _selected || 0 < _selected.Count,
                "ensure we return only non-empty collections");

            _result = null == _selected
                ? Result.Cancelled
                : Result.Succeeded;
        }

        public bool IsEmpty =>
            null == _selected
            || 0 == _selected.Count;

        public IList<T> Selected => _selected;

        #region Sample common filtering helper method

        public static bool IsTable(Element e)
        {
            var rc = false;

            var cat = e.Category;

            if (null != cat)
                if (cat.Id.Value.Equals(
                    (int)BuiltInCategory.OST_Furniture))
                    if (e is FamilyInstance fi)
                    {
                        var fname = fi.Symbol.Family.Name;

                        rc = fname.Equals("SampleTableFamilyName");
                    }

            return rc;
        }

        #endregion // Common filtering helper method

        public Result ShowResult()
        {
            if (Result.Failed == _result)
            {
                Debug.Assert(0 < _msg.Length,
                    "expected a non-empty error message");

                Util.ErrorMsg(_msg);
            }

            return _result;
        }

        #region JtSelectionFilter

        private class JtSelectionFilter : ISelectionFilter
        {
            private readonly BuiltInCategory? _bic;
            private readonly IsSelectable _f;
            private readonly Type _t;

            public JtSelectionFilter(
                Type t,
                BuiltInCategory? bic,
                IsSelectable f)
            {
                _t = t;
                _bic = bic;
                _f = f;
            }

            public bool AllowElement(Element e)
            {
                return e is T
                       && HasBic(e)
                       && _f(e);
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return true;
            }

            private bool HasBic(Element e)
            {
                return null == _bic
                       || (null != e.Category
                       && e.Category.Id.Value.Equals(
                           (int)_bic));
            }
        }

        #endregion // JtSelectionFilter

        // There is no need to provide external access to 
        // the error message or result code, since that 
        // can all be encapsulated in the call to ShowResult.

        //public string ErrorMessage
        //{
        //  {
        //    return _msg;
        //  }
        //}

        //public Result Result
        //{
        //  {
        //    return _result;
        //  }
        //}
    }
}