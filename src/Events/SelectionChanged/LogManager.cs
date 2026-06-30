// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.Events.SelectionChanged.CS
{
    public static class LogManager
    {
        private static readonly TraceListener TxtListener;

        private static readonly string AssemblyLocation =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static LogManager()
        {
            var logFile = Path.Combine(AssemblyLocation, "SelectionChanged.log");

            if (File.Exists(logFile)) File.Delete(logFile);

            TxtListener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(TxtListener);
            Trace.AutoFlush = true;
        }

        /// <summary>True when ExpectedSelectionChanged.log is present (regression test mode).</summary>
        public static bool RegressionTestNow
            => File.Exists(Path.Combine(AssemblyLocation, "ExpectedSelectionChanged.log"));

        public static void LogFinalize()
        {
            Trace.Flush();
            TxtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(TxtListener);
        }

        public static void WriteLog(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
