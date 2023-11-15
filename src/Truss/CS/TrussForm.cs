// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Truss.CS
{
    /// <summary>
    ///     window form contains one three picture box to show the
    ///     profile of truss geometry and profile and tabControl.
    ///     User can create truss, edit profile of truss and change type of truss members.
    /// </summary>
    public partial class TrussForm : Form
    {
        private const string NoAssociatedLevel = "<not associated>"; //when select truss doesn't have associate level
        private FamilyInstance m_column1; //one of 2 columns which truss build on
        private FamilyInstance m_column2; //one of 2 columns which truss build on
        private readonly UIDocument m_activeDocument; //active document in Revit
        private IEnumerable<FamilySymbol> m_beamTypes; //stores all the beam types (FamilySymbol)
        private bool m_bottomChord; //draw bottom chord when it's true, otherwise forbids to do it.
        private readonly ExternalCommandData m_commandData; //object which contains reference of Revit Application
        private FamilyInstance m_selecedtBeam; //store the selected beam
        private FamilySymbol m_selectedBeamType; //store the selected beam type
        private TrussType m_selectedTrussType; //selected truss type
        private ViewPlan m_selectedView; //store the selected view
        private int m_selectMemberIndex; //index of selected truss member
        private bool m_topChord; //allows to draw top chord when it's true, otherwise forbids to do it.
        private Autodesk.Revit.DB.Structure.Truss m_truss; //store the truss created by this sample
        private readonly ArrayList m_trussTypes; //stores all the truss types
        private IEnumerable<ViewPlan> m_views; //stores all the ViewPlan use to create truss
        private TrussGeometry m_trussGeometry; //TrussGeometry object store geometry info of Truss

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        public TrussForm(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_activeDocument = m_commandData.Application.ActiveUIDocument;
            InitializeComponent();
            m_trussTypes = new ArrayList();
            //Get user selection
            if (!GetSelectTrussOrColumns())
            {
                TaskDialog.Show("Select", "Please select 1 existing truss or 2 columns before load this application.");
                Close();
            }

            // get all the beam types, truss types and all the view plans from the active document
            DataInitialize();
        }

        /// <summary>
        ///     Get 1 truss or 2 columns from selection
        /// </summary>
        /// <returns>return false if selection incorrect</returns>
        private bool GetSelectTrussOrColumns()
        {
            if (m_activeDocument.Selection.GetElementIds().Count > 2 ||
                0 == m_activeDocument.Selection.GetElementIds().Count)
                return false;

            var es = new ElementSet();
            foreach (var elementId in m_activeDocument.Selection.GetElementIds())
                es.Insert(m_activeDocument.Document.GetElement(elementId));
            var iter = es.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
                switch (iter.Current)
                {
                    case Autodesk.Revit.DB.Structure.Truss truss when null == m_truss:
                        m_truss = truss;
                        break;
                    case Autodesk.Revit.DB.Structure.Truss truss:
                        return false;
                    case FamilyInstance familyInstance when StructuralType.Column == familyInstance.StructuralType:
                    {
                        if (null == m_column1)
                            m_column1 = familyInstance;
                        else
                            m_column2 = familyInstance;
                        break;
                    }
                    case FamilyInstance familyInstance:
                        return false;
                    default:
                        return false;
                }

            return null != m_truss || (null != m_column1 && null != m_column2);
        }

        /// <summary>
        ///     get all the beam types, truss types and all the view plans from the active document
        /// </summary>
        public void DataInitialize()
        {
            // get all the beam types
            GetBeamTypes();

            // get all the truss types
            // there's no truss type in the active document
            var filteredElementCollector = new FilteredElementCollector(m_activeDocument.Document);
            filteredElementCollector.OfClass(typeof(FamilySymbol));
            filteredElementCollector.OfCategory(BuiltInCategory.OST_Truss);
            IList<TrussType> trussTypes = filteredElementCollector.Cast<TrussType>().ToList();

            if (null == trussTypes || 0 == trussTypes.Count)
            {
                TaskDialog.Show("Load Truss Type", "Please load at least one truss type into your project.");
                Close();
            }

            foreach (var trussType in trussTypes)
            {
                if (null == trussType) continue;

                var trussTypeName = trussType.get_Parameter
                    (BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM).AsString();
                TrussTypeComboBox.Items.Add(trussTypeName);
                m_trussTypes.Add(trussType);
            }

            // get all the views
            // Skip view templates because they're behind-the-scene and invisible in project browser; also invalid for API..., etc.

            m_views = from elem in
                    new FilteredElementCollector(m_activeDocument.Document).OfClass(typeof(ViewPlan)).ToElements()
                let viewPlan = elem as ViewPlan
                where viewPlan != null && !viewPlan.IsTemplate
                select viewPlan;
            foreach (View view in m_views) ViewComboBox.Items.Add(view.Name);
        }

        /// <summary>
        ///     get all the beam types
        /// </summary>
        private void GetBeamTypes()
        {
            m_beamTypes = from elem in
                    new FilteredElementCollector(m_activeDocument.Document).OfClass(
                        typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_StructuralFraming)
                let type = elem as FamilySymbol
                select type;

            // can't obtain beam types from the active document
            if (null == m_beamTypes ||
                !m_beamTypes.Any())
            {
                TaskDialog.Show("Load Structural Framing Family",
                    "No Structural Framing Family loaded. Please load one of the Structure Framing Family.");
                Close();
            }

            foreach (var familySymbol in m_beamTypes)
            {
                var beamTypeName = familySymbol.get_Parameter(
                    BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM).AsString();
                BeamTypeComboBox.Items.Add(beamTypeName);
            }
        }

        /// <summary>
        ///     initialize the UI ComboBox's appearance
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TrussForm_Load(object sender, EventArgs e)
        {
            TrussTypeComboBox.SelectedIndex = 0;
            ViewComboBox.SelectedIndex = 0;
            //user pre-select a truss in Revit UI
            if (m_truss != null)
            {
                // show the truss type and level of pre-selected truss in the ComboBox
                var nameOfTrussType = m_truss.TrussType.get_Parameter
                    (BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM).AsString();
                TrussTypeComboBox.SelectedIndex = TrussTypeComboBox.Items.IndexOf(nameOfTrussType);
                var viewName = m_truss.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
                if (null == viewName || 0 == viewName.AsString().CompareTo(NoAssociatedLevel))
                {
                    ViewComboBox.Items.Add(NoAssociatedLevel);
                    ViewComboBox.SelectedIndex = ViewComboBox.Items.IndexOf(NoAssociatedLevel);
                }
                else
                {
                    var nameOfViewPlane = viewName.AsString().Substring(8);
                    ViewComboBox.SelectedIndex = ViewComboBox.Items.IndexOf(nameOfViewPlane);
                }

                TrussTypeComboBox.Enabled = false;
                CreateButton.Enabled = false;
                ViewComboBox.Enabled = false;
                m_trussGeometry = new TrussGeometry(m_truss, m_commandData);
                TrussGraphicsTabControl.SelectedIndex = 1;
            }
        }

        /// <summary>
        ///     get selected truss type, this data will be used in truss creation
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TrussTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_selectedTrussType = (TrussType)m_trussTypes[TrussTypeComboBox.SelectedIndex];
        }

        /// <summary>
        ///     create new truss
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void CreateButton_Click(object sender, EventArgs e)
        {
            try
            {
                var transaction = new Transaction(m_commandData.Application.ActiveUIDocument.Document, "CreateTruss");
                transaction.Start();
                // create the truss
                m_truss = CreateTruss();
                m_truss.Location.Move(new XYZ(0, 0, m_selectedView.GenLevel.Elevation));
                transaction.Commit();
                m_trussGeometry = new TrussGeometry(m_truss, m_commandData);
                TrussGraphicsTabControl.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Exception", ex.Message);
            }

            TrussTypeComboBox.Enabled = false;
            CreateButton.Enabled = false;
            ViewComboBox.Enabled = false;
        }

        /// <summary>
        ///     create truss in Revit
        /// </summary>
        /// <returns>new created truss</returns>
        public Autodesk.Revit.DB.Structure.Truss CreateTruss()
        {
            var document = m_commandData.Application.ActiveUIDocument.Document;
            //sketchPlane
            var origin = new XYZ(0, 0, 0);
            var xDirection = new XYZ(1, 0, 0);
            var yDirection = new XYZ(0, 1, 0);
            var plane = Plane.CreateByOriginAndBasis(xDirection, yDirection, origin);
            var sketchPlane = SketchPlane.Create(document, plane);
            //new base Line
            Curve frame1Curve = null;
            Curve frame2Curve = null;
            if (m_column1.Location is LocationCurve curve) frame1Curve = curve.Curve;
            if (m_column2.Location is LocationCurve locationCurve) frame2Curve = locationCurve.Curve;

            var centerPoint1 = (frame1Curve as Line).GetEndPoint(0);

            var centerPoint2 = (frame2Curve as Line).GetEndPoint(0);
            var startPoint = new XYZ(centerPoint1.X, centerPoint1.Y, 0);
            var endPoint = new XYZ(centerPoint2.X, centerPoint2.Y, 0);
            Line baseLine = null;

            try
            {
                baseLine = Line.CreateBound(startPoint, endPoint);
            }
            catch (ArgumentException)
            {
                TaskDialog.Show("Argument Exception", "Two column you selected are too close to create truss.");
            }

            return Autodesk.Revit.DB.Structure.Truss.Create(document, m_selectedTrussType.Id, sketchPlane.Id, baseLine);
        }

        /// <summary>
        ///     draw profile, top chord and bottom of truss
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ProfileEditPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            m_trussGeometry?.Draw2D(e.Graphics, Pens.Blue);

            var font = new Font("Verdana", 10, FontStyle.Regular);
            var indicator = "Draw Top Chord and Bottom Chord here:";
            e.Graphics.DrawString(indicator, font, Brushes.Blue, new PointF(20, 10));
        }

        /// <summary>
        ///     add point to top chord and bottom chord
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ProfileEditPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // draw top chord line
            if (m_topChord)
                m_trussGeometry.AddTopChordPoint(e.X, e.Y);
            // draw bottom chord line
            else if (m_bottomChord) m_trussGeometry.AddBottomChordPoint(e.X, e.Y);

            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     change move point of top chord lineTool and bottom chord lineTool
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ProfileEditPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_topChord)
                m_trussGeometry.AddTopChordMovePoint(e.X, e.Y);
            else if (m_bottomChord) m_trussGeometry.AddBottomChordMovePoint(e.X, e.Y);

            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     begin to draw top chord
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TopChordButton_Click(object sender, EventArgs e)
        {
            m_topChord = true;
            m_bottomChord = false;
            ProfileEditPictureBox.Cursor = Cursors.Cross;
            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     begin to draw bottom chord
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void BottomChordButton_Click(object sender, EventArgs e)
        {
            m_topChord = false;
            m_bottomChord = true;
            ProfileEditPictureBox.Cursor = Cursors.Cross;
            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     update the truss according to the top chord line and the bottom chord line
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var transaction = new Transaction(m_activeDocument.Document, "SetProfile");
            transaction.Start();
            //update the truss
            m_trussGeometry.SetProfile(m_commandData);
            transaction.Commit();
            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     restore profile of truss
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void RestoreButton_Click(object sender, EventArgs e)
        {
            var transaction = new Transaction(m_activeDocument.Document, "RemoveProfile");
            transaction.Start();
            // restore the profile
            m_trussGeometry.RemoveProfile();
            transaction.Commit();
            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     clear points of top and bottom chord line
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void CleanChordbutton_Click(object sender, EventArgs e)
        {
            m_trussGeometry.ClearChords();
            m_topChord = false;
            m_bottomChord = false;
            ProfileEditPictureBox.Cursor = Cursors.Default;
            ProfileEditPictureBox.Refresh();
        }

        /// <summary>
        ///     draw geometry of truss, and draw selected line red
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TrussGeometryPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            m_trussGeometry?.Draw2D(e.Graphics, Pens.Blue);

            var font = new Font("Verdana", 10, FontStyle.Regular);
            var indicator = "Select a beam from the truss:";
            e.Graphics.DrawString(indicator, font, Brushes.Blue, new PointF(20, 10));
        }

        /// <summary>
        ///     get selected truss member (Beam)
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TrussGeometryPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            // indicates whether the mouse moves over or hovers on one beam
            // if true, paints the beam to red
            m_selectMemberIndex = m_trussGeometry.SelectTrussMember(e.X, e.Y);
            TrussMembersPictureBox.Refresh();
        }

        /// <summary>
        ///     select truss member (beam)
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void TrussGeometryPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // clicks in the canvas but doesn't select anything
            if (-1 == m_selectMemberIndex)
            {
                BeamTypeComboBox.Enabled = false;
                ChangeBeamTypeButton.Enabled = false;
                return;
            }

            // clicks in the canvas and selects a beam
            m_selecedtBeam = m_trussGeometry.GetSelectedBeam(m_commandData);
            if (null != m_selecedtBeam)
            {
                var symbol = m_selecedtBeam.Symbol;
                // get the type of the selected beam
                var nameOfSymbol = symbol.get_Parameter(
                    BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM).AsString();
                var index = BeamTypeComboBox.Items.IndexOf(nameOfSymbol);
                // show the beam type in the ComboBox
                BeamTypeComboBox.SelectedIndex = index;
                BeamTypeComboBox.Enabled = true;
                ChangeBeamTypeButton.Enabled = true;
                TrussMembersPictureBox.Refresh();
            }
        }

        /// <summary>
        ///     change selected beam type
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void BeamTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // choose a new beam type for the selected beam
            m_selectedBeamType = m_beamTypes.ElementAt(BeamTypeComboBox.SelectedIndex);
        }

        /// <summary>
        ///     change type of selected beam
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ChangeBeamTypeButton_Click(object sender, EventArgs e)
        {
            // apply the beam type change
            var transaction = new Transaction(m_activeDocument.Document, "ChangeSelectedBeamType");
            transaction.Start();
            if (null != m_selecedtBeam) m_selecedtBeam.Symbol = m_selectedBeamType;
            transaction.Commit();
        }

        /// <summary>
        ///     change selected ViewPlan
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // choose another view for the truss creation
            if (ViewComboBox.SelectedIndex < m_views.Count())
                m_selectedView = m_views.ElementAt(ViewComboBox.SelectedIndex);
        }

        /// <summary>
        ///     quit the "top chord" or "bottom chord" drawing operation
        ///     by pressing the "ESC" key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrussGraphicsTabControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            // the "Profile Edit" tab page is active 
            // and the user presses the "ESC" key (e.KeyChar == 27)
            if (TrussGraphicsTabControl.SelectedTab == ProfileEditTabPage &&
                e.KeyChar == (char)27)
            {
                // quit the "chord" drawing and remove the move indicating line
                m_topChord = false;
                m_bottomChord = false;
                ProfileEditPictureBox.Cursor = Cursors.Default;
                m_trussGeometry.ClearMovePoint();
                ProfileEditPictureBox.Refresh();
            }
        }

        /// <summary>
        ///     won't let open truss member tab until truss create successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrussGraphicsTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // if no truss is created, locks the tab pages to the 1st tab page
            if (null == m_trussGeometry)
            {
                TrussGraphicsTabControl.SelectedIndex = 0;
            }
            else
            {
                m_trussGeometry.Reset();
                m_selecedtBeam = null;
            }
        }

        /// <summary>
        ///     Close dialogue box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
