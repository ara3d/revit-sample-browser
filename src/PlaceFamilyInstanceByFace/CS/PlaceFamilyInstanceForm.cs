// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.PlaceFamilyInstanceByFace.CS
{
    /// <summary>
    ///     The main UI for creating family instance by face
    /// </summary>
    public partial class PlaceFamilyInstanceForm : Form
    {
        // the base type
        private readonly BasedType m_baseType = BasedType.Point;

        // the creator
        private readonly FamilyInstanceCreator m_creator;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="creator">the family instance creator</param>
        /// <param name="type">based-type</param>
        public PlaceFamilyInstanceForm(FamilyInstanceCreator creator, BasedType type)
        {
            m_creator = creator;
            m_creator.CheckFamilySymbol(type);
            m_baseType = type;
            InitializeComponent();

            // set the face name list and the default value
            foreach (var name in creator.FaceNameList) comboBoxFace.Items.Add(name);
            if (comboBoxFace.Items.Count > 0) SetFaceIndex(0);

            // set the family name list and the default value
            foreach (var symbolName in m_creator.FamilySymbolNameList) comboBoxFamily.Items.Add(symbolName);
            if (m_creator.DefaultFamilySymbolIndex < 0)
                comboBoxFamily.SelectedItem = m_creator.FamilySymbolNameList[0];
            else
                comboBoxFamily.SelectedItem =
                    m_creator.FamilySymbolNameList[m_creator.DefaultFamilySymbolIndex];

            // set UI display according to baseType
            switch (m_baseType)
            {
                case BasedType.Point:
                    Text = "Place Point-Based Family Instance";
                    labelFirst.Text = "Location :";
                    labelSecond.Text = "Direction :";
                    break;
                case BasedType.Line:
                    comboBoxFamily.SelectedItem = "Line-based";
                    Text = "Place Line-Based Family Instance";

                    labelFirst.Text = "Start Point :";
                    labelSecond.Text = "End Point :";
                    break;
            }

            AdjustComboBoxDropDownListWidth(comboBoxFace);
            AdjustComboBoxDropDownListWidth(comboBoxFamily);
        }

        /// <summary>
        ///     Get face information when the selected face is changed
        /// </summary>
        /// <param name="index">the index of the new selected face</param>
        private void SetFaceIndex(int index)
        {
            comboBoxFace.SelectedItem = m_creator.FaceNameList[index];

            var boundingBox = m_creator.GetFaceBoundingBox(index);
            var totle = boundingBox.Min + boundingBox.Max;
            switch (m_baseType)
            {
                case BasedType.Point:
                    PointControlFirst.SetPointData(totle / 2.0f);
                    PointControlSecond.SetPointData(new XYZ(1.0f, 0f, 0f));
                    break;
                case BasedType.Line:
                    PointControlFirst.SetPointData(boundingBox.Min);
                    PointControlSecond.SetPointData(boundingBox.Max);
                    break;
            }
        }

        /// <summary>
        ///     Create a family instance according the selected options by user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var retBool = false;
            try
            {
                var transaction = new Transaction(m_creator.RevitDoc.Document, "CreateFamilyInstance");
                transaction.Start();
                switch (m_baseType)
                {
                    case BasedType.Point:
                        retBool = m_creator.CreatePointFamilyInstance(PointControlFirst.GetPointData()
                            , PointControlSecond.GetPointData()
                            , comboBoxFace.SelectedIndex
                            , comboBoxFamily.SelectedIndex);
                        break;

                    case BasedType.Line:
                        retBool = m_creator.CreateLineFamilyInstance(PointControlFirst.GetPointData()
                            , PointControlSecond.GetPointData()
                            , comboBoxFace.SelectedIndex
                            , comboBoxFamily.SelectedIndex);
                        break;
                }

                transaction.Commit();
            }
            catch (ApplicationException)
            {
                TaskDialog.Show("Revit",
                    "Failed in creating family instance, maybe because the family symbol is wrong type, please check and choose again.");
                return;
            }
            catch (Exception ee)
            {
                TaskDialog.Show("Revit", ee.Message);
                return;
            }

            if (retBool)
            {
                Close();
                DialogResult = DialogResult.OK;
            }
            else
            {
                TaskDialog.Show("Revit", "The line is perpendicular to this face, please input the position again.");
            }
        }

        /// <summary>
        ///     Process the SelectedIndexChanged event of comboBoxFace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFaceIndex(comboBoxFace.SelectedIndex);
        }

        /// <summary>
        ///     Adjust the comboBox dropDownList width
        /// </summary>
        /// <param name="senderComboBox">the comboBox</param>
        private void AdjustComboBoxDropDownListWidth(ComboBox senderComboBox)
        {
            Graphics g = null;
            try
            {
                var width = senderComboBox.Width;
                g = senderComboBox.CreateGraphics();
                var font = senderComboBox.Font;

                // checks if a scrollbar will be displayed.
                // if yes, then get its width to adjust the size of the drop down list.
                var vertScrollBarWidth =
                    senderComboBox.Items.Count > senderComboBox.MaxDropDownItems
                        ? SystemInformation.VerticalScrollBarWidth
                        : 0;

                foreach (var s in senderComboBox.Items) //Loop through list items and check size of each items.
                    if (s != null)
                    {
                        var newWidth = (int)g.MeasureString(s.ToString().Trim(), font).Width
                                       + vertScrollBarWidth;
                        if (width < newWidth)
                            width = newWidth; //set the width of the drop down list to the width of the largest item.
                    }

                senderComboBox.DropDownWidth = width;
            }
            catch
            {
            }
            finally
            {
                g?.Dispose();
            }
        }
    }
}
