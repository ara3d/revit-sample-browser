//
// (C) Copyright 2003-2019 by Autodesk, Inc.
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
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
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
            var part = doc.GetElement(refObj) as FabricationPart;

            if (part == null)
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
            builder.AppendLine("Alias: " + part.Alias);

            // cid
            builder.AppendLine("CID: " + part.ItemCustomId.ToString());

            // domain type
            builder.AppendLine("Domain Type: " + part.DomainType.ToString());

            // hanger rod kit
            if (part.IsAHanger())
            {
               var rodKitName = "None";
               var rodKit = part.HangerRodKit;
               if (rodKit > 0)
                  rodKitName = config.GetAncillaryGroupName(part.HangerRodKit) + ": " + config.GetAncillaryName(part.HangerRodKit);

               builder.AppendLine("Hanger Rod Kit: " + rodKitName);
            }

            // insulation specification
            var insSpec = config.GetInsulationSpecificationGroup(part.InsulationSpecification)
                + ": " + config.GetInsulationSpecificationName(part.InsulationSpecification);
            builder.AppendLine("Insulation Specification: " + insSpec);

            // has no connections
            builder.AppendLine("Has No Connections: " + part.HasNoConnections().ToString());

            // item number
            builder.AppendLine("Item Number: " + part.ItemNumber);

            // material
            var material = config.GetMaterialGroup(part.Material) + ": " + config.GetMaterialName(part.Material);
            builder.AppendLine("Material: " + material);

            // part guid
            builder.AppendLine("Part Guid: " + part.PartGuid.ToString());

            // part status
            builder.AppendLine("Part Status: " + config.GetPartStatusDescription(part.PartStatus));

            // product code
            builder.AppendLine("Product Code: " + part.ProductCode);

            // service
            builder.AppendLine("Service Name: " + part.ServiceName);

            // get the service type name
            builder.AppendLine("Service Type: " + config.GetServiceTypeName(part.ServiceType));

            // specification
            var spec = config.GetSpecificationGroup(part.Specification) + ": " + config.GetSpecificationName(part.Specification);
            builder.AppendLine("Specification: " + spec);

            // centerline length
            builder.AppendLine("Centerline Length: " + GetStringFromNumber(doc, part.CenterlineLength, SpecTypeId.Length));

            TaskDialog.Show("Fabrication Part [" + part.Id.ToString() + "]", builder.ToString());

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
