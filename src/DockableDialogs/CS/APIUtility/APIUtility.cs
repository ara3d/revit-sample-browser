// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Text;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.APIUtility
{
    /// <summary>
    ///     A simple class to manage an interface into the Revit API.
    /// </summary>
    public partial class ApiUtility
    {
        private UIApplication m_uiApplication;

        /// <summary>
        ///     Get the current Modeless command.
        /// </summary>
        public ModelessCommand ModelessCommand { get; } = new ModelessCommand();

        /// <summary>
        ///     Store a reference to the application.
        /// </summary>
        public void Initialize(UIApplication uiApplication)
        {
            m_uiApplication = uiApplication;
        }

        /// <summary>
        ///     Return dockable pane inforamtion, given a dockable pane Guid.
        /// </summary>
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

            var paneId = new DockablePaneId(paneGuid);
            return GetPaneSummary(paneId);
        }

        /// <summary>
        ///     Return dockable pane inforamtion, given a DockablePaneId
        /// </summary>
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

        /// <summary>
        ///     Return dockable pane inforamtion, given a DockablePaneId
        /// </summary>
        public static string GetPaneSummary(DockablePane pane)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-RevitDockablePane- Title: " + pane.GetTitle() + ", Id-Guid: " + pane.Id.Guid);
            return sb.ToString();
        }

        /// <summary>
        ///     Display docking state information as a string.
        /// </summary>
        public static string GetDockStateSummary(DockablePaneState paneState)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" -DockablePaneState-");
            sb.AppendLine(" Left: " + paneState.FloatingRectangle.Left);
            sb.AppendLine(" Right: " + paneState.FloatingRectangle.Right);
            sb.AppendLine(" Top: " + paneState.FloatingRectangle.Top);
            sb.AppendLine(" Bottom: " + paneState.FloatingRectangle.Bottom);
            sb.AppendLine(" Position: " + paneState.DockPosition);
            sb.AppendLine(" Tab target guid:" + paneState.TabBehind.Guid);
            return sb.ToString();
        }
    }
}
