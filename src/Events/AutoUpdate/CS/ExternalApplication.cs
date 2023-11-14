// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace RevitMultiSample.AutoUpdate.CS
{
    /// <summary>
    ///     This class implements IExternalApplication and demonstrate how to subscribe event
    ///     and modify model in event handler. We just demonstrate changing
    ///     the ProjectInformation.Address property, but user can do other modification for
    ///     current project, like delete element, create new element, etc.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        /// <summary>
        ///     get assembly path.
        /// </summary>
        private static readonly string Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     create the temp file name.
        /// </summary>
        private readonly string m_tempFile = Path.Combine(Directory, "temp.log");

        /// <summary>
        ///     This listener is used to monitor the events arguments and the result of the sample.
        ///     It will be bound to log file AutoUpdate.log, it will be added to Trace.Listeners.
        /// </summary>
        private TextWriterTraceListener m_txtListener;

        /// <summary>
        ///     Implement this method to subscribe event.
        /// </summary>
        /// <param name="application">the controlled application.</param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the internal reference.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Create a temp file to dump the log, and copy this file to log file 
                // at the end of the sample.
                CreateTempFile();

                // Register event. In this sample, we trigger this event from UI, so it must 
                // be registered on ControlledApplication. 
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
            // remove the event.
            application.ControlledApplication.DocumentOpened -= application_DocumentOpened;
            CloseLogFile();
            return Result.Succeeded;
        }

        /// <summary>
        ///     This is the event handler. We modify the model and dump log in this method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void application_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            // dump the args to log file.
            DumpEventArgs(args);

            // get document from event args.
            var doc = args.Document;

            if (doc.IsFamilyDocument) return;
            try
            {
                //now event framework will not provide transaction,user need start by self(2009/11/18)
                var eventTransaction = new Transaction(doc, "Event handler modify project information");
                eventTransaction.Start();
                // assign specified value to ProjectInformation.Address property. 
                // User can change another properties of document or create(delete) something as he likes.
                // Please pay attention that ProjectInformation property is empty for family document.
                // But it isn't the correct usage. So please don't run this sample on family document.
                doc.ProjectInformation.Address =
                    "United States - Massachusetts - Waltham - 610 Lincoln St";
                eventTransaction.Commit();
            }
            catch (Exception ee)
            {
                Trace.WriteLine("Failed to modify project information!-" + ee.Message);
            }

            // write the value to log file to check whether the operation is successful.
            Trace.WriteLine("The value after running the sample ------>");
            Trace.WriteLine("    [Address]         :" + doc.ProjectInformation.Address);
        }

        /// <summary>
        ///     Dump the events arguments to log file: AutoUpdat.log.
        /// </summary>
        /// <param name="args"></param>
        private void DumpEventArgs(DocumentOpenedEventArgs args)
        {
            Trace.WriteLine("DocumentOpenedEventArgs Parameters ------>");
            Trace.WriteLine("    Event Cancel      : " + args.IsCancelled()); // is it be cancelled?
            Trace.WriteLine("    Event Cancellable : " + args.Cancellable); // Cancellable
            Trace.WriteLine("    Status            : " + args.Status); // Status
        }

        /// <summary>
        ///     Create a log file to track the subscribed events' work process.
        /// </summary>
        private void CreateTempFile()
        {
            if (File.Exists(m_tempFile)) File.Delete(m_tempFile);
            m_txtListener = new TextWriterTraceListener(m_tempFile);
            Trace.AutoFlush = true;
            Trace.Listeners.Add(m_txtListener);
        }

        /// <summary>
        ///     Close the log file and remove the corresponding listener.
        /// </summary>
        private void CloseLogFile()
        {
            // close listeners now.
            Trace.Flush();
            Trace.Listeners.Remove(m_txtListener);
            Trace.Close();
            m_txtListener.Close();

            // copy temp file to log file and delete the temp file.
            var logFile = Path.Combine(Directory, "AutoUpdate.log");
            if (File.Exists(logFile)) File.Delete(logFile);
            File.Copy(m_tempFile, logFile);
            File.Delete(m_tempFile);
        }
    }
}
