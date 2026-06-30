// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents
{
    public class ModelessCommand
    {
        private ModelessCommandData m_data = new();

        public void Make(ModelessCommandData commandData)
        {
            lock (this)
            {
                m_data = commandData;
            }
        }

        public ModelessCommandData Take()
        {
            lock (this)
            {
                return m_data;
            }
        }
    }

    public enum ModelessCommandType
    {
        PrintMainPageStatistics,
        PrintSelectedPageStatistics,
        Return
    }

    public class ModelessCommandData
    {
        public ModelessCommandType CommandType;
        public string SelectedPaneId;
        public string WindowSummaryData;
    }
}
