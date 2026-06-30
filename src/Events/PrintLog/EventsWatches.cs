// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.Events.PrintLog.CS
{
    public sealed class EventsWatches
    {
        private Stopwatch m_docPrintWatch;

        private Stopwatch m_viewPrintWatch;

        public Stopwatch ViewPrintWatch
        {
            get => m_viewPrintWatch;
            set => m_viewPrintWatch = value;
        }

        public Stopwatch DocPrintWatch
        {
            get => m_docPrintWatch;
            set => m_docPrintWatch = value;
        }
    }
}
