// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.Events.AutoStamp.CS
{
    /// <summary>ViewPrinting creates a TextNote; ViewPrinted deletes it and logs to PrintEventsLog.txt.</summary>
    public sealed class EventsReactor
    {
        private readonly string m_assemblyPath;

        private TextWriterTraceListener m_eventsLog;

        private ElementId m_newTextNoteId;

        public EventsReactor()
        {
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public void CloseLogFiles()
        {
            Trace.Flush();
            Trace.Close();
            Trace.Flush();
            if (null != m_eventsLog)
            {
                Trace.Listeners.Remove(m_eventsLog);
                m_eventsLog.Flush();
                m_eventsLog.Close();
            }
        }

        public void AppViewPrinting(object sender, ViewPrintingEventArgs e)
        {
            if (null == m_eventsLog) SetupLogFiles();
            Trace.WriteLine($"{Environment.NewLine}View Print Start: ------------------------");
            DumpEventArguments(e);

            var failureOccured = false;
            try
            {
                var strText = string.Format("Printer Name: {0}{1}User Name: {2}",
                    e.Document.PrintManager.PrinterName, Environment.NewLine, Environment.UserName);
#if !(Debug || DEBUG)
                // Release builds use fixed text so PrintEventsLog.txt compares cleanly across machines.
                strText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
#endif
                using (var eventTransaction = new Transaction(e.Document, "External Tool"))
                {
                    eventTransaction.Start();
                    var options = new TextNoteOptions
                    {
                        HorizontalAlignment = HorizontalTextAlignment.Center,
                        TypeId = e.Document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType)
                    };
                    var newTextNote = TextNote.Create(e.Document, e.View.Id, XYZ.Zero, strText, options);
                    eventTransaction.Commit();

                    if (null != newTextNote)
                    {
                        Trace.WriteLine("Create TextNote element successfully...");
                        m_newTextNoteId = newTextNote.Id;
                    }
                    else
                    {
                        failureOccured = true;
                    }
                }
            }
            catch (Exception ex)
            {
                failureOccured = true;
                Trace.WriteLine($"Exception occurred when creating TextNote, print will be canceled, ex: {ex.Message}");
            }
            finally
            {
                if (failureOccured && e.Cancellable) e.Cancel();
            }
        }

        public void AppViewPrinted(object sender, ViewPrintedEventArgs e)
        {
            Trace.WriteLine($"{Environment.NewLine}View Print End: -------");
            DumpEventArguments(e);

            // Cancelled prints never created the TextNote.
            if (RevitAPIEventStatus.Cancelled != e.Status)
            {
                // Event handlers must start their own Transaction (Revit does not provide one).
                var eventTransaction = new Transaction(e.Document, "External Tool");
                eventTransaction.Start();
                e.Document.Delete(m_newTextNoteId);
                eventTransaction.Commit();
                Trace.WriteLine("Succeeded to delete the created TextNote element.");
            }
        }

        private void SetupLogFiles()
        {
            if (null != m_eventsLog) return;

            var printEventsLogFile = Path.Combine(m_assemblyPath, "PrintEventsLog.txt");
            if (File.Exists(printEventsLogFile)) File.Delete(printEventsLogFile);

            m_eventsLog = new TextWriterTraceListener(printEventsLogFile);
            Trace.Listeners.Add(m_eventsLog);
            Trace.AutoFlush = true;
        }

        private static void DumpEventArguments(RevitAPIEventArgs eventArgs)
        {
            if (eventArgs.GetType().Equals(typeof(ViewPrintingEventArgs)))
            {
                Trace.WriteLine("ViewPrintingEventArgs Parameters ------>");
                var args = eventArgs as ViewPrintingEventArgs;
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

        private static void DumpViewInfo(View view, string prefix)
        {
            Trace.WriteLine(string.Format("{0} ViewName: {1}, ViewType: {2}",
                prefix, view.Name, view.ViewType));
        }
    }
}
