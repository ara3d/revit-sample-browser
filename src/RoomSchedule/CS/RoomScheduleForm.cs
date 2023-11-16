// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.RoomSchedule.CS
{
    /// <summary>
    ///     Room Schedule form, used to retrieve data from .xls data source and create new rooms.
    /// </summary>
    public partial class RoomScheduleForm : Form
    {
        // All levels in Revit document.
        private readonly List<Level> m_allLevels = new List<Level>();

        // All available phases in Revit document.
        private readonly List<Phase> m_allPhases = new List<Phase>();

        // Revit external command data
        private readonly ExternalCommandData m_commandData;

        // Reserve name of data source
        private string m_dataBaseName;

        // Current active document
        private readonly Document m_document;

        // Room data information
        private readonly RoomsData m_roomData;

        // Room work sheet name
        private string m_roomTableName;

        // All rooms data from spread sheet
        private DataTable m_spreadRoomsTable;

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="commandData">Revit external command data</param>
        public RoomScheduleForm(ExternalCommandData commandData)
        {
            // UI initialization 
            InitializeComponent();

            // reserve Revit command data and get rooms information, 
            // and then display rooms information in DataGrideView
            m_commandData = commandData;
            m_document = m_commandData.Application.ActiveUIDocument.Document;
            m_roomData = new RoomsData(commandData.Application.ActiveUIDocument.Document);

            // bind levels and phases data to level and phase ComboBox controls
            GetAllLevelsAndPhases();

            // list all levels and phases
            levelComboBox.DisplayMember = "Name";
            levelComboBox.DataSource = m_allLevels;
            levelComboBox.SelectedIndex = 0;

            phaseComboBox.DisplayMember = "Name";
            phaseComboBox.DataSource = m_allPhases;
            phaseComboBox.SelectedIndex = 0;

            // if there is no phase, newRoomButton will be disabled.
            if (m_allPhases.Count == 0) newRoomButton.Enabled = false;

            // check to see whether current Revit document was mapped to spreadsheet.
            UpdateRoomMapSheetInfo();
        }

        /// <summary>
        ///     Get all available levels and phases from current document
        /// </summary>
        private void GetAllLevelsAndPhases()
        {
            // get all levels which can place rooms
            foreach (PlanTopology planTopology in m_document.PlanTopologies) m_allLevels.Add(planTopology.Level);

            // get all phases by filter type
            var collector = new FilteredElementCollector(m_document);
            ICollection<Element> allPhases = collector.OfClass(typeof(Phase)).ToElements();
            foreach (Phase phs in allPhases) m_allPhases.Add(phs);
        }

        /// <summary>
        ///     Create shared parameter for Rooms category
        /// </summary>
        /// <returns>True, shared parameter exists; false, doesn't exist</returns>
        private bool CreateMyRoomSharedParameter()
        {
            // Create Room Shared Parameter Routine: -->
            // 1: Check whether the Room shared parameter("External Room ID") has been defined.
            // 2: Share parameter file locates under sample directory of this .dll module.
            // 3: Add a group named "SDKSampleRoomScheduleGroup".
            // 4: Add a shared parameter named "External Room ID" to "Rooms" category, which is visible.
            //    The "External Room ID" parameter will be used to map to spreadsheet based room ID(which is unique)

            try
            {
                // check whether shared parameter exists
                if (ShareParameterExists(RoomsData.SharedParam)) return true;

                // create shared parameter file
                var modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var paramFile = modulePath + "\\RoomScheduleSharedParameters.txt";
                if (File.Exists(paramFile)) File.Delete(paramFile);
                var fs = File.Create(paramFile);
                fs.Close();

                // cache application handle
                var revitApp = m_commandData.Application.Application;

                // prepare shared parameter file
                m_commandData.Application.Application.SharedParametersFilename = paramFile;

                // open shared parameter file
                var parafile = revitApp.OpenSharedParameterFile();

                // create a group
                var apiGroup = parafile.Groups.Create("SDKSampleRoomScheduleGroup");

                // create a visible "External Room ID" of text type.
                var externalDefinitionCreationOptions =
                    new ExternalDefinitionCreationOptions(RoomsData.SharedParam, SpecTypeId.String.Text);
                var roomSharedParamDef = apiGroup.Definitions.Create(externalDefinitionCreationOptions);

                // get Rooms category
                var roomCat =
                    m_commandData.Application.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory
                        .OST_Rooms);
                var categories = revitApp.Create.NewCategorySet();
                categories.Insert(roomCat);

                // insert the new parameter
                var binding = revitApp.Create.NewInstanceBinding(categories);
                m_commandData.Application.ActiveUIDocument.Document.ParameterBindings.Insert(roomSharedParamDef,
                    binding);
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create shared parameter: " + ex.Message);
            }
        }

        /// <summary>
        ///     Test if the Room binds a specified shared parameter
        /// </summary>
        /// <param name="paramName">Parameter name to be checked</param>
        /// <returns>true, the definition exists, false, doesn't exist.</returns>
        private bool ShareParameterExists(string paramName)
        {
            var bindingMap = m_document.ParameterBindings;
            var iter = bindingMap.ForwardIterator();
            iter.Reset();

            while (iter.MoveNext())
            {
                var tempDefinition = iter.Key;

                // find the definition of which the name is the appointed one
                if (string.Compare(tempDefinition.Name, paramName) != 0) continue;

                // get the category which is bound
                var binding = bindingMap.get_Item(tempDefinition) as ElementBinding;
                var bindCategories = binding.Categories;
                foreach (Category category in bindCategories)
                    if (category.Name
                        == m_document.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms).Name)
                        // the definition with appointed name was bound to Rooms, return true
                        return true;
            }

            // 
            // return false if shared parameter doesn't exist
            return false;
        }

        /// <summary>
        ///     My custom message box
        /// </summary>
        /// <param name="strMsg">message to be popped up</param>
        /// <param name="icon">icon to be displayed</param>
        public static void MyMessageBox(string strMsg, MessageBoxIcon icon)
        {
            TaskDialog.Show("Room Schedule", strMsg, TaskDialogCommonButtons.Ok);
        }

        /// <summary>
        ///     Update control display of form
        ///     call this method when create new rooms or switch the room show(show all or show by level)
        /// </summary>
        /// <param name="bUpdateAllRooms">whether retrieve all rooms from Revit project once more</param>
        private void UpdateFormDisplay(bool bUpdateAllRooms)
        {
            // update Revit Rooms data when there is room creation
            if (bUpdateAllRooms) m_roomData.UpdateRoomsData();

            revitRoomDataGridView.DataSource = null;
            if (showAllRoomsCheckBox.Checked)
                // show all rooms in Revit project
                revitRoomDataGridView.DataSource = new DataView(m_roomData.GenRoomsDataTable(null));
            else
                // show all rooms in specified level
                levelComboBox_SelectedIndexChanged(null, null);

            // update this DataGridView
            revitRoomDataGridView.Update();
        }

        /// <summary>
        ///     Display current Room sheet information: Excel path
        /// </summary>
        private void UpdateRoomMapSheetInfo()
        {
            var hashCode = m_document.GetHashCode();
            var xlsAndTable = new SheetInfo("", "");
            if (CrtlApplication.EventReactor.DocMappedSheetInfo(hashCode, ref xlsAndTable))
                roomExcelTextBox.Text = "Mapped Sheet: " + xlsAndTable.FileName + ": " + xlsAndTable.SheetName;
        }

        /// <summary>
        ///     Some preparation and check before creating room.
        /// </summary>
        /// <param name="curPhase">Current phase used to create room, all rooms will be created in this phase.</param>
        /// <returns>Number indicates how many new rooms were created.</returns>
        private int RoomCreationStart()
        {
            var nNewRoomsSize = 0;
            // transaction is used to cancel room creation when exception occurs
            var myTransaction = new SubTransaction(m_document);
            try
            {
                // Preparation before room creation starts
                Phase curPhase = null;
                if (!RoomCreationPreparation(ref curPhase)) return 0;

                // get all existing rooms which have mapped to spreadsheet rooms.
                // we should skip the creation for those spreadsheet rooms which have been mapped by Revit rooms.
                var existingRooms = new Dictionary<ElementId, string>();
                foreach (var room in m_roomData.Rooms)
                {
                    var sharedParameter = room.LookupParameter(RoomsData.SharedParam);
                    if (null != sharedParameter && false == string.IsNullOrEmpty(sharedParameter.AsString()))
                        existingRooms.Add(room.Id, sharedParameter.AsString());
                }

                myTransaction.Start();
                // create rooms with spread sheet based rooms data
                for (var row = 0; row < m_spreadRoomsTable.Rows.Count; row++)
                {
                    // get the ID column value and use it to check whether this spreadsheet room is mapped by Revit room.
                    var externaId = m_spreadRoomsTable.Rows[row][RoomsData.RoomId].ToString();
                    if (existingRooms.ContainsValue(externaId))
                        // skip the spreadsheet room creation if it's mapped by Revit room
                        continue;

                    // create rooms in specified phase, but without placing them.
                    var newRoom = m_document.Create.NewRoom(curPhase);
                    if (null == newRoom)
                    {
                        // abort the room creation and pop up failure message
                        myTransaction.RollBack();

                        MyMessageBox("Create room failed.", MessageBoxIcon.Warning);
                        return 0;
                    }

                    // set the shared parameter's value of Revit room 
                    var sharedParam = newRoom.LookupParameter(RoomsData.SharedParam);
                    if (null == sharedParam)
                    {
                        // abort the room creation and pop up failure message
                        myTransaction.RollBack();
                        MyMessageBox("Failed to get shared parameter, please try again.", MessageBoxIcon.Warning);
                        return 0;
                    }

                    sharedParam.Set(externaId);

                    // Update this new room with values of spreadsheet
                    UpdateNewRoom(newRoom, row);

                    // remember how many new rooms were created, based on spread sheet data
                    nNewRoomsSize++;
                }

                // end this transaction if create all rooms successfully.
                myTransaction.Commit();
            }
            catch (Exception ex)
            {
                // cancel this time transaction when exception occurs
                if (myTransaction.HasStarted()) myTransaction.RollBack();

                MyMessageBox(ex.Message, MessageBoxIcon.Warning);
                return 0;
            }

            // output unplaced rooms creation message
            var strMessage = string.Empty;
            var nSkippedRooms = m_spreadRoomsTable.Rows.Count - nNewRoomsSize;
            if (nSkippedRooms > 0)
                strMessage = string.Format("{0} unplaced {1} created successfully.\r\n{2} skipped, {3}",
                    nNewRoomsSize,
                    nNewRoomsSize > 1 ? "rooms were" : "room was",
                    nSkippedRooms + (nSkippedRooms > 1 ? " were" : " was"),
                    nSkippedRooms > 1
                        ? "because they were already mapped by Revit rooms."
                        : "because it was already mapped by Revit rooms.");
            else
                strMessage = string.Format("{0} unplaced {1} created successfully.",
                    nNewRoomsSize,
                    nNewRoomsSize > 1 ? "rooms were" : "room was");

            // output creation message 
            MyMessageBox(strMessage, MessageBoxIcon.Information);
            return nNewRoomsSize;
        }

        /// <summary>
        ///     Some preparation and check before creating room.
        /// </summary>
        /// <param name="curPhase">Current phase used to create room, all rooms will be created in this phase.</param>
        /// <returns></returns>
        private bool RoomCreationPreparation(ref Phase curPhase)
        {
            // check to see whether there is available spread sheet based rooms to create
            if (null == m_spreadRoomsTable || null == m_spreadRoomsTable.Rows || m_spreadRoomsTable.Rows.Count == 0)
            {
                MyMessageBox("There is no available spread sheet based room to create.", MessageBoxIcon.Warning);
                return false;
            }

            // create shared parameter for "Room" category elements
            CreateMyRoomSharedParameter();

            // create Revit rooms by using spread sheet based rooms
            // add "ID" data of spread sheet to Room element's share parameter: "External Room ID"
            var column = m_spreadRoomsTable.Columns[RoomsData.RoomId];
            if (column == null)
            {
                MyMessageBox("Failed to get ID data of spread sheet rooms.", MessageBoxIcon.Warning);
                return false;
            }

            // get phase used to create room
            foreach (var phase in m_allPhases)
                if (string.Compare(phase.Name, phaseComboBox.Text) == 0)
                {
                    curPhase = phase;
                    break;
                }

            if (null == curPhase)
            {
                MyMessageBox("No available phase used to create room.", MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Update new room with values in spreadsheet, currently there are three columns need to be set.
        /// </summary>
        /// <param name="newRoom">New room to be updated.</param>
        /// <param name="index">The index of row in spreadsheet, use values of this row to update the new room.</param>
        private void UpdateNewRoom(Room newRoom, int row)
        {
            string[] constantColumns = { RoomsData.RoomName, RoomsData.RoomNumber, RoomsData.RoomComments };
            foreach (var constantColumn in constantColumns)
                if (m_spreadRoomsTable.Columns.IndexOf(constantColumn) != -1)
                {
                    // if value is not null or empty, set new rooms related parameter.
                    var colValue = m_spreadRoomsTable.Rows[row][constantColumn].ToString();
                    if (string.IsNullOrEmpty(colValue)) continue;

                    switch (constantColumn)
                    {
                        case RoomsData.RoomName:
                            newRoom.Name = colValue;
                            break;
                        case RoomsData.RoomNumber:
                            newRoom.Number = colValue;
                            break;
                        case RoomsData.RoomComments:
                            var commentParam = newRoom.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                            commentParam?.Set(colValue);
                            break;
                    }
                }
        }

        /// <summary>
        ///     Import room spread sheet and display them in form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importRoomButton_Click(object sender, EventArgs e)
        {
            using (var sfdlg = new OpenFileDialog())
            {
                // file dialog initialization 
                sfdlg.Title = "Import Excel File";
                sfdlg.Filter = "Excel File(*.xls)|*.xls";
                sfdlg.RestoreDirectory = true;
                //
                // initialize the default file name
                var hashCode = m_document.GetHashCode();
                var xlsAndTable = new SheetInfo(string.Empty, string.Empty);
                if (CrtlApplication.EventReactor.DocMappedSheetInfo(hashCode, ref xlsAndTable))
                    sfdlg.FileName = xlsAndTable.FileName;
                //
                // import the select
                if (DialogResult.OK == sfdlg.ShowDialog())
                    try
                    {
                        // create xls data source connector and retrieve data from it
                        m_dataBaseName = sfdlg.FileName;
                        var xlsCon = new XlsDbConnector(m_dataBaseName);

                        // bind table data to grid view and ComboBox control
                        tablesComboBox.DataSource = xlsCon.RetrieveAllTables();

                        // close the connection
                        xlsCon.Dispose();
                    }
                    catch (Exception ex)
                    {
                        tablesComboBox.DataSource = null;
                        MyMessageBox(ex.Message, MessageBoxIcon.Warning);
                    }
            }
        }

        /// <summary>
        ///     Select one table(work sheet) and display its data to DataGridView control.
        ///     after selection, generate data table from data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tablesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update spread sheet based rooms
            sheetDataGridView.DataSource = null;
            m_roomTableName = tablesComboBox.SelectedValue as string;
            XlsDbConnector xlsCon = null;
            try
            {
                m_spreadRoomsTable?.Clear();

                // get all rooms table then close this connection immediately
                xlsCon = new XlsDbConnector(m_dataBaseName);

                // generate room data table from room work sheet.
                m_spreadRoomsTable = xlsCon.GenDataTable(m_roomTableName);
                newRoomButton.Enabled = 0 == m_spreadRoomsTable.Rows.Count ? false : true;

                // close connection
                xlsCon.Dispose();

                // update data source of DataGridView
                sheetDataGridView.DataSource = new DataView(m_spreadRoomsTable);
            }
            catch (Exception ex)
            {
                // close connection and update data source
                xlsCon.Dispose();
                sheetDataGridView.DataSource = null;
                MyMessageBox(ex.Message, MessageBoxIcon.Warning);
                return;
            }

            // update the static s_DocMapDict variable when user changes the Excel and room table
            var hashCode = m_document.GetHashCode();
            if (CrtlApplication.EventReactor.DocMonitored(hashCode))
            {
                // update spread sheet to which document is being mapped.
                CrtlApplication.EventReactor.UpdateSheeInfo(hashCode, new SheetInfo(m_dataBaseName, m_roomTableName));

                // update current mapped room sheet information, only show this when Revit rooms were mapped to Excel sheet.
                UpdateRoomMapSheetInfo();
            }
        }

        /// <summary>
        ///     Filter rooms by specified level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the selected level, by comparing its name and ComboBox selected item's name
            Level selLevel = null;
            foreach (var level in m_allLevels)
                if (0 == string.Compare(level.Name, levelComboBox.Text))
                {
                    selLevel = level;
                    break;
                }

            if (selLevel == null)
            {
                MyMessageBox("There is no available level to get rooms.", MessageBoxIcon.Warning);
                return;
            }

            // update data source of DataGridView
            revitRoomDataGridView.DataSource = null;
            revitRoomDataGridView.DataSource = new DataView(m_roomData.GenRoomsDataTable(selLevel));
        }

        /// <summary>
        ///     Create new rooms according to spreadsheet based rooms data and specified phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newRoomButton_Click(object sender, EventArgs e)
        {
            // Create room process:
            // 1: Create shared parameter for "Room" category elements if it doesn't exist.
            // 2: Create rooms by using spread sheet's data: 
            //    a: We should make sure that each of spreadsheet room is mapped by only one Revit room; 
            //       if not, many Revit rooms map to one spreadsheet room will confuse user;
            //    b: Set Name, Number and comment values of new rooms by spreadsheet relative data.
            // 3: Subscribe document Save, SaveAs and Close event handlers.
            // 4: Update all rooms data and pop up message

            // Create rooms now
            var nNewRoomsSize = RoomCreationStart();
            if (nNewRoomsSize <= 0) return;

            // Reserve this document by its hash code, this document will be updated when it's about to be saved.
            var hashCode = m_document.GetHashCode();
            if (!CrtlApplication.EventReactor.DocMonitored(hashCode))
            {
                // reserves this document and current .xls file and table.
                CrtlApplication.EventReactor.UpdateSheeInfo(hashCode, new SheetInfo(m_dataBaseName, m_roomTableName));

                // show current Excel and sheet name sample is mapped to, only show them after unplaced rooms were created.
                UpdateRoomMapSheetInfo();
            }

            // update Revit rooms data and display of controls.
            UpdateFormDisplay(true);
        }

        /// <summary>
        ///     Show all rooms in current document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAllRoomsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // disable and enable some controls
            levelComboBox.Enabled = !showAllRoomsCheckBox.Checked;

            // update room display, there is no new creation, so it's not necessary to retrieve all rooms
            UpdateFormDisplay(false);
        }

        /// <summary>
        ///     Close the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Clear all values of shared parameters
        ///     Allow user to create more unplaced rooms and update map relationships between Revit and spreadsheet rooms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearIDButton_Click(object sender, EventArgs e)
        {
            var nCount = 0;
            foreach (var room in m_roomData.Rooms)
            {
                Parameter param = null;
                var bExist = RoomsData.ShareParameterExists(room, RoomsData.SharedParam, ref param);
                if (bExist && null != param && false == string.IsNullOrEmpty(param.AsString()))
                {
                    param.Set(string.Empty);
                    nCount++;
                }
            }

            // update Revit rooms display
            if (nCount > 0) UpdateFormDisplay(false);
        }
    }
}
