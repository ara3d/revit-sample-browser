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
    // Thread-safe exchange between the UI analyzer and the calculation work thread.
    public class SharedResults
    {
        private bool m_completed;
        private int m_numberWhenLastRead;
        private readonly IList<UV> m_points = new List<UV>();
        private readonly IList<ValueAtPoint> m_values = new List<ValueAtPoint>();
        private readonly object m_mylock = new object();

        // Called by the analyzer when it no longer needs results; AddResult then returns false.
        public void SetCompleted()
        {
            lock (m_mylock)
            {
                m_completed = true;
            }
        }

        public bool GetResults(out IList<UV> points, out IList<ValueAtPoint> values)
        {
            var hasMoreResults = false;
            points = null;
            values = null;

            lock (m_mylock)
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

        // Returns false once the analyzer has signaled completion.
        public bool AddResult(UV point, double value)
        {
            var accepted = false;

            lock (m_mylock)
            {
                if (!m_completed)
                {
                    var doubleList = new List<double> { value };
                    m_values.Add(new ValueAtPoint(doubleList));
                    m_points.Add(point);
                    accepted = true;
                }
            }

            return accepted;
        }
    }
}
