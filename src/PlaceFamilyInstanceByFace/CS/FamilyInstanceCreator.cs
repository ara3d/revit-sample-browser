// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GeoInstance = Autodesk.Revit.DB.GeometryInstance;
using GeoElement = Autodesk.Revit.DB.GeometryElement;
using RevitElement = Autodesk.Revit.DB.Element;

namespace Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS
{
    /// <summary>
    ///     This is main data class for creating family Instance by face
    /// </summary>
    public class FamilyInstanceCreator
    {
        // Revit document
        // Creation application
        private Application m_appCreator;

        /// <summary>
        ///     Constructor, Store the Revit application
        /// </summary>
        /// <param name="app"></param>
        public FamilyInstanceCreator(UIApplication app)
        {
            RevitDoc = app.ActiveUIDocument;
            m_appCreator = app.Application.Create;
            if (!CheckSelectedElementSet()) throw new Exception("Please select an element with face geometry.");
        }
        // all face names
        // all face instances
        // all family symbols
        // all family symbol names
        // the index default family symbol in family list

        /// <summary>
        ///     Store the all face names, they will be displayed in a combo box
        /// </summary>
        public List<string> FaceNameList { get; } = new List<string>();

        /// <summary>
        ///     Revit document
        /// </summary>
        public UIDocument RevitDoc { get; }

        /// <summary>
        ///     Store all face instances for convenience to create a face-based family instance
        /// </summary>
        public List<Face> FaceList { get; } = new List<Face>();

        /// <summary>
        ///     Store all family symbol in current Revit document
        /// </summary>
        public List<FamilySymbol> FamilySymbolList { get; } = new List<FamilySymbol>();

        /// <summary>
        ///     Store all family symbol names
        /// </summary>
        public List<string> FamilySymbolNameList { get; } = new List<string>();

        /// <summary>
        ///     The index of default family symbol, will set it as default value when initializing UI
        ///     For based point, its name is "Point-based"
        ///     For based line, its name is "Line-based"
        ///     The prepared rfa files provide them
        /// </summary>
        public int DefaultFamilySymbolIndex { get; private set; } = -1;

        /// <summary>
        ///     1. Find all family symbols in current Revit document and store them
        ///     2. Find the index of default family symbol
        ///     Point("Point-based"); Line("Line-based")
        /// </summary>
        public void CheckFamilySymbol(BasedType type)
        {
            DefaultFamilySymbolIndex = -1;
            FamilySymbolList.Clear();

            var familySymbolItor =
                new FilteredElementCollector(RevitDoc.Document).OfClass(typeof(FamilySymbol)).GetElementIterator();

            var defaultSymbolName = string.Empty;
            switch (type)
            {
                case BasedType.Point:
                    defaultSymbolName = "Point-based";
                    break;
                case BasedType.Line:
                    defaultSymbolName = "Line-based";
                    break;
            }

            var hasDefaultSymbol = false;
            var ii = 0;

            while (familySymbolItor.MoveNext())
            {
                var symbol = (FamilySymbol)familySymbolItor.Current;
                if (null == symbol) continue;

                if (!hasDefaultSymbol && 0 == string.Compare(defaultSymbolName, symbol.Name))
                {
                    hasDefaultSymbol = true;
                    DefaultFamilySymbolIndex = ii;
                }

                // family symbol
                FamilySymbolList.Add(symbol);

                // family symbol name
                var familyCategoryname = string.Empty;
                if (null != symbol.Family.FamilyCategory)
                    familyCategoryname = $"{symbol.Family.FamilyCategory.Name} : ";
                FamilySymbolNameList.Add($"{familyCategoryname}{symbol.Family.Name} : {symbol.Name}");
                ii++;
            }

            if (!hasDefaultSymbol)
            {
                FamilySymbol loadedfamilySymbol = null;
                try
                {
                    RevitDoc.Document.LoadFamilySymbol($@"{defaultSymbolName}.rfa"
                        , defaultSymbolName
                        , out loadedfamilySymbol);
                }
                catch (Exception)
                {
                    TaskDialog.Show("Revit", "Can't load the prepared rfa.");
                }

                if (null == loadedfamilySymbol) return;
                FamilySymbolList.Add(loadedfamilySymbol);

                var familyCategoryname = string.Empty;
                if (null != loadedfamilySymbol.Family.FamilyCategory)
                    familyCategoryname = $"{loadedfamilySymbol.Family.FamilyCategory.Name}: ";
                FamilySymbolNameList.Add(
                    $"{familyCategoryname}{loadedfamilySymbol.Family.Name}: {loadedfamilySymbol.Name}");
                DefaultFamilySymbolIndex = FamilySymbolList.Count - 1;
            }
        }

