//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.WindowWizard.CS
{   
    /// <summary>
    /// The class is used to create ReferencePlane
    /// </summary>
    class CreateRefPlane
    {
        #region Class Implementation
        /// <summary>
        /// This method is used to create ReferencePlane along to a host referenceplane with a offset parameter
        /// </summary>
        /// <param name="doc">the document</param>
        /// <param name="host">the host ReferencePlane</param>
        /// <param name="view">the view</param>
        /// <param name="offSet">the offset of the host</param>
        /// <param name="cutVec">the cutVec of the ReferencePlane</param>
        /// <param name="name">the name of the ReferencePlane</param>
        /// <returns>ReferencePlane</returns>
        public Autodesk.Revit.DB.ReferencePlane Create(Document doc, Autodesk.Revit.DB.ReferencePlane host, View view, XYZ offSet, XYZ cutVec, string name)
        {
            var bubbleEnd = new XYZ ();
            var freeEnd = new XYZ ();
            try
            {
                var refPlane = host as Autodesk.Revit.DB.ReferencePlane;
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
        #endregion
    }  
}
