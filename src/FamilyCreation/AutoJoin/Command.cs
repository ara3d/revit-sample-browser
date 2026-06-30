// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using AppCreation = Autodesk.Revit.Creation.Application;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoJoin.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public static AppCreation SAppCreation;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            Transaction trans = new(commandData.Application.ActiveUIDocument.Document,
                "Ara3D.RevitSampleBrowser.AutoJoin");
            trans.Start();
            if (null == SAppCreation)
                // Shared with Intersection for geometry options.
                SAppCreation = commandData.Application.Application.Create;

            var doc = commandData.Application.ActiveUIDocument;
            CombinableElementArray solids
                = new();

            ElementSet es = new();
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

                trans.Commit();
                return Result.Succeeded;
            }

            AutoJoin autojoin = new();
            autojoin.Join(doc.Document);
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
