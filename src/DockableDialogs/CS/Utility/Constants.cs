// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    internal class Globals
    {
        public const string ApplicationName = "DockableDialogs";
        public const string DiagnosticsTabName = "DockableDialogs";
        public const string DiagnosticsPanelName = "DockableDialogs Panel";

        public const string RegisterPage = "Register Page";
        public const string ShowPage = "Show Page";
        public const string HidePage = "Hide Page";

        public static DockablePaneId SmUserDockablePaneId =
            new DockablePaneId(new Guid("{3BAFCF52-AC5C-4CF8-B1CB-D0B1D0E90237}"));
    }
}