        /// <summary>
        ///     Create a based-point family instance by face
        /// </summary>
        /// <param name="locationP">the location point</param>
        /// <param name="directionP">the direction</param>
        /// <param name="faceIndex">the index of the selected face</param>
        /// <param name="familySymbolIndex">the index of the selected family symbol</param>
        /// <returns></returns>
        public bool CreatePointFamilyInstance(XYZ locationP, XYZ directionP, int faceIndex
            , int familySymbolIndex)
        {
            var face = FaceList[faceIndex];

            if (!FamilySymbolList[familySymbolIndex].IsActive)
                FamilySymbolList[familySymbolIndex].Activate();

            var instance = RevitDoc.Document.Create.NewFamilyInstance(face
                , locationP, directionP, FamilySymbolList[familySymbolIndex]);

            var instanceId = new List<ElementId> { instance.Id };
            RevitDoc.Selection.SetElementIds(instanceId);
            return true;
        }

        /// <summary>
        ///     Create a based-line family instance by face
        /// </summary>
        /// <param name="startP">the start point</param>
        /// <param name="endP">the end point</param>
        /// <param name="faceIndex">the index of the selected face</param>
        /// <param name="familySymbolIndex">the index of the selected family symbol</param>
        /// <returns></returns>
        public bool CreateLineFamilyInstance(XYZ startP, XYZ endP, int faceIndex
            , int familySymbolIndex)
        {
            var face = FaceList[faceIndex];
            var projectedStartP = Project(face.Triangulate().Vertices as List<XYZ>, startP);
            var projectedEndP = Project(face.Triangulate().Vertices as List<XYZ>, endP);

            if (projectedStartP.IsAlmostEqualTo(projectedEndP)) return false;

            var line = Line.CreateBound(projectedStartP, projectedEndP);
            if (!FamilySymbolList[familySymbolIndex].IsActive)
                FamilySymbolList[familySymbolIndex].Activate();
            var instance = RevitDoc.Document.Create.NewFamilyInstance(face, line
                , FamilySymbolList[familySymbolIndex]);

            var instanceId = new List<ElementId> { instance.Id };
            RevitDoc.Selection.SetElementIds(instanceId);
            return true;
        }

        /// <summary>
        ///     Judge whether the selected elementSet has face geometry
        /// </summary>
        /// <returns>true is having face geometry, false is having no face geometry</returns>
        public bool CheckSelectedElementSet()
        {
            // judge whether an or more element is selected
            var es = new ElementSet();
            foreach (var elementId in RevitDoc.Selection.GetElementIds())
            {
                es.Insert(RevitDoc.Document.GetElement(elementId));
            }

            if (1 != es.Size) return false;

            FaceList.Clear();
            FaceNameList.Clear();

            // judge whether the selected element has face geometry
            foreach (var elemId in RevitDoc.Selection.GetElementIds())
            {
                var elem = RevitDoc.Document.GetElement(elemId);
                CheckSelectedElement(elem);
                break;
            }

            return 0 < FaceList.Count;
        }

