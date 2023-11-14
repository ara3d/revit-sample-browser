// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
    /// <summary>
    ///     This form is intent to fetch edges for hosted sweep creation or modification.
    ///     It contains a picture box for geometry preview and a tree view to list all the edges
    ///     which hosted sweep can be created on.
    ///     If the user mouse-over an edge where a hosted sweep can be created, the edge will be
    ///     highlighted in yellow. If user clicks on the highlighted edge, the edge will
    ///     be marked as selected in red color. Click it again to un-select, the color will turn back.
    ///     Edge selection from preview box will be reflected in edge list and vice versa.
    ///     The geometry displayed in the picture box can be rotated with left mouse or
    ///     arrow keys (up, down, left and right) and zoomed with right mouse.
    /// </summary>
    public partial class EdgeFetchForm : Form
    {
        /// <summary>
        ///     Active element displayed in the preview.
        /// </summary>
        private Element m_activeElem;

        /// <summary>
        ///     Flag to indicate whether or not we should cancel expand or collapse
        ///     the tree-node which contains children.
        /// </summary>
        private bool m_cancelExpandOrCollapse;

        /// <summary>
        ///     Move the Graphics origin to preview center and flip its y-Axis.
        /// </summary>
        private Matrix m_centerMatrix;

        /// <summary>
        ///     Contains all the data need to fetch edges.
        /// </summary>
        private readonly CreationData m_creationData;

        /// <summary>
        ///     Yield rotation and scale transformation for current geometry display.
        /// </summary>
        private readonly TrackBall m_trackBall;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public EdgeFetchForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Customize constructor.
        /// </summary>
        /// <param name="creationData"></param>
        public EdgeFetchForm(CreationData creationData)
            : this()
        {
            m_creationData = creationData;
            treeViewHost.StateImageList = imageListForCheckBox;
            m_trackBall = new TrackBall();
        }

        /// <summary>
        ///     Initialize the combo box data source with Autodesk.Revit.DB.
        ///     e.g. FasciaTypes, GutterTypes, SlabEdgeTypes, and so on.
        /// </summary>
        private void InitializeTypes()
        {
            var objects = new List<object>();
            object selected = null;
            foreach (var obj in m_creationData.Creator.AllTypes)
            {
                objects.Add(obj);
                if (m_creationData.Symbol != null)
                    if ((obj as ElementType).Id == m_creationData.Symbol.Id)
                        selected = obj;
            }

            comboBoxTypes.DataSource = objects;
            comboBoxTypes.DisplayMember = "Name";

            if (selected != null)
                comboBoxTypes.SelectedItem = selected;
        }

        /// <summary>
        ///     Initialize the TreeView: create a tree according to geometry edges
        ///     and set each node's check status to unchecked.
        /// </summary>
        private void InitializeTree()
        {
            var creator = m_creationData.Creator;

            var rootNode = new TreeNode
            {
                StateImageIndex = (int)CheckState.Unchecked
            };
            foreach (var pair in creator.SupportEdges)
            {
                var elem = pair.Key;
                var elemNode = new TreeNode("[Id:" + elem.Id + "] " + elem.Name)
                {
                    StateImageIndex = (int)CheckState.Unchecked
                };
                rootNode.Nodes.Add(elemNode);
                elemNode.Tag = elem;
                var i = 1;
                foreach (var edge in pair.Value)
                {
                    var edgeNode = new TreeNode("Edge " + i)
                    {
                        StateImageIndex = (int)CheckState.Unchecked,
                        Tag = edge
                    };
                    elemNode.Nodes.Add(edgeNode);
                    ++i;
                }
            }

            rootNode.Text = "Roofs";
            if (creator is SlabEdgeCreator) rootNode.Text = "Floors";
            treeViewHost.Nodes.Add(rootNode);
            treeViewHost.TopNode.Expand();
        }

        /// <summary>
        ///     Initialize element geometry.
        /// </summary>
        private void InitializeElementGeometry()
        {
            foreach (var elemGeom in m_creationData.Creator.ElemGeomDic.Values)
            {
                elemGeom.InitializeTransform(pictureBoxPreview.Width * 0.8, pictureBoxPreview.Height * 0.8);
                elemGeom.ResetEdgeStates();
            }
        }

        /// <summary>
        ///     Initialize tree check states according to edges which hosted sweep can be created on.
        /// </summary>
        private void InitializeTreeCheckStates()
        {
            if (m_creationData.EdgesForHostedSweep.Count == 0) return;

            // Initialize edge binding selection state
            foreach (var edge in m_creationData.EdgesForHostedSweep)
            foreach (var elemGeom in m_creationData.Creator.ElemGeomDic.Values)
                if (elemGeom.EdgeBindingDic.ContainsKey(edge))
                    elemGeom.EdgeBindingDic[edge].IsSelected = true;

            // Initialize tree node selection state
            // check on all the edges on which we created hostd sweeps
            var root = treeViewHost.Nodes[0];
            foreach (TreeNode elemNode in root.Nodes)
            foreach (TreeNode edgeNode in elemNode.Nodes)
            {
                var edge = edgeNode.Tag as Edge;
                if (m_creationData.EdgesForHostedSweep.IndexOf(edge) != -1)
                {
                    edgeNode.StateImageIndex = (int)CheckState.Checked;
                    UpdateParent(edgeNode);
                }
            }
        }

        /// <summary>
        ///     Initialize text properties of this form.
        /// </summary>
        private void InitializeText()
        {
            Text = "Pick edges for " + m_creationData.Creator.Name;
            label.Text = "Select a type for " + m_creationData.Creator.Name;
            groupBoxEdges.Text = "All edges for " + m_creationData.Creator.Name;
        }

        /// <summary>
        ///     Initialize something related to the geometry preview.
        /// </summary>
        private void InitializePreview()
        {
            m_centerMatrix = new Matrix(1, 0, 0, -1,
                pictureBoxPreview.Width / 2.0f, pictureBoxPreview.Height / 2.0f);
            KeyPreview = true;

            foreach (var elem in m_creationData.Creator.ElemGeomDic.Keys)
            {
                m_activeElem = elem;
                break;
            }
        }

        /// <summary>
        ///     Extract the checked edges in the whole tree to CreationData.EdgesForHostedSweep.
        /// </summary>
        private void ExtractCheckedEdgesAndSelectedSymbol()
        {
            m_creationData.EdgesForHostedSweep.Clear();
            var rootNode = treeViewHost.Nodes[0];
            foreach (TreeNode hostNode in rootNode.Nodes)
            foreach (TreeNode edgeNode in hostNode.Nodes)
                if (edgeNode.StateImageIndex == (int)CheckState.Checked)
                    m_creationData.EdgesForHostedSweep.Add(edgeNode.Tag as Edge);
            m_creationData.Symbol = comboBoxTypes.SelectedItem as ElementType;
        }

        /// <summary>
        ///     Update tree node check status, it will impact its children and parents' status.
        /// </summary>
        /// <param name="node">Tree node to update</param>
        /// <param name="state">CheckState value</param>
        private void UpdateNodeCheckStatus(TreeNode node, CheckState state)
        {
            node.StateImageIndex = (int)state;
            if (node.Tag != null && node.Tag is Edge edge && m_activeElem != null)
            {
                var elem = node.Parent.Tag as Element;
                var elemGeom = m_creationData.Creator.ElemGeomDic[elem];
                elemGeom.EdgeBindingDic[edge].IsSelected =
                    node.StateImageIndex == (int)CheckState.Checked;
            }

            UpdateChildren(node);
            UpdateParent(node);
        }

        /// <summary>
        ///     Recursively update tree children's status to match its parent status.
        /// </summary>
        /// <param name="node">Parent node whose children will be updated</param>
        private void UpdateChildren(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child.StateImageIndex != node.StateImageIndex)
                {
                    child.StateImageIndex = node.StateImageIndex;

                    if (m_activeElem != null && child.Tag != null && child.Tag is Edge edge)
                    {
                        var elem = child.Parent.Tag as Element;
                        var elemGeom = m_creationData.Creator.ElemGeomDic[elem];
                        elemGeom.EdgeBindingDic[edge].IsSelected =
                            child.StateImageIndex == (int)CheckState.Checked;
                    }
                }

                UpdateChildren(child);
            }
        }

        /// <summary>
        ///     Recursively update tree parent's status to match its children status.
        /// </summary>
        /// <param name="node">Child whose parents will be updated</param>
        private void UpdateParent(TreeNode node)
        {
            var parent = node.Parent;
            if (parent == null) return;
            foreach (TreeNode brother in parent.Nodes)
                if (brother.StateImageIndex != node.StateImageIndex)
                {
                    parent.StateImageIndex = (int)CheckState.Indeterminate;
                    UpdateParent(parent);
                    return;
                }

            parent.StateImageIndex = node.StateImageIndex;
            UpdateParent(parent);
        }

        /// <summary>
        ///     Switch geometry displayed in the preview according to the tree node.
        /// </summary>
        /// <param name="node">Tree node to active</param>
        private void ActiveNode(TreeNode node)
        {
            switch (node.Tag)
            {
                case null:
                    return;
                case Element tag:
                    m_activeElem = tag;
                    break;
                case Edge _:
                    m_activeElem = node.Parent.Tag as Element;
                    break;
            }
        }

        /// <summary>
        ///     Clear Highlighted status of all highlighted edges.
        /// </summary>
        private void ClearAllHighLight()
        {
            if (m_activeElem == null) return;
            var elemGeom = m_creationData.Creator.ElemGeomDic[m_activeElem];

            foreach (var edge in m_creationData.Creator.SupportEdges[m_activeElem])
                elemGeom.EdgeBindingDic[edge].IsHighLighted = false;
        }

        /// <summary>
        ///     Get the related tree node of an edit
        /// </summary>
        /// <param name="edge">Given edge to find its tree-node</param>
        /// <returns>Tree-node matched with the given edge</returns>
        private TreeNode GetEdgeTreeNode(Edge edge)
        {
            TreeNode result = null;
            var root = treeViewHost.Nodes[0];
            var todo = new Stack<TreeNode>();
            todo.Push(root);
            while (todo.Count > 0)
            {
                var node = todo.Pop();
                if (node.Tag != null && node.Tag is Edge tag && tag == edge)
                {
                    result = node;
                    break;
                }

                foreach (TreeNode tmpNode in node.Nodes) todo.Push(tmpNode);
            }

            return result;
        }

        /// <summary>
        ///     Extract checked edges and verify there are edges
        ///     checked in the treeView, if there aren't edges to be checked, complain
        ///     about it with a message box, otherwise close this from.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            ExtractCheckedEdgesAndSelectedSymbol();
            if (m_creationData.EdgesForHostedSweep.Count == 0)
            {
                TaskDialog.Show("Revit", "At least one edge should be selected!");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     This form Load event handle, all the initializations will be in here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdgeFetch_Load(object sender, EventArgs e)
        {
            InitializeTypes();
            InitializeTree();
            InitializeElementGeometry();
            InitializeTreeCheckStates();
            InitializeText();
            InitializePreview();
        }

        /// <summary>
        ///     Suppress the default behaviors that double-click
        ///     on a tree-node which contains children will make the tree-node collapse or expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHost_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (m_cancelExpandOrCollapse)
                e.Cancel = true;
            m_cancelExpandOrCollapse = false;
        }

        /// <summary>
        ///     Suppress the default behaviors that double-click
        ///     on a tree-node which contains children will make the tree-node collapse or expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHost_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (m_cancelExpandOrCollapse)
                e.Cancel = true;
            m_cancelExpandOrCollapse = false;
        }

        /// <summary>
        ///     Draw the geometry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPreview_Paint(object sender, PaintEventArgs e)
        {
            if (m_activeElem == null) return;

            ElementGeometry elemGeo = null;
            if (!m_creationData.Creator.ElemGeomDic.TryGetValue(m_activeElem, out elemGeo)) return;

            e.Graphics.Transform = m_centerMatrix;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            elemGeo.Draw(e.Graphics);
        }

        /// <summary>
        ///     Initialize the track ball.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPreview_MouseDown(object sender, MouseEventArgs e)
        {
            m_trackBall.OnMouseDown(pictureBoxPreview.Width, pictureBoxPreview.Height, e);
        }

        /// <summary>
        ///     Rotate or zoom the displayed geometry, or highlight the edge under the mouse location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_activeElem == null) return;

            m_trackBall.OnMouseMove(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_creationData.Creator.ElemGeomDic[m_activeElem].Rotation *= m_trackBall.Rotation;
                    pictureBoxPreview.Refresh();
                    break;
                case MouseButtons.Right:
                    m_creationData.Creator.ElemGeomDic[m_activeElem].Scale *= m_trackBall.Scale;
                    pictureBoxPreview.Refresh();
                    break;
                case MouseButtons.None:
                {
                    ClearAllHighLight();
                    pictureBoxPreview.Refresh();
                    var mat = m_centerMatrix.Clone();
                    mat.Invert();
                    var pts = new PointF[1] { e.Location };
                    mat.TransformPoints(pts);
                    var elemGeom = m_creationData.Creator.ElemGeomDic[m_activeElem];
                    foreach (var edge in m_creationData.Creator.SupportEdges[m_activeElem])
                        if (elemGeom.EdgeBindingDic.ContainsKey(edge))
                            if (elemGeom.EdgeBindingDic[edge].HighLight(pts[0].X, pts[0].Y))
                                pictureBoxPreview.Refresh();
                    break;
                }
            }
        }

        /// <summary>
        ///     Select or unselect edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_activeElem == null || e.Button != MouseButtons.Left) return;

            var elemGeom = m_creationData.Creator.ElemGeomDic[m_activeElem];
            foreach (var edge in m_creationData.Creator.SupportEdges[m_activeElem])
                if (elemGeom.EdgeBindingDic.ContainsKey(edge))
                    if (elemGeom.EdgeBindingDic[edge].IsHighLighted)
                    {
                        var isSelect = elemGeom.EdgeBindingDic[edge].IsSelected;
                        elemGeom.EdgeBindingDic[edge].IsHighLighted = false;
                        elemGeom.EdgeBindingDic[edge].IsSelected = !isSelect;

                        var node = GetEdgeTreeNode(edge);

                        var state = isSelect ? CheckState.Unchecked : CheckState.Checked;
                        UpdateNodeCheckStatus(node, state);
                        pictureBoxPreview.Refresh();
                        treeViewHost.Refresh();
                        return;
                    }
        }

        /// <summary>
        ///     Highlight the edge in the preview
        ///     if mouse-over an edge tree-node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHost_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            var node = e.Node;
            treeViewHost.SelectedNode = e.Node;
            ClearAllHighLight();
            ActiveNode(node);
            pictureBoxPreview.Refresh();
            if (m_activeElem == null || node.Tag == null || !(node.Tag is Edge tag)) return;

            var elemGeom = m_creationData.Creator.ElemGeomDic[m_activeElem];
            elemGeom.EdgeBindingDic[tag].IsHighLighted = true;
            pictureBoxPreview.Refresh();
        }

        /// <summary>
        ///     Use arrow keys to rotate the display of geometry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdgeFetch_KeyDown(object sender, KeyEventArgs e)
        {
            m_trackBall.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    m_creationData.Creator.ElemGeomDic[m_activeElem].Rotation *= m_trackBall.Rotation;
                    pictureBoxPreview.Refresh();
                    break;
            }
        }

        /// <summary>
        ///     Suppress the key input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHost_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        /// <summary>
        ///     Select or un-select the key-node
        ///     if down the left mouse button in the area of check-box or label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHost_MouseDown(object sender, MouseEventArgs e)
        {
            var hitInfo = treeViewHost.HitTest(e.Location);

            if (e.Button == MouseButtons.Left &&
                (hitInfo.Location == TreeViewHitTestLocations.StateImage ||
                 hitInfo.Location == TreeViewHitTestLocations.Label))
            {
                // mouse down in area of state image or label.                
                var node = hitInfo.Node;
                if (node.Nodes.Count > 0)
                    // cancel the expand or collapse of node which has children.
                    m_cancelExpandOrCollapse = true;

                // active the node.
                ActiveNode(node);

                // select or un-select the node.
                var checkState = (CheckState)((node.StateImageIndex + 1) % 2);
                UpdateNodeCheckStatus(node, checkState);

                pictureBoxPreview.Refresh();
            }
        }
    }
}
