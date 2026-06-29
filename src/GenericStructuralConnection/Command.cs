// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS
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
