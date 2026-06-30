// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.CreateDimensions.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double Precision = 0.0000001;
        private string m_errorMessage = " ";
        private ExternalCommandData m_revit;
        private readonly ArrayList m_walls = [];

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            try
            {
                m_revit = revit;
                var view = m_revit.Application.ActiveUIDocument.Document.ActiveView;
                switch (view)
                {
                    case View3D view3D:
                        message += "Only create dimensions in 2D";
                        return Result.Failed;
                    case ViewSheet viewSheet:
                        message += "Only create dimensions in 2D";
                        return Result.Failed;
                }

                if (!AddDimension())
                {
                    message = m_errorMessage;
                    return Result.Failed;
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        private bool Initialize()
        {
            ElementSet selections = new();
            foreach (var elementId in m_revit.Application.ActiveUIDocument.Selection.GetElementIds())
            {
                selections.Insert(m_revit.Application.ActiveUIDocument.Document.GetElement(elementId));
            }

            if (0 == selections.Size)
            {
                m_errorMessage += "Please select Basic walls";
                return false;
            }

            foreach (Element e in selections)
            {
                if (e is Wall wall)
                {
                    if ("Basic" != wall.WallType.Kind.ToString()) continue;
                    m_walls.Add(wall);
                }
            }

            if (0 == m_walls.Count)
            {
                m_errorMessage += "Please select Basic walls";
                return false;
            }

            return true;
        }

        public bool AddDimension()
        {
            if (!Initialize()) return false;

            Transaction transaction = new(m_revit.Application.ActiveUIDocument.Document, "Add Dimensions");
            transaction.Start();
            foreach (var wall in m_walls)
            {
                if (wall is not Wall wallTemp) continue;

                var location = wallTemp.Location;
                if (location is not LocationCurve locationline) continue;


                Line newLine = null;

                ReferenceArray referenceArray = new();

                AnalyticalPanel analyticalModel = null;
                var document = wallTemp.Document;
                var assocManager =
                    AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
                if (assocManager != null)
                {
                    var associatedElementId = assocManager.GetAssociatedElementId(wallTemp.Id);
                    if (associatedElementId != ElementId.InvalidElementId)
                    {
                        var associatedElement = document.GetElement(associatedElementId);
                        if (associatedElement is not null and AnalyticalPanel panel)
                            analyticalModel = panel;
                    }
                }

                IList<Curve> activeCurveList = analyticalModel.GetOuterContour().ToList();
                foreach (var aCurve in activeCurveList)
                {
                    // find non-vertical curve from analytical model
                    if (aCurve.GetEndPoint(0).Z == aCurve.GetEndPoint(1).Z)
                        newLine = aCurve as Line;
                    if (aCurve.GetEndPoint(0).Z != aCurve.GetEndPoint(1).Z)
                    {
                        AnalyticalModelSelector amSelector = new(aCurve)
                        {
                            CurveSelector = AnalyticalCurveSelector.StartPoint
                        };

                        referenceArray.Append(analyticalModel.GetReference(amSelector));
                    }

                    if (2 == referenceArray.Size)
                        break;
                }

                if (referenceArray.Size != 2)
                {
                    m_errorMessage += "Did not find two references";
                    return false;
                }

                try
                {
                    var app = m_revit.Application;
                    var doc = app.ActiveUIDocument.Document;

                    XYZ p1 = new(
                        newLine.GetEndPoint(0).X + 5,
                        newLine.GetEndPoint(0).Y + 5,
                        newLine.GetEndPoint(0).Z);
                    XYZ p2 = new(
                        newLine.GetEndPoint(1).X + 5,
                        newLine.GetEndPoint(1).Y + 5,
                        newLine.GetEndPoint(1).Z);

                    var newLine2 = Line.CreateBound(p1, p2);
                    doc.Create.NewDimension(
                        doc.ActiveView, newLine2, referenceArray);
                }
                catch (Exception ex)
                {
                    m_errorMessage += ex.ToString();
                    return false;
                }
            }

            transaction.Commit();
            return true;
        }
    }
}