        /// <summary>
        ///     Get the bounding box of a face, the BoundingBoxXYZ will be set in UI as default value
        /// </summary>
        /// <param name="indexFace">the index of face</param>
        /// <returns>the bounding box</returns>
        public BoundingBoxXYZ GetFaceBoundingBox(int indexFace)
        {
            var mesh = FaceList[indexFace].Triangulate();

            var maxP = new XYZ(double.MinValue, double.MinValue, double.MinValue);
            var minP = new XYZ(double.MaxValue, double.MaxValue, double.MaxValue);
            foreach (var tempXyz in mesh.Vertices)
            {
                minP = new XYZ(
                    Math.Min(minP.X, tempXyz.X),
                    Math.Min(minP.Y, tempXyz.Y),
                    Math.Min(minP.Z, tempXyz.Z));

                maxP = new XYZ(
                    Math.Max(maxP.X, tempXyz.X),
                    Math.Max(maxP.Y, tempXyz.Y),
                    Math.Max(maxP.Z, tempXyz.Z));
            }

            var retBounding = new BoundingBoxXYZ
            {
                Max = maxP,
                Min = minP
            };
            return retBounding;
        }

        /// <summary>
        ///     Judge whether an element has face geometry
        /// </summary>
        /// <param name="elem">the element to be checked</param>
        /// <returns>true is having face geometry, false is having no face geometry</returns>
        private bool CheckSelectedElement(RevitElement elem)
        {
            if (null == elem) return false;
            var opts = new Options
            {
                View = RevitDoc.Document.ActiveView,
                ComputeReferences = true
            };
            // Get geometry of the element
            var geoElement = elem.get_Geometry(opts);
            InquireGeometry(geoElement, elem);

            return true;
        }

        /// <summary>
        ///     Inquire an geometry element to get all face instances
        /// </summary>
        /// <param name="geoElement">the geometry element</param>
        /// <param name="elem">the element, it provides the prefix of face name</param>
        /// <returns></returns>
        private bool InquireGeometry(GeoElement geoElement, RevitElement elem)
        {
            if (null == geoElement || null == elem) return false;

            //GeometryObjectArray geoArray = null;
            var objects = geoElement.GetEnumerator();
            //if (null != geoElement && null != geoElement.Objects)
            if (null != geoElement && objects.MoveNext())
            {
                //geoArray = geoElement.Objects;
            }
            else
            {
                return false;
            }

            objects.Reset();
            //foreach (GeometryObject obj in geoArray)
            while (objects.MoveNext())
            {
                var obj = objects.Current;

                if (obj is GeoInstance instance)
                {
                    InquireGeometry(instance.SymbolGeometry, elem);
                }
                else if (!(obj is Solid))
                {
                    // is not Solid instance
                    continue;
                }

                // continue when obj is Solid instance
                var solid = obj as Solid;
                if (null == solid) continue;
                var faces = solid.Faces;
                if (faces.IsEmpty) continue;

                // get the face name list
                var category = string.Empty;
                if (null != elem.Category && null != elem.Name) category = elem.Category.Name;

                var ii = 0;
                foreach (Face tempFace in faces)
                {
                    if (tempFace is PlanarFace)
                    {
                        FaceNameList.Add(
                            $"{category} : {elem.Name} ({ii})");
                        FaceList.Add(tempFace);
                        ii++;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Project a point on a face
        /// </summary>
        /// <param name="xyzArray">the face points, them fix a face </param>
        /// <param name="point">the point</param>
        /// <returns>the projected point on this face</returns>
        private static XYZ Project(List<XYZ> xyzArray, XYZ point)
        {
            var a = xyzArray[0] - xyzArray[1];
            var b = xyzArray[0] - xyzArray[2];
            var c = point - xyzArray[0];

            var normal = a.CrossProduct(b);

            try
            {
                normal = normal.Normalize();
            }
            catch (Exception)
            {
                normal = XYZ.Zero;
            }

            var retProjectedPoint = point - normal.DotProduct(c) * normal;
            return retProjectedPoint;
        }
    }
}
