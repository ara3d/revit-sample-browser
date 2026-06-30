// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.EnergyAnalysisModel.CS
{
    public class EnergyAnalysisModel
    {
        private EnergyAnalysisDetailModel m_energyAnalysisDetailModel;
        private readonly Document m_revitDoc;

        public EnergyAnalysisModel(Document doc)
        {
            m_revitDoc = doc;
            Options = new EnergyAnalysisDetailModelOptions();
        }

        public EnergyAnalysisDetailModelOptions Options { get; set; }

        public void Initialize()
        {
            m_energyAnalysisDetailModel = EnergyAnalysisDetailModel.Create(m_revitDoc, Options);
            m_energyAnalysisDetailModel.TransformModel();
        }

        public XElement GetAnalyticalOpenings()
        {
            XElement openingsNode = new("OpeningsModels");
            openingsNode.Add(new XAttribute("Name", "OpeningsModels"));

            var openings = m_energyAnalysisDetailModel.GetAnalyticalOpenings();
            foreach (var opening in openings)
            {
                XElement openNode = new("Open");
                openNode.Add(new XAttribute("Name", opening.Name));
                openingsNode.Add(openNode);

                var openingSurface = opening.GetAnalyticalSurface();
                if (null == openingSurface)
                    continue;
                XElement surfaceNode = new("Surface");
                surfaceNode.Add(new XAttribute("Name", openingSurface.Name));
                openNode.Add(surfaceNode);
            }

            return openingsNode;
        }

        public XElement GetAnalyticalShadingSurfaces()
        {
            XElement shadingSurfacesNode = new("ShadingSurfaces1");
            shadingSurfacesNode.Add(new XAttribute("Name", "ShadingSurfaces"));

            var shadingSurfaces = m_energyAnalysisDetailModel.GetAnalyticalShadingSurfaces();
            SurfacesToXElement(shadingSurfacesNode, shadingSurfaces);

            return shadingSurfacesNode;
        }

        public XElement GetAnalyticalSpaces()
        {
            XElement energyAnalysisSpacesNode = new("AnalyticalSpaces");
            energyAnalysisSpacesNode.Add(new XAttribute("Name", "AnalyticalSpaces"));
            var energyAnalysisSpaces = m_energyAnalysisDetailModel.GetAnalyticalSpaces();
            foreach (var space in energyAnalysisSpaces)
            {
                XElement spaceNode = new("Space");
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
                XElement surfaceNode = new("Surface");
                surfaceNode.Add(new XAttribute("Name", surface.Name));
                node.Add(surfaceNode);
            }
        }

        public void RefreshAnalysisData(TreeView treeView)
        {
            treeView.Nodes.Clear();

            TreeNode node = new("BuildingModel");
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
            TreeNode node = new(element.FirstAttribute.Value);
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
            Options.Tier = tierValue switch
            {
                "Final" => EnergyAnalysisDetailModelTier.Final,
                "FirstLevelBoundaries" => EnergyAnalysisDetailModelTier.FirstLevelBoundaries,
                "NotComputed" => EnergyAnalysisDetailModelTier.NotComputed,
                "SecondLevelBoundaries" => EnergyAnalysisDetailModelTier.SecondLevelBoundaries,
                _ => EnergyAnalysisDetailModelTier.SecondLevelBoundaries,
            };
        }
    }
}
