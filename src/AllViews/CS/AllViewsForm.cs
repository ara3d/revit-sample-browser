// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.AllViews.CS
{
    /// <summary>
    ///     This is a dialog should appear that contains the following:
    ///     A tree view represents all the views' names.
    ///     A list of all title blocks.
    ///     An edit box for the sheet's name.
    /// </summary>
    public partial class AllViewsForm : Form
    {
        public bool invalidViewport = true;

        public XYZ m_getBoxCenter;
        public double m_getLabelLineLength;

        public XYZ m_getLabelLineOffset;
        public XYZ m_getMaxBoxOutline;
        public XYZ m_getMaxLabelOutline;

        public XYZ m_getMinBoxOutline;

        public XYZ m_getMinLabelOutline;
        public ViewportRotation m_getOrientation;

        public ViewportRotation m_setRotation = ViewportRotation.None;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="data"></param>
        public AllViewsForm(ViewsMgr data)
        {
            m_data = data;
            InitializeComponent();
        }

        public void UpdateControls()
        {
            if (invalidViewport)
            {
                setRotationButton.Enabled = false;
                setLabelOffsetButton.Enabled = false;
                setLabelLengthButton.Enabled = false;
                noneRadioButton.Enabled = false;
                clockWiseRadioButton.Enabled = false;
                counterClockWiseRadioButton.Enabled = false;
                setLabelOffsetXTextBox.Enabled = false;
                setLabelOffsetYTextBox.Enabled = false;
                setLabelLineLengthTextBox.Enabled = false;

                //BoxOutline
                getMinBoxOutlineTextBox.Text = "";
                getMaxBoxOutlineTextBox.Text = "";

                //LabelOutline
                getMinLabelOutlineTextBox.Text = "";
                getMaxLabelOutlineTextBox.Text = "";

                //LabelLineOffset
                getLabelLineOffsetTextBox.Text = "";

                //LabelLineLength
                getLabelLineLengthTextBox.Text = "";

                //Others
                getBoxCenterTextBox.Text = "";
                getOrientationTtextBox.Text = "";
            }
            else
            {
                setRotationButton.Enabled = true;
                setLabelOffsetButton.Enabled = true;
                setLabelLengthButton.Enabled = true;
                noneRadioButton.Enabled = true;
                clockWiseRadioButton.Enabled = true;
                counterClockWiseRadioButton.Enabled = true;
                setLabelOffsetXTextBox.Enabled = true;
                setLabelOffsetYTextBox.Enabled = true;
                setLabelLineLengthTextBox.Enabled = true;

                //BoxOutline
                getMinBoxOutlineTextBox.Text = "(" + m_getMinBoxOutline.X + ", " + m_getMinBoxOutline.Y + ")";
                getMaxBoxOutlineTextBox.Text = "(" + m_getMaxBoxOutline.X + ", " + m_getMaxBoxOutline.Y + ")";

                //LabelOutline
                getMinLabelOutlineTextBox.Text = "(" + m_getMinLabelOutline.X + ", " + m_getMinLabelOutline.Y + ")";
                getMaxLabelOutlineTextBox.Text = "(" + m_getMaxLabelOutline.X + ", " + m_getMaxLabelOutline.Y + ")";

                //LabelLineOffset
                getLabelLineOffsetTextBox.Text = "(" + m_getLabelLineOffset.X + ", " + m_getLabelLineOffset.Y + ")";

                //LabelLineLength
                getLabelLineLengthTextBox.Text = m_getLabelLineLength.ToString();

                //Others
                getBoxCenterTextBox.Text = "(" + m_getBoxCenter.X + ", " + m_getBoxCenter.Y + ")";
                getOrientationTtextBox.Text = m_getOrientation.ToString();
            }
        }

        private void AllViewsForm_Load(object sender, EventArgs e)
        {
            allViewsTreeView.Nodes.Add(m_data.AllViewsNames);
            allViewsTreeView.TopNode.Expand();

            foreach (string s in m_data.AllTitleBlocksNames) titleBlocksListBox.Items.Add(s);
        }

        private void oKButton_Click(object sender, EventArgs e)
        {
            m_data.SelectViews();
            m_data.SheetName = sheetNameTextBox.Text;

            if (1 == titleBlocksListBox.SelectedItems.Count)
            {
                var titleBlock = titleBlocksListBox.SelectedItems[0].ToString();
                m_data.ChooseTitleBlock(titleBlock);
            }
        }

        private void CheckNode(TreeNode node, bool check)
        {
            if (0 < node.Nodes.Count)
            {
                if (node.Checked)
                    node.Expand();
                else
                    node.Collapse();

                foreach (TreeNode t in node.Nodes)
                {
                    t.Checked = check;
                    CheckNode(t, check);
                }
            }
        }

        private void allViewsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNode(e.Node, e.Node.Checked);
        }

        /// <summary>
        ///     Select title block to generate sheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleBlocksListBox_MouseClick(object sender, MouseEventArgs e)
        {
            var idx = titleBlocksListBox.SelectedIndex;
            if (0 < idx) titleBlocksListBox.SetSelected(idx, true);
        }

        private void selectViewportButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_data.SelectViewport(this, selectSheetNameTextBox.Text, selectAssociatedViewNameTextBox.Text);
                UpdateControls();
            }
            catch (Exception exception)
            {
                UpdateControls();
                MessageBox.Show("ERROR: " + exception.Message);
            }
        }

        private void setRotationButton_Click(object sender, EventArgs e)
        {
            m_data.SetRotation(this, m_setRotation);
            UpdateControls();
        }

        private void setLabelOffsetButton_Click(object sender, EventArgs e)
        {
            if (setLabelOffsetXTextBox.Text.Length != 0 && setLabelOffsetYTextBox.Text.Length != 0)
            {
                m_data.SetLabelOffset(this,
                    Convert.ToDouble(setLabelOffsetXTextBox.Text),
                    Convert.ToDouble(setLabelOffsetYTextBox.Text));
                UpdateControls();
            }
        }

        private void setLabelLengthButton_Click(object sender, EventArgs e)
        {
            if (setLabelLineLengthTextBox.Text.Length != 0)
            {
                m_data.SetLabelLength(this, Convert.ToDouble(setLabelLineLengthTextBox.Text));
                UpdateControls();
            }
        }

        private void selectViewportNameTextBox_TextChanged(object sender, EventArgs e)
        {
            invalidViewport = true;
            UpdateControls();
        }

        private void selectSheetNameTextBox_TextChanged(object sender, EventArgs e)
        {
            invalidViewport = true;
            UpdateControls();
        }

        private void noneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_setRotation = ViewportRotation.None;
        }

        private void clockWiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_setRotation = ViewportRotation.Clockwise;
        }

        private void counterClockWiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_setRotation = ViewportRotation.Counterclockwise;
        }
    }
}
