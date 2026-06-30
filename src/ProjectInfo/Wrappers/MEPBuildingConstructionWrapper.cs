// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class MepBuildingConstructionWrapper : IWrapper
    {
        private readonly MEPBuildingConstruction m_mEpBuildingConstruction;

        public MepBuildingConstructionWrapper(MEPBuildingConstruction mEpBuildingConstruction)
        {
            m_mEpBuildingConstruction = mEpBuildingConstruction;
        }

        [DisplayName("Roofs")]
        public ConstructionWrapper Roof
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Roof));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Roof,
                value.Handle as Construction);
        }

        [DisplayName("Exterior Walls")]
        public ConstructionWrapper ExteriorWall
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWall));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWall,
                value.Handle as Construction);
        }

        [DisplayName("Interior Walls")]
        public ConstructionWrapper InteriorWall
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.InteriorWall));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.InteriorWall,
                value.Handle as Construction);
        }

        [DisplayName("Ceilings")]
        public ConstructionWrapper Ceiling
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Ceiling));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Ceiling,
                value.Handle as Construction);
        }

        [DisplayName("Doors")]
        public ConstructionWrapper Door
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Door));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Door,
                value.Handle as Construction);
        }

        [DisplayName("Slabs")]
        public ConstructionWrapper Slab
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Slab));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Slab,
                value.Handle as Construction);
        }

        [DisplayName("Floors")]
        public ConstructionWrapper Floor
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Floor));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Floor,
                value.Handle as Construction);
        }

        [DisplayName("Exterior Windows")]
        public ConstructionWrapper ExteriorWindow
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWindow));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWindow,
                value.Handle as Construction);
        }

        [DisplayName("Interior Windows")]
        public ConstructionWrapper InteriorWindow
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWindow));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWindow,
                value.Handle as Construction);
        }

        [DisplayName("Skylights")]
        public ConstructionWrapper Skylight
        {
            get =>
                new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Skylight));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Skylight,
                value.Handle as Construction);
        }

        [Browsable(false)]
        public object Handle => m_mEpBuildingConstruction;

        [Browsable(false)]
        public string Name
        {
            get => m_mEpBuildingConstruction.Name;
            set => m_mEpBuildingConstruction.Name = value;
        }

        public ICollection<Construction> GetConstructions(ConstructionType constructionType)
        {
            return m_mEpBuildingConstruction.GetConstructions(constructionType);
        }
    }
}
