// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Ara3D.RevitSampleBrowser.CloudAPISample.CS.View;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
namespace Ara3D.RevitSampleBrowser.CloudAPISample.CS.Samples.Migration
{
    /// <summary>
    ///     This is a sample shows how to migrate A360 Teams project to BIM360 Docs
    ///     with Revit Cloud API
    /// </summary>
    public class MigrationToBim360 : SampleContext
    {
        public const string FModelsGuid = "modelsguid.json";

        public const string FLinksInfo = "linkinfo.json";

        public MigrationToBim360()
        {
            Model = new UiMigrationViewModel();
            View = new ViewMigrationToBim360(this);
        }

        public IMigrationModel Model { get; }

        public override void Terminate()
        {
            View = null;
        }

        public IEnumerator Upload(string directory, Guid accountId, Guid projectId,
            ObservableCollection<MigrationRule> modelRules)
        {
            var view = (ViewMigrationToBim360)View;
            if (!Directory.Exists(directory))
            {
                view.UpdateUploadingProgress("Directory doesn't exist.", 0);
                yield break;
            }

            view.UpdateUploadingProgress("Ready to start.", 2);

            var models = Directory.GetFiles(directory, "*.rvt", SearchOption.AllDirectories);
            var count = 0;

            OpenOptions ops = new()
            {
                OpenForeignOption = OpenForeignOption.DoNotOpen,
                DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets
            };

            // Open all documents and analyze if document has links. 
            // If so, save the relationship info into "mapLocalModelPathToLinksName"
            Dictionary<string, List<string>> mapLocalModelPathToLinksName = [];
            foreach (var model in models)
            {
                var name = Path.GetFileName(model);
                var progress = count++ * 100 / models.Length;
                var modelpath = ModelPathUtils.ConvertUserVisiblePathToModelPath(model);
                view.UpdateUploadingProgress($"Analyzing {name}", progress);
                yield return null;

                try
                {
                    var doc = Application.Application.OpenDocumentFile(modelpath, ops);
                    List<string> links = [];
                    foreach (var linkInstance in doc.GetElements<RevitLinkType>())
                    {
                        links.Add(linkInstance.Name);
                    }

                    if (links.Count > 0) mapLocalModelPathToLinksName[model] = links;
                    doc.Close(false);
                }
                catch (Exception e)
                {
                    var msg = $"Failed to analyze {name} - {e.Message}";
                    view.UpdateUploadingProgress(msg, progress);
                    MessageBox.Show(msg);
                    yield break;
                }

                yield return null;
            }

            // All link info should be dump to local file in case something wrong happens during uploading process
            var jsonString = JsonSerializer.Serialize(mapLocalModelPathToLinksName);

            File.WriteAllText(FLinksInfo, jsonString);

            view.UpdateUploadingProgress("Analyzing finished", 100);

            // Begin Uploading
            count = 0;
            Dictionary<string, string> mapModelsNameToGuid = [];
            foreach (var model in models)
            {
                var name = Path.GetFileName(model);
                var progress = count++ * 100 / models.Length;
                var modelpath = ModelPathUtils.ConvertUserVisiblePathToModelPath(model);
                view.UpdateUploadingProgress($"Uploading {name}", progress);
                yield return null;

                try
                {
                    var doc = Application.Application.OpenDocumentFile(modelpath, ops);
                    var folderUrn = CloudApiHelper.GetTargetFolderUrn(modelRules, directory, model, Model.AvailableFolders)?.Urn;
                    doc.SaveAsCloudModel(accountId, projectId, folderUrn, $"Migrated_{Path.GetFileName(model)}");
                    var modelPath = doc.GetCloudModelPath();
                    mapModelsNameToGuid.Add(name, $"{modelPath.GetProjectGUID()},{modelPath.GetModelGUID()}");
                    doc.Close(false);
                }
                catch (Exception e)
                {
                    var msg = $"Failed to upload {name} - {e.Message}";
                    view.UpdateUploadingProgress(msg, progress);
                    MessageBox.Show(msg);
                    yield break;
                }

                yield return null;
            }

            jsonString = JsonSerializer.Serialize(mapModelsNameToGuid);
            File.WriteAllText(FModelsGuid, jsonString);

            view.UpdateUploadingProgress("Uploading finished", 100);
        }

        public IEnumerator ReloadLinks(string directory, Guid projectId)
        {
            var view = (ViewMigrationToBim360)View;
            if (!Directory.Exists(directory))
            {
                view.UpdateReloadingProgress("Directory doesn't exist.", 0);
                yield break;
            }

            view.UpdateReloadingProgress("Ready to start.", 2);

            OpenOptions ops = new();
            TransactWithCentralOptions transOp = new();
            SynchronizeWithCentralOptions swcOptions = new();
            var count = 0;

            // Read mapping info
            var jsonString = File.ReadAllText(FLinksInfo);

            var mapLocalModelPathToLinksName = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString);
            jsonString = File.ReadAllText(FModelsGuid);
            var mapModelsNameToGuid = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            // Try to open each cloud model and reload if they have links, making that point to cloud path.
            foreach (var kvp in mapLocalModelPathToLinksName)
            {
                var localPath = kvp.Key;
                var name = Path.GetFileName(localPath);
                var guids = mapModelsNameToGuid[name].Split(',');
                var hostModelPath =
                    ModelPathUtils.ConvertCloudGUIDsToCloudPath("US", Guid.Parse(guids[0]), Guid.Parse(guids[1]));

                var progress = count++ * 100 / mapLocalModelPathToLinksName.Count();
                view.UpdateReloadingProgress($"Reloading {name}", progress);
                yield return null;

                try
                {
                    var doc = Application.Application.OpenDocumentFile(hostModelPath, ops,
                        new DefaultOpenFromCloudCallback());
                    foreach (var linkInstance in doc.GetElements<RevitLinkType>())
                    {
                        if (mapModelsNameToGuid.TryGetValue(linkInstance.Name, out _))
                        {
                            guids = mapModelsNameToGuid[linkInstance.Name].Split(',');
                            var linkModelPath =
                                ModelPathUtils.ConvertCloudGUIDsToCloudPath("US", Guid.Parse(guids[0]),
                                    Guid.Parse(guids[1]));
                            linkInstance.LoadFrom(linkModelPath, new WorksetConfiguration());
                        }
                    }

                    doc.SynchronizeWithCentral(transOp, swcOptions);
                    doc.Close(false);
                }
                catch (Exception e)
                {
                    var msg = $"Failed to reload {name} - {e.Message}";
                    view.UpdateUploadingProgress(msg, progress);
                    MessageBox.Show(msg);
                    yield break;
                }
            }

            view.UpdateReloadingProgress("Reloading finished", 100);
        }
    }
}
