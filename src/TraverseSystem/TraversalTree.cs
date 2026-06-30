// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;
using System.Xml;
namespace Ara3D.RevitSampleBrowser.TraverseSystem.CS
{
    public class TreeNode
    {

        /// <summary>
        ///     Active document of Revit
        /// </summary>
        private readonly Document m_document;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc">Revit document</param>
        /// <param name="id">Element's Id</param>
        public TreeNode(Document doc, ElementId id)
        {
            m_document = doc;
            Id = id;
            ChildNodes = [];
        }

        public ElementId Id { get; }

        public FlowDirectionType Direction { get; set; }

        public TreeNode Parent { get; set; }

        public List<TreeNode> ChildNodes { get; set; }

        /// <summary>
        ///     The connector of the previous element to which current element is connected
        /// </summary>
        public Connector InputConnector { get; set; }

        private Element GetElementById(ElementId eid)
        {
            return m_document.GetElement(eid);
        }

        public void DumpIntoXml(XmlWriter writer)
        {
            // Write node information
            var element = GetElementById(Id);
            if (element is FamilyInstance fi)
            {
                var mepModel = fi.MEPModel;
                var type = string.Empty;
                switch (mepModel)
                {
                    case MechanicalEquipment _:
                        type = "MechanicalEquipment";
                        writer.WriteStartElement(type);
                        break;
                    case MechanicalFitting mf:
                        type = "MechanicalFitting";
                        writer.WriteStartElement(type);
                        writer.WriteAttributeString("Category", element.Category.Name);
                        writer.WriteAttributeString("PartType", mf.PartType.ToString());
                        break;
                    default:
                        type = "FamilyInstance";
                        writer.WriteStartElement(type);
                        writer.WriteAttributeString("Category", element.Category.Name);
                        break;
                }

                writer.WriteAttributeString("Name", element.Name);
                writer.WriteAttributeString("Id", element.Id.ToString());
                writer.WriteAttributeString("Direction", Direction.ToString());
                writer.WriteEndElement();
            }
            else
            {
                var type = element.GetType().Name;

                writer.WriteStartElement(type);
                writer.WriteAttributeString("Name", element.Name);
                writer.WriteAttributeString("Id", element.Id.ToString());
                writer.WriteAttributeString("Direction", Direction.ToString());
                writer.WriteEndElement();
            }

            foreach (var node in ChildNodes)
            {
                if (ChildNodes.Count > 1) writer.WriteStartElement("Path");

                node.DumpIntoXml(writer);

                if (ChildNodes.Count > 1) writer.WriteEndElement();
            }
        }
    }

    public class TraversalTree
    {
        // Active document of Revit
        private readonly Document m_document;

        // The flag whether the MEP system of the traversal is a mechanical system or piping system
        private readonly bool m_isMechanicalSystem;

        // The starting element node
        private TreeNode m_startingElementNode;

        // The MEP system of the traversal
        private readonly MEPSystem m_system;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="activeDocument">Revit document</param>
        /// <param name="system">The MEP system to traverse</param>
        public TraversalTree(Document activeDocument, MEPSystem system)
        {
            m_document = activeDocument;
            m_system = system;
            m_isMechanicalSystem = system is MechanicalSystem;
        }

        public void Traverse()
        {
            // Get the starting element node
            m_startingElementNode = GetStartingElementNode();

            // Traverse the system recursively
            Traverse(m_startingElementNode);
        }

        /// <summary>
        ///     Get the starting element node.
        ///     If the system has base equipment then get it;
        ///     Otherwise get the owner of the open connector in the system
        /// </summary>
        /// <returns>The starting element node</returns>
        private TreeNode GetStartingElementNode()
        {
            TreeNode startingElementNode = null;

            var equipment = m_system.BaseEquipment;
            //
            // If the system has base equipment then get it;
            // Otherwise get the owner of the open connector in the system
            startingElementNode = equipment != null ? new TreeNode(m_document, equipment.Id) : new TreeNode(m_document, GetOwnerOfOpenConnector().Id);

            startingElementNode.Parent = null;
            startingElementNode.InputConnector = null;

            return startingElementNode;
        }

