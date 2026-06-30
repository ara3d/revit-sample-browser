#region Header

//
// CmdRectDuctCorners.cs - determine the corners of a rectangular duct
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdRectDuctCorners : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            if (ProductType.MEP != app.Application.Product)
            {
                message = "Please run this command in Revit MEP.";
                return Result.Failed;
            }

            //SelElementSet sel = uidoc.Selection.Elements; // 2014

            var ids = uidoc.Selection.GetElementIds(); // 2015

            //if( 0 == sel.Size ) // 2014

            if (0 == ids.Count) // 2015
            {
                message = "Please select some rectangular ducts.";
                return Result.Failed;
            }

            var log = $"{Assembly.GetExecutingAssembly().Location}.{DateTime.Now:yyyyMMdd}.log";

            if (File.Exists(log)) File.Delete(log);

            TraceListener listener
                = new TextWriterTraceListener(log);

            Trace.Listeners.Add(listener);

            try
            {
                Trace.WriteLine("Begin");

                //foreach( Duct duct in sel ) // 2014

                foreach (var id in ids) // 2015
                    if (doc.GetElement(id) is not Duct duct)
                    {
                        Trace.TraceError("The selection is not a duct!");
                    }
                    else
                    {
                        Trace.WriteLine("========================");
                        Trace.WriteLine($"Duct: Id = {duct.Id.Value}");

                        Util.AnalyseDuct(duct);
                    }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                Trace.Flush();
                listener.Close();
                Trace.Close();
                Trace.Listeners.Remove(listener);
            }

            return Result.Failed;
        }
    }
}