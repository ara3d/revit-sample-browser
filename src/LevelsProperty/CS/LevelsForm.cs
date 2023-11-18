// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.LevelsProperty.CS
{
    /// <summary>
    ///     form for new levels
    /// </summary>
    public partial class LevelsForm : Form
    {
        //Record changed item
        private int[] m_changedItemsFlag;

        private readonly ElementId[] m_deleteExistLevelIdValue;
        private int m_deleteExistLevelTotal;

        //Class Command's object reference
        private readonly Command m_objectReference;
        private int m_systemLevelChangedFlag;

        //Record system levels' total
        private int m_systemLevelsTotal;

        /// <summary>
        ///     form for new levels
        /// </summary>
        public LevelsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The constructor is used to initialize some object.
        /// </summary>
        /// <param name="opt">Used to get the Command class's object.</param>
        public LevelsForm(Command opt)
        {
            InitializeComponent();

            m_objectReference = opt;

            //Set control on UI
            LevelName = new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 142
            };

            LevelElevation = new DataGridViewTextBoxColumn
            {
                HeaderText = "Elevation",
                Width = 142
            };

            levelsDataGridView.Columns.AddRange(LevelName, LevelElevation);

            bindingSource1.DataSource = typeof(LevelsDataSource);
            //Must place below code on the code "dataGridView1.DataSource = bindingSource1"
            levelsDataGridView.AutoGenerateColumns = false;
            levelsDataGridView.DataSource = bindingSource1;
            LevelName.DataPropertyName = "Name";
            LevelElevation.DataPropertyName = "Elevation";

            //pass datum to BindingSource
            bindingSource1.DataSource = m_objectReference.SystemLevelsDatum;

            //Record system levels' total
            m_systemLevelsTotal = m_objectReference.SystemLevelsDatum.Count;

            //Record changed items
            m_changedItemsFlag = new int[m_systemLevelsTotal];

            //Record deleted items
            m_deleteExistLevelIdValue = new ElementId[m_systemLevelsTotal];
            m_deleteExistLevelTotal = 0;
        }

        /// <summary>
        ///     Used to a new item in the dataGridView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, EventArgs e)
        {
            string newLevelName;
            double newLevelElevation;

            //If it exists some Levels on Revit, 
            //the added item's Name and Elevation uses last a Level's Name and Elevation.
            //Otherwise it uses a default data.
            if (bindingSource1.Count > 0)
            {
                bindingSource1.MoveLast();
                var lastItem = bindingSource1.Current as LevelsDataSource;

                var lastLevelName = lastItem.Name;
                var lastLevelElevation = lastItem.Elevation;
                newLevelName = $"{lastLevelName}'";
                newLevelElevation = lastLevelElevation + Unit.CovertFromApi(m_objectReference.UnitTypeId, 10);
            }
            else
            {
                newLevelName = "Level" + " " + "1";
                newLevelElevation = 0;
            }

            var newLevel = new LevelsDataSource
            {
                Name = newLevelName,
                Elevation = newLevelElevation
            };

            bindingSource1.Add(newLevel);
        }

        /// <summary>
        ///     Used to delete a item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (1 == levelsDataGridView.RowCount)
            {
                TaskDialog.Show("Revit", "Deleting the only open view in the project is not allowed.");
                return;
            }

            if (bindingSource1.Position > m_systemLevelsTotal - 1)
            {
                bindingSource1.RemoveCurrent();
                return;
            }

            if (bindingSource1.Position <= m_systemLevelsTotal - 1 && bindingSource1.Position >= 0)
            {
                var aRow = bindingSource1.Current as LevelsDataSource;
                m_deleteExistLevelIdValue[m_deleteExistLevelTotal] = aRow.LevelIdValue;
                m_deleteExistLevelTotal++;

                bindingSource1.RemoveCurrent();

                m_systemLevelsTotal--;

                var temChangedItemsFlag = new int[m_systemLevelsTotal];
                for (int i = 0, j = 0; i < m_systemLevelsTotal; i++, j++)
                {
                    if (bindingSource1.Position == i) j++;
                    temChangedItemsFlag[i] = m_changedItemsFlag[j];
                }

                m_changedItemsFlag = temChangedItemsFlag;

                return;
            }

            if (bindingSource1.Position < 0) TaskDialog.Show("Revit", "No have Level.");
        }

        /// <summary>
        ///     Judge if the inputted Name is unique.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (0 == levelsDataGridView.CurrentCell.ColumnIndex)
            {
                var newName = e.FormattedValue as string;

                var newNameArray = new char[newName.Length];
                newNameArray = newName.ToCharArray();
                for (var i = 0; i < newName.Length; ++i)
                    if ('\\' == newNameArray[i] || ':' == newNameArray[i] || '{' == newNameArray[i] ||
                        '}' == newNameArray[i] || '[' == newNameArray[i] || ']' == newNameArray[i] ||
                        '|' == newNameArray[i] || ';' == newNameArray[i] || '<' == newNameArray[i] ||
                        '>' == newNameArray[i] || '?' == newNameArray[i] || '`' == newNameArray[i] ||
                        '~' == newNameArray[i])
                    {
                        TaskDialog.Show("Revit", "Name cannot contain any of the following characters:\r\n\\ "
                                                 + ": { } [ ] | ; < > ? ` ~ \r\nor any of the non-printable characters.");

                        e.Cancel = true;

                        return;
                    }

                var oldName = levelsDataGridView.CurrentCell.FormattedValue as string;
                if (newName != oldName)
                    foreach (var systemLevelDatum in m_objectReference.SystemLevelsDatum)
                    {
                        if (systemLevelDatum.Name == newName)
                        {
                            TaskDialog.Show("Revit", "The name entered is already in use. Enter a unique name.");
                            e.Cancel = true;
                        }
                    }
            }
        }

        /// <summary>
        ///     Judge if the inputted Elevation is valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            TaskDialog.Show("Revit", e.Exception.Message);
        }

        /// <summary>
        ///     Record the changed Item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (bindingSource1.Position < m_systemLevelsTotal)
            {
                m_systemLevelChangedFlag = 1;
                m_changedItemsFlag[bindingSource1.Position] = 1;
            }
        }

        /// <summary>
        ///     Used to make setting apply to the model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            //Delete existed Levels
            for (var i = 0; i < m_deleteExistLevelTotal; i++)
                m_objectReference.DeleteLevel(m_deleteExistLevelIdValue[i]);

            var tempLevels = new List<LevelsDataSource>();

            //Set all changed Levels' name and elevation
            if (1 == m_systemLevelChangedFlag)
                for (var i = 0; i < m_changedItemsFlag.LongLength; i++)
                    if (1 == m_changedItemsFlag[i])
                    {
                        bindingSource1.Position = i;
                        var changeItem = bindingSource1.Current as LevelsDataSource;

                        if (false == m_objectReference.SetLevel(changeItem.LevelIdValue, changeItem.Name,
                                changeItem.Elevation))
                        {
                            changeItem.Name = $"TempName{changeItem.Name}";
                            tempLevels.Add(changeItem);
                            m_objectReference.SetLevel(changeItem.LevelIdValue, changeItem.Name, changeItem.Elevation);
                        }
                    }

            foreach (var item in tempLevels)
            {
                item.Name = item.Name.Remove(0, 8); // Remove the "TempName" string
                m_objectReference.SetLevel(item.LevelIdValue, item.Name, item.Elevation);
            }

            //Create new Levels
            for (var i = m_systemLevelsTotal; i < bindingSource1.Count; i++)
            {
                bindingSource1.Position = i;
                var newItem = bindingSource1.Current as LevelsDataSource;
                m_objectReference.CreateLevel(newItem.Name, newItem.Elevation);
            }
        }
    }
}