        /// <summary>
        ///     Get the owner of the open connector as the starting element
        /// </summary>
        /// <returns>The owner</returns>
        private Element GetOwnerOfOpenConnector()
        {
            Element element = null;

            //
            // Get an element from the system's terminals
            var elements = m_system.Elements;
            foreach (Element ele in elements)
            {
                element = ele;
                break;
            }

            // Get the open connector recursively
            var openConnector = GetOpenConnector(element, null);

            return openConnector.Owner;
        }

        /// <summary>
        ///     Get the open connector of the system if the system has no base equipment
        /// </summary>
        /// <param name="element">An element in the system</param>
        /// <param name="inputConnector">
        ///     The connector of the previous element
        ///     to which the element is connected
        /// </param>
        /// <returns>The found open connector</returns>
        private Connector GetOpenConnector(Element element, Connector inputConnector)
        {
            Connector openConnector = null;
            ConnectorManager cm = null;
            //
            // Get the connector manager of the element
            if (element is FamilyInstance fi)
            {
                cm = fi.MEPModel.ConnectorManager;
            }
            else
            {
                var mepCurve = element as MEPCurve;
                cm = mepCurve.ConnectorManager;
            }

            foreach (Connector conn in cm.Connectors)
            {
                // Ignore the connector does not belong to any MEP System or belongs to another different MEP system
                if (conn.MEPSystem == null || conn.MEPSystem.Id != m_system.Id) continue;

                // If the connector is connected to the input connector, they will have opposite flow directions.
                if (inputConnector != null && conn.IsConnectedTo(inputConnector)) continue;

                // If the connector is not connected, it is the open connector
                if (!conn.IsConnected)
                {
                    openConnector = conn;
                    break;
                }

                //
                // If open connector not found, then look for it from elements connected to the element
                foreach (Connector refConnector in conn.AllRefs)
                {
                    // Ignore non-EndConn connectors and connectors of the current element
                    if (refConnector.ConnectorType != ConnectorType.End ||
                        refConnector.Owner.Id == conn.Owner.Id)
                        continue;

                    // Ignore connectors of the previous element
                    if (inputConnector != null && refConnector.Owner.Id == inputConnector.Owner.Id) continue;

                    openConnector = GetOpenConnector(refConnector.Owner, conn);
                    if (openConnector != null) return openConnector;
                }
            }

            return openConnector;
        }

        private void Traverse(TreeNode elementNode)
        {
            //
            // Find all child nodes and analyze them recursively
            AppendChildren(elementNode);
            foreach (var node in elementNode.ChildNodes)
            {
                Traverse(node);
            }
        }

        private void AppendChildren(TreeNode elementNode)
        {
            var nodes = elementNode.ChildNodes;
            ConnectorSet connectors;
            //
            // Get connector manager
            var element = GetElementById(elementNode.Id);
            if (element is FamilyInstance fi)
            {
                connectors = fi.MEPModel.ConnectorManager.Connectors;
            }
            else
            {
                var mepCurve = element as MEPCurve;
                connectors = mepCurve.ConnectorManager.Connectors;
            }

            // Find connected connector for each connector
            foreach (Connector connector in connectors)
            {
                var mepSystem = connector.MEPSystem;
                // Ignore the connector does not belong to any MEP System or belongs to another different MEP system
                if (mepSystem == null || mepSystem.Id != m_system.Id) continue;

                //
                // Get the direction of the TreeNode object
                if (elementNode.Parent == null)
                {
                    if (connector.IsConnected) elementNode.Direction = connector.Direction;
                }
                else
                {
                    // If the connector is connected to the input connector, they will have opposite flow directions.
                    // Then skip it.
                    if (connector.IsConnectedTo(elementNode.InputConnector))
                    {
                        elementNode.Direction = connector.Direction;
                        continue;
                    }
                }

                // Get the connector connected to current connector
                var connectedConnector = ConnectorHelper.GetConnectedConnector(connector);
                if (connectedConnector != null)
                {
                    var node = new TreeNode(m_document, connectedConnector.Owner.Id)
                    {
                        InputConnector = connector,
                        Parent = elementNode
                    };
                    nodes.Add(node);
                }
            }

            nodes.Sort((t1, t2) => t1.Id > t2.Id ? 1 : t1.Id < t2.Id ? -1 : 0
            );
        }

