// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RevitMultiSample.SelectionChanged.CS
{
    /// <summary>
    ///     One log file will be created by this class for tracking events raised.
    /// </summary>
    public static class LogManager
    {
        // a trace listener for the output log of SelectionChanged sample
        private static readonly TraceListener TxtListener;

        // the directory where this assembly is in.
        private static readonly string AssemblyLocation =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Static constructor which creates a log file.
        /// </summary>
        static LogManager()
        {
            var logFile = Path.Combine(AssemblyLocation, "SelectionChanged.log");

            // if it already existed, delete the old log file.
            if (File.Exists(logFile)) File.Delete(logFile);

            // Create SelectionChanged.log
            TxtListener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(TxtListener);
            Trace.AutoFlush = true;
        }

        /// <summary>
        ///     Retrieval if regression test is running now.
        ///     If the ExpectedSelectionChanged.log exists in the assembly folder returns true, else returns false.
        /// </summary>
        public static bool RegressionTestNow 
            => File.Exists(Path.Combine(AssemblyLocation, "ExpectedSelectionChanged.log"));

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
        ///     Write specified message into log file.
        /// </summary>
        /// <param name="message">the message which will be written into the log file. </param>
        public static void WriteLog(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
