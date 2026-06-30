// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Data;
using System.IO;

using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class ScheduleHelper
    {
        public static string ReplaceIllegalCharacters(string stringWithIllegalChar)
        {
            var updated = stringWithIllegalChar;
            foreach (var ch in Path.GetInvalidFileNameChars())
                updated = updated.Replace(ch, '_');
            return updated;
        }

        public static void GetNumberOfRowsAndColumns(Document doc, PanelScheduleView psView, SectionType sectionType,
                    ref int nRows, ref int nCols)
        {
            using Transaction openSectionData = new(doc, "openSectionData");
            openSectionData.Start();
            var sectionData = psView.GetSectionData(sectionType);
            nRows = sectionData.NumberOfRows;
            nCols = sectionData.NumberOfColumns;
            openSectionData.RollBack();
        }

        public static void CreateAndAddSchedules(UIDocument uiDocument)
        {
            TransactionGroup tGroup = new(uiDocument.Document, "Create schedules and sheets");
            tGroup.Start();

            foreach (var schedule in ViewHelper.CreateSchedules(uiDocument))
                ViewHelper.AddScheduleToNewSheet(uiDocument.Document, schedule);

            tGroup.Assimilate();
        }

        public static DataTable CreateTable()
        {
            DataTable propDataTable = new("ParameterTable");
            propDataTable.Columns.Add(new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Parameter",
                Caption = "Parameter",
                ReadOnly = true
            });
            propDataTable.Columns.Add(new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Value",
                Caption = "Value",
                ReadOnly = true
            });
            return propDataTable;
        }

        public static void AddDataRow(string parameterName, string value, DataTable parameterTable)
        {
            var newRow = parameterTable.NewRow();
            newRow["Parameter"] = parameterName;
            newRow["Value"] = value;
            parameterTable.Rows.Add(newRow);
        }

    }
}