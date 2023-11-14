// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ErrorHandling.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand and IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand, IExternalApplication
    {
        /// <summary>
        ///     The failure definition id for warning
        /// </summary>
        public static FailureDefinitionId IdWarning;

        /// <summary>
        ///     The failure definition id for error
        /// </summary>
        public static FailureDefinitionId IdError;

        /// <summary>
        ///     The active document
        /// </summary>
        private Document m_doc;

        /// <summary>
        ///     The failure definition for error
        /// </summary>
        private FailureDefinition m_fdError;

        /// <summary>
        ///     The failure definition for warning
        /// </summary>
        private FailureDefinition m_fdWarning;

        /// <summary>
        ///     The Revit application
        /// </summary>
        private Application m_revitApp;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Create failure definition Ids
                var guid1 = new Guid("0C3F66B5-3E26-4d24-A228-7A8358C76D39");
                var guid2 = new Guid("93382A45-89A9-4cfe-8B94-E0B0D9542D34");
                new Guid("A16D08E2-7D06-4bca-96B0-C4E4CC0512F8");
                IdWarning = new FailureDefinitionId(guid1);
                IdError = new FailureDefinitionId(guid2);

                // Create failure definitions and add resolutions
                m_fdWarning =
                    FailureDefinition.CreateFailureDefinition(IdWarning, FailureSeverity.Warning,
                        "I am the warning.");
                m_fdError = FailureDefinition.CreateFailureDefinition(IdError, FailureSeverity.Error,
                    "I am the error");

                m_fdWarning.AddResolutionType(FailureResolutionType.MoveElements, "MoveElements",
                    typeof(DeleteElements));
                m_fdWarning.AddResolutionType(FailureResolutionType.DeleteElements, "DeleteElements",
                    typeof(DeleteElements));
                m_fdWarning.SetDefaultResolutionType(FailureResolutionType.DeleteElements);

                m_fdError.AddResolutionType(FailureResolutionType.DetachElements, "DetachElements",
                    typeof(DeleteElements));
                m_fdError.AddResolutionType(FailureResolutionType.DeleteElements, "DeleteElements",
                    typeof(DeleteElements));
                m_fdError.SetDefaultResolutionType(FailureResolutionType.DeleteElements);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            m_revitApp = commandData.Application.Application;
            m_doc = commandData.Application.ActiveUIDocument.Document;

            var level1 = GetLevel();
            if (level1 == null) throw new Exception("[ERROR] Failed to get level 1");

            try
            {
                //
                // Post a warning and resolve it in FailurePreproccessor
                try
                {
                    var transaction = new Transaction(m_doc, "Warning_FailurePreproccessor");
                    var options = transaction.GetFailureHandlingOptions();
                    var preproccessor = new FailurePreproccessor();
                    options.SetFailuresPreprocessor(preproccessor);
                    transaction.SetFailureHandlingOptions(options);
                    transaction.Start();
                    var fm = new FailureMessage(IdWarning);
                    m_doc.PostFailure(fm);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    message = "Failed to commit transaction Warning_FailurePreproccessor";
                    return Result.Failed;
                }

                //
                // Dismiss the overlapped wall warning in FailurePreproccessor
                try
                {
                    var transaction = new Transaction(m_doc, "Warning_FailurePreproccessor_OverlappedWall");
                    var options = transaction.GetFailureHandlingOptions();
                    var preproccessor = new FailurePreproccessor();
                    options.SetFailuresPreprocessor(preproccessor);
                    transaction.SetFailureHandlingOptions(options);
                    transaction.Start();

                    var line = Line.CreateBound(new XYZ(-10, 0, 0), new XYZ(-20, 0, 0));
                    Wall.Create(m_doc, line, level1.Id, false);
                    Wall.Create(m_doc, line, level1.Id, false);
                    m_doc.Regenerate();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    message = "Failed to commit transaction Warning_FailurePreproccessor_OverlappedWall";
                    return Result.Failed;
                }

                //
                // Post an error and resolve it in FailuresProcessingEvent
                try
                {
                    m_revitApp.FailuresProcessing += FailuresProcessing;
                    var transaction = new Transaction(m_doc, "Error_FailuresProcessingEvent");
                    transaction.Start();

                    var line = Line.CreateBound(new XYZ(0, 10, 0), new XYZ(20, 10, 0));
                    var wall = Wall.Create(m_doc, line, level1.Id, false);
                    m_doc.Regenerate();

                    var fm = new FailureMessage(IdError);
                    var fr = DeleteElements.Create(m_doc, wall.Id);
                    fm.AddResolution(FailureResolutionType.DeleteElements, fr);
                    m_doc.PostFailure(fm);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    message = "Failed to commit transaction Error_FailuresProcessingEvent";
                    return Result.Failed;
                }

                //
                // Post an error and resolve it in FailuresProcessor
                try
                {
                    var processor = new FailuresProcessor();
                    Application.RegisterFailuresProcessor(processor);
                    var transaction = new Transaction(m_doc, "Error_FailuresProcessor");
                    transaction.Start();

                    var line = Line.CreateBound(new XYZ(0, 20, 0), new XYZ(20, 20, 0));
                    var wall = Wall.Create(m_doc, line, level1.Id, false);
                    m_doc.Regenerate();

                    var fm = new FailureMessage(IdError);
                    var fr = DeleteElements.Create(m_doc, wall.Id);
                    fm.AddResolution(FailureResolutionType.DeleteElements, fr);
                    m_doc.PostFailure(fm);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    message = "Failed to commit transaction Error_FailuresProcessor";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            var failuresAccessor = e.GetFailuresAccessor();
            //failuresAccessor
            var transactionName = failuresAccessor.GetTransactionName();

            var fmas = failuresAccessor.GetFailureMessages();
            if (fmas.Count == 0)
            {
                e.SetProcessingResult(FailureProcessingResult.Continue);
                return;
            }

            if (transactionName.Equals("Error_FailuresProcessingEvent"))
            {
                foreach (var fma in fmas)
                {
                    var id = fma.GetFailureDefinitionId();
                    if (id == IdError) failuresAccessor.ResolveFailure(fma);
                }

                e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
                return;
            }

            e.SetProcessingResult(FailureProcessingResult.Continue);
        }

        /// <summary>
        ///     Gets the level named "Level 1"
        /// </summary>
        /// <returns></returns>
        private Level GetLevel()
        {
            Level level1 = null;

            var collector = new FilteredElementCollector(m_doc);
            var filter = new ElementClassFilter(typeof(Level));
            var levels = collector.WherePasses(filter).ToElements();

            foreach (Level level in levels)
                if (level.Name.Equals("Level 1"))
                {
                    level1 = level;
                    break;
                }

            return level1;
        }
    }

    public class FailurePreproccessor : IFailuresPreprocessor
    {
        /// <summary>
        ///     This method is called when there have been failures found at the end of a transaction and Revit is about to start
        ///     processing them.
        /// </summary>
        /// <param name="failuresAccessor">The Interface class that provides access to the failure information. </param>
        /// <returns></returns>
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            var fmas = failuresAccessor.GetFailureMessages();
            if (fmas.Count == 0) return FailureProcessingResult.Continue;

            var transactionName = failuresAccessor.GetTransactionName();
            switch (transactionName)
            {
                case "Warning_FailurePreproccessor":
                {
                    foreach (var fma in fmas)
                    {
                        var id = fma.GetFailureDefinitionId();
                        if (id == Command.IdWarning) failuresAccessor.DeleteWarning(fma);
                    }

                    return FailureProcessingResult.ProceedWithCommit;
                }
                case "Warning_FailurePreproccessor_OverlappedWall":
                {
                    foreach (var fma in fmas)
                    {
                        var id = fma.GetFailureDefinitionId();
                        if (id == BuiltInFailures.OverlapFailures.WallsOverlap) failuresAccessor.DeleteWarning(fma);
                    }

                    return FailureProcessingResult.ProceedWithCommit;
                }
                default:
                    return FailureProcessingResult.Continue;
            }
        }
    }

    /// <summary>
    ///     Implements the interface IFailuresProcessor
    /// </summary>
    public class FailuresProcessor : IFailuresProcessor
    {
        /// <summary>
        ///     This method is being called in case of exception or document destruction to dismiss any possible pending failure UI
        ///     that may have left on the screen
        /// </summary>
        /// <param name="document">Document for which pending failures processing UI should be dismissed </param>
        public void Dismiss(Document document)
        {
        }

        /// <summary>
        ///     Method that Revit will invoke to process failures at the end of transaction.
        /// </summary>
        /// <param name="failuresAccessor">Provides all necessary data to perform the resolution of failures.</param>
        /// <returns></returns>
        public FailureProcessingResult ProcessFailures(FailuresAccessor failuresAccessor)
        {
            var fmas = failuresAccessor.GetFailureMessages();
            if (fmas.Count == 0) return FailureProcessingResult.Continue;

            var transactionName = failuresAccessor.GetTransactionName();
            if (transactionName.Equals("Error_FailuresProcessor"))
            {
                foreach (var fma in fmas)
                {
                    var id = fma.GetFailureDefinitionId();
                    if (id == Command.IdError) failuresAccessor.ResolveFailure(fma);
                }

                return FailureProcessingResult.ProceedWithCommit;
            }

            return FailureProcessingResult.Continue;
        }
    }
}
