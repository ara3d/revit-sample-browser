using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_LinkedFileElements sample.</summary>
    internal static partial class Util
    {
        public static void AddFaceBasedFamilyToLinks(Document doc)
        {
            var alignedLinkId = new ElementId((Int64)125929);

            var symbolId = new ElementId((long)126580);

            var fs = doc.GetElement(symbolId)
                as FamilySymbol;

            var linkInstance = doc.GetElement(
                alignedLinkId) as RevitLinkInstance;

            var linkDocument = linkInstance
                .GetLinkDocument();

            var wallCollector
                = new FilteredElementCollector(linkDocument);

            wallCollector.OfClass(typeof(Wall));

            var targetWall = wallCollector.FirstElement()
                as Wall;

            var exteriorFaceRef
                = HostObjectUtils.GetSideFaces(
                        targetWall, ShellLayerType.Exterior)
                    .First();

            var linkToExteriorFaceRef
                = exteriorFaceRef.CreateLinkReference(
                    linkInstance);

            var wallLine = (targetWall.Location
                as LocationCurve).Curve as Line;

            var wallVector = (wallLine.GetEndPoint(1)
                              - wallLine.GetEndPoint(0)).Normalize();

            using var t = new Transaction(doc);
            t.Start("Add to face");

            doc.Create.NewFamilyInstance(
                linkToExteriorFaceRef, XYZ.Zero,
                wallVector, fs);

            t.Commit();
        }

        /// <summary>
        ///     Tag all walls in all linked documents
        /// </summary>
        public static void TagAllLinkedWalls(Document doc)
        {
            var xyz = new XYZ(-20, 20, 0);

            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance));

            foreach (var elem in collector)
            {
                var instance = elem
                    as RevitLinkInstance;

                var linkDoc = instance.GetLinkDocument();

                var type = doc.GetElement(
                    instance.GetTypeId()) as RevitLinkType;

                if (RevitLinkType.IsLoaded(doc, type.Id))
                {
                    var walls
                        = new FilteredElementCollector(linkDoc)
                            .OfCategory(BuiltInCategory.OST_Walls)
                            .OfClass(typeof(Wall));

                    foreach (Wall wall in walls)
                    {
                        var newRef = new Reference(wall)
                            .CreateLinkReference(instance);

                        using var tx = new Transaction(doc);
                        tx.Start("Create tags");

                        var newTag = IndependentTag.Create(
                            doc, doc.ActiveView.Id, newRef, true,
                            TagMode.TM_ADDBY_MATERIAL,
                            TagOrientation.Horizontal, xyz);

                        var linkIds = newTag.GetTaggedElementIds();
                        var linkInstanceId = linkIds.First().LinkInstanceId;
                        var linkedElementId = linkIds.First().LinkedElementId;

                        tx.Commit();
                    }
                }
            }
        }

        public static IEnumerable<Document> GetLinkedDocuments(
            Document doc)
        {
            throw new NotImplementedException();
        }

        public static Face SelectFace(UIApplication uiapp)
        {
            var doc = uiapp.ActiveUIDocument.Document;

            var doc2 = GetLinkedDocuments(
                doc);

            var sel
                = uiapp.ActiveUIDocument.Selection;

            var pickedRef = sel.PickObject(
                ObjectType.PointOnElement,
                "Please select a Face");

            var elem = doc.GetElement(pickedRef.ElementId);

            var et = elem.GetType();

            if (typeof(RevitLinkType) == et
                || typeof(RevitLinkInstance) == et
                || typeof(Instance) == et)
            {
                foreach (var d in doc2)
                    if (elem.Name.Contains(d.Title))
                    {
                        var pickedRefInLink = pickedRef
                            .CreateReferenceInLink();

                        var myElement = d.GetElement(
                            pickedRefInLink.ElementId);

                        var myGeometryObject = myElement
                            .GetGeometryObjectFromReference(
                                pickedRefInLink) as Face;

                        return myGeometryObject;
                    }
            }
            else
            {
                var myElement = doc.GetElement(
                    pickedRef.ElementId);

                var myGeometryObject = myElement
                        .GetGeometryObjectFromReference(pickedRef)
                    as Face;

                return myGeometryObject;
            }

            return null;
        }
    }
}
