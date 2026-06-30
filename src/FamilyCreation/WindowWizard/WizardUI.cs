// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public partial class WizardForm : Form
    {
        private readonly BindingSource m_bindSource = [];

        private readonly ToolTip m_copyTip = new();

        private readonly ToolTip m_errorTip = new();

        private readonly Font m_highFont;

        private readonly Font m_commonFont;

        private readonly ToolTip m_newTip = new();

        private readonly WizardParameter m_para;

        private readonly List<string> m_types = [];

        private readonly BindingList<DoubleHungWinPara> m_paraList = [];

        public WizardForm(WizardParameter para)
        {
            m_para = para;
            InitializeComponent();
            InitializePara();
            m_newTip.SetToolTip(button_newType, "Add new type");
            m_copyTip.SetToolTip(button_duplicateType, "Duplicate the type");
            m_errorTip.ShowAlways = false;
            m_highFont = InputDimensionLabel.Font;
            m_commonFont = WindowPropertyLabel.Font;
            SetPanelVisibility(2);
        }

        private void Step1_NextButton_Click(object sender, EventArgs e)
        {
            if (panel1.Visible)
            {
                InitializePara();
                SetPanelVisibility(2);
            }
            else if (panel2.Visible)
            {
                TransforData();
                SetPanelVisibility(3);
            }
            else if (panel3.Visible)
            {
                SetPanelVisibility(4);
                SetGridData();
            }
            else if (panel4.Visible)
            {
                m_para.PathName = m_pathName.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetPanelVisibility(int panelNum)
        {
            switch (panelNum)
            {
                case 1:
                    panel1.Visible = true;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel4.Visible = false;
                    Step1_BackButton.Enabled = false;
                    break;
                case 2:
                    panel1.Visible = false;
                    panel2.Visible = true;
                    panel3.Visible = false;
                    panel4.Visible = false;
                    Step1_BackButton.Enabled = false;
                    InputDimensionLabel.ForeColor = Color.Black;
                    InputDimensionLabel.Font = m_highFont;
                    WindowPropertyLabel.ForeColor = Color.Gray;
                    WindowPropertyLabel.Font = m_commonFont;
                    InputPathLabel.ForeColor = Color.Gray;
                    InputPathLabel.Font = m_commonFont;
                    break;
                case 3:
                    panel3.Visible = true;
                    panel1.Visible = false;
                    panel2.Visible = false;
                    panel4.Visible = false;
                    Step1_BackButton.Enabled = true;
                    Step1_NextButton.Text = "Next >";
                    InputPathLabel.ForeColor = Color.Gray;
                    InputDimensionLabel.Font = m_commonFont;
                    WindowPropertyLabel.ForeColor = Color.Black;
                    WindowPropertyLabel.Font = m_highFont;
                    InputPathLabel.ForeColor = Color.Gray;
                    InputPathLabel.Font = m_commonFont;
                    break;
                case 4:
                    panel1.Visible = false;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel4.Visible = true;
                    Step1_BackButton.Enabled = true;
                    Step1_NextButton.Text = "Finish";
                    InputPathLabel.ForeColor = Color.Gray;
                    InputDimensionLabel.Font = m_commonFont;
                    WindowPropertyLabel.ForeColor = Color.Gray;
                    WindowPropertyLabel.Font = m_commonFont;
                    InputPathLabel.ForeColor = Color.Black;
                    InputPathLabel.Font = m_highFont;
                    break;
            }
        }

        private void Step1_BackButton_Click(object sender, EventArgs e)
        {
            if (panel2.Visible)
                SetPanelVisibility(1);
            else if (panel3.Visible)
                SetPanelVisibility(2);
            else if (panel4.Visible) SetPanelVisibility(3);
        }

        private void TransforData()
        {
            if (m_para.Template == "DoubleHung")
            {
                DoubleHungWinPara dbhungPara = new(m_para.Validator.IsMetric)
                {
                    Height = Convert.ToDouble(m_height.Text),
                    Width = Convert.ToDouble(m_width.Text),
                    Inset = Convert.ToDouble(m_inset.Text),
                    SillHeight = Convert.ToDouble(m_sillHeight.Text),
                    Type = m_comboType.Text
                };
                m_para.CurrentPara = dbhungPara;
                if (!m_para.WinParaTab.Contains(dbhungPara.Type))
                {
                    m_para.WinParaTab.Add(dbhungPara.Type, dbhungPara);
                    m_comboType.Items.Add(dbhungPara.Type);
                }
                else
                {
                    m_para.WinParaTab[dbhungPara.Type] = dbhungPara;
                }
            }

            Update();
        }

        private void InitializePara()
        {
            DoubleHungWinPara dbhungPara = new(m_para.Validator.IsMetric);
            if (!m_para.WinParaTab.Contains(dbhungPara.Type))
            {
                m_para.WinParaTab.Add(dbhungPara.Type, dbhungPara);
                m_types.Add(dbhungPara.Type);
            }
            else
            {
                m_para.WinParaTab[dbhungPara.Type] = dbhungPara;
            }

            m_bindSource.DataSource = m_types;
            m_comboType.Items.Add(m_para.CurrentPara.Type);
            m_comboType.SelectedIndex = 0;
            m_glassMat.DataSource = m_para.GlassMaterials;
            m_sashMat.DataSource = m_para.FrameMaterials;
            m_pathName.Text = m_para.PathName;
            SetParaText(dbhungPara);
        }

        private void button_newType_Click(object sender, EventArgs e)
        {
            TransforData();
            DoubleHungWinPara newPara = new(m_para.Validator.IsMetric);
            SetParaText(newPara);
            m_comboType.Focus();
        }

        private void button_duplicateType_Click(object sender, EventArgs e)
        {
            TransforData();
            DoubleHungWinPara copyPara = new((DoubleHungWinPara)m_para.CurrentPara);
            SetParaText(copyPara);
            m_comboType.Focus();
        }

        private void SetParaText(WindowParameter para)
        {
            var dbhungPara = para as DoubleHungWinPara;
            m_sillHeight.Text = dbhungPara.SillHeight.ToString();
            m_width.Text = dbhungPara.Width.ToString();
            m_height.Text = dbhungPara.Height.ToString();
            m_inset.Text = dbhungPara.Inset.ToString();
            m_comboType.Text = dbhungPara.Type;
        }

        private void SetGridData()
        {
            m_paraList.Clear();
            foreach (string key in m_para.WinParaTab.Keys)
            {
                if (m_para.WinParaTab[key] is not DoubleHungWinPara para) continue;
                m_paraList.Add(para);
            }

            dataGridView1.DataSource = m_paraList;
        }

        private void m_height_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(m_height);
        }

        private void m_width_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(m_width);
        }

        private void m_inset_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(m_inset);
        }

        private void m_sillHeight_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(m_sillHeight);
        }

        private void m_comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_para.CurrentPara = m_para.WinParaTab[m_comboType.SelectedItem.ToString()] as WindowParameter;
            SetParaText(m_para.CurrentPara);
        }

        private void m_glassMat_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_para.GlassMat = m_glassMat.SelectedItem.ToString();
        }

        private void m_sashMat_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_para.SashMat = m_sashMat.SelectedItem.ToString();
        }

        private bool ValidateInput(Control control)
        {
            if (null == control) return true;
            if (string.IsNullOrEmpty(control.Text))
            {
                Step1_NextButton.Enabled = false;
                return false;
            }

            if (control is not TextBox textbox) return true;
            var value = 0.0;
            var result = m_para.Validator.IsDouble(textbox.Text, ref value);
            if (!string.IsNullOrEmpty(result))
            {
                m_errorTip.SetToolTip(textbox, result);
                textbox.Text = string.Empty;
                Step1_NextButton.Enabled = false;
                return false;
            }

            m_errorTip.RemoveAll();
            switch (textbox.Name)
            {
                case "m_height":
                    result = m_para.Validator.IsHeightInRange(value);
                    break;
                case "m_width":
                    result = m_para.Validator.IsWidthInRange(value);
                    break;
                case "m_inset":
                    result = m_para.Validator.IsInsetInRange(value);
                    break;
                case "m_sillHeight":
                    result = m_para.Validator.IsSillHeightInRange(value);
                    break;
            }

            if (!string.IsNullOrEmpty(result))
            {
                m_errorTip.SetToolTip(textbox, result);
                Step1_NextButton.Enabled = false;
                return false;
            }

            m_errorTip.RemoveAll();
            Step1_NextButton.Enabled = true;
            return true;
        }

        private void m_buttonBrowser_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                CheckPathExists = true,
                SupportMultiDottedExtensions = true,
                OverwritePrompt = true,
                ValidateNames = true,
                Filter = "Family file(*.rfa)|*.rfa|All files(*.*)|*.*",
                FilterIndex = 2
            };
            if (DialogResult.OK == saveDialog.ShowDialog())
                if (!string.IsNullOrEmpty(saveDialog.FileName))
                    m_pathName.Text = saveDialog.FileName;
        }

        private void m_height_Leave(object sender, EventArgs e)
        {
            CheckValue(m_height);
        }

        private void CheckValue(Control control)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                control.Focus();
                m_errorTip.SetToolTip(control, "Please input a valid value");
            }

            if (!ValidateInput(control))
            {
                control.Focus();
                control.Text = string.Empty;
            }
        }

        private void m_width_Leave(object sender, EventArgs e)
        {
            CheckValue(m_width);
        }

        private void m_inset_Leave(object sender, EventArgs e)
        {
            CheckValue(m_inset);
        }

        private void m_sillHeight_Leave(object sender, EventArgs e)
        {
            CheckValue(m_sillHeight);
        }

        private void m_comboType_Leave(object sender, EventArgs e)
        {
            CheckValue(m_comboType);
        }

        private void Step1_HelpButton_Click(object sender, EventArgs e)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var sp = Path.DirectorySeparatorChar; //{'\\'};
            path = path.Substring(0, path.LastIndexOf(sp));
            path = $"{path.Substring(0, path.LastIndexOf(sp))}{sp}ReadMe_WindowWizard.rtf";
            Process.Start(path);
        }
    }
}
