// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    ///     Form used to display project information
    /// </summary>
    public partial class ProjectInfoForm : Form
    {
        /// <summary>
        ///     Wrapper for ProjectInfo
        /// </summary>
        private readonly ProjectInfoWrapper m_projectInfoWrapper;

        /// <summary>
        ///     Initialize component
        /// </summary>
        public ProjectInfoForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Initialize PropertyGrid
        /// </summary>
        /// <param name="projectInfoWrapper">ProjectInfo wrapper</param>
        public ProjectInfoForm(ProjectInfoWrapper projectInfoWrapper)
            : this()
        {
            m_projectInfoWrapper = projectInfoWrapper;

            // Initialize propertyGrid with CustomDescriptor
            propertyGrid1.SelectedObject = new WrapperCustomDescriptor(m_projectInfoWrapper);
        }
    }
}
