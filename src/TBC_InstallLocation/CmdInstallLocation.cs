#region Header

//
// CmdInstallLocation.cs - determine Revit product install location
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdInstallLocation : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application.Application;

            var reg_path_product
                = Util.RegPathForFlavour(
                    app.Product, app.VersionNumber);

            var product_code
                = Util.GetRevitProductCode(reg_path_product);

            var install_location
                = Util.GetRevitInstallLocation(product_code);

            var msg = Util.FormatInstallLocationData(
                "Running application",
                app.VersionName,
                product_code,
                install_location);

            foreach (ProductType p in
                Enum.GetValues(typeof(ProductType)))
                try
                {
                    reg_path_product = Util.RegPathForFlavour(
                        p, app.VersionNumber);

                    product_code = Util.GetRevitProductCode(
                        reg_path_product);

                    install_location = Util.GetRevitInstallLocation(
                        product_code);

                    msg += Util.FormatInstallLocationData(
                        "\n\nInstalled product",
                        p.ToString(),
                        product_code,
                        install_location);
                }
                catch (Exception)
                {
                }

            Util.InfoMsg(msg);

            return Result.Failed;
        }
    }
}
