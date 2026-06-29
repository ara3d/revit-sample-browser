// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LoadAndPlaceNextItemFile : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                var cl = new FilteredElementCollector(doc);
                cl.OfClass(typeof(Level));
                var levels = cl.ToElements();

                Level levelOne = null;
                foreach (Level level in levels)
                {
                    if (level != null && level.Name.Equals("Level 1"))
                    {
                        levelOne = level;
                        break;
                    }
                }

                if (levelOne == null)
                    return Result.Failed;

                using (var config = FabricationConfiguration.GetFabricationConfiguration(doc))
                {
                    if (config == null)
                    {
                        message = "No fabrication configuration in use";
                        return Result.Failed;
                    }

                    using (var configInfo = config.GetFabricationConfigurationInfo())
                    {
                        using (var source = FabricationConfigurationInfo.FindSourceFabricationConfiguration(configInfo))
                        {
                            if (source == null)
                            {
                                message = "Source fabrication configuration not found";
                                return Result.Failed;
                            }

                            using (var trans = new Transaction(doc, "Load And Place Next Item File"))
                            {
                                trans.Start();

                                // reload the configuration
                                config.ReloadConfiguration();

                                // get the item folders
                                var itemFolders = config.GetItemFolders();

                                // get the next unloaded item file from the item folders structure
                                var nextFile = GetNextUnloadedItemFile(itemFolders);
                                if (nextFile == null)
                                {
                                    message = "Could not locate the next unloaded item file";
                                    return Result.Failed;
                                }

                                var itemFilesToLoad = new List<FabricationItemFile> { nextFile };

                                // load the item file into the config
                                var failedItems = config.LoadItemFiles(itemFilesToLoad);
                                if (failedItems != null && failedItems.Count > 0)
                                {
                                    message = $"Could not load the item file: {nextFile.Identifier}";
                                    return Result.Failed;
                                }

                                // create a part from the item file
                                using (var part = FabricationPart.Create(doc, nextFile, levelOne.Id))
                                {
                                    doc.Regenerate();

                                    var selectedElements = new List<ElementId> { part.Id };

                                    uiDoc.Selection.SetElementIds(selectedElements);
                                    uiDoc.ShowElements(selectedElements);

                                    trans.Commit();
                                }
                            }
                        }
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private FabricationItemFile GetNextUnloadedItemFile(IList<FabricationItemFolder> itemFolders)
        {
            if (itemFolders == null)
                return null;

            foreach (var folder in itemFolders)
            {
                var file = GetNextUnloadedItemFileRecursive(folder);
                if (file != null)
                    return file;
            }

            return null;
        }

        private FabricationItemFile GetNextUnloadedItemFileRecursive(FabricationItemFolder folder)
        {
            if (folder == null)
                return null;

            var files = folder.GetItemFiles();
            if (files != null && files.Count > 0)
                foreach (var file in files)
                {
                    if (file != null && file.IsLoaded() == false && file.IsValid())
                        return file;
                }

            var subFolders = folder.GetSubFolders();
            if (subFolders != null && subFolders.Count > 0)
                foreach (var subFolder in subFolders)
                {
                    var file = GetNextUnloadedItemFileRecursive(subFolder);
                    if (file != null)
                        return file;
                }

            return null;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UnloadUnusedItemFiles : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                using (var config = FabricationConfiguration.GetFabricationConfiguration(doc))
                {
                    if (config == null)
                    {
                        message = "No fabrication configuration in use";
                        return Result.Failed;
                    }

                    using (var trans = new Transaction(doc, "Unload unused item files"))
                    {
                        trans.Start();

                        config.ReloadConfiguration();

                        var loadedFiles = config.GetAllLoadedItemFiles();
                        var unusedFiles = loadedFiles.Where(x => x.IsUsed == false).ToList();
                        if (unusedFiles.Count == 0)
                        {
                            message = "No unuseed item files found";
                            return Result.Failed;
                        }

                        if (config.CanUnloadItemFiles(unusedFiles) == false)
                        {
                            message = "Cannot unload item files";
                            return Result.Failed;
                        }

                        config.UnloadItemFiles(unusedFiles);

                        trans.Commit();

                        var builder = new StringBuilder();
                        unusedFiles.ForEach(x => builder.AppendLine(Path.GetFileNameWithoutExtension(x.Identifier)));

                        var td = new TaskDialog("Unload Unused Item Files")
                        {
                            MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                            TitleAutoPrefix = false,
                            MainInstruction = "The following item files were unloaded:",
                            MainContent = builder.ToString()
                        };

                        td.Show();
                    }
                }

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
