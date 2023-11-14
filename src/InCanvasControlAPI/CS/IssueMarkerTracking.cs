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
    ///     A class tracks all issue markers in a given document. It also tracks the index of the active selected marker.
    /// </summary>
    public class IssueMarkerTracking
    {
        private readonly HashSet<IssueMarker> m_issueMarkerSet = new HashSet<IssueMarker>();

        private int m_selectedIndex;

        /// <summary>
        ///     Creates IssueMarkerTracking for the opened document and initializes selected index
        /// </summary>
        /// <param name="document">An opened Revit document</param>
        public IssueMarkerTracking(Document document)
        {
            Document = document;
            Id = Guid.NewGuid();
            m_selectedIndex = -1;
        }

        /// <summary>
        ///     Document this object tracks
        /// </summary>
        public Document Document { get; }

        /// <summary>
        ///     Tracker's GUID. This is needed to safely clean up after document closes.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Adds a marker to this tracking
        /// </summary>
        /// <param name="marker">Marker to be updated by selector or updater.</param>
        public void SubscribeMarker(IssueMarker marker)
        {
            m_issueMarkerSet.Add(marker);
        }

        /// <summary>
        ///     Removes marker that tracks the element specified by id.
        /// </summary>
        /// <param name="elementId">Tracked element id</param>
        public void RemoveMarkerByElement(ElementId elementId)
        {
            m_issueMarkerSet.RemoveWhere(m => m.TrackedElementId == elementId);
        }

        /// <summary>
        ///     Gets the issue marker that tracks element specified by id
        /// </summary>
        /// <param name="elementId">Tracked element id</param>
        /// <returns>A corresponding Issue Marker</returns>
        public IssueMarker GetMarkerByElementId(ElementId elementId)
        {
            return m_issueMarkerSet.Where(m => m.TrackedElementId == elementId).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the issue marker by it's id in TemporaryGraphicsManager
        /// </summary>
        /// <param name="index">Index of the in-canvas control</param>
        /// <returns>A corresponding Issue Marker</returns>
        public IssueMarker GetMarkerByIndex(int index)
        {
            return m_issueMarkerSet.Where(m => m.ControlIndex == index).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the index of selected marker. This is used by selector
        /// </summary>
        /// <returns>The index of selected marker</returns>
        public int GetSelected()
        {
            return m_selectedIndex;
        }

        /// <summary>
        ///     Sets the index of selected marker. This is used by selector
        /// </summary>
        /// <param name="index">Index of the marker</param>
        public void SetSelected(int index)
        {
            m_selectedIndex = index;
        }
    }
}
