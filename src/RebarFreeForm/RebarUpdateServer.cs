// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.RebarFreeForm.CS
{
    public class TargetFace
    {
        public TargetFace()
        {
            Transform = Transform.Identity;
            Offset = 0.0;
            Face = null;
        }

        public Face Face { get; set; }

        public Transform Transform { get; set; }

        public double Offset { get; set; }
    }

    public enum BarHandle
    {
        FirstHandle,
        SecondHandle,
        ThirdHandle,
        StartHandle,
        EndHandle
    }

    /// <summary>
    ///     IRebarUpdateServer that builds straight free-form bar sets from three planar face constraints.
    ///     First/last bars intersect face pairs; intermediate bars are evenly spaced. Start/end handles auto-search host faces.
    /// </summary>
    public class RebarUpdateServer : IRebarUpdateServer
    {
        /// <summary>Pass to Rebar.CreateFreeForm to bind this external server.</summary>
        public static Guid SampleGuid = new("64D176BA-EB3E-4E96-877D-46A3B0C17B93");

        public Guid GetServerId()
        {
            return SampleGuid;
        }

        public ExternalServiceId GetServiceId()
        {
            return ExternalServices.BuiltInExternalServices.RebarUpdateService;
        }

        public string GetName()
        {
            return "RebarUpdateServerSample";
        }

        public string GetVendorId()
        {
            return "ADSK";
        }

        public string GetDescription()
        {
            return "Sample to demonstrate implementing an external server to handle rebar constraints calculation";
        }

        public bool GetCustomHandles(RebarHandlesData data)
        {
            data.AddCustomHandle((int)BarHandle.FirstHandle);
            data.AddCustomHandle((int)BarHandle.SecondHandle);
            data.AddCustomHandle((int)BarHandle.ThirdHandle);
            data.SetStartHandle((int)BarHandle.StartHandle);
            data.SetEndHandle((int)BarHandle.EndHandle);
            return true;
        }

        public bool GetHandlesPosition(RebarHandlePositionData data)
        {
            if (data.GetNumberOfBars() <= 0)
                return false;

            var firstBar = data.GetBarGeometry(0);
            data.SetPosition((int)BarHandle.FirstHandle, firstBar[0].Evaluate(0.5, true));
            data.SetPosition((int)BarHandle.SecondHandle, firstBar[0].Evaluate(0.3, true));
            data.SetPosition((int)BarHandle.ThirdHandle, firstBar[0].Evaluate(0.7, true));
            data.SetPosition((int)BarHandle.StartHandle, firstBar[0].Evaluate(0, true));
            data.SetPosition((int)BarHandle.EndHandle, firstBar[0].Evaluate(1, true));
            return true;
        }

        public bool GetCustomHandleName(RebarHandleNameData handleNameData)
        {
            switch (handleNameData.GetCustomHandleTag())
            {
                case (int)BarHandle.FirstHandle:
                    handleNameData.SetCustomHandleName("First Handle");
                    break;
                case (int)BarHandle.SecondHandle:
                    handleNameData.SetCustomHandleName("Second Handle");
                    break;
                case (int)BarHandle.ThirdHandle:
                    handleNameData.SetCustomHandleName("Third Handle");
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool GenerateCurves(RebarCurvesData data)
        {
            TargetFace firstFace = new();
            TargetFace secondFace = new();
            TargetFace thirdFace = new();
            var constraints = data.GetRebarUpdateCurvesData().GetCustomConstraints();
            foreach (var constraint in constraints)
            {
                if (constraint.NumberOfTargets > 1)
                    return false;
                var tempTrf = Transform.Identity;
                double dfOffset = 0;
                if (!SampleBrowserUtils.GetOffsetFromConstraintAtTarget(data.GetRebarUpdateCurvesData(), constraint, 0, out dfOffset))
                    return false;

                switch ((BarHandle)constraint.GetCustomHandleTag())
                {
                    case BarHandle.FirstHandle:
                        {
                            var face = constraint.GetTargetHostFaceAndTransform(0, tempTrf);
                            firstFace = new TargetFace { Face = face, Transform = tempTrf, Offset = dfOffset };
                            break;
                        }
                    case BarHandle.SecondHandle:
                        {
                            var face = constraint.GetTargetHostFaceAndTransform(0, tempTrf);
                            secondFace = new TargetFace { Face = face, Transform = tempTrf, Offset = dfOffset };
                            break;
                        }
                    case BarHandle.ThirdHandle:
                        {
                            var face = constraint.GetTargetHostFaceAndTransform(0, tempTrf);
                            thirdFace = new TargetFace { Face = face, Transform = tempTrf, Offset = dfOffset };
                            break;
                        }
                }
            }

            if (firstFace.Face == null || secondFace.Face == null || thirdFace.Face == null)
                return false;

            var thisBar = GetCurrentRebar(data.GetRebarUpdateCurvesData());
            // Selected model curve overrides intersection geometry for bar shapes.
            var selectedCurve = GetSelectedCurveElement(thisBar, data.GetRebarUpdateCurvesData());
            List<Curve> curves = new();
            Curve originalBar = null;
            var singleBar = GetOffsetCurveAtIntersection(firstFace, secondFace);
            if (selectedCurve != null)
            {
                var trf = Transform.CreateTranslation(singleBar.GetEndPoint(0) -
                                                      selectedCurve.GeometryCurve.GetEndPoint(0));
                originalBar = singleBar;
                singleBar = selectedCurve.GeometryCurve.CreateTransformed(trf);
            }

            if (singleBar == null)
                return false;

            // Non-single layout rules here all space intermediate bars evenly by count/spacing.
            var layout = data.GetRebarUpdateCurvesData().GetLayoutRule();
            switch (layout)
            {
                case RebarLayoutRule.Single:
                    curves.Add(singleBar);
                    break;
                case RebarLayoutRule.FixedNumber:
                case RebarLayoutRule.NumberWithSpacing:
                case RebarLayoutRule.MaximumSpacing:
                case RebarLayoutRule.MinimumClearSpacing:
                    curves.Add(singleBar);
                    var lastBar = GetOffsetCurveAtIntersection(firstFace, thirdFace);

                    var firstBar = selectedCurve != null ? originalBar : singleBar;
                    if (lastBar == null || !AlignBars(ref firstBar, ref lastBar))
                        return false;
                    if (selectedCurve != null)
                    {
                        var trf = Transform.CreateTranslation(lastBar.GetEndPoint(0) -
                                                              selectedCurve.GeometryCurve.GetEndPoint(0));
                        lastBar = selectedCurve.GeometryCurve.CreateTransformed(trf);
                    }

                    if (!GenerateSet(singleBar, lastBar, layout,
                            data.GetRebarUpdateCurvesData().GetBarsNumber(),
                            data.GetRebarUpdateCurvesData().Spacing, ref curves,
                            selectedCurve == null ? null : selectedCurve.GeometryCurve))
                        return false;
                    curves.Add(lastBar);
                    break;
            }

            if (curves.Count <= 0)
                return false;

            List<Curve> distribPath = new();
            for (var ii = 0; ii < curves.Count - 1; ii++)
                distribPath.Add(Line.CreateBound(curves[ii].Evaluate(0.5, true), curves[ii + 1].Evaluate(0.5, true)));
            if (distribPath.Count > 0)
                data.SetDistributionPath(distribPath);

            for (var ii = 0; ii < curves.Count; ii++)
            {
                List<Curve> barCurve = new()
                { curves[ii] };
                data.AddBarGeometry(barCurve);

                // Hook normals set here are reset when TrimExtendCurves replaces bar geometry.
                for (var i = 0; i < 2; i++)
                {
                    var normal = ComputeNormal(curves[ii], firstFace, i);
                    if (normal != null && !normal.IsZeroLength())
                        data.GetRebarUpdateCurvesData().SetHookPlaneNormalForBarIdx(i, ii, normal);
                }
            }

            return true;
        }

        /// <summary>
        ///     Auto-creates start/end face constraints when missing, then trims or extends bars; recalculates hook normals afterward.
        /// </summary>
        public bool TrimExtendCurves(RebarTrimExtendData data)
        {
            if (GetSelectedCurveElement(GetCurrentRebar(data.GetRebarUpdateCurvesData()),
                    data.GetRebarUpdateCurvesData()) != null)
                return true;

            IList<Curve> allbars = [];
            for (var ii = 0; ii < data.GetRebarUpdateCurvesData().GetBarsNumber(); ii++)
                allbars.Add(data.GetRebarUpdateCurvesData().GetBarGeometry(ii)[0]);
            List<TargetFace> hostFaces = new();

            for (var iBarEnd = 0; iBarEnd < 2; iBarEnd++)
            {
                List<TargetFace> faces = new();
                var constraint = iBarEnd == 0
                    ? data.GetRebarUpdateCurvesData().GetStartConstraint()
                    : data.GetRebarUpdateCurvesData().GetEndConstraint();

                if (constraint == null)
                {
                    if (hostFaces.Count <= 0)
                    {
                        // ComputeReferences required so found faces can become constraint targets.
                        Options geomOptions = new()
                        {
                            ComputeReferences = true
                        };
                        var elemGeometry = data.GetRebarUpdateCurvesData().GetCustomConstraints()[0].GetTargetElement(0)
                            .get_Geometry(geomOptions);
                        if (elemGeometry == null)
                            return false;
                        hostFaces = GetFacesFromElement(elemGeometry);
                    }

                    foreach (var bar in allbars)
                    {
                        faces.Add(SearchForFace(bar, hostFaces, iBarEnd));
                    }

                    List<Reference> refs = new();
                    foreach (var face in faces)
                    {
                        if (face.Face.Reference != null && !refs.Contains(face.Face.Reference))
                            refs.Add(face.Face.Reference);
                    }

                    if (refs.Count > 0)
                    {
                        if (iBarEnd == 0)
                            data.CreateStartConstraint(refs, false, 0.0);
                        else
                            data.CreateEndConstraint(refs, false, 0.0);
                    }
                }
                else
                {
                    for (var nTarget = 0; nTarget < constraint.NumberOfTargets; nTarget++)
                    {
                        var trf = Transform.Identity;
                        var constrainedFace = constraint.GetTargetHostFaceAndTransform(nTarget, trf);
                        if (constrainedFace == null)
                            continue;
                        double dfOffset;
                        if (!SampleBrowserUtils.GetOffsetFromConstraintAtTarget(data.GetRebarUpdateCurvesData(), constraint, 0,
                                out dfOffset))
                            faces.Add(new TargetFace { Face = constrainedFace, Transform = trf, Offset = dfOffset });
                    }
                }

                // Try tangent extension first, then the bar segment itself.
                for (var idx = 0; idx < allbars.Count; idx++)
                {
                    XYZ intersection;
                    var barCurve = allbars[idx];
                    if (barCurve is not Line) // Straight bars only.
                        return false;
                    var tangent = Line.CreateUnbound(barCurve.GetEndPoint(iBarEnd),
                        barCurve.ComputeDerivatives(iBarEnd, true).BasisX.Normalize() * (iBarEnd == 0 ? -1 : 1));
                    var dfOffset = 0.0;
                    if (GetIntersection(tangent, faces, out intersection, out dfOffset) ||
                        GetIntersection(barCurve, faces, out intersection, out dfOffset))
                    {
                        Curve newCurve = null;
                        try
                        {
                            var barDir = (barCurve.GetEndPoint(1) - barCurve.GetEndPoint(0)).Normalize();
                            newCurve = iBarEnd == 0
                                ? Line.CreateBound(intersection - (barDir * dfOffset), barCurve.GetEndPoint(1))
                                : Line.CreateBound(barCurve.GetEndPoint(0), intersection + (barDir * dfOffset));
                        }
                        catch
                        {
                        }

                        if (newCurve != null)
                            allbars[idx] = newCurve;
                    }
                }
            }

            TargetFace firstFace = new();
            var constraints = data.GetRebarUpdateCurvesData().GetCustomConstraints();
            foreach (var constraint in constraints)
            {
                if ((BarHandle)constraint.GetCustomHandleTag() == BarHandle.FirstHandle)
                {
                    var tempTrf = Transform.Identity;
                    double dfOffset;
                    if (!SampleBrowserUtils.GetOffsetFromConstraintAtTarget(data.GetRebarUpdateCurvesData(), constraint, 0, out dfOffset))
                        return false;
                    firstFace = new TargetFace
                    {
                        Face = constraint.GetTargetHostFaceAndTransform(0, tempTrf),
                        Transform = tempTrf,
                        Offset = dfOffset
                    };
                    break;
                }
            }

            for (var ii = 0; ii < allbars.Count; ii++)
            {
                List<Curve> barCurve = new()
                { allbars[ii] };
                data.AddBarGeometry(barCurve);
                // Hook normals reset when bar geometry is replaced.
                for (var i = 0; i < 2; i++)
                {
                    var normal = ComputeNormal(allbars[ii], firstFace, i);
                    if (normal != null && !normal.IsZeroLength())
                        data.GetRebarUpdateCurvesData().SetHookPlaneNormalForBarIdx(i, ii, normal);
                }
            }

            return true;
        }

        private Rebar GetCurrentRebar(RebarUpdateCurvesData data)
        {
            var rebarId = data.GetRebarId();
            return data.GetDocument().GetElement(rebarId) as Rebar;
        }

        private CurveElement GetSelectedCurveElement(Rebar bar, RebarUpdateCurvesData data)
        {
            bar.GetFreeFormAccessor();
            var paramCurveId = bar.LookupParameter(AddSharedParams.CurveIdName);
            if (paramCurveId == null)
                return null;
            var id = ElementId.Parse(paramCurveId.AsString());
            return data.GetDocument().GetElement(id) as CurveElement;
        }

        private TargetFace SearchForFace(Curve curve, List<TargetFace> faces, int iEnd)
        {
            TargetFace bestFace = new();
            var minDistance = double.MaxValue;
            var tangent = Line.CreateUnbound(curve.GetEndPoint(iEnd),
                curve.ComputeDerivatives(iEnd, true).BasisX.Normalize() * (iEnd == 0 ? -1 : 1));
            foreach (var hostFace in faces)
            {
                IntersectionResultArray results;
                if (hostFace.Face.Intersect(tangent.CreateTransformed(hostFace.Transform.Inverse), out results) ==
                    SetComparisonResult.Overlap)
                    foreach (IntersectionResult intersect in results)
                    {
                        var distance = hostFace.Transform.OfPoint(intersect.XYZPoint)
                            .DistanceTo(curve.GetEndPoint(iEnd));
                        // Ignore intersections behind the tangent origin (param < 0).
                        var param = tangent.Project(hostFace.Transform.OfPoint(intersect.XYZPoint)).Parameter;
                        if (param >= 0 && distance < minDistance)
                        {
                            bestFace = hostFace;
                            minDistance = distance;
                        }
                    }

                if (hostFace.Face.Intersect(curve.CreateTransformed(hostFace.Transform.Inverse), out results) ==
                    SetComparisonResult.Overlap)
                    foreach (IntersectionResult intersect in results)
                    {
                        var distance = hostFace.Transform.OfPoint(intersect.XYZPoint)
                            .DistanceTo(curve.GetEndPoint(iEnd));
                        if (distance < minDistance)
                        {
                            bestFace = hostFace;
                            minDistance = distance;
                        }
                    }
            }

            return bestFace;
        }

        private XYZ ComputeNormal(Curve curve, TargetFace face, int iEnd)
        {
            var curveTangent = curve.ComputeDerivatives(iEnd, true).BasisX.Normalize();
            var refPoint = curve.GetEndPoint(iEnd);
            var proj = face.Face.Project(face.Transform.Inverse.OfPoint(refPoint));
            return proj == null ? null : face.Face.ComputeNormal(proj.UVPoint).Negate().CrossProduct(curveTangent);
        }

        private bool GetIntersection(Curve curve, List<TargetFace> faces, out XYZ intersection,
            out double offsetFromFace)
        {
            intersection = new XYZ();
            offsetFromFace = 0.0;

            foreach (var face in faces)
            {
                IntersectionResultArray results;
                var curveTrf = curve.CreateTransformed(face.Transform.Inverse);
                if (face.Face.Intersect(curveTrf, out results) == SetComparisonResult.Overlap)
                    foreach (IntersectionResult result in results)
                    {
                        if (curveTrf.Project(result.XYZPoint).Parameter < 0)
                            continue;
                        intersection = face.Transform.OfPoint(result.XYZPoint);
                        offsetFromFace = face.Offset;
                        return true;
                    }
            }

            return false;
        }

        private bool GenerateSet(Curve firstCurve, Curve lastCurve, RebarLayoutRule layout, int nbOfBars,
            double spacing, ref List<Curve> curves, Curve overrideCurve)
        {
            try
            {
                var startLine = Line.CreateBound(firstCurve.Evaluate(0, true), lastCurve.Evaluate(0, true));
                var endLine = Line.CreateBound(firstCurve.Evaluate(1, true), lastCurve.Evaluate(1, true));
                var barNumber = nbOfBars - 2;
                //see how many bar we can fit
                var numberOfBarsWhichCanFit = (int)((startLine.Length - double.Epsilon) / spacing) + 2;
                switch (layout)
                {
                    case RebarLayoutRule.NumberWithSpacing when numberOfBarsWhichCanFit != nbOfBars:
                        return false;
                    case RebarLayoutRule.MaximumSpacing:
                    case RebarLayoutRule.MinimumClearSpacing:
                        barNumber = numberOfBarsWhichCanFit - 2;
                        break;
                }

                for (var ii = 0; ii < barNumber; ii++)
                {
                    var nEval = (ii + 1) / (double)(barNumber + 1);
                    var newBar = overrideCurve != null
                        ? overrideCurve.CreateTransformed(
                            Transform.CreateTranslation(startLine.Evaluate(nEval, true) - overrideCurve.GetEndPoint(0)))
                        : Line.CreateBound(startLine.Evaluate(nEval, true), endLine.Evaluate(nEval, true));
                    curves.Add(newBar);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool AlignBars(ref Curve firstBar, ref Curve secondBar)
        {
            try
            {
                if (firstBar.Evaluate(0, true).DistanceTo(secondBar.Evaluate(0, true)) >
                    firstBar.Evaluate(0, true).DistanceTo(secondBar.Evaluate(1, true)))
                    secondBar = Line.CreateBound(secondBar.GetEndPoint(1), secondBar.GetEndPoint(0));
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Curve GetOffsetCurveAtIntersection(TargetFace firstFace, TargetFace secondFace)
        {
            Curve firstCurve;
            var result = firstFace.Face.Intersect(secondFace.Face, out firstCurve);
            // if faces do not intersect, or do not return a Line, then consider the input invalid and return error
            if (result == FaceIntersectionFaceResult.NonIntersecting || firstCurve is not Line)
                return null;
            var pointOnCurve = firstCurve.Evaluate(0, true);
            var firstOffsetVec = firstFace.Face.ComputeNormal(firstFace.Face.Project(pointOnCurve).UVPoint).Normalize();
            var secondOffsetVec = secondFace.Face.ComputeNormal(secondFace.Face.Project(pointOnCurve).UVPoint)
                .Normalize();
            var offsetVec = (firstOffsetVec * firstFace.Offset) + (secondOffsetVec * secondFace.Offset);
            var offsetTrf = Transform.CreateTranslation(offsetVec);
            return firstCurve.CreateTransformed(offsetTrf.Multiply(firstFace.Transform));
        }

        private List<TargetFace> GetFacesFromElement(GeometryElement geometryElement, Transform trf = null)
        {
            List<TargetFace> result = new();
            if (geometryElement != null)
                foreach (var geometryObject in geometryElement)
                {
                    var solid = geometryObject as Solid;
                    if (solid == null)
                    {
                        var geometryInstance = geometryObject as GeometryInstance;
                        if (geometryInstance != null)
                        {
                            var transform = geometryInstance.Transform;
                            var nestedFaces = GetFacesFromElement(geometryInstance.SymbolGeometry, transform);
                            if (nestedFaces == null)
                                return null;
                            foreach (var nestedFace in nestedFaces)
                            {
                                result.Add(nestedFace);
                            }
                        }
                    }
                    else
                    {
                        foreach (Face face in solid.Faces)
                        {
                            result.Add(new TargetFace
                            { Face = face, Transform = trf ?? Transform.Identity });
                        }
                    }
                }

            return result.Count > 0 ? result : null;
        }
    }
}
