// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = System.Drawing.Color;

namespace Ara3D.RevitSampleBrowser.Massing.ParameterValuesFromImage.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SetParameterValueWithImageData : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("9F405E24-3799-4b56-828F-14842ABE4802"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction trans = new(doc, "Ara3D.RevitSampleBrowser.ParameterValuesFromImage");
            trans.Start();
            Bitmap image = new($"{doc.PathName}_grayscale.bmp");
            FilteredElementCollector collector = new(doc);
            ICollection<Element> collection = collector.OfClass(typeof(DividedSurface)).ToElements();
            foreach (var element in collection)
            {
                var ds = element as DividedSurface;
                GridNode gn = new();
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

                                Color pixelColor = new();
                                try
                                {
                                    pixelColor = image.GetPixel(image.Width - v, image.Height - u);
                                    double grayscale = 255 - ((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
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
