// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using RvtSamples.Properties;

namespace Ara3D.RevitSampleBrowser.RvtSamples.CS
{
    /// <summary>
    ///     Main external application class.
    ///     A generic menu generator application.
    ///     Read a text file and add entries to the Revit menu.
    ///     Any number and location of entries is supported.
    /// </summary>
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     Default pulldown menus for samples
        /// </summary>
        public enum DefaultPulldownMenus
        {
            /// <summary>
            ///     Menu for Basics category
            /// </summary>
            Basics,

            /// <summary>
            ///     Menu for Geometry category
            /// </summary>
            Geometry,

            /// <summary>
            ///     Menu for Parameters category
            /// </summary>
            Parameters,

            /// <summary>
            ///     Menu for Elements category
            /// </summary>
            Elements,

            /// <summary>
            ///     Menu for Families category
            /// </summary>
            Families,

            /// <summary>
            ///     Menu for Materials category
            /// </summary>
            Materials,

            /// <summary>
            ///     Menu for Annotation category
            /// </summary>
            Annotation,

            /// <summary>
            ///     Menu for Views category
            /// </summary>
            Views,

            /// <summary>
            ///     Menu for Rooms/Spaces category
            /// </summary>
            RoomsAndSpaces,

            /// <summary>
            ///     Menu for Data Exchange category
            /// </summary>
            DataExchange,

            /// <summary>
            ///     Menu for MEP category
            /// </summary>
            Mep,

            /// <summary>
            ///     Menu for Structure category
            /// </summary>
            Structure,

            /// <summary>
            ///     Menu for Analysis category
            /// </summary>
            Analysis,

            /// <summary>
            ///     Menu for Massing category
            /// </summary>
            Massing,

            /// <summary>
            ///     Menu for Selection category
            /// </summary>
            Selection
        }

        /// <summary>
        ///     Name of file which contains information required for menu items
        /// </summary>
        public const string FileNameStem = "RvtSamples.txt";

        /// <summary>
        ///     Separator of category for samples have more than one category
        /// </summary>
        private static readonly char[] SCharSeparatorOfCategory = { ',' };

        /// <summary>
        ///     chars which will be trimmed in the file to include extra sample list files
        /// </summary>
        private static readonly char[] STrimChars = { ' ', '"', '\'', '<', '>' };

        /// <summary>
        ///     The start symbol of lines to include extra sample list files
        /// </summary>
        private static readonly string SIncludeSymbol = "#include";

        /// <summary>
        ///     Assembly directory
        /// </summary>
        private static readonly string SAssemblyDirectory =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     The controlled application of Revit
        /// </summary>
        private UIControlledApplication m_application;

        /// <summary>
        ///     List contains information of samples not belong to default pulldown menus
        /// </summary>
        private readonly SortedList<string, List<SampleItem>> m_customizedMenus =
            new SortedList<string, List<SampleItem>>();

        /// <summary>
        ///     List contains information of samples belong to default pulldown menus
        /// </summary>
        private readonly SortedList<string, List<SampleItem>> m_defaultMenus =
            new SortedList<string, List<SampleItem>>();

        /// <summary>
        ///     Panel for RvtSamples
        /// </summary>
        private RibbonPanel m_panelRvtSamples;

        /// <summary>
        ///     List contains all pulldown button items contain sample items
        /// </summary>
        private readonly SortedList<string, PulldownButton>
            m_pulldownButtons = new SortedList<string, PulldownButton>();

        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If false is returned then Revit should inform the user that the external application
        ///     failed to load and the release the public reference.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            m_application = application;
            var rc = Result.Failed;
            string[] lines = null;
            var n = 0;
            var k = 0;

