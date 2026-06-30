// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace Ara3D.RevitSampleBrowser.Events.CancelSave.CS
{
    public static class LogManager
    {
        private static readonly TraceListener TxtListener;

        private static readonly string AssemblyLocation =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static LogManager()
        {
            var actualLogFile = Path.Combine(AssemblyLocation, "CancelSave.log");

            if (File.Exists(actualLogFile)) File.Delete(actualLogFile);

            TxtListener = new TextWriterTraceListener(actualLogFile);
            Trace.Listeners.Add(TxtListener);
            Trace.AutoFlush = true;
        }

        /// <summary>True when ExpectedOutPut.log is present (regression test mode).</summary>
        public static bool RegressionTestNow
            => File.Exists(Path.Combine(AssemblyLocation, "ExpectedOutPut.log"));

        public static void LogFinalize()
        {
            Trace.Flush();
            TxtListener.Close();
            Trace.Close();
            Trace.Listeners.Remove(TxtListener);
        }

        public static void WriteLog(EventArgs args, Document doc)
        {
            Trace.WriteLine("");
            Trace.WriteLine($"[Event] {EventLoggingHelper.GetRevitDbEventName(args.GetType())}: {EventLoggingHelper.TitleNoExt(doc.Title)}");
        }

        public static void WriteLog(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
