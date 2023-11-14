// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.CreateFillPattern.CS
{
    /// <summary>
    /// </summary>
    public partial class PatternForm : Form
    {
        /// <summary>
        ///     Current revit document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     Current UI document
        /// </summary>
        private readonly UIDocument m_docUi;

        /// <summary>
        ///     Constructor
        /// </summary>
        public PatternForm(ExternalCommandData commandData)
        {
            m_docUi = commandData.Application.ActiveUIDocument;
            m_doc = commandData.Application.ActiveUIDocument.Document;
            InitializeComponent();

            IniTreeView();
        }

        /// <summary>
        ///     Select all elements in current document with specified element type filter
        /// </summary>
        /// <typeparam name="T">Target element type</typeparam>
        /// <returns>List of the elements</returns>
        private List<T> GetAllElements<T>()
        {
            var elementFilter = new ElementClassFilter(typeof(T));
            var collector = new FilteredElementCollector(m_doc);
            collector = collector.WherePasses(elementFilter);
            return collector.Cast<T>().ToList();
        }

        /// <summary>
        ///     Close the window after close button is clicked
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Initial nodes of treeviews
        /// </summary>
        private void IniTreeView()
        {
            treeViewLinePattern.Nodes.Clear();
            var iniNode2 = new TreeNode("LinePatterns");
            treeViewLinePattern.Nodes.Add(iniNode2);

            var lstLinePatterns = GetAllElements<LinePatternElement>();
            foreach (var t in lstLinePatterns)
            {
                var node = new TreeNode(t.Name)
                {
                    Name = t.Id.ToString()
                };
                iniNode2.Nodes.Add(node);
            }

            treeViewFillPattern.Nodes.Clear();
            var iniNode1 = new TreeNode("FillPatterns");
            treeViewFillPattern.Nodes.Add(iniNode1);

            var lstFillPatterns = GetAllElements<FillPatternElement>();
            for (var i = 0; i < lstFillPatterns.Count; i++)
            {
                var node = new TreeNode(lstFillPatterns[i].Name)
                {
                    Name = i.ToString()
                };
                iniNode1.Nodes.Add(node);
            }
        }

        /// <summary>
        ///     Create a fillpattern element
        /// </summary>
        /// <param name="patternName">The fillpattern name</param>
        /// <returns>Created fillpattern element</returns>
        private FillPatternElement GetOrCreateFacePattern(string patternName)
        {
            var target = FillPatternTarget.Model;
            var fillPatternElement = FillPatternElement.GetFillPatternElementByName(m_doc, target, patternName);

            if (fillPatternElement == null)
            {
                //Create a fillpattern with specified angle and spacing
                var fillPattern = new FillPattern(patternName, target,
                    FillPatternHostOrientation.ToView, 0.5, 0.5, 0.5);

                var trans = new Transaction(m_doc);
                trans.Start("Create a fillpattern element");
                fillPatternElement = FillPatternElement.Create(m_doc, fillPattern);
                trans.Commit();
            }

            return fillPatternElement;
        }

        /// <summary>
        ///     Create a complex fillpattern element
        /// </summary>
        /// <param name="patternName">The fillpattern name</param>
        /// <returns>Created fillpattern element</returns>
        private FillPatternElement GetOrCreateComplexFacePattern(string patternName)
        {
            var target = FillPatternTarget.Model;
            var fillPatternElement = FillPatternElement.GetFillPatternElementByName(m_doc, target, patternName);

            if (fillPatternElement == null)
            {
                // Create the fill pattern
                var fillPattern = new FillPattern(patternName, target,
                    FillPatternHostOrientation.ToHost);

                // Add grids
                var grids = new List<FillGrid>
                {
                    //Horizontal lines.  
                    CreateGrid(new UV(0, 0.1), 0.5, 0, 0.55, 1.0, 0.1),
                    CreateGrid(new UV(0, 0.5), 0.5, 0, 0.55, 1.0, 0.1),
                    // Vertical lines.  
                    CreateGrid(new UV(0, 0.1), 0.55, Math.PI / 2, 0.5, 0.4, 0.6),
                    CreateGrid(new UV(1.0, 0.1), 0.55, Math.PI / 2, 0.5, 0.4, 0.6)
                };

                fillPattern.SetFillGrids(grids);

                // Create the fill pattern element. Now document is modified; transaction is needed
                var t = new Transaction(m_doc, "Create fill pattern");
                t.Start();
                fillPatternElement = FillPatternElement.Create(m_doc, fillPattern);

                t.Commit();
            }

            return fillPatternElement;
        }

        /// <summary>
        ///     Create a fillgrid
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="offset"></param>
        /// <param name="angle"></param>
        /// <param name="shift"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        private FillGrid CreateGrid(UV origin, double offset, double angle,
            double shift, params double[] segments)
        {
            var fillGrid = new FillGrid();
            // The arguments: origin, offset (vertical distance between lines), 
            // angle, shift (delta between location of start point per line)
            // The last two arguments are the segments: e.g. 1.0 units on, 
            // 0.1 units off (units are Revit units (ft))
            fillGrid.Origin = origin;
            fillGrid.Offset = offset;
            fillGrid.Angle = angle;
            fillGrid.Shift = shift;
            var segmentsList = new List<double>();
            foreach (var d in segments) segmentsList.Add(d);
            fillGrid.SetSegments(segmentsList);

            return fillGrid;
        }

        /// <summary>
        ///     Create a linepattern element
        /// </summary>
        /// <param name="patternName">The linepattern name</param>
        /// <returns>Created linepattern element</returns>
        private LinePatternElement CreateLinePatternElement(string patternName)
        {
            //Create list of segments which define the line pattern
            var lstSegments = new List<LinePatternSegment>
            {
                new LinePatternSegment(LinePatternSegmentType.Dot, 0.0),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02),
                new LinePatternSegment(LinePatternSegmentType.Dash, 0.03),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02)
            };

            var linePattern = new LinePattern(patternName);
            linePattern.SetSegments(lstSegments);

            var trans = new Transaction(m_doc);
            trans.Start("Create a linepattern element");
            var linePatternElement = LinePatternElement.Create(m_doc, linePattern);
            trans.Commit();
            return linePatternElement;
        }

        /// <summary>
        ///     Create a fillpattern element and apply it to target material
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonCreateFillPattern_Click(object sender, EventArgs e)
        {
            var targetWall = GetSelectedWall();
            if (targetWall == null)
            {
                TaskDialog.Show("Create Fill Pattern",
                    "Before applying a FillPattern to a wall's surfaces, you must first select a wall.");
                Close();
                return;
            }

            var mySurfacePattern = GetOrCreateFacePattern("MySurfacePattern");
            var targetMaterial = m_doc.GetElement(targetWall.GetMaterialIds(false).First()) as Material;
            var trans = new Transaction(m_doc);
            trans.Start("Apply fillpattern to surface");
            targetMaterial.SurfaceForegroundPatternId = mySurfacePattern.Id;
            trans.Commit();
            Close();
        }

        /// <summary>
        ///     Create a linepattern element and apply it to grids
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonCreateLinePattern_Click(object sender, EventArgs e)
        {
            var lstGridTypeIds = new List<ElementId>();
            GetSelectedGridTypeIds(lstGridTypeIds);
            if (lstGridTypeIds.Count == 0)
            {
                TaskDialog.Show("Apply To Grids",
                    "Before applying a LinePattern to Grids, you must first select at least one grid.");
                Close();
                return;
            }

            var myLinePatternElement = CreateLinePatternElement("MyLinePattern");
            foreach (var typeId in lstGridTypeIds)
            {
                var gridType = m_doc.GetElement(typeId);
                //set the parameter value of End Segment Pattern
                SetParameter("End Segment Pattern", myLinePatternElement.Id, gridType);
            }

            Close();
        }

        /// <summary>
        ///     Set a parameter value of a target element
        /// </summary>
        /// <param name="paramName">The parameter name</param>
        /// <param name="eid">Id value of the parameter</param>
        /// <param name="elem">Target element</param>
        private void SetParameter(string paramName, ElementId eid, Element elem)
        {
            foreach (Parameter param in elem.Parameters)
                if (param.Definition.Name == paramName)
                {
                    var trans = new Transaction(m_doc);
                    trans.Start("Set parameter value");
                    param.Set(eid);
                    trans.Commit();
                    break;
                }
        }

        /// <summary>
        ///     Apply fillpattern to surface
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonApplyToSurface_Click(object sender, EventArgs e)
        {
            var targetWall = GetSelectedWall();
            if (targetWall == null)
            {
                TaskDialog.Show("Apply To Surface",
                    "Before applying a FillPattern to a wall's surfaces, you must first select a wall.");
                Close();
                return;
            }

            if (treeViewFillPattern.SelectedNode == null || treeViewFillPattern.SelectedNode.Parent == null)
            {
                TaskDialog.Show("Apply To Surface",
                    "Before applying a FillPattern to a wall's surfaces, you must first select one FillPattern.");
                return;
            }

            var lstPatterns = GetAllElements<FillPatternElement>();
            var patternIndex = int.Parse(treeViewFillPattern.SelectedNode.Name);
            var targetMaterial = m_doc.GetElement(targetWall.GetMaterialIds(false).First()) as Material;
            var trans = new Transaction(m_doc);
            trans.Start("Apply fillpattern to surface");
            targetMaterial.SurfaceForegroundPatternId = lstPatterns[patternIndex].Id;
            trans.Commit();

            Close();
        }

        /// <summary>
        ///     Get a selected wall
        /// </summary>
        /// <returns>Selected wall</returns>
        private Wall GetSelectedWall()
        {
            Wall wall = null;
            foreach (var elemId in m_docUi.Selection.GetElementIds())
            {
                var elem = m_doc.GetElement(elemId);
                wall = elem as Wall;
                if (wall != null)
                    return wall;
            }

            return wall;
        }

        /// <summary>
        ///     Apply fillpatterns to cutting surface
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonApplyToCutSurface_Click(object sender, EventArgs e)
        {
            var targetWall = GetSelectedWall();
            if (targetWall == null)
            {
                TaskDialog.Show("Apply To CutSurface",
                    "Before applying a FillPattern to a wall's cut surfaces, you must first select a wall.");
                Close();
                return;
            }

            if (treeViewFillPattern.SelectedNode == null || treeViewFillPattern.SelectedNode.Parent == null)
            {
                TaskDialog.Show("Apply To CutSurface",
                    "Before applying a FillPattern to a wall's cutting surfaces, you must first select a FillPattern.");
                return;
            }

            var lstPatterns = GetAllElements<FillPatternElement>();
            var patternIndex = int.Parse(treeViewFillPattern.SelectedNode.Name);
            var targetMaterial = m_doc.GetElement(targetWall.GetMaterialIds(false).First()) as Material;

            var trans = new Transaction(m_doc);
            trans.Start("Apply fillpattern to cutting surface");
            targetMaterial.CutForegroundPatternId = lstPatterns[patternIndex].Id;
            trans.Commit();

            Close();
        }

        /// <summary>
        ///     Apply linepattern to grids
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonApplyToGrids_Click(object sender, EventArgs e)
        {
            var lstGridTypeIds = new List<ElementId>();
            GetSelectedGridTypeIds(lstGridTypeIds);
            if (lstGridTypeIds.Count == 0)
            {
                TaskDialog.Show("Apply To Grids",
                    "Before applying a LinePattern to Grids, you must first select at least one grid.");
                Close();
                return;
            }

            if (treeViewLinePattern.SelectedNode == null || treeViewLinePattern.Parent == null)
            {
                TaskDialog.Show("Apply To Grids",
                    "Before applying a LinePattern to Grids, you must first select a LinePattern.");
                return;
            }

            var eid = ElementId.Parse(treeViewLinePattern.SelectedNode.Name);
            foreach (var typeId in lstGridTypeIds)
            {
                var gridType = m_doc.GetElement(typeId);
                //set the parameter value of End Segment Pattern
                SetParameter("End Segment Pattern", eid, gridType);
            }

            Close();
        }

        /// <summary>
        ///     Get selected GridType Ids
        /// </summary>
        /// <param name="lstGridTypeIds">Selected GridType Ids</param>
        private void GetSelectedGridTypeIds(List<ElementId> lstGridTypeIds)
        {
            foreach (var elemId in m_docUi.Selection.GetElementIds())
            {
                var elem = m_doc.GetElement(elemId);
                if (elem is Grid grid)
                {
                    var gridTypeId = grid.GetTypeId();
                    if (!lstGridTypeIds.Contains(gridTypeId))
                        lstGridTypeIds.Add(gridTypeId);
                }
            }
        }

        /// <summary>
        ///     To create a brick-like patternss
        /// </summary>
        /// <param name="sender">This object</param>
        /// <param name="e">A object contains the event data</param>
        private void buttonCreateComplexFillPattern_Click(object sender, EventArgs e)
        {
            var targetWall = GetSelectedWall();
            if (targetWall == null)
            {
                TaskDialog.Show("Create Fill Pattern",
                    "Before applying a FillPattern to a wall's surfaces, you must first select a wall.");
                Close();
                return;
            }

            var mySurfacePattern = GetOrCreateComplexFacePattern("MyComplexPattern");
            var targetMaterial = m_doc.GetElement(targetWall.GetMaterialIds(false).First()) as Material;
            var trans = new Transaction(m_doc);
            trans.Start("Apply complex fillpattern to surface");
            targetMaterial.SurfaceForegroundPatternId = mySurfacePattern.Id;
            trans.Commit();
            Close();
        }

        private void PatternForm_Load(object sender, EventArgs e)
        {
        }
    }
}
