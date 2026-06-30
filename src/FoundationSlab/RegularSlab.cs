// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FoundationSlab.CS
{
    public class RegularSlab
    {
        private readonly Level m_level;
        private readonly ElementType m_type;

        public RegularSlab(Floor floor, CurveArray floorProfile, BoundingBoxXYZ bBox)
        {
            if (null != floor)
            {
                var markPara = floor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                if (null != markPara) Mark = markPara.AsString();

                m_level = floor.Document.GetElement(floor.LevelId) as Level;
                m_type = floor.Document.GetElement(floor.GetTypeId()) as ElementType;
                Id = floor.Id;
                Profile = floorProfile;
            }

            if (null != bBox) CreateOctagonProfile(bBox.Min, bBox.Max);
        }

        public bool Selected { get; set; } = true;

        public string Mark { get; }

        public string LevelName => m_level.Name;

        public string SlabTypeName => m_type.Name;

        public ElementId Id { get; }

        public CurveArray Profile { get; }

        public CurveArray OctagonalProfile { get; private set; }

        public BoundingBoxXYZ BBox { get; private set; }

        private bool CreateOctagonProfile(XYZ min, XYZ max)
        {
            var xOffset = Math.Abs(max.Y - min.Y) / 8;
            var yOffset = Math.Abs(max.X - min.X) / 8;
            var z = max.Z;

            var points = new XYZ[8];
            points[0] = new XYZ(min.X, min.Y, z);
            points[1] = new XYZ((min.X + max.X) / 2, min.Y - yOffset, z);
            points[2] = new XYZ(max.X, min.Y, z);
            points[3] = new XYZ(max.X + xOffset, (min.Y + max.Y) / 2, z);
            points[4] = new XYZ(max.X, max.Y, z);
            points[5] = new XYZ((min.X + max.X) / 2, max.Y + yOffset, z);
            points[6] = new XYZ(min.X, max.Y, z);
            points[7] = new XYZ(min.X - xOffset, (min.Y + max.Y) / 2, z);

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

            var newMin = new XYZ(min.X - xOffset, min.Y - yOffset, z);
            var newMax = new XYZ(max.X + xOffset, max.Y + yOffset, z);
            BBox = new BoundingBoxXYZ
            {
                Min = newMin,
                Max = newMax
            };

            return true;
        }
    }
}
