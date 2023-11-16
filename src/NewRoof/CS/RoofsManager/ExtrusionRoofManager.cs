// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofsManager
{
    /// <summary>
    ///     The ExtrusionRoofManager class is to manage the creation of the Extrusion roof.
    /// </summary>
    internal class ExtrusionRoofManager
    {
        // To store a reference to the commandData
        private readonly ExternalCommandData m_commandData;

        // To store a reference to the creation application
        private Application m_creationApp;

        // To store a reference to the creation document
        private readonly Document m_creationDoc;

        /// <summary>
        ///     The construct of ExtrusionRoofManager class.
        /// </summary>
        /// <param name="commandData">A reference to the commandData.</param>
        public ExtrusionRoofManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_creationDoc = m_commandData.Application.ActiveUIDocument.Document.Create;
            m_creationApp = m_commandData.Application.Application.Create;
        }

        /// <summary>
        ///     Create a extrusion roof.
        /// </summary>
        /// <param name="profile">The profile combined of straight lines and arcs.</param>
        /// <param name="refPlane">The reference plane for the extrusion roof.</param>
        /// <param name="level">The reference level of the roof to be created.</param>
        /// <param name="roofType">The type of the newly created roof.</param>
        /// <param name="extrusionStart">The extrusion start point.</param>
        /// <param name="extrusionEnd">The extrusion end point.</param>
        /// <returns>Return a new created extrusion roof.</returns>
        public ExtrusionRoof CreateExtrusionRoof(CurveArray profile, Autodesk.Revit.DB.ReferencePlane refPlane,
            Level level, RoofType roofType,
            double extrusionStart, double extrusionEnd)
        {
            ExtrusionRoof extrusionRoof = null;
            var createRoofTransaction =
                new Transaction(m_commandData.Application.ActiveUIDocument.Document, "ExtrusionRoof");
            createRoofTransaction.Start();
            try
            {
                extrusionRoof =
                    m_creationDoc.NewExtrusionRoof(profile, refPlane, level, roofType, extrusionStart, extrusionEnd);
                createRoofTransaction.Commit();
            }
            catch (Exception e)
            {
                createRoofTransaction.RollBack();
                throw e;
            }

            return extrusionRoof;
        }
    }
}
