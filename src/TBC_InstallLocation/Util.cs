using Autodesk.Revit.ApplicationServices;
using Microsoft.Win32;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_InstallLocation sample.</summary>
    internal static partial class Util
    {
        private const string RegPathUninstall
            = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string RegPathForFlavourTemplate
            = @"SOFTWARE\Autodesk\Revit\Autodesk {0} {1}";

        internal static string RegPathForFlavour(
            ProductType flavour,
            string version)
        {
            return string.Format(RegPathForFlavourTemplate,
                flavour, version);
        }
        internal static string GetSubkeyValue(
            string reg_path_key,
            string subkey_name,
            string value_name)
        {
            using var key = Registry.LocalMachine.OpenSubKey(reg_path_key);
            using var subkey = key.OpenSubKey(subkey_name);
            return subkey.GetValue(value_name) as string;
        }

        internal static string GetRevitProductCode(string reg_path_product)
        {
            return GetSubkeyValue(reg_path_product,
                "Components", "ProductCode");
        }

        internal static string GetRevitInstallLocation(string product_code)
        {
            return GetSubkeyValue(RegPathUninstall,
                product_code, "InstallLocation");
        }

        internal static string FormatInstallLocationData(
            string description,
            string version_name,
            string product_code,
            string install_location)
        {
            return $"{description}: {version_name}\nProduct code: {product_code}\nInstall location: {install_location}";
        }
    }
}