            try
            {
                // Check whether the file contains samples' list exists
                // If not, return failure
                var filename = FileNameStem;
                if (!GetFilepath(ref filename))
                {
                    ErrorMsg($"{FileNameStem} not found.");
                    return rc;
                }

                // Read all lines from the file
                lines = ReadAllLinesWithInclude(filename);
                // Remove comments
                lines = RemoveComments(lines);

                // Add default pulldown menus of samples to Revit
                m_panelRvtSamples = application.CreateRibbonPanel("RvtSamples");
                var i = 0;
                var pdData = new List<PulldownButtonData>(3);
                foreach (var category in Enum.GetNames(typeof(DefaultPulldownMenus)))
                {
                    if ((i + 1) % 3 == 1) pdData.Clear();

                    //
                    // Prepare PulldownButtonData for add stacked buttons operation
                    //
                    var displayName = GetDisplayNameByEnumName(category);

                    var sampleItems = new List<SampleItem>();
                    m_defaultMenus.Add(category, sampleItems);

                    var data = new PulldownButtonData(displayName, displayName);
                    pdData.Add(data);

                    //
                    // Add stacked buttons to RvtSamples panel and set their display names and images
                    //
                    if ((i + 1) % 3 == 0)
                    {
                        var addedButtons = m_panelRvtSamples.AddStackedItems(pdData[0], pdData[1], pdData[2]);
                        foreach (var item in addedButtons)
                        {
                            var name = item.ItemText;
                            var enumName = GetEnumNameByDisplayName(name);
                            var button = item as PulldownButton;
                            button.Image = new BitmapImage(
                                new Uri(Path.Combine(SAssemblyDirectory, $"Icons\\{enumName}.ico"),
                                    UriKind.Absolute));
                            button.ToolTip = Resource.ResourceManager.GetString(enumName);
                            m_pulldownButtons.Add(name, button);
                        }
                    }

                    i++;
                }

                //
                // Add sample items to the pulldown buttons
                //
                n = lines.GetLength(0);
                k = 0;
                while (k < n) AddSample(lines, n, ref k);

                AddSamplesToDefaultPulldownMenus();
                AddCustomizedPulldownMenus();

                rc = Result.Succeeded;
            }
            catch (Exception e)
            {
                var s = $"{e.Message}: n = {n}, k = {k}, lines[k] = {(k < n ? lines[k] : "eof")}";

                ErrorMsg(s);
            }

            return rc;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Get a button's enum name by its display name
        /// </summary>
        /// <param name="name">display name</param>
        /// <returns>enum name</returns>
        private string GetEnumNameByDisplayName(string name)
        {
            string enumName = null;
            switch (name)
            {
                case "Rooms/Spaces":
                    enumName = DefaultPulldownMenus.RoomsAndSpaces.ToString();
                    break;
                case "Data Exchange":
                    enumName = DefaultPulldownMenus.DataExchange.ToString();
                    break;
                default:
                    enumName = name;
                    break;
            }

            return enumName;
        }

        /// <summary>
        ///     Get a button's display name by its enum name
        /// </summary>
        /// <param name="enumName">The button's enum name</param>
        /// <returns>The button's display name</returns>
        private string GetDisplayNameByEnumName(string enumName)
        {
            string displayName = null;
            if (enumName.Equals(DefaultPulldownMenus.RoomsAndSpaces.ToString()))
                displayName = "Rooms/Spaces";
            else if (enumName.Equals(DefaultPulldownMenus.DataExchange.ToString()))
                displayName = "Data Exchange";
            else
                displayName = enumName;

            return displayName;
        }

        /// <summary>
        ///     Display error message
        /// </summary>
        /// <param name="msg">Message to display</param>
        public void ErrorMsg(string msg)
        {
            Debug.WriteLine($"RvtSamples: {msg}");
            TaskDialog.Show("RvtSamples", msg, TaskDialogCommonButtons.Ok);
        }

        /// <summary>
        ///     Read file contents, including contents of files included in the current file
        /// </summary>
        /// <param name="filename">Current file to be read</param>
        /// <returns>All lines of file contents</returns>
        private string[] ReadAllLinesWithInclude(string filename)
        {
            if (!File.Exists(filename))
            {
                ErrorMsg($"{filename} not found.");
                return new string[] { };
            }

            var lines = File.ReadAllLines(filename);

            var n = lines.GetLength(0);
            var allLines = new ArrayList(n);
            foreach (var line in lines)
            {
                var s = line.TrimStart();
                if (s.ToLower().StartsWith(SIncludeSymbol))
                {
                    var filename2 = s.Substring(SIncludeSymbol.Length);
                    filename2 = filename2.Trim(STrimChars);
                    allLines.AddRange(ReadAllLinesWithInclude(filename2));
                }
                else
                {
                    allLines.Add(line);
                }
            }

            return allLines.ToArray(typeof(string)) as string[];
        }

        /// <summary>
        ///     Get the input file path.
        ///     Search and return the full path for the given file
        ///     in the current exe directory or one or two directory levels higher.
        /// </summary>
        /// <param name="filename">Input filename stem, output full file path</param>
        /// <returns>True if found, false otherwise.</returns>
        private bool GetFilepath(ref string filename)
        {
            var path = Path.Combine(SAssemblyDirectory, filename);
            var rc = File.Exists(path);

            // Get full path of the file
            if (rc) filename = Path.GetFullPath(path);
            return rc;
        }

