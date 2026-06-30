// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class CreateAlignment
    {
        private readonly Document m_document;
        private readonly FamilyItemFactory m_familyCreator;

        public CreateAlignment(Document doc)
        {
            m_document = doc;
            m_familyCreator = m_document.FamilyCreate;
        }

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
                SubTransaction subTransaction = new(m_document);
                subTransaction.Start();
                m_familyCreator.NewAlignment(view, pFace1.Reference, pFace2.Reference);
                subTransaction.Commit();
            }
        }
    }
}
