// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    public class FindSouthFacingBase
    {
        protected Document Document { get; set; }

        protected bool IsSouthFacing(XYZ direction)
        {
            return ViewHelper.IsSouthFacing(direction);
        }

        protected XYZ TransformByProjectLocation(XYZ direction)
        {
            return ViewHelper.TransformByProjectLocation(Document, direction);
        }

        protected void Write(string label, Curve curve)
        {
            EventLoggingHelper.Write(label, curve);
        }

        protected void CloseFile()
        {
            EventLoggingHelper.CloseFile();
        }
    }
}
