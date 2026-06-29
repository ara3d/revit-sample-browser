// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Mep;
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
                            if (FabricationPartHelper.IsADuct(part1))
                            {
                                if (SampleBrowserUtils.IsACoupling(part1))
                                    part1.ItemNumber = $"DUCT COUPLING: {m_ductCouplingNum++}";
                                else
                                    part1.ItemNumber = $"DUCT: {m_ductNum++}";
                            }
                            else if (FabricationPartHelper.IsAPipe(part1))
                            {
                                if (SampleBrowserUtils.IsACoupling(part1))
                                    part1.ItemNumber = $"PIPE COUPLING: {m_pipeCouplingNum++}";
                                else
                                    part1.ItemNumber = $"PIPE: {m_pipeNum++}";
                            }
                            else if (part1.IsAHanger())
                            {
                                part1.ItemNumber = $"HANGER: {m_hangerNum++}";
                            }
                            else
                            {
                                part1.ItemNumber = $"MISC: {m_otherNum++}";
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
    }
}
