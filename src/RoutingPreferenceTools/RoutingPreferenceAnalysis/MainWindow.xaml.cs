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

using Ara3D.RevitSampleBrowser.Common.Units;
namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceAnalysis
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UIApplication m_application;

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

        private double GetSelectedSize()
        {
            return double.Parse(comboBox_Sizes.SelectedItem.ToString());
        }

        private void PopulateSizeList()
        {
            comboBox_Sizes.Items.Clear();
            var sizes = Analyzer.GetAvailableSegmentSizes(GetSelectedPipeType().RoutingPreferenceManager,
                m_application.ActiveUIDocument.Document);
            foreach (var size in sizes)
            {
                var unitAdjustedSize = DocumentUnits.ConvertValueDocumentUnits(size, m_application.ActiveUIDocument.Document);
                comboBox_Sizes.Items.Add(unitAdjustedSize.ToString());
            }

            comboBox_Sizes.SelectedIndex = 0;
        }

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

        private PipeType GetSelectedPipeType()
        {
            var selectedString = comboBox_PipeTypes.SelectedItem.ToString();
            var selectedsplit = selectedString.Split(':');
            if (selectedsplit.Length != 2)
                throw new ArgumentException();

            var id = ElementId.Parse(selectedsplit[1]);
            return m_application.ActiveUIDocument.Document.GetElement(id) as PipeType;
        }

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

        private void comboBox_PipeTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSizeList();
        }
    }
}
