// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class CreateDimension
    {
        private Application m_application;
        private readonly Document m_document;

        public CreateDimension(Application app, Document doc)
        {
            m_application = app;
            m_document = doc;
        }

        public Dimension AddDimension(View view, Autodesk.Revit.DB.ReferencePlane refPlane1,
            Autodesk.Revit.DB.ReferencePlane refPlane2, Autodesk.Revit.DB.ReferencePlane refPlane)
        {
            var startPoint = new XYZ();
            var endPoint = new XYZ();
            var refArray = new ReferenceArray();
            var ref1 = refPlane1.GetReference();
            var ref2 = refPlane2.GetReference();
            var ref3 = refPlane.GetReference();
            startPoint = refPlane1.FreeEnd;
            endPoint = refPlane2.FreeEnd;
            var line = Line.CreateBound(startPoint, endPoint);
            if (null != ref1 && null != ref2 && null != ref3)
            {
                refArray.Append(ref1);
                refArray.Append(ref3);
                refArray.Append(ref2);
            }

            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            var dim = m_document.FamilyCreate.NewDimension(view, line, refArray);
            subTransaction.Commit();
            return dim;
        }

        public Dimension AddDimension(View view, Autodesk.Revit.DB.ReferencePlane refPlane, Face face)
        {
            var startPoint = new XYZ();
            var endPoint = new XYZ();
            var refArray = new ReferenceArray();
            var ref1 = refPlane.GetReference();
            var pFace = face as PlanarFace;
            var ref2 = pFace.Reference;
            if (null != ref1 && null != ref2)
            {
                refArray.Append(ref1);
                refArray.Append(ref2);
            }

            startPoint = refPlane.FreeEnd;
            endPoint = new XYZ(startPoint.X, pFace.Origin.Y, startPoint.Z);
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            var line = Line.CreateBound(startPoint, endPoint);
            var dim = m_document.FamilyCreate.NewDimension(view, line, refArray);
            subTransaction.Commit();
            return dim;
        }

        public Dimension AddDimension(View view, Face face1, Face face2)
        {
            var startPoint = new XYZ();
            var endPoint = new XYZ();
            var refArray = new ReferenceArray();
            var pFace1 = face1 as PlanarFace;
            var ref1 = pFace1.Reference;
            var pFace2 = face2 as PlanarFace;
            var ref2 = pFace2.Reference;
            if (null != ref1 && null != ref2)
            {
                refArray.Append(ref1);
                refArray.Append(ref2);
            }

            startPoint = pFace1.Origin;
            endPoint = new XYZ(startPoint.X, pFace2.Origin.Y, startPoint.Z);
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            var line = Line.CreateBound(startPoint, endPoint);
            var dim = m_document.FamilyCreate.NewDimension(view, line, refArray);
            subTransaction.Commit();
            return dim;
        }
    }
}
