// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Diagnostics;

namespace Revit.SDK.Samples.PrintLog.CS
{
    /// <summary>
    ///     This class consists of two watches for view print and document print processes.
    ///     Each watch will be used to calculate the cost time of print individual view and total views..
    /// </summary>
    public sealed class EventsWatches
    {
        /// <summary>
        ///     Time watch for document print process.
        ///     It'll be renewed when DocumentPrinting is raised and then used to calculate the print time
        ///     for all views; the calculation will occurs in DocumentPrinted event.
        /// </summary>
        private Stopwatch m_docPrintWatch;

        /// <summary>
        ///     Time watch for view print process.
        ///     It'll be renewed when ViewPrinting is raised and then used to calculate the print time
        ///     for each view; the calculation will occurs in ViewPrinted event.
        /// </summary>
        private Stopwatch m_viewPrintWatch;

        /// <summary>
        ///     Get/set the watch of view print process.
        /// </summary>
        public Stopwatch ViewPrintWatch
        {
            get => m_viewPrintWatch;
            set => m_viewPrintWatch = value;
        }

        /// <summary>
        ///     Get/set the watch of document print process.
        /// </summary>
        public Stopwatch DocPrintWatch
        {
            get => m_docPrintWatch;
            set => m_docPrintWatch = value;
        }
    }
}
