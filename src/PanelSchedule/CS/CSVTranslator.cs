// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    /// <summary>
    ///     Translate the panel schedule view data from Revit to CSV.
    /// </summary>
    internal class CsvTranslator : Translator
    {
        /// <summary>
        ///     create a CSVTranslator instance for a PanelScheduleView instance.
        /// </summary>
        /// <param name="psView">the exporting panel schedule view instance.</param>
        public CsvTranslator(PanelScheduleView psView)
        {
            PsView = psView;
        }

        /// <summary>
        ///     export to a CSV file that contains the PanelScheduleView instance data.
        /// </summary>
        /// <returns>the exported file path</returns>
        public override string Export()
        {
            var asemblyName = Assembly.GetExecutingAssembly().Location;

            var panelScheduleCsvFile =
                asemblyName.Replace("PanelSchedule.dll", ReplaceIllegalCharacters(PsView.Name) + ".csv");

            if (File.Exists(panelScheduleCsvFile)) File.Delete(panelScheduleCsvFile);

            using (var sw = File.CreateText(panelScheduleCsvFile))
            {
                //sw.WriteLine("This is my file.");
                DumpPanelScheduleData(sw);
                sw.Close();
            }

            return panelScheduleCsvFile;
        }

        /// <summary>
        ///     dump PanelScheduleData to comma delimited.
        /// </summary>
        /// <param name="sw"></param>
        private void DumpPanelScheduleData(StreamWriter sw)
        {
            DumpSectionData(sw, PsView, SectionType.Header);
            DumpSectionData(sw, PsView, SectionType.Body);
            DumpSectionData(sw, PsView, SectionType.Summary);
            DumpSectionData(sw, PsView, SectionType.Footer);
        }

        /// <summary>
        ///     dump SectionData to comma delimited.
        /// </summary>
        /// <param name="sw">exporting file stream</param>
        /// <param name="psView">the PanelScheduleView instance is exporting.</param>
        /// <param name="sectionType">which section is exporting, it can be Header, Body, Summary or Footer.</param>
        private void DumpSectionData(StreamWriter sw, PanelScheduleView psView, SectionType sectionType)
        {
            var nRowsSection = 0;
            var nColsSection = 0;
            GetNumberOfRowsAndColumns(PsView.Document, PsView, sectionType, ref nRowsSection, ref nColsSection);

            for (var ii = 0; ii < nRowsSection; ++ii)
            {
                var oneRow = new StringBuilder();
                for (var jj = 0; jj < nColsSection; ++jj)
                    try
                    {
                        oneRow.AppendFormat("{0},", PsView.GetCellText(sectionType, ii, jj));
                    }
                    catch (Exception)
                    {
                        // do nothing.
                    }

                sw.WriteLine(oneRow.ToString());
            }
        }
    }
}
