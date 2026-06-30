// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    public partial class ProjectInfoForm : Form
    {
        private readonly ProjectInfoWrapper m_projectInfoWrapper;

        public ProjectInfoForm()
        {
            InitializeComponent();
        }

        public ProjectInfoForm(ProjectInfoWrapper projectInfoWrapper)
            : this()
        {
            m_projectInfoWrapper = projectInfoWrapper;

            // Initialize propertyGrid with CustomDescriptor
            propertyGrid1.SelectedObject = new WrapperCustomDescriptor(m_projectInfoWrapper);
        }
    }
}
