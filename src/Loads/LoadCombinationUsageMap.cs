// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Loads.CS
{
    /// <summary>
    ///     The map class which store the data and display in usageDataGridView
    /// </summary>
    public class UsageMap
    {
        // Private Members
        private readonly Loads m_dataBuffer; // A reference of Loads
        private string m_name; // Indicate the name column of Usage DataGridView control

        /// <summary>
        ///     Constructor with Set = false, Name="",
        ///     This should not be called.
        /// </summary>
        /// <param name="dataBuffer">The reference of Loads</param>
        public UsageMap(Loads dataBuffer)
        {
            m_dataBuffer = dataBuffer;
        }

        public UsageMap(Loads dataBuffer, string name)
        {
            m_dataBuffer = dataBuffer;
            Set = false;
            m_name = name;
        }

        public UsageMap(Loads dataBuffer, bool set, string name)
        {
            m_dataBuffer = dataBuffer;
            Set = set;
            m_name = name;
        }

        public bool Set { get; set; }

        public string Name
        {
            get => m_name;
            set
            {
                if (null == value)
                {
                    TaskDialog.Show("Revit", "The usage name should not be null.");
                    return;
                }

                if (null == m_name)
                {
                    m_name = value;
                    return;
                }

                var canModify = m_dataBuffer.ModifyUsageName(m_name, value);
                if (canModify) m_name = value;
            }
        }
    }
}
