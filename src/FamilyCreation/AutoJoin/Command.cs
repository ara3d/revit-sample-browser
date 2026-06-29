// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AppCreation = Autodesk.Revit.Creation.Application;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoJoin.CS
{
    /// <summary>
    ///     This sample demonstrates how to automatically join geometry
    ///     between multiple generic forms for use in family modeling and massing.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public static AppCreation SAppCreation;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var trans = new Transaction(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.AutoJoin");
            trans.Start();
            if (null == SAppCreation)
                // share for class Intersection.
                SAppCreation = commandData.Application.Application.Create;

            var doc = commandData.Application.ActiveUIDocument;
            var solids
                = new CombinableElementArray();

            var es = new ElementSet();
            foreach (ElementId elemId in es)
            {
                es.Insert(doc.Document.GetElement(elemId));
            }

            if (0 < es.Size)
            {
                foreach (var elementId in doc.Selection.GetElementIds())
                {
                    var element = doc.Document.GetElement(elementId);
                    Trace.WriteLine(element.GetType().ToString());

                    switch (element)
                    {
                        case GenericForm gf when !gf.IsSolid:
                            continue;
                        case CombinableElement ce:
                            solids.Append(ce);
                            break;
                    }
                }

                if (solids.Size < 2)
                {
                    message = "At least 2 combinable elements should be selected.";
                    trans.RollBack();
                    return Result.Failed;
                }

                doc.Document.CombineElements(solids);

                //The selected generic forms are joined, whether or not they overlap.
                trans.Commit();
                return Result.Succeeded;
            }

            var autojoin = new AutoJoin();
            autojoin.Join(doc.Document);
            //All overlapping generic forms are joined.
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
