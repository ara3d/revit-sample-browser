// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    public interface IAreaReinData
    {
        bool FillInData(AreaReinforcement areaRein);
    }
}
