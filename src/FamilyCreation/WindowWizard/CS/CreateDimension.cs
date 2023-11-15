// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.WindowWizard.CS
{
    /// <summary>
    ///     The class allows users to create dimension using Document.FamilyCreate.NewDimension() function
    /// </summary>
    internal class CreateDimension
    {
        /// <summary>
        ///     store the application
        /// </summary>
        private Application m_application;

        /// <summary>
        ///     store the document
        /// </summary>
        private readonly Document m_document;

        /// <summary>
        ///     constructor of CreateDimension class
        /// </summary>
        /// <param name="app">the application</param>
        /// <param name="doc">the document</param>
        public CreateDimension(Application app, Document doc)
        {
            m_application = app;
            m_document = doc;
        }

        /// <summary>
        ///     This method is used to create dimension among three reference planes
        /// </summary>
        /// <param name="view">the view</param>
        /// <param name="refPlane1">the first reference plane</param>
        /// <param name="refPlane2">the second reference plane</param>
        /// <param name="refPlane">the middle reference plane</param>
        /// <returns>the new dimension</returns>
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

        /// <summary>
        ///     The method is used to create dimension between referenceplane and face
        /// </summary>
        /// <param name="view">the view in which the dimension is created</param>
        /// <param name="refPlane">the reference plane</param>
        /// <param name="face">the face</param>
        /// <returns>the new dimension</returns>
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

        /// <summary>
        ///     The method is used to create dimension between two faces
        /// </summary>
        /// <param name="view">the view</param>
        /// <param name="face1">the first face</param>
        /// <param name="face2">the second face</param>
        /// <returns>the new dimension</returns>
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
