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
        private EnergyAnalysisDetailModel m_energyAnalysisDetailModel;
        private readonly Document m_revitDoc;
        private EnergyAnalysisDetailModelOptions m_options;

        public EnergyAnalysisModel(Document doc)
        {
            m_revitDoc = doc;
            Options = new EnergyAnalysisDetailModelOptions();
        }

        public EnergyAnalysisDetailModelOptions Options
        {
            get => m_options;
            set => m_options = value;
        }

        public void Initialize()
        {
            m_energyAnalysisDetailModel = EnergyAnalysisDetailModel.Create(m_revitDoc, Options);
            m_energyAnalysisDetailModel.TransformModel();
        }

        public XElement GetAnalyticalOpenings()
        {
            var openingsNode = new XElement("OpeningsModels");
            openingsNode.Add(new XAttribute("Name", "OpeningsModels"));

            var openings = m_energyAnalysisDetailModel.GetAnalyticalOpenings();
            foreach (var opening in openings)
            {
                var openNode = new XElement("Open");
                openNode.Add(new XAttribute("Name", opening.Name));
                openingsNode.Add(openNode);

                var openingSurface = opening.GetAnalyticalSurface();
                if (null == openingSurface)
                    continue;
                var surfaceNode = new XElement("Surface");
                surfaceNode.Add(new XAttribute("Name", openingSurface.Name));
                openNode.Add(surfaceNode);
            }

            return openingsNode;
        }

        public XElement GetAnalyticalShadingSurfaces()
        {
            var shadingSurfacesNode = new XElement("ShadingSurfaces1");
            shadingSurfacesNode.Add(new XAttribute("Name", "ShadingSurfaces"));

            var shadingSurfaces = m_energyAnalysisDetailModel.GetAnalyticalShadingSurfaces();
            SurfacesToXElement(shadingSurfacesNode, shadingSurfaces);

            return shadingSurfacesNode;
        }

        public XElement GetAnalyticalSpaces()
        {
            var energyAnalysisSpacesNode = new XElement("AnalyticalSpaces");
            energyAnalysisSpacesNode.Add(new XAttribute("Name", "AnalyticalSpaces"));
            var energyAnalysisSpaces = m_energyAnalysisDetailModel.GetAnalyticalSpaces();
            foreach (var space in energyAnalysisSpaces)
            {
                var spaceNode = new XElement("Space");
                spaceNode.Add(new XAttribute("Name", space.ComposedName));
                energyAnalysisSpacesNode.Add(spaceNode);

                var analyticalSurfaces = space.GetAnalyticalSurfaces();
                SurfacesToXElement(spaceNode, analyticalSurfaces);
            }

            return energyAnalysisSpacesNode;
        }

        private void SurfacesToXElement(XElement node, IList<EnergyAnalysisSurface> analyticalSurfaces)
        {
            foreach (var surface in analyticalSurfaces)
            {
                var surfaceNode = new XElement("Surface");
                surfaceNode.Add(new XAttribute("Name", surface.Name));
                node.Add(surfaceNode);
            }
        }

        public void RefreshAnalysisData(TreeView treeView)
        {
            treeView.Nodes.Clear();

            var node = new TreeNode("BuildingModel");
            treeView.Nodes.Add(node);

            var spaceNode = XElementToTreeNode(GetAnalyticalSpaces());
            node.Nodes.Add(spaceNode);

            var openingNode = XElementToTreeNode(GetAnalyticalOpenings());
            node.Nodes.Add(openingNode);

            var shadingNode = XElementToTreeNode(GetAnalyticalShadingSurfaces());
            node.Nodes.Add(shadingNode);
        }

        private TreeNode XElementToTreeNode(XElement element)
        {
            if (null == element.FirstAttribute)
                return null;
            var node = new TreeNode(element.FirstAttribute.Value);
            if (!element.HasElements)
                return node;
            foreach (var ele in element.Elements())
            {
                node.Nodes.Add(XElementToTreeNode(ele));
            }

            return node;
        }

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
                default:
                    Options.Tier = EnergyAnalysisDetailModelTier.SecondLevelBoundaries;
                    break;
            }
        }
    }
}
