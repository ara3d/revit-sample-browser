// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

/*
 * This sample demonstrates how to use DirectContext3D to draw graphics. 
 * This file defines a DirectContext3D server.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

namespace Revit.SDK.Samples.DuplicateGraphics.CS
{
    internal class RevitElementDrawingServer : IDirectContext3DServer
    {
        private RenderingPassBufferStorage m_edgeBufferStorage;

        private readonly Element m_element;

        private readonly Guid m_guid;

        private RenderingPassBufferStorage m_nonTransparentFaceBufferStorage;
        private readonly XYZ m_offset;
        private RenderingPassBufferStorage m_transparentFaceBufferStorage;
        private readonly UIDocument m_uiDocument;

        public RevitElementDrawingServer(UIDocument uiDoc, Element elem, XYZ offset)
        {
            m_guid = Guid.NewGuid();

            m_uiDocument = uiDoc;
            m_element = elem;
            m_offset = offset;
        }

        public Document Document => m_uiDocument != null ? m_uiDocument.Document : null;

        public Guid GetServerId()
        {
            return m_guid;
        }

        public string GetVendorId()
        {
            return "ADSK";
        }

        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.DirectContext3DService;
        }

        public string GetName()
        {
            return "Revit Element Drawing Server";
        }

        public string GetDescription()
        {
            return "Duplicates graphics from a Revit element.";
        }

        // Corresponds to functionality that is not used in this sample.
        public string GetApplicationId()
        {
            return "";
        }

        // Corresponds to functionality that is not used in this sample.
        public string GetSourceId()
        {
            return "";
        }

        // Corresponds to functionality that is not used in this sample.
        public bool UsesHandles()
        {
            return false;
        }

        // Tests whether this server should be invoked for the given view.
        // The server only wants to be invoked for 3D views that are part of the document that contains the element in m_element.
        public bool CanExecute(View view)
        {
            if (!m_element.IsValidObject)
                return false;
            if (view.ViewType != ViewType.ThreeD)
                return false;

            var doc = view.Document;
            var otherDoc = m_element.Document;
            return doc.Equals(otherDoc);
        }

        // Reports a bounding box of the geometry that this server submits for drawing.
        public Outline GetBoundingBox(View view)
        {
            try
            {
                var boundingBox = m_element.get_BoundingBox(null);

                var outline = new Outline(boundingBox.Min + m_offset, boundingBox.Max + m_offset);

                return outline;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        // Indicates that this server will submit geometry during the rendering pass for transparent geometry.
        public bool UseInTransparentPass(View view)
        {
            return true;
        }

        // Submits the geometry for rendering.
        public void RenderScene(View view, DisplayStyle displayStyle)
        {
            try
            {
                // Populate geometry buffers if they are not initialized or need updating.
                if (m_nonTransparentFaceBufferStorage == null ||
                    m_nonTransparentFaceBufferStorage.NeedsUpdate(displayStyle) ||
                    m_transparentFaceBufferStorage == null ||
                    m_transparentFaceBufferStorage.NeedsUpdate(displayStyle) ||
                    m_edgeBufferStorage == null || m_edgeBufferStorage.NeedsUpdate(displayStyle))
                {
                    var options = new Options();
                    var geomElem = m_element.get_Geometry(options);

                    CreateBufferStorageForElement(geomElem, displayStyle);
                }

                // Submit a subset of the geometry for drawing. Determine what geometry should be submitted based on
                // the type of the rendering pass (opaque or transparent) and DisplayStyle (wireframe or shaded).

                // If the server is requested to submit transparent geometry, DrawContext().IsTransparentPass()
                // will indicate that the current rendering pass is for transparent objects.
                var faceBufferStorage = DrawContext.IsTransparentPass()
                    ? m_transparentFaceBufferStorage
                    : m_nonTransparentFaceBufferStorage;

                // Conditionally submit triangle primitives (for non-wireframe views).
                if (displayStyle != DisplayStyle.Wireframe &&
                    faceBufferStorage.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(faceBufferStorage.VertexBuffer,
                        faceBufferStorage.VertexBufferCount,
                        faceBufferStorage.IndexBuffer,
                        faceBufferStorage.IndexBufferCount,
                        faceBufferStorage.VertexFormat,
                        faceBufferStorage.EffectInstance, PrimitiveType.TriangleList, 0,
                        faceBufferStorage.PrimitiveCount);

                // Conditionally submit line segment primitives.
                if (displayStyle != DisplayStyle.Shading &&
                    m_edgeBufferStorage.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(m_edgeBufferStorage.VertexBuffer,
                        m_edgeBufferStorage.VertexBufferCount,
                        m_edgeBufferStorage.IndexBuffer,
                        m_edgeBufferStorage.IndexBufferCount,
                        m_edgeBufferStorage.VertexFormat,
                        m_edgeBufferStorage.EffectInstance, PrimitiveType.LineList, 0,
                        m_edgeBufferStorage.PrimitiveCount);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        // Initialize and populate buffers that hold graphics primitives, set up related parameters that are needed for drawing.
        private void CreateBufferStorageForElement(GeometryElement geomElem, DisplayStyle displayStyle)
        {
            var allSolids = new List<Solid>();

            foreach (var geomObj in geomElem)
                if (geomObj is Solid solid)
                {
                    if (solid.Volume > 1e-06)
                        allSolids.Add(solid);
                }

            m_nonTransparentFaceBufferStorage = new RenderingPassBufferStorage(displayStyle);
            m_transparentFaceBufferStorage = new RenderingPassBufferStorage(displayStyle);
            m_edgeBufferStorage = new RenderingPassBufferStorage(displayStyle);

            // Collect primitives (and associated rendering parameters, such as colors) from faces and edges.
            foreach (var solid in allSolids)
            {
                foreach (Face face in solid.Faces)
                    if (face.Area > 1e-06)
                    {
                        var mesh = face.Triangulate();

                        var materialId = face.MaterialElementId;
                        var isTransparent = false;
                        var cwt = new ColorWithTransparency(127, 127, 127, 0);
                        if (materialId != ElementId.InvalidElementId)
                        {
                            var material = m_element.Document.GetElement(materialId) as Material;

                            var color = material.Color;
                            var transparency0To100 = material.Transparency;
                            var transparency0To255 = (uint)(transparency0To100 / 100f * 255f);

                            cwt = new ColorWithTransparency(color.Red, color.Green, color.Blue, transparency0To255);
                            if (transparency0To255 > 0) isTransparent = true;
                        }

                        var env = face.GetBoundingBox();
                        var center = 0.5 * (env.Min + env.Max);
                        var normal = face.ComputeNormal(center);

                        var meshInfo = new MeshInfo(mesh, normal, cwt);

                        if (isTransparent)
                        {
                            m_transparentFaceBufferStorage.Meshes.Add(meshInfo);
                            m_transparentFaceBufferStorage.VertexBufferCount += mesh.Vertices.Count;
                            m_transparentFaceBufferStorage.PrimitiveCount += mesh.NumTriangles;
                        }
                        else
                        {
                            m_nonTransparentFaceBufferStorage.Meshes.Add(meshInfo);
                            m_nonTransparentFaceBufferStorage.VertexBufferCount += mesh.Vertices.Count;
                            m_nonTransparentFaceBufferStorage.PrimitiveCount += mesh.NumTriangles;
                        }
                    }

                foreach (Edge edge in solid.Edges)
                    // if (edge.Length > 1e-06)
                {
                    var xyzs = edge.Tessellate();

                    m_edgeBufferStorage.VertexBufferCount += xyzs.Count;
                    m_edgeBufferStorage.PrimitiveCount += xyzs.Count - 1;
                    m_edgeBufferStorage.EdgeXyZs.Add(xyzs);
                }
            }

            // Fill out buffers with primitives based on the intermediate information about faces and edges.
            ProcessFaces(m_nonTransparentFaceBufferStorage);
            ProcessFaces(m_transparentFaceBufferStorage);
            ProcessEdges(m_edgeBufferStorage);
        }

        // Create and populate a pair of vertex and index buffers. Also update parameters associated with the format of the vertices.
        private void ProcessFaces(RenderingPassBufferStorage bufferStorage)
        {
            var meshes = bufferStorage.Meshes;
            var numVerticesInMeshesBefore = new List<int>();
            if (meshes.Count == 0) return;

            var useNormals = bufferStorage.DisplayStyle == DisplayStyle.Shading ||
                             bufferStorage.DisplayStyle == DisplayStyle.ShadingWithEdges;

            // Vertex attributes are stored sequentially in vertex buffers. The attributes can include position, normal vector, and color.
            // All vertices within a vertex buffer must have the same format. Possible formats are enumerated by VertexFormatBits.
            // Vertex format also determines the type of rendering effect that can be used with the vertex buffer. In this sample,
            // the color is always encoded in the vertex attributes.

            bufferStorage.FormatBits =
                useNormals ? VertexFormatBits.PositionNormalColored : VertexFormatBits.PositionColored;

            // The format of the vertices determines the size of the vertex buffer.
            var vertexBufferSizeInFloats =
                (useNormals ? VertexPositionNormalColored.GetSizeInFloats() : VertexPositionColored.GetSizeInFloats()) *
                bufferStorage.VertexBufferCount;
            numVerticesInMeshesBefore.Add(0);

            bufferStorage.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(vertexBufferSizeInFloats);

            if (useNormals)
            {
                // A VertexStream is used to write data into a VertexBuffer.
                var vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionNormalColored();
                foreach (var meshInfo in meshes)
                {
                    var mesh = meshInfo.Mesh;
                    foreach (var vertex in mesh.Vertices)
                        vertexStream.AddVertex(new VertexPositionNormalColored(vertex + m_offset, meshInfo.Normal,
                            meshInfo.ColorWithTransparency));

                    numVerticesInMeshesBefore.Add(numVerticesInMeshesBefore.Last() + mesh.Vertices.Count);
                }
            }
            else
            {
                // A VertexStream is used to write data into a VertexBuffer.
                var vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionColored();
                foreach (var meshInfo in meshes)
                {
                    var mesh = meshInfo.Mesh;
                    // make the color of all faces white in HLR
                    var color = bufferStorage.DisplayStyle == DisplayStyle.HLR
                        ? new ColorWithTransparency(255, 255, 255, meshInfo.ColorWithTransparency.GetTransparency())
                        : meshInfo.ColorWithTransparency;
                    foreach (var vertex in mesh.Vertices)
                        vertexStream.AddVertex(new VertexPositionColored(vertex + m_offset, color));

                    numVerticesInMeshesBefore.Add(numVerticesInMeshesBefore.Last() + mesh.Vertices.Count);
                }
            }

            bufferStorage.VertexBuffer.Unmap();

            // Primitives are specified using a pair of vertex and index buffers. An index buffer contains a sequence of indices into
            // the associated vertex buffer, each index referencing a particular vertex.

            var meshNumber = 0;
            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            var indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            {
                // An IndexStream is used to write data into an IndexBuffer.
                var indexStream = bufferStorage.IndexBuffer.GetIndexStreamTriangle();
                foreach (var meshInfo in meshes)
                {
                    var mesh = meshInfo.Mesh;
                    var startIndex = numVerticesInMeshesBefore[meshNumber];
                    for (var i = 0; i < mesh.NumTriangles; i++)
                    {
                        var mt = mesh.get_Triangle(i);
                        // Add three indices that define a triangle.
                        indexStream.AddTriangle(new IndexTriangle((int)(startIndex + mt.get_Index(0)),
                            (int)(startIndex + mt.get_Index(1)),
                            (int)(startIndex + mt.get_Index(2))));
                    }

                    meshNumber++;
                }
            }
            bufferStorage.IndexBuffer.Unmap();

            // VertexFormat is a specification of the data that is associated with a vertex (e.g., position).
            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            // Effect instance is a specification of the appearance of geometry. For example, it may be used to specify color, if there is no color information provided with the vertices.
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }

        // A helper function, analogous to ProcessFaces.
        private void ProcessEdges(RenderingPassBufferStorage bufferStorage)
        {
            var edges = bufferStorage.EdgeXyZs;
            if (edges.Count == 0)
                return;

            // Edges are encoded as line segment primitives whose vertices contain only position information.
            bufferStorage.FormatBits = VertexFormatBits.Position;

            var edgeVertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * bufferStorage.VertexBufferCount;
            var numVerticesInEdgesBefore = new List<int> { 0 };

            bufferStorage.VertexBuffer = new VertexBuffer(edgeVertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(edgeVertexBufferSizeInFloats);
            {
                var vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPosition();
                foreach (var xyzs in edges)
                {
                    foreach (var vertex in xyzs) vertexStream.AddVertex(new VertexPosition(vertex + m_offset));

                    numVerticesInEdgesBefore.Add(numVerticesInEdgesBefore.Last() + xyzs.Count);
                }
            }
            bufferStorage.VertexBuffer.Unmap();

            var edgeNumber = 0;
            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexLine.GetSizeInShortInts();
            var indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            {
                var indexStream = bufferStorage.IndexBuffer.GetIndexStreamLine();
                foreach (var xyzs in edges)
                {
                    var startIndex = numVerticesInEdgesBefore[edgeNumber];
                    for (var i = 1; i < xyzs.Count; i++)
                        // Add two indices that define a line segment.
                        indexStream.AddLine(new IndexLine(startIndex + i - 1,
                            startIndex + i));
                    edgeNumber++;
                }
            }
            bufferStorage.IndexBuffer.Unmap();

            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }

        // A container to hold information associated with a triangulated face.
        private class MeshInfo
        {
            public readonly ColorWithTransparency ColorWithTransparency;

            public readonly Mesh Mesh;
            public readonly XYZ Normal;

            public MeshInfo(Mesh mesh, XYZ normal, ColorWithTransparency color)
            {
                Mesh = mesh;
                Normal = normal;
                ColorWithTransparency = color;
            }
        }

        // A class that brings together all the data and rendering parameters that are needed to draw one sequence of primitives (e.g., triangles)
        // with the same format and appearance.
        private class RenderingPassBufferStorage
        {
            public RenderingPassBufferStorage(DisplayStyle displayStyle)
            {
                DisplayStyle = displayStyle;
                Meshes = new List<MeshInfo>();
                EdgeXyZs = new List<IList<XYZ>>();
            }

            public DisplayStyle DisplayStyle { get; }

            public VertexFormatBits FormatBits { get; set; }

            public List<MeshInfo> Meshes { get; }

            public List<IList<XYZ>> EdgeXyZs { get; }

            public int PrimitiveCount { get; set; }

            public int VertexBufferCount { get; set; }

            public int IndexBufferCount { get; set; }

            public VertexBuffer VertexBuffer { get; set; }

            public IndexBuffer IndexBuffer { get; set; }

            public VertexFormat VertexFormat { get; set; }

            public EffectInstance EffectInstance { get; set; }

            public bool NeedsUpdate(DisplayStyle newDisplayStyle)
            {
                if (newDisplayStyle != DisplayStyle)
                    return true;

                if (PrimitiveCount > 0)
                    if (VertexBuffer == null || !VertexBuffer.IsValid() ||
                        IndexBuffer == null || !IndexBuffer.IsValid() ||
                        VertexFormat == null || !VertexFormat.IsValid() ||
                        EffectInstance == null || !EffectInstance.IsValid())
                        return true;

                return false;
            }
        }
    }
}