        /// <summary>
        ///     Remove all comments and empty lines from a given array of lines.
        ///     Comments are delimited by '#' to the end of the line.
        /// </summary>
        private string[] RemoveComments(string[] lines)
        {
            var n = lines.GetLength(0);
            var a = new string[n];
            var i = 0;
            foreach (var line in lines)
            {
                var s = line;
                var j = s.IndexOf('#');
                if (0 <= j) s = s.Substring(0, j);
                s = s.Trim();
                if (0 < s.Length) a[i++] = s;
            }

            var b = new string[i];
            n = i;
            for (i = 0; i < n; ++i) b[i] = a[i];
            return b;
        }

        /// <summary>
        ///     Add a new command to the corresponding pulldown button.
        /// </summary>
        /// <param name="lines">
        ///     Array of lines defining sample's category, display name, description, large image, image, assembly
        ///     and classname
        /// </param>
        /// <param name="n">Total number of lines in array</param>
        /// <param name="i">Current index in array</param>
        private void AddSample(string[] lines, int n, ref int i)
        {
            if (n < i + 6)
                throw new Exception($"Incomplete record at line {i} of {FileNameStem}");

            var categories = lines[i++].Trim();
            var displayName = lines[i++].Trim();
            var description = lines[i++].Trim();
            var largeImage = lines[i++].Remove(0, 11).Trim();
            var image = lines[i++].Remove(0, 6).Trim();
            var assembly = lines[i++].Trim();
            var className = lines[i++].Trim();

            if (!File.Exists(assembly)) // jeremy
                ErrorMsg($"Assembly '{assembly}' specified in line {i} of {FileNameStem} not found");

            var testClassName = false; // jeremy
            if (testClassName)
            {
                Debug.Print("RvtSamples: testing command {0} in assembly '{1}'.", className, assembly);

                try
                {
                    // first load the revit api assembly, otherwise we cannot query the external app for its types:

                    //Assembly revit = Assembly.LoadFrom( "C:/Program Files/Revit Architecture 2009/Program/RevitAPI.dll" );
                    //string root = "C:/Program Files/Autodesk Revit Architecture 2010/Program/";
                    //Assembly adWindows = Assembly.LoadFrom( root + "AdWindows.dll" );
                    //Assembly uiFramework = Assembly.LoadFrom( root + "UIFramework.dll" );
                    //Assembly revit = Assembly.LoadFrom( root + "RevitAPI.dll" );

                    // load the assembly into the current application domain:

                    var a = Assembly.LoadFrom(assembly);

                    if (null == a)
                    {
                        ErrorMsg($"Unable to load assembly '{assembly}' specified in line {i} of {FileNameStem}");
                    }
                    else
                    {
                        // get the type to use:
                        var t = a.GetType(className);
                        if (null == t)
                        {
                            ErrorMsg(
                                $"External command class {className} in assembly '{assembly}' specified in line {i} of {FileNameStem} not found");
                        }
                        else
                        {
                            // get the method to call:
                            var m = t.GetMethod("Execute");
                            if (null == m)
                                ErrorMsg(
                                    $"External command class {className} in assembly '{assembly}' specified in line {i} of {FileNameStem} does not define an Execute method");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg(
                        $"Exception '{ex.Message}' \ntesting assembly '{assembly}' \nspecified in line {i} of {FileNameStem}");
                }
            }

            //
            // If sample belongs to default category, add the sample item to the sample list of the default category
            // If not, store the information for adding to RvtSamples panel later
            //
            var entries = categories.Split(SCharSeparatorOfCategory, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in entries)
            {
                var category = value.Trim();
                var item = new SampleItem(category, displayName, description, largeImage, image, assembly, className);
                if (m_pulldownButtons.ContainsKey(category))
                {
                    m_defaultMenus.Values[m_defaultMenus.IndexOfKey(GetEnumNameByDisplayName(category))].Add(item);
                }
                else if (m_customizedMenus.ContainsKey(category))
                {
                    var sampleItems = m_customizedMenus.Values[m_customizedMenus.IndexOfKey(category)];
                    sampleItems.Add(item);
                }
                else
                {
                    var sampleItems = new List<SampleItem> { item };
                    m_customizedMenus.Add(category, sampleItems);
                }
            }
        }

        /// <summary>
        ///     Add sample item to pulldown menu
        /// </summary>
        /// <param name="pullDownButton">Pulldown menu</param>
        /// <param name="item">Sample item to be added</param>
        private void AddSampleToPulldownMenu(PulldownButton pullDownButton, SampleItem item)
        {
            var pushButtonData = new PushButtonData(item.DisplayName, item.DisplayName, item.Assembly, item.ClassName);
            var pushButton = pullDownButton.AddPushButton(pushButtonData);
            if (!string.IsNullOrEmpty(item.LargeImage))
            {
                var largeImageSource = new BitmapImage(new Uri(item.LargeImage, UriKind.Absolute));
                pushButton.LargeImage = largeImageSource;
            }

            if (!string.IsNullOrEmpty(item.Image))
            {
                var imageSource = new BitmapImage(new Uri(item.Image, UriKind.Absolute));
                pushButton.Image = imageSource;
            }

            pushButton.ToolTip = item.Description;
        }

        /// <summary>
        ///     Comparer to sort sample items by their display name
        /// </summary>
        /// <param name="s1">sample item 1</param>
        /// <param name="s2">sample item 2</param>
        /// <returns>compare result</returns>
        private static int SortByDisplayName(SampleItem item1, SampleItem item2)
        {
            return string.Compare(item1.DisplayName, item2.DisplayName);
        }

        /// <summary>
        ///     Sort samples in one category by the sample items' display name
        /// </summary>
        /// <param name="menus">samples to be sorted</param>
        private void SortSampleItemsInOneCategory(SortedList<string, List<SampleItem>> menus)
        {
            var iCount = menus.Count;

            for (var j = 0; j < iCount; j++)
            {
                var sampleItems = menus.Values[j];
                sampleItems.Sort(SortByDisplayName);
            }
        }

        /// <summary>
        ///     Add samples of categories in default categories
        /// </summary>
        private void AddSamplesToDefaultPulldownMenus()
        {
            var iCount = m_defaultMenus.Count;

            // Sort sample items in every category by display name
            SortSampleItemsInOneCategory(m_defaultMenus);

            for (var i = 0; i < iCount; i++)
            {
                var category = m_defaultMenus.Keys[i];
                var sampleItems = m_defaultMenus.Values[i];
                var menuButton =
                    m_pulldownButtons.Values[m_pulldownButtons.IndexOfKey(GetDisplayNameByEnumName(category))];
                foreach (var item in sampleItems)
                {
                    AddSampleToPulldownMenu(menuButton, item);
                }
            }
        }

        /// <summary>
        ///     Add samples of categories not in default categories
        /// </summary>
        private void AddCustomizedPulldownMenus()
        {
            var iCount = m_customizedMenus.Count;

            // Sort sample items in every category by display name
            SortSampleItemsInOneCategory(m_customizedMenus);

            var i = 0;

            while (iCount >= 3)
            {
                var name = m_customizedMenus.Keys[i++];
                var data1 = new PulldownButtonData(name, name);
                name = m_customizedMenus.Keys[i++];
                var data2 = new PulldownButtonData(name, name);
                name = m_customizedMenus.Keys[i++];
                var data3 = new PulldownButtonData(name, name);
                var buttons = m_panelRvtSamples.AddStackedItems(data1, data2, data3);
                AddSamplesToStackedButtons(buttons);

                iCount -= 3;
            }

            switch (iCount)
            {
                case 2:
                {
                    var name = m_customizedMenus.Keys[i++];
                    var data1 = new PulldownButtonData(name, name);
                    name = m_customizedMenus.Keys[i++];
                    var data2 = new PulldownButtonData(name, name);
                    var buttons = m_panelRvtSamples.AddStackedItems(data1, data2);
                    AddSamplesToStackedButtons(buttons);
                    break;
                }
                case 1:
                {
                    var name = m_customizedMenus.Keys[i];
                    var pulldownButtonData = new PulldownButtonData(name, name);
                    var button = m_panelRvtSamples.AddItem(pulldownButtonData) as PulldownButton;
                    var sampleItems = m_customizedMenus.Values[m_customizedMenus.IndexOfKey(button.Name)];
                    foreach (var item in sampleItems)
                    {
                        AddSampleToPulldownMenu(button, item);
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Add samples to corresponding pulldown button
        /// </summary>
        /// <param name="buttons">pulldown buttons</param>
        private void AddSamplesToStackedButtons(IList<RibbonItem> buttons)
        {
            foreach (var rItem in buttons)
            {
                var button = rItem as PulldownButton;
                var sampleItems = m_customizedMenus.Values[m_customizedMenus.IndexOfKey(button.Name)];
                foreach (var item in sampleItems)
                {
                    AddSampleToPulldownMenu(button, item);
                }
            }
        }
    }
}
