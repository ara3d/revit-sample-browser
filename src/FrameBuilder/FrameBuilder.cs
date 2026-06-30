// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    using ModelElement = Element;

    public class FrameBuilder
    {
        private Application m_appCreator;
        private readonly FrameData m_data;
        private readonly Document m_docCreator;

        public FrameBuilder(FrameData data)
        {
            if (null == data)
                throw new ArgumentNullException(nameof(data),
                    "constructor FrameBuilder(FrameData data)'s parameter shouldn't be null ");
            m_data = data;

            m_appCreator = data.CommandData.Application.Application.Create;
            m_docCreator = data.CommandData.Application.ActiveUIDocument.Document.Create;
        }

        private FrameBuilder()
        {
        }

        public void CreateFraming()
        {
            var t = new Transaction(m_data.CommandData.Application.ActiveUIDocument.Document,
                Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            m_data.UpdateLevels();
            var frameElems = new List<FamilyInstance>();
            var matrixUv = XyzMath.CreateMatrix(m_data.XNumber, m_data.YNumber, m_data.Distance);

            for (var ii = 0; ii < m_data.FloorNumber; ii++)
            {
                var baseLevel = m_data.Levels.Values[ii];
                var topLevel = m_data.Levels.Values[ii + 1];

                var matrixXSize = matrixUv.GetLength(0);
                var matrixYSize = matrixUv.GetLength(1);

                foreach (var point2D in matrixUv)
                {
                    frameElems.Add(NewColumn(point2D, baseLevel, topLevel));
                }

                for (var j = 0; j < matrixYSize; j++)
                for (var i = 0; i < matrixXSize; i++)
                {
                    if (i != matrixXSize - 1) frameElems.Add(NewBeam(matrixUv[i, j], matrixUv[i + 1, j], topLevel));
                    if (j != matrixYSize - 1) frameElems.Add(NewBeam(matrixUv[i, j], matrixUv[i, j + 1], topLevel));
                }

                // Braces connect each column midpoint to the midpoint of adjoining beams.
                for (var j = 0; j < matrixYSize; j++)
                for (var i = 0; i < matrixXSize; i++)
                {
                    if (i != matrixXSize - 1)
                        frameElems.AddRange(
                            NewBraces(matrixUv[i, j], matrixUv[i + 1, j], baseLevel, topLevel));
                    if (j != matrixYSize - 1)
                        frameElems.AddRange(
                            NewBraces(matrixUv[i, j], matrixUv[i, j + 1], baseLevel, topLevel));
                }
            }

            MoveRotateFrame(frameElems);
            t.Commit();
        }

        private FamilyInstance NewColumn(UV point2D, Level baseLevel, Level topLevel)
        {
            var point = new XYZ(point2D.U, point2D.V, 0);

            if (!m_data.ColumnSymbol.IsActive)
                m_data.ColumnSymbol.Activate();
            var column =
                m_docCreator.NewFamilyInstance(point, m_data.ColumnSymbol, baseLevel, StructuralType.Column);

            SetParameter(column, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, topLevel.Id);
            SetParameter(column, BuiltInParameter.FAMILY_BASE_LEVEL_PARAM, baseLevel.Id);
            SetParameter(column, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, 0.0);
            SetParameter(column, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, 0.0);
            return column;
        }

        private FamilyInstance NewBeam(UV point2D1, UV point2D2, Level topLevel)
        {
            var height = topLevel.Elevation;
            var startPoint = new XYZ(point2D1.U, point2D1.V, height);
            var endPoint = new XYZ(point2D2.U, point2D2.V, height);
            var baseLine = Line.CreateBound(startPoint, endPoint);
            if (!m_data.BeamSymbol.IsActive)
                m_data.BeamSymbol.Activate();
            var beam =
                m_docCreator.NewFamilyInstance(baseLine, m_data.BeamSymbol, topLevel, StructuralType.Beam);
            return beam;
        }

        private List<FamilyInstance> NewBraces(UV point2D1, UV point2D2, Level baseLevel, Level topLevel)
        {
            var topHeight = topLevel.Elevation;
            var baseHeight = baseLevel.Elevation;
            var middleElevation = (topHeight + baseHeight) / 2;
            var startPoint = new XYZ(point2D1.U, point2D1.V, middleElevation);
            var endPoint = new XYZ(point2D2.U, point2D2.V, middleElevation);
            var middlePoint = new XYZ((point2D1.U + point2D2.U) / 2, (point2D1.V + point2D2.V) / 2, topHeight);

            var firstBaseLine = Line.CreateBound(startPoint, middlePoint);
            if (!m_data.BraceSymbol.IsActive)
                m_data.BraceSymbol.Activate();
            var firstBrace =
                m_docCreator.NewFamilyInstance(firstBaseLine, m_data.BraceSymbol, topLevel, StructuralType.Brace);

            var secondBaseLine = Line.CreateBound(endPoint, middlePoint);
            var secondBrace =
                m_docCreator.NewFamilyInstance(secondBaseLine, m_data.BraceSymbol, topLevel, StructuralType.Brace);
            var result = new List<FamilyInstance>
            {
                firstBrace,
                secondBrace
            };
            return result;
        }

        private bool SetParameter(ModelElement elem,
            BuiltInParameter builtInPara, ElementId value)
        {
            var para = elem.get_Parameter(builtInPara);
            if (null != para && para.StorageType == StorageType.ElementId && !para.IsReadOnly)
            {
                var result = para.Set(value);
                return result;
            }

            return false;
        }

        private bool SetParameter(ModelElement elem,
            BuiltInParameter builtInPara, double value)
        {
            var para = elem.get_Parameter(builtInPara);
            if (null != para && para.StorageType == StorageType.Double && !para.IsReadOnly)
            {
                var result = para.Set(value);
                return result;
            }

            return false;
        }

        private void MoveRotateFrame(List<FamilyInstance> frameElems)
        {
            var doc = m_data.CommandData.Application.ActiveUIDocument.Document;
            foreach (var elem in frameElems)
            {
                MoveElement(doc, elem, m_data.FrameOrigin);
                RotateElement(m_data.CommandData.Application, elem, m_data.FrameOrigin, m_data.FrameOriginAngle);
            }
        }

        private void MoveElement(Autodesk.Revit.DB.Document doc, Element elem, UV translation2D)
        {
            var translation3D = new XYZ(translation2D.U, translation2D.V, 0.0);
            ElementTransformUtils.MoveElement(doc, elem.Id, translation3D);
        }

        // ElementTransformUtils.RotateElement expects angle in radians.
        private void RotateElement(UIApplication app, Element elem, UV center, double angle)
        {
            var axisPnt1 = new XYZ(center.U, center.V, 0.0);
            var axisPnt2 = new XYZ(center.U, center.V, 1.0);
            var axis = Line.CreateBound(axisPnt1, axisPnt2);
            ElementTransformUtils.RotateElement(elem.Document, elem.Id, axis, angle);
        }
    }
}
