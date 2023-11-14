// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.ProjectInfo.CS
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
