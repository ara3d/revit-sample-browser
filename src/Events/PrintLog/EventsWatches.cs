// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.Events.PrintLog.CS
{
    public sealed class EventsWatches
    {
        public Stopwatch ViewPrintWatch { get; set; }

        public Stopwatch DocPrintWatch { get; set; }
    }
}
