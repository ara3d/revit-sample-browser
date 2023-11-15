// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw nothing
    /// </summary>
    internal class EmptyTool : Tool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public EmptyTool()
        {
            Type = ToolType.None;
        }

        /// <summary>
        ///     Draw nothing
        /// </summary>
        /// <param name="graphic">Graphics object</param>
        public override void Draw(Graphics graphic)
        {
        }
    }
}
