// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.CapitalizeAllTextNotes.CS
{
    /// <summary>
    ///     Find all the TextNote instances in the document and change their formatting to 'AllCaps'
    /// </summary>
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

                // Iterate through the document and find all the TextNote elements
                var collector = new FilteredElementCollector(document);
                collector.OfClass(typeof(TextNote));
                if (collector.GetElementCount() == 0)
                {
                    message = "The document does not contain TextNote elements";
                    return Result.Failed;
                }

                // Record all TextNotes that are not yet formatted to be 'AllCaps'
                var textNotesToUpdate = new ElementSet();
                foreach (var element in collector)
                {
                    var textNote = (TextNote)element;

                    // Extract the FormattedText from the TextNote
                    var formattedText = textNote.GetFormattedText();

                    // If 'GetAllCapsStatus()' returns 'FormatStatus.All' then all the characters in
                    // the text have the 'AllCaps' status.
                    // If there are no characters that have the 'AllCaps' status then 'GetAllCapsStatus()'
                    // will return 'FormatStatus.None'.  And if only some of the characters have 
                    // the 'AllCaps' status then 'GetAllCapsStatus()' returns 'FormatStatus.Mixed'
                    //
                    // Note that it is also possible to test whether all characters are 
                    // bold, italic, underlined, or have superscript or subscript formatting.
                    // See 'GetBoldStatus', 'GetItalicStatus', 'GetUnderlineStatus', 
                    // 'GetSuperscriptStatus', and 'GetSubscriptStatus' respectively.
                    //
                    // Note that it is also possible to only test a subset of characters in the FormattedText
                    // This is done by calling these methods with a 'TextRange' that specifies 
                    // the range of characters to be tested.

                    if (formattedText.GetAllCapsStatus() != FormatStatus.All) textNotesToUpdate.Insert(textNote);
                }

                // Check whether we found any TextNotes that need to be formatted
                if (textNotesToUpdate.IsEmpty)
                {
                    message = "No TextNote elements needed updating";
                    return Result.Failed;
                }

                // Apply the 'AllCaps' formatting to the TextNotes that still need it.
                using (var transaction = new Transaction(document, "Capitalize All TextNotes"))
                {
                    transaction.Start();

                    foreach (TextNote textNote in textNotesToUpdate)
                    {
                        var formattedText = textNote.GetFormattedText();

                        // Apply the 'AllCaps' status to all characters.
                        // Note that there are also methods to apply bold, italic, underline, 
                        // superscript and subscript formatting.  
                        // And that the formatting can be applied both to the entire text
                        // (as is done here), or to a subset by calling these methods with a 'TextRange'

                        formattedText.SetAllCapsStatus(true);

                        // After making the changes to the Formatted text
                        // it is necessary to apply them to the TextNote element
                        // (or elements) that should get these changes.
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
