// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    public class EmptyTool : Tool
    {
        public EmptyTool()
        {
            Type = ToolType.None;
        }

        public override void Draw(Graphics graphic)
        {
        }
    }
}
