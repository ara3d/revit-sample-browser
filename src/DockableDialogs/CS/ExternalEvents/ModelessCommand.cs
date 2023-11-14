// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    /// <summary>
    ///     A thread-safe class for getting and setting modeless-command data.
    /// </summary>
    public class ModelessCommand
    {
        private ModelessCommandData m_data = new ModelessCommandData();

        /// <summary>
        ///     Set data into the command.
        /// </summary>
        public void Make(ModelessCommandData commandData)
        {
            lock (this)
            {
                m_data = commandData;
            }
        }

        /// <summary>
        ///     Get data from the command.
        /// </summary>
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
