// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.AutoParameter.CS
{
    /// <summary>
    ///     add parameters(family parameters/shared parameters) to the opened family file
    ///     the parameters are recorded in txt file following certain formats
    /// </summary>
    internal class FamilyParameterAssigner
    {
        private readonly Application m_app;
        private readonly string m_assemblyPath;
        private readonly string m_familyFilePath = string.Empty;

        // set the paramName as key of dictionary for exclusiveness (the names of parameters should be unique)
        private readonly Dictionary<string /*paramName*/, FamilyParam> m_familyParams;

        private readonly FamilyManager m_manager;

        // indicate whether the parameter files have been loaded. If yes, no need to load again.
        private bool m_paramLoaded;
        private DefinitionFile m_sharedFile;
        private readonly string m_sharedFilePath = string.Empty;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="app">
        ///     the active revit application
        /// </param>
        /// <param name="doc">
        ///     the family document which will have parameters added in
        /// </param>
        public FamilyParameterAssigner(Application app, Document doc)
        {
            m_app = app;
            m_manager = doc.FamilyManager;
            m_familyParams = new Dictionary<string, FamilyParam>();
            m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_paramLoaded = false;
        }

        /// <summary>
        ///     load the family parameter file (if exists) and shared parameter file (if exists)
        ///     only need to load once
        /// </summary>
        /// <returns>
        ///     if succeeded, return true; otherwise false
        /// </returns>
        public bool LoadParametersFromFile()
        {
            if (m_paramLoaded) return true;
            // load family parameter file
            bool famParamFileExist;
            var succeeded = LoadFamilyParameterFromFile(out famParamFileExist);
            if (!succeeded) return false;

            // load shared parameter file
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

        /// <summary>
        ///     load family parameters from the text file
        /// </summary>
        /// <returns>
        ///     return true if succeeded; otherwise false
        /// </returns>
        private bool LoadFamilyParameterFromFile(out bool exist)
        {
            exist = true;
            // step 1: find the file "FamilyParameter.txt" and open it
            var fileName = m_assemblyPath + "\\FamilyParameter.txt";
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

                // step 2: read each line, if the line records the family parameter data, store it
                // record the content of the current line
                string line;
                // record the row number of the current line
                var lineNumber = 0;
                while (null != (line = reader.ReadLine()))
                {
                    ++lineNumber;
                    // step 2.1: verify the line
                    // check whether the line is blank line (contains only whitespaces)
                    var match = Regex.Match(line, @"^\s*$");
                    if (match.Success) continue;

                    // check whether the line starts from "#" or "*" (comment line)
                    match = Regex.Match(line, @"\s*['#''*'].*");
                    if (match.Success) continue;

                    // step 2.2: get the parameter data
                    // it's a valid line (has the format of "paramName   paramGroup    paramType    isInstance", separate by tab or by spaces)
                    // split the line to an array containing parameter items (format of string[] {"paramName", "paramGroup", "paramType", "isInstance"})
                    var lineData = Regex.Split(line, @"\s+");
                    // check whether the array has blank items (containing only spaces)
                    var values = new List<string>();
                    foreach (var data in lineData)
                    {
                        match = Regex.Match(data, @"^\s*$");
                        if (match.Success) continue;

                        values.Add(data);
                    }

                    // verify the parameter items (should have 4 items exactly: paramName, paramGroup, paramType and isInstance)
                    if (4 != values.Count)
                    {
                        MessageManager.MessageBuff.Append("Loading family parameter data from \"FamilyParam.txt\".");
                        MessageManager.MessageBuff.Append("Line [\"" + line + "]\"" +
                                                          "doesn't follow the valid format.\n");
                        return false;
                    }

                    // get the paramName
                    var paramName = values[0];
                    // get the paramGroup
                    var paramGroup = new ForgeTypeId(values[1]);

                    // get the paramType
                    var paramType = new ForgeTypeId(values[2]);
                    // get data "isInstance"
                    var isInstanceString = values[3];
                    var isInstance = Convert.ToBoolean(isInstanceString);

                    // step 2.3: store the parameter fetched, check for exclusiveness (as the names of parameters should keep unique)
                    var param = new FamilyParam(paramName, paramGroup, paramType, isInstance, lineNumber);
                    // the family parameter with the same name has already been stored to the dictionary, raise an error
                    if (m_familyParams.ContainsKey(paramName))
                    {
                        var duplicatedParam = m_familyParams[paramName];
                        var warning = "Line " + param.Line + "has a duplicate parameter name with Line " +
                                      duplicatedParam.Line + "\n";
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

        /// <summary>
        ///     load family parameters from the text file
        /// </summary>
        /// <param name="exist">
        ///     indicate whether the shared parameter file exists
        /// </param>
        /// <returns>
        ///     return true if succeeded; otherwise false
        /// </returns>
        private bool LoadSharedParameterFromFile(out bool exist)
        {
            exist = true;
            var filePath = m_assemblyPath + "\\SharedParameter.txt";
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

        /// <summary>
        ///     add parameters to the family file
        /// </summary>
        /// <returns>
        ///     if succeeded, return true; otherwise false
        /// </returns>
        public bool AddParameters()
        {
            // add the loaded family parameters to the family
            var succeeded = AddFamilyParameter();
            if (!succeeded) return false;

            // add the loaded shared parameters to the family
            succeeded = AddSharedParameter();
            return succeeded;
        }

        /// <summary>
        ///     add family parameter to the family
        /// </summary>
        /// <returns>
        ///     if succeeded, return true; otherwise false
        /// </returns>
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
                    MessageManager.MessageBuff.Append("Line " + famParam.Line + ": paramName \"" + famParam.Name +
                                                      "\"already exists in the family document.\n");
                }
            }

            // there're errors in the family parameter text file
            if (!allParamValid) return false;

            foreach (var param in m_familyParams.Values)
                try
                {
                    m_manager.AddParameter(param.Name, param.Group, param.Type, param.IsInstance);
                }
                catch (Exception e)
                {
                    MessageManager.MessageBuff.AppendLine(e.Message);
                    return false;
                }

            return true;
        }

        /// <summary>
        ///     load shared parameters from shared parameter file and add them to family
        /// </summary>
        /// <returns>
        ///     if succeeded, return true; otherwise false
        /// </returns>
        private bool AddSharedParameter()
        {
            if (File.Exists(m_sharedFilePath) &&
                null == m_sharedFile)
            {
                MessageManager.MessageBuff.AppendLine("SharedParameter.txt has an invalid format.");
                return false;
            }

            foreach (var group in m_sharedFile.Groups)
            foreach (ExternalDefinition def in group.Definitions)
            {
                // check whether the parameter already exists in the document
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

            return true;
        }
    } // end of class "FamilyParameterAssigner"

    /// <summary>
    ///     record the data of a parameter: its name, its group, etc
    /// </summary>
    internal class FamilyParam
    {
        /// <summary>
        ///     default constructor, hide this by making it "private"
        /// </summary>
        private FamilyParam()
        {
        }

        /// <summary>
        ///     constructor which exposes for invoking
        /// </summary>
        /// <param name="name">
        ///     parameter name
        /// </param>
        /// <param name="group">
        ///     indicate which group the parameter belongs to
        /// </param>
        /// <param name="type">
        ///     the type of the parameter
        /// </param>
        /// <param name="isInstance">
        ///     indicate whethe the parameter is an instance parameter
        /// </param>
        /// <param name="line">
        ///     record the location of this parameter in the family parameter file
        /// </param>
        public FamilyParam(string name, ForgeTypeId group, ForgeTypeId type, bool isInstance, int line)
        {
            Name = name;
            Group = group;
            Type = type;
            IsInstance = isInstance;
            Line = line;
        }

        /// <summary>
        ///     the caption of the parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     the group of the parameter
        /// </summary>
        public ForgeTypeId Group { get; }

        /// <summary>
        ///     the type of the parameter
        /// </summary>
        public ForgeTypeId Type { get; }

        /// <summary>
        ///     indicate whether the parameter is an instance parameter or a type parameter
        /// </summary>
        public bool IsInstance { get; }

        /// <summary>
        ///     record the location of this parameter in the family parameter file
        /// </summary>
        public int Line { get; }
    }
}
