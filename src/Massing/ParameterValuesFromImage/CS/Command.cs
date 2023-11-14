// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Color = System.Drawing.Color;

namespace Revit.SDK.Samples.ParameterValuesFromImage.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class used to set parameter values from image data
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetParameterValueWithImageData : IExternalCommand
    {
        private static AddInId _appId = new AddInId(new Guid("9F405E24-3799-4b56-828F-14842ABE4802"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var trans = new Transaction(doc, "Revit.SDK.Samples.ParameterValuesFromImage");
            trans.Start();
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
                                var param = familyinstance.LookupParameter("Grayscale");
                                if (param == null)
                                {
                                    trans.RollBack();
                                    throw new Exception("Panel family must have a Grayscale instance parameter");
                                }

                                var pixelColor = new Color();
                                try
                                {
                                    pixelColor = image.GetPixel(image.Width - v, image.Height - u);
                                    double grayscale = 255 - (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                                    if (grayscale == 0)
                                        doc.Delete(familyinstance.Id);
                                    else
                                        param.Set(grayscale / 255);
                                }
                                catch (Exception)
                                {
                                    //       TaskDialog.Show("Revit", "Exception: " + u + ", " + v);                                        
                                }
                            }
                        }
                    }
                }
            }

            doc.Regenerate();
            ;
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
