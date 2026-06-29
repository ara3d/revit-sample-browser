// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    /// <summary>
    ///     base class of sketch object to draw 2D geometry object
    /// </summary>
    public abstract class ObjectSketch
    {
        /// <summary>
        ///     reserve lines that form the profile
        /// </summary>
        protected readonly List<ObjectSketch> Objects = new List<ObjectSketch>();

        /// <summary>
        ///     pen to draw the object
        /// </summary>
        protected readonly Pen Pen = new Pen(Color.DarkGreen);

        /// <summary>
        ///     defines a local geometric transform
        /// </summary>
        protected Matrix Transform;

        /// <summary>
        ///     bounding box of the geometry object
        /// </summary>
        public RectangleF BoundingBox { get; protected set; }

        /// <summary>
        ///     geometric object draw itself
        /// </summary>
        /// <param name="g"></param>
        /// <param name="translate"></param>
        public abstract void Draw(Graphics g, Matrix translate);
    }
}
