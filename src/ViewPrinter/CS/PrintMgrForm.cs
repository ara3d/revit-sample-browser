// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Control = System.Windows.Forms.Control;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.ViewPrinter.CS
{
    public partial class PrintMgrForm : Form
    {
        private readonly PrintMgr m_printMgr;

        public PrintMgrForm(PrintMgr printMgr)
        {
            if (null == printMgr)
                throw new ArgumentNullException(nameof(printMgr));
            m_printMgr = printMgr;

            InitializeComponent();
        }

        private void setupButton_Click(object sender, EventArgs e)
        {
            m_printMgr.ChangePrintSetup();
            printSetupNameLabel.Text = m_printMgr.PrintSetupName;
        }

        /// <summary>
        ///     Initialize the UI data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintMgrForm_Load(object sender, EventArgs e)
        {
            printerNameComboBox.DataSource = m_printMgr.InstalledPrinterNames;
            // the selectedValueChange event have to add event handler after
            // data source be set, or else the delegate method will be invoked meaningless.
            printerNameComboBox.SelectedValueChanged += printerNameComboBox_SelectedValueChanged;
            printerNameComboBox.SelectedItem = m_printMgr.PrinterName;
            if (m_printMgr.VerifyPrintToFile(printToFileCheckBox))
                printToFileCheckBox.Checked = m_printMgr.IsPrintToFile;

            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    copiesNumericUpDown,
                    numberofcoyiesLabel
                };
            m_printMgr.VerifyCopies(controlsToEnableOrNot);

            controlsToEnableOrNot.Clear();
            controlsToEnableOrNot.Add(printToFileNameLabel);
            controlsToEnableOrNot.Add(printToFileNameTextBox);
            controlsToEnableOrNot.Add(browseButton);
            m_printMgr.VerifyPrintToFileName(controlsToEnableOrNot);

            m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton);

            if (m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton))
            {
                singleFileRadioButton.Checked = m_printMgr.IsCombinedFile;
                separateFileRadioButton.Checked = !m_printMgr.IsCombinedFile;
            }

            if (!m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton)
                && m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton))
                separateFileRadioButton.Checked = true;
            singleFileRadioButton.CheckedChanged += combineRadioButton_CheckedChanged;

            switch (m_printMgr.PrintRange)
            {
                case PrintRange.Current:
                    currentWindowRadioButton.Checked = true;
                    break;
                case PrintRange.Select:
                    selectedViewsRadioButton.Checked = true;
                    break;
                case PrintRange.Visible:
                    visiblePortionRadioButton.Checked = true;
                    break;
            }

            currentWindowRadioButton.CheckedChanged += currentWindowRadioButton_CheckedChanged;
            visiblePortionRadioButton.CheckedChanged += visiblePortionRadioButton_CheckedChanged;
            selectedViewsRadioButton.CheckedChanged += selectedViewsRadioButton_CheckedChanged;

            printToFileNameTextBox.Text =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{m_printMgr.DocumentTitle}";
            controlsToEnableOrNot.Clear();
            controlsToEnableOrNot.Add(selectedViewSheetSetLabel);
            controlsToEnableOrNot.Add(selectedViewSheetSetButton);
            if (m_printMgr.VerifySelectViewSheetSet(controlsToEnableOrNot))
                selectedViewSheetSetLabel.Text = m_printMgr.SelectedViewSheetSetName;

            orderCheckBox.Checked = m_printMgr.PrintOrderReverse;
            orderCheckBox.CheckedChanged += orderCheckBox_CheckedChanged;

            if (m_printMgr.VerifyCollate(collateCheckBox)) collateCheckBox.Checked = m_printMgr.Collate;
            collateCheckBox.CheckedChanged += collateCheckBox_CheckedChanged;

            printSetupNameLabel.Text = m_printMgr.PrintSetupName;
        }

        private void printerNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printMgr.PrinterName = printerNameComboBox.SelectedItem as string;

            // Verify the relative controls is enable or not, according to the printer changed.
            m_printMgr.VerifyPrintToFile(printToFileCheckBox);

            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    copiesNumericUpDown,
                    numberofcoyiesLabel
                };
            m_printMgr.VerifyCopies(controlsToEnableOrNot);

            controlsToEnableOrNot.Clear();
            controlsToEnableOrNot.Add(printToFileNameLabel);
            controlsToEnableOrNot.Add(printToFileNameTextBox);
            controlsToEnableOrNot.Add(browseButton);

            if (!string.IsNullOrEmpty(printToFileNameTextBox.Text))
                printToFileNameTextBox.Text = printToFileNameTextBox.Text.Remove(
                    printToFileNameTextBox.Text.LastIndexOf(".")) + m_printMgr.PostFix;

            m_printMgr.VerifyPrintToFileName(controlsToEnableOrNot);

            m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton);
            m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton);
        }

        private void printToFileCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printMgr.IsPrintToFile = printToFileCheckBox.Checked;

            // Verify the relative controls is enable or not, according to the print to file
            // check box is checked or not.
            var controlsToEnableOrNot =
                new Collection<Control>
                {
                    copiesNumericUpDown,
                    numberofcoyiesLabel
                };
            m_printMgr.VerifyCopies(controlsToEnableOrNot);

            controlsToEnableOrNot.Clear();
            controlsToEnableOrNot.Add(printToFileNameLabel);
            controlsToEnableOrNot.Add(printToFileNameTextBox);
            controlsToEnableOrNot.Add(browseButton);
            m_printMgr.VerifyPrintToFileName(controlsToEnableOrNot);

            m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton);
        }

        private void combineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton))
                m_printMgr.IsCombinedFile = singleFileRadioButton.Checked;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var newName = m_printMgr.ChangePrintToFileName();
            if (!string.IsNullOrEmpty(newName)) printToFileNameTextBox.Text = newName;
        }

        private void currentWindowRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (currentWindowRadioButton.Checked)
            {
                m_printMgr.PrintRange = PrintRange.Current;

                var controlsToEnableOrNot =
                    new Collection<Control>
                    {
                        selectedViewSheetSetLabel,
                        selectedViewSheetSetButton
                    };
                m_printMgr.VerifySelectViewSheetSet(controlsToEnableOrNot);

                if (m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton))
                {
                    m_printMgr.IsCombinedFile = true;
                    singleFileRadioButton.Checked = true;
                    separateFileRadioButton.Checked = false;
                }

                m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton);
                m_printMgr.VerifyCollate(collateCheckBox);
            }
        }

        private void visiblePortionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (visiblePortionRadioButton.Checked)
            {
                m_printMgr.PrintRange = PrintRange.Visible;

                var controlsToEnableOrNot =
                    new Collection<Control>
                    {
                        selectedViewSheetSetLabel,
                        selectedViewSheetSetButton
                    };
                m_printMgr.VerifySelectViewSheetSet(controlsToEnableOrNot);

                if (m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton))
                {
                    m_printMgr.IsCombinedFile = true;
                    singleFileRadioButton.Checked = true;
                    separateFileRadioButton.Checked = false;
                }

                m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton);
                m_printMgr.VerifyCollate(collateCheckBox);
            }
        }

        private void selectedViewsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedViewsRadioButton.Checked)
            {
                m_printMgr.PrintRange = PrintRange.Select;

                var controlsToEnableOrNot =
                    new Collection<Control>
                    {
                        selectedViewSheetSetLabel,
                        selectedViewSheetSetButton
                    };
                m_printMgr.VerifySelectViewSheetSet(controlsToEnableOrNot);

                m_printMgr.VerifyPrintToSingleFile(singleFileRadioButton);
                if (m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton))
                    separateFileRadioButton.Checked = true;
                m_printMgr.VerifyPrintToSeparateFile(separateFileRadioButton);
                m_printMgr.VerifyCollate(collateCheckBox);
            }
        }

        private void orderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printMgr.PrintOrderReverse = orderCheckBox.Checked;
        }

        private void collateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_printMgr.Collate = collateCheckBox.Checked;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            m_printMgr.SelectViewSheetSet();
            selectedViewSheetSetLabel.Text = m_printMgr.SelectedViewSheetSetName;
        }

        private void copiesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                m_printMgr.CopyNumber = (int)copiesNumericUpDown.Value;
            }
            catch (InvalidOperationException)
            {
                collateCheckBox.Enabled = false;
                return;
            }

            m_printMgr.VerifyCollate(collateCheckBox);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_printMgr.SubmitPrint();
            }
            catch (Exception)
            {
                PrintMgr.MyMessageBox("Print Failed");
            }
        }
    }
}
