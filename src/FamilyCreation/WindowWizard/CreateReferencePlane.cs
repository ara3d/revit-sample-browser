// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    /// <summary>
    ///     The class is used to create ReferencePlane
    /// </summary>
    public class CreateRefPlane
    {
        /// <summary>
        ///     This method is used to create ReferencePlane along to a host referenceplane with a offset parameter
        /// </summary>
        /// <param name="doc">the document</param>
        /// <param name="host">the host ReferencePlane</param>
        /// <param name="view">the view</param>
        /// <param name="offSet">the offset of the host</param>
        /// <param name="cutVec">the cutVec of the ReferencePlane</param>
        /// <param name="name">the name of the ReferencePlane</param>
        /// <returns>ReferencePlane</returns>
        public Autodesk.Revit.DB.ReferencePlane Create(Document doc, Autodesk.Revit.DB.ReferencePlane host, View view,
            XYZ offSet, XYZ cutVec, string name)
        {
            var bubbleEnd = new XYZ();
            var freeEnd = new XYZ();
            try
            {
                var refPlane = host;
                if (refPlane != null)
                {
                    bubbleEnd = refPlane.BubbleEnd.Add(offSet);
                    freeEnd = refPlane.FreeEnd.Add(offSet);
                    var subTransaction = new SubTransaction(doc);
                    subTransaction.Start();
                    refPlane = doc.FamilyCreate.NewReferencePlane(bubbleEnd, freeEnd, cutVec, view);
                    refPlane.Name = name;
                    subTransaction.Commit();
                }

                return refPlane;
            }
            catch
            {
                return null;
            }
        }
    }
}
