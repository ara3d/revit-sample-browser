// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser
{
    public static class CommandParser
    {
        public static readonly List<ICommandDefinition> Definitions =
        [
            new NotElementTypeCmdDefinition(),
            new ElementTypeCmdDefinition(),
            new OwnerViewFilterCmdDefinition(),
            new SelectionCmdDefinition(),
            new VisibleInViewCmdDefinition(),
            new CategoryCmdDefinition(),
            new ClassCmdDefinition(),
            new ElementIdCmdDefinition(),
            new LevelCmdDefinition(),
            new NameCmdDefinition(),
            new ParameterCmdDefinition(),
            new RoomCmdDefinition(),
            new RuleBasedFilterCmdDefinition(),
            new StructuralTypeCmdDefinition(),
            new UniqueIdCmdDefinition(),
            new WorksetCmdDefinition(),
        ];
        private static readonly Dictionary<string, ICommandDefinition> classifierToDefinitionMap = [];
        private static readonly Dictionary<string, ICommandDefinition> keywordToDefinitionMap = [];


        public static void Init()
        {

        }
        static CommandParser()
        {
            RegisterClassifiers();
            RegisterKeywords();
            InitDefinitions();
        }
        private static void RegisterClassifiers()
        {
            foreach (var definition in Definitions)
            {
                foreach (var classifier in definition.GetClassifiers())
                {
                    var key = classifier.NormalizeForLookup();
                    if (classifierToDefinitionMap.ContainsKey(key))
                    {
                        throw new Exception("Should it not be possible to be here, but here we are...");
                    }
                    classifierToDefinitionMap[key] = definition;
                }
            }
        }
        private static void RegisterKeywords()
        {
            foreach (var definition in Definitions)
            {
                foreach (var keyword in definition.GetKeywords())
                {
                    var key = keyword.NormalizeForLookup();
                    if (keywordToDefinitionMap.ContainsKey(key))
                    {
                        throw new Exception("Should it not be possible to be here, but here we are...");
                    }
                    keywordToDefinitionMap[key] = definition;
                }
            }
        }
        private static void InitDefinitions()
        {
            foreach (var definition in Definitions.OfType<INeedInitialization>())
            {
                definition.Init();
            }
        }
        public static void LoadDocumentSpecificData(Document document)
        {
            if (document == null) return;

            foreach (var definition in Definitions.OfType<INeedInitializationWithDocument>())
            {
                definition.Init(document);
            }
        }


        public static readonly char[] Separators = new[] { ':' };
        public static IEnumerable<ICommand> Parse(string cmdText)
        {
            var splittedByClassifier = cmdText.Split(Separators, 2, System.StringSplitOptions.None);

            string argument = null;
            ICommandDefinition selectedDefinition = null;

            if (splittedByClassifier.Length == 1)
            {
                var keyword = cmdText.NormalizeForLookup();
                keywordToDefinitionMap.TryGetValue(keyword, out selectedDefinition);
                argument = splittedByClassifier[0].Trim();
            }
            if (splittedByClassifier.Length == 2)
            {
                var classifier = splittedByClassifier[0].NormalizeForLookup();
                classifierToDefinitionMap.TryGetValue(classifier, out selectedDefinition);
                argument = splittedByClassifier[1].Trim();
            }

            if (selectedDefinition == null)
            {
                foreach (var definition in Definitions)
                {
                    if (definition.CanRecognizeArgument(argument))
                    {
                        selectedDefinition = definition;
                        break;
                    }
                }
            }

            if (selectedDefinition != null)
            {
                yield return selectedDefinition.Create(cmdText, argument);
                yield break;
            }

            if (string.IsNullOrEmpty(argument)) yield break;

            var genericSearchResult = DoGenericSearch(cmdText, argument);
            if (genericSearchResult.Any())
            {
                var ordered = genericSearchResult.OrderByDescending(x => x.Score).ToArray();

                var prevScore = ordered.First().Score;

                foreach (var item in ordered)
                {
                    if (Math.Abs(item.Score - prevScore) < 0.13)
                    {
                        yield return item;
                    }
                    else
                    {
                        yield break;
                    }
                }

                yield break;
            }

            //

            yield return NameCmdDefinition.Instance.Create(cmdText, argument);
        }

        private static IEnumerable<ICommand> DoGenericSearch(string cmdText, string argument)
        {
            foreach (var definition in Definitions)
            {
                if (definition.CanParticipateInGenericSearch())
                {
                    var result = definition.Create(cmdText, argument);
                    if (result.Arguments.Any())
                    {
                        yield return result;
                    }
                }
            }
        }

        public static ICommandDefinition GetCommandDefinitionForClassifier(string classifier)
        {
            ICommandDefinition result;
            classifierToDefinitionMap.TryGetValue(classifier.NormalizeForLookup(), out result);
            return result;
        }

        private static string NormalizeForLookup(this String text)
        {
            return text.ToLower().RemoveWhitespace();
        }
    }
}