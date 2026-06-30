// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoParameter.CS
{
    /// <summary>
    /// Loads family and shared parameters from text files beside the assembly and adds them to a family document.
    /// </summary>
    public class FamilyParameterAssigner
    {
        private readonly Application m_app;
        private readonly string m_assemblyPath;
        private readonly string m_familyFilePath = string.Empty;

        // paramName keys must be unique across family and shared parameters.
        private readonly Dictionary<string /*paramName*/, FamilyParam> m_familyParams;

        private readonly FamilyManager m_manager;

        // Parameter files are loaded once per assigner instance.
        private bool m_paramLoaded;
        private DefinitionFile m_sharedFile;
        private readonly string m_sharedFilePath = string.Empty;

        public FamilyParameterAssigner(Application app, Document doc)
        {
            m_app = app;
            m_manager = doc.FamilyManager;
            m_familyParams = new Dictionary<string, FamilyParam>();
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_paramLoaded = false;
        }

        public bool LoadParametersFromFile()
        {
            if (m_paramLoaded) return true;
            bool famParamFileExist;
            var succeeded = LoadFamilyParameterFromFile(out famParamFileExist);
            if (!succeeded) return false;

            bool sharedParamFileExist;
            succeeded = LoadSharedParameterFromFile(out sharedParamFileExist);
            if (!(famParamFileExist || sharedParamFileExist))
            {
                MessageManager.MessageBuff.AppendLine(
                    "Neither familyParameter.txt nor sharedParameter.txt exists in the assembly folder.");
                return false;
            }

            if (!succeeded) return false;
            m_paramLoaded = true;
            return true;
        }

        private bool LoadFamilyParameterFromFile(out bool exist)
        {
            exist = true;
            var fileName = $"{m_assemblyPath}\\FamilyParameter.txt";
            if (!File.Exists(fileName))
            {
                exist = false;
                return true;
            }

            FileStream file = null;
            StreamReader reader = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);

                string line;
                var lineNumber = 0;
                while (null != (line = reader.ReadLine()))
                {
                    ++lineNumber;
                    var match = Regex.Match(line, @"^\s*$");
                    if (match.Success) continue;

                    // Lines starting with # or * are comments.
                    match = Regex.Match(line, @"\s*['#''*'].*");
                    if (match.Success) continue;

                    // Expected format: paramName, paramGroup, paramType, isInstance (whitespace-separated).
                    var lineData = Regex.Split(line, @"\s+");
                    var values = new List<string>();
                    foreach (var data in lineData)
                    {
                        match = Regex.Match(data, @"^\s*$");
                        if (match.Success) continue;

                        values.Add(data);
                    }

                    if (4 != values.Count)
                    {
                        MessageManager.MessageBuff.Append("Loading family parameter data from \"FamilyParam.txt\".");
                        MessageManager.MessageBuff.Append($"Line [\"{line}]\"doesn't follow the valid format.\n");
                        return false;
                    }

                    var paramName = values[0];
                    var paramGroup = new ForgeTypeId(values[1]);
                    var paramType = new ForgeTypeId(values[2]);
                    var isInstanceString = values[3];
                    var isInstance = Convert.ToBoolean(isInstanceString);

                    var param = new FamilyParam(paramName, paramGroup, paramType, isInstance, lineNumber);
                    if (m_familyParams.ContainsKey(paramName))
                    {
                        var duplicatedParam = m_familyParams[paramName];
                        var warning =
                            $"Line {param.Line}has a duplicate parameter name with Line {duplicatedParam.Line}\n";
                        MessageManager.MessageBuff.Append(warning);
                        continue;
                    }

                    m_familyParams.Add(paramName, param);
                }
            }
            catch (Exception e)
            {
                MessageManager.MessageBuff.AppendLine(e.Message);
                return false;
            }
            finally
            {
                reader?.Close();
                file?.Close();
            }

            return true;
        }

        private bool LoadSharedParameterFromFile(out bool exist)
        {
            exist = true;
            var filePath = $"{m_assemblyPath}\\SharedParameter.txt";
            if (!File.Exists(filePath))
            {
                exist = false;
                return true;
            }

            m_app.SharedParametersFilename = filePath;
            try
            {
                m_sharedFile = m_app.OpenSharedParameterFile();
            }
            catch (Exception e)
            {
                MessageManager.MessageBuff.AppendLine(e.Message);
                return false;
            }

            return true;
        }

        public bool AddParameters()
        {
            var succeeded = AddFamilyParameter();
            if (!succeeded) return false;

            succeeded = AddSharedParameter();
            return succeeded;
        }

        private bool AddFamilyParameter()
        {
            var allParamValid = true;
            if (File.Exists(m_familyFilePath) &&
                0 == m_familyParams.Count)
            {
                MessageManager.MessageBuff.AppendLine("No family parameter available for adding.");
                return false;
            }

            foreach (FamilyParameter param in m_manager.Parameters)
            {
                var name = param.Definition.Name;
                if (m_familyParams.ContainsKey(name))
                {
                    allParamValid = false;
                    var famParam = m_familyParams[name];
                    MessageManager.MessageBuff.Append(
                        $"Line {famParam.Line}: paramName \"{famParam.Name}\"already exists in the family document.\n");
                }
            }

            if (!allParamValid) return false;

            foreach (var param in m_familyParams.Values)
            {
                try
                {
                    m_manager.AddParameter(param.Name, param.Group, param.Type, param.IsInstance);
                }
                catch (Exception e)
                {
                    MessageManager.MessageBuff.AppendLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        private bool AddSharedParameter()
        {
            if (File.Exists(m_sharedFilePath) &&
                null == m_sharedFile)
            {
                MessageManager.MessageBuff.AppendLine("SharedParameter.txt has an invalid format.");
                return false;
            }

            foreach (var group in m_sharedFile.Groups)
            {
                foreach (ExternalDefinition def in group.Definitions)
                {
                    var param = m_manager.get_Parameter(def.Name);
                    if (null != param) continue;

                    try
                    {
                        m_manager.AddParameter(def, def.GetGroupTypeId(), true);
                    }
                    catch (Exception e)
                    {
                        MessageManager.MessageBuff.AppendLine(e.Message);
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class FamilyParam
    {
        private FamilyParam()
        {
        }

        public FamilyParam(string name, ForgeTypeId group, ForgeTypeId type, bool isInstance, int line)
        {
            Name = name;
            Group = group;
            Type = type;
            IsInstance = isInstance;
            Line = line;
        }

        public string Name { get; }

        public ForgeTypeId Group { get; }

        public ForgeTypeId Type { get; }

        public bool IsInstance { get; }

        /// <summary>Source line number in FamilyParameter.txt.</summary>
        public int Line { get; }
    }
}
