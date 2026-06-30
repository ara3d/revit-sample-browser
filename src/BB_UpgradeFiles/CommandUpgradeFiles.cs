using Ara3D.Logging;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Text;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandUpgradeFiles : NamedCommand
{
    public override string Name => "Upgrade Files";

    public override void Execute(object arg)
    {
        var app = arg as UIApplication;
        OpenFolderDialog dlg = new()
        {
            Multiselect = false
        };
        if (dlg.ShowDialog() != true)
            return;

        DirectoryPath folder = new(dlg.FolderName);
        var outputFolder = folder.RelativeFolder("upgrade");
        StringBuilder sb = new();
        var logger = Logger.Create(sb);

        foreach (var f in folder.GetFiles("*.rvt"))
        {
            try
            {
                var doc = app.OpenNoDialog(f);

                try
                {
                    var outputFile = f.ChangeDirectory(outputFolder);
                    logger.Log($"Saving file to {outputFile.GetFileName()}");
                    doc.SaveCopy(outputFile);
                }
                finally
                {
                    doc.Close(false);
                }

            }
            catch (Exception e)
            {
                logger.Log($"Saving file to {e.Message}");
            }
        }
    }
}