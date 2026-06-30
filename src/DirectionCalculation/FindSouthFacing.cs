// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    public class FindSouthFacingBase
    {
        protected Document Document { get; set; }

        protected bool IsSouthFacing(XYZ direction) => ViewHelper.IsSouthFacing(direction);

        protected XYZ TransformByProjectLocation(XYZ direction) =>
            ViewHelper.TransformByProjectLocation(Document, direction);

        protected void Write(string label, Curve curve) => EventLoggingHelper.Write(label, curve);

        protected void CloseFile() => EventLoggingHelper.CloseFile();
    }
}
