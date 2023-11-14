// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using ArgumentException = Autodesk.Revit.Exceptions.ArgumentException;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;
using RevitFreeFormElement = Autodesk.Revit.DB.FreeFormElement;

namespace Revit.SDK.Samples.FreeFormElement.CS
{
    /// <summary>
    ///     Utilities supporting the creation of a family containing a FreeFormElement which is cut out from existing geometry
    /// </summary>
    internal class FreeFormElementUtils
    {
        public enum FailureCondition
        {
            Success,
            CurvesNotContigous,
            CurveLoopAboveTarget,
            NoIntersection
        }

        /// <summary>
        ///     Creates a negative block family from the geometry of the target element and boundaries.
        /// </summary>
        /// <remarks>This is the main implementation of the sample command.</remarks>
        /// <param name="targetElement">The target solid element.</param>
        /// <param name="boundaries">The selected curve element boundaries.</param>
        /// <param name="familyLoadOptions">The family load options when loading the new family.</param>
        /// <param name="familyTemplate">The family template.</param>
        public static FailureCondition CreateNegativeBlock(Element targetElement, IList<Reference> boundaries,
            IFamilyLoadOptions familyLoadOptions, string familyTemplate)
        {
            var doc = targetElement.Document;
            var app = doc.Application;

            // Get curve loop for boundary
            var curves = GetContiguousCurvesFromSelectedCurveElements(doc, boundaries);
            CurveLoop loop = null;
            try
            {
                loop = CurveLoop.Create(curves);
            }
            catch (ArgumentException)
            {
                // Curves are not contiguous
                return FailureCondition.CurvesNotContigous;
            }

            var loops = new List<CurveLoop>();
            loops.Add(loop);

            // Get elevation of loop
            var elevation = curves[0].GetEndPoint(0).Z;

            // Get height for extrusion
            var bbox = targetElement.get_BoundingBox(null);
            var height = bbox.Max.Z - elevation;

            if (height <= 1e-5)
                return FailureCondition.CurveLoopAboveTarget;

            height += 1;

            // Create family
            var familyDoc = app.NewFamilyDocument(familyTemplate);

            // Create block from boundaries
            var block = GeometryCreationUtilities.CreateExtrusionGeometry(loops, XYZ.BasisZ, height);

            // Subtract target element
            var fromElement = GetTargetSolids(targetElement);

            var solidCount = fromElement.Count;

            // Merge all found solids into single one
            Solid toSubtract = null;
            if (solidCount == 1)
                toSubtract = fromElement[0];

            else if (solidCount > 1)
                toSubtract =
                    BooleanOperationsUtils.ExecuteBooleanOperation(fromElement[0], fromElement[1],
                        BooleanOperationsType.Union);

            if (solidCount > 2)
                for (var i = 2; i < solidCount; i++)
                    toSubtract = BooleanOperationsUtils.ExecuteBooleanOperation(toSubtract, fromElement[i],
                        BooleanOperationsType.Union);

            // Subtract merged solid from overall block
            try
            {
                BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(block, toSubtract,
                    BooleanOperationsType.Difference);
            }
            catch (InvalidOperationException)
            {
                return FailureCondition.NoIntersection;
            }

            // Create freeform element
            using (var t = new Transaction(familyDoc, "Add element"))
            {
                t.Start();
                RevitFreeFormElement.Create(familyDoc, block);
                t.Commit();
            }

            // Load family into document
            var family = familyDoc.LoadFamily(doc, familyLoadOptions);

            familyDoc.Close(false);

            // Get symbol as first symbol of loaded family
            var collector = new FilteredElementCollector(doc);
            collector.WherePasses(new FamilySymbolFilter(family.Id));
            var fs = collector.FirstElement() as FamilySymbol;


            // Place instance at location of original curves
            using (var t2 = new Transaction(doc, "Place instance"))
            {
                t2.Start();
                if (!fs.IsActive)
                    fs.Activate();
                doc.Create.NewFamilyInstance(XYZ.Zero, fs, StructuralType.NonStructural);
                t2.Commit();
            }

            return FailureCondition.Success;
        }

        /// <summary>
        ///     Gets a list of curves which are ordered correctly and oriented correctly to form a closed loop.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="boundaries">The list of curve element references which are the boundaries.</param>
        /// <returns>The list of curves.</returns>
        public static IList<Curve> GetContiguousCurvesFromSelectedCurveElements(Document doc,
            IList<Reference> boundaries)
        {
            var curves = new List<Curve>();

            // Build a list of curves from the curve elements
            foreach (var reference in boundaries)
            {
                var curveElement = doc.GetElement(reference) as CurveElement;
                curves.Add(curveElement.GeometryCurve.Clone());
            }

            // Walk through each curve (after the first) to match up the curves in order
            for (var i = 0; i < curves.Count; i++)
            {
                var curve = curves[i];
                var endPoint = curve.GetEndPoint(1);

                // find curve with start point = end point
                for (var j = i + 1; j < curves.Count; j++)
                    // Is there a match end->start, if so this is the next curve
                    if (curves[j].GetEndPoint(0).IsAlmostEqualTo(endPoint, 1e-05))
                    {
                        var tmpCurve = curves[i + 1];
                        curves[i + 1] = curves[j];
                        curves[j] = tmpCurve;
                    }
                    // Is there a match end->end, if so, reverse the next curve
                    else if (curves[j].GetEndPoint(1).IsAlmostEqualTo(endPoint, 1e-05))
                    {
                        var tmpCurve = curves[i + 1];
                        curves[i + 1] = CreateReversedCurve(curves[j]);
                        curves[j] = tmpCurve;
                    }
            }

            return curves;
        }

