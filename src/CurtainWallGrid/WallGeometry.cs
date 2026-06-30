// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    /// <summary>
    ///     the class manages the creation operation for the curtain wall
    /// </summary>
    public class WallGeometry
    {

        private PointD m_endPointD;

        // the refferred drawing class for the curtain wall

        // the selected ViewPlan used for curtain wall creation

        // the selected wall type

        private PointD m_startPointD;

        public WallGeometry(MyDocument myDoc)
        {
            MyDocument = myDoc;
            Drawing = new WallDrawing(this);
        }


        public MyDocument MyDocument { get; }

        /// <summary>
        ///     the refferred drawing class for the curtain wall
        /// </summary>
        public WallDrawing Drawing { get; }

        /// <summary>
        ///     the selected ViewPlan used for curtain wall creation
        /// </summary>
        public ViewPlan SelectedView { get; set; }

        public WallType SelectedWallType { get; set; }

        public PointD StartPointD
        {
            get => m_startPointD;
            set => m_startPointD = value;
        }

        public XYZ StartXyz { get; set; }

        public PointD EndPointD
        {
            get => m_endPointD;
            set => m_endPointD = value;
        }

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
