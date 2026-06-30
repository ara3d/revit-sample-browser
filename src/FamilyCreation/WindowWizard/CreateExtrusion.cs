// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class CreateExtrusion
    {
        private readonly Application m_appCreator;
        private readonly Document m_document;
        private readonly FamilyItemFactory m_familyCreator;

        public CreateExtrusion(Autodesk.Revit.ApplicationServices.Application app, Document doc)
        {
            m_document = doc;
            m_appCreator = app.Create;
            m_familyCreator = doc.FamilyCreate;
        }

        public CurveArray CreateRectangle(double left, double right, double top, double bottom, double yCoordinate)
        {
            var curveArray = m_appCreator.NewCurveArray();
            try
            {
                var p0 = new XYZ(left, yCoordinate, top);
                var p1 = new XYZ(right, yCoordinate, top);
                var p2 = new XYZ(right, yCoordinate, bottom);
                var p3 = new XYZ(left, yCoordinate, bottom);
                var line1 = Line.CreateBound(p0, p1);
                var line2 = Line.CreateBound(p1, p2);
                var line3 = Line.CreateBound(p2, p3);
                var line4 = Line.CreateBound(p3, p0);
                curveArray.Append(line1);
                curveArray.Append(line2);
                curveArray.Append(line3);
                curveArray.Append(line4);
                return curveArray;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public CurveArray CreateCurveArrayByOffset(CurveArray origin, double offset)
        {
            var counter = 0;
            var curveArr = m_appCreator.NewCurveArray();
            var offsetx = new XYZ(offset, 0, 0);
            var offsetz = new XYZ(0, 0, offset);
            var p0 = new XYZ();
            var p1 = new XYZ();
            var p2 = new XYZ();
            var p3 = new XYZ();
            foreach (Curve curve in origin)
            {
                var temp = curve as Line;
                if (temp != null)
                {
                    switch (counter)
                    {
                        case 0:
                            p0 = temp.GetEndPoint(0).Subtract(offsetz).Subtract(offsetx);
                            break;
                        case 1:
                            p1 = temp.GetEndPoint(0).Subtract(offsetz).Add(offsetx);
                            break;
                        case 2:
                            p2 = temp.GetEndPoint(0).Add(offsetx).Add(offsetz);
                            break;
                        default:
                            p3 = temp.GetEndPoint(0).Subtract(offsetx).Add(offsetz);
                            break;
                    }
                }

                counter++;
            }

            var line = Line.CreateBound(p0, p1);
            curveArr.Append(line);
            line = Line.CreateBound(p1, p2);
            curveArr.Append(line);
            line = Line.CreateBound(p2, p3);
            curveArr.Append(line);
            line = Line.CreateBound(p3, p0);
            curveArr.Append(line);
            return curveArr;
        }

        public Extrusion NewExtrusion(CurveArrArray curveArrArray, Autodesk.Revit.DB.ReferencePlane workPlane,
            double startOffset, double endOffset)
        {
            try
            {
                var subTransaction = new SubTransaction(m_document);
                subTransaction.Start();
                var sketch = SketchPlane.Create(m_document, workPlane.GetPlane());
                var rectExtrusion =
                    m_familyCreator.NewExtrusion(true, curveArrArray, sketch, Math.Abs(endOffset - startOffset));
                rectExtrusion.StartOffset = startOffset;
                rectExtrusion.EndOffset = endOffset;
                subTransaction.Commit();
                return rectExtrusion;
            }
            catch
            {
                return null;
            }
        }
    }
}
