// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using Autodesk.Revit.DB;

namespace Revit.Samples.DirectionCalculation
{
    /// <summary>
    ///     Base class for "Find South Facing..." utilities.
    /// </summary>
    public class FindSouthFacingBase
    {
        private TextWriter m_writer;
        private Document m_document;

        protected Document Document
        {
            get => m_document;
            set => m_document = value;
        }

        /// <summary>
        ///     Identifies if a particular direction is "south-facing".  This means within a range of -45 degrees to 45 degrees
        ///     to the south vector (the negative Y axis).
        /// </summary>
        /// <param name="direction">The normalized direction to test.</param>
        /// <returns>True if the vector is in the designated range.</returns>
        protected bool IsSouthFacing(XYZ direction)
        {
            var angleToSouth = direction.AngleTo(-XYZ.BasisY);

            return Math.Abs(angleToSouth) < Math.PI / 4;
        }

        /// <summary>
        ///     Transform a direction vector by the rotation angle of the ActiveProjectLocation.
        /// </summary>
        /// <param name="direction">The normalized direction.</param>
        /// <returns>The transformed location.</returns>
        protected XYZ TransformByProjectLocation(XYZ direction)
        {
            // Obtain the active project location's position.
            var position = Document.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            // Construct a rotation transform from the position angle.
            /* If I cared about transforming points as well as vectors, 
             I would need to concatenate two different transformations:
                //Obtain a rotation transform for the angle about true north
                Transform rotationTransform = Transform.get_Rotation(
                  XYZ.Zero, XYZ.BasisZ, pna );

                //Obtain a translation vector for the offsets
                XYZ translationVector = new XYZ(projectPosition.EastWest, projectPosition.NorthSouth, projectPosition.Elevation);

                Transform translationTransform = Transform.CreateTranslation(translationVector);

                //Combine the transforms into one.
                Transform finalTransform = translationTransform.Multiply(rotationTransform);
            */
            var transform = Transform.CreateRotation(XYZ.BasisZ, position.Angle);
            // Rotate the input direction by the transform
            var rotatedDirection = transform.OfVector(direction);
            return rotatedDirection;
        }

        /// <summary>
        ///     Debugging aid.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="curve"></param>
        protected void Write(string label, Curve curve)
        {
            if (m_writer == null)
                m_writer = new StreamWriter(@"c:\Directions.txt");
            var start = curve.GetEndPoint(0);
            var end = curve.GetEndPoint(1);

            m_writer.WriteLine(label + " {0} {1}", XyzToString(start), XyzToString(end));
        }

        /// <summary>
        ///     Debugging aid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private string XyzToString(XYZ point)
        {
            return "( " + point.X + ", " + point.Y + ", " + point.Z + ")";
        }

        /// <summary>
        ///     Debugging aid.
        /// </summary>
        protected void CloseFile()
        {
            m_writer?.Close();
        }
    }
}
