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

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.MultiThreading.WorkThread.CS
{
    /// <summary>
    ///     This class is for exchange of results between the
    ///     analyzer which displays the results and the work-thread
    ///     that calculates them. All operations are thread-safe;
    /// </summary>
    internal class SharedResults
    {
        // If completed
        private bool m_completed;

        // Last read number
        private int m_numberWhenLastRead;

        // List of UV points
        private readonly IList<UV> m_points = new List<UV>();

        // List of ValueAtPoints
        private readonly IList<ValueAtPoint> m_values = new List<ValueAtPoint>();

        // lock object
        private readonly object m_mylock = new object();

        /// <summary>
        ///     Signaling no more results are needed
        /// </summary>
        /// <remarks>
        ///     This is set by the analyzer if it needs no more data.
        ///     We will let this know to the work-thread when it attempts
        ///     to add more results. When the work-tread results are not
        ///     needed anymore, it will stop even when not finished yet
        ///     and returns (which basically means it will die).
        /// </remarks>
        public void SetCompleted()
        {
            lock (m_mylock)
            {
                m_completed = true;
            }
        }

        /// <summary>
        ///     Returns a list of points and values acquired so far.
        /// </summary>
        /// <returns>
        ///     False if there have been no new results acquired from
        ///     the work-thread since the last time this method was called.
        /// </returns>
        public bool GetResults(out IList<UV> points, out IList<ValueAtPoint> values)
        {
            var hasMoreResults = false;
            points = null;
            values = null;

            lock (m_mylock) // lock the access
            {
                hasMoreResults = m_values.Count != m_numberWhenLastRead;

                if (hasMoreResults)
                {
                    points = m_points;
                    values = m_values;
                    m_numberWhenLastRead = m_values.Count;
                }
            }

            return hasMoreResults;
        }

        /// <summary>
        ///     Adding one pair of point/value to the collected
        ///     results for the current analysis.
        /// </summary>
        /// The work-thread calls this every time it has another result to add.
        /// <returns>
        ///     Returns False if no more values can be accepted, which signals
        ///     to the work-thread that the analysis was interrupted and
        ///     that the thread is supposed to stop and return immediately.
        /// </returns>
        public bool AddResult(UV point, double value)
        {
            var accepted = false;

            lock (m_mylock) // lock the access
            {
                // do nothing if reading has been completed
                if (!m_completed)
                {
                    // First, the double is converted to a one-item
                    // list of ValueAtPoint. Than the list is added
                    // to the list of values, while the UV is added
                    // to the list of points.

                    var doubleList = new List<double> { value };
                    m_values.Add(new ValueAtPoint(doubleList));
                    m_points.Add(point);
                    accepted = true;
                }
            }

            return accepted;
        }
    } // class
}
