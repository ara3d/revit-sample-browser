// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateWallsUnderBeams.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateWallsUnderBeams : IExternalCommand
    {
        private const double Precision = 0.0000000001;

        private readonly ArrayList m_beamCollection;
        private string m_errorInformation;
        private Level m_level;
        private WallType m_selectedWallType;

        public CreateWallsUnderBeams()
        {
            WallTypes = [];
            m_beamCollection = [];
            IsSturctual = true;
        }

        public IList<WallType> WallTypes { get; private set; }

        public object SelectedWallType
        {
            set => m_selectedWallType = value as WallType;
        }

        public bool IsSturctual { get; set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var project = revit.ActiveUIDocument;

            // Find the selection of beams in Revit
            ElementSet selection = new();
            foreach (var elementId in project.Selection.GetElementIds())
            {
                selection.Insert(project.Document.GetElement(elementId));
            }

            foreach (Element e in selection)
            {
                if (e is FamilyInstance m)
                    if (StructuralType.Beam == m.StructuralType)
                        // Store all the beams the user selected in Revit
                        m_beamCollection.Add(e);
            }

            if (0 == m_beamCollection.Count)
            {
                message = "Can not find any beams.";
                return Result.Failed;
            }

            // Make sure all the beams have horizontal analytical line
            if (!CheckBeamHorizontal())
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // Search all the wall types in the Revit
            FilteredElementCollector filteredElementCollector = new(project.Document);
            filteredElementCollector.OfClass(typeof(WallType));
            WallTypes = filteredElementCollector.Cast<WallType>().ToList();

            // Show the dialog for the user select the wall style
            using (CreateWallsUnderBeamsForm displayForm = new(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return Result.Failed;
            }

            if (!BeginCreate(project.Document))
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private bool BeginCreate(Document project)
        {
            // Begin to create walls along and under each beam
            foreach (var beam in m_beamCollection)
            {
                // Get each selected beam.
                if (beam is not FamilyInstance m)
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                // the wall will be created using beam's model line as path.   
                if (m.Location is not LocationCurve curve)
                {
                    m_errorInformation = "The beam should have location curve.";
                    return false;
                }

                var beamCurve = curve.Curve;

                var levelId = m.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                m_level = project.GetElement(levelId) as Level;
                if (null == m_level)
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                Transaction t = new(project, Guid.NewGuid().GetHashCode().ToString());
                t.Start();
                var createdWall = Wall.Create(project, beamCurve, m_selectedWallType.Id,
                    m_level.Id, 10, 0, true, IsSturctual);
                if (null == createdWall)
                {
                    m_errorInformation = "Can not create the walls";
                    return false;
                }

                var offset = beamCurve.GetEndPoint(0).Z - m_level.Elevation;
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(levelId);
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(offset - (3000 / 304.8));
                createdWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(levelId);
                t.Commit();
            }

            return true;
        }

        private bool CheckBeamHorizontal()
        {
            foreach (var beam in m_beamCollection)
            {
                // And check if Z coordinate of start point and end point of the curve are same.
                var m = beam as FamilyInstance;
                var beamCurve = m.Location is LocationCurve curve ? curve.Curve : null;
                if (null == beamCurve)
                {
                    m_errorInformation = "The beam should have location curve.";
                    return false;
                }

                if (beamCurve.GetEndPoint(0).Z - beamCurve.GetEndPoint(1).Z is >= Precision or <= -Precision)
                {
                    m_errorInformation = "Please only select horizontal beams.";
                    return false;
                }
            }

            return true;
        }
    }
}
