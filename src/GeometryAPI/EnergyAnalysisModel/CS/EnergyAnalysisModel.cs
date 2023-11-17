// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.EnergyAnalysisModel.CS
{
    public class EnergyAnalysisModel
    {
        // An EnergyAnalysisDetailModel member that can get all analysis data includes surfaces, spaces and openings.
        private EnergyAnalysisDetailModel m_energyAnalysisDetailModel;

        // Options for Energy Analysis process
        // revit document
        private readonly Document m_revitDoc;
        private EnergyAnalysisDetailModelOptions m_options;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc">Revit Document</param>
        public EnergyAnalysisModel(Document doc)
        {
            m_revitDoc = doc;
            Options = new EnergyAnalysisDetailModelOptions();
        }

        // Options Property
        public EnergyAnalysisDetailModelOptions Options
        {
            get => m_options;
            set => m_options = value;
        }

        /// <summary>
        ///     Get EnergyAnalysisDetailModel object and Initialize it.
        /// </summary>
        public void Initialize()
        {
            // create the model with a document and options.
            m_energyAnalysisDetailModel = EnergyAnalysisDetailModel.Create(m_revitDoc, Options);
            m_energyAnalysisDetailModel.TransformModel();
        }

        /// <summary>
        ///     This method get all openings surfaces from current model
        /// </summary>
        /// <returns>XElement that places openings surfaces</returns>
        public XElement GetAnalyticalOpenings()
        {
            // openings for the first EnergyAnalysisDetailModel whose openings should not be merged
            var openingsNode = new XElement("OpeningsModels");
            openingsNode.Add(new XAttribute("Name", "OpeningsModels"));

            // get EnergyAnalysisOpenings from Model1
            var openings = m_energyAnalysisDetailModel.GetAnalyticalOpenings();
            foreach (var opening in openings)
            {
                var openNode = new XElement("Open");
                openNode.Add(new XAttribute("Name", opening.Name));
                // add individual opening node to whol openings node
                openingsNode.Add(openNode);

                // get surfaces from opening
                var openingSurface = opening.GetAnalyticalSurface();
                if (null == openingSurface)
                    continue;
                var surfaceNode = new XElement("Surface");
                surfaceNode.Add(new XAttribute("Name", openingSurface.Name));
                openNode.Add(surfaceNode);
            }

            // return the whole openings node
            return openingsNode;
        }

        /// <summary>
        ///     This method get all Analytical ShadingSurfaces from current model
        /// </summary>
        /// <returns>XElement that places shading surfaces</returns>
        public XElement GetAnalyticalShadingSurfaces()
        {
            // create a node that places all shading surfaces
            var shadingSurfacesNode = new XElement("ShadingSurfaces1");
            shadingSurfacesNode.Add(new XAttribute("Name", "ShadingSurfaces"));

            // get shadingSurfaces from Model
            var shadingSurfaces = m_energyAnalysisDetailModel.GetAnalyticalShadingSurfaces();
            SurfacesToXElement(shadingSurfacesNode, shadingSurfaces);

            return shadingSurfacesNode;
        }

        /// <summary>
        ///     Extract Analytical data about Space and its surfaces
        /// </summary>
        /// <returns>XElment that includes all data about AnalyticalSpace</returns>
        public XElement GetAnalyticalSpaces()
        {
            // create a node that place all spaces.
            var energyAnalysisSpacesNode = new XElement("AnalyticalSpaces");
            energyAnalysisSpacesNode.Add(new XAttribute("Name", "AnalyticalSpaces"));
            // get EnergyAnalysisSpaces from m_energyAnalysisDetailModel
            var energyAnalysisSpaces = m_energyAnalysisDetailModel.GetAnalyticalSpaces();
            // get surface from each Space
            foreach (var space in energyAnalysisSpaces)
            {
                var spaceNode = new XElement("Space");
                spaceNode.Add(new XAttribute("Name", space.ComposedName));
                // add individual space node to spaces collection node
                energyAnalysisSpacesNode.Add(spaceNode);

                var analyticalSurfaces = space.GetAnalyticalSurfaces();
                SurfacesToXElement(spaceNode, analyticalSurfaces);
            }

            // return the whole Spaces Node
            return energyAnalysisSpacesNode;
        }

        /// <summary>
        ///     The method adds given surfaces to specific XElement
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="analyticalSurfaces">The surfaces list that will be added into the para node</param>
        private void SurfacesToXElement(XElement node, IList<EnergyAnalysisSurface> analyticalSurfaces)
        {
            // go through all surfaces
            foreach (var surface in analyticalSurfaces)
            {
                var surfaceNode = new XElement("Surface");
                surfaceNode.Add(new XAttribute("Name", surface.Name));
                // add individual surface node to parent node
                node.Add(surfaceNode);
            }
        }

        /// <summary>
        ///     Get Analytical data and pass them to UI controls
        /// </summary>
        /// <param name="treeView"></param>
        public void RefreshAnalysisData(TreeView treeView)
        {
            treeView.Nodes.Clear();

            //treeView.Nodes adds first level node
            var node = new TreeNode("BuildingModel");
            treeView.Nodes.Add(node);

            // append space surfaces node
            var spaceNode = XElementToTreeNode(GetAnalyticalSpaces());
            node.Nodes.Add(spaceNode);

            // append opening surfaces node
            var openingNode = XElementToTreeNode(GetAnalyticalOpenings());
            node.Nodes.Add(openingNode);

            // append shading surfaces node
            var shadingNode = XElementToTreeNode(GetAnalyticalShadingSurfaces());
            node.Nodes.Add(shadingNode);
        }

        /// <summary>
        ///     This method converts XElement nodes to Tree nodes so that analysis data could be displayed in UI treeView
        /// </summary>
        /// <param name="element">XElement to be converted</param>
        /// <returns>Tree Node that comes from XElement</returns>
        private TreeNode XElementToTreeNode(XElement element)
        {
            if (null == element.FirstAttribute)
                return null;
            var node = new TreeNode(element.FirstAttribute.Value);
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
        ///     This method converts UI selected string to EnergyAnalysisDetailModelTier enum
        /// </summary>
        /// <param name="tierValue">Selected string from UI</param>
        public void SetTier(string tierValue)
        {
            switch (tierValue)
            {
                case "Final":
                    Options.Tier = EnergyAnalysisDetailModelTier.Final;
                    break;
                case "FirstLevelBoundaries":
                    Options.Tier = EnergyAnalysisDetailModelTier.FirstLevelBoundaries;
                    break;
                case "NotComputed":
                    Options.Tier = EnergyAnalysisDetailModelTier.NotComputed;
                    break;
                case "SecondLevelBoundaries":
                    Options.Tier = EnergyAnalysisDetailModelTier.SecondLevelBoundaries;
                    break;
                // the default Tier is SecondLevelBoundaries
                default:
                    Options.Tier = EnergyAnalysisDetailModelTier.SecondLevelBoundaries;
                    break;
            }
        }
    }
}
