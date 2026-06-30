// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
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
                var uidoc = commandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                var collection = uidoc.Selection.GetElementIds();

                using (Transaction trans = new(doc, "Part Renumber"))
                {
                    trans.Start();

                    List<FabricationPart> fabParts = new();
                    foreach (var elementId in collection)
                    {
                        if (doc.GetElement(elementId) is FabricationPart part)
                        {
                            part.ItemNumber = string.Empty;
                            fabParts.Add(part);
                        }
                    }

                    if (fabParts.Count == 0)
                    {
                        message = "Select at least one fabrication part";
                        return Result.Failed;
                    }

                    // Notes, order, and service differ between duplicates we still want to match.
                    List<FabricationPartCompareType> ignoreFields = new()
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
                            if (FabricationPartHelper.IsADuct(part1))
                            {
                                part1.ItemNumber = SampleBrowserUtils.IsACoupling(part1) ? $"DUCT COUPLING: {m_ductCouplingNum++}" : $"DUCT: {m_ductNum++}";
                            }
                            else
                            {
                                part1.ItemNumber = FabricationPartHelper.IsAPipe(part1)
                                    ? SampleBrowserUtils.IsACoupling(part1) ? $"PIPE COUPLING: {m_pipeCouplingNum++}" : $"PIPE: {m_pipeNum++}"
                                    : part1.IsAHanger() ? $"HANGER: {m_hangerNum++}" : $"MISC: {m_otherNum++}";
                            }
                        }

                        for (var j = i + 1; j < fabParts.Count; j++)
                        {
                            var part2 = fabParts[j];
                            if (string.IsNullOrWhiteSpace(part2.ItemNumber))
                                if (part1.IsSameAs(part2, ignoreFields))
                                    part2.ItemNumber = part1.ItemNumber;
                        }
                    }

                    trans.Commit();
                }

                TaskDialog td = new("Fabrication Part Renumber")
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
