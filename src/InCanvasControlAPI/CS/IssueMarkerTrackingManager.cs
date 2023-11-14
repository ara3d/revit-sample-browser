// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

namespace Revit.SDK.Samples.InCanvasControlAPI.CS
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

        /// <summary>
        ///     Gets an instance of IssueMarkerTrackingManager.
        /// </summary>
        /// <returns>An instance of IssueMarkerTrackingManager</returns>
        public static IssueMarkerTrackingManager GetInstance()
        {
            return _manager ?? (_manager = new IssueMarkerTrackingManager());
        }

        /// <summary>
        ///     Gets tracking for specified document
        /// </summary>
        /// <param name="doc">A Revit document</param>
        /// <returns>A corresponding instance of IssueMarkerTracking</returns>
        public IssueMarkerTracking GetTracking(Document doc)
        {
            if (m_trackings.Where(track => track.Document.Equals(doc)).FirstOrDefault() is IssueMarkerTracking tracking)
                return tracking;
            return null;
        }

        /// <summary>
        ///     Adds IssueMarkerTracking for the given document
        /// </summary>
        /// <param name="doc">A Revit document</param>
        public void AddTracking(Document doc)
        {
            if (!m_trackings.Any(track => track.Document.Equals(doc)))
                m_trackings.Add(new IssueMarkerTracking(doc));
        }

        /// <summary>
        ///     Removes IssueMarkerTracking from this manager
        /// </summary>
        /// <param name="guid">A GUID of the tracking</param>
        public void DeleteTracking(Guid guid)
        {
            m_trackings.RemoveWhere(track => track.Id == guid);
        }

        /// <summary>
        ///     Clears all trackings
        /// </summary>
        public void ClearTrackings()
        {
            m_trackings.Clear();
        }
    }
}
