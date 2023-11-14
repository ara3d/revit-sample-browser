// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB.PointClouds;

namespace RevitMultiSample.CS.PointCloudEngine
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

        /// <summary>
        ///     Constructs a new instance of the engine.
        /// </summary>
        /// <param name="type">The type of point cloud served by this engine instance.</param>
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
            switch (m_type)
            {
                case PointCloudEngineType.RandomizedPoints:
                    return new PredefinedPointCloud(identifier, true);
                case PointCloudEngineType.FileBased:
                    return new FileBasedPointCloud(identifier);
                case PointCloudEngineType.Predefined:
                default:
                    return new PredefinedPointCloud(identifier);
            }
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
