// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Selections.CS
{
    public partial class SelectionForm : Form
    {
        private readonly SelectionManager m_manager;

        public SelectionForm(SelectionManager manager)
        {
            InitializeComponent();
            m_manager = manager;
        }

        private void PickElementButton_Click(object sender, EventArgs e)
        {
            m_manager.SelectionType = SelectionType.Element;
            Close();
        }

        private void MoveToButton_Click(object sender, EventArgs e)
        {
            m_manager.SelectionType = SelectionType.Point;
            Close();
        }
    }
}
