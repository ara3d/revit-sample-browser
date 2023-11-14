// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Drawing;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw nothing
    /// </summary>
    internal class EmptyTool : ITool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public EmptyTool()
        {
            m_type = ToolType.None;
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
