// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.Custom2DExporter.CS
{
    /// <summary>
    ///     This is an implementation of IExportContext2D, to be passed to an instance of CustomExporter object. It exports:
    ///     - (if CustomExporter has IncludeGeometricObjects == true) exports all model geometry, tessellated into lines and
    ///     represented by a list of points  contained in m_points.
    ///     - (if CustomExporter has Export2DIncludingAnnotationObjects == true) exports all annotation geometry, tessellated
    ///     into lines represented by a list of points contained in m_points.
    ///     - (if CustomExporter has Export2DIncludingAnnotationObjects == true) export all text strings in text notes, divided
    ///     by a newline and collected in m_texts.
    ///     - (if CustomExporter has Export2DGeometricObjectsIncludingPatternLines == true) exports all patterns, tessellated
    ///     into lines and represented by a list of points  contained in m_points.
    ///     - the exporter also keeps a tally of all exported elements in m_numElements.
    ///     The methods OnCurve, OnPolyline, OnFaceBegin, OnFaceEdge2D and OnFaceSilhouette2D return RenderNodeAction.Proceed,
    ///     will makes sure that all geometry comes in tessellated in OnLineSegment and OnPolylineSegments.
    ///     Note1: Some annotation geometry is exported in OnCurve and OnPolyline, where it is tessellated and stored in
    ///     m_points.
    ///     Note2: If you wish to process geometry without tessellating it, then you need to implement curve export in
    ///     OnCurve, OnPolyline, OnFaceBegin, OnFaceEdge2D and OnFaceSilhouette2D and return RenderNodeAction.Skip.
    ///     Note3: Special instance transforms may not always be taken into account.
    /// </summary>
    public class TessellatedGeomAndText2DExportContext : IExportContext2D
    {
        private Element m_currentElem;

        /// <summary>
        ///     The list of (start, end) points for all tessellated lines
        /// </summary>
        private readonly IList<XYZ> m_points = new List<XYZ>();

        public TessellatedGeomAndText2DExportContext(out IList<XYZ> points)
        {
            points = m_points;
        }

        /// <summary>
        ///     The number of all processed elements, as well as breakdown by some element kinds
        /// </summary>
        public int NumElements { get; private set; }

        public int NumTexts { get; private set; }

        /// <summary>
        ///     All text collected in the view, with a newline between each TextNode.
        /// </summary>
        public string Texts { get; private set; }

        public bool Start()
        {
            return true;
        }

        public void Finish()
        {
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            return RenderNodeAction.Skip;
        }

        public void OnElementEnd(ElementId elementId)
        {
        }

        public RenderNodeAction OnElementBegin2D(ElementNode node)
        {
            NumElements++;

            m_currentElem = node.Document.GetElement(node.ElementId);

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd2D(ElementNode node)
        {
            m_currentElem = null;
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
        }

        public RenderNodeAction OnCurve(CurveNode node)
        {
            // Customize tessellation of annotation curves
            if (m_currentElem.Category.CategoryType == CategoryType.Annotation)
            {
                IList<XYZ> list = new List<XYZ>();

                var curve = node.GetCurve();
                if (curve is Line line)
                {
                    list.Add(line.GetEndPoint(0));
                    list.Add(line.GetEndPoint(1));
                }
                else
                {
                    list = curve.Tessellate();
                }

                Utilities.AddTo(m_points, list);
                return RenderNodeAction.Skip;
            }

            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnPolyline(PolylineNode node)
        {
            // Customize processing of annotation polylines
            if (m_currentElem.Category.CategoryType == CategoryType.Annotation)
            {
                var pLine = node.GetPolyline();
                var list = pLine.GetCoordinates();
                Utilities.AddTo(m_points, list);
                return RenderNodeAction.Skip;
            }

            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnFaceEdge2D(FaceEdgeNode node)
        {
            // Customize tessellation of annotation face edges
            //if (m_currentElem.Category.CategoryType == CategoryType.Annotation)
            //{
            //   Curve curve = node.GetFaceEdge().AsCurve();
            //   if (curve != null)
            //   {
            //      curve = curve.CreateTransformed(node.GetInstanceTransform());
            //      IList<XYZ> list = curve.Tessellate();
            //      Utilities.addTo(m_points, list);
            //   }
            //}
            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnFaceSilhouette2D(FaceSilhouetteNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnText(TextNode node)
        {
            Texts += "\n" + node.Text;
            ++NumTexts;
        }

        public void OnLight(LightNode node)
        {
        }

        public void OnRPC(RPCNode node)
        {
        }

        public void OnMaterial(MaterialNode node)
        {
        }

        public void OnPolymesh(PolymeshTopology node)
        {
        }

        public void OnLineSegment(LineSegment segment)
        {
            var segmentStart = segment.StartPoint;
            var segmentEnd = segment.EndPoint;

            IList<XYZ> list = new List<XYZ>();
            list.Add(segmentStart);
            list.Add(segmentEnd);
            Utilities.AddTo(m_points, list);
        }

        public void OnPolylineSegments(PolylineSegments segments)
        {
            var segPoints = segments.GetVertices();
            Utilities.AddTo(m_points, segPoints);
        }
    }
}
