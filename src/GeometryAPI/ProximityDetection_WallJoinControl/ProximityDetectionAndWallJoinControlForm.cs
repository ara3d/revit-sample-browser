// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    public partial class ProximityDetectionAndWallJoinControlForm : Form
    {
        private readonly Document m_doc;
        private readonly ProximityDetection m_proximityDetection;
        private readonly WallJoinControl m_walljoinControl;

        public ProximityDetectionAndWallJoinControlForm(Document doc,
            ProximityDetection proximityDetection,
            WallJoinControl walljoinControl)
        {
            InitializeComponent();

            m_doc = doc;
            m_proximityDetection = proximityDetection;
            m_walljoinControl = walljoinControl;
        }

        private void ClearTreeviewData()
        {
            treeViewResults.Nodes.Clear();
        }

        private void RefreshTreeviewData(string operation, XElement element)
        {
            treeViewResults.Nodes.Clear();

            TreeNode node = new(operation);
            treeViewResults.Nodes.Add(node);

            var spaceNode = XElementToTreeNode(element);
            node.Nodes.Add(spaceNode);
        }

        private TreeNode XElementToTreeNode(XElement element)
        {
            if (0 == element.Name.LocalName.Length)
                return null;

            var nodename = element.Name.LocalName;
            foreach (var att in element.Attributes())
            {
                nodename += $" ({att.Name.LocalName}:{att.Value})";
            }

            TreeNode node = new(nodename);

            if (!element.HasElements)
                return node;
            foreach (var ele in element.Elements())
            {
                node.Nodes.Add(XElementToTreeNode(ele));
            }

            return node;
        }

        private IEnumerable<Wall> GetAllWalls()
        {
            FilteredElementCollector wallCollector = new(m_doc);
            wallCollector.OfClass(typeof(Wall));

            return wallCollector.OfType<Wall>();
        }

        private ICollection<Element> GetAllEgresses()
        {
            FilteredElementCollector collector = new(m_doc);
            ElementClassFilter filterFamilyInstance = new(typeof(FamilyInstance));
            ElementCategoryFilter filterDoorCategory = new(BuiltInCategory.OST_Doors);
            LogicalAndFilter filterDoorInstance = new(filterDoorCategory, filterFamilyInstance);
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
            Transaction tran = new(m_doc, "GeometryCreation_BooleanOperation");
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

            treeViewResults.ExpandAll();

            tran.Commit();
        }
    }
}
