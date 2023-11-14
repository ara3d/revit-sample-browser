// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.RebarFreeForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                if (doc == null)
                    return Result.Failed;

                //Fetch a RebarBarType element to be used in Rebar creation.
                var fec = new FilteredElementCollector(doc).OfClass(typeof(RebarBarType));
                if (fec.GetElementCount() <= 0)
                    return Result.Failed;
                var barType = fec.FirstElement() as RebarBarType;
                CurveElement curveElem = null;
                using (var tran = new Transaction(doc, "Create Rebar"))
                {
                    Element host = null;
                    var sel = commandData.Application.ActiveUIDocument.Selection;
                    try
                    {
                        //Select structural Host.
                        var hostRef = sel.PickObject(ObjectType.Element, "Select Host");
                        host = doc.GetElement(hostRef.ElementId);
                        if (host == null)
                            return Result.Failed;
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                        return Result.Failed;
                    }

                    try
                    {
                        //Select curve element
                        var lineRef = sel.PickObject(ObjectType.Element, "Select Model curve");
                        curveElem = doc.GetElement(lineRef.ElementId) as CurveElement;
                    }
                    catch (Exception)
                    {
                        curveElem = null;
                    }

                    tran.Start();

                    // Create Rebar Free Form by specifying the GUID defining the custom external server.
                    // The Rebar element returned needs to receive constraints, so that regeneration can
                    // call the custom geometry calculations and create the bars 
                    var rebar = Rebar.CreateFreeForm(doc, RebarUpdateServer.SampleGuid, barType, host);
                    // Get all bar handles to set constraints to them, so that the bar can generate its geometry
                    var rManager = rebar.GetRebarConstraintsManager();
                    var handles = rManager.GetAllHandles();

                    // if bar has no handles then the server can't generate rebar geometry 
                    if (handles.Count <= 0)
                    {
                        tran.RollBack();
                        return Result.Failed;
                    }

                    // iterate through the rebar handles and prompt for face selection for each of them, to get user input
                    foreach (var handle in handles)
                    {
                        if (handle.GetHandleType() == RebarHandleType.StartOfBar ||
                            handle.GetHandleType() == RebarHandleType.EndOfBar)
                            continue; // Start handle and end handle will receive constraints from the custom external server execution
                        try
                        {
                            var reference = sel.PickObject(ObjectType.Face,
                                "Select face for " + handle.GetHandleName());
                            if (reference == null)
                                continue;
                            // create constraint using the picked faces and set it to the associated handle
                            var refs = new List<Reference> { reference };
                            var constraint = RebarConstraint.Create(handle, refs, true, 0.0);
                            rManager.SetPreferredConstraintForHandle(handle, constraint);
                        }
                        catch (Exception e)
                        {
                            message = e.Message;
                            tran.RollBack();
                            return Result.Cancelled;
                        }
                    }

                    try
                    {
                        //here we add a value to the shared parameter and add it to the regeneration dependencies
                        var newSharedParam = rebar.LookupParameter(AddSharedParams.ParamName);
                        var newSharedParam2 = rebar.LookupParameter(AddSharedParams.CurveIdName);
                        if (newSharedParam != null && newSharedParam2 != null)
                        {
                            newSharedParam.Set(0);
                            newSharedParam2.Set(curveElem == null
                                ? ElementId.InvalidElementId.ToString()
                                : curveElem.Id.ToString());

                            var accesRebar = rebar.GetFreeFormAccessor();
                            accesRebar.AddUpdatingSharedParameter(newSharedParam.Id);
                            accesRebar.AddUpdatingSharedParameter(newSharedParam2.Id);
                        }
                        // The AddSharedParams command should be executed to create and bind these parameters to rebar.
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        tran.RollBack();
                        return Result.Cancelled;
                    }

                    tran.Commit();
                    return Result.Succeeded;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
