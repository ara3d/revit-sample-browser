// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.TransactionControl.CS
{
    /// <summary>
    ///     A Form used to create a wall
    /// </summary>
    public partial class CreateWallForm : Form
    {
        /// <summary>
        ///     A reference to the external command data.
        /// </summary>
        private readonly ExternalCommandData m_commandData;

        /// <summary>
        ///     The created wall
        /// </summary>
        private Wall m_createdWall;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">the external command data</param>
        public CreateWallForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            m_commandData = commandData;
            Initialize();
        }

        /// <summary>
        ///     The created wall
        /// </summary>
        public Wall CreatedWall => m_createdWall;

        /// <summary>
        ///     do some initialization work: get all levels and wall types in active document
        ///     and add them to combo box
        /// </summary>
        private void Initialize()
        {
            //add levels to combo box levelsComboBox
            var document = m_commandData.Application.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(document);
            var filter = new ElementClassFilter(typeof(Level));
            var levels = collector.WherePasses(filter).ToElements();

            foreach (var element in levels)
            {
                if (element is Level level) levelsComboBox.Items.Add(level);
            }

            if (levelsComboBox.Items.Count > 0)
            {
                levelsComboBox.DisplayMember = "Name";
                levelsComboBox.SelectedIndex = 0;
            }

            //add wall types to combo box wallTypesComboBox
            var filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.OfClass(typeof(WallType));
            foreach (var element in filteredElementCollector)
            {
                if (!(element is WallType wallType)) continue;
                wallTypesComboBox.Items.Add(wallType);
            }

            if (wallTypesComboBox.Items.Count > 0)
            {
                wallTypesComboBox.DisplayMember = "Name";
                wallTypesComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///     try to create a wall. if failed, keep this dialog,
        ///     otherwise, close it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var document = m_commandData.Application.ActiveUIDocument.Document;
                double x, y, z;
                XYZ sPoint;

                //try to convert string to double, and return without closing dialog if failed
                if (double.TryParse(spXTextBox.Text, out x) &&
                    double.TryParse(spYTextBox.Text, out y) &&
                    double.TryParse(spZTextBox.Text, out z))
                {
                    sPoint = new XYZ(x, y, z);
                }
                else
                {
                    TaskDialog.Show("Revit", "Failed to get the start point.");
                    return;
                }

                //try to convert string to double, and return without closing dialog if failed
                XYZ ePoint;
                if (double.TryParse(epXTextBox.Text, out x) &&
                    double.TryParse(epYTextBox.Text, out y) &&
                    double.TryParse(epZTextBox.Text, out z))
                {
                    ePoint = new XYZ(x, y, z);
                }
                else
                {
                    TaskDialog.Show("Revit", "Failed to get the end point.");
                    return;
                }

                if (sPoint.IsAlmostEqualTo(ePoint))
                {
                    TaskDialog.Show("Revit", "The start point and end point cannot have the same location.");
                    return;
                }

                var line = Line.CreateBound(sPoint, ePoint);

                var level = levelsComboBox.SelectedItem as Level;
                var wallType = wallTypesComboBox.SelectedItem as WallType;

                //check whether parameters used to create wall are not null
                if (null == line)
                {
                    TaskDialog.Show("Revit", "Create line failed.");
                    return;
                }

                if (null == level || null == wallType)
                {
                    TaskDialog.Show("Revit", "Please select a level and a wall type.");
                    return;
                }

                m_createdWall = Wall.Create(document, line, wallType.Id, level.Id, 10, 0, true, true);
                document.Regenerate();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }
    }
}
