#region Header

//
// CmdNewDimensionLabel.cs - create a new dimension label in a family document
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

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     Create a new dimension label in a family document.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewDimensionLabel : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            if (!doc.IsFamilyDocument)
            {
                message = "Please run this command in afamily document.";
                return Result.Failed;
            }

            var creApp = app.Application.Create;
            var creDoc = doc.Create;

            using Transaction t = new(doc);
            t.Start("New Dimension Label");


            var skplane = Util.FindSketchPlane(doc, XYZ.BasisZ);

            if (null == skplane)
            {
                //Plane geometryPlane = creApp.NewPlane( XYZ.BasisZ, XYZ.Zero ); // 2016
                var geometryPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero); // 2017

                //skplane = doc.FamilyCreate.NewSketchPlane( geometryPlane ); // 2013

                skplane = SketchPlane.Create(doc, geometryPlane); // 2014
            }

            var length = 1.23;

            var start = XYZ.Zero;
            var end = creApp.NewXYZ(0, length, 0);


            var line = Line.CreateBound(start, end); // 2014

            var modelCurve
                = doc.FamilyCreate.NewModelCurve(
                    line, skplane);

            ReferenceArray ra = new();

            ra.Append(modelCurve.GeometryCurve.Reference);

            start = creApp.NewXYZ(length, 0, 0);
            end = creApp.NewXYZ(length, length, 0);

            line = Line.CreateBound(start, end);

            modelCurve = doc.FamilyCreate.NewModelCurve(
                line, skplane);

            ra.Append(modelCurve.GeometryCurve.Reference);

            start = creApp.NewXYZ(0, 0.2 * length, 0);
            end = creApp.NewXYZ(length, 0.2 * length, 0);

            line = Line.CreateBound(start, end);

            var dim
                = doc.FamilyCreate.NewLinearDimension(
                    doc.ActiveView, line, ra);

            var familyParam
                = doc.FamilyManager.AddParameter(
                    "length",
                    //BuiltInParameterGroup.PG_IDENTITY_DATA, // 2021
                    GroupTypeId.IdentityData, // 2022
                                              //ParameterType.Length, // 2021 
                    SpecTypeId.Length, // 2022
                    false);

            //dim.Label = familyParam; // 2013
            dim.FamilyLabel = familyParam; // 2014

            t.Commit();

            return Result.Succeeded;
        }
    }
}