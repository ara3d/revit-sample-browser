// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Threading;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.MultiThreading.WorkThread.CS
{
    /// <summary>
    ///     A main class of the delegated thread.
    /// </summary>
    /// <remarks>
    ///     It has few data the calculations needs and
    ///     one method that will run on a separate thread.
    /// </remarks>
    public class ThreadAgent
    {
        // BoundingBoxUV
        private readonly BoundingBoxUV m_bbox;

        // Density
        private readonly int m_density;

        // Results
        private readonly SharedResults m_results;

        // The main method for calculating results for the face analysis
        private Thread m_thread;

        /// <summary>
        ///     A constructor initializes a bounding box and
        ///     the density of the grid for the values to be calculated at.
        /// </summary>
        public ThreadAgent(BoundingBoxUV bbox, int density, SharedResults results)
        {
            m_bbox = bbox;
            m_density = density;
            m_results = results;
        }

        /// <summary>
        ///     A property to test whether the calculation thread is still alive.
        /// </summary>
        public bool IsThreadAlive => m_thread != null && m_thread.IsAlive;

        /// <summary>
        ///     Creates and starts a work thread operating upon the given  shared results.
        /// </summary>
        /// <returns>
        ///     True if a work thread could be started successfully.
        /// </returns>
        public bool Start()
        {
            if (IsThreadAlive) return false;

            m_thread = new Thread(Run);
            m_thread.Start(m_results);
            return true;
        }

        /// <summary>
        ///     Waits for the work thread to finish
        /// </summary>
        public void WaitToFinish()
        {
            if (IsThreadAlive) m_thread.Join();
        }

        /// <summary>
        ///     The main method for calculating results for the face analysis.
        /// </summary>
        /// <remarks>
        ///     The calculated values do not mean anything particular.
        ///     They are just to demonstrate how to process a potentially
        ///     time-demanding analysis in a delegated work-thread.
        /// </remarks>
        /// <param name="data">
        ///     The instance of a Result object to which the results
        ///     will be periodically delivered until we either finish
        ///     the process or are asked to stop.
        /// </param>
        private void Run(object data)
        {
            var results = data as SharedResults;

            var uRange = m_bbox.Max.U - m_bbox.Min.U;
            var vRange = m_bbox.Max.V - m_bbox.Min.V;
            var uStep = uRange / m_density;
            var vStep = vRange / m_density;

            for (var u = 0; u <= m_density; u++)
            {
                var uPos = m_bbox.Min.U + u * uStep;
                var uVal = (double)(u * (m_density - u));

                for (var v = 0; v <= m_density; v++)
                {
                    var vPos = m_bbox.Min.V + v * vStep;
                    var vVal = (double)(v * (m_density - v));

                    var point = new UV(uPos, vPos);
                    var value = Math.Min(uVal, vVal);

                    // We pretend the calculation of values is far more complicated
                    // while what we really do is taking a nap for a few milliseconds 

                    Thread.Sleep(100);

                    // If adding the result is not accepted it means the analysis
                    // have been interrupted and we are supposed to get out ASAP

                    if (!results.AddResult(point, value)) return;
                }
            } // for
        }
    } // class
}
