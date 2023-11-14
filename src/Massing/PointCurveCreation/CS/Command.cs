//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PointCurveCreation.CS
{
    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create reference points following parabolic arcs 
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class PointsParabola : IExternalCommand
    {
        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        static AddInId appId = new AddInId(new Guid("B6FBC0C1-F3AE-4ffa-AB46-B4CF94304827"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "PointsParabola");
            transaction.Start();
            double yctr = 0;
            XYZ xyz = null;
            ReferencePoint rp = null;
            var power = 1.2;
            while (power < 1.5)
            {
                double xctr = 0;
                double zctr = 0;
                while (zctr < 100)
                {
                    zctr = Math.Pow(xctr, power);
                    xyz = new XYZ(xctr, yctr, zctr);
                    rp = doc.FamilyCreate.NewReferencePoint(xyz);
                    if (xctr > 0)
                    {
                        xyz = new XYZ(-xctr, yctr, zctr);
                        rp = doc.FamilyCreate.NewReferencePoint(xyz);
                    }
                    xctr++;
                }
                power = power + 0.1;
                yctr = yctr + 50;
                zctr = 0;
            }
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create reference points constrained to a model curve
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class PointsOnCurve : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("22D07F77-A3F7-490c-B0D8-0EC10D8DE7C7"));
        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "PointsOnCurve");
            transaction.Start();
            var start = new XYZ(0, 0, 0);
            var end = new XYZ(50, 50, 0);
            var line = Line.CreateBound(start, end);
            var geometryPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, start);
            var skplane = SketchPlane.Create(doc, geometryPlane);
            var modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);

            for (var i = 0.1; i <= 1; i = i + 0.1)
            {
                var locationOnCurve = new PointLocationOnCurve(PointOnCurveMeasurementType.NormalizedCurveParameter, i, PointOnCurveMeasureFrom.Beginning);
                var poe = app.Create.NewPointOnEdge(modelcurve.GeometryCurve.Reference, locationOnCurve);
                var rp2 = doc.FamilyCreate.NewReferencePoint(poe);
            }
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create reference points based on comma-delimited XYZ data in a text file
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class PointsFromTextFile : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("C6D0F4DB-81C3-4927-9D68-8936D6EE67DD"));

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "PointsParabola");
            transaction.Start();
            var filename = "sphere.csv";
            var filepath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (File.Exists(filepath + "\\" + filename))
            {
                var readFile = new StreamReader(filepath + "\\" + filename);
                string line;
                while ((line = readFile.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    var xyz = new XYZ(Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));
                    var rp = doc.FamilyCreate.NewReferencePoint(xyz);
                }
            }
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create curve based on points placed using the equation y=cos(x)
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class SineCurve : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("F18A831C-AE42-43cc-91FD-6B5D461A1AC7"));

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "PointsParabola");
            transaction.Start();
            var pnt_ctr = 0;
            double xctr = 0;
            var xyz = new XYZ();
            var rparray = new ReferencePointArray();
            while (pnt_ctr < 500)
            {
                xyz = new XYZ(xctr, 0, (Math.Cos(xctr)) * 10);
                var rp = doc.FamilyCreate.NewReferencePoint(xyz);
                rparray.Append(rp);
                xctr = xctr + 0.1;
                pnt_ctr++;
            }
            var curve = doc.FamilyCreate.NewCurveByPoints(rparray);
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create curve based on points placed using the equation y=ScalingFactor * CosH(x/ScalingFactor)
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CatenaryCurve : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("817C2A99-BF00-4029-86F3-2D10550F1410"));

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "CatenaryCurve");
            transaction.Start();
            for (double scalingFactor = 1; scalingFactor <= 2; scalingFactor = scalingFactor + 0.5)
            {
                var rpArray = new ReferencePointArray();
                for (double x = -5; x <= 5; x = x + 0.5)
                {
                    var y = scalingFactor * Math.Cosh(x / scalingFactor);
                    if (y < 50)
                    {
                        var rp = doc.FamilyCreate.NewReferencePoint(new XYZ(x, y, 0));
                        rpArray.Append(rp);
                    }
                }
                var cbp = doc.FamilyCreate.NewCurveByPoints(rpArray);
            }
            transaction.Commit();

            return Result.Succeeded;
        }
    }


    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to create loft form based on curves and points created using the equation z = cos(x) + cos(y)
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CyclicSurface : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("3F926F3E-D93A-41cd-9ABF-A31594A827B3"));

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var cdata = commandData;
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "CyclicSurface");
            transaction.Start();
            var xyz = new XYZ();
            var refArAr = new ReferenceArrayArray();
            var x = 0;
            double z = 0;
            while (x < 800)
            {
                var rpAr = new ReferencePointArray();
                var y = 0;
                while (y < 800)
                {
                    z = 50 * (Math.Cos((Math.PI / 180) * x) + Math.Cos((Math.PI / 180) * y));
                    xyz = new XYZ(x, y, z);
                    var rp = doc.FamilyCreate.NewReferencePoint(xyz);
                    rpAr.Append(rp);
                    y = y + 40;
                }
                var curve = doc.FamilyCreate.NewCurveByPoints(rpAr);
                var refAr = new ReferenceArray();
                refAr.Append(curve.GeometryCurve.Reference);
                refArAr.Append(refAr);
                x = x + 40;
            }
            var form = doc.FamilyCreate.NewLoftForm(true, refArAr);
            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
