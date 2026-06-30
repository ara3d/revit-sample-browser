// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public abstract class ObjectSketch
    {
        protected RectangleF m_boundingBox = new RectangleF();

        protected readonly List<ObjectSketch> Objects = new List<ObjectSketch>();

        protected readonly Pen Pen = new Pen(Color.DarkGreen);

        protected Matrix Transform;

        public RectangleF BoundingBox => m_boundingBox;

        public abstract void Draw(Graphics g, Matrix translate);
    }
}
