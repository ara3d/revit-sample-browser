// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents;
using Autodesk.Revit.UI;
using System;
using System.Text;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.APIUtility
{
    public partial class ApiUtility
    {
        private UIApplication m_uiApplication;

        public ModelessCommand ModelessCommand { get; } = new ModelessCommand();

        public void Initialize(UIApplication uiApplication)
        {
            m_uiApplication = uiApplication;
        }

        public string GetPaneSummary(string paneGuidString)
        {
            Guid paneGuid;
            try
            {
                paneGuid = new Guid(paneGuidString);
            }
            catch (Exception)
            {
                return "Invalid Guid";
            }

            DockablePaneId paneId = new(paneGuid);
            return GetPaneSummary(paneId);
        }

        public string GetPaneSummary(DockablePaneId id)
        {
            try
            {
                var pane = m_uiApplication.GetDockablePane(id);
                return GetPaneSummary(pane);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GetPaneSummary(DockablePane pane)
        {
            StringBuilder sb = new();
            sb.AppendLine($"-RevitDockablePane- Title: {pane.GetTitle()}, Id-Guid: {pane.Id.Guid}");
            return sb.ToString();
        }

        public static string GetDockStateSummary(DockablePaneState paneState)
        {
            StringBuilder sb = new();
            sb.AppendLine(" -DockablePaneState-");
            sb.AppendLine($" Left: {paneState.FloatingRectangle.Left}");
            sb.AppendLine($" Right: {paneState.FloatingRectangle.Right}");
            sb.AppendLine($" Top: {paneState.FloatingRectangle.Top}");
            sb.AppendLine($" Bottom: {paneState.FloatingRectangle.Bottom}");
            sb.AppendLine($" Position: {paneState.DockPosition}");
            sb.AppendLine($" Tab target guid:{paneState.TabBehind.Guid}");
            return sb.ToString();
        }
    }
}
