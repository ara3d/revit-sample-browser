// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;

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
            Trace.WriteLine($"[Event] {GetEventName(args.GetType())}: {TitleNoExt(doc.Title)}");
        }

        /// <summary>
        ///     Write specified message into log file.
        /// </summary>
        /// <param name="message">the message which will be written into the log file. </param>
        public static void WriteLog(string message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        ///     Get event name from its EventArgs, without namespace prefix
        /// </summary>
        /// <param name="type">Generic event arguments type.</param>
        /// <returns>the event name</returns>
        private static string GetEventName(Type type)
        {
            var argName = type.ToString();
            var tail = "EventArgs";
            var head = "Autodesk.Revit.DB.Events.";
            var firstIndex = head.Length;
            var length = argName.Length - head.Length - tail.Length;
            var eventName = argName.Substring(firstIndex, length);
            return eventName;
        }

        /// <summary>
        ///     This method will remove the extension name of file name(if have).
        ///     Document.Title will return title of project depends on OS setting:
        ///     If we choose show extension name by IE:Tools\Folder Options, then the title will end with accordingly extension
        ///     name.
        ///     If we don't show extension, the Document.Title will only return file name without extension.
        /// </summary>
        /// <param name="orgTitle">Origin file name to be revised.</param>
        /// <returns>New file name without extension name.</returns>
        private static string TitleNoExt(string orgTitle)
        {
            // return null directly if it's null
            if (string.IsNullOrEmpty(orgTitle)) return "";

            // Remove the extension 
            var pos = orgTitle.LastIndexOf('.');
            return -1 != pos ? orgTitle.Remove(pos) : orgTitle;
        }
    }
}
