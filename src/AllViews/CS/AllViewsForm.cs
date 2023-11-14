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
        public bool InvalidViewport = true;

        public XYZ GetBoxCenter;
        public double GetLabelLineLength;

        public XYZ GetLabelLineOffset;
        public XYZ GetMaxBoxOutline;
        public XYZ GetMaxLabelOutline;

        public XYZ GetMinBoxOutline;

        public XYZ GetMinLabelOutline;
        public ViewportRotation GetOrientation;

        public ViewportRotation SetRotation = ViewportRotation.None;

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
            if (InvalidViewport)
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
                getMinBoxOutlineTextBox.Text = "(" + GetMinBoxOutline.X + ", " + GetMinBoxOutline.Y + ")";
                getMaxBoxOutlineTextBox.Text = "(" + GetMaxBoxOutline.X + ", " + GetMaxBoxOutline.Y + ")";

                //LabelOutline
                getMinLabelOutlineTextBox.Text = "(" + GetMinLabelOutline.X + ", " + GetMinLabelOutline.Y + ")";
                getMaxLabelOutlineTextBox.Text = "(" + GetMaxLabelOutline.X + ", " + GetMaxLabelOutline.Y + ")";

                //LabelLineOffset
                getLabelLineOffsetTextBox.Text = "(" + GetLabelLineOffset.X + ", " + GetLabelLineOffset.Y + ")";

                //LabelLineLength
                getLabelLineLengthTextBox.Text = GetLabelLineLength.ToString();

                //Others
                getBoxCenterTextBox.Text = "(" + GetBoxCenter.X + ", " + GetBoxCenter.Y + ")";
                getOrientationTtextBox.Text = GetOrientation.ToString();
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
            m_data.SetRotation(this, SetRotation);
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
            InvalidViewport = true;
            UpdateControls();
        }

        private void selectSheetNameTextBox_TextChanged(object sender, EventArgs e)
        {
            InvalidViewport = true;
            UpdateControls();
        }

        private void noneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetRotation = ViewportRotation.None;
        }

        private void clockWiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetRotation = ViewportRotation.Clockwise;
        }

        private void counterClockWiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetRotation = ViewportRotation.Counterclockwise;
        }
    }
}
