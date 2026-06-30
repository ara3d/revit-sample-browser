// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.RevitAddIns;

namespace Ara3D.RevitSampleBrowser.ExternalCommand.CS.RevitAddInUtilitySample
{
    public partial class RevitAddInUtilitySampleForm : Form
    {
        public RevitAddInUtilitySampleForm()
        {
            InitializeComponent();
            AddInsInfoButton.Enabled = false;
            OpenAddInFileButton.Enabled = false;
        }

        private void CreateAddInManifestButton_Click(object sender, EventArgs e)
        {
            var manifest = new RevitAddInManifest();

            var fileInfo = new FileInfo("..\\ExternalCommandRegistration\\ExternalCommandRegistration.dll");

            var application1 = new RevitAddInApplication(
                "ExternalApplication", fileInfo.FullName, Guid.NewGuid(),
                "Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS.ExternalApplicationClass", "adsk");

            // Not visible without an active document; disabled when a wall is selected.
            var command1 = new RevitAddInCommand(
                fileInfo.FullName, Guid.NewGuid(),
                "Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS.ExternalCommandCreateWall", "adsk")
 {
     Description = "A simple external command which is used to create a wall.",
     Text = "@createWallText",
     AvailabilityClassName = "Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS.WallSelection",
     LanguageType = LanguageType.English_USA,
     LargeImage = "@CreateWall",
     TooltipImage = "@CreateWallTooltip",
     VisibilityMode = VisibilityMode.NotVisibleWhenNoActiveDocument,
     LongDescription = "This command will not be visible in Revit Structure or there is no active document."
 };
            command1.LongDescription += " And this command will be disabled if user selected a wall. ";

            // Not visible in family documents or without an active document; disabled outside 3D views.
            var command2 = new RevitAddInCommand(
                fileInfo.FullName, Guid.NewGuid(),
                "Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS.ExternalCommand3DView", "adsk")
            {
                Description = "A simple external command which show a message box.",
                Text = "@view3DText",
                AvailabilityClassName = "Ara3D.RevitSampleBrowser.ExternalCommandRegistration.CS.View3D",
                LargeImage = "@View3D",
                LanguageType = LanguageType.English_USA,
                VisibilityMode = VisibilityMode.NotVisibleInFamily | VisibilityMode.NotVisibleWhenNoActiveDocument,
                LongDescription = "This command will not be visible in Revit MEP, Family Document or no active document."
            };
            command2.LongDescription += " And this command will be disabled if the active view is not a 3D view. ";

            manifest.AddInApplications.Add(application1);
            manifest.AddInCommands.Add(command1);
            manifest.AddInCommands.Add(command2);

            fileInfo = new FileInfo("ExteranlCommand.Sample.addin");
            manifest.SaveAs(fileInfo.FullName);
            AddInsInfoButton_Click(null, null);
            AddInsInfoButton.Enabled = true;
            OpenAddInFileButton.Enabled = true;
        }

        private void AddInsInfoButton_Click(object sender, EventArgs e)
        {
            var fileInfo = new FileInfo("ExteranlCommand.Sample.addin");
            var revitAddInManifest =
                AddInManifestUtility.GetRevitAddInManifest(fileInfo.FullName);

            treeView1.Nodes.Clear();
            if (revitAddInManifest.AddInApplications.Count >= 1)
            {
                var apps = treeView1.Nodes.Add("External Applications");
                foreach (var app in revitAddInManifest.AddInApplications)
                {
                    var appNode = apps.Nodes.Add(app.Name);
                    appNode.Nodes.Add($"Name: {app.Name}");
                    appNode.Nodes.Add($"Assembly: {app.Assembly}");
                    appNode.Nodes.Add($"AddInId: {app.AddInId}");
                    appNode.Nodes.Add($"Full Class Name: {app.FullClassName}");
                }
            }

            if (revitAddInManifest.AddInCommands.Count >= 1)
            {
                var cmds = treeView1.Nodes.Add("External Commands");
                foreach (var cmd in revitAddInManifest.AddInCommands)
                {
                    var cmdNode = cmds.Nodes.Add(cmd.Text);
                    cmdNode.Nodes.Add($"Assembly: {cmd.Assembly}");
                    cmdNode.Nodes.Add($"AddInId: {cmd.AddInId}");
                    cmdNode.Nodes.Add($"Full Class Name: {cmd.FullClassName}");
                    cmdNode.Nodes.Add($"Text: {cmd.Text}");
                    cmdNode.Nodes.Add($"Description: {cmd.Description}");
                    cmdNode.Nodes.Add($"LanguageType: {cmd.LanguageType}");
                    cmdNode.Nodes.Add($"LargeImage: {cmd.LargeImage}");
                    cmdNode.Nodes.Add($"LongDescription: {cmd.LongDescription}");
                    cmdNode.Nodes.Add($"TooltipImage: {cmd.TooltipImage}");
                    cmdNode.Nodes.Add($"VisibilityMode: {cmd.VisibilityMode}");
                    cmdNode.Nodes.Add($"AvailabilityClassName: {cmd.AvailabilityClassName}");
                }
            }
        }

        private void RevitProductsButton_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            var allProductsNode = treeView1.Nodes.Add("Installed Revit Products: ");
            foreach (var revitProduct in RevitProductUtility.GetAllInstalledRevitProducts())
            {
                var productNode = allProductsNode.Nodes.Add(revitProduct.Name);
                productNode.Nodes.Add($"Product Name: {revitProduct.Name}");
                productNode.Nodes.Add($"AllUsersAddInFolder: {revitProduct.AllUsersAddInFolder}");
                productNode.Nodes.Add($"Architecture: {revitProduct.Architecture}");
                productNode.Nodes.Add($"Current User AddIn Folder: {revitProduct.CurrentUserAddInFolder}");
                productNode.Nodes.Add($"Install Location: {revitProduct.InstallLocation}");
                productNode.Nodes.Add($"ProductCode: {revitProduct.ProductCode}");
                productNode.Nodes.Add($"Version: {revitProduct.Version}");
            }
        }

        private void OpenAddInFileButton_Click(object sender, EventArgs e)
        {
            var fileInfo = new FileInfo("ExteranlCommand.Sample.addin");
            System.Diagnostics.Process.Start(fileInfo.FullName);
        }
    }
}
