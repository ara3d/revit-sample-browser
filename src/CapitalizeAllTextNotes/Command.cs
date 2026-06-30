// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CapitalizeAllTextNotes.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                var textNotes = document.GetElements<TextNote>().ToList();
                if (textNotes.Count == 0)
                {
                    message = "The document does not contain TextNote elements";
                    return Result.Failed;
                }

                var textNotesToUpdate = new ElementSet();
                foreach (var element in textNotes)
                {
                    var textNote = (TextNote)element;

                    var formattedText = textNote.GetFormattedText();

                    // FormatStatus.All/None/Mixed from GetAllCapsStatus(); mutate via SetFormattedText after SetAllCapsStatus.

                    if (formattedText.GetAllCapsStatus() != FormatStatus.All) 
                        textNotesToUpdate.Insert(textNote);
                }

                if (textNotesToUpdate.IsEmpty)
                {
                    message = "No TextNote elements needed updating";
                    return Result.Failed;
                }

                using (var transaction = new Transaction(document, "Capitalize All TextNotes"))
                {
                    transaction.Start();

                    foreach (TextNote textNote in textNotesToUpdate)
                    {
                        var formattedText = textNote.GetFormattedText();
                        formattedText.SetAllCapsStatus(true);
                        textNote.SetFormattedText(formattedText);
                    }

                    transaction.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
