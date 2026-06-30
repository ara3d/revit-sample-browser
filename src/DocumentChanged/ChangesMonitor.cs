// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Data;

namespace Ara3D.RevitSampleBrowser.DocumentChanged.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        private static ControlledApplication _ctrlApp;

        public static DataTable ChangesInfoTable { get; set; }

        public static ChangesInformationForm InfoForm { get; set; }

        public Result OnStartup(UIControlledApplication application)
        {
            // initialize member variables.
            _ctrlApp = application.ControlledApplication;
            ChangesInfoTable = CreateChangeInfoTable();
            InfoForm = new ChangesInformationForm(ChangesInfoTable);

            // register the DocumentChanged event
            _ctrlApp.DocumentChanged += CtrlApp_DocumentChanged;

            // show dialog
            InfoForm.Show();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            _ctrlApp.DocumentChanged -= CtrlApp_DocumentChanged;
            InfoForm = null;
            ChangesInfoTable = null;
            return Result.Succeeded;
        }

        private void CtrlApp_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var doc = e.GetDocument();

            // dump the element information
            var addedElem = e.GetAddedElementIds();
            foreach (var id in addedElem)
            {
                AddChangeInfoRow(id, doc, "Added");
            }

            var deletedElem = e.GetDeletedElementIds();
            foreach (var id in deletedElem)
            {
                AddChangeInfoRow(id, doc, "Deleted");
            }

            var modifiedElem = e.GetModifiedElementIds();
            foreach (var id in modifiedElem)
            {
                AddChangeInfoRow(id, doc, "Modified");
            }
        }

        private void AddChangeInfoRow(ElementId id, Document doc, string changeType)
        {
            // retrieve the changed element
            var elem = doc.GetElement(id);

            var newRow = ChangesInfoTable.NewRow();

            if (elem == null)
            {
                // this branch is for deleted element due to the deleted element cannot be retrieve from the document.
                newRow["ChangeType"] = changeType;
                newRow["Id"] = id.ToString();
                newRow["Name"] = "";
                newRow["Category"] = "";
                newRow["Document"] = "";
            }
            else
            {
                newRow["ChangeType"] = changeType;
                newRow["Id"] = id.ToString();
                newRow["Name"] = elem.Name;
                newRow["Category"] = elem.Category.Name;
                newRow["Document"] = doc.Title;
            }

            ChangesInfoTable.Rows.Add(newRow);
        }

        private DataTable CreateChangeInfoTable()
        {
            DataTable changesInfoTable = new("ChangesInfoTable");

            DataColumn styleColumn = new("ChangeType", typeof(string))
            {
                Caption = "ChangeType"
            };
            changesInfoTable.Columns.Add(styleColumn);

            DataColumn idColum = new("Id", typeof(string))
            {
                Caption = "Id"
            };
            changesInfoTable.Columns.Add(idColum);

            DataColumn nameColum = new("Name", typeof(string))
            {
                Caption = "Name"
            };
            changesInfoTable.Columns.Add(nameColum);

            DataColumn categoryColum = new("Category", typeof(string))
            {
                Caption = "Category"
            };
            changesInfoTable.Columns.Add(categoryColum);

            DataColumn docColum = new("Document", typeof(string))
            {
                Caption = "Document"
            };
            changesInfoTable.Columns.Add(docColum);

            // return this data table 
            return changesInfoTable;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ExternalApplication.InfoForm ??= new ChangesInformationForm(ExternalApplication.ChangesInfoTable);
            ExternalApplication.InfoForm.Show();

            return Result.Succeeded;
        }
    }
}
