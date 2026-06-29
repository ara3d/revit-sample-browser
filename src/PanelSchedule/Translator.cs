// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    /// <summary>
    ///     Translate the panel schedule view data from Revit to some formats, HTML, CSV etc.
    /// </summary>
    public abstract class Translator
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
        protected string ReplaceIllegalCharacters(string stringWithIllegalChar) =>
            ScheduleHelper.ReplaceIllegalCharacters(stringWithIllegalChar);

        protected void GetNumberOfRowsAndColumns(Document doc, PanelScheduleView psView, SectionType sectionType,
            ref int nRows, ref int nCols) =>
            ScheduleHelper.GetNumberOfRowsAndColumns(doc, psView, sectionType, ref nRows, ref nCols);
    }
}
