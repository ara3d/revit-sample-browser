// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;

namespace RevitMultiSample.RoomSchedule
{
    /// <summary>
    ///     One struct defines the content of mapped Excel spreadsheet: the full name of this file and the sheet.
    ///     Only the opened sheet is reserved by this struct.
    /// </summary>
    public class SheetInfo
    {
        /// <summary>
        ///     Excel file name, it's full path
        /// </summary>
        private string m_fileName;

        /// <summary>
        ///     Sheet table within excel file, it's opened by sample
        /// </summary>
        private string m_sheetName;

        /// <summary>
        ///     Ctor method
        /// </summary>
        /// <param name="fileName">Full path name file.</param>
        /// <param name="sheetName">The sheet name of spreadsheet which was opened.</param>
        public SheetInfo(string fileName, string sheetName)
        {
            m_fileName = fileName;
            m_sheetName = sheetName;
        }

        /// <summary>
        ///     Get or set the full name of spreadsheet file
        /// </summary>
        public string FileName
        {
            get => m_fileName;
            set => m_fileName = value;
        }

        /// <summary>
        ///     Get or set the sheet name within the spreadsheet, the sheet name was mapped by Revit rooms.
        /// </summary>
        public string SheetName
        {
            get => m_sheetName;
            set => m_sheetName = value;
        }
    }

    /// <summary>
    ///     Class consists of delegate methods of DocumentSaving/SavingAs and DocumentClosing events.
    ///     These delegates will be raised once document is about to be saved or closed.
    ///     But, delegate will update mapped spreadsheet only when user created rooms for current document.
    ///     (That's, user clicks the button "Create Unplaced Rooms" and new rooms was created successfully).
    ///     Otherwise, these events handler methods won't do any update even if they were raised.
    /// </summary>
    public sealed class EventsReactor : IDisposable
    {
        /// <summary>
        ///     Array of documents' hash code and mapped Excel file and opened table.
        ///     The mapped excel and its table will be updated when events DocumentSave/SaveAs are raised.
        ///     The update occurs only when new room was created according to excel spreadsheet.
        /// </summary>
        private readonly Dictionary<int, SheetInfo> m_docMapDict = new Dictionary<int, SheetInfo>();

        /// <summary>
        ///     Specified log file name
        /// </summary>
        private readonly string m_logFile;

        /// <summary>
        ///     Logging writer used to write logging to log specified log file.
        ///     It's not recommended to access m_logWriter and call it's method, because maybe it's not initialized yet.
        ///     Please call DumpLog to dump related logging
        /// </summary>
        private StreamWriter m_logWriter;

        /// <summary>
        ///     This class will dump information to log file to tell user what happened
        /// </summary>
        /// <param name="logFile"></param>
        public EventsReactor(string logFile)
        {
            m_logFile = logFile;
        }

        /// <summary>
        ///     Release the file handling
        /// </summary>
        public void Dispose()
        {
            if (null != m_logWriter)
            {
                // close the stream
                m_logWriter.Flush();
                m_logWriter.Close();
                m_logWriter = null;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     Finalizer, we need to ensure the file stream was closed
        ///     This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~EventsReactor()
        {
            Dispose();
        }

        /// <summary>
        ///     Delegate for document save as event, it will update spreadsheet if document was mapped to spreadsheet.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">EventArgs of this event.</param>
        public void DocumentSavingAs(object sender, DocumentSavingAsEventArgs e)
        {
            DumpLog("Raised DocumentSavingAs -> Document: " + Path.GetFileNameWithoutExtension(e.Document.Title));
            UpdateMappedSpreadsheet(e.Document);
        }

        /// <summary>
        ///     Delegate for document save event, it will update spreadsheet if document was mapped to spreadsheet.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">EventArgs of this event.</param>
        public void DocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            DumpLog("Raised DocumentSaving -> Document: " + Path.GetFileNameWithoutExtension(e.Document.Title));
            UpdateMappedSpreadsheet(e.Document);
        }

        /// <summary>
        ///     Removed the document which was closed, event reactor doesn't need to monitor this document any more.
        ///     DocumentId is designed to identify one document, it's equal to hash code of this document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            DumpLog("Raised DocumentClosed.");
            m_docMapDict.Remove(e.DocumentId);
        }

