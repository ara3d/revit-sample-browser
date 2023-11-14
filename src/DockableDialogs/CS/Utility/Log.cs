// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Diagnostics;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    public class Log
    {
        public static void Message(string message, int level = 0)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            if (level > 0)
                TaskDialog.Show("Revit", message);
        }
    }
}
