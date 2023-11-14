// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    ///     Translate the panel schedule view data from Revit to some formats, HTML, CSV etc.
    /// </summary>
    internal abstract class Translator
    {
        /// <summary>
        ///     the panel schedule view instance to be exported.
        /// </summary>
        protected PanelScheduleView PsView;

        public abstract string Export();

        /// <summary>
        ///     An utility method to replace illegal characters of the Panel Schedule view name.
        /// </summary>
        /// <param name="stringWithIllegalChar">the Panel Schedule view name.</param>
        /// <returns>the updated string without illegal characters.</returns>
        protected string ReplaceIllegalCharacters(string stringWithIllegalChar)
        {
            var illegalChars = Path.GetInvalidFileNameChars();

            var updated = stringWithIllegalChar;
            foreach (var ch in illegalChars) updated = updated.Replace(ch, '_');

            return updated;
        }

        /// <summary>
        ///     An utility method to get the number of rows and columns of the section which is exporting.
        /// </summary>
        /// <param name="doc">Revit document.</param>
        /// <param name="psView">the exporting panel schedule view</param>
        /// <param name="sectionType">the exporting section of the panel schedule.</param>
        /// <param name="nRows">the number of rows.</param>
        /// <param name="nCols">the number of columns.</param>
        protected void GetNumberOfRowsAndColumns(Document doc, PanelScheduleView psView, SectionType sectionType,
            ref int nRows, ref int nCols)
        {
            var openSectionData = new Transaction(doc, "openSectionData");
            openSectionData.Start();

            var sectionData = psView.GetSectionData(sectionType);
            nRows = sectionData.NumberOfRows;
            nCols = sectionData.NumberOfColumns;

            openSectionData.RollBack();
        }
    }
}
