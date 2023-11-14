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
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Revit.SDK.Samples.RoomSchedule
{
    /// <summary>
    /// An integrated class to connect .xls data source, retrieve / update data
    /// </summary>
    class XlsDBConnector : IDisposable
    {
                // The connection created
        private OleDbConnection m_objConn;

        // One command for this connection
        private OleDbCommand m_command;

        // The connection string
        private string m_connectStr;

        // All available tables(work sheets) in xls data source
        private List<string> m_tables = new List<string>();
        

                /// <summary>
        /// Class constructor, to retrieve data from .xls data source 
        /// </summary>
        /// <param name="strXlsFile">The .xls file to be connected. 
        /// This file should exist and it can be writable.</param>
        public XlsDBConnector(string strXlsFile)
        {
            // Validate the specified
            if (!ValidateFile(strXlsFile)) {
                throw new ArgumentException("The specified file doesn't exists or has readonly attribute.", strXlsFile);
            }

            // establish a connection to the data source.
            m_connectStr = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = \"" + strXlsFile +
                "\"; Extended Properties = \"Excel 8.0;HDR=YES;\"";

            // create the .xls connection
            m_objConn = new OleDbConnection(m_connectStr);
            m_objConn.Open();
        }

        /// <summary>
        /// Close the OleDb connection
        /// </summary>
        public void Dispose()
        {
            if (null != m_objConn)
            {
                // close the OleDbConnection 
                m_objConn.Close();
                m_objConn = null;
                GC.SuppressFinalize(this);
            }
        }
        
        /// <summary>
        /// Finalizer, we need to ensure the connection was closed
        /// This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~XlsDBConnector()
        {
            Dispose();
        }
        

                /// <summary>
        /// Get all available table names from .xls data source
        /// </summary>
        public List<string> RetrieveAllTables()
        {
            // clear the old tables list firstly 
            m_tables.Clear();

            // get all table names from data source
            var schemaTable = m_objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                new object[] { null, null, null, "TABLE" });
            for (var i = 0; i < schemaTable.Rows.Count; i++)
            {
                m_tables.Add(schemaTable.Rows[i].ItemArray[2].ToString().TrimEnd('$'));
            }


            return m_tables;
        }


        /// <summary>
        /// Generate a DataTable data from xls data source, by a specified table name 
        /// </summary>
        /// <param name="tableName">Table name to be retrieved </param>
        /// <returns>The generated DataTable from work sheet</returns>
        public DataTable GenDataTable(string tableName)
        {
            // Get all data via command and then fill data to table
            var strCom = "Select * From [" + tableName + "$]";
            var myCommand = new OleDbDataAdapter(strCom, m_objConn);
            var myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "[" + tableName + "$]");

            try
            {
                // check to see whether the constant columns(defined in RoomsData class) exist in spread sheet.
                // These columns are necessary when updating spread sheet

                // define a flag variable to remember whether column is found
                // duplicate column is not allowed in spreadsheet
                var bHasColumn = new bool[5]; 
                Array.Clear(bHasColumn, 0, 5); // clear the variable to false

                // five constant columns which must exist and to be checked
                string[] constantNames = { RoomsData.RoomID, RoomsData.RoomName, 
                    RoomsData.RoomNumber, RoomsData.RoomArea, RoomsData.RoomComments };

                // remember all duplicate columns, used to pop up error message
                var duplicateColumns = string.Empty; 
                for (var i = 0; i < myDataSet.Tables[0].Columns.Count; i++)
                {
                    // get each column and check it
                    var columnName = myDataSet.Tables[0].Columns[i].ColumnName;

                    // check whether there are expected columns one by one
                    for (var col = 0; col < bHasColumn.Length; col++)
                    {
                        var bDupliate = CheckSameColName(columnName, constantNames[col]);
                        if (bDupliate)
                        {
                            if (false == bHasColumn[col])
                            {
                                bHasColumn[col] = true;
                            }
                            else
                            {
                                // this column is duplicate, reserve it
                                duplicateColumns += string.Format("[{0}], ", constantNames[col]); 
                            }
                        }
                    }
                }

                // check to see whether there are duplicate columns
                if (duplicateColumns.Length > 0)
                {
                    // duplicate columns are not allowed
                    var message = string.Format("There are duplicate column(s) in the spread sheet: {0}.", duplicateColumns);
                    throw new Exception(message);
                }


                // check whether all required columns are there.
                var missingColumns = string.Empty; // reserve all column names which are missing.
                for (var col = 0; col < bHasColumn.Length; col++)
                {
                    if (bHasColumn[col] == false)
                    {
                        missingColumns += string.Format("[{0}], ", constantNames[col]);
                    }
                }

                // check to see whether any required columns are missing.
                if (missingColumns.Length != 0)
                {
                    // some columns are missing, pop up these column names
                    var message = string.Format("Required columns are missing: {0}.", missingColumns);
                    throw new Exception(message);
                }

                // if no exception occurs, return the table of dataset directly
                return myDataSet.Tables[0];
            }
            catch (Exception ex)
            {
                // throw exception
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Execute SQL command, such as: update and insert
        /// </summary>
        /// <param name="strCmd">command to be executed</param>
        /// <returns>the number of rows affected by this command</returns>
        public int ExecuteCommnand(string strCmd)
        {
            try
            {
                if (null == m_command)
                {
                    m_command = m_objConn.CreateCommand();
                }
                m_command.CommandText = strCmd;
                return m_command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString() + strCmd);
            }
        }
        

                /// <summary>
        /// This method will validate and update attributes the specified file.
        /// The file should exist and it should have writable attribute.
        /// If it's readonly, this method will try to set the attribute to writable.
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        private bool ValidateFile(string strFile)
        {
            // exists check
            if(!File.Exists(strFile)) {
                return false;
            }
            //
            // writable attribute set
            File.SetAttributes(strFile, FileAttributes.Normal);
            return (FileAttributes.Normal == File.GetAttributes(strFile));
        }
        /// <summary>
        /// Check if two columns names are the same
        /// </summary>
        /// <param name="baseName">first name</param>
        /// <param name="compName">second name</param>
        /// <returns>true, the two names are same; false, they are different.</returns>
        private static bool CheckSameColName(string baseName, string compName)
        {
            if (string.Compare(baseName, compName) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
            };
}
