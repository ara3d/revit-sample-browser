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

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using BuildingCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    /// <summary>
    /// Hosts modeless search-result navigation and idling-based zoom.
    /// Replaces upstream App.cs for sample-browser launch without IExternalApplication.
    /// </summary>
    internal static class StringSearchHost
    {
        static UIApplication _uiapp;
        static JtWindowHandle _revitWindow;
        static long _pendingElementId;
        static bool _subscribing;

        public delegate void SetElementId(long id);

        public static void EnsureInitialized(UIApplication uiapp)
        {
            _uiapp = uiapp ?? throw new ArgumentNullException(nameof(uiapp));
            _revitWindow = new JtWindowHandle(uiapp.MainWindowHandle);
        }

        public static void ShowNavigator(SortableBindingList<SearchHit> data)
        {
            SearchHitNavigator.Show(data, SetPendingElementId, _revitWindow);
            Subscribe();
        }

        public static void Shutdown()
        {
            SearchHitNavigator.Shutdown();
            Unsubscribe();
        }

        static void SetPendingElementId(long id)
        {
            _pendingElementId = id;
        }

        static void Subscribe()
        {
            if (!_subscribing && _uiapp != null)
            {
                _uiapp.Idling += OnIdling;
                _subscribing = true;
            }
        }

        static void Unsubscribe()
        {
            if (_subscribing && _uiapp != null)
            {
                _uiapp.Idling -= OnIdling;
                _subscribing = false;
            }
        }

        static void OnIdling(object sender, IdlingEventArgs ea)
        {
            if (!SearchHitNavigator.IsShowing)
            {
                Unsubscribe();
            }

            var id = _pendingElementId;

            if (id == 0)
            {
                return;
            }

            var uiapp = sender as UIApplication ?? _uiapp;
            if (uiapp?.ActiveUIDocument == null)
            {
                return;
            }

            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var eid = new ElementId(id);
            var e = doc.GetElement(eid);

            Debug.Print(
                "Element id {0} requested --> {1}",
                id, new ElementData(e));

            var ids = new List<ElementId>(1) { eid };
            uidoc.Selection.SetElementIds(ids);
            uidoc.ShowElements(ids);

            _pendingElementId = 0;
        }
    }
}
