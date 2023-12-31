// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    /// <summary>
    ///     all class that can be the datasource of propertygrid must inherit from it
    /// </summary>
    public interface IAreaReinData
    {
        /// <summary>
        ///     fill datasource with the data of AreaReinforcement
        /// </summary>
        /// <param name="areaRein"></param>
        /// <returns>is successful</returns>
        bool FillInData(AreaReinforcement areaRein);
    }
}
