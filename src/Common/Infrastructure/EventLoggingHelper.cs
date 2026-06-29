// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Ara3D.RevitSampleBrowser.Common.Geometry;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class EventLoggingHelper
    {
        private static StreamWriter s_writer;

        public static void Log(string msg)
                {
                    var dt = DateTime.Now.ToString("u");
                    Trace.WriteLine($"{dt} {msg}");
                }

        public static void Write(string label, Curve curve)
                {
                    if (s_writer == null)
                        s_writer = new StreamWriter(@"c:\Directions.txt");
                    var start = curve.GetEndPoint(0);
                    var end = curve.GetEndPoint(1);
                    s_writer.WriteLine($"{label} {XyzMath.XyzToString(start)} {XyzMath.XyzToString(end)}");
                }

        public static void CloseFile() => s_writer?.Close();

        public static void ShowDialog(string title, string message)
                {
                    var td = new TaskDialog(title)
                    {
                        MainInstruction = message,
                        TitleAutoPrefix = false
                    };
                    td.Show();
                }

        public static string GetRevitDbEventName(Type type)
                {
                    const string tail = "EventArgs";
                    const string head = "Autodesk.Revit.DB.Events.";
                    var argName = type.ToString();
                    return argName.Substring(head.Length, argName.Length - head.Length - tail.Length);
                }

        public static string GetRevitUiEventName(Type type)
                {
                    const string tail = "EventArgs";
                    const string head = "Autodesk.Revit.UI.Events.";
                    var argName = type.ToString();
                    return argName.Substring(head.Length, argName.Length - head.Length - tail.Length);
                }

        public static string TitleNoExt(string orgTitle)
                {
                    if (string.IsNullOrEmpty(orgTitle)) return "";
                    var pos = orgTitle.LastIndexOf('.');
                    return pos != -1 ? orgTitle.Remove(pos) : orgTitle;
                }

    }
}