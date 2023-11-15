// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    ///     wrapper interface
    /// </summary>
    public interface IWrapper
    {
        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        object Handle { get; }

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        string Name { get; }
    }
}
