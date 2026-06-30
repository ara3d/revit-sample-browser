#region Header

//
// CmdNewSpotElevation.cs - insert a new spot elevation on top surface of beam
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewSpotElevation : IExternalCommand
    {
        private void CreatePerspectiveViewMatchingCamera(
            Document doc,
            XYZ camera_position,
            XYZ target)
        {
            using Transaction trans = new(doc);
            trans.Start("Map Forge Viewer Camera");

            var typ
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .First(
                        x => x.ViewFamily.Equals(
                            ViewFamily.ThreeDimensional));

            var view3D = View3D.CreatePerspective(
                doc, typ.Id);

            Random rnd = new();
            view3D.Name = $"Camera{rnd.Next()}";

            var up = XYZ.BasisZ;

            var sightDir = target.Subtract(camera_position).Normalize();

            ViewOrientation3D orientation = new(
                camera_position, up, sightDir);

            view3D.SetOrientation(orientation);

            view3D.LookupParameter("Far Clip Active")
                .Set(0);

            view3D.LookupParameter("Crop Region Visible")
                .Set(1);

            view3D.LookupParameter("Crop View")
                .Set(1);

            trans.Commit();
        }

        private void ViewApiCreateViewSample()
        {
            Document doc = null;
            Level level = null;
            var viewFamily3d = ElementId.InvalidElementId;

            var viewFamilyTypes
                = from e in new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                  let type = e as ViewFamilyType
                  where type.ViewFamily == ViewFamily.CeilingPlan
                  select type;

            var ceilingPlan = ViewPlan.Create(doc,
                viewFamilyTypes.First().Id, level.Id);

            ceilingPlan.Name = $"New Ceiling Plan for {level.Name}";

            ceilingPlan.DetailLevel = ViewDetailLevel.Fine;

            var view = View3D.CreateIsometric(
                doc, viewFamily3d);

            XYZ eyePosition = new(10, 10, 10);
            XYZ upDirection = new(-1, 0, 1);
            XYZ forwardDirection = new(1, 0, 1);

            view.SetOrientation(new ViewOrientation3D(
                eyePosition, upDirection, forwardDirection));
        }

        private Application _create;
        private bool NewSpotElevation(Document doc)
        {
            if (Util.GetFirstElementOfTypeNamed(
                doc, typeof(View), "West") is not View westView)
            {
                Util.ErrorMsg("No view found named 'West'.");
                return false;
            }

            ElementId instanceId = new((Int64)230298);

            if (doc.GetElement(
                instanceId) is not FamilyInstance beam)
            {
                Util.ErrorMsg("Beam 230298 not found.");
                return false;
            }

            using Transaction t = new(doc);
            t.Start("Create Spot Elevation");

            var topReference
                = Util.FindTopMostReference(beam);

            var lCurve = beam.Location
                as LocationCurve;

            for (var i = 0; i < 3; ++i)
            {
                var lCurvePnt = lCurve.Curve.Evaluate(
                    0.5 * i, true);

                var bendPnt = lCurvePnt.Add(
                    _create.NewXYZ(0, 1, 4));

                var endPnt = lCurvePnt.Add(
                    _create.NewXYZ(0, 2, 4));

                var d = doc.Create.NewSpotElevation(
                    westView, topReference, lCurvePnt, bendPnt,
                    endPnt, lCurvePnt, true);
            }

            t.Commit();

            return true;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;
            _create = app.Application.Create;

            return NewSpotElevation(doc)
                ? Result.Succeeded
                : Result.Failed;
        }
    }
}
