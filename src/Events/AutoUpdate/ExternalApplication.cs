// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.AutoUpdate.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        private static readonly string Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private readonly string m_tempFile = Path.Combine(Directory, "temp.log");

        private TextWriterTraceListener m_txtListener;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                CreateTempFile();

                // UI-triggered events must register on ControlledApplication.
                application.ControlledApplication.DocumentOpened += application_DocumentOpened;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= application_DocumentOpened;
            CloseLogFile();
            return Result.Succeeded;
        }

        public void application_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            DumpEventArgs(args);

            var doc = args.Document;

            if (doc.IsFamilyDocument) return;
            try
            {
                // Event handlers must start their own Transaction (Revit does not provide one).
                var eventTransaction = new Transaction(doc, "Event handler modify project information");
                eventTransaction.Start();
                // ProjectInformation is empty on family documents; do not run this sample on them.
                doc.ProjectInformation.Address =
                    "United States - Massachusetts - Waltham - 610 Lincoln St";
                eventTransaction.Commit();
            }
            catch (Exception ee)
            {
                Trace.WriteLine($"Failed to modify project information!-{ee.Message}");
            }

            Trace.WriteLine("The value after running the sample ------>");
            Trace.WriteLine($"    [Address]         :{doc.ProjectInformation.Address}");
        }

        private void DumpEventArgs(DocumentOpenedEventArgs args)
        {
            Trace.WriteLine("DocumentOpenedEventArgs Parameters ------>");
            Trace.WriteLine($"    Event Cancel      : {args.IsCancelled()}");
            Trace.WriteLine($"    Event Cancellable : {args.Cancellable}");
            Trace.WriteLine($"    Status            : {args.Status}");
        }

        private void CreateTempFile()
        {
            if (File.Exists(m_tempFile)) File.Delete(m_tempFile);
            m_txtListener = new TextWriterTraceListener(m_tempFile);
            Trace.AutoFlush = true;
            Trace.Listeners.Add(m_txtListener);
        }

        private void CloseLogFile()
        {
            Trace.Flush();
            Trace.Listeners.Remove(m_txtListener);
            Trace.Close();
            m_txtListener.Close();

            var logFile = Path.Combine(Directory, "AutoUpdate.log");
            if (File.Exists(logFile)) File.Delete(logFile);
            File.Copy(m_tempFile, logFile);
            File.Delete(m_tempFile);
        }
    }
}
