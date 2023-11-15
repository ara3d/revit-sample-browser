// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    /// <summary>
    ///     The factory to create the corresponding FrameReinMaker, such as BeamFramReinMaker.
    /// </summary>
    internal class FrameReinMakerFactory
    {
        // Private members
        private readonly ExternalCommandData m_commandData; // the ExternalCommandData reference
        private FamilyInstance m_hostObject; // the host object

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        public FrameReinMakerFactory(ExternalCommandData commandData)
        {
            m_commandData = commandData;

            if (!GetHostObject()) throw new Exception("Please select a beam or column.");
        }

        /// <summary>
        ///     check the condition of host object and see whether the rebars can be placed on
        /// </summary>
        /// <returns></returns>
        public bool AssertData()
        {
            // judge whether is any rebar exist in the beam or column
            return new FilteredElementCollector(m_commandData.Application.ActiveUIDocument.Document)
                .OfClass(typeof(Rebar))
                .Cast<Rebar>().Count(x => x.GetHostId() == m_hostObject.Id) == 0;
        }

        /// <summary>
        ///     The main method which create the corresponding FrameReinMaker according to
        ///     the host object type, and invoke Run() method to create reinforcement rebars
        /// </summary>
        /// <returns>true if the creation is successful, otherwise false</returns>
        public bool Work()
        {
            // define an IFrameReinMaker interface to create reinforcement rebars
            IFrameReinMaker maker = null;

            // create FrameReinMaker instance according to host object type
            switch (m_hostObject.StructuralType)
            {
                case StructuralType.Beam: // if host object is a beam
                    maker = new BeamFramReinMaker(m_commandData, m_hostObject);
                    break;
                case StructuralType.Column: // if host object is a column
                    maker = new ColumnFramReinMaker(m_commandData, m_hostObject);
                    break;
            }

            // invoke Run() method to do the reinforcement creation
            maker.Run();

            return true;
        }

        /// <summary>
        ///     Get the selected element as the host object, also check if the selected element is expected host object
        /// </summary>
        /// <returns>true if get the selected element, otherwise false.</returns>
        private bool GetHostObject()
        {
            var selectedIds = new List<ElementId>();
            foreach (var elemId in m_commandData.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                var elem = m_commandData.Application.ActiveUIDocument.Document.GetElement(elemId);
                selectedIds.Add(elem.Id);
            }

            if (selectedIds.Count != 1)
                return false;
            //
            // Construct filters to find expected host object: 
            // . Host should be Beam/Column structural type.
            // . and it's material type should be Concrete
            // . and it should be FamilyInstance
            //
            // Structural type filters firstly
            var stFilter = new LogicalOrFilter(
                new ElementStructuralTypeFilter(StructuralType.Beam),
                new ElementStructuralTypeFilter(StructuralType.Column));
            // StructuralMaterialType should be Concrete
            var hostFilter = new LogicalAndFilter(stFilter,
                new StructuralMaterialTypeFilter(StructuralMaterialType.Concrete));
            //
            // Expected host object
            var collector =
                new FilteredElementCollector(m_commandData.Application.ActiveUIDocument.Document, selectedIds);
            m_hostObject = collector
                .OfClass(typeof(FamilyInstance)) // FamilyInstance
                .WherePasses(hostFilter) // Filters
                .FirstElement() as FamilyInstance;
            return null != m_hostObject;
        }
    }
}
