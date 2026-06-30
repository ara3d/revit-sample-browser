// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.InCanvasControlAPI.CS
{
    /// <summary>
    ///     This class manages instances of IssueMarkerTracking on per-document basis.
    /// </summary>
    public class IssueMarkerTrackingManager
    {
        private static IssueMarkerTrackingManager _manager;

        private readonly HashSet<IssueMarkerTracking> m_trackings = new HashSet<IssueMarkerTracking>();

        private IssueMarkerTrackingManager()
        {
        }

        public static IssueMarkerTrackingManager GetInstance()
        {
            return _manager ?? (_manager = new IssueMarkerTrackingManager());
        }

        public IssueMarkerTracking GetTracking(Document doc)
        {
            if (m_trackings.Where(track => track.Document.Equals(doc)).FirstOrDefault() is IssueMarkerTracking tracking)
                return tracking;
            return null;
        }

        public void AddTracking(Document doc)
        {
            if (!m_trackings.Any(track => track.Document.Equals(doc)))
                m_trackings.Add(new IssueMarkerTracking(doc));
        }

        public void DeleteTracking(Guid guid)
        {
            m_trackings.RemoveWhere(track => track.Id == guid);
        }

        public void ClearTrackings()
        {
            m_trackings.Clear();
        }
    }
}
