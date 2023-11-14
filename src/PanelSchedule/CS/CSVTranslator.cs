// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    ///     Translate the panel schedule view data from Revit to CSV.
    /// </summary>
    internal class CSVTranslator : Translator
    {
        /// <summary>
        ///     create a CSVTranslator instance for a PanelScheduleView instance.
        /// </summary>
        /// <param name="psView">the exporting panel schedule view instance.</param>
        public CSVTranslator(PanelScheduleView psView)
        {
            m_psView = psView;
        }

        /// <summary>
        ///     export to a CSV file that contains the PanelScheduleView instance data.
        /// </summary>
        /// <returns>the exported file path</returns>
        public override string Export()
        {
            var asemblyName = Assembly.GetExecutingAssembly().Location;

            var panelScheduleCSVFile =
                asemblyName.Replace("PanelSchedule.dll", ReplaceIllegalCharacters(m_psView.Name) + ".csv");

            if (File.Exists(panelScheduleCSVFile)) File.Delete(panelScheduleCSVFile);

            using (var sw = File.CreateText(panelScheduleCSVFile))
            {
                //sw.WriteLine("This is my file.");
                DumpPanelScheduleData(sw);
                sw.Close();
            }

            return panelScheduleCSVFile;
        }

        /// <summary>
        ///     dump PanelScheduleData to comma delimited.
        /// </summary>
        /// <param name="sw"></param>
        private void DumpPanelScheduleData(StreamWriter sw)
        {
            DumpSectionData(sw, m_psView, SectionType.Header);
            DumpSectionData(sw, m_psView, SectionType.Body);
            DumpSectionData(sw, m_psView, SectionType.Summary);
            DumpSectionData(sw, m_psView, SectionType.Footer);
        }

        /// <summary>
        ///     dump SectionData to comma delimited.
        /// </summary>
        /// <param name="sw">exporting file stream</param>
        /// <param name="psView">the PanelScheduleView instance is exporting.</param>
        /// <param name="sectionType">which section is exporting, it can be Header, Body, Summary or Footer.</param>
        private void DumpSectionData(StreamWriter sw, PanelScheduleView psView, SectionType sectionType)
        {
            var nRows_Section = 0;
            var nCols_Section = 0;
            getNumberOfRowsAndColumns(m_psView.Document, m_psView, sectionType, ref nRows_Section, ref nCols_Section);

            for (var ii = 0; ii < nRows_Section; ++ii)
            {
                var oneRow = new StringBuilder();
                for (var jj = 0; jj < nCols_Section; ++jj)
                    try
                    {
                        oneRow.AppendFormat("{0},", m_psView.GetCellText(sectionType, ii, jj));
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
