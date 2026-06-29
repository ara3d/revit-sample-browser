// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.Events.CancelSave.CS
{
    /// <summary>
    ///     One log file will be created by this class for tracking events raise.
    /// </summary>
    public static class LogManager
    {
        // a trace listener for the output log of CancelSave sample
        private static readonly TraceListener TxtListener;

        // the directory where this assembly in.
        private static readonly string AssemblyLocation =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Static constructor which creates a log file.
        /// </summary>
        static LogManager()
        {
            // Create CancelSave.log .
            var actullyLogFile = Path.Combine(AssemblyLocation, "CancelSave.log");

            // if already existed, delete it.
            if (File.Exists(actullyLogFile)) File.Delete(actullyLogFile);

            TxtListener = new TextWriterTraceListener(actullyLogFile);
            Trace.Listeners.Add(TxtListener);
            Trace.AutoFlush = true;
        }

        /// <summary>
        ///     Retrieval if doing regression test now.
        ///     If the ExpectedOutPut.log exists in the assembly folder returns true, else returns false.
        /// </summary>
        public static bool RegressionTestNow 
            => File.Exists(Path.Combine(AssemblyLocation, "ExpectedOutPut.log"));

        /// <summary>
        ///     Finalize and close the output log.
        /// </summary>
        public static void LogFinalize()
        {
            Trace.Flush();
            TxtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(TxtListener);
        }

        /// <summary>
        ///     Write log to file: which event occurred in which document.
        /// </summary>
        /// <param name="args">Event arguments that contains the event data.</param>
        /// <param name="doc">document in which the event is occur.</param>
        public static void WriteLog(EventArgs args, Document doc)
        {
            Trace.WriteLine("");
            Trace.WriteLine($"[Event] {EventLoggingHelper.GetRevitDbEventName(args.GetType())}: {EventLoggingHelper.TitleNoExt(doc.Title)}");
        }

        /// <summary>
        ///     Write specified message into log file.
        /// </summary>
        /// <param name="message">the message which will be written into the log file. </param>
        public static void WriteLog(string message) => Trace.WriteLine(message);
    }
}
