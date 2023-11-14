// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;

namespace RevitMultiSample.WindowWizard.CS
{
    /// <summary>
    ///     The class allows users to create alignment
    /// </summary>
    internal class CreateAlignment
    {
        /// <summary>
        ///     store the document
        /// </summary>
        private readonly Document m_document;

        /// <summary>
        ///     store the family item factory of creation
        /// </summary>
        private readonly FamilyItemFactory m_familyCreator;

        /// <summary>
        ///     The constructor of CreateAlignment class
        /// </summary>
        /// <param name="doc">the document</param>
        public CreateAlignment(Document doc)
        {
            m_document = doc;
            m_familyCreator = m_document.FamilyCreate;
        }

        /// <summary>
        ///     The method is used to create alignment between two faces
        /// </summary>
        /// <param name="view">the view</param>
        /// <param name="face1">face1</param>
        /// <param name="face2">face2</param>
        public void AddAlignment(View view, Face face1, Face face2)
        {
            PlanarFace pFace1 = null;
            PlanarFace pFace2 = null;
            if (face1 is PlanarFace planarFace)
                pFace1 = planarFace;
            if (face2 is PlanarFace face)
                pFace2 = face;
            if (pFace1 != null && pFace2 != null)
            {
                var subTransaction = new SubTransaction(m_document);
                subTransaction.Start();
                m_familyCreator.NewAlignment(view, pFace1.Reference, pFace2.Reference);
                subTransaction.Commit();
            }
        }
    }
}
