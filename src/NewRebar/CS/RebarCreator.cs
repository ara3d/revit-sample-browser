// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Forms;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Geom;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS
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
        private readonly UIDocument m_rvtUiDoc;

        /// <summary>
        ///     Constructor, initialize fields and do some assert.
        ///     If the Assert throw exception, creation will fail.
        /// </summary>
        /// <param name="commandData">ExternalCommandData</param>
        public RebarCreator(ExternalCommandData commandData)
        {
            m_rvtUiDoc = commandData.Application.ActiveUIDocument;
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
            foreach (var elemId in m_rvtUiDoc.Selection.GetElementIds())
            {
                var elem = m_rvtUiDoc.Document.GetElement(elemId);
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
            var collector = new FilteredElementCollector(m_rvtUiDoc.Document, selectedIds);
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
            using (var form = new NewRebarForm(m_rvtUiDoc.Document))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    var barType = form.RebarBarType;
                    var barShape = form.RebarShape;

                    var profilePoints = m_geometryData.ProfilePoints;
                    var origin = profilePoints[0];
                    var yVec = profilePoints[1] - origin;
                    var xVec = profilePoints[3] - origin;

                    m_createdRebar = Rebar.CreateFromRebarShape(m_rvtUiDoc.Document, barShape, barType, m_rebarHost,
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
