// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofsManager
{
    /// <summary>
    ///     The kinds of roof than can be created.
    /// </summary>
    public enum CreateRoofKind
    {
        FootPrintRoof,
        ExtrusionRoof
    }

    /// <summary>
    ///     The RoofsManager is used to manage the operations between Revit and UI.
    /// </summary>
    public class RoofsManager
    {
        // To store a reference to the commandData.
        private readonly ExternalCommandData m_commandData;

        // To store a reference to the ExtrusionRoofManager to create extrusion roof.
        private readonly ExtrusionRoofManager m_extrusionRoofManager;

        // To store a reference to the FootPrintRoofManager to create footprint roof.
        private readonly FootPrintRoofManager m_footPrintRoofManager;

        // To store the levels info in the Revit.
        private readonly List<Level> m_levels;

        // roofs list

        // To store the footprint roof lines.

        // To store the profile lines.

        // Reference Plane for creating extrusion roof
        private readonly List<Autodesk.Revit.DB.ReferencePlane> m_referencePlanes;

        // To store the roof types info in the Revit.
        private List<RoofType> m_roofTypes;

        // To store the selected elements in the Revit
        private readonly Selection m_selection;

        // Transaction for manual mode
        private readonly Transaction m_transaction;

        // Current creating roof kind.
        public CreateRoofKind RoofKind;

        /// <summary>
        ///     The constructor of RoofsManager class.
        /// </summary>
        /// <param name="commandData">The ExternalCommandData</param>
        public RoofsManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_levels = new List<Level>();
            m_roofTypes = new List<RoofType>();
            m_referencePlanes = new List<Autodesk.Revit.DB.ReferencePlane>();

            FootPrint = new CurveArray();
            Profile = new CurveArray();

            m_footPrintRoofManager = new FootPrintRoofManager(commandData);
            m_extrusionRoofManager = new ExtrusionRoofManager(commandData);
            m_selection = commandData.Application.ActiveUIDocument.Selection;

            m_transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");

            RoofKind = CreateRoofKind.FootPrintRoof;
            Initialize();
        }

        /// <summary>
        ///     Get the Level elements.
        /// </summary>
        public ReadOnlyCollection<Level> Levels => new ReadOnlyCollection<Level>(m_levels);

        /// <summary>
        ///     Get the RoofTypes.
        /// </summary>
        public ReadOnlyCollection<RoofType> RoofTypes => new ReadOnlyCollection<RoofType>(m_roofTypes);

        /// <summary>
        ///     Get the RoofTypes.
        /// </summary>
        public ReadOnlyCollection<Autodesk.Revit.DB.ReferencePlane> ReferencePlanes =>
            new ReadOnlyCollection<Autodesk.Revit.DB.ReferencePlane>(m_referencePlanes);

        /// <summary>
        ///     Get all the footprint roofs in Revit.
        /// </summary>
        public ElementSet FootPrintRoofs { get; private set; }

        /// <summary>
        ///     Get all the extrusion roofs in Revit.
        /// </summary>
        public ElementSet ExtrusionRoofs { get; private set; }

        /// <summary>
        ///     Get the footprint roof lines.
        /// </summary>
        public CurveArray FootPrint { get; }

        /// <summary>
        ///     Get the extrusion profile lines.
        /// </summary>
        public CurveArray Profile { get; }

        /// <summary>
        ///     Initialize the data member
        /// </summary>
        private void Initialize()
        {
            var doc = m_commandData.Application.ActiveUIDocument.Document;
            var iter = new FilteredElementCollector(doc).OfClass(typeof(Level)).GetElementIterator();
            iter.Reset();
            while (iter.MoveNext()) m_levels.Add(iter.Current as Level);

            var filteredElementCollector = new FilteredElementCollector(doc);
            filteredElementCollector.OfClass(typeof(RoofType));
            m_roofTypes = filteredElementCollector.Cast<RoofType>().ToList();

            // FootPrint Roofs
            FootPrintRoofs = new ElementSet();
            iter = new FilteredElementCollector(doc).OfClass(typeof(FootPrintRoof)).GetElementIterator();
            iter.Reset();
            while (iter.MoveNext()) FootPrintRoofs.Insert(iter.Current as FootPrintRoof);

            // Extrusion Roofs
            ExtrusionRoofs = new ElementSet();
            iter = new FilteredElementCollector(doc).OfClass(typeof(ExtrusionRoof)).GetElementIterator();
            iter.Reset();
            while (iter.MoveNext()) ExtrusionRoofs.Insert(iter.Current as ExtrusionRoof);

            // Reference Planes
            iter = new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.ReferencePlane))
                .GetElementIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var plane = iter.Current as Autodesk.Revit.DB.ReferencePlane;
                // just use the vertical plane
                if (Math.Abs(plane.Normal.DotProduct(XYZ.BasisZ)) < 1.0e-09)
                {
                    if (plane.Name == "Reference Plane") plane.Name = "Reference Plane" + "(" + plane.Id + ")";
                    m_referencePlanes.Add(plane);
                }
            }
        }

        /// <summary>
        ///     Select elements in Revit to obtain the footprint roof lines or extrusion profile lines.
        /// </summary>
        /// <returns>A curve array to hold the footprint roof lines or extrusion profile lines.</returns>
        public CurveArray WindowSelect()
        {
            return RoofKind == CreateRoofKind.FootPrintRoof ? SelectFootPrint() : SelectProfile();
        }

        /// <summary>
        ///     Select elements in Revit to obtain the footprint roof lines.
        /// </summary>
        /// <returns>A curve array to hold the footprint roof lines.</returns>
        public CurveArray SelectFootPrint()
        {
            FootPrint.Clear();
            while (true)
            {
                var es = new ElementSet();
                foreach (var elementId in m_selection.GetElementIds())
                {
                    es.Insert(m_commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
                }

                es.Clear();
                IList<Element> selectResult;
                try
                {
                    selectResult = m_selection.PickElementsByRectangle();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }

                if (selectResult.Count != 0)
                {
                    foreach (var element in selectResult)
                    {
                        switch (element)
                        {
                            case Wall wall:
                            {
                                var wallCurve = wall.Location as LocationCurve;
                                FootPrint.Append(wallCurve.Curve);
                                continue;
                            }
                            case ModelCurve modelCurve:
                                FootPrint.Append(modelCurve.GeometryCurve);
                                break;
                        }
                    }

                    break;
                }

                var result = TaskDialog.Show("Warning",
                    "You should select a curve loop, or a wall loop, or loops combination \r\nof walls and curves to create a footprint roof.",
                    TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel);
                if (result == TaskDialogResult.Cancel) break;
            }

            return FootPrint;
        }

        /// <summary>
        ///     Create a footprint roof.
        /// </summary>
        /// <param name="level">The base level of the roof to be created.</param>
        /// <param name="roofType">The type of the newly created roof.</param>
        /// <returns>Return a new created footprint roof.</returns>
        public FootPrintRoof CreateFootPrintRoof(Level level, RoofType roofType)
        {
            var roof = m_footPrintRoofManager.CreateFootPrintRoof(FootPrint, level, roofType);
            if (roof != null) FootPrintRoofs.Insert(roof);
            return roof;
        }

        /// <summary>
        ///     Select elements in Revit to obtain the extrusion profile lines.
        /// </summary>
        /// <returns>A curve array to hold the extrusion profile lines.</returns>
        public CurveArray SelectProfile()
        {
            Profile.Clear();
            while (true)
            {
                m_selection.GetElementIds().Clear();
                IList<Element> selectResult;
                try
                {
                    selectResult = m_selection.PickElementsByRectangle();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }

                if (selectResult.Count != 0)
                {
                    foreach (var element in selectResult)
                    {
                        if (element is ModelCurve modelCurve)
                        {
                            Profile.Append(modelCurve.GeometryCurve);
                        }
                    }

                    break;
                }

                var result = TaskDialog.Show("Warning",
                    "You should select a  connected lines or arcs, \r\nnot closed in a loop to create extrusion roof.",
                    TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel);
                if (result == TaskDialogResult.Cancel) break;
            }

            return Profile;
        }

        /// <summary>
        ///     Create a extrusion roof.
        /// </summary>
        /// <param name="refPlane">The reference plane for the extrusion roof.</param>
        /// <param name="level">The reference level of the roof to be created.</param>
        /// <param name="roofType">The type of the newly created roof.</param>
        /// <param name="extrusionStart">The extrusion start point.</param>
        /// <param name="extrusionEnd">The extrusion end point.</param>
        /// <returns>Return a new created extrusion roof.</returns>
        public ExtrusionRoof CreateExtrusionRoof(Autodesk.Revit.DB.ReferencePlane refPlane,
            Level level, RoofType roofType, double extrusionStart, double extrusionEnd)
        {
            var roof = m_extrusionRoofManager.CreateExtrusionRoof(Profile, refPlane, level, roofType, extrusionStart,
                extrusionEnd);
            if (roof != null) ExtrusionRoofs.Insert(roof);
            return roof;
        }

        /// <summary>
        ///     Begin a transaction.
        /// </summary>
        /// <returns></returns>
        public TransactionStatus BeginTransaction()
        {
            if (m_transaction.GetStatus() == TransactionStatus.Started)
                TaskDialog.Show("Revit", "Transaction started already");
            return m_transaction.Start();
        }

        /// <summary>
        ///     Finish a transaction.
        /// </summary>
        /// <returns></returns>
        public TransactionStatus EndTransaction()
        {
            return m_transaction.Commit();
        }

        /// <summary>
        ///     Abort a transaction.
        /// </summary>
        public TransactionStatus AbortTransaction()
        {
            return m_transaction.RollBack();
        }
    }
}
