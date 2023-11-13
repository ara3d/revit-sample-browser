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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ParameterValuesFromImage.CS
{
    /// <summary>
    /// A class inherits IExternalCommand interface.
    /// This class used to set parameter values from image data
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class SetParameterValueWithImageData : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("9F405E24-3799-4b56-828F-14842ABE4802"));

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            var trans = new Transaction(doc, "Revit.SDK.Samples.ParameterValuesFromImage");
            trans.Start();
            Parameter param = null;
            var image = new Bitmap(doc.PathName + "_grayscale.bmp");
            var collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfClass(typeof(DividedSurface)).ToElements();
            foreach (var element in collection)
            {
                var ds = element as DividedSurface;
                var gn = new GridNode();
                for (var u = 0; u < ds.NumberOfUGridlines; u++)
                {
                    gn.UIndex = u;
                    for (var v = 0; v < ds.NumberOfVGridlines; v++)
                    {
                        gn.VIndex = v;
                        if (ds.IsSeedNode(gn))
                        {
                            var familyinstance = ds.GetTileFamilyInstance(gn, 0);
                            if (familyinstance != null)
                            {
                               param = familyinstance.LookupParameter("Grayscale");
                                if (param == null)
                                {
                                    trans.RollBack();
                                    throw new Exception("Panel family must have a Grayscale instance parameter");
                                }
                                else
                                {
                                    var pixelColor = new System.Drawing.Color();
                                    try
                                    {
                                        pixelColor = image.GetPixel(image.Width - v, image.Height - u);
                                        double grayscale = 255 - ((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                                        if (grayscale == 0)
                                        {
                                            doc.Delete(familyinstance.Id);
                                        }
                                        else
                                        {
                                            param.Set(grayscale / 255);
                                        }
                                    }
                                    catch (System.Exception)
                                    {
                                        //       TaskDialog.Show("Revit", "Exception: " + u + ", " + v);                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
            doc.Regenerate(); ;
            trans.Commit();
            return Result.Succeeded;
        }
    }

}
