// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms
{
    /// <summary>
    ///     The ExtrusionRoofWrapper class is use to edit a extrusion roof in a PropertyGrid.
    ///     It contains a extrusion roof.
    /// </summary>
    public class ExtrusionRoofWrapper
    {
        // To store the extrusion roof which will be edited in a PropertyGrid.
        private readonly ExtrusionRoof m_roof;

        /// <summary>
        ///     The construct of the ExtrusionRoofWrapper class.
        /// </summary>
        /// <param name="roof">The extrusion roof which will be edited in a PropertyGrid.</param>
        public ExtrusionRoofWrapper(ExtrusionRoof roof)
        {
            m_roof = roof;
        }

        /// <summary>
        ///     The reference plane of the extrusion roof.
        /// </summary>
        [Category("Constrains")]
        [Description("The reference plane of the extrusion roof.")]
        public string WorkPlane
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
                return para.AsString();
            }
        }

        /// <summary>
        ///     The extrusion start point of the extrusion roof.
        /// </summary>
        [Category("Constrains")]
        [DisplayName("Extrusion Start")]
        [Description(
            "The extrusion of a roof can extend in either direction along the reference plane. If the extrusion extends away from the plane, the start and end points are positive values. If the extrusion extends toward the plane, the start and end points are negative.")]
        public string ExtrusionStart
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.EXTRUSION_START_PARAM);
                return para.AsValueString();
            }
            set
            {
                var para = m_roof.get_Parameter(BuiltInParameter.EXTRUSION_START_PARAM);
                if (para.SetValueString(value) == false) throw new Exception("Invalid Input");
            }
        }

        /// <summary>
        ///     The extrusion end point of the extrusion roof.
        /// </summary>
        [Category("Constrains")]
        [DisplayName("Extrusion End")]
        [Description(
            "The extrusion of a roof can extend in either direction along the reference plane. If the extrusion extends away from the plane, the start and end points are positive values. If the extrusion extends toward the plane, the start and end points are negative.")]
        public string ExtrusionEnd
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.EXTRUSION_END_PARAM);
                return para.AsValueString();
            }
            set
            {
                var para = m_roof.get_Parameter(BuiltInParameter.EXTRUSION_END_PARAM);
                if (para.SetValueString(value) == false) throw new Exception("Invalid Input");
            }
        }

        /// <summary>
        ///     The reference level of the extrusion roof.
        /// </summary>
        [TypeConverter(typeof(LevelConverter))]
        [Category("Constrains")]
        [DisplayName("Reference Level")]
        [Description("The reference level of the extrusion roof.")]
        public Level ReferenceLevel
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                return LevelConverter.GetLevelById(para.AsElementId());
            }
            set
            {
                // update reference level
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                para.Set(value.Id);
            }
        }

        /// <summary>
        ///     The offset from the reference level of the extrusion roof.
        /// </summary>
        [Category("Constrains")]
        [DisplayName("Level Offset")]
        [Description("The offset from the reference level.")]
        public string LevelOffset
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_OFFSET_PARAM);
                return para.AsValueString();
            }
            set
            {
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_OFFSET_PARAM);
                if (para.SetValueString(value) == false) throw new Exception("Invalid Input");
            }
        }
    }
}
