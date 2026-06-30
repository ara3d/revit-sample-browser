// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
namespace Ara3D.RevitSampleBrowser.RoomSchedule.CS
{
    /// <summary>
    ///     One struct defines the content of mapped Excel spreadsheet: the full name of this file and the sheet.
    ///     Only the opened sheet is reserved by this struct.
    /// </summary>
    public class SheetInfo
    {
        private string m_fileName;

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

        public string FileName
        {
            get => m_fileName;
            set => m_fileName = value;
        }

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
        private readonly Dictionary<int, SheetInfo> m_docMapDict = new Dictionary<int, SheetInfo>();

        private readonly string m_logFile;

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

        public void DocumentSavingAs(object sender, DocumentSavingAsEventArgs e)
        {
            DumpLog($"Raised DocumentSavingAs -> Document: {Path.GetFileNameWithoutExtension(e.Document.Title)}");
            UpdateMappedSpreadsheet(e.Document);
        }

        public void DocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            DumpLog($"Raised DocumentSaving -> Document: {Path.GetFileNameWithoutExtension(e.Document.Title)}");
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

            var updatedRows = 0; // number of rows which were updated
            var newRows = 0; // number of rows which were added into spread sheet
            var dbConnector = new XlsDbConnector(mappedXlsAndTable.FileName);

            var stepNo = -1;
            DumpLog($"{Environment.NewLine}Start to update spreadsheet room......");
            foreach (var room in roomData.Rooms)
            {
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

                    var param = room.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    var comments = null != param ? param.AsString() : "";
                    if (string.IsNullOrEmpty(comments))
                    {
                        // this room doesn't have comment value
                        bCommnetIsNull = true;
                        // use <null> for room with empty comment by default when updating spread sheet
                        comments = "<null>";
                    }

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
            DumpLog($"Finish updating spreadsheet room.{Environment.NewLine}");
        }

        private static bool ValidateRevitRoom(Document activeDocument, Room room, ref double roomArea,
            ref string externalId)
        {
            roomArea = 0.0f;
            externalId = string.Empty;
            if (null == room.Location || null == activeDocument.GetElement(room.LevelId)) return false;

            // if the area is zero to less than zero, skip the update too
            try
            {
                var areaStr = SampleBrowserUtils.GetProperty(activeDocument, room, BuiltInParameter.ROOM_AREA, false);
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
            var bExist = ParameterAccess.ShareParameterExists(room, RoomsData.SharedParam, ref externalIdSharedParam);
            if (false == bExist || null == externalIdSharedParam)
                return false;
            externalId = externalIdSharedParam.AsString();
            return true;
        }

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

        private void DumpLog(string strLog)
        {
            // Create writer only when there is dump
            if (null == m_logWriter)
            {
                if (File.Exists(m_logFile)) File.Delete(m_logFile);
                m_logWriter = new StreamWriter(m_logFile)
                {
                    AutoFlush = true
                };
            }

            //
            // dump log now
            m_logWriter.WriteLine(strLog);
        }
    }
}
