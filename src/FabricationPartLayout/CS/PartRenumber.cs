// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PartRenumber : IExternalCommand
    {
        private int m_ductCouplingNum = 1;
        private int m_ductNum = 1;
        private int m_hangerNum = 1;
        private int m_otherNum = 1;
        private int m_pipeCouplingNum = 1;
        private int m_pipeNum = 1;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                var collection = uidoc.Selection.GetElementIds();

                using (var trans = new Transaction(doc, "Part Renumber"))
                {
                    trans.Start();

                    var fabParts = new List<FabricationPart>();
                    foreach (var elementId in collection)
                    {
                        if (doc.GetElement(elementId) is FabricationPart part)
                        {
                            part.ItemNumber = string.Empty; // wipe the item number
                            fabParts.Add(part);
                        }
                    }

                    if (fabParts.Count == 0)
                    {
                        message = "Select at least one fabrication part";
                        return Result.Failed;
                    }

                    // ignore certain fields
                    var ignoreFields = new List<FabricationPartCompareType>
                    {
                        FabricationPartCompareType.Notes,
                        FabricationPartCompareType.OrderNo,
                        FabricationPartCompareType.Service
                    };

                    for (var i = 0; i < fabParts.Count; i++)
                    {
                        var part1 = fabParts[i];
                        if (string.IsNullOrWhiteSpace(part1.ItemNumber))
                        {
                            // part has not already been checked
                            if (IsADuct(part1))
                            {
                                if (IsACoupling(part1))
                                    part1.ItemNumber = "DUCT COUPLING: " + m_ductCouplingNum++;
                                else
                                    part1.ItemNumber = "DUCT: " + m_ductNum++;
                            }
                            else if (IsAPipe(part1))
                            {
                                if (IsACoupling(part1))
                                    part1.ItemNumber = "PIPE COUPLING: " + m_pipeCouplingNum++;
                                else
                                    part1.ItemNumber = "PIPE: " + m_pipeNum++;
                            }
                            else if (part1.IsAHanger())
                            {
                                part1.ItemNumber = "HANGER: " + m_hangerNum++;
                            }
                            else
                            {
                                part1.ItemNumber = "MISC: " + m_otherNum++;
                            }
                        }

                        for (var j = i + 1; j < fabParts.Count; j++)
                        {
                            var part2 = fabParts[j];
                            if (string.IsNullOrWhiteSpace(part2.ItemNumber))
                                // part2 has not been checked
                                if (part1.IsSameAs(part2, ignoreFields))
                                    // items are the same, so give them the same item number
                                    part2.ItemNumber = part1.ItemNumber;
                        }
                    }

                    trans.Commit();
                }

                var td = new TaskDialog("Fabrication Part Renumber")
                {
                    MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                    TitleAutoPrefix = false,
                    MainInstruction = "Renumber was successful",
                    AllowCancellation = false,
                    CommonButtons = TaskDialogCommonButtons.Ok
                };

                td.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Checks if the given part is fabrication ductwork.
        /// </summary>
        /// <param name="fabPart">The part to check.</param>
        /// <returns>True if the part is fabrication ductwork.</returns>
        private bool IsADuct(FabricationPart fabPart)
        {
            return fabPart != null && fabPart.Category.BuiltInCategory == BuiltInCategory.OST_FabricationDuctwork;
        }

        /// <summary>
        ///     Checks if the part is fabrication pipework.
        /// </summary>
        /// <param name="fabPart">The part to check.</param>
        /// <returns>True if the part is fabrication pipework.</returns>
        private bool IsAPipe(FabricationPart fabPart)
        {
            return fabPart != null && fabPart.Category.BuiltInCategory == BuiltInCategory.OST_FabricationPipework;
        }

        /// <summary>
        ///     Checks if the part is a coupling.
        ///     The CID's (the fabrication part item customer Id) that are recognized internally as couplings are:
        ///     CID 522, 1112 - Round Ductwork
        ///     CID 1522 - Oval Ductwork
        ///     CID 4522 - Rectangular Ductwork
        ///     CID 2522 - Pipe Work
        ///     CID 3522 - Electrical
        /// </summary>
        /// <param name="fabPart">The part to check.</param>
        /// <returns>True if the part is a coupling.</returns>
        private bool IsACoupling(FabricationPart fabPart)
        {
            if (fabPart != null)
            {
                var cid = fabPart.ItemCustomId;
                if (cid == 522 || cid == 1522 || cid == 2522 || cid == 3522 || cid == 1112) return true;
            }

            return false;
        }
    }
}
