// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Threading;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.MultiThreading.WorkThread.CS
{
    public class ThreadAgent
    {
        private readonly BoundingBoxUV m_bbox;
        private readonly int m_density;
        private readonly SharedResults m_results;
        private Thread m_thread;

        public ThreadAgent(BoundingBoxUV bbox, int density, SharedResults results)
        {
            m_bbox = bbox;
            m_density = density;
            m_results = results;
        }

        public bool IsThreadAlive => m_thread != null && m_thread.IsAlive;

        public bool Start()
        {
            if (IsThreadAlive) return false;

            m_thread = new Thread(Run);
            m_thread.Start(m_results);
            return true;
        }

        public void WaitToFinish()
        {
            if (IsThreadAlive) m_thread.Join();
        }

        // Demo work-thread loop; values are arbitrary. Stop when SharedResults rejects a result.
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

                    Thread.Sleep(100);

                    if (!results.AddResult(point, value)) return;
                }
            }
        }
    }
}
