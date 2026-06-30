// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    internal class ExportContextAdnMesh : IExportContext
    {
        readonly Document _doc;

        readonly Stack<Transform> _transformationStack
            = new Stack<Transform>();

        readonly VertexLookupInt _vertices = new VertexLookupInt();

        readonly List<int> _triangles = new List<int>();

        /// <summary>
        ///     List of normal vectors, defined by an index into the normal lookup for each triangle vertex.
        /// </summary>
        readonly List<int> _normalIndices = new List<int>();

        readonly NormalLookupXyz _normals = new NormalLookupXyz();

        readonly CentroidVolume _centroidVolume = new CentroidVolume();

        Color _color;
        double _transparency;
        readonly List<AdnMeshData> _data;

        public ExportContextAdnMesh(Document doc)
        {
            _doc = doc;
            _data = new List<AdnMeshData>();
            _transformationStack.Push(Transform.Identity);
        }

        public AdnMeshData[] MeshData => _data.ToArray();

        Transform CurrentTransform => _transformationStack.Peek();

        /// <summary>
        ///     Store a triangle, adding new vertices for it to our vertex lookup dictionary if needed
        ///     and accumulating its volume and centroid contribution.
        /// </summary>
        void StoreTriangle(
            IList<XYZ> vertices,
            PolymeshFacet triangle,
            XYZ normal)
        {
            var currentTransform = CurrentTransform;

            var p = new[]
            {
                currentTransform.OfPoint(vertices[triangle.V1]),
                currentTransform.OfPoint(vertices[triangle.V2]),
                currentTransform.OfPoint(vertices[triangle.V3])
            };

            _centroidVolume.AddTriangle(p);

            for (var i = 0; i < 3; ++i)
            {
                var q = new PointInt(p[i]);

                _triangles.Add(_vertices.AddVertex(q));

                _normalIndices.Add(_normals.AddNormal(
                    currentTransform.OfVector(normal)));
            }
        }

        public void Finish()
        {
            Debug.Print("Finish");
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            Debug.Print("ElementBegin id " + elementId);

            _vertices.Clear();
            _triangles.Clear();
            _normals.Clear();
            _normalIndices.Clear();
            _centroidVolume.Init();

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            Debug.Print("ElementEnd");

            _centroidVolume.Complete();

            var metadataId = _doc.GetElement(elementId).UniqueId;

            var meshData = new AdnMeshData(
                _vertices, _triangles, _normals, _normalIndices,
                new PointInt(_centroidVolume.Centroid),
                _color, _transparency, metadataId);

            _data.Add(meshData);
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnFaceEnd(FaceNode node)
        {
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            var symbol = _doc.GetElement(
                node.GetSymbolGeometryId().SymbolId) as FamilySymbol;

            Debug.Assert(null != symbol,
                "expected valid family symbol");

            if (symbol != null)
            {
                Debug.Print("InstanceBegin "
                            + symbol.Category.Name + " : "
                            + symbol.Family.Name + " : "
                            + symbol.Name);
            }

            _transformationStack.Push(CurrentTransform
                .Multiply(node.GetTransform()));

            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            Debug.Print("InstanceEnd");

            _transformationStack.Pop();
        }

        public void OnLight(LightNode node)
        {
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            _transformationStack.Push(CurrentTransform
                .Multiply(node.GetTransform()));

            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            _transformationStack.Pop();
        }

        public void OnMaterial(MaterialNode node)
        {
            var c = node.Color;
            var t = node.Transparency;

            Debug.Print("Colour "
                        + string.Format("({0},{1},{2})", c.Red, c.Green, c.Blue)
                        + ", transparency " + t.ToString("0.##"));

            _color = c;
            _transparency = t;
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            var nPts = node.NumberOfPoints;
            var nFacets = node.NumberOfFacets;

            var distrib = node.DistributionOfNormals;

            Debug.Print(string.Format(
                "Polymesh {0} vertices {1} facets",
                nPts, nFacets));

            var iFacet = 0;

            var vertices = node.GetPoints();
            XYZ normal;

            foreach (var triangle in node.GetFacets())
            {
                if (DistributionOfNormals.OnePerFace == distrib)
                {
                    normal = node.GetNormal(0);
                }
                else if (DistributionOfNormals.OnEachFacet == distrib)
                {
                    normal = node.GetNormal(iFacet++);
                }
                else
                {
                    Debug.Assert(DistributionOfNormals.AtEachPoint == distrib, "what else?");

                    normal = node.GetNormal(triangle.V1)
                             + node.GetNormal(triangle.V2)
                             + node.GetNormal(triangle.V3);
                    normal /= 3.0;
                }

                StoreTriangle(vertices, triangle, normal);
            }
        }

        public void OnRPC(RPCNode node)
        {
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            var view = _doc.GetElement(node.ViewId) as View3D;

            Debug.Assert(null != view, "expected valid 3D view");

            Debug.Print("ViewBegin " + view.Name);

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            Debug.Print("ViewEnd");
        }

        public bool Start()
        {
            Debug.Print("Start");
            return true;
        }
    }
}
