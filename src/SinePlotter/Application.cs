// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Ara3D.RevitSampleBrowser.SinePlotter.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static double _periodVal = 0.2;
        private static double _cyclesVal = 10;
        private static double _amplitudeVal = 3;
        private static double _partitionsVal = 3;
        private static ComboBox _prismComboBox;
        private TextBox m_amplitudeBox;
        private string m_assemblyName;
        private string m_assemblyPath;
        private TextBox m_cyclesBox;
        private string m_imageFolder;
        private TextBox m_partitionsBox;

        private TextBox m_periodBox;

        /// <summary>
        ///     Implements the OnShutdown event
        /// </summary>
        /// <param name="application"></param>
        /// <returns>Indicates if the application completes its work successfully.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Implements the OnStartup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns>Indicates if the application completes its work successfully.</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_assemblyName = $"{Assembly.GetExecutingAssembly().GetName().Name}.dll";
            m_imageFolder = $"{GetProjectDirectory()}/Resources/";

            var panel = application.CreateRibbonPanel("ArrayPrismsOnASineCurve");
            AddCurvePropertiesTextFields(panel);
            AddPartitionsTextField(panel);
            panel.AddSeparator();
            AddPrismComboBox(panel);
            panel.AddSeparator();
            AddRunButton(panel);

            return Result.Succeeded;
        }

        public static string GetFamilySymbolName()
        {
            return _prismComboBox.Current.Name;
        }

        public static double GetPeriod()
        {
            return _periodVal;
        }

        public static double GetNumberOfCycles()
        {
            return _cyclesVal;
        }

        public static double GetAplitude()
        {
            return _amplitudeVal;
        }

        public static double GetNumberOfPartitions()
        {
            return _partitionsVal;
        }

        private void AddPrismComboBox(RibbonPanel panel)
        {
            //Family instance #1
            ComboBoxMemberData comboBoxMemberData1 = new("cylinder", "cylinder prism");
            //Family instance #2
            ComboBoxMemberData comboBoxMemberData2 = new("rectangle", "rectangular prism");
            //Family instance #3 
            ComboBoxMemberData comboBoxMemberData3 = new("regularpolygon", "regular polygon prism");
            //Family instance #4
            ComboBoxMemberData comboBoxMemberData4 = new("isotriangle", "isotriangle prism");

            //make a combo box group group 
            ComboBoxData comboBxData = new("ComboBox");
            _prismComboBox = panel.AddItem(comboBxData) as ComboBox;
            _prismComboBox.ToolTip = "select a prism to array on a curve";
            _prismComboBox.AddItem(comboBoxMemberData1);
            _prismComboBox.AddItem(comboBoxMemberData2);
            _prismComboBox.AddItem(comboBoxMemberData3);
            _prismComboBox.AddItem(comboBoxMemberData4);
        }

        private void AddCurvePropertiesTextFields(RibbonPanel panel)
        {
            //Inactive textfields that just display information about the active input fields
            TextBoxData periodLabelData = new("curve period field");
            TextBoxData cyclesLabelData = new("curve cycles field");
            TextBoxData amplitudeLabelData = new("curve amplitude field");
            //Stack them horizontally
            var textFieldList = panel.AddStackedItems(periodLabelData, cyclesLabelData, amplitudeLabelData);
            //Call method to customize the text boxes and make them inactive
            var periodToolTip =
                "A double value denoting the period of the curve, i.e. how often the curve goes a full repition around the unit circle.";
            CustomizeTextBox(panel, (TextBox)textFieldList[0], periodToolTip, "curve period:", 0, false, 130);
            var cyclesToolTip = "A double value denoting the number of circles the curve makes.";
            CustomizeTextBox(panel, (TextBox)textFieldList[1], cyclesToolTip, "curve cycles:", 0, false, 130);
            var amplitudeToolTip = "A double value denoting how far the curve gets away from the x-axis.";
            CustomizeTextBox(panel, (TextBox)textFieldList[2], amplitudeToolTip, "curve amplitude:", 0, false, 130);

            //Active text fields for user input
            TextBoxData periodBoxData = new("curve period");
            TextBoxData cyclesBoxData = new("curve cycles");
            TextBoxData amplitudeBoxData = new("curve amplitude");
            //Stack them horizontally
            var textBoxList = panel.AddStackedItems(periodBoxData, cyclesBoxData, amplitudeBoxData);

            m_periodBox = (TextBox)textBoxList[0];
            m_cyclesBox = (TextBox)textBoxList[1];
            m_amplitudeBox = (TextBox)textBoxList[2];

            //Call method to customize the text boxes and make them active
            CustomizeTextBox(panel, m_periodBox,
                "Define the period of the sine curve", $"{_periodVal}", _periodVal, true, 50);
            CustomizeTextBox(panel, m_cyclesBox,
                "Define the number of cycles of the sine curve", $"{_cyclesVal}", _cyclesVal, true, 50);
            CustomizeTextBox(panel, m_amplitudeBox,
                "Define the amplitude of the sine curve", $"{_amplitudeVal}", _amplitudeVal, true, 50);
        }

        private void AddPartitionsTextField(RibbonPanel panel)
        {
            TextBoxData partitionsFieldData = new("curve partitions field");
            var partitionsFieldBox = panel.AddItem(partitionsFieldData) as TextBox;
            //Call method to customize the text box and set it as inactive
            CustomizeTextBox(panel, partitionsFieldBox, null, "number of partitions:", 0, false, 130);
            //Call method to customize the text box and set it as active
            TextBoxData partitionsBoxData = new("curve partitions box");
            m_partitionsBox = panel.AddItem(partitionsBoxData) as TextBox;
            CustomizeTextBox(panel, m_partitionsBox, "Define the number of partitions", $"{_partitionsVal}", _partitionsVal,
                true, 50);
        }

        private void AddRunButton(RibbonPanel panel)
        {
            PushButtonData pushButtonData = new("arrayPrisms", "run",
                $"{m_assemblyPath}\\{m_assemblyName}", "Ara3D.RevitSampleBrowser.SinePlotter.CS.Command")
            {
                LargeImage = new BitmapImage(new Uri($"{m_imageFolder}Start.png"))
            };
            _ = panel.AddItem(pushButtonData) as PushButton;
        }

        private void CustomizeTextBox(RibbonPanel panel, TextBox txtBox, string tip, string displayedText,
            double defaultVal, bool isEnabled, int width)
        {
            txtBox.Value = displayedText;
            txtBox.ToolTip = tip;
            txtBox.Width = width;
            txtBox.Enabled = isEnabled;

            if (isEnabled) txtBox.EnterPressed += TextBoxEnterPressed;
        }

        private void TextBoxEnterPressed(object sender, TextBoxEnterPressedEventArgs e)
        {
            //cast sender as TextBox to retrieve text value
            var textBox = sender as TextBox;
            double value;
            if (double.TryParse(textBox.Value.ToString(), out value))
            {
                if (textBox.Name.Equals(m_periodBox.Name))
                {
                    if (value is < 0.1 or > 3)
                        TaskDialog.Show("TextBox Input",
                            $"The input value for {textBox.Name} has to be between 0.1 and 3.0");
                    else _periodVal = value;
                }
                else if (textBox.Name.Equals(m_cyclesBox.Name))
                {
                    if (value <= 0)
                        TaskDialog.Show("TextBox Input",
                            $"The input value for {textBox.Name} has to be greater than zero.");
                    else _cyclesVal = value;
                }
                else if (textBox.Name.Equals(m_amplitudeBox.Name))
                {
                    if (value is < -4 or > 4)
                        TaskDialog.Show("TextBox Input",
                            $"The input value for {textBox.Name} has to be between -4.0 and 4.0");
                    else _amplitudeVal = value;
                }
                else
                {
                    if (value <= 0)
                        TaskDialog.Show("TextBox Input",
                            $"The input value for {textBox.Name} has to be greater than zero.");
                    else _partitionsVal = value;
                }
            }
            else
            {
                TaskDialog.Show("TextBox Input", $"The input value for {textBox.Name} has to be a double.");
            }
        }

        private string GetProjectDirectory()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //move two directory levels back
            var grandParentDir = Directory.GetParent(assemblyPath).Parent.FullName;
            return grandParentDir;
        }
    }
}
