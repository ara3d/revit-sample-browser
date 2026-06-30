// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Units;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Text;
namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PartInfo : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part to start.");

                if (doc.GetElement(refObj) is not FabricationPart part)
                {
                    message = "The selected element is not a fabrication part.";
                    return Result.Failed;
                }

                var config = FabricationConfiguration.GetFabricationConfiguration(doc);
                if (config == null)
                {
                    message = "no valid fabrication configuration";
                    return Result.Failed;
                }

                StringBuilder builder = new();

                builder.AppendLine($"Alias: {part.Alias}");

                builder.AppendLine($"CID: {part.ItemCustomId}");

                builder.AppendLine($"Domain Type: {part.DomainType}");

                if (part.IsAHanger())
                {
                    var rodKitName = "None";
                    var rodKit = part.HangerRodKit;
                    if (rodKit > 0)
                        rodKitName =
                            $"{config.GetAncillaryGroupName(part.HangerRodKit)}: {config.GetAncillaryName(part.HangerRodKit)}";

                    builder.AppendLine($"Hanger Rod Kit: {rodKitName}");
                }

                var insSpec =
                    $"{config.GetInsulationSpecificationGroup(part.InsulationSpecification)}: {config.GetInsulationSpecificationName(part.InsulationSpecification)}";
                builder.AppendLine($"Insulation Specification: {insSpec}");

                builder.AppendLine($"Has No Connections: {part.HasNoConnections()}");

                builder.AppendLine($"Item Number: {part.ItemNumber}");

                var material = $"{config.GetMaterialGroup(part.Material)}: {config.GetMaterialName(part.Material)}";
                builder.AppendLine($"Material: {material}");

                builder.AppendLine($"Part Guid: {part.PartGuid}");

                builder.AppendLine($"Part Status: {config.GetPartStatusDescription(part.PartStatus)}");

                builder.AppendLine($"Product Code: {part.ProductCode}");

                builder.AppendLine($"Service Name: {part.ServiceName}");

                builder.AppendLine($"Service Type: {config.GetServiceTypeName(part.ServiceType)}");

                var spec =
                    $"{config.GetSpecificationGroup(part.Specification)}: {config.GetSpecificationName(part.Specification)}";
                builder.AppendLine($"Specification: {spec}");

                builder.AppendLine(
                    $"Centerline Length: {ValueFormatting.FormatNumber(doc, part.CenterlineLength, SpecTypeId.Length)}");

                TaskDialog.Show($"Fabrication Part [{part.Id}]", builder.ToString());

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
