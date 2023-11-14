// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Autodesk.Revit.DB;
using Revit.SDK.Samples.NewRoof.CS;

namespace Revit.SDK.Samples.NewRoof.RoofForms.CS
{
    /// <summary>
    ///     The Util class is used to translate Revit coordination to windows coordination.
    /// </summary>
    public class Util
    {
        /// <summary>
        ///     Translate a Revit 3D point to a windows 2D point according the boundingbox.
        /// </summary>
        /// <param name="pointXyz">A Revit 3D point</param>
        /// <param name="boundingbox">The boundingbox of the roof whose footprint lines will be displayed in GDI.</param>
        /// <returns>A windows 2D point.</returns>
        public static PointF Translate(XYZ pointXyz, BoundingBoxXYZ boundingbox)
        {
            var centerX = (boundingbox.Min.X + boundingbox.Max.X) / 2;
            var centerY = (boundingbox.Min.Y + boundingbox.Max.Y) / 2;
            return new PointF((float)(pointXyz.X - centerX), -(float)(pointXyz.Y - centerY));
        }
    }

    /// <summary>
    ///     The FootPrintRoofLine class is used to edit the foot print data of a footprint roof.
    /// </summary>
    public class FootPrintRoofLine
    {
        // To store the model curve data which the foot print data stand for.
        // To store the boundingbox of the roof
        private readonly BoundingBoxXYZ m_boundingbox;

        // To store the footprint roof which the foot print data belong to.
        private readonly FootPrintRoof m_roof;

        /// <summary>
        ///     The construct of the FootPrintRoofLine class.
        /// </summary>
        /// <param name="roof">The footprint roof which the foot print data belong to.</param>
        /// <param name="curve">The model curve data which the foot print data stand for.</param>
        public FootPrintRoofLine(FootPrintRoof roof, ModelCurve curve)
        {
            m_roof = roof;
            ModelCurve = curve;
            m_boundingbox = m_roof.get_BoundingBox(Command.ActiveView);
        }

        /// <summary>
        ///     Get the model curve data which the foot print data stand for.
        /// </summary>
        [Browsable(false)]
        public ModelCurve ModelCurve { get; }

        /// <summary>
        ///     Get the id value of the model curve.
        /// </summary>
        [Browsable(false)]
        public ElementId Id => ModelCurve.Id;

        /// <summary>
        ///     Get the name of the model curve.
        /// </summary>
        [Browsable(false)]
        public string Name => ModelCurve.Name;

        /// <summary>
        ///     Get/Set the slope definition of a model curve of the roof.
        /// </summary>
        [Description("The slope definition of the FootPrintRoof line.")]
        public bool DefinesSlope
        {
            get => m_roof.get_DefinesSlope(ModelCurve);
            set => m_roof.set_DefinesSlope(ModelCurve, value);
        }

        /// <summary>
        ///     Get/Set the slope angle of the FootPrintRoof line..
        /// </summary>
        [Description("The slope angle of the FootPrintRoof line.")]
        public double SlopeAngle
        {
            get => m_roof.get_SlopeAngle(ModelCurve);
            set => m_roof.set_SlopeAngle(ModelCurve, value);
        }

        /// <summary>
        ///     Get/Set the offset of the FootPrintRoof line.
        /// </summary>
        [Description("The offset of the FootPrintRoof line.")]
        public double Offset
        {
            get => m_roof.get_Offset(ModelCurve);
            set => m_roof.set_Offset(ModelCurve, value);
        }

        /// <summary>
        ///     Get/Set the overhang value of the FootPrintRoof line if the roof is created by picked wall.
        /// </summary>
        [Description("The overhang value of the FootPrintRoof line if the roof is created by picked wall.")]
        public double Overhang
        {
            get => m_roof.get_Overhang(ModelCurve);
            set => m_roof.set_Overhang(ModelCurve, value);
        }

        /// <summary>
        ///     Get/Set ExtendIntoWall value whether you want the overhang to be measured from the core of the wall or not.
        /// </summary>
        [Description("whether you want the overhang to be measured from the core of the wall or not.")]
        public bool ExtendIntoWall
        {
            get => m_roof.get_ExtendIntoWall(ModelCurve);
            set => m_roof.set_ExtendIntoWall(ModelCurve, value);
        }