        /// <summary>
        ///     Check if document is monitored by this event reactor
        /// </summary>
        /// <param name="docHashcode">Hashcode of document.</param>
        /// <returns></returns>
        public bool DocMonitored(int docHashcode)
        {
            return m_docMapDict.ContainsKey(docHashcode);
        }

        /// <summary>
        ///     Get the sheet information of document.
        /// </summary>
        /// <param name="hashCode">The hash code of document.</param>
        /// <param name="sheetInfo">The mapped spread file and sheet information.</param>
        /// <returns>
        ///     Indicates whether find the spread sheet mapped by this document.
        ///     True if mapped spreadsheet information found, else false.
        /// </returns>
        public bool DocMappedSheetInfo(int hashCode, ref SheetInfo sheetInfo)
        {
            return DocMonitored(hashCode) && m_docMapDict.TryGetValue(hashCode, out sheetInfo);
        }

        /// <summary>
        ///     Update or reset the sheet information to which document is being mapped.
        /// </summary>
        /// <param name="hashCode">Hash code of document used as key to find mapped spreadsheet.</param>
        /// <param name="newSheetInfo">New value for spreadsheet.</param>
        public void UpdateSheeInfo(int hashCode, SheetInfo newSheetInfo)
        {
            if (!DocMonitored(hashCode))
            {
                m_docMapDict.Add(hashCode, newSheetInfo);
            }
            else
            {
                m_docMapDict.Remove(hashCode);
                m_docMapDict.Add(hashCode, newSheetInfo);
            }
        }

