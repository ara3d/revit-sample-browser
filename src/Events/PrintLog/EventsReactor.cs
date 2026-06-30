// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.Events.PrintLog.CS
{
    /// <summary>Logs print events to PrintLog.txt (timestamps) and PrintEventsLog.txt (event args only, for cross-machine compare).</summary>
    public sealed class EventsReactor
    {
        private readonly string m_assemblyPath;

        // Key is Document.GetHashCode(). PathName throws on unsaved docs; Title can duplicate across files.
        // Revit guarantees stable hash codes for CLI document objects within a session.
        private readonly Dictionary<int, EventsWatches> m_docEventsWatches;

        private TextWriterTraceListener m_eventsLog;

        private TextWriterTraceListener m_printLog;

        public EventsReactor()
        {
            m_docEventsWatches = new Dictionary<int, EventsWatches>();
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public void CloseLogFiles()
        {
            Trace.Flush();
            Trace.Close();
            Trace.Flush();
            if (null != m_printLog)
            {
                Trace.Listeners.Remove(m_printLog);
                m_printLog.Flush();
                m_printLog.Close();
            }

            if (null != m_eventsLog)
            {
                Trace.Listeners.Remove(m_eventsLog);
                m_eventsLog.Flush();
                m_eventsLog.Close();
            }
        }

        public void AppDocumentPrinting(object sender, DocumentPrintingEventArgs e)
        {
            SetupLogFiles();
            DumpPrintEnv(Environment.UserName, e.Document.PrintManager.PrinterName, e.Document.Title);
            Trace.WriteLine($"{Environment.NewLine}Document Print Start: >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            StartNewWatch(e.Document, false);
            DumpEventArguments(e);
        }

        public void AppViewPrinting(object sender, ViewPrintingEventArgs e)
        {
            Trace.WriteLine($"{Environment.NewLine}View Print Start: -----------------------------------------------");
            StartNewWatch(e.Document, true);
            DumpEventArguments(e);
        }

        public void AppViewPrinted(object sender, ViewPrintedEventArgs e)
        {
            Trace.WriteLine($"{Environment.NewLine}View Print End: -------");
            StopWatch(e.Document, true);
            DumpEventArguments(e);
        }

        public void AppDocumentPrinted(object sender, DocumentPrintedEventArgs e)
        {
            Trace.WriteLine($"{Environment.NewLine}Document Print End: <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            StopWatch(e.Document, false);
            DumpEventArguments(e);
        }

        private void SetupLogFiles()
        {
            if (null != m_printLog && null != m_eventsLog) return;

            var printLogFile = Path.Combine(m_assemblyPath, "PrintLog.txt");
            var printEventsLogFile = Path.Combine(m_assemblyPath, "PrintEventsLog.txt");
            if (File.Exists(printLogFile)) File.Delete(printLogFile);
            if (File.Exists(printEventsLogFile)) File.Delete(printEventsLogFile);

            m_printLog = new TextWriterTraceListener(printLogFile);
            m_eventsLog = new TextWriterTraceListener(printEventsLogFile);
            Trace.Listeners.Add(m_printLog);
            Trace.Listeners.Add(m_eventsLog);
            Trace.AutoFlush = true;
        }

        private void StartNewWatch(Document curDoc, bool isViewWatch)
        {
            DumpDateTime("Start");

            EventsWatches watches;
            var result = m_docEventsWatches.TryGetValue(curDoc.GetHashCode(), out watches);
            if (!result || null == watches)
            {
                watches = new EventsWatches();
                m_docEventsWatches.Add(curDoc.GetHashCode(), watches);
            }

            if (isViewWatch)
                watches.ViewPrintWatch = Stopwatch.StartNew();
            else
                watches.DocPrintWatch = Stopwatch.StartNew();
        }

        private void StopWatch(Document curDoc, bool isViewWatch)
        {
            DumpDateTime("End");

            EventsWatches watches;
            var result = m_docEventsWatches.TryGetValue(curDoc.GetHashCode(), out watches);
            if (!result)
            {
                Trace.Write("Failed to find the watch, time calculation is skipped.");
                return;
            }

            if (isViewWatch)
            {
                watches.ViewPrintWatch.Stop();
                m_printLog.WriteLine(
                    $"Succeeded to print view, costs {watches.ViewPrintWatch.Elapsed.TotalMilliseconds} ms.");
            }
            else
            {
                watches.DocPrintWatch.Stop();
                m_printLog.WriteLine(
                    $"Succeeded to print document, totally costs {watches.DocPrintWatch.Elapsed.TotalMilliseconds} ms.");
            }
        }

        private void DumpPrintEnv(string user, string printer, string projectName)
        {
            m_printLog.WriteLine("Start to Print..................................................");
            m_printLog.WriteLine($">> Print user: {user}");
            m_printLog.WriteLine($">> Printer name: {printer}");
            m_printLog.WriteLine($">> Project name: {projectName}");
        }

        private void DumpDateTime(string prefix)
        {
            SetupLogFiles();
            m_printLog.WriteLine($"{prefix} Time: {DateTime.Now}");
        }

        private static void DumpEventArguments(RevitAPIEventArgs eventArgs)
        {
            if (eventArgs.GetType().Equals(typeof(DocumentPrintingEventArgs)))
            {
                Trace.WriteLine("DocumentPrintingEventArgs Parameters ------>");
                var args = eventArgs as DocumentPrintingEventArgs;
                Trace.WriteLine($"    Event Cancellable   : {args.Cancellable}");
                Trace.WriteLine("    Views to be printed : ");
                DumpViewsInfo(args.Document, args.GetViewElementIds(), "    ");
            }
            else if (eventArgs.GetType().Equals(typeof(DocumentPrintedEventArgs)))
            {
                Trace.WriteLine("DocumentPrintedEventArgs Parameters ------>");
                var args = eventArgs as DocumentPrintedEventArgs;
                Trace.WriteLine($"    Event Status      : {args.Status}");
                Trace.WriteLine($"    Event Cancellable : {args.Cancellable}");
                var ids = args.GetPrintedViewElementIds();
                if (null == ids || 0 == ids.Count)
                {
                    Trace.WriteLine("    Views been printed: <null>");
                }
                else
                {
                    Trace.WriteLine("    Views been printed: ");
                    DumpViewsInfo(args.Document, ids, "      ");
                }

                ids = args.GetFailedViewElementIds();
                if (null == ids || 0 == ids.Count)
                {
                    Trace.WriteLine("    Views failed: <null>");
                }
                else
                {
                    Trace.WriteLine("    Views Failed : ");
                    DumpViewsInfo(args.Document, ids, "      ");
                }
            }
            else if (eventArgs.GetType().Equals(typeof(ViewPrintingEventArgs)))
            {
                Trace.WriteLine("ViewPrintingEventArgs Parameters ------>");
                var args = eventArgs as ViewPrintingEventArgs;
                Trace.WriteLine($"    Event Cancellable   : {args.Cancellable}");
                Trace.WriteLine($"    TotalViews          : {args.TotalViews}");
                Trace.WriteLine($"    View Index          : {args.Index}");
                Trace.WriteLine("    View Information    :");
                DumpViewInfo(args.View, "      ");
            }
            else if (eventArgs.GetType().Equals(typeof(ViewPrintedEventArgs)))
            {
                Trace.WriteLine("ViewPrintedEventArgs Parameters ------>");
                var args = eventArgs as ViewPrintedEventArgs;
                Trace.WriteLine($"    Event Status        : {args.Status}");
                Trace.WriteLine($"    TotalViews          : {args.TotalViews}");
                Trace.WriteLine($"    View Index          : {args.Index}");
                Trace.WriteLine("    View Information    :");
                DumpViewInfo(args.View, "      ");
            }
        }

        private static void DumpViewsInfo(Document activeDoc, IList<ElementId> viewIds, string prefix)
        {
            var index = 0;
            foreach (var id in viewIds)
            {
                if (activeDoc.GetElement(id) is View curView) DumpViewInfo(curView, $"{prefix}#{index++}");
            }
        }

        private static void DumpViewInfo(View view, string prefix)
        {
            Trace.WriteLine($"{prefix} ViewName: {view.Name}, ViewType: {view.ViewType}");
        }
    }
}
