//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Data;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace Revit.SDK.Samples.ChangesMonitor.CS
{
    /// <summary>
    /// A class inherits IExternalApplication interface and provide an entry of the sample.
    /// It create a modeless dialog to track the changes.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
                /// <summary>
        /// A controlled application used to register the DocumentChanged event. Because all trigger points
        /// in this sample come from UI, the event must be registered to ControlledApplication. 
        /// If the trigger point is from API, user can register it to application 
        /// which can retrieve from ExternalCommand.
        /// </summary>
        private static ControlledApplication m_CtrlApp;

        /// <summary>
        /// data table for information windows.
        /// </summary>
        private static DataTable m_ChangesInfoTable;

        /// <summary>
        /// The window is used to show changes' information.
        /// </summary>
        private static ChangesInformationForm m_InfoForm;
                
                /// <summary>
        /// Property to get and set private member variables of changes log information.
        /// </summary>
        public static DataTable ChangesInfoTable
        {
            get => m_ChangesInfoTable;
            set => m_ChangesInfoTable = value;
        }

        /// <summary>
        /// Property to get and set private member variables of info form.
        /// </summary>
        public static ChangesInformationForm InfoForm
        {
            get => m_InfoForm;
            set => m_InfoForm = value;
        }
                
                /// <summary>
        /// Implement this method to implement the external application which should be called when 
        /// Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">An object that is passed to the external application 
        /// which contains the controlled application.</param> 
        /// <returns>Return the status of the external application. 
        /// A result of Succeeded means that the external application successfully started. 
        /// Cancelled can be used to signify that the user cancelled the external operation at 
        /// some point.
        /// If false is returned then Revit should inform the user that the external application 
        /// failed to load and the release the internal reference.</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // initialize member variables.
            m_CtrlApp = application.ControlledApplication;
            m_ChangesInfoTable = CreateChangeInfoTable();
            m_InfoForm = new ChangesInformationForm(ChangesInfoTable);

            // register the DocumentChanged event
            m_CtrlApp.DocumentChanged += new EventHandler<Autodesk.Revit.DB.Events.DocumentChangedEventArgs>(CtrlApp_DocumentChanged);

            // show dialog
            m_InfoForm.Show();

            return Result.Succeeded;
        }

        /// <summary>
        /// Implement this method to implement the external application which should be called when 
        /// Revit is about to exit,Any documents must have been closed before this method is called.
        /// </summary>
        /// <param name="application">An object that is passed to the external application 
        /// which contains the controlled application.</param>
        /// <returns>Return the status of the external application. 
        /// A result of Succeeded means that the external application successfully shutdown. 
        /// Cancelled can be used to signify that the user cancelled the external operation at 
        /// some point.
        /// If false is returned then the Revit user should be warned of the failure of the external 
        /// application to shut down correctly.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            m_CtrlApp.DocumentChanged -= CtrlApp_DocumentChanged;
            m_InfoForm = null;
            m_ChangesInfoTable = null;
            return Result.Succeeded;
        }
        
                /// <summary>
        /// This method is the event handler, which will dump the change information to tracking dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CtrlApp_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            // get the current document.
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
        
                /// <summary>
        /// This method is used to retrieve the changed element and add row to data table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doc"></param>
        /// <param name="changeType"></param>
        private void AddChangeInfoRow(ElementId id, Document doc, string changeType)
        {
            // retrieve the changed element
            var elem = doc.GetElement(id);

            var newRow = m_ChangesInfoTable.NewRow();

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
           
            m_ChangesInfoTable.Rows.Add(newRow);
        }

        /// <summary>
        /// Generate a data table with five columns for display in window
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
    /// This class inherits IExternalCommand interface and used to retrieve the dialog again.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
                public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (ExternalApplication.InfoForm == null)
            {
                ExternalApplication.InfoForm = new ChangesInformationForm(ExternalApplication.ChangesInfoTable);
            }
            ExternalApplication.InfoForm.Show();

            return Result.Succeeded;
        }
            }

}
