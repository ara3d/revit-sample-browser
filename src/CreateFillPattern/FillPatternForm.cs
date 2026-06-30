// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.CreateFillPattern.CS
{
    public partial class PatternForm : Form
    {
        private readonly Document m_doc;

        private readonly UIDocument m_docUi;

        public PatternForm(ExternalCommandData commandData)
        {
            m_docUi = commandData.Application.ActiveUIDocument;
            m_doc = commandData.Application.ActiveUIDocument.Document;
            InitializeComponent();

            IniTreeView();
        }

        private List<T> GetAllElements<T>()
        {
            ElementClassFilter elementFilter = new(typeof(T));
            FilteredElementCollector collector = new(m_doc);
            collector = collector.WherePasses(elementFilter);
            return collector.Cast<T>().ToList();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void IniTreeView()
        {
            treeViewLinePattern.Nodes.Clear();
            TreeNode iniNode2 = new("LinePatterns");
            treeViewLinePattern.Nodes.Add(iniNode2);

            var lstLinePatterns = GetAllElements<LinePatternElement>();
            foreach (var t in lstLinePatterns)
            {
                TreeNode node = new(t.Name)
                {
                    Name = t.Id.ToString()
                };
                iniNode2.Nodes.Add(node);
            }

            treeViewFillPattern.Nodes.Clear();
            TreeNode iniNode1 = new("FillPatterns");
            treeViewFillPattern.Nodes.Add(iniNode1);

            var lstFillPatterns = GetAllElements<FillPatternElement>();
            for (var i = 0; i < lstFillPatterns.Count; i++)
            {
                TreeNode node = new(lstFillPatterns[i].Name)
                {
                    Name = i.ToString()
                };
                iniNode1.Nodes.Add(node);
            }
        }

        private FillPatternElement GetOrCreateFacePattern(string patternName)
        {
            var target = FillPatternTarget.Model;
            var fillPatternElement = FillPatternElement.GetFillPatternElementByName(m_doc, target, patternName);

            if (fillPatternElement == null)
            {
                FillPattern fillPattern = new(patternName, target,
                    FillPatternHostOrientation.ToView, 0.5, 0.5, 0.5);

                Transaction trans = new(m_doc);
                trans.Start("Create a fillpattern element");
                fillPatternElement = FillPatternElement.Create(m_doc, fillPattern);
                trans.Commit();
            }

            return fillPatternElement;
        }

        private FillPatternElement GetOrCreateComplexFacePattern(string patternName)
        {
            var target = FillPatternTarget.Model;
            var fillPatternElement = FillPatternElement.GetFillPatternElementByName(m_doc, target, patternName);

            if (fillPatternElement == null)
            {
                FillPattern fillPattern = new(patternName, target,
                    FillPatternHostOrientation.ToHost);

                // Add grids
                List<FillGrid> grids =
                [
                    //Horizontal lines.  
                    CreateGrid(new UV(0, 0.1), 0.5, 0, 0.55, 1.0, 0.1),
                    CreateGrid(new UV(0, 0.5), 0.5, 0, 0.55, 1.0, 0.1),
                    // Vertical lines.  
                    CreateGrid(new UV(0, 0.1), 0.55, Math.PI / 2, 0.5, 0.4, 0.6),
                    CreateGrid(new UV(1.0, 0.1), 0.55, Math.PI / 2, 0.5, 0.4, 0.6)
                ];

                fillPattern.SetFillGrids(grids);

                Transaction t = new(m_doc, "Create fill pattern");
                t.Start();
                fillPatternElement = FillPatternElement.Create(m_doc, fillPattern);

                t.Commit();
            }

            return fillPatternElement;
        }

        private FillGrid CreateGrid(UV origin, double offset, double angle,
            double shift, params double[] segments)
        {
            FillGrid fillGrid = new()
            {
                // The arguments: origin, offset (vertical distance between lines), 
                // angle, shift (delta between location of start point per line)
                // The last two arguments are the segments: e.g. 1.0 units on, 
                // 0.1 units off (units are Revit units (ft))
                Origin = origin,
                Offset = offset,
                Angle = angle,
                Shift = shift
            };
            List<double> segmentsList = [.. segments];

            fillGrid.SetSegments(segmentsList);

            return fillGrid;
        }

        private LinePatternElement CreateLinePatternElement(string patternName)
        {
            //Create list of segments which define the line pattern
            List<LinePatternSegment> lstSegments =
            [
                new LinePatternSegment(LinePatternSegmentType.Dot, 0.0),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02),
                new LinePatternSegment(LinePatternSegmentType.Dash, 0.03),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02)
            ];

            LinePattern linePattern = new(patternName);
            linePattern.SetSegments(lstSegments);

            Transaction trans = new(m_doc);
            trans.Start("Create a linepattern element");
            var linePatternElement = LinePatternElement.Create(m_doc, linePattern);
            trans.Commit();
            return linePatternElement;
        }

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
            Transaction trans = new(m_doc);
            trans.Start("Apply fillpattern to surface");
            targetMaterial.SurfaceForegroundPatternId = mySurfacePattern.Id;
            trans.Commit();
            Close();
        }

        private void buttonCreateLinePattern_Click(object sender, EventArgs e)
        {
            List<ElementId> lstGridTypeIds = [];
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
                SetParameter("End Segment Pattern", myLinePatternElement.Id, gridType);
            }

            Close();
        }

        private void SetParameter(string paramName, ElementId eid, Element elem)
        {
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name == paramName)
                {
                    Transaction trans = new(m_doc);
                    trans.Start("Set parameter value");
                    param.Set(eid);
                    trans.Commit();
                    break;
                }
            }
        }

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
            Transaction trans = new(m_doc);
            trans.Start("Apply fillpattern to surface");
            targetMaterial.SurfaceForegroundPatternId = lstPatterns[patternIndex].Id;
            trans.Commit();

            Close();
        }

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

            Transaction trans = new(m_doc);
            trans.Start("Apply fillpattern to cutting surface");
            targetMaterial.CutForegroundPatternId = lstPatterns[patternIndex].Id;
            trans.Commit();

            Close();
        }

        private void buttonApplyToGrids_Click(object sender, EventArgs e)
        {
            List<ElementId> lstGridTypeIds = [];
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
                SetParameter("End Segment Pattern", eid, gridType);
            }

            Close();
        }

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
            Transaction trans = new(m_doc);
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