        /// <summary>
        ///     Utility to create a new curve with the same geometry but in the reverse direction.
        /// </summary>
        /// <param name="orig">The original curve.</param>
        /// <returns>The reversed curve.</returns>
        /// <throws cref="NotImplementedException">If the curve type is not supported by this utility.</throws>
        private static Curve CreateReversedCurve(Curve orig)
        {
            if (!SupportsLoopUtilities(orig))
                throw new NotImplementedException("CreateReversedCurve for type " + orig.GetType().Name);

            if (orig is Line)
                return Line.CreateBound(orig.GetEndPoint(1), orig.GetEndPoint(0));
            if (orig is Arc)
                return Arc.Create(orig.GetEndPoint(1), orig.GetEndPoint(0), orig.Evaluate(0.5, true));
            throw new Exception("CreateReversedCurve - Unreachable");
        }

        /// <summary>
        ///     Identifies if the curve type is supported in these utilities.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>True if the curve type is supported, false otherwise.</returns>
        public static bool SupportsLoopUtilities(Curve curve)
        {
            return curve is Line || curve is Arc;
        }

        /// <summary>
        ///     Identifies if the curve lies entirely in an XY plane (Z = constant)
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>True if the curve lies in an XY plane, false otherwise.</returns>
        public static bool IsCurveInXYPlane(Curve curve)
        {
            // quick reject - are endpoints at same Z
            var zDelta = curve.GetEndPoint(1).Z - curve.GetEndPoint(0).Z;

            if (Math.Abs(zDelta) > 1e-05)
                return false;

            if (!(curve is Line) && !curve.IsCyclic)
            {
                // Create curve loop from curve and connecting line to get plane
                var curves = new List<Curve>();
                curves.Add(curve);
                curves.Add(Line.CreateBound(curve.GetEndPoint(1), curve.GetEndPoint(0)));
                var curveLoop = CurveLoop.Create(curves);

                var normal = curveLoop.GetPlane().Normal.Normalize();
                if (!normal.IsAlmostEqualTo(XYZ.BasisZ) && !normal.IsAlmostEqualTo(XYZ.BasisZ.Negate()))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets all target solids in a given element.
        /// </summary>
        /// <remarks>
        ///     Includes solids and solids in first level instances only.  Deeper levels are ignored.  Empty solids are not
        ///     returned.
        /// </remarks>
        /// <param name="element">The element.</param>
        /// <returns>The list of solids.</returns>
        public static IList<Solid> GetTargetSolids(Element element)
        {
            var solids = new List<Solid>();


            var options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;
            var geomElem = element.get_Geometry(options);
            foreach (var geomObj in geomElem)
                if (geomObj is Solid)
                {
                    var solid = (Solid)geomObj;
                    if (solid.Faces.Size > 0 && solid.Volume > 0.0) solids.Add(solid);
                    // Single-level recursive check of instances. If viable solids are more than
                    // one level deep, this example ignores them.
                }
                else if (geomObj is GeometryInstance)
                {
                    var geomInst = (GeometryInstance)geomObj;
                    var instGeomElem = geomInst.GetInstanceGeometry();
                    foreach (var instGeomObj in instGeomElem)
                        if (instGeomObj is Solid)
                        {
                            var solid = (Solid)instGeomObj;
                            if (solid.Faces.Size > 0 && solid.Volume > 0.0) solids.Add(solid);
                        }
                }

            return solids;
        }

        /// <summary>
        ///     Finds the Generic Model template from the family template directory path, if it exists.
        /// </summary>
        /// <param name="familyPath">The family template directory path.</param>
        /// <returns>The template path, or empty string if not found.</returns>
        public static string FindGenericModelTemplate(string familyPath)
        {
            var hardCodedResult =
                Path.Combine(Path.GetDirectoryName(typeof(CreateNegativeBlockCommand).Assembly.Location),
                    "Generic Model.rft");
            try
            {
                var files = Directory.EnumerateFiles(familyPath, "Generic Model.rft", SearchOption.AllDirectories);

                var result = files.FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                    return result;

                files = Directory.EnumerateFiles(familyPath, "Metric Generic Model.rft", SearchOption.AllDirectories);

                result = files.FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                    return result;
                return hardCodedResult;
            }
            catch (Exception)
            {
                return hardCodedResult;
            }
        }
    }
}
