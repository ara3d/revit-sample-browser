// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace RevitMultiSample.PrintLog.CS
{
    /// <summary>
    ///     This class has four handler methods which will be subscribed to ViewPrint and DocumentPrint events separately,
    ///     It contains other methods which are used to dump related information to log files.
    ///     The handler methods will be used to dump designed information to log files when they are
    ///     raised by Print operations(both UI and API).
    ///     This class contains two log file handlers m_printLog and m_eventsLog, they will dump information
    ///     to PrintLog.txt and PrintEventsLog.txt separately.
    ///     PrintEventsLog.txt only contains the information of events, like events arguments and sequences.
    ///     The PrintLog.txt contains all information of print, date time stamp and cost time of print are dumped especially.
    /// </summary>
    public sealed class EventsReactor
    {
        /// <summary>
        ///     Current assembly path
        /// </summary>
        private readonly string m_assemblyPath;

        /// <summary>
        ///     This member will be used to reserve document and accordingly timers of current events.
        ///     Because events are registered in controlled application, there are maybe more than one document.
        ///     The int value is hash code of document, it's unique for each document.
        ///     The events watches will reserve the start times of ViewPrint and DocumentPrint and then calculate the
        ///     individual and total times for each view and all views.
        ///     Note:
        ///     How to identify document, maybe you want to use PathName or Title, but they don't work:
        ///     . PathName is not accessible for newly created document, exception will thrown once you access it.
        ///     . Title may be duplicated if two .rvt with same name were opened.
        ///     Though Microsoft says hash code does not guarantee unique return values for different objects.
        ///     (refer to http://msdn.microsoft.com/en-us/library/system.object.gethashcode.aspx)
        ///     In RevitAPI, hash code can be used for identifying document because API will guarantee the same
        ///     and only one CLI object each time access them.
        /// </summary>
        private readonly Dictionary<int, EventsWatches> m_docEventsWatches;

        /// <summary>
        ///     This listener is used to monitor the events raising sequences and arguments of events.
        ///     it will be bound to log file PrintEventsLog.txt, it will be added to Trace.Listeners.
        ///     This log file will only contain information of event raising sequence, event arguments, etc.
        ///     Any information which is related to time stamp won't be dumped to this file.
        ///     This file can be used to check if events work well in different platforms, for example:
        ///     By this sample, if user printed something, Revit journal will record all operation of users,
        ///     meanwhile PrintEventsLog.txt will be generated. If user run the journal in other machine, user will get another
        ///     PrintEventsLog.txt, by comparing the two files user can figure out easily if the two prints work equally.
        /// </summary>
        private TextWriterTraceListener m_eventsLog;

        /// <summary>
        ///     This listener is used to print all required information to print log and it it'll be bound to log file
        ///     PrintLog.txt.
        ///     It will be added to Trace.Listeners.
        ///     Detailed information of print, like: Printer name, views to-be-printed or printed,
        ///     Print times of view and document, will be dumped to that log file.
        ///     By this log file user can easily find the detailed information of print process.
        /// </summary>
        private TextWriterTraceListener m_printLog;

        /// <summary>
        ///     Constructor method
        ///     This method will only initialize the m_docEventsWatches and m_assemblyPath.
        ///     Notice that this method won't open log files.
        /// </summary>
        public EventsReactor()
        {
            // Members initialization
            m_docEventsWatches = new Dictionary<int, EventsWatches>();
            // 
            // Get assembly path 
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        ///     Close log files now
        /// </summary>
        public void CloseLogFiles()
        {
            // Flush trace and close it
            Trace.Flush();
            Trace.Close();
            //
            // Close listeners 
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

        /// <summary>
        ///     Handler method for DocumentPrinting event.
        ///     This method will dump printer name, views to be printed and user name, etc.
        ///     Besides, this handler will reserve the start time of whole print process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppDocumentPrinting(object sender, DocumentPrintingEventArgs e)
        {
            // ensure log files are specified and dump header information
            SetupLogFiles();
            //
            // Dump environment of print: user name, printer name and project title.
            DumpPrintEnv(Environment.UserName, e.Document.PrintManager.PrinterName, e.Document.Title);
            //
            // Start new watch for DocumentPrint
            Trace.WriteLine(Environment.NewLine + "Document Print Start: >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            StartNewWatch(e.Document, false);
            //
            // Dump the events arguments 
            DumpEventArguments(e);
        }

        /// <summary>
        ///     Handler method for ViewPrinting event.
        ///     This method will dump detailed information of view, like view type, id and start start time of print for this view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppViewPrinting(object sender, ViewPrintingEventArgs e)
        {
            // header information
            Trace.WriteLine(Environment.NewLine + "View Print Start: -----------------------------------------------");
            //
            // Start new watch for ViewPrint
            StartNewWatch(e.Document, true);
            //
            // Dump the events arguments 
            DumpEventArguments(e);
        }

        /// <summary>
        ///     Handler method for ViewPrinted event.
        ///     This handler will dump information of printed view, like View name, type and end time of print.
        ///     Besides, It will calculate cost time of print for this view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppViewPrinted(object sender, ViewPrintedEventArgs e)
        {
            // header information
            Trace.WriteLine(Environment.NewLine + "View Print End: -------");
            //
            // Stop watch and calculate the cost time
            StopWatch(e.Document, true);
            //
            // Dump the events arguments 
            DumpEventArguments(e);
        }

        /// <summary>
        ///     Handler method for DocumentPrinted event.
        ///     This method will dump all views printed and failed views(if any), total for all print.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppDocumentPrinted(object sender, DocumentPrintedEventArgs e)
        {
            // header information
            Trace.WriteLine(Environment.NewLine + "Document Print End: <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            //
            // Stop watch and calculate the cost time
            StopWatch(e.Document, false);
            //
            // Dump the events arguments 
            DumpEventArguments(e);
        }

        /// <summary>
        ///     For singleton consideration, setup log file only when events are raised.
        ///     m_printLog and m_eventsLog will be initialized and added to Trace.Listeners
        ///     PrintLog.txt and PrintEventsLog.txt will be removed if existed.
        /// </summary>
        private void SetupLogFiles()
        {
            // singleton consideration
            if (null != m_printLog && null != m_eventsLog) return;
            //
            // delete existed log files
            var printLogFile = Path.Combine(m_assemblyPath, "PrintLog.txt");
            var printEventsLogFile = Path.Combine(m_assemblyPath, "PrintEventsLog.txt");
            if (File.Exists(printLogFile)) File.Delete(printLogFile);
            if (File.Exists(printEventsLogFile)) File.Delete(printEventsLogFile);
            //
            // Create listeners and add to Trace.Listeners to monitor the string to be emitted
            m_printLog = new TextWriterTraceListener(printLogFile);
            m_eventsLog = new TextWriterTraceListener(printEventsLogFile);
            Trace.Listeners.Add(m_printLog);
            Trace.Listeners.Add(m_eventsLog);
            Trace.AutoFlush = true; // set auto flush to ensure the emitted string can be dumped in time
        }

        /// <summary>
        ///     Start to dump current date/time and start watch count.
        /// </summary>
        /// <param name="curDoc">Current document.</param>
        /// <param name="isViewWatch">
        ///     Indicates if current watch is for view or document.
        ///     True means we need to start watch for ViewPrint, else start watch for DocumentPrint.
        /// </param>
        private void StartNewWatch(Document curDoc, bool isViewWatch)
        {
            // Just dump current date time to printlog.txt
            DumpDateTime("Start");
            //
            // Start new watch for view print or document print
            EventsWatches watches;
            var result = m_docEventsWatches.TryGetValue(curDoc.GetHashCode(), out watches);
            if (!result || null == watches)
            {
                watches = new EventsWatches();
                m_docEventsWatches.Add(curDoc.GetHashCode(), watches);
            }

            //
            // Start watch for ViewPrint and DocumentPrint
            if (isViewWatch)
                watches.ViewPrintWatch = Stopwatch.StartNew();
            else
                watches.DocPrintWatch = Stopwatch.StartNew();
        }

        /// <summary>
        ///     Stop watch for print and then calculate the time cost,
        ///     Besides, it will dump current date time to PrintLog.txt.
        /// </summary>
        /// <param name="curDoc"></param>
        /// <param name="isViewWatch"></param>
        private void StopWatch(Document curDoc, bool isViewWatch)
        {
            // Just dump current date time to printlog.txt
            DumpDateTime("End");
            //
            // Calculate the elapse time print for this view
            EventsWatches watches;
            var result = m_docEventsWatches.TryGetValue(curDoc.GetHashCode(), out watches);
            if (!result)
            {
                Trace.Write("Failed to find the watch, time calculation is skipped.");
                return;
            }

            //
            // Dump the cost time to PrintLog.txt
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

        /// <summary>
        ///     Dump global environment of print: user, printer and project name.
        ///     This only will be dumped to PrintLog.txt.
        /// </summary>
        /// <param name="user">User name who prints current document.</param>
        /// <param name="printer">Printer name.</param>
        /// <param name="projectName">Current document title.</param>
        private void DumpPrintEnv(string user, string printer, string projectName)
        {
            m_printLog.WriteLine("Start to Print..................................................");
            m_printLog.WriteLine(">> Print user: " + user);
            m_printLog.WriteLine(">> Printer name: " + printer);
            m_printLog.WriteLine(">> Project name: " + projectName);
        }

        /// <summary>
        ///     This method will dump date time to log file, but it will only dump to PrintLog.txt file.
        ///     The date/time stamp should not be dumped to PrintEventsLog.txt.
        /// </summary>
        /// <param name="prefix">Prefix string to be dumped to log file.</param>
        private void DumpDateTime(string prefix)
        {
            // ensure log file has been specified
            SetupLogFiles();
            m_printLog.WriteLine($"{prefix} Time: {DateTime.Now}");
        }

        /// <summary>
        ///     Dump the events arguments to log files: PrintLog.txt and PrintEventsLog.txt.
        ///     This method will only dump EventArguments of ViewPrint and DocumentPrint,
        ///     that's, 4 event arguments will be handled here:
        ///     . DocumentPrintingEventArgs
        ///     . DocumentPrintedEventArgs
        ///     . ViewPrintingEventArgs
        ///     . ViewPrintedEventArgs
        /// </summary>
        /// <param name="eventArgs">Event argument to be dumped. </param>
        private static void DumpEventArguments(RevitAPIEventArgs eventArgs)
        {
            // Dump parameters now:
            // white space is for align purpose.
            if (eventArgs.GetType().Equals(typeof(DocumentPrintingEventArgs)))
            {
                Trace.WriteLine("DocumentPrintingEventArgs Parameters ------>");
                var args = eventArgs as DocumentPrintingEventArgs;
                Trace.WriteLine("    Event Cancellable   : " + args.Cancellable); // cancellable
                Trace.WriteLine("    Views to be printed : "); // Views
                DumpViewsInfo(args.Document, args.GetViewElementIds(), "    ");
            }
            else if (eventArgs.GetType().Equals(typeof(DocumentPrintedEventArgs)))
            {
                Trace.WriteLine("DocumentPrintedEventArgs Parameters ------>");
                var args = eventArgs as DocumentPrintedEventArgs;
                Trace.WriteLine("    Event Status      : " + args.Status); // Status
                Trace.WriteLine("    Event Cancellable : " + args.Cancellable); // Cancellable 
                //
                // PrintedViews
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

                //
                // FailedViews
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
                Trace.WriteLine("    Event Cancellable   : " + args.Cancellable); // Cancellable
                Trace.WriteLine("    TotalViews          : " + args.TotalViews); // TotalViews
                Trace.WriteLine("    View Index          : " + args.Index); // Index
                Trace.WriteLine("    View Information    :"); // View
                DumpViewInfo(args.View, "      ");
            }
            else if (eventArgs.GetType().Equals(typeof(ViewPrintedEventArgs)))
            {
                Trace.WriteLine("ViewPrintedEventArgs Parameters ------>");
                var args = eventArgs as ViewPrintedEventArgs;
                Trace.WriteLine("    Event Status        : " + args.Status); // Cancellable
                Trace.WriteLine("    TotalViews          : " + args.TotalViews); // TotalViews
                Trace.WriteLine("    View Index          : " + args.Index); // Index
                Trace.WriteLine("    View Information    :"); // View
                DumpViewInfo(args.View, "      ");
            }
            // no handling for other argument
        }

        /// <summary>
        ///     Dump information of views: ViewType, Id and ViewName.
        ///     The information will be dumped to both PrintLog.txt and PrintEventsLog.txt.
        /// </summary>
        /// <param name="Document">Current active document.</param>
        /// <param name="viewIds">Views to be dumped to log files.</param>
        /// <param name="prefix">Prefix mark for each line dumped to log files.</param>
        private static void DumpViewsInfo(Document activeDoc, IList<ElementId> viewIds, string prefix)
        {
            var index = 0;
            foreach (var id in viewIds)
            {
                if (activeDoc.GetElement(id) is View curView) DumpViewInfo(curView, $"{prefix}#{index++}");
            }
        }

        /// <summary>
        ///     Dump information of single view: ViewType and ViewName.
        ///     The information will be dumped to both PrintLog.txt and PrintEventsLog.txt.
        /// </summary>
        /// <param name="view">View element to be dumped to log files.</param>
        /// <param name="prefix">Prefix mark for each line dumped to log files.</param>
        private static void DumpViewInfo(View view, string prefix)
        {
            Trace.WriteLine($"{prefix} ViewName: {view.Name}, ViewType: {view.ViewType}");
        }
    }
}
