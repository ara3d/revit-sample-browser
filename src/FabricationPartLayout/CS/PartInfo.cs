// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

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
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                var refObj = uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part to start.");

                if (!(doc.GetElement(refObj) is FabricationPart part))
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

                var builder = new StringBuilder();

                // alias
                builder.AppendLine($"Alias: {part.Alias}");

                // cid
                builder.AppendLine($"CID: {part.ItemCustomId}");

                // domain type
                builder.AppendLine($"Domain Type: {part.DomainType}");

                // hanger rod kit
                if (part.IsAHanger())
                {
                    var rodKitName = "None";
                    var rodKit = part.HangerRodKit;
                    if (rodKit > 0)
                        rodKitName =
                            $"{config.GetAncillaryGroupName(part.HangerRodKit)}: {config.GetAncillaryName(part.HangerRodKit)}";

                    builder.AppendLine($"Hanger Rod Kit: {rodKitName}");
                }

                // insulation specification
                var insSpec =
                    $"{config.GetInsulationSpecificationGroup(part.InsulationSpecification)}: {config.GetInsulationSpecificationName(part.InsulationSpecification)}";
                builder.AppendLine($"Insulation Specification: {insSpec}");

                // has no connections
                builder.AppendLine($"Has No Connections: {part.HasNoConnections()}");

                // item number
                builder.AppendLine($"Item Number: {part.ItemNumber}");

                // material
                var material = $"{config.GetMaterialGroup(part.Material)}: {config.GetMaterialName(part.Material)}";
                builder.AppendLine($"Material: {material}");

                // part guid
                builder.AppendLine($"Part Guid: {part.PartGuid}");

                // part status
                builder.AppendLine($"Part Status: {config.GetPartStatusDescription(part.PartStatus)}");

                // product code
                builder.AppendLine($"Product Code: {part.ProductCode}");

                // service
                builder.AppendLine($"Service Name: {part.ServiceName}");

                // get the service type name
                builder.AppendLine($"Service Type: {config.GetServiceTypeName(part.ServiceType)}");

                // specification
                var spec =
                    $"{config.GetSpecificationGroup(part.Specification)}: {config.GetSpecificationName(part.Specification)}";
                builder.AppendLine($"Specification: {spec}");

                // centerline length
                builder.AppendLine(
                    $"Centerline Length: {GetStringFromNumber(doc, part.CenterlineLength, SpecTypeId.Length)}");

                TaskDialog.Show($"Fabrication Part [{part.Id}]", builder.ToString());

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private string GetStringFromNumber(Document doc, double number, ForgeTypeId specTypeId)
        {
            return UnitFormatUtils.Format(doc.GetUnits(), specTypeId, number, false);
        }
    }
}
