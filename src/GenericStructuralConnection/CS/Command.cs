//
// (C) Copyright 2003-2015 by Autodesk, Inc.
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


using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GenericStructuralConnection.CS
{
    /// <summary>
    ///     Demonstrate how to create a generic structural connection.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var ret = Result.Succeeded;
            try
            {
                // Get the document from external command data.
                var activeDoc = commandData.Application.ActiveUIDocument;

                using (var frm = new StructuralConnectionForm())
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var opt = frm.UserOption;
                        switch (opt)
                        {
                            case CommandOption.CreateGeneric:
                            {
                                ret = GenericStructuralConnectionOps.CreateGenericStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.DeleteGeneric:
                            {
                                ret = GenericStructuralConnectionOps.DeleteGenericStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.ReadGeneric:
                            {
                                ret = GenericStructuralConnectionOps.ReadGenericStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.UpdateGeneric:
                            {
                                ret = GenericStructuralConnectionOps.UpdateGenericStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.CreateDetailed:
                            {
                                ret = DetailedStructuralConnectionOps.CreateDetailedStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.ChangeDetailed:
                            {
                                ret = DetailedStructuralConnectionOps.ChangeDetailedStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.CopyDetailed:
                            {
                                ret = DetailedStructuralConnectionOps.CopyDetailedStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                            case CommandOption.MatchPropDetailed:
                            {
                                ret = DetailedStructuralConnectionOps.MatchPropertiesDetailedStructuralConnection(
                                    activeDoc, ref message);
                                break;
                            }
                            case CommandOption.ResetDetailed:
                            {
                                ret = DetailedStructuralConnectionOps.ResetDetailedStructuralConnection(activeDoc,
                                    ref message);
                                break;
                            }
                        }
                    }
                    else
                    {
                        ret = Result.Cancelled;
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                ret = Result.Failed;
            }

            return ret;
        }
    }
}