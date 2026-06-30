// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.PointClouds;

namespace Ara3D.RevitSampleBrowser.PointCloudEngine.CS
{
    /// <summary>
    ///     The type of engine.
    /// </summary>
    /// <remarks>
    ///     Because the same engine implementation is used for all types of engines in this sample, a member of this enumerated
    ///     type
    ///     is used to determine the logic necessary to create the IPointCloudAccess instance.
    /// </remarks>
    public enum PointCloudEngineType
    {
        /// <summary>
        ///     A predefined point cloud engine (non-randomized).
        /// </summary>
        Predefined,

        /// <summary>
        ///     A predefined point cloud engine (randomized).
        /// </summary>
        RandomizedPoints,

        /// <summary>
        ///     A file based point cloud engine.
        /// </summary>
        FileBased
    }

    /// <summary>
    ///     An implementation of IPointCloudEngine used by all the custom engines in this sample.
    /// </summary>
    public class BasicPointCloudEngine : IPointCloudEngine
    {
        private readonly PointCloudEngineType m_type;

        public BasicPointCloudEngine(PointCloudEngineType type)
        {
            m_type = type;
        }

        /// <summary>
        ///     Implementation of IPointCloudEngine.CreatePointCloudAccess().
        /// </summary>
        /// <param name="identifier">The identifier (or file name) for the desired point cloud.</param>
        /// <returns>The IPointCloudAccess implementation serving this point cloud.</returns>
        public IPointCloudAccess CreatePointCloudAccess(string identifier)
        {
            return m_type switch
            {
                PointCloudEngineType.RandomizedPoints => new PredefinedPointCloud(identifier, true),
                PointCloudEngineType.FileBased => new FileBasedPointCloud(identifier),
                _ => new PredefinedPointCloud(identifier),
            };
        }

        /// <summary>
        ///     Implementation of IPointCloudEngine.Free().
        /// </summary>
        public void Free()
        {
            //Nothing to do
        }
    }
}
