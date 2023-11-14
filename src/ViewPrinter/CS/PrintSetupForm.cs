// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Control = System.Windows.Forms.Control;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    public partial class PrintSetupForm : Form
    {
        private readonly double INCHES_IN_FEET = 12.0;
        private readonly PrintSTP m_printSetup;
        private bool m_stopUpdateFlag;

        public PrintSetupForm(PrintSTP printSetup)
        {
            m_printSetup = printSetup;
            InitializeComponent();
        }

        private void PrintSetupForm_Load(object sender, EventArgs e)
        {
            printerNameLabel.Text = m_printSetup.PrinterName;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
            printSetupsComboBox.SelectedValueChanged += printSetupsComboBox_SelectedValueChanged;
            renameButton.Enabled = deleteButton.Enabled =
                m_printSetup.SettingName.Equals("<In-Session>") ? false : true;

            paperSizeComboBox.DataSource = m_printSetup.PaperSizes;
            paperSizeComboBox.SelectedItem = m_printSetup.PaperSize;
            paperSizeComboBox.SelectedValueChanged += sizeComboBox_SelectedValueChanged;

            paperSourceComboBox.DataSource = m_printSetup.PaperSources;
            paperSourceComboBox.SelectedItem = m_printSetup.PaperSource;
            paperSourceComboBox.SelectedValueChanged += sourceComboBox_SelectedValueChanged;

            if (m_printSetup.PageOrientation == PageOrientationType.Landscape)
                landscapeRadioButton.Checked = true;
            else
                portraitRadioButton.Checked = true;
            landscapeRadioButton.CheckedChanged += landscapeRadioButton_CheckedChanged;
            portraitRadioButton.CheckedChanged += portraitRadioButton_CheckedChanged;

            marginTypeComboBox.DataSource = m_printSetup.MarginTypes;
            offsetRadioButton.CheckedChanged += offsetRadioButton_CheckedChanged;
            centerRadioButton.CheckedChanged += centerRadioButton_CheckedChanged;
            userDefinedMarginYTextBox.TextChanged += userDefinedMarginYTextBox_TextChanged;
            userDefinedMarginXTextBox.TextChanged += userDefinedMarginXTextBox_TextChanged;

            marginTypeComboBox.SelectedItem = m_printSetup.SelectedMarginType;
            marginTypeComboBox.SelectedValueChanged += marginTypeComboBox_SelectedValueChanged;

            if (m_printSetup.PaperPlacement == PaperPlacementType.Center)
            {
                centerRadioButton.Checked = true;
                offsetRadioButton.Checked = false;
            }
            else
            {
                offsetRadioButton.Checked = true;
                centerRadioButton.Checked = false;
            }

            if (m_printSetup.HiddenLineViews == HiddenLineViewsType.RasterProcessing)
                rasterRadioButton.Checked = true;
            else
                vectorRadioButton.Checked = true;
            rasterRadioButton.CheckedChanged += rasterRadioButton_CheckedChanged;
            vectorRadioButton.CheckedChanged += vectorRadioButton_CheckedChanged;

            if (m_printSetup.ZoomType == ZoomType.Zoom)
            {
                zoomRadioButton.Checked = true;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
            }
            else
            {
                fitToPageRadioButton.Checked = true;
            }

            zoomRadioButton.CheckedChanged += zoomRadioButton_CheckedChanged;
            fitToPageRadioButton.CheckedChanged += fitToPageRadioButton_CheckedChanged;

            rasterQualityComboBox.DataSource = m_printSetup.RasterQualities;
            rasterQualityComboBox.SelectedItem = m_printSetup.RasterQuality;
            rasterQualityComboBox.SelectedValueChanged += rasterQualityComboBox_SelectedValueChanged;

            colorsComboBox.DataSource = m_printSetup.Colors;
            colorsComboBox.SelectedItem = m_printSetup.Color;
            colorsComboBox.SelectedValueChanged += colorsComboBox_SelectedValueChanged;

            ViewLinksInBlueCheckBox.Checked = m_printSetup.ViewLinksinBlue;
            ViewLinksInBlueCheckBox.CheckedChanged += ViewLinksInBlueCheckBox_CheckedChanged;

            hideScopeBoxedCheckBox.Checked = m_printSetup.HideScopeBoxes;
            hideScopeBoxedCheckBox.CheckedChanged += hideScopeBoxedCheckBox_CheckedChanged;

            hideRefWorkPlanesCheckBox.Checked = m_printSetup.HideReforWorkPlanes;
            hideRefWorkPlanesCheckBox.CheckedChanged += hideRefWorkPlanesCheckBox_CheckedChanged;

            hideCropBoundariesCheckBox.Checked = m_printSetup.HideCropBoundaries;
            hideCropBoundariesCheckBox.CheckedChanged += hideCropBoundariesCheckBox_CheckedChanged;

            hideUnreferencedViewTagsCheckBox.Checked = m_printSetup.HideUnreferencedViewTags;
            hideUnreferencedViewTagsCheckBox.CheckedChanged += hideUnreferencedViewTagsCheckBox_CheckedChanged;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            m_printSetup.Save();
        }

        private void printSetupsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            m_printSetup.SettingName = printSetupsComboBox.SelectedItem as string;

            paperSizeComboBox.SelectedItem = m_printSetup.PaperSize;
            paperSourceComboBox.SelectedItem = m_printSetup.PaperSource;
            if (m_printSetup.PageOrientation == PageOrientationType.Landscape)
                landscapeRadioButton.Checked = true;
            else
                portraitRadioButton.Checked = true;
            if (m_printSetup.PaperPlacement == PaperPlacementType.Center)
                centerRadioButton.Checked = true;
            else
                offsetRadioButton.Checked = true;
            if (m_printSetup.VerifyMarginType(marginTypeComboBox))
                marginTypeComboBox.SelectedItem = m_printSetup.SelectedMarginType;

            if (m_printSetup.HiddenLineViews == HiddenLineViewsType.RasterProcessing)
                rasterRadioButton.Checked = true;
            else
                vectorRadioButton.Checked = true;
            if (m_printSetup.ZoomType == ZoomType.Zoom)
            {
                zoomRadioButton.Checked = true;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
            }
            else
            {
                fitToPageRadioButton.Checked = true;
                m_printSetup.ZoomType = ZoomType.Zoom;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
                m_printSetup.ZoomType = ZoomType.FitToPage;
            }

            rasterQualityComboBox.SelectedItem = m_printSetup.RasterQuality;
            colorsComboBox.SelectedItem = m_printSetup.Color;
            ViewLinksInBlueCheckBox.Checked = m_printSetup.ViewLinksinBlue;
            hideScopeBoxedCheckBox.Checked = m_printSetup.HideScopeBoxes;
            hideRefWorkPlanesCheckBox.Checked = m_printSetup.HideReforWorkPlanes;
            hideCropBoundariesCheckBox.Checked = m_printSetup.HideCropBoundaries;
            hideUnreferencedViewTagsCheckBox.Checked = m_printSetup.HideUnreferencedViewTags;

            renameButton.Enabled = deleteButton.Enabled =
                m_printSetup.SettingName.Equals("<In-Session>") ? false : true;

            revertButton.Enabled = false;
        }

        private void sizeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.PaperSize = paperSizeComboBox.SelectedItem as string;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void sourceComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.PaperSource = paperSourceComboBox.SelectedItem as string;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void portraitRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (portraitRadioButton.Checked) m_printSetup.PageOrientation = PageOrientationType.Portrait;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void landscapeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (landscapeRadioButton.Checked) m_printSetup.PageOrientation = PageOrientationType.Landscape;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void centerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!centerRadioButton.Checked)
                return;

            m_printSetup.PaperPlacement = PaperPlacementType.Center;

            m_printSetup.VerifyMarginType(marginTypeComboBox);

            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    userDefinedMarginXTextBox,
                    userDefinedMarginYTextBox
                };
            if (m_printSetup.VerifyUserDefinedMargin(controlsToEnableOrNot))
            {
                userDefinedMarginXTextBox.Text = (m_printSetup.OriginOffsetX * INCHES_IN_FEET).ToString();
                userDefinedMarginYTextBox.Text = (m_printSetup.OriginOffsetY * INCHES_IN_FEET).ToString();

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void offsetRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!offsetRadioButton.Checked)
                return;

            m_printSetup.PaperPlacement = PaperPlacementType.LowerLeft;

            m_printSetup.VerifyMarginType(marginTypeComboBox);

            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    userDefinedMarginXTextBox,
                    userDefinedMarginYTextBox
                };
            if (m_printSetup.VerifyUserDefinedMargin(controlsToEnableOrNot))
            {
                userDefinedMarginXTextBox.Text = (m_printSetup.OriginOffsetX * INCHES_IN_FEET).ToString();
                userDefinedMarginYTextBox.Text = (m_printSetup.OriginOffsetY * INCHES_IN_FEET).ToString();

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void marginTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.SelectedMarginType = (MarginType)marginTypeComboBox.SelectedItem;

            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    userDefinedMarginXTextBox,
                    userDefinedMarginYTextBox
                };
            if (m_printSetup.VerifyUserDefinedMargin(controlsToEnableOrNot))
            {
                userDefinedMarginXTextBox.Text = (m_printSetup.OriginOffsetX * INCHES_IN_FEET).ToString();
                userDefinedMarginYTextBox.Text = (m_printSetup.OriginOffsetY * INCHES_IN_FEET).ToString();
            }

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void vectorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (vectorRadioButton.Checked)
            {
                m_printSetup.HiddenLineViews = HiddenLineViewsType.VectorProcessing;

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void rasterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rasterRadioButton.Checked)
            {
                m_printSetup.HiddenLineViews = HiddenLineViewsType.RasterProcessing;

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void fitToPageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fitToPageRadioButton.Checked)
            {
                m_printSetup.ZoomType = ZoomType.FitToPage;
                centerRadioButton.Checked = true;

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void zoomRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (zoomRadioButton.Checked)
            {
                m_printSetup.ZoomType = ZoomType.Zoom;
                offsetRadioButton.Checked = true;
                m_printSetup.Zoom = (int)zoomPercentNumericUpDown.Value;

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void zoomPercentNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (zoomRadioButton.Checked)
            {
                m_printSetup.Zoom = (int)zoomPercentNumericUpDown.Value;

                if (!revertButton.Enabled) revertButton.Enabled = true;
            }
        }

        private void rasterQualityComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.RasterQuality = (RasterQualityType)rasterQualityComboBox.SelectedItem;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void colorsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.Color = (ColorDepthType)colorsComboBox.SelectedItem;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void ViewLinksInBlueCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.ViewLinksinBlue = ViewLinksInBlueCheckBox.Checked;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void hideScopeBoxedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideScopeBoxes = hideScopeBoxedCheckBox.Checked;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void hideRefWorkPlanesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideReforWorkPlanes = hideRefWorkPlanesCheckBox.Checked;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void hideCropBoundariesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideCropBoundaries = hideCropBoundariesCheckBox.Checked;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void hideUnreferencedViewTagsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideUnreferencedViewTags = hideUnreferencedViewTagsCheckBox.Checked;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void userDefinedMarginXTextBox_TextChanged(object sender, EventArgs e)
        {
            double doubleValue;
            if (!double.TryParse(userDefinedMarginXTextBox.Text, out doubleValue))
            {
                PrintMgr.MyMessageBox("Invalid input");
                return;
            }

            m_printSetup.OriginOffsetX = doubleValue / INCHES_IN_FEET;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void userDefinedMarginYTextBox_TextChanged(object sender, EventArgs e)
        {
            double doubleValue;
            if (!double.TryParse(userDefinedMarginYTextBox.Text, out doubleValue))
            {
                PrintMgr.MyMessageBox("Invalid input");
                return;
            }

            m_printSetup.OriginOffsetY = doubleValue / INCHES_IN_FEET;

            if (!revertButton.Enabled) revertButton.Enabled = true;
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveAsForm(m_printSetup))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.Update();
            m_stopUpdateFlag = false;

            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new ReNameForm(m_printSetup))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.Update();
            m_stopUpdateFlag = false;

            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            m_printSetup.Revert();

            printSetupsComboBox_SelectedValueChanged(null, null);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            m_printSetup.Delete();

            m_stopUpdateFlag = true;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.Update();
            m_stopUpdateFlag = false;

            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
        }
    }
}
