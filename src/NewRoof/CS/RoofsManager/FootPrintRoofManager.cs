// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofsManager
{
    /// <summary>
    ///     The FootPrintRoofManager class is to manage the creation of the footprint roof.
    /// </summary>
    internal class FootPrintRoofManager
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
        public FootPrintRoofManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_creationDoc = m_commandData.Application.ActiveUIDocument.Document.Create;
            m_creationApp = m_commandData.Application.Application.Create;
        }

        /// <summary>
        ///     Create a footprint roof.
        /// </summary>
        /// <param name="footPrint">The footprint is a curve loop, or a wall loop, or loops combined of walls and curves</param>
        /// <param name="level">The base level of the roof to be created.</param>
        /// <param name="roofType">The type of the newly created roof.</param>
        /// <returns>Return a new created footprint roof.</returns>
        public FootPrintRoof CreateFootPrintRoof(CurveArray footPrint, Level level, RoofType roofType)
        {
            FootPrintRoof footprintRoof = null;
            var createRoofTransaction =
                new Transaction(m_commandData.Application.ActiveUIDocument.Document, "FootPrintRoof");
            createRoofTransaction.Start();
            try
            {
                new ModelCurveArray();
                footprintRoof = m_creationDoc.NewFootPrintRoof(footPrint, level, roofType, out _);
                createRoofTransaction.Commit();
            }
            catch (Exception e)
            {
                createRoofTransaction.RollBack();
                throw e;
            }

            return footprintRoof;
        }
    }
}
