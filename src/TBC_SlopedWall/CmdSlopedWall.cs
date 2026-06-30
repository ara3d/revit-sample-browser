#region Header

//
// CmdSlopedWall.cs - create a sloped wall
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdSlopedWall : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            //Autodesk.Revit.Creation.Application ac
            //  = app.Application.Create;

            //CurveArray profile = ac.NewCurveArray(); // 2012
            List<Curve> profile = new(4); // 2012

            double length = 10;
            double heightStart = 5;
            double heightEnd = 8;

            var p = XYZ.Zero;
            XYZ q = new(length, 0.0, 0.0);

            //profile.Append( ac.NewLineBound( p, q ) ); // 2012
            profile.Add(Line.CreateBound(p, q)); // 2014

            p = q;
            q += heightEnd * XYZ.BasisZ;

            //profile.Append( ac.NewLineBound( p, q ) ); // 2012
            profile.Add(Line.CreateBound(p, q)); // 2014

            p = q;
            q = new XYZ(0.0, 0.0, heightStart);

            //profile.Append( ac.NewLineBound( p, q ) ); // 2012
            //profile.Add( ac.NewLineBound( p, q ) ); // 2013
            profile.Add(Line.CreateBound(p, q)); // 2014

            p = q;
            q = XYZ.Zero;

            //profile.Append( ac.NewLineBound( p, q ) ); // 2012
            //profile.Add( ac.NewLineBound( p, q ) ); // 2013
            profile.Add(Line.CreateBound(p, q)); // 2014

            using Transaction t = new(doc);
            t.Start("Create Sloped Wall");

            //Wall wall = doc.Create.NewWall( profile, false ); // 2012
            var wall = Wall.Create(doc, profile, false); // 2013

            t.Commit();

            return Result.Succeeded;
        }
    }

    #region TestWall

    // Wall profile points need ~1e-9 verticality tolerance, not 1e-5; see https://forums.autodesk.com/t5/Autodesk-Revit-API/Why-cannot-create-the-wall-by-following-profiles/m-p/3186912

    #endregion // TestWall
}