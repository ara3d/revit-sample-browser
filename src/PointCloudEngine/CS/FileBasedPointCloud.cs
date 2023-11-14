// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;

namespace Revit.SDK.Samples.CS.PointCloudEngine
{
    /// <summary>
    ///     An implementation for a file-based point cloud.
    /// </summary>
    /// <example>
    ///     The file format is based upon XML.  A sample XML looks like:
    ///     <code>
    ///    <PointCloud>
    ///             <Scale value="2.5" />
    ///             <Cell>
    ///                 <LowerLeft X="-30" Y="-30" Z="0" />
    ///                 <UpperRight X="30" Y="30" Z="200" />
    ///                 <Color value="#000000" />
    ///                 <Randomize value="True" />
    ///             </Cell>
    ///             <Cell>
    ///                 <LowerLeft X="-30" Y="-10" Z="10" />
    ///                 <UpperRight X="-29" Y="10" Z="150" />
    ///                 <Color value="#CC3300" />
    ///                 <Randomize value="False" />
    ///             </Cell>
    ///         </PointCloud>
    /// </code>
    ///     The scale value applies to the entire point cloud.  One or more cell values should be supplied,
    ///     with the coordinates of the opposing corners, a color, and an option whether or not to randomize
    ///     the generated points.
    /// </example>
    internal class FileBasedPointCloud : PointCloudAccessBase, IPointCloudAccess
    {
        private readonly string m_fileName;

        /// <summary>
        ///     Constructs a new XML-based point cloud access.
        /// </summary>
        /// <param name="fileName">The full path to the file.</param>
        public FileBasedPointCloud(string fileName)
        {
            m_fileName = fileName;

            Setup();
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.GetName().
        /// </summary>
        /// <returns>The name (the file name).</returns>
        public string GetName()
        {
            return m_fileName;
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.GetColorEncoding()
        /// </summary>
        /// <returns>The color encoding.</returns>
        public PointCloudColorEncoding GetColorEncoding()
        {
            return PointCloudColorEncoding.ABGR;
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.CreatePointSetIterator().
        /// </summary>
        /// <param name="rFilter">The filter.</param>
        /// <param name="viewId">The view id (unused).</param>
        /// <returns>The new iterator.</returns>
        public IPointSetIterator CreatePointSetIterator(PointCloudFilter rFilter, ElementId viewId)
        {
            return new PointCloudAccessBaseIterator(this, rFilter);
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.CreatePointSetIterator().
        /// </summary>
        /// <param name="rFilter">The filter.</param>
        /// <param name="density">The density.</param>
        /// <param name="viewId">The view id (unused).</param>
        /// <returns>The new iterator.</returns>
        public IPointSetIterator CreatePointSetIterator(PointCloudFilter rFilter, double density, ElementId viewId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.GetExtent().
        /// </summary>
        /// <returns>The extents of the point cloud.</returns>
        public Outline GetExtent()
        {
            return GetOutline();
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.GetOffset().
        /// </summary>
        /// <remarks>This method is not used by Revit and will be removed in a later pre-release build.</remarks>
        /// <returns>Zero.</returns>
        public XYZ GetOffset()
        {
            return XYZ.Zero;
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.GetUnitsToFeetConversionFactor().
        /// </summary>
        /// <returns>The scale.</returns>
        public double GetUnitsToFeetConversionFactor()
        {
            return GetScale();
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.ReadPoints().
        /// </summary>
        /// <param name="rFilter">The filter.</param>
        /// <param name="viewId">The view id (unused).</param>
        /// <param name="buffer">The point cloud buffer.</param>
        /// <param name="nBufferSize">The maximum number of points.</param>
        /// <returns>The number of points read.</returns>
        public int ReadPoints(PointCloudFilter rFilter, ElementId viewId, IntPtr buffer, int nBufferSize)
        {
            var read = ReadSomePoints(rFilter, buffer, nBufferSize, 0);

            return read;
        }

        /// <summary>
        ///     The implementation of IPointCloudAccess.Free().
        /// </summary>
        public void Free()
        {
        }

        /// <summary>
        ///     Sets up the file-based point cloud.
        /// </summary>
        private void Setup()
        {
            if (File.Exists(m_fileName))
            {
                var reader = new StreamReader(m_fileName);
                var xmlDoc = XDocument.Load(new XmlTextReader(reader));
                reader.Close();

                SetupFrom(xmlDoc.Element("PointCloud"));
            }
        }
    }
}
