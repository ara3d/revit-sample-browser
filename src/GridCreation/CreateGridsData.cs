// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Resources;
using Ara3D.RevitSampleBrowser.GridCreation.CS.Properties;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    /// <summary>
    ///     Base class of all grid creation data class
    /// </summary>
    public class CreateGridsData
    {
        protected static readonly ResourceManager ResManager = Resources.ResourceManager;

        /// <summary>
        ///     Application Creation object to create new elements
        /// </summary>
        protected Application AppCreator;

        /// <summary>
        ///     Document Creation object to create new elements
        /// </summary>
        protected Document DocCreator;

        /// <summary>
        ///     Array list contains all grid labels in current document
        /// </summary>
        private readonly ArrayList m_labelsList;

        protected readonly Autodesk.Revit.DB.Document RevitDoc;

        protected readonly ForgeTypeId m_unit;

        public CreateGridsData(UIApplication application, ArrayList labels)
        {
            RevitDoc = application.ActiveUIDocument.Document;
            AppCreator = application.Application.Create;
            DocCreator = application.ActiveUIDocument.Document.Create;
            m_labelsList = labels;
        }

        public CreateGridsData(UIApplication application, ArrayList labels, ForgeTypeId unit)
        {
            RevitDoc = application.ActiveUIDocument.Document;
            AppCreator = application.Application.Create;
            DocCreator = application.ActiveUIDocument.Document.Create;
            m_labelsList = labels;
            m_unit = unit;
        }

        public ForgeTypeId Unit => m_unit;

        public ArrayList LabelsList => m_labelsList;

        /// <summary>
        ///     Get the line to create grid according to the specified bubble location
        /// </summary>
        /// <param name="line">The original selected line</param>
        /// <param name="bubLoc">bubble location</param>
        /// <returns>The line to create grid</returns>
        protected Line TransformLine(Line line, BubbleLocation bubLoc)
        {
            Line lineToCreate;

            // Create grid according to the bubble location
            if (bubLoc == BubbleLocation.StartPoint)
            {
                lineToCreate = line;
            }
            else
            {
                var startPoint = line.GetEndPoint(1);
                var endPoint = line.GetEndPoint(0);
                lineToCreate = NewLine(startPoint, endPoint);
            }

            return lineToCreate;
        }

        protected Arc TransformArc(Arc arc, BubbleLocation bubLoc)
        {
            Arc arcToCreate;

            if (bubLoc == BubbleLocation.StartPoint)
            {
                arcToCreate = arc;
            }
            else
            {
                // Get start point, end point of the arc and the middle point on it 
                var startPoint = arc.GetEndPoint(0);
                var endPoint = arc.GetEndPoint(1);
                var clockwise = arc.Normal.Z == -1;

                // Get start angel and end angel of arc
                var startDegree = arc.GetEndParameter(0);
                var endDegree = arc.GetEndParameter(1);

                switch (clockwise)
                {
                    // Handle the case that the arc is clockwise
                    case true when startDegree > 0 && endDegree > 0:
                        startDegree = 2 * Values.Pi - startDegree;
                        endDegree = 2 * Values.Pi - endDegree;
                        break;
                    case true when startDegree < 0:
                    {
                        var temp = endDegree;
                        endDegree = -1 * startDegree;
                        startDegree = -1 * temp;
                        break;
                    }
                }

                var sumDegree = (startDegree + endDegree) / 2;
                while (sumDegree > 2 * Values.Pi) sumDegree -= 2 * Values.Pi;

                while (sumDegree < -2 * Values.Pi) sumDegree += 2 * Values.Pi;

                var midPoint = new XYZ(arc.Center.X + arc.Radius * Math.Cos(sumDegree),
                    arc.Center.Y + arc.Radius * Math.Sin(sumDegree), 0);

                arcToCreate = Arc.Create(endPoint, startPoint, midPoint);
            }

            return arcToCreate;
        }

        protected Arc TransformArc(XYZ origin, double radius, double startDegree, double endDegree,
            BubbleLocation bubLoc)
        {
            Arc arcToCreate;
            // Get start point and end point of the arc and the middle point on the arc
            var startPoint = new XYZ(origin.X + radius * Math.Cos(startDegree),
                origin.Y + radius * Math.Sin(startDegree), origin.Z);
            var midPoint = new XYZ(origin.X + radius * Math.Cos((startDegree + endDegree) / 2),
                origin.Y + radius * Math.Sin((startDegree + endDegree) / 2), origin.Z);
            var endPoint = new XYZ(origin.X + radius * Math.Cos(endDegree),
                origin.Y + radius * Math.Sin(endDegree), origin.Z);

            if (bubLoc == BubbleLocation.StartPoint)
                arcToCreate = Arc.Create(startPoint, endPoint, midPoint);
            else
                arcToCreate = Arc.Create(endPoint, startPoint, midPoint);

            return arcToCreate;
        }

        /// <summary>
        ///     Split a circle into the upper and lower parts
        /// </summary>
        /// <param name="arc">Arc to be split</param>
        /// <param name="upperArc">Upper arc of the circle</param>
        /// <param name="lowerArc">Lower arc of the circle</param>
        /// <param name="bubLoc">bubble location</param>
        protected void TransformCircle(Arc arc, ref Arc upperArc, ref Arc lowerArc, BubbleLocation bubLoc)
        {
            var center = arc.Center;
            var radius = arc.Radius;
            var xRightPoint = new XYZ(center.X + radius, center.Y, 0);
            var xLeftPoint = new XYZ(center.X - radius, center.Y, 0);
            var yUpperPoint = new XYZ(center.X, center.Y + radius, 0);
            var yLowerPoint = new XYZ(center.X, center.Y - radius, 0);
            if (bubLoc == BubbleLocation.StartPoint)
            {
                upperArc = Arc.Create(xRightPoint, xLeftPoint, yUpperPoint);
                lowerArc = Arc.Create(xLeftPoint, xRightPoint, yLowerPoint);
            }
            else
            {
                upperArc = Arc.Create(xLeftPoint, xRightPoint, yUpperPoint);
                lowerArc = Arc.Create(xRightPoint, xLeftPoint, yLowerPoint);
            }
        }

        protected Line NewLine(XYZ start, XYZ end)
        {
            return Line.CreateBound(start, end);
        }

        protected Grid NewGrid(Line line)
        {
            return Grid.Create(RevitDoc, line);
        }

        protected Grid NewGrid(Arc arc)
        {
            return Grid.Create(RevitDoc, arc);
        }

        protected Grid CreateLinearGrid(Line line)
        {
            return Grid.Create(RevitDoc, line);
        }

        protected void CreateGrids(CurveArray curves)
        {
            foreach (Curve c in curves)
            {
                var line = c as Line;
                var arc = c as Arc;

                if (line != null) Grid.Create(RevitDoc, line);

                if (arc != null) Grid.Create(RevitDoc, arc);
            }
        }

        /// <summary>
        ///     Add curve to curve array for batch creation
        /// </summary>
        /// <param name="curves">curve array stores all curves for batch creation</param>
        /// <param name="curve">curve to be added</param>
        public static void AddCurveForBatchCreation(ref CurveArray curves, Curve curve)
        {
            curves.Append(curve);
        }

        public static void ShowMessage(string message, string caption)
        {
            TaskDialog.Show(caption, message, TaskDialogCommonButtons.Ok);
        }
    }
}