        /// <summary>
        ///     Update mapped spread sheet when document is about to be saved or saved as
        ///     This method will update spread sheet room data([Area] column) with actual area value of mapped Revit Room.
        ///     or add Revit room to spreadsheet if it is not mapped to room of spreadsheet.        ///
        /// </summary>
        /// <param name="activeDocument">Current active document.</param>
        private void UpdateMappedSpreadsheet(Document activeDocument)
        {
            // Programming Routines:
            //
            // 1: Update spreadsheet when:
            //    a: there is room work sheet table;
            //    b: there is rooms data;
            //    c: shared parameter exists;
            // 2: Skip update and insert operations for below rooms:
            //    a: the rooms are not placed or located;
            //    b: the rooms whose shared parameter(defined by sample) are not retrieved,
            //       some rooms maybe don't have shared parameter at all, despite user create for Rooms category.
            // 3: Update spreadsheet rooms values by Revit room actual values.
            //    a: if shared parameter exists(is not null), update row by using this parameter's value;
            //    b: if shared parameter doesn't exist (is null), update row by Id value of room, which will avoid the duplicate 
            //       ID columns occur in spreadsheet.
            // 4: Insert Revit rooms data to spreadsheet if:
            //    a: failed to update values of rooms (maybe there no matched ID value in spread sheet rows).
            // 

            //
            // check which table to be updated.
            SheetInfo mappedXlsAndTable;
            var hasValue = m_docMapDict.TryGetValue(activeDocument.GetHashCode(), out mappedXlsAndTable);
            if (!hasValue || null == mappedXlsAndTable ||
                string.IsNullOrEmpty(mappedXlsAndTable.FileName) || string.IsNullOrEmpty(mappedXlsAndTable.SheetName))
            {
                DumpLog("This document isn't mapped to spreadsheet yet.");
                return;
            }

            // retrieve all rooms in project(maybe there are new rooms created manually by user)
            var roomData = new RoomsData(activeDocument);
            if (roomData.Rooms.Count <= 0)
            {
                DumpLog("This document doesn't have any room yet.");
                return;
            }

            // create a connection and update values of spread sheet
            var updatedRows = 0; // number of rows which were updated
            var newRows = 0; // number of rows which were added into spread sheet
            var dbConnector = new XlsDbConnector(mappedXlsAndTable.FileName);

            // check whether there is room table. 
            // get all available rooms in current document once more
            var stepNo = -1;
            DumpLog(Environment.NewLine + "Start to update spreadsheet room......");
            foreach (var room in roomData.Rooms)
            {
                // check Whether We Update This Room
                stepNo++;
                double roomArea = 0.0f;
                var externalId = string.Empty;
                if (!ValidateRevitRoom(activeDocument, room, ref roomArea, ref externalId))
                {
                    DumpLog($"#{stepNo}--> Room:{room.Number} was skipped.");
                    continue;
                }

                // try to update  
                try
                {
                    // flag used to indicate whether update is successful 
                    var bUpdateFailed = false; // reserve whether this room updated successfully.
                    // if room comment is empty, use <null> for mapped room, use <Added from Revit> for not mapped room in spread sheet.
                    var bCommnetIsNull = false;

                    // get comments of room
                    var param = room.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    var comments = null != param ? param.AsString() : "";
                    if (string.IsNullOrEmpty(comments))
                    {
                        // this room doesn't have comment value
                        bCommnetIsNull = true;
                        // use <null> for room with empty comment by default when updating spread sheet
                        comments = "<null>";
                    }

                    // create update SQL clause, 
                    // when filtering row to be updated, use Room.Id if "External Room ID" is null.
                    var updateStr =
                        $"Update [{mappedXlsAndTable.SheetName}$] SET [{RoomsData.RoomName}] = '{room.Name}', [{RoomsData.RoomNumber}] = '{room.Number}', [{RoomsData.RoomComments}] = '{comments}', [{RoomsData.RoomArea}] = '{roomArea:N3}' Where [{RoomsData.RoomId}] = {(string.IsNullOrEmpty(externalId) ? room.Id.ToString() : externalId)}";

                    // execute the command and check the size of updated rows 
                    var afftectedRows = dbConnector.ExecuteCommnand(updateStr);
                    if (afftectedRows == 0)
                    {
                        bUpdateFailed = true;
                    }
                    else
                    {
                        // count how many rows were updated
                        DumpLog($"#{stepNo}--> {updateStr}");
                        updatedRows += afftectedRows;

                        // if "External Room ID" is null but update successfully, which means:
                        // in spreadsheet there is existing row whose "ID" value equals to room.Id, so we should
                        // set Revit room's "External Room ID" value to Room.Id for consistence after update .
                        if (string.IsNullOrEmpty(externalId)) SetExternalRoomIdToRoomId(room);
                    }

                    // Add this new room to spread sheet if fail to update spreadsheet 
                    if (bUpdateFailed)
                    {
                        // try to insert this new room to spread sheet, some rules:
                        // a: if the "External Room ID" exists, set ID column to this external id value, 
                        //    if the "External Room ID" doesn't exist, use the actual Revit room id as the ID column value.
                        // b: use comments in room if room's description exists,
                        //    else, use constant string: "<Added from Revit>" for Comments column in spreadsheet.

                        var insertStr =
                            $"Insert Into [{mappedXlsAndTable.SheetName}$] ([{RoomsData.RoomId}], [{RoomsData.RoomComments}], [{RoomsData.RoomName}], [{RoomsData.RoomNumber}], [{RoomsData.RoomArea}]) Values('{(string.IsNullOrEmpty(externalId) ? room.Id.ToString() : externalId)}', '{(bCommnetIsNull || string.IsNullOrEmpty(comments) ? "<Added from Revit>" : comments)}', '{room.Name}', '{room.Number}', '{roomArea:N3}')";

                        // try to insert it 
                        afftectedRows = dbConnector.ExecuteCommnand(insertStr);
                        if (afftectedRows != 0)
                        {
                            // remember the number of new rows
                            var succeedMsg =
                                $"#{stepNo}--> Succeeded to insert spreadsheet Room - Name:{room.Name}, Number:{room.Number}, Area:{roomArea:N3}";
                            DumpLog(succeedMsg);
                            newRows += afftectedRows;

                            // if the Revit room doesn't have external id value(may be a room created manually)
                            // set its "External Room ID" value to Room.Id, because the room was added/mapped to spreadsheet, 
                            // and the value of ID column in sheet is just the Room.Id, we should keep this consistence.
                            if (string.IsNullOrEmpty(externalId)) SetExternalRoomIdToRoomId(room);
                        }
                        else
                        {
                            DumpLog($"#{stepNo}--> Failed: {insertStr}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // close the connection 
                    DumpLog($"#{stepNo}--> Exception: {ex.Message}");
                    dbConnector.Dispose();
                    RoomScheduleForm.MyMessageBox(ex.Message, MessageBoxIcon.Warning);
                    return;
                }
            }

            // close the connection 
            dbConnector.Dispose();

            // output the affected result message
            var sumMsg =
                $"{Path.GetFileName(mappedXlsAndTable.FileName)}:[{mappedXlsAndTable.SheetName}]: {updatedRows} rows were updated and {newRows} rows were added into successfully.";
            DumpLog(sumMsg);
            DumpLog("Finish updating spreadsheet room." + Environment.NewLine);
        }

        /// <summary>
        ///     Check to see if we need to update spreadsheet data according to this Revit room.
        ///     We don't need to update spreadsheet rooms if Revit room:
        ///     . Which is one unplaced room.
        ///     . The room has area which is zero.
        ///     . Special room which doesn't have custom shared parameter at all.
        /// </summary>
        /// <param name="activeDocument">Current active document.</param>
        /// <param name="roomObj">Room object to be checked.</param>
        /// <param name="roomArea">Room area of this Revit room.</param>
        /// <param name="externalId">The value of custom shared parameter of this room.</param>
        /// <returns>Indicates whether it succeeded to get room area and shared parameter value.</returns>
        private static bool ValidateRevitRoom(Document activeDocument, Room room, ref double roomArea,
            ref string externalId)
        {
            roomArea = 0.0f;
            externalId = string.Empty;
            if (null == room.Location || null == activeDocument.GetElement(room.LevelId)) return false;

            // get Area of room, if converting to double value fails, skip this. 
            // if the area is zero to less than zero, skip the update too
            try
            {
                // get area without unit, then converting it to double will be ok.
                var areaStr = RoomsData.GetProperty(activeDocument, room, BuiltInParameter.ROOM_AREA, false);
                roomArea = double.Parse(areaStr);
                if (roomArea <= double.Epsilon) return false;
            }
            catch
            {
                // parse double value failed, continue the loop 
                return false;
            }

            // get the shared parameter value of room
            Parameter externalIdSharedParam = null;
            var bExist = RoomsData.ShareParameterExists(room, RoomsData.SharedParam, ref externalIdSharedParam);
            if (false == bExist || null == externalIdSharedParam)
                return false;
            externalId = externalIdSharedParam.AsString();
            return true;
        }

        /// <summary>
        ///     Set shared parameter (whose name is "External Room ID") value to Room.Id
        /// </summary>
        /// <param name="room">The room used to get the room which to be updated</param>
        private static bool SetExternalRoomIdToRoomId(Room room)
        {
            try
            {
                var shareParam = room.LookupParameter(RoomsData.SharedParam);
                if (null != shareParam) return shareParam.Set(room.Id.ToString());
            }
            catch
            {
                // none
            }

            return false;
        }

        /// <summary>
        ///     Dump log file now
        /// </summary>
        private void DumpLog(string strLog)
        {
            // Create writer only when there is dump
            if (null == m_logWriter)
            {
                if (File.Exists(m_logFile)) File.Delete(m_logFile);
                m_logWriter = new StreamWriter(m_logFile);
                m_logWriter.AutoFlush = true;
            }

            //
            // dump log now
            m_logWriter.WriteLine(strLog);
        }
    }
}
