// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Data;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DocumentChanged.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        private static ControlledApplication _ctrlApp;

        private static DataTable _changesInfoTable;

        private static ChangesInformationForm _infoForm;

        public static DataTable ChangesInfoTable
        {
            get => _changesInfoTable;
            set => _changesInfoTable = value;
        }

        public static ChangesInformationForm InfoForm
        {
            get => _infoForm;
            set => _infoForm = value;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // initialize member variables.
            _ctrlApp = application.ControlledApplication;
            _changesInfoTable = CreateChangeInfoTable();
            _infoForm = new ChangesInformationForm(ChangesInfoTable);

            // register the DocumentChanged event
            _ctrlApp.DocumentChanged += CtrlApp_DocumentChanged;

            // show dialog
            _infoForm.Show();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            _ctrlApp.DocumentChanged -= CtrlApp_DocumentChanged;
            _infoForm = null;
            _changesInfoTable = null;
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

            var newRow = _changesInfoTable.NewRow();

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

            _changesInfoTable.Rows.Add(newRow);
        }

        private DataTable CreateChangeInfoTable()
        {
            var changesInfoTable = new DataTable("ChangesInfoTable");

            var styleColumn = new DataColumn("ChangeType", typeof(string))
            {
                Caption = "ChangeType"
            };
            changesInfoTable.Columns.Add(styleColumn);

            var idColum = new DataColumn("Id", typeof(string))
            {
                Caption = "Id"
            };
            changesInfoTable.Columns.Add(idColum);

            var nameColum = new DataColumn("Name", typeof(string))
            {
                Caption = "Name"
            };
            changesInfoTable.Columns.Add(nameColum);

            var categoryColum = new DataColumn("Category", typeof(string))
            {
                Caption = "Category"
            };
            changesInfoTable.Columns.Add(categoryColum);

            var docColum = new DataColumn("Document", typeof(string))
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
            if (ExternalApplication.InfoForm == null)
                ExternalApplication.InfoForm = new ChangesInformationForm(ExternalApplication.ChangesInfoTable);
            ExternalApplication.InfoForm.Show();

            return Result.Succeeded;
        }
    }
}