        /// <summary>
        ///     Draw the footprint line in GDI.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        public void Draw(Graphics graphics, Pen pen)
        {
            var curve = ModelCurve.GeometryCurve;
            DrawCurve(graphics, pen, curve);
        }

        /// <summary>
        ///     Draw the curve in GDI.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="curve"></param>
        private void DrawCurve(Graphics graphics, Pen pen, Curve curve)
        {
            var poinsts = new List<PointF>();
            foreach (var point in curve.Tessellate()) poinsts.Add(Util.Translate(point, m_boundingbox));
            graphics.DrawCurve(pen, poinsts.ToArray());
        }
    }

    /// <summary>
    ///     The FootPrintRoofWrapper class is use to edit a footprint roof in a PropertyGrid.
    ///     It contains a footprint roof.
    /// </summary>
    public class FootPrintRoofWrapper
    {
        // To store the footprint line data of the roof which will be edited.
        private FootPrintRoofLine m_footPrintLine;

        // To store the footprint roof which will be edited in a PropertyGrid.
        private readonly FootPrintRoof m_roof;

        // To store the footprint lines data of the roof.
        private readonly List<FootPrintRoofLine> m_roofLines;

        /// <summary>
        ///     The construct of the FootPrintRoofWrapper class.
        /// </summary>
        /// <param name="roof">The footprint roof which will be edited in a PropertyGrid.</param>
        public FootPrintRoofWrapper(FootPrintRoof roof)
        {
            m_roof = roof;
            m_roofLines = new List<FootPrintRoofLine>();
            var curveloops = m_roof.GetProfiles();

            foreach (ModelCurveArray curveloop in curveloops)
            foreach (ModelCurve curve in curveloop)
                m_roofLines.Add(new FootPrintRoofLine(m_roof, curve));

            FootPrintRoofLineConverter.SetStandardValues(m_roofLines);
            m_footPrintLine = m_roofLines[0];

            Boundingbox = m_roof.get_BoundingBox(Command.ActiveView);
        }

        /// <summary>
        ///     Get the bounding box of the roof.
        /// </summary>
        [Browsable(false)]
        public BoundingBoxXYZ Boundingbox { get; }

        /// <summary>
        ///     Get/Set the current footprint roof line which will be edited in the PropertyGrid.
        /// </summary>
        [TypeConverter(typeof(FootPrintRoofLineConverter))]
        [Category("Footprint Roof Line Information")]
        public FootPrintRoofLine FootPrintLine
        {
            get => m_footPrintLine;
            set
            {
                m_footPrintLine = value;
                OnFootPrintRoofLineChanged(this, new EventArgs());
            }
        }

        /// <summary>
        ///     The base level of the footprint roof.
        /// </summary>
        [TypeConverter(typeof(LevelConverter))]
        [Category("Constrains")]
        [DisplayName("Base Level")]
        public Level BaseLevel
        {
            get
            {
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                return LevelConverter.GetLevelById(para.AsElementId());
            }
            set
            {
                // update base level
                var para = m_roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                para.Set(value.Id);
            }
        }

        /// <summary>
        ///     The eave cutter type of the footprint roof.
        /// </summary>
        [Category("Construction")]
        [DisplayName("Rafter Cut")]
        [Description("The eave cutter type of the footprint roof.")]
        public EaveCutterType EaveCutterType
        {
            get => m_roof.EaveCuts;
            set => m_roof.EaveCuts = value;
        }

        /// <summary>
        ///     Get the footprint roof lines data.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<FootPrintRoofLine> FootPrintRoofLines =>
            new ReadOnlyCollection<FootPrintRoofLine>(m_roofLines);

        // To store the boundingbox of the roof

        public event EventHandler OnFootPrintRoofLineChanged;

        /// <summary>
        ///     Draw the footprint lines.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="displayPen">A display pen.</param>
        /// <param name="highlightPen">A highlight pen.</param>
        public void DrawFootPrint(Graphics graphics, Pen displayPen, Pen highlightPen)
        {
            foreach (var line in m_roofLines)
                if (line.Id == m_footPrintLine.Id)
                    line.Draw(graphics, highlightPen);
                else
                    line.Draw(graphics, displayPen);
        }
    }
}
