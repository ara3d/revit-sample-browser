// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.CurtainWallGrid.CS
{
    /// <summary>
    ///     the class manages the creation operation for the curtain wall
    /// </summary>
    public class WallGeometry
    {
        //store the start point of baseline (in Autodesk.Revit.DB.XYZ format)

        // store the end point of baseline (in PointD format)
        private PointD m_endPointD;
        // the document of this sample

        // the refferred drawing class for the curtain wall

        // the selected ViewPlan used for curtain wall creation

        // the selected wall type

        // store the start point of baseline (in PointD format)
        private PointD m_startPointD;

        /// <summary>
        ///     default constructor
        /// </summary>
        /// <param name="myDoc">
        ///     the document of the sample
        /// </param>
        public WallGeometry(MyDocument myDoc)
        {
            MyDocument = myDoc;
            Drawing = new WallDrawing(this);
        }

        //store the end point of baseline (in Autodesk.Revit.DB.XYZ format)

        /// <summary>
        ///     the document of this sample
        /// </summary>
        public MyDocument MyDocument { get; }

        /// <summary>
        ///     the refferred drawing class for the curtain wall
        /// </summary>
        public WallDrawing Drawing { get; }

        /// <summary>
        ///     the selected ViewPlan used for curtain wall creation
        /// </summary>
        public ViewPlan SelectedView { get; set; }

        /// <summary>
        ///     the selected wall type
        /// </summary>
        public WallType SelectedWallType { get; set; }

        /// <summary>
        ///     store the start point of baseline (in PointD format)
        /// </summary>
        public PointD StartPointD
        {
            get => m_startPointD;
            set => m_startPointD = value;
        }

        /// <summary>
        ///     Get start point of baseline
        /// </summary>
        public XYZ StartXyz { get; set; }

        /// <summary>
        ///     store the end point of baseline (in PointD format)
        /// </summary>
        public PointD EndPointD
        {
            get => m_endPointD;
            set => m_endPointD = value;
        }

        /// <summary>
        ///     Get end point of baseline
        /// </summary>
        public XYZ EndXyz { get; set; }

        /// <summary>
        ///     create the curtain wall to the active document of Revit
        /// </summary>
        /// <returns>
        ///     the created curtain wall
        /// </returns>
        public Wall CreateCurtainWall()
        {
            if (null == SelectedWallType || null == SelectedView) return null;

            //baseline
            //new baseline and transform coordinate on windows UI to Revit UI
            StartXyz = new XYZ(m_startPointD.X, m_startPointD.Y, 0);
            EndXyz = new XYZ(m_endPointD.X, m_endPointD.Y, 0);
            Line baseline = null;
            try
            {
                baseline = Line.CreateBound(StartXyz, EndXyz);
            }
            catch (ArgumentException)
            {
                TaskDialog.Show("Revit",
                    "The start point and the end point of the line are too close, please re-draw it.");
            }

            var act = new Transaction(MyDocument.Document);
            act.Start(Guid.NewGuid().GetHashCode().ToString());
            var wall = Wall.Create(MyDocument.Document, baseline, SelectedWallType.Id,
                SelectedView.GenLevel.Id, 20, 0, false, false);
            act.Commit();
            var act2 = new Transaction(MyDocument.Document);
            act2.Start(Guid.NewGuid().GetHashCode().ToString());
            MyDocument.UiDocument.ShowElements(wall);
            act2.Commit();
            return wall;
        }
    } // end of class
}