        private Element GetElementById(ElementId eid)
        {
            return m_document.GetElement(eid);
        }

        public void DumpIntoXml(string fileName)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    "
            };
            var writer = XmlWriter.Create(fileName, settings);

            // Write the root element
            var mepSystemType = string.Empty;
            mepSystemType = m_system is MechanicalSystem ? "MechanicalSystem" : "PipingSystem";
            writer.WriteStartElement(mepSystemType);

            // Write basic information of the MEP system
            WriteBasicInfo(writer);
            // Write paths of the traversal
            WritePaths(writer);

            // Close the root element
            writer.WriteEndElement();

            writer.Flush();
            writer.Close();
        }

        private void WriteBasicInfo(XmlWriter writer)
        {
            MechanicalSystem ms = null;
            PipingSystem ps = null;
            if (m_isMechanicalSystem)
                ms = m_system as MechanicalSystem;
            else
                ps = m_system as PipingSystem;

            // Write basic information of the system
            writer.WriteStartElement("BasicInformation");

            // Write Name property
            writer.WriteStartElement("Name");
            writer.WriteString(m_system.Name);
            writer.WriteEndElement();

            // Write Id property
            writer.WriteStartElement("Id");
            writer.WriteValue(m_system.Id.ToString());
            writer.WriteEndElement();

            // Write UniqueId property
            writer.WriteStartElement("UniqueId");
            writer.WriteString(m_system.UniqueId);
            writer.WriteEndElement();

            // Write SystemType property
            writer.WriteStartElement("SystemType");
            if (m_isMechanicalSystem)
                writer.WriteString(ms.SystemType.ToString());
            else
                writer.WriteString(ps.SystemType.ToString());
            writer.WriteEndElement();

            // Write Category property
            writer.WriteStartElement("Category");
            writer.WriteAttributeString("Id", m_system.Category.Id.ToString());
            writer.WriteAttributeString("Name", m_system.Category.Name);
            writer.WriteEndElement();

            // Write IsWellConnected property
            writer.WriteStartElement("IsWellConnected");
            if (m_isMechanicalSystem)
                writer.WriteValue(ms.IsWellConnected);
            else
                writer.WriteValue(ps.IsWellConnected);
            writer.WriteEndElement();

            // Write HasBaseEquipment property
            writer.WriteStartElement("HasBaseEquipment");
            var hasBaseEquipment = m_system.BaseEquipment != null;
            writer.WriteValue(hasBaseEquipment);
            writer.WriteEndElement();

            // Write TerminalElementsCount property
            writer.WriteStartElement("TerminalElementsCount");
            writer.WriteValue(m_system.Elements.Size);
            writer.WriteEndElement();

            // Write Flow property
            writer.WriteStartElement("Flow");
            if (m_isMechanicalSystem)
                writer.WriteValue(ms.GetFlow());
            else
                writer.WriteValue(ps.GetFlow());
            writer.WriteEndElement();

            // Close basic information
            writer.WriteEndElement();
        }

        private void WritePaths(XmlWriter writer)
        {
            writer.WriteStartElement("Path");
            m_startingElementNode.DumpIntoXml(writer);
            writer.WriteEndElement();
        }
    }
}
