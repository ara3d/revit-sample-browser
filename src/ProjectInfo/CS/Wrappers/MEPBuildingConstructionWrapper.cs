// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    ///     Wrapper class for MEPBuildingConstruction
    /// </summary>
    public class MepBuildingConstructionWrapper : IWrapper
    {
        /// <summary>
        ///     MEPBuildingConstruction
        /// </summary>
        private readonly MEPBuildingConstruction m_mEpBuildingConstruction;

        /// <summary>
        ///     Initializes private variables.
        /// </summary>
        /// <param name="mEpBuildingConstruction">MEPBuildingConstruction</param>
        public MepBuildingConstructionWrapper(MEPBuildingConstruction mEpBuildingConstruction)
        {
            m_mEpBuildingConstruction = mEpBuildingConstruction;
        }

        /// <summary>
        ///     Gets or sets Roofs
        /// </summary>
        [DisplayName("Roofs")]
        public ConstructionWrapper Roof
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Roof));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Roof,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Exterior Walls
        /// </summary>
        [DisplayName("Exterior Walls")]
        public ConstructionWrapper ExteriorWall
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWall));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWall,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Interior Walls
        /// </summary>
        [DisplayName("Interior Walls")]
        public ConstructionWrapper InteriorWall
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.InteriorWall));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.InteriorWall,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Ceilings
        /// </summary>
        [DisplayName("Ceilings")]
        public ConstructionWrapper Ceiling
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Ceiling));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Ceiling,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Doors
        /// </summary>
        [DisplayName("Doors")]
        public ConstructionWrapper Door
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Door));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Door,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Slabs
        /// </summary>
        [DisplayName("Slabs")]
        public ConstructionWrapper Slab
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Slab));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Slab,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Floors
        /// </summary>
        [DisplayName("Floors")]
        public ConstructionWrapper Floor
        {
            get => new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Floor));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Floor,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Exterior Windows
        /// </summary>
        [DisplayName("Exterior Windows")]
        public ConstructionWrapper ExteriorWindow
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWindow));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWindow,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Interior Windows
        /// </summary>
        [DisplayName("Interior Windows")]
        public ConstructionWrapper InteriorWindow
        {
            get => new ConstructionWrapper(
                m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.ExteriorWindow));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.ExteriorWindow,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets or sets Skylights
        /// </summary>
        [DisplayName("Skylights")]
        public ConstructionWrapper Skylight
        {
            get =>
                new ConstructionWrapper(m_mEpBuildingConstruction.GetBuildingConstruction(ConstructionType.Skylight));
            set => m_mEpBuildingConstruction.SetBuildingConstruction(ConstructionType.Skylight,
                value.Handle as Construction);
        }

        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        [Browsable(false)]
        public object Handle => m_mEpBuildingConstruction;

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get => m_mEpBuildingConstruction.Name;
            set => m_mEpBuildingConstruction.Name = value;
        }

        /// <summary>
        ///     Get constructions
        /// </summary>
        /// <param name="constructionType">ConstructionType</param>
        /// <returns>Related Constructions specified by constructionTypes</returns>
        public ICollection<Construction> GetConstructions(ConstructionType constructionType)
        {
            return m_mEpBuildingConstruction.GetConstructions(constructionType);
        }
    }
}
