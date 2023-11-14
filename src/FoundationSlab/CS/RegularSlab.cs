// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.FoundationSlab.CS
{
    /// <summary>
    ///     A class of regular slab.
    /// </summary>
    public class RegularSlab
    {
        private readonly Level m_level; // The level of the slab.
        private readonly ElementType m_type; // The type of the slab.

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="floor">The floor object.</param>
        /// <param name="floorProfile">The floor's profile.</param>
        /// <param name="bBox">The floor's bounding box.</param>
        public RegularSlab(Floor floor, CurveArray floorProfile, BoundingBoxXYZ bBox)
        {
            // Get floor's properties.
            if (null != floor)
            {
                // Get floor's Mark property.
                var markPara = floor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                if (null != markPara) Mark = markPara.AsString();

                m_level = floor.Document.GetElement(floor.LevelId) as Level; // Get floor's level.
                m_type = floor.Document.GetElement(floor.GetTypeId()) as ElementType; // Get floor's type.
                Id = floor.Id; // Get floor's Id.
                Profile = floorProfile; // Get floor's profile.
            }

            // Create an octagonal profile for the floor according to it's bounding box.
            if (null != bBox) CreateOctagonProfile(bBox.Min, bBox.Max);
        }

        /// <summary>
        ///     Selected property.
        /// </summary>
        public bool Selected { get; set; } = true;

        /// <summary>
        ///     Mark property.
        /// </summary>
        public string Mark { get; }

        /// <summary>
        ///     LevelName property.
        /// </summary>
        public string LevelName => m_level.Name;

        /// <summary>
        ///     SlabTypeName property.
        /// </summary>
        public string SlabTypeName => m_type.Name;

        /// <summary>
        ///     Id property.
        /// </summary>
        public ElementId Id { get; }

        /// <summary>
        ///     Profile property.
        /// </summary>
        public CurveArray Profile { get; }

        /// <summary>
        ///     OctagonalProfile property.
        /// </summary>
        public CurveArray OctagonalProfile { get; private set; }

        /// <summary>
        ///     BBox property.
        /// </summary>
        public BoundingBoxXYZ BBox { get; private set; }

        /// <summary>
        ///     Create an octagonal profile.
        /// </summary>
        /// <param name="min">The min point of the floor's bounding box.</param>
        /// <param name="max">The max point of the floor's bounding box.</param>
        /// <returns>The bool value suggests successful or not.</returns>
        private bool CreateOctagonProfile(XYZ min, XYZ max)
        {
            // Calculate the x/y offset.
            var xOffset = Math.Abs(max.Y - min.Y) / 8;
            var yOffset = Math.Abs(max.X - min.X) / 8;
            var z = max.Z;

            // Calculate the eight points of the octagon.
            var points = new XYZ[8];
            points[0] = new XYZ(min.X, min.Y, z);
            points[1] = new XYZ((min.X + max.X) / 2, min.Y - yOffset, z);
            points[2] = new XYZ(max.X, min.Y, z);
            points[3] = new XYZ(max.X + xOffset, (min.Y + max.Y) / 2, z);
            points[4] = new XYZ(max.X, max.Y, z);
            points[5] = new XYZ((min.X + max.X) / 2, max.Y + yOffset, z);
            points[6] = new XYZ(min.X, max.Y, z);
            points[7] = new XYZ(min.X - xOffset, (min.Y + max.Y) / 2, z);

            // Get the octagonal profile.
            OctagonalProfile = new CurveArray();
            for (var i = 0; i < 8; i++)
            {
                Line line;
                if (7 == i)
                    line = Line.CreateBound(points[i], points[0]);
                else
                    line = Line.CreateBound(points[i], points[i + 1]);
                OctagonalProfile.Append(line);
            }

            // Get the octagonal profile's bounding box.
            var newMin = new XYZ(min.X - xOffset, min.Y - yOffset, z);
            var newMax = new XYZ(max.X + xOffset, max.Y + yOffset, z);
            BBox = new BoundingBoxXYZ();
            BBox.Min = newMin;
            BBox.Max = newMax;

            return true;
        }
    }
}
