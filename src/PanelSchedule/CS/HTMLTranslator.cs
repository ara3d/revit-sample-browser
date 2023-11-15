// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    /// <summary>
    ///     Translate the panel schedule view data from Revit to HTML table.
    /// </summary>
    internal class HtmlTranslator : Translator
    {
        /// <summary>
        ///     create a Translator instance for a PanelScheduleView instance.
        /// </summary>
        /// <param name="psView">the exporting panel schedule view instance.</param>
        public HtmlTranslator(PanelScheduleView psView)
        {
            PsView = psView;
        }

        /// <summary>
        ///     export to a HTML page that contains the PanelScheduleView instance data.
        /// </summary>
        /// <returns>the exported file path</returns>
        public override string Export()
        {
            var asemblyName = Assembly.GetExecutingAssembly().Location;
            var tempFile = asemblyName.Replace("PanelSchedule.dll", "template.html");

            if (!File.Exists(tempFile))
            {
                var messageDlg = new TaskDialog("Warnning Message");
                messageDlg.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                messageDlg.MainContent =
                    "Can not find 'template.html', please make sure the 'template.html' file is in the same folder as the external command assembly.";
                messageDlg.Show();
                return null;
            }

            var panelScheduleFile =
                asemblyName.Replace("PanelSchedule.dll", ReplaceIllegalCharacters(PsView.Name) + ".html");

            var doc = new XmlDocument();
            var tw = new XmlTextWriter(panelScheduleFile, null);
            doc.Load(tempFile);

            var psTable = doc.DocumentElement.SelectSingleNode("//div/table[1]");
            DumpPanelScheduleData(psTable, doc);

            doc.Save(tw);

            return panelScheduleFile;
        }

        /// <summary>
        ///     dump PanelScheduleData to a 'table' node in HTML.
        /// </summary>
        /// <param name="panelScheduleDataNode">a 'table' node in HTML.</param>
        /// <param name="doc"></param>
        private void DumpPanelScheduleData(XmlNode panelScheduleDataNode, XmlDocument doc)
        {
            DumpSectionData(panelScheduleDataNode, doc, PsView, SectionType.Header);
            DumpSectionData(panelScheduleDataNode, doc, PsView, SectionType.Body);
            DumpSectionData(panelScheduleDataNode, doc, PsView, SectionType.Summary);
            DumpSectionData(panelScheduleDataNode, doc, PsView, SectionType.Footer);
        }

        /// <summary>
        ///     dump SectionData to the 'tr' nodes in HTML.
        /// </summary>
        /// <param name="panelScheduleDataNode">a 'table' node in HTML.</param>
        /// <param name="doc">HTML page</param>
        /// <param name="psView">the PanelScheduleView instance is exporting.</param>
        /// <param name="sectionType">which section is exporting, it can be Header, Body, Summary or Footer.</param>
        private void DumpSectionData(XmlNode panelScheduleDataNode, XmlDocument doc, PanelScheduleView psView,
            SectionType sectionType)
        {
            var nRowsSection = 0;
            var nColsSection = 0;
            GetNumberOfRowsAndColumns(PsView.Document, PsView, sectionType, ref nRowsSection, ref nColsSection);

            for (var ii = 0; ii < nRowsSection; ++ii)
            {
                // add a <tr> node for each row
                var trNode = doc.CreateElement("tr");
                panelScheduleDataNode.AppendChild(trNode);

                for (var jj = 0; jj < nColsSection; ++jj)
                {
                    // add <td> node for each cell
                    var tdNode = doc.CreateElement("td");

                    try
                    {
                        tdNode.InnerText = PsView.GetCellText(sectionType, ii, jj);
                    }
                    catch (Exception)
                    {
                        // do nothing.
                    }

                    trNode.AppendChild(tdNode);
                }
            }
        }
    }
}
