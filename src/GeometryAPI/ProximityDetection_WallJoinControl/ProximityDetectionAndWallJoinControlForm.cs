// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    /// <summary>
    ///     The form that show the operations and results
    /// </summary>
    public partial class ProximityDetectionAndWallJoinControlForm : Form
    {
        /// <summary>
        ///     revit document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     The object that is responsible for proximity detection
        /// </summary>
        private readonly ProximityDetection m_proximityDetection;

        /// <summary>
        ///     The object that is responsible for controlling the joint of walls
        /// </summary>
        private readonly WallJoinControl m_walljoinControl;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc">Revit document</param>
        /// <param name="proximityDetection">The object that is responsible for proximity detection</param>
        /// <param name="walljoinControl">The object that is responsible for controlling the joint of walls</param>
        public ProximityDetectionAndWallJoinControlForm(Document doc,
            ProximityDetection proximityDetection,
            WallJoinControl walljoinControl)
        {
            InitializeComponent();

            m_doc = doc;
            m_proximityDetection = proximityDetection;
            m_walljoinControl = walljoinControl;
        }

        /// <summary>
        ///     Clear the treeview's data
        /// </summary>
        private void ClearTreeviewData()
        {
            treeViewResults.Nodes.Clear();
        }

        /// <summary>
        ///     Refresh the treeview's data by given XML
        /// </summary>
        /// <param name="operation">The operation name</param>
        /// <param name="element">The given XML</param>
        private void RefreshTreeviewData(string operation, XElement element)
        {
            treeViewResults.Nodes.Clear();

            //treeView.Nodes adds first level node
            var node = new TreeNode(operation);
            treeViewResults.Nodes.Add(node);

            // append node
            var spaceNode = XElementToTreeNode(element);
            node.Nodes.Add(spaceNode);
        }

        /// <summary>
        ///     This method converts XElement nodes to Tree nodes
        /// </summary>
        /// <param name="element">XElement to be converted</param>
        /// <returns>Tree Node that comes from XElement</returns>
        private TreeNode XElementToTreeNode(XElement element)
        {
            if (0 == element.Name.LocalName.Length)
                return null;

            var nodename = element.Name.LocalName;
            foreach (var att in element.Attributes())
            {
                nodename += $" ({att.Name.LocalName}:{att.Value})";
            }

            //TreeNode node = new TreeNode(element.FirstAttribute.Value);
            var node = new TreeNode(nodename);

            if (!element.HasElements)
                // return if it is leaf node
                return node;
            // convert its child elements
            foreach (var ele in element.Elements())
            {
                node.Nodes.Add(XElementToTreeNode(ele));
            }

            // return whole node
            return node;
        }

        /// <summary>
        ///     Get all of the walls
        /// </summary>
        /// <returns>All walls' set</returns>
        private IEnumerable<Wall> GetAllWalls()
        {
            var wallCollector = new FilteredElementCollector(m_doc);
            wallCollector.OfClass(typeof(Wall));

            return wallCollector.OfType<Wall>();
        }

        /// <summary>
        ///     Get all of the egresses
        /// </summary>
        /// <returns>All egresses' set</returns>
        private ICollection<Element> GetAllEgresses()
        {
            var collector = new FilteredElementCollector(m_doc);
            var filterFamilyInstance = new ElementClassFilter(typeof(FamilyInstance));
            var filterDoorCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var filterDoorInstance = new LogicalAndFilter(filterDoorCategory, filterFamilyInstance);
            return collector.WherePasses(filterDoorInstance).ToElements();
        }

        private void radioButtonFindColumnsInWall_CheckedChanged(object sender, EventArgs e)
        {
            ClearTreeviewData();

            labelDescription.Text = "Find columns in wall";
        }

        private void radioButtonFindBlockingElements_CheckedChanged(object sender, EventArgs e)
        {
            ClearTreeviewData();

            labelDescription.Text = "Find elements blocking egress";
        }

        private void radioButtonFindNearbyWalls_CheckedChanged(object sender, EventArgs e)
        {
            ClearTreeviewData();

            labelDescription.Text = "Find walls (nearly joined to) end of walls";
        }

        private void radioButtonCheckJoinedWalls_CheckedChanged(object sender, EventArgs e)
        {
            ClearTreeviewData();

            labelDescription.Text = "Check walls join/disjoin states";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var tran = new Transaction(m_doc, "GeometryCreation_BooleanOperation");
            tran.Start();

            if (radioButtonFindColumnsInWall.Checked)
                RefreshTreeviewData("Find columns in wall", m_proximityDetection.FindColumnsInWall(GetAllWalls()));
            else if (radioButtonFindBlockingElements.Checked)
                RefreshTreeviewData("Find elements blocking egress",
                    m_proximityDetection.FindBlockingElements(GetAllEgresses()));
            else if (radioButtonFindNearbyWalls.Checked)
                RefreshTreeviewData("Find walls (nearly joined to) end of walls",
                    m_proximityDetection.FindNearbyWalls(GetAllWalls()));
            else
                RefreshTreeviewData("Check walls join/disjoin states",
                    m_walljoinControl.CheckJoinedWalls(GetAllWalls()));

            // expand all child
            treeViewResults.ExpandAll();

            tran.Commit();
        }
    }
}
