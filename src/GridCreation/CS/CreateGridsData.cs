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
        /// <summary>
        ///     Resource manager
        /// </summary>
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

        /// <summary>
        ///     The active document of Revit
        /// </summary>
        protected readonly Autodesk.Revit.DB.Document RevitDoc;

        /// <summary>
        ///     Current display unit type
        /// </summary>
        protected readonly ForgeTypeId m_unit;

        /// <summary>
        ///     Constructor without display unit type
        /// </summary>
        /// <param name="application">Revit application</param>
        /// <param name="labels">All existing labels in Revit's document</param>
        public CreateGridsData(UIApplication application, ArrayList labels)
        {
            RevitDoc = application.ActiveUIDocument.Document;
            AppCreator = application.Application.Create;
            DocCreator = application.ActiveUIDocument.Document.Create;
            m_labelsList = labels;
        }

        /// <summary>
        ///     Constructor with display unit type
        /// </summary>
        /// <param name="application">Revit application</param>
        /// <param name="labels">All existing labels in Revit's document</param>
        /// <param name="unit">Current length display unit type</param>
        public CreateGridsData(UIApplication application, ArrayList labels, ForgeTypeId unit)
        {
            RevitDoc = application.ActiveUIDocument.Document;
            AppCreator = application.Application.Create;
            DocCreator = application.ActiveUIDocument.Document.Create;
            m_labelsList = labels;
            m_unit = unit;
        }

        /// <summary>
        ///     Current display unit type
        /// </summary>
        public ForgeTypeId Unit => m_unit;

        /// <summary>
        ///     Get array list contains all grid labels in current document
        /// </summary>
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

        /// <summary>
        ///     Get the arc to create grid according to the specified bubble location
        /// </summary>
        /// <param name="arc">The original selected line</param>
        /// <param name="bubLoc">bubble location</param>
        /// <returns>The arc to create grid</returns>
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

        /// <summary>
        ///     Get the arc to create grid according to the specified bubble location
        /// </summary>
        /// <param name="origin">Arc grid's origin</param>
        /// <param name="radius">Arc grid's radius</param>
        /// <param name="startDegree">Arc grid's start degree</param>
        /// <param name="endDegree">Arc grid's end degree</param>
        /// <param name="bubLoc">Arc grid's Bubble location</param>
        /// <returns>The expected arc to create grid</returns>
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

        /// <summary>
        ///     Create a new bound line
        /// </summary>
        /// <param name="start">start point of line</param>
        /// <param name="end">end point of line</param>
        /// <returns></returns>
        protected Line NewLine(XYZ start, XYZ end)
        {
            return Line.CreateBound(start, end);
        }

        /// <summary>
        ///     Create a grid with a line
        /// </summary>
        /// <param name="line">Line to create grid</param>
        /// <returns>Newly created grid</returns>
        protected Grid NewGrid(Line line)
        {
            return Grid.Create(RevitDoc, line);
        }

        /// <summary>
        ///     Create a grid with an arc
        /// </summary>
        /// <param name="arc">Arc to create grid</param>
        /// <returns>Newly created grid</returns>
        protected Grid NewGrid(Arc arc)
        {
            return Grid.Create(RevitDoc, arc);
        }

        /// <summary>
        ///     Create linear grid
        /// </summary>
        /// <param name="line">The linear curve to be transferred to grid</param>
        /// <returns>The newly created grid</returns>
        protected Grid CreateLinearGrid(Line line)
        {
            return Grid.Create(RevitDoc, line);
        }

        /// <summary>
        ///     Create batch of grids with curves
        /// </summary>
        /// <param name="curves">Curves used to create grids</param>
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

        /// <summary>
        ///     Show a message box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="caption">title of message box</param>
        public static void ShowMessage(string message, string caption)
        {
            TaskDialog.Show(caption, message, TaskDialogCommonButtons.Ok);
        }
    }
}
