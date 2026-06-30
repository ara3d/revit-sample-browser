// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public interface IWrapper
    {
        object Handle { get; }

        string Name { get; }
    }
}
