// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceAnalysis
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UIApplication m_application;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="application"></param>
        public MainWindow(UIApplication application)
        {
            InitializeComponent();
            m_application = application;
            if (m_application.ActiveUIDocument == null)
            {
                TaskDialog.Show("RP Analysis", "Null Document");
                Close();
            }

            PopulatePipeTypeList();
            PopulateSizeList();
        }

        /// <summary>
        ///     Get the selected pipe size (in inches) from the dialog
        /// </summary>
        private double GetSelectedSize()
        {
            return double.Parse(comboBox_Sizes.SelectedItem.ToString());
        }

        /// <summary>
        ///     Display all sizes available in given pipe type in the dialog.
        /// </summary>
        private void PopulateSizeList()
        {
            comboBox_Sizes.Items.Clear();
            var sizes = Analyzer.GetAvailableSegmentSizes(GetSelectedPipeType().RoutingPreferenceManager,
                m_application.ActiveUIDocument.Document);
            foreach (var size in sizes)
            {
                var unitAdjustedSize = Convert.ConvertValueDocumentUnits(size, m_application.ActiveUIDocument.Document);
                comboBox_Sizes.Items.Add(unitAdjustedSize.ToString());
            }

            comboBox_Sizes.SelectedIndex = 0;
        }

        /// <summary>
        ///     Get all the PipeTypes in the document and display them in the window's combo box.
        /// </summary>
        private void PopulatePipeTypeList()
        {
            var collector = new FilteredElementCollector(m_application.ActiveUIDocument.Document);
            collector.OfClass(typeof(PipeType));
            foreach (var pipeType in collector.ToElements().Cast<PipeType>())
            {
                comboBox_PipeTypes.Items.Add($"{pipeType.Name}, Id: {pipeType.Id}");
            }

            comboBox_PipeTypes.SelectedIndex = 0;
        }

        /// <summary>
        ///     Look up a pipe type by ElementId string and return it.
        /// </summary>
        /// <returns></returns>
        private PipeType GetSelectedPipeType()
        {
            var selectedString = comboBox_PipeTypes.SelectedItem.ToString();
            var selectedsplit = selectedString.Split(':');
            if (selectedsplit.Length != 2)
                throw new ArgumentException();

            var id = ElementId.Parse(selectedsplit[1]);
            return m_application.ActiveUIDocument.Document.GetElement(id) as PipeType;
        }

        /// <summary>
        ///     Get the selected PipeType from the combo box in the window, get its RoutingPreferenceManager, and analyze it for
        ///     problems.
        ///     Write all potential problems to the window's text field as formatted XML.
        /// </summary>
        private void button_CheckWarnings_Click(object sender, RoutedEventArgs e)
        {
            var routingPrefernceAnalyzer = new Analyzer(GetSelectedPipeType().RoutingPreferenceManager,
                m_application.ActiveUIDocument.Document);
            var results = routingPrefernceAnalyzer.GetWarnings();

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = false
            };
            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, xmlWriterSettings);
            results.WriteTo(writer);
            writer.Flush();
            writer.Close();
            textBox_output.Text = output.ToString();
        }

        /// <summary>
        ///     Button CheckSpecificSize click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_CheckSpecificSize_Click(object sender, RoutedEventArgs e)
        {
            var routingPrefernceAnalyzer = new Analyzer(GetSelectedPipeType().RoutingPreferenceManager,
                GetSelectedSize(), m_application.ActiveUIDocument.Document);
            var results = routingPrefernceAnalyzer.GetSpecificSizeQuery();

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = false
            };
            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, xmlWriterSettings);
            results.WriteTo(writer);
            writer.Flush();
            writer.Close();
            textBox_output.Text = output.ToString();
        }

        /// <summary>
        ///     ComboBox PipeTypes selection changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_PipeTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSizeList();
        }
    }
}
