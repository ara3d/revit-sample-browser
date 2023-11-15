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

        /// <summary>
        ///     constructor with Set = false
        /// </summary>
        /// <param name="dataBuffer">The reference of Loads</param>
        /// <param name="name">The value set to Name property</param>
        public UsageMap(Loads dataBuffer, string name)
        {
            m_dataBuffer = dataBuffer;
            Set = false;
            m_name = name;
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer">The reference of Loads</param>
        /// <param name="set">The value set to Set property</param>
        /// <param name="name">The value set to Name property</param>
        public UsageMap(Loads dataBuffer, bool set, string name)
        {
            m_dataBuffer = dataBuffer;
            Set = set;
            m_name = name;
        }

        /// <summary>
        ///     is selected in Usage DataGridView control
        /// </summary>
        public bool Set { get; set; }

        /// <summary>
        ///     usage name
        /// </summary>
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
