//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewRebar.CS
{
    /// <summary>
    ///     This class wraps the creation of Rebar. Its "Execute" method shows
    ///     the main dialog for user and after that a Rebar will be created
    ///     if user click OK button on the main dialog.
    /// </summary>
    internal class RebarCreator
    {
        /// <summary>
        ///     Newly created Rebar.
        /// </summary>
        private Rebar m_createdRebar;

        /// <summary>
        ///     GeometrySupport object.
        /// </summary>
        private GeometrySupport m_geometryData;

        /// <summary>
        ///     Revit FamilyInstance object, it will be the host of Rebar.
        /// </summary>
        private FamilyInstance m_rebarHost;

        /// <summary>
        ///     Revit UI document
        /// </summary>
        private readonly UIDocument m_rvtUIDoc;

        /// <summary>
        ///     Constructor, initialize fields and do some assert.
        ///     If the Assert throw exception, creation will fail.
        /// </summary>
        /// <param name="commandData">ExternalCommandData</param>
        public RebarCreator(ExternalCommandData commandData)
        {
            m_rvtUIDoc = commandData.Application.ActiveUIDocument;
            Assert();
        }

        /// <summary>
        ///     Do some check for the selection elements, includes geometry check.
        ///     If the data doesn't meet our need, Exception will be thrown.
        /// </summary>
        private void Assert()
        {
            // Reserve all element ids for following iteration
            var selectedIds = new List<ElementId>();
            foreach (var elemId in m_rvtUIDoc.Selection.GetElementIds())
            {
                var elem = m_rvtUIDoc.Document.GetElement(elemId);
                selectedIds.Add(elem.Id);
            }

            if (selectedIds.Count == 0)
                throw new Exception("Please select a concrete beam or column to create rebar.");

            //
            // Construct filter to find expected rebar host
            // Structural type filters firstly
            var stFilter = new LogicalOrFilter(
                new ElementStructuralTypeFilter(StructuralType.Beam),
                new ElementStructuralTypeFilter(StructuralType.Column));
            // + StructuralMaterial 
            var hostFilter = new LogicalAndFilter(stFilter,
                new StructuralMaterialTypeFilter(StructuralMaterialType.Concrete));
            // Expected rebar host: it should be family instance
            var collector = new FilteredElementCollector(m_rvtUIDoc.Document, selectedIds);
            var rebarHost =
                collector.OfClass(typeof(FamilyInstance)).WherePasses(hostFilter).FirstElement() as FamilyInstance;
            // Make sure the selected beam or column is rectangular.
            try
            {
                m_geometryData = new GeometrySupport(rebarHost);
            }
            catch
            {
                throw new Exception("Please select a beam or column in rectangular shape.");
            }

            m_rebarHost = rebarHost;

            // Judge the rebar host is a valid host.
            var rebarHostData = RebarHostData.GetRebarHostData(rebarHost);
            if (rebarHostData == null || !rebarHostData.IsValidHost())
                throw new Exception("The selected element is not a valid rebar host.");

            // Make sure the selected beam or column doesn't contain any rebar.
            if (rebarHostData.GetRebarsInHost().Count > 0)
                throw new Exception("Please select a beam or a column which doesn't contain any rebar.");
        }

        /// <summary>
        ///     Present the main dialog for user to prepare the parameters for Rebar creation,
        ///     and after that if user click the OK button, a new Rebar will be created.
        /// </summary>
        public void Execute()
        {
            using (var form = new NewRebarForm(m_rvtUIDoc.Document))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    var barType = form.RebarBarType;
                    var barShape = form.RebarShape;

                    var profilePoints = m_geometryData.ProfilePoints;
                    var origin = profilePoints[0];
                    var yVec = profilePoints[1] - origin;
                    var xVec = profilePoints[3] - origin;

                    m_createdRebar = Rebar.CreateFromRebarShape(m_rvtUIDoc.Document, barShape, barType, m_rebarHost,
                        origin, xVec, yVec);

                    LayoutRebar();
                }
            }
        }

        /// <summary>
        ///     Move and Scale the  created Rebar to specified box.
        /// </summary>
        private void LayoutRebar()
        {
            var profilePoints = m_geometryData.OffsetPoints(0.1);
            var origin = profilePoints[0];
            var yVec = profilePoints[1] - origin;
            var xVec = profilePoints[3] - origin;

            var arcDef =
                (m_createdRebar.Document.GetElement(m_createdRebar.GetShapeId()) as RebarShape)
                .GetRebarShapeDefinition() as RebarShapeDefinitionByArc;

            var rebarShapeDrivenAccessor = m_createdRebar.GetShapeDrivenAccessor();
            if (arcDef != null && arcDef.Type == RebarShapeDefinitionByArcType.Spiral)
            {
                rebarShapeDrivenAccessor.ScaleToBoxFor3D(origin, xVec, yVec, 10.0);
                rebarShapeDrivenAccessor.Height = m_geometryData.DrivingLength - 0.1;
                rebarShapeDrivenAccessor.Pitch = 0.1;
                rebarShapeDrivenAccessor.BaseFinishingTurns = 3;
                rebarShapeDrivenAccessor.TopFinishingTurns = 3;
            }
            else
            {
                rebarShapeDrivenAccessor.ScaleToBox(origin, xVec, yVec);
                var barSpacing = 0.1;
                var barNum = (int)(m_geometryData.DrivingLength / barSpacing);
                rebarShapeDrivenAccessor.SetLayoutAsNumberWithSpacing(
                    barNum, barSpacing, true, true, true);
            }
        }
    }
}