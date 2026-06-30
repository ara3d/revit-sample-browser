// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    public abstract class Translator
    {
        protected PanelScheduleView PsView;

        public abstract string Export();

        protected string ReplaceIllegalCharacters(string stringWithIllegalChar)
        {
            return ScheduleHelper.ReplaceIllegalCharacters(stringWithIllegalChar);
        }

        protected void GetNumberOfRowsAndColumns(Document doc, PanelScheduleView psView, SectionType sectionType,
            ref int nRows, ref int nCols)
        {
            ScheduleHelper.GetNumberOfRowsAndColumns(doc, psView, sectionType, ref nRows, ref nCols);
        }
    }
}
