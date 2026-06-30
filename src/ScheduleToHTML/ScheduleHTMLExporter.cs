// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ScheduleToHTML.CS
{
    public class ScheduleHtmlExporter
    {
        private TableSectionData m_bodySection;

        private TableSectionData m_headerSection;

        private readonly ViewSchedule m_theSchedule;

        private HtmlTextWriter m_writer;

        private readonly List<Tuple<int, int>> m_writtenCells = new List<Tuple<int, int>>();

        /// <summary>
        ///     Constructs a new instance of the schedule exporter operating on the input schedule.
        /// </summary>
        /// <param name="input">The schedule to be exported.</param>
        public ScheduleHtmlExporter(ViewSchedule input)
        {
            m_theSchedule = input;
        }

        private static Color Black => new Color(0, 0, 0);

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

        private void WriteHeader()
        {
            // Clear written cells
            m_writtenCells.Clear();

            // Start table to represent the header
            m_writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            m_writer.RenderBeginTag(HtmlTextWriterTag.Table);

            m_headerSection = m_theSchedule.GetTableData().GetSectionData(SectionType.Header);
            var numberOfRows = m_headerSection.NumberOfRows;
            var numberOfColumns = m_headerSection.NumberOfColumns;

            for (var iRow = m_headerSection.FirstRowNumber; iRow < numberOfRows; iRow++)
                WriteHeaderSectionRow(iRow, numberOfColumns);

            // Close header table
            m_writer.RenderEndTag();
        }

        private void WriteBody()
        {
            // Clear written cells
            m_writtenCells.Clear();

            // Write the start of the body table
            m_writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            m_writer.RenderBeginTag(HtmlTextWriterTag.Table);

            m_bodySection = m_theSchedule.GetTableData().GetSectionData(SectionType.Body);
            var numberOfRows = m_bodySection.NumberOfRows;
            var numberOfColumns = m_bodySection.NumberOfColumns;

            for (var iRow = m_bodySection.FirstRowNumber; iRow < numberOfRows; iRow++)
                WriteBodySectionRow(iRow, numberOfColumns);

            // Close the table
            m_writer.RenderEndTag();
        }

        private static string GetColorHtmlString(Color color)
        {
            return string.Format("#{0}{1}{2}", color.Red.ToString("X"), color.Green.ToString("X"),
                color.Blue.ToString("X"));
        }

        private bool ColorsEqual(Color color1, Color color2)
        {
            return color1.Red == color2.Red && color1.Green == color2.Green && color1.Blue == color2.Blue;
        }

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

        private void WriteHeaderSectionRow(int iRow, int numberOfColumns)
        {
            WriteSectionRow(SectionType.Header, m_headerSection, iRow, numberOfColumns);
        }

        private void WriteBodySectionRow(int iRow, int numberOfColumns)
        {
            WriteSectionRow(SectionType.Body, m_bodySection, iRow, numberOfColumns);
        }

        private void WriteSectionRow(SectionType secType, TableSectionData data, int iRow, int numberOfColumns)
        {
            // Start the table row tag.
            m_writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            for (var iCol = data.FirstColumnNumber; iCol < numberOfColumns; iCol++)
            {
                // Skip already written cells
                if (m_writtenCells.Contains(new Tuple<int, int>(iRow, iCol)))
                    continue;

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

internal enum HtmlTextWriterAttribute
{
    Align,
    Bgcolor,
    Border,
    Colspan,
    Rowspan
}

internal enum HtmlTextWriterTag
{
    B,
    Div,
    I,
    Table,
    Td,
    Tr,
    U
}

internal sealed class HtmlTextWriter : IDisposable
{
    private readonly TextWriter m_writer;
    private readonly Stack<string> m_openTags = new Stack<string>();
    private readonly List<Tuple<string, string>> m_pendingAttributes = new List<Tuple<string, string>>();

    public HtmlTextWriter(TextWriter writer)
    {
        m_writer = writer;
    }

    public void AddAttribute(HtmlTextWriterAttribute attribute, string value)
    {
        m_pendingAttributes.Add(new Tuple<string, string>(GetAttributeName(attribute), value));
    }

    public void RenderBeginTag(HtmlTextWriterTag tag)
    {
        var tagName = GetTagName(tag);
        m_writer.Write("<");
        m_writer.Write(tagName);
        foreach (var attribute in m_pendingAttributes)
        {
            m_writer.Write(" ");
            m_writer.Write(attribute.Item1);
            m_writer.Write("=\"");
            m_writer.Write(WebUtility.HtmlEncode(attribute.Item2));
            m_writer.Write("\"");
        }

        m_pendingAttributes.Clear();
        m_writer.Write(">");
        m_openTags.Push(tagName);
    }

    public void RenderEndTag()
    {
        m_writer.Write("</");
        m_writer.Write(m_openTags.Pop());
        m_writer.Write(">");
    }

    public void Write(string value)
    {
        m_writer.Write(value == "&nbsp;" ? value : WebUtility.HtmlEncode(value));
    }

    public void Dispose()
    {
        m_writer?.Dispose();
    }

    private static string GetAttributeName(HtmlTextWriterAttribute attribute)
    {
        return attribute.ToString().ToLowerInvariant();
    }

    private static string GetTagName(HtmlTextWriterTag tag)
    {
        return tag.ToString().ToLowerInvariant();
    }
}
