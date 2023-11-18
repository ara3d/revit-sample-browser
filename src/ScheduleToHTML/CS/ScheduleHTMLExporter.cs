// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ScheduleToHTML.CS
{
    /// <summary>
    ///     A class that can export a schedule to HTML.
    /// </summary>
    public class ScheduleHtmlExporter
    {
        /// <summary>
        ///     The body section of the table.
        /// </summary>
        private TableSectionData m_bodySection;

        /// <summary>
        ///     The header section of the table.
        /// </summary>
        private TableSectionData m_headerSection;

        /// <summary>
        ///     The schedule being exported.
        /// </summary>
        private readonly ViewSchedule m_theSchedule;

        /// <summary>
        ///     The writer for the HTML file.
        /// </summary>
        private HtmlTextWriter m_writer;

        /// <summary>
        ///     A collection of cells which have already been output.  This is needed to deal with
        ///     cell merging - each cell should be written only once even as all the cells are iterated in
        ///     order.
        /// </summary>
        private readonly List<Tuple<int, int>> m_writtenCells = new List<Tuple<int, int>>();

        /// <summary>
        ///     Constructs a new instance of the schedule exporter operating on the input schedule.
        /// </summary>
        /// <param name="input">The schedule to be exported.</param>
        public ScheduleHtmlExporter(ViewSchedule input)
        {
            m_theSchedule = input;
        }

        /// <summary>
        ///     A predefined color value used for comparison.
        /// </summary>
        private static Color Black => new Color(0, 0, 0);

        /// <summary>
        ///     A predefined color value used for comparison.
        /// </summary>
        private static Color White => new Color(255, 255, 255);

        /// <summary>
        ///     Exports the schedule to formatted HTML.
        /// </summary>
        /// <param name="bInteractive">true if the export is being run interactively, false for journal playback.</param>
        /// <param name="errMessage">String to contain message to display if the export fails.</param>
        /// <returns>true if HTML exported without error, false otherswise.</returns>
        public bool ExportToHtml(bool bInteractive, ref string errMessage)
        {
            // Setup file location in temp directory
            var folder = Environment.GetEnvironmentVariable("TEMP");
            var htmlFile = Path.Combine(folder, $"{ReplaceIllegalCharacters(m_theSchedule.Name)}.html");

            // Initialize StringWriter instance, but handle any io exceptions and close as appropriate. 
            StreamWriter stringWriter = null;
            try
            {
                stringWriter = new StreamWriter(htmlFile);

                // Put HtmlTextWriter in using block because it needs to call Dispose.
                using (m_writer = new HtmlTextWriter(stringWriter))
                {
                    m_writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                    m_writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    //Write schedule header
                    WriteHeader();

                    //Write schedule body
                    WriteBody();

                    m_writer.RenderEndTag();
                }
            }
            catch (IOException e)
            {
                // set error message and return failure,  finally will close stringWriter if necessary. 
                errMessage = $"Exception occured generating HTML: {e.Message} Command canceled.";
                return false;
            }
            finally
            {
                stringWriter?.Close();
            }

            // Show the created file, but only if in interactive mode.
            if (bInteractive)
                Process.Start(htmlFile);
            return true;
        }

        /// <summary>
        ///     Writes the header section of the table to the HTML file.
        /// </summary>
        private void WriteHeader()
        {
            // Clear written cells
            m_writtenCells.Clear();

            // Start table to represent the header
            m_writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            m_writer.RenderBeginTag(HtmlTextWriterTag.Table);

            // Get header section and write each cell
            m_headerSection = m_theSchedule.GetTableData().GetSectionData(SectionType.Header);
            var numberOfRows = m_headerSection.NumberOfRows;
            var numberOfColumns = m_headerSection.NumberOfColumns;

            for (var iRow = m_headerSection.FirstRowNumber; iRow < numberOfRows; iRow++)
                WriteHeaderSectionRow(iRow, numberOfColumns);

            // Close header table
            m_writer.RenderEndTag();
        }

        /// <summary>
        ///     Writes the body section of the table to the HTML file.
        /// </summary>
        private void WriteBody()
        {
            // Clear written cells
            m_writtenCells.Clear();

            // Write the start of the body table
            m_writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            m_writer.RenderBeginTag(HtmlTextWriterTag.Table);

            // Get body section and write contents
            m_bodySection = m_theSchedule.GetTableData().GetSectionData(SectionType.Body);
            var numberOfRows = m_bodySection.NumberOfRows;
            var numberOfColumns = m_bodySection.NumberOfColumns;

            for (var iRow = m_bodySection.FirstRowNumber; iRow < numberOfRows; iRow++)
                WriteBodySectionRow(iRow, numberOfColumns);

            // Close the table
            m_writer.RenderEndTag();
        }

        /// <summary>
        ///     Gets the Color value formatted for HTML (#XXXXXX) output.
        /// </summary>
        /// <param name="color">he color.</param>
        /// <returns>The color string.</returns>
        private static string GetColorHtmlString(Color color)
        {
            return string.Format("#{0}{1}{2}", color.Red.ToString("X"), color.Green.ToString("X"),
                color.Blue.ToString("X"));
        }

        /// <summary>
        ///     Compares two colors.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <returns>True if the colors are equal, false otherwise.</returns>
        private bool ColorsEqual(Color color1, Color color2)
        {
            return color1.Red == color2.Red && color1.Green == color2.Green && color1.Blue == color2.Blue;
        }

        /// <summary>
        ///     Gets the HTML string representing this horizontal alignment.
        /// </summary>
        /// <param name="style">The horizontal alignment.</param>
        /// <returns>The related string.</returns>
        private static string GetAlignString(HorizontalAlignmentStyle style)
        {
            switch (style)
            {
                case HorizontalAlignmentStyle.Left:
                    return "left";
                case HorizontalAlignmentStyle.Center:
                    return "center";
                case HorizontalAlignmentStyle.Right:
                    return "right";
            }

            return "";
        }

        /// <summary>
        ///     Writes a row of the header.
        /// </summary>
        /// <param name="iRow">The row number.</param>
        /// <param name="numberOfColumns">The number of columns to write.</param>
        private void WriteHeaderSectionRow(int iRow, int numberOfColumns)
        {
            WriteSectionRow(SectionType.Header, m_headerSection, iRow, numberOfColumns);
        }

        /// <summary>
        ///     Writes a row of the body.
        /// </summary>
        /// <param name="iRow">The row number.</param>
        /// <param name="numberOfColumns">The number of columns to write.</param>
        private void WriteBodySectionRow(int iRow, int numberOfColumns)
        {
            WriteSectionRow(SectionType.Body, m_bodySection, iRow, numberOfColumns);
        }

        /// <summary>
        ///     Writes a row of a table section.
        /// </summary>
        /// <param name="iRow">The row number.</param>
        /// <param name="numberOfColumns">The number of columns to write.</param>
        /// <param name="secType">The section type.</param>
        /// <param name="data">The table section data.</param>
        private void WriteSectionRow(SectionType secType, TableSectionData data, int iRow, int numberOfColumns)
        {
            // Start the table row tag.
            m_writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            // Loop over the table section row.
            for (var iCol = data.FirstColumnNumber; iCol < numberOfColumns; iCol++)
            {
                // Skip already written cells
                if (m_writtenCells.Contains(new Tuple<int, int>(iRow, iCol)))
                    continue;

                // Get style
                var style = data.GetTableCellStyle(iRow, iCol);
                var numberOfStyleTags = 1;

                // Merged cells
                var mergedCell = data.GetMergedCell(iRow, iCol);

                // If merged cell spans multiple columns
                if (mergedCell.Left != mergedCell.Right)
                    m_writer.AddAttribute(HtmlTextWriterAttribute.Colspan,
                        (mergedCell.Right - mergedCell.Left + 1).ToString());

                // If merged cell spans multiple rows
                if (mergedCell.Top != mergedCell.Bottom)
                    m_writer.AddAttribute(HtmlTextWriterAttribute.Rowspan,
                        (mergedCell.Bottom - mergedCell.Top + 1).ToString());

                // Remember all written cells related to the merge 
                for (var iMergedRow = mergedCell.Top; iMergedRow <= mergedCell.Bottom; iMergedRow++)
                for (var iMergedCol = mergedCell.Left; iMergedCol <= mergedCell.Right; iMergedCol++)
                    m_writtenCells.Add(new Tuple<int, int>(iMergedRow, iMergedCol));

                // Write formatting attributes for the upcoming cell
                // Background color
                if (!ColorsEqual(style.BackgroundColor, White))
                    m_writer.AddAttribute(HtmlTextWriterAttribute.Bgcolor, GetColorHtmlString(style.BackgroundColor));

                // Horizontal alignment
                m_writer.AddAttribute(HtmlTextWriterAttribute.Align, GetAlignString(style.FontHorizontalAlignment));

                // Write cell tag
                m_writer.RenderBeginTag(HtmlTextWriterTag.Td);

                // Write subtags for the cell
                // Underline
                if (style.IsFontUnderline)
                {
                    m_writer.RenderBeginTag(HtmlTextWriterTag.U);
                    numberOfStyleTags++;
                }

                //Italic
                if (style.IsFontItalic)
                {
                    m_writer.RenderBeginTag(HtmlTextWriterTag.I);
                    numberOfStyleTags++;
                }

                //Bold
                if (style.IsFontBold)
                {
                    m_writer.RenderBeginTag(HtmlTextWriterTag.B);
                    numberOfStyleTags++;
                }

                // Write cell text
                var cellText = m_theSchedule.GetCellText(secType, iRow, iCol);
                if (cellText.Length > 0)
                    m_writer.Write(cellText);
                else
                    m_writer.Write("&nbsp;");

                // Close open style tags & cell tag
                for (var i = 0; i < numberOfStyleTags; i++) m_writer.RenderEndTag();
            }

            // Close row tag
            m_writer.RenderEndTag();
        }

        /// <summary>
        ///     An utility method to replace illegal characters of the Schedule name when creating the HTML file name.
        /// </summary>
        /// <param name="stringWithIllegalChar">The Schedule name.</param>
        /// <returns>The updated string without illegal characters.</returns>
        private static string ReplaceIllegalCharacters(string stringWithIllegalChar)
        {
            var illegalChars = Path.GetInvalidFileNameChars();

            var updated = stringWithIllegalChar;
            foreach (var ch in illegalChars)
            {
                updated = updated.Replace(ch, '_');
            }

            return updated;
        }
    }
}
