// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    public class LogManager
    {
        private readonly DataTable m_eventsLog;

        private readonly string m_filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Temp file is copied to EventsMonitor.log on close so the log stays readable while Revit runs.
        private string m_tempFile;

        private TraceListener m_txtListener;

        public LogManager()
        {
            CreateLogFile();
            m_eventsLog = CreateEventsLogTable();
        }

        public DataTable EventsLog => m_eventsLog;

        private void CreateLogFile()
        {
            m_tempFile = Path.Combine(m_filePath, "Temp.log");
            if (File.Exists(m_tempFile)) File.Delete(m_tempFile);
            m_txtListener = new TextWriterTraceListener(m_tempFile);
            Trace.Listeners.Add(m_txtListener);
        }

        public void CloseLogFile()
        {
            Trace.Flush();
            Trace.Listeners.Remove(m_txtListener);
            Trace.Close();
            m_txtListener.Close();

            var log = Path.Combine(m_filePath, "EventsMonitor.log");
            if (File.Exists(log)) File.Delete(log);
            File.Copy(m_tempFile, log);
            File.Delete(m_tempFile);
        }

        private DataTable CreateEventsLogTable()
        {
            var eventsInfoLogTable = new DataTable("EventsLogInfoTable");

            var timeColumn = new DataColumn("Time", typeof(string))
            {
                Caption = "Time"
            };
            eventsInfoLogTable.Columns.Add(timeColumn);

            var eventColumn = new DataColumn("Event", typeof(string))
            {
                Caption = "Event"
            };
            eventsInfoLogTable.Columns.Add(eventColumn);

            var typeColumn = new DataColumn("Type", typeof(string))
            {
                Caption = "Type"
            };
            eventsInfoLogTable.Columns.Add(typeColumn);

            return eventsInfoLogTable;
        }

        public void TrackEvent(object sender, EventArgs args)
        {
            var newRow = m_eventsLog.NewRow();

            newRow["Time"] = DateTime.Now.ToString();
            newRow["Event"] = EventLoggingHelper.GetRevitDbEventName(args.GetType());
            newRow["Type"] = sender.GetType().ToString();

            m_eventsLog.Rows.Add(newRow);
        }

        public void WriteLogFile(object sender, EventArgs args)
        {
            Trace.WriteLine("*********************************************************");
            if (null == args) return;
            var type = args.GetType();
            var eventName = EventLoggingHelper.GetRevitDbEventName(type);
            Trace.WriteLine($"Raised {sender.GetType()}.{eventName}");
            Trace.WriteLine("---------------------------------------------------------");

            Trace.WriteLine("  Start to dump Sender and EventAgrs of Event...\n");
            if (null != sender)
                Trace.WriteLine($"    [Event Sender]: {sender.GetType().FullName}");
            else
                Trace.WriteLine("      Sender is null, it's unexpected!!!");

            var propertyInfos = type.GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    if (!propertyInfo.CanRead) continue;

                    var propertyName = propertyInfo.Name;
                    switch (propertyName)
                    {
                        case "Document":
                        case "Cancellable":
                        case "Cancel":
                        case "Status":
                        case "DocumentType":
                        case "Format":
                            var propertyValue = propertyInfo.GetValue(args, null);
                            Trace.WriteLine($"    [Property]: {propertyInfo.Name}");
                            Trace.WriteLine($"    [Value]: {propertyValue}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"    [Property Exception]: {propertyInfo.Name}, {ex.Message}");
                }
            }
        }
    }
}
