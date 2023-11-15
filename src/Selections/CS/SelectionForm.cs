// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Selections.CS
{
    /// <summary>
    ///     A Form to show selection from dialog.
    /// </summary>
    public partial class SelectionForm : Form
    {
        private readonly SelectionManager m_manager;

        /// <summary>
        ///     Form initialize.
        /// </summary>
        /// <param name="manager"></param>
        public SelectionForm(SelectionManager manager)
        {
            InitializeComponent();
            m_manager = manager;
        }

        /// <summary>
        ///     Set the selection type for picking element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickElementButton_Click(object sender, EventArgs e)
        {
            m_manager.SelectionType = SelectionType.Element;
            Close();
        }

        /// <summary>
        ///     Set the selection type for picking point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToButton_Click(object sender, EventArgs e)
        {
            m_manager.SelectionType = SelectionType.Point;
            Close();
        }
    }
}
