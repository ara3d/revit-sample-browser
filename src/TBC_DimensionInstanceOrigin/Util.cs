using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DimensionInstanceOrigin sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Retrieve origin and direction of the left
        ///     reference plane within the given family instance.
        /// </summary>
        internal static bool GetFamilyInstanceReferencePlaneLocation(
            FamilyInstance fi,
            out XYZ origin,
            out XYZ normal)
        {
            var found = false;
            origin = XYZ.Zero;
            normal = XYZ.Zero;

            var r = fi
                .GetReferences(FamilyInstanceReferenceType.Left)
                .FirstOrDefault();

            if (null != r)
            {
                var doc = fi.Document;

                using var t = new Transaction(doc);
                t.Start("Create Temporary Sketch Plane");
                var sk = SketchPlane.Create(doc, r);
                if (null != sk)
                {
                    var pl = sk.GetPlane();
                    origin = pl.Origin;
                    normal = pl.Normal;
                    found = true;
                }

                t.RollBack();
            }

            return found;
        }

        /// <summary>
        ///     Retrieve the given family instance's
        ///     non-visible geometry point reference.
        /// </summary>
        internal static Reference GetFamilyInstancePointReference(
            FamilyInstance fi)
        {
            var opt = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true
            };

            return fi.get_Geometry(opt)
                .OfType<Point>()
                .Select(x => x.Reference)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Create vertical dimensioning.
        /// </summary>
        internal static void CreateVerticalDimensioning(ViewSection viewSection)
        {
            var doc = viewSection.Document;

            var point3 = new XYZ(417.8, 80.228, 46.8);
            var point4 = new XYZ(417.8, 80.811, 46.3);

            var geomLine3 = Line.CreateBound(point3, point4);
            var dummyLine = Line.CreateBound(XYZ.Zero, XYZ.BasisY);

            using var tx = new Transaction(doc);
            tx.Start("tx");

            var line3 = doc.Create.NewDetailCurve(
                viewSection, geomLine3) as DetailLine;

            var dummy = doc.Create.NewDetailCurve(
                viewSection, dummyLine) as DetailLine;

            var refArray = new ReferenceArray();
            refArray.Append(dummy.GeometryCurve.Reference);
            refArray.Append(line3.GeometryCurve.GetEndPointReference(0));
            refArray.Append(line3.GeometryCurve.GetEndPointReference(1));
            var dimPoint1 = new XYZ(417.8, 80.118, 46.8);
            var dimPoint2 = new XYZ(417.8, 80.118, 46.3);
            var dimLine3 = Line.CreateBound(dimPoint1, dimPoint2);

            doc.Create.NewDimension(
                viewSection, dimLine3, refArray);

            doc.Delete(dummy.Id);
            tx.Commit();
        }

        internal static void DimensionBetweenDetaiLines(Document doc)
        {
            var view = doc.ActiveView;

            var p = XYZ.Zero;
            double d = 20;
            var vx = d * XYZ.BasisX;
            var vy = d * XYZ.BasisY;

            using var tx = new Transaction(doc);
            tx.Start("DimensionHardWired");

            var location1 = p;
            var location2 = p + vy;
            var location3 = p + vx;
            var location4 = p + vx + vy;

            var curve1 = Line.CreateBound(location1, location2);
            var curve2 = Line.CreateBound(location3, location4);

            DetailCurve dCurve1;
            DetailCurve dCurve2;

            if (doc.IsFamilyDocument)
            {
                if (doc.OwnerFamily is {FamilyCategory: { }})
                    if (!doc.OwnerFamily.FamilyCategory.Name.Contains("詳細"))
                    {
                        TaskDialog.Show("Dimension Detail Lines",
                            "Please open a detail based family template.");

                        return;
                    }

                dCurve1 = doc.FamilyCreate.NewDetailCurve(view, curve1);
                dCurve2 = doc.FamilyCreate.NewDetailCurve(view, curve2);
            }
            else
            {
                dCurve1 = doc.Create.NewDetailCurve(view, curve1);
                dCurve2 = doc.Create.NewDetailCurve(view, curve2);
            }

            var line = Line.CreateBound(location2, location4);

            var refArray = new ReferenceArray();

            refArray.Append(dCurve1.GeometryCurve.Reference);
            refArray.Append(dCurve2.GeometryCurve.Reference);

            if (doc.IsFamilyDocument)
                doc.FamilyCreate.NewDimension(view, line, refArray);
            else
                doc.Create.NewDimension(view, line, refArray);
            tx.Commit();
        }
    }

    internal class ScottWilsonVoodooMagic
    {
        public enum SpecialReferenceType
        {
            Left = 0,
            CenterLR = 1,
            Right = 2,
            Front = 3,
            CenterFB = 4,
            Back = 5,
            Bottom = 6,
            CenterElevation = 7,
            Top = 8
        }

        public static Edge GetInstanceEdgeFromSymbolRef(
            Reference symbolRef,
            Document dbDoc)
        {
            Edge instEdge = null;

            var gOptions = new Options();
            gOptions.ComputeReferences = true;
            gOptions.DetailLevel = ViewDetailLevel.Undefined;
            gOptions.IncludeNonVisibleObjects = false;

            var elem = dbDoc.GetElement(symbolRef.ElementId);

            var stableRefSymbol = symbolRef
                .ConvertToStableRepresentation(dbDoc);

            var tokenList = stableRefSymbol.Split(':');

            var stableRefInst = $"{tokenList[3]}:{tokenList[4]}:{tokenList[5]}";

            var geomElem = elem.get_Geometry(
                gOptions);

            foreach (var geomElemObj in geomElem)
            {
                var geomInst = geomElemObj
                    as GeometryInstance;

                if (geomInst != null)
                {
                    var gInstGeom = geomInst
                        .GetInstanceGeometry();

                    foreach (var gGeomObject
                        in gInstGeom)
                    {
                        var solid = gGeomObject as Solid;
                        if (solid != null)
                            foreach (Edge edge in solid.Edges)
                            {
                                var stableRef = edge.Reference
                                    .ConvertToStableRepresentation(
                                        dbDoc);

                                if (stableRef == stableRefInst)
                                {
                                    instEdge = edge;
                                    break;
                                }
                            }

                        if (instEdge != null)
                            break;
                    }
                }

                if (instEdge != null)
                    break;
            }

            return instEdge;
        }

        public static Reference GetSpecialFamilyReference(
            FamilyInstance inst,
            SpecialReferenceType refType)
        {
            Reference indexRef = null;

            var idx = (int) refType;

            if (inst != null)
            {
                var dbDoc = inst.Document;

                var geomOptions = dbDoc.Application.Create
                    .NewGeometryOptions();

                if (geomOptions != null)
                {
                    geomOptions.ComputeReferences = true;
                    geomOptions.DetailLevel = ViewDetailLevel.Undefined;
                    geomOptions.IncludeNonVisibleObjects = true;
                }

                var gElement = inst.get_Geometry(
                    geomOptions);

                var gInst = gElement.First()
                    as GeometryInstance;

                string sampleStableRef = null;

                if (gInst != null)
                {
                    var gSymbol = gInst
                        .GetSymbolGeometry();

                    if (gSymbol != null)
                        foreach (var geomObj in gSymbol)
                            if (geomObj is Solid solid)
                            {
                                if (solid.Faces.Size > 0)
                                {
                                    var face = solid.Faces.get_Item(0);

                                    sampleStableRef = face.Reference
                                        .ConvertToStableRepresentation(
                                            dbDoc);

                                    break;
                                }
                            }
                            else if (geomObj is Curve curve)
                            {
                                sampleStableRef = curve.Reference
                                    .ConvertToStableRepresentation(dbDoc);

                                break;
                            }
                            else if (geomObj is Point point)
                            {
                                sampleStableRef = point.Reference
                                    .ConvertToStableRepresentation(dbDoc);

                                break;
                            }

                    if (sampleStableRef != null)
                    {
                        var refTokens = sampleStableRef.Split(':');

                        var customStableRef = $"{refTokens[0]}:{refTokens[1]}:{refTokens[2]}:{refTokens[3]}:{idx}";

                        indexRef = Reference
                            .ParseFromStableRepresentation(
                                dbDoc, customStableRef);

                        var geoObj = inst
                            .GetGeometryObjectFromReference(
                                indexRef);

                        if (geoObj != null)
                        {
                            var finalToken = "";

                            switch (geoObj)
                            {
                                case Edge:
                                    finalToken = ":LINEAR";
                                    break;
                                case Face:
                                    finalToken = ":SURFACE";
                                    break;
                            }

                            customStableRef += finalToken;

                            indexRef = Reference
                                .ParseFromStableRepresentation(
                                    dbDoc, customStableRef);
                        }
                        else
                        {
                            indexRef = null;
                        }
                    }
                }
                else
                {
                    throw new Exception("No Symbol Geometry found...");
                }
            }

            return indexRef;
        }
    }
}
