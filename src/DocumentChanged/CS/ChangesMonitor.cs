// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Data;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ChangesMonitor.CS
{
    /// <summary>
    ///     A class inherits IExternalApplication interface and provide an entry of the sample.
    ///     It create a modeless dialog to track the changes.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        /// <summary>
        ///     A controlled application used to register the DocumentChanged event. Because all trigger points
        ///     in this sample come from UI, the event must be registered to ControlledApplication.
        ///     If the trigger point is from API, user can register it to application
        ///     which can retrieve from ExternalCommand.
        /// </summary>
        private static ControlledApplication _ctrlApp;

        /// <summary>
        ///     data table for information windows.
        /// </summary>
        private static DataTable _changesInfoTable;

        /// <summary>
        ///     The window is used to show changes' information.
        /// </summary>
        private static ChangesInformationForm _infoForm;

        /// <summary>
        ///     Property to get and set private member variables of changes log information.
        /// </summary>
        public static DataTable ChangesInfoTable
        {
            get => _changesInfoTable;
            set => _changesInfoTable = value;
        }

        /// <summary>
        ///     Property to get and set private member variables of info form.
        /// </summary>
        public static ChangesInformationForm InfoForm
        {
            get => _infoForm;
            set => _infoForm = value;
        }

        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the internal reference.
        /// </returns>
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

        /// <summary>
        ///     This method is the event handler, which will dump the change information to tracking dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CtrlApp_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            // get the current document.
            var doc = e.GetDocument();

            // dump the element information
            var addedElem = e.GetAddedElementIds();
            foreach (var id in addedElem) AddChangeInfoRow(id, doc, "Added");

            var deletedElem = e.GetDeletedElementIds();
            foreach (var id in deletedElem) AddChangeInfoRow(id, doc, "Deleted");

            var modifiedElem = e.GetModifiedElementIds();
            foreach (var id in modifiedElem) AddChangeInfoRow(id, doc, "Modified");
        }

        /// <summary>
        ///     This method is used to retrieve the changed element and add row to data table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doc"></param>
        /// <param name="changeType"></param>
        private void AddChangeInfoRow(ElementId id, Document doc, string changeType)
        {
            // retrieve the changed element
            var elem = doc.GetElement(id);

            var newRow = _changesInfoTable.NewRow();

            // set the relative information of this event into the table.
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

        /// <summary>
        ///     Generate a data table with five columns for display in window
        /// </summary>
        /// <returns>The DataTable to be displayed in window</returns>
        private DataTable CreateChangeInfoTable()
        {
            // create a new dataTable
            var changesInfoTable = new DataTable("ChangesInfoTable");

            // Create a "ChangeType" column. It will be "Added", "Deleted" and "Modified".
            var styleColumn = new DataColumn("ChangeType", typeof(string));
            styleColumn.Caption = "ChangeType";
            changesInfoTable.Columns.Add(styleColumn);

            // Create a "Id" column. It will be the Element ID
            var idColum = new DataColumn("Id", typeof(string));
            idColum.Caption = "Id";
            changesInfoTable.Columns.Add(idColum);

            // Create a "Name" column. It will be the Element Name
            var nameColum = new DataColumn("Name", typeof(string));
            nameColum.Caption = "Name";
            changesInfoTable.Columns.Add(nameColum);

            // Create a "Category" column. It will be the Category Name of the element.
            var categoryColum = new DataColumn("Category", typeof(string));
            categoryColum.Caption = "Category";
            changesInfoTable.Columns.Add(categoryColum);

            // Create a "Document" column. It will be the document which own the changed element.
            var docColum = new DataColumn("Document", typeof(string));
            docColum.Caption = "Document";
            changesInfoTable.Columns.Add(docColum);

            // return this data table 
            return changesInfoTable;
        }
    }

    /// <summary>
    ///     This class inherits IExternalCommand interface and used to retrieve the dialog again.
    /// </summary>
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
