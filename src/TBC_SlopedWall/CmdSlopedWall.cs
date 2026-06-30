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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

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
            var profile = new List<Curve>(4); // 2012

            double length = 10;
            double heightStart = 5;
            double heightEnd = 8;

            var p = XYZ.Zero;
            var q = new XYZ(length, 0.0, 0.0);

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

            using var t = new Transaction(doc);
            t.Start("Create Sloped Wall");

            //Wall wall = doc.Create.NewWall( profile, false ); // 2012
            var wall = Wall.Create(doc, profile, false); // 2013

            t.Commit();

            return Result.Succeeded;
        }
    }

    #region TestWall

    /// <summary>
    ///     Answer to http://forums.autodesk.com/t5/Autodesk-Revit-API/Why-cannot-create-the-wall-by-following-profiles/m-p/3186912/highlight/false#M2351
    ///     1. Please look at the following two commands in The Building Coder samples: CmdCreateGableWall and CmdSlopedWall.
    ///     2. I actually went and tested your code, and I see that you are checking the verticality using an epsilon value of 1.e-5. I would suggest raising that to 1.e-9, which is more
    ///     like what Revit used internally. I did so, and then the IsVertical test fails on your three given points, so the message box saying "not vertical" is displayed. After that,
    ///     Revit displays an error message saying "Can't make Extrusion.я" At the same time, I temporarily see three model lines on the graphics screen. I have to select Cancel in the
    ///     Revit message box, though, and afterwards the three model lines disappear again.
    ///     So I would say the problem lies in your three points. They are not well enough aligned. You need higher precision.
    /// </summary>


    #endregion // TestWall
}