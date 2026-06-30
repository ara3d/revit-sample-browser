// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    public class StairsAutomationUtility
    {
        private readonly int m_stairsNumber;

        public Level BottomLevel { get; set; }

        public Level TopLevel { get; set; }

        protected Document Document { get; }

        protected Stairs Stairs { get; private set; }

        protected StairsAutomationUtility(Document document, int stairsNumber)
        {
            Document = document;
            m_stairsNumber = stairsNumber;
        }

        public static StairsAutomationUtility Create(Document document, int stairsNumber)
        {
            return new StairsAutomationUtility(document, stairsNumber);
        }

        private void SetupLevels()
        {
            var targetLevels = StairsHelper.FindTargetLevels(Document, "Level 1", "Level 2", "Level 3");
            switch (m_stairsNumber)
            {
                case 3:
                    BottomLevel = targetLevels.Item1;
                    TopLevel = targetLevels.Item2;
                    break;
                default:
                    BottomLevel = targetLevels.Item1;
                    TopLevel = targetLevels.Item3;
                    break;
            }
        }

        // Not currently used.
#if false
        private static bool IsStoryLevel(Level level)
        {
            Parameter p = level.get_Parameter(BuiltInParameter.LEVEL_IS_BUILDING_STORY);
            return (p != null && p.AsInteger() != 0);
        }

        private bool IsBetweenExtents(Level level)
        {
            if (level.Id == BottomLevel.Id || level.Id == TopLevel.Id)
                return false;
            return level.Elevation > BottomLevel.Elevation && level.Elevation < TopLevel.Elevation;
        }

        private IEnumerable<Level> FindStoryLevelsBetweenExtents()
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(Level));

            return collector.Cast<Level>().Where<Level>(level => IsBetweenExtents(level) && IsStoryLevel(level));
        }
#endif

        /// <summary>
        ///     Sets up and returns the hardcoded configuration corresponding to the stairs number.
        /// </summary>
        /// <remarks>
        ///     Some configuration types will require the ability to make changes to the stairs element.
        ///     Thus, this method needs an open transaction.
        /// </remarks>
        /// <returns></returns>
        protected virtual IStairsConfiguration SetupHardcodedConfiguration()
        {
            switch (m_stairsNumber)
            {
                // Straight run 1 level
                case 0:
                    {
                        StairsSingleStraightRun run = new(Stairs, BottomLevel,
                            Transform.CreateTranslation(new XYZ(100, 0, 0)));
                        run.SetRunWidth(15.0);
                        return run;
                    }
                // Curved run 1 level
                case 1:
                    {
                        StairsSingleCurvedRun run = new(Stairs, BottomLevel, 6.0);
                        run.SetRunWidth(10.0);
                        return run;
                    }
                // Curve run 2 level
                case 2:
                    return new StairsSingleCurvedRun(Stairs, BottomLevel, 3.0,
                        Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI, new XYZ(10, -20, 0)));
                // Standard stair 1 level
                case 3:
                    {
                        StairsStandardConfiguration configuration = new(Stairs, BottomLevel, 1,
                            Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI / 4.0, new XYZ(-20, -20, 0)))
                        {
                            EqualizeRuns = true
                        };
                        configuration.Initialize();
                        return configuration;
                    }
                // Standard stair multi-level
                case 4:
                    {
                        StairsStandardConfiguration configuration = new(Stairs, BottomLevel, 3,
                            Transform.CreateRotationAtPoint(XYZ.BasisZ, 7.0 * Math.PI / 6.0, new XYZ(15, 10, 0)))
                        {
                            RunWidth = 6.0,
                            RunOffset = 8.0 / 12.0,
                            LandingWidth = 4.0,
                            EqualizeRuns = true
                        };
                        configuration.Initialize();
                        return configuration;
                    }
                case 100:
                    {
                        return new StairsSingleSketchedStraightRun(Stairs, BottomLevel,
                            Transform.CreateTranslation(new XYZ(50, 0, 0)));
                    }
                case 101:
                    {
                        return new StairsSingleSketchedCurvedRun(Stairs, BottomLevel, 6.0,
                            Transform.CreateTranslation(new XYZ(-10, 0, 0)));
                    }
            }

            return null;
        }

        public void GenerateStairs()
        {
            SetupLevels();

            // Prepare and maintain StairsEditScope for stairs creation activities
            using StairsEditScope editScope = new(Document, "Stairs Automation");
            // Instantiate the new stairs element.
            var stairsElementId = editScope.Start(BottomLevel.Id, TopLevel.Id);

            // Remember the stairs for use in creation of the run and landing configurations.
            Stairs = Document.GetElement(stairsElementId) as Stairs;

            // Setup a transaction for use during the run and landing creation
            using (Transaction t = new(Document, "Stairs Automation"))
            {
                t.Start();

                // Setup the configuration
                var configuration = SetupHardcodedConfiguration();
                if (configuration == null)
                    return;

                // Create each run
                var numberOfRuns = configuration.GetNumberOfRuns();
                for (var i = 0; i < numberOfRuns; i++) configuration.CreateStairsRun(Document, stairsElementId, i);

                // Create each landing
                var numberOfLandings = configuration.GetNumberOfLandings();
                for (var i = 0; i < numberOfLandings; i++)
                    configuration.CreateLanding(Document, stairsElementId, i);

                t.Commit();
            }

            editScope.Commit(new StairsEditScopeFailuresPreprocessor());
        }
    }

    public class StairsEditScopeFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }
}
