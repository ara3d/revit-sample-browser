// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;

namespace Revit.SDK.Samples.FrameBuilder.CS
{
    using ModelElement = Element;

    /// <summary>
    ///     create columns, beams and braces to create framing
    /// </summary>
    public class FrameBuilder
    {
        private Application m_appCreator; // buffer of API object
        private readonly FrameData m_data; // necessary data to create frame
        private readonly Document m_docCreator; // buffer of API object

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="data">data necessary to initialize object</param>
        public FrameBuilder(FrameData data)
        {
            // initialize members
            if (null == data)
                throw new ArgumentNullException("data",
                    "constructor FrameBuilder(FrameData data)'s parameter shouldn't be null ");
            m_data = data;

            m_appCreator = data.CommandData.Application.Application.Create;
            m_docCreator = data.CommandData.Application.ActiveUIDocument.Document.Create;
        }

        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private FrameBuilder()
        {
        }

        /// <summary>
        ///     create framing according to FramingData
        /// </summary>
        /// <returns>columns, beams and braces</returns>
        public void CreateFraming()
        {
            var t = new Transaction(m_data.CommandData.Application.ActiveUIDocument.Document,
                Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            m_data.UpdateLevels();
            var frameElems = new List<FamilyInstance>();
            var matrixUV = CreateMatrix(m_data.XNumber, m_data.YNumber, m_data.Distance);

            // iterate levels from lower one to higher one by one according to FloorNumber
            for (var ii = 0; ii < m_data.FloorNumber; ii++)
            {
                var baseLevel = m_data.Levels.Values[ii];
                var topLevel = m_data.Levels.Values[ii + 1];

                var matrixXSize = matrixUV.GetLength(0); //length of matrix's x range
                var matrixYSize = matrixUV.GetLength(1); //length of matrix's y range

                // insert columns in an array format according to the calculated matrix
                foreach (var point2D in matrixUV) frameElems.Add(NewColumn(point2D, baseLevel, topLevel));

                // insert beams between the tops of each adjacent column in the X and Y direction
                for (var j = 0; j < matrixYSize; j++)
                for (var i = 0; i < matrixXSize; i++)
                {
                    //create beams in x direction
                    if (i != matrixXSize - 1) frameElems.Add(NewBeam(matrixUV[i, j], matrixUV[i + 1, j], topLevel));
                    //create beams in y direction
                    if (j != matrixYSize - 1) frameElems.Add(NewBeam(matrixUV[i, j], matrixUV[i, j + 1], topLevel));
                }

                // insert braces between the mid point of each column 
                // and the mid point of each adjoining beam
                for (var j = 0; j < matrixYSize; j++)
                for (var i = 0; i < matrixXSize; i++)
                {
                    //create braces in x direction
                    if (i != matrixXSize - 1)
                        frameElems.AddRange(
                            NewBraces(matrixUV[i, j], matrixUV[i + 1, j], baseLevel, topLevel));
                    //create braces in y direction
                    if (j != matrixYSize - 1)
                        frameElems.AddRange(
                            NewBraces(matrixUV[i, j], matrixUV[i, j + 1], baseLevel, topLevel));
                }
            }

            MoveRotateFrame(frameElems);
            t.Commit();
        }

        /// <summary>
        ///     create a 2D matrix of coordinates to form an array format
        /// </summary>
        /// <param name="xNumber">number of Columns in the X direction</param>
        /// <param name="yNumber">number of Columns in the Y direction</param>
        /// <param name="distance">distance between columns</param>
        private static UV[,] CreateMatrix(int xNumber, int yNumber, double distance)
        {
            var result = new UV[xNumber, yNumber];

            for (var i = 0; i < xNumber; i++)
            for (var j = 0; j < yNumber; j++)
                result[i, j] = new UV(i * distance, j * distance);
            return result;
        }

        /// <summary>
        ///     create column of certain type in given position
        /// </summary>
        /// <param name="point2D">2D coordinate of the column</param>
        /// <param name="columnType">specified type of the column</param>
        /// <param name="baseLevel">base level of the column</param>
        /// <param name="topLevel">top level of the colunm</param>
        private FamilyInstance NewColumn(UV point2D, Level baseLevel, Level topLevel)
        {
            //create column of specified type with certain level and start point 
            var point = new XYZ(point2D.U, point2D.V, 0);

            if (!m_data.ColumnSymbol.IsActive)
                m_data.ColumnSymbol.Activate();
            var column =
                m_docCreator.NewFamilyInstance(point, m_data.ColumnSymbol, baseLevel, StructuralType.Column);

            //set baselevel & toplevel of the column            
            SetParameter(column, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, topLevel.Id);
            SetParameter(column, BuiltInParameter.FAMILY_BASE_LEVEL_PARAM, baseLevel.Id);
            SetParameter(column, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, 0.0);
            SetParameter(column, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, 0.0);
            return column;
        }

        /// <summary>
        ///     create beam of certain type in given position
        /// </summary>
        /// <param name="point2D1">first point of the location line in 2D</param>
        /// <param name="point2D2">second point of the location line in 2D</param>
        /// <param name="baseLevel">base level of the beam</param>
        /// <param name="topLevel">top level of the beam</param>
        /// <returns>nothing</returns>
        private FamilyInstance NewBeam(UV point2D1, UV point2D2, Level topLevel)
        {
            // calculate the start point and end point of Beam's location line in 3D
            var height = topLevel.Elevation;
            var startPoint = new XYZ(point2D1.U, point2D1.V, height);
            var endPoint = new XYZ(point2D2.U, point2D2.V, height);
            // create Beam and set its location
            var baseLine = Line.CreateBound(startPoint, endPoint);
            if (!m_data.BeamSymbol.IsActive)
                m_data.BeamSymbol.Activate();
            var beam =
                m_docCreator.NewFamilyInstance(baseLine, m_data.BeamSymbol, topLevel, StructuralType.Beam);
            return beam;
        }

        /// <summary>
        ///     create 2 braces between the mid point of 2 column and the mid point of adjoining beam
        /// </summary>
        /// <param name="point2D1">first point of the location line in 2D</param>
        /// <param name="point2D2">second point of the location line in 2D</param>
        /// <param name="baseLevel">the base level of the brace</param>
        /// <param name="topLevel">the top level of the brace</param>
        private List<FamilyInstance> NewBraces(UV point2D1, UV point2D2, Level baseLevel, Level topLevel)
        {
            // calculate the start point and end point of the location lines of two braces
            var topHeight = topLevel.Elevation;
            var baseHeight = baseLevel.Elevation;
            var middleElevation = (topHeight + baseHeight) / 2;
            var startPoint = new XYZ(point2D1.U, point2D1.V, middleElevation);
            var endPoint = new XYZ(point2D2.U, point2D2.V, middleElevation);
            var middlePoint = new XYZ((point2D1.U + point2D2.U) / 2, (point2D1.V + point2D2.V) / 2, topHeight);

            // create two brace; then set their location line and reference level
            var firstBaseLine = Line.CreateBound(startPoint, middlePoint);
            if (!m_data.BraceSymbol.IsActive)
                m_data.BraceSymbol.Activate();
            var firstBrace =
                m_docCreator.NewFamilyInstance(firstBaseLine, m_data.BraceSymbol, topLevel, StructuralType.Brace);

            var secondBaseLine = Line.CreateBound(endPoint, middlePoint);
            var secondBrace =
                m_docCreator.NewFamilyInstance(secondBaseLine, m_data.BraceSymbol, topLevel, StructuralType.Brace);
            var result = new List<FamilyInstance>();
            result.Add(firstBrace);
            result.Add(secondBrace);
            return result;
        }

        /// <summary>
        ///     set parameter whose storage type is Autodesk.Revit.DB.ElementId
        /// </summary>
        /// <param name="elem">Element has parameter</param>
        /// <param name="builtInPara">BuiltInParameter to find parameter</param>
        /// <param name="value">value to set</param>
        /// <returns>is successful</returns>
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

        /// <summary>
        ///     set parameter whose storage type is double
        /// </summary>
        /// <param name="elem">Element has parameter</param>
        /// <param name="builtInPara">BuiltInParameter to find parameter</param>
        /// <param name="value">value to set</param>
        /// <returns>is successful</returns>
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


        /// <summary>
        ///     move and rotate the Frame
        /// </summary>
        /// <param name="frameElems">columns, beams and braces included in frame</param>
        private void MoveRotateFrame(List<FamilyInstance> frameElems)
        {
            var doc = m_data.CommandData.Application.ActiveUIDocument.Document;
            foreach (var elem in frameElems)
            {
                MoveElement(doc, elem, m_data.FrameOrigin);
                RotateElement(m_data.CommandData.Application, elem, m_data.FrameOrigin, m_data.FrameOriginAngle);
            }
        }

        /// <summary>
        ///     move an element in horizontal plane
        /// </summary>
        /// <param name="elem">element to be moved</param>
        /// <param name="translation2D">the 2D vector by which the element is to be moved</param>
        /// <returns>is successful</returns>
        private void MoveElement(Autodesk.Revit.DB.Document doc, Element elem, UV translation2D)
        {
            var translation3D = new XYZ(translation2D.U, translation2D.V, 0.0);
            ElementTransformUtils.MoveElement(doc, elem.Id, translation3D);
        }

        /// <summary>
        ///     rotate an element a specified number of degrees
        ///     around a given center in horizontal plane
        /// </summary>
        /// <param name="elem">element to be rotated</param>
        /// <param name="center">the center of rotation</param>
        /// <param name="angle">
        ///     the number of degrees, in radians,
        ///     by which the element is to be rotated around the specified axis
        /// </param>
        /// <returns>is successful</returns>
        private void RotateElement(UIApplication app, Element elem, UV center, double angle)
        {
            var axisPnt1 = new XYZ(center.U, center.V, 0.0);
            var axisPnt2 = new XYZ(center.U, center.V, 1.0);
            var axis = Line.CreateBound(axisPnt1, axisPnt2);
            //axis.
            ElementTransformUtils.RotateElement(elem.Document, elem.Id, axis, angle);
        }
    }
}
