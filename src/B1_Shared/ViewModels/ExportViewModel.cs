using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using ExcelExporterImporter.Annotations;
using ExcelExporterImporter.Common;
using log4net;
using OfficeOpenXml;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;

namespace ExcelExporterImporter.ViewModels
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        private const int UniqueIdColumn = 1;
        private const int UniqueIdRow = 1;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, string> knownStandards = new Dictionary<string, string>();
        private readonly ParametersSettings parametersSettings;
        private readonly List<string> readonlyStandards = new List<string>();
        private readonly Document revitDocument;
        private readonly Dictionary<string, ViewSchedule> schedulesInModel = new Dictionary<string, ViewSchedule>();
        private readonly Window window;

        private BackgroundWorker backgroundWorker;
        private bool bCheckAllImport;
        private string buttonText;
        private CancellationTokenSource cancellationTokenSource;
        private bool checkExportSchedule;
        private bool checkExportStandard;
        private Dispatcher dispatcher;
        private bool enableButtons;
        private bool enableButtonsBasic;
        private ExcelPackage excelPackageToImport;
        private ExportOptions exportOption = ExportOptions.SeparateTables;
        private ExportOptionsBasic exportOptionBasic = ExportOptionsBasic.SeparateTables;
        private string exportPrefix;
        private string exportPrefixBasic;
        private FileInfo fiExportFile;
        private string importFolder;
        private Progress progress;
        private int selectedTab;

        private string sFileExport;
        private string sIconButton;
        private string sShowExportButton;
        private string sShowImportButton;
        private string title;
        private bool useExportPrefix;
        private bool useExportPrefixBasic;

        /// <summary>
        /// </summary>
        /// <param name="window"></param>
        /// <param name="revitDocument"></param>
        public ExportViewModel(Window window, Document revitDocument) : this()
        {
            this.revitDocument = revitDocument;
            this.window = window;
            Command = new DelegateCommand<object>(OnSubmit, CanSubmit);
            Title = Resources.Export + " - " + AddinInfo.AddinName;
            EnableButtons = true;
            EnableButtonsBasic = true;
            ButtonText = Resources.TitleBtnExport;
            IconButton = B1Paths.ResourcePath("IconButtonExportMini.png");
            ParametersSettings.LoadFromFile(B1Paths.ParametersSettingsPath, out parametersSettings);
            FillLists();
        }

        /// <summary>
        /// </summary>
        public ExportViewModel()
        {
            SchedulesList = new ObservableCollection<CheckedListItem<string>>();
            SchedulesListBasic = new ObservableCollection<CheckedListItem<string>>();

            StandardsList = new ObservableCollection<CheckedListItem<string>>();

            OrderedSchedulesForExport = CollectionViewSource.GetDefaultView(SchedulesList);
            OrderedSchedulesForExport.SortDescriptions.Add(new SortDescription("Item", ListSortDirection.Ascending));

            OrderedSchedulesForExportBasic = CollectionViewSource.GetDefaultView(SchedulesListBasic);
            OrderedSchedulesForExportBasic.SortDescriptions.Add(
                new SortDescription("Item", ListSortDirection.Ascending));

        }

        public ICollectionView OrderedSchedulesForExport { get; set; }
        public ICollectionView OrderedSchedulesForExportBasic { get; set; }
        public ICollectionView OrderedItemsForImport { get; set; }
        public ICollectionView OrderedItemsForNotImport { get; set; }
        public ObservableCollection<CheckedListItem<string>> SchedulesList { get; set; }
        public ObservableCollection<CheckedListItem<string>> SchedulesListBasic { get; set; }
        public ObservableCollection<CheckedListItem<string>> StandardsList { get; set; }
        public ObservableCollection<CheckedListItem<string>> ImportItemList { get; set; }
        public ObservableCollection<CheckedListItem<string>> NotImportItemList { get; set; }

        public ICommand Command { get; }
        public Action CloseAction { get; set; }

        public string Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                OnPropertyChanged();
            }
        }

        public bool EnableButtons
        {
            get => enableButtons;
            set
            {
                if (value.Equals(enableButtons)) return;
                enableButtons = value;
                OnPropertyChanged();
            }
        }

        public bool EnableButtonsBasic
        {
            get => enableButtonsBasic;
            set
            {
                if (value.Equals(enableButtonsBasic)) return;
                enableButtonsBasic = value;
                OnPropertyChanged();
            }
        }

        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                ButtonText = value == 1 ? Resources.TitleBtnImport : Resources.TitleBtnExport;
                IconButton = value == 1 ? Constants.IconImportButton : Constants.IconExportButton;
                OnPropertyChanged();
            }
        }

        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                OnPropertyChanged();
            }
        }

        public string IconButton
        {
            get => sIconButton;
            set
            {
                sIconButton = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Parameter which contains the path of the last directory used for an import.
        /// </summary>
        public string LastImportFolder
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.Default.LastImportPath))
                {
                    if (string.IsNullOrEmpty(Settings.Default.LastExportPath))
                        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return Settings.Default.LastExportPath;
                }

                return Settings.Default.LastImportPath;
            }
            set
            {
                if (value != Settings.Default.LastImportPath)
                {
                    Settings.Default.LastImportPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Parameter which contains the path of the last directory used for an export.
        /// </summary>
        public string ExportFolder
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.Default.LastExportPath))
                {
                    if (string.IsNullOrEmpty(Settings.Default.LastImportPath))
                        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return Settings.Default.LastImportPath;
                }

                return Settings.Default.LastExportPath;
            }
            set
            {
                if (value != Settings.Default.LastExportPath)
                {
                    Settings.Default.LastExportPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ExportFolderBasic
        {
            get => Settings.Default.LastExportPathBasic;
            set
            {
                if (value != Settings.Default.LastExportPathBasic)
                {
                    Settings.Default.LastExportPathBasic = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImportFolder
        {
            get => importFolder;
            set
            {
                importFolder = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property that contains the visibility value for the Export button
        /// </summary>
        public string ShowExportButton
        {
            get => sShowExportButton;
            set
            {
                sShowExportButton = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property that contains the visibility value for the Import button
        /// </summary>
        public string ShowImportButton
        {
            get => sShowImportButton;
            set
            {
                sShowImportButton = value;
                OnPropertyChanged();
            }
        }

        public bool CheckAllImport
        {
            get => bCheckAllImport;
            set
            {
                bCheckAllImport = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the check box for all selected schedules
        /// </summary>
        public bool CheckExportSchedule
        {
            get => checkExportSchedule;
            set
            {
                checkExportSchedule = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the check box for all selected standard
        /// </summary>
        public bool CheckExportStandard
        {
            get => checkExportStandard;
            set
            {
                checkExportStandard = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the export mode uni
        /// </summary>
        public bool ExportModeUni
        {
            get => Settings.Default.LastExportModeUni;
            set
            {
                if (value) ExportModeBid = false;
                Settings.Default.LastExportModeUni = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the export mode bi
        /// </summary>
        public bool ExportModeBid
        {
            get
            {
                if (!Settings.Default.LastExportModeUni && !Settings.Default.LastExportModeBid) return true;
                return Settings.Default.LastExportModeBid;
            }
            set
            {
                if (value) ExportModeUni = false;
                Settings.Default.LastExportModeBid = value;
                OnPropertyChanged();
            }
        }

        public bool UseExportPrefix
        {
            get => useExportPrefix;
            set
            {
                useExportPrefix = value;
                OnPropertyChanged();
            }
        }

        public bool UseExportPrefixBasic
        {
            get => useExportPrefixBasic;
            set
            {
                useExportPrefixBasic = value;
                OnPropertyChanged();
            }
        }

        public string ExportPrefix
        {
            get => exportPrefix;
            set
            {
                exportPrefix = value;
                OnPropertyChanged();
            }
        }

        public string ExportPrefixBasic
        {
            get => exportPrefixBasic;
            set
            {
                exportPrefixBasic = value;
                OnPropertyChanged();
            }
        }

        public ExportOptions ExportOption
        {
            get => exportOption;
            set
            {
                exportOption = value;
                OnPropertyChanged();
            }
        }

        public ExportOptionsBasic ExportOptionBasic
        {
            get => exportOptionBasic;
            set
            {
                exportOptionBasic = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method for filling the lists for basic export and advanced export
        /// </summary>
        private void FillLists()
        {
            //ElementId sheetCategoryElement = new ElementId(BuiltInCategory.OST_Sheets);// Variable for validation
            //He retrieves the list of Schedules
            var viewSchedules = new FilteredElementCollector(revitDocument).OfClass(typeof(ViewSchedule));
            foreach (ViewSchedule viewSchedule in viewSchedules)
            {
                if (viewSchedule.IsTitleblockRevisionSchedule) continue;

                SchedulesList.Add(new CheckedListItem<string>(viewSchedule.Id.ToString(), viewSchedule.Name,
                    viewSchedule));
                SchedulesListBasic.Add(new CheckedListItem<string>(viewSchedule.Id.ToString(), viewSchedule.Name,
                    viewSchedule));
                schedulesInModel.Add(viewSchedule.UniqueId, viewSchedule);
            }

            StandardsList = new ObservableCollection<CheckedListItem<string>>
            {
                new CheckedListItem<string>(Constants.StandardsGroupLineStylesUniqueId, Constants.StandardsLineStyles,
                    new List<string> {Constants.StandardsGroupItemLineStylesUniqueId}),
                new CheckedListItem<string>(Constants.StandardsGroupObjectStylesUniqueId,
                    Constants.StandardsObjectStyles,
                    new List<string>
                    {
                        Constants.StandardsGroupItemAnnotationObjectsUniqueId,
                        Constants.StandardsGroupItemModelObjectsUniqueId,
                        Constants.StandardsGroupItemAnalyticalModelObjectsUniqueId
                    }),
                new CheckedListItem<string>(Constants.StandardsGroupFamilyListingUniqueId,
                    Constants.StandardsFamilyListing,
                    new List<string> {Constants.StandardsGroupItemFamilyListingUniqueId}),
                new CheckedListItem<string>(Constants.StandardsGroupSharedParametersUniqueId,
                    Constants.StandardsSharedParametersSettings,
                    new List<string> {Constants.StandardsGroupItemProjectSharedParametersSettingsUniqueId}),
                new CheckedListItem<string>(Constants.StandardsGroupProjectParametersUniqueId,
                    Constants.StandardsProjectParametersSettings,
                    new List<string> {Constants.StandardsGroupItemProjectParametersSettingsUniqueId}),
                new CheckedListItem<string>(Constants.StandardsGroupProjectInformationUniqueId,
                    Constants.StandardsProjectInformation,
                    new List<string> {Constants.StandardsGroupItemProjectInformationUniqueId})
            };
            //Variable used for import
            knownStandards.Add(Constants.StandardsGroupItemLineStylesUniqueId, Constants.StandardsLineStyles);
            knownStandards.Add(Constants.StandardsGroupItemAnnotationObjectsUniqueId,
                Constants.StandardsAnnotationObjects);
            knownStandards.Add(Constants.StandardsGroupItemModelObjectsUniqueId, Constants.StandardsModelObjects);
            knownStandards.Add(Constants.StandardsGroupItemAnalyticalModelObjectsUniqueId,
                Constants.StandardsAnalyticalModelObjects);
            knownStandards.Add(Constants.StandardsGroupItemSheetListingUniqueId, Constants.StandardsSheetListing);
            knownStandards.Add(Constants.StandardsGroupItemViewListingUniqueId, Constants.StandardsViewListing);
            knownStandards.Add(Constants.StandardsGroupItemProjectInformationUniqueId,
                Constants.StandardsProjectInformation);
            knownStandards.Add(Constants.StandardsGroupItemMaterialsUniqueId, Constants.StandardsMaterials);
            knownStandards.Add(Constants.StandardsGroupItemProjectParametersSettingsUniqueId,
                Constants.StandardsProjectParameters);
            knownStandards.Add(Constants.StandardsGroupItemProjectSharedParametersSettingsUniqueId,
                Constants.StandardsProjectSharedParameters);
            knownStandards.Add(Constants.StandardsGroupItemFamilyListingUniqueId, Constants.StandardsFamilyListing);


            readonlyStandards.Add(Constants.StandardsGroupItemFamilyListingUniqueId);
            readonlyStandards.Add(Constants.StandardsGroupItemProjectSharedParametersSettingsUniqueId);
            readonlyStandards.Add(Constants.StandardsGroupItemProjectParametersSettingsUniqueId);
        }

        /// <summary>
        ///     Handle the Click event of the bind buttons
        /// </summary>
        /// <param name="arg"></param>
        private void OnSubmit(object arg)
        {
            switch (arg.ToString())
            {
                case "OK":
                    if (ExportModeBid)
                        Export("Schedules");
                    else
                        Export("Basic");
                    break;
                case "Cancel":
                    CloseAction();
                    break;
                case "ExportSchedulesCheck":
                    if (CheckExportSchedule)
                        CheckAllItems(SchedulesList, true);
                    else
                        CheckAllItems(SchedulesList, false);
                    break;
                case "ExportStandardCheck":
                    if (CheckExportStandard)
                        CheckAllItems(StandardsList, true);
                    else
                        CheckAllItems(StandardsList, false);
                    break;
                case "BrowseExportFolder":
                    var folderBrowserDialog = new VistaFolderBrowserDialog();
                    folderBrowserDialog.SelectedPath = ExportFolder;
                    var dialogResult = folderBrowserDialog.ShowDialog();
                    if (dialogResult.HasValue && dialogResult.Value) ExportFolder = folderBrowserDialog.SelectedPath;

                    break;
            }
        }

        private void Export(string sTypeExport)
        {
            if (string.IsNullOrEmpty(sTypeExport)) sTypeExport = "Schedules";
            var schedules = SchedulesList.Where(sl => sl.IsChecked).ToList();
            var standards = StandardsList.Where(s => s.IsChecked).ToList();

            var sRevitFilename = Path.GetFileNameWithoutExtension(revitDocument.PathName);
            if (string.IsNullOrEmpty(sRevitFilename))
            {
                if (string.IsNullOrEmpty(revitDocument.Title))
                    sRevitFilename = "Default";
                else
                    sRevitFilename = revitDocument.Title;
            }

            if (!schedules.Any() && !standards.Any())
            {
                MessageBox.Show(window, Resources.NothingToExportMessage, Resources.NothingToExportTitle,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (schedules.Any())
            {
                var sFileName = sRevitFilename + "_Schedules.xlsx";
                var exportDlg = new SaveFileDialog
                {
                    Filter = @"Excel Files|*.xlsx;*.xls", CheckFileExists = false, RestoreDirectory = true,
                    InitialDirectory = ExportFolder, FileName = sFileName
                };
                if (exportDlg.ShowDialog() == DialogResult.OK)
                {
                    var filesToOverwrite = new List<FileInfo>();
                    fiExportFile = new FileInfo(exportDlg.FileName);
                    ExportFolder = fiExportFile.DirectoryName;
                    if (fiExportFile.Exists) filesToOverwrite.Add(fiExportFile);
                    if (DeleteExistFile(filesToOverwrite))
                    {
                        sFileExport = sTypeExport;
                        ExecuteExport();
                    }
                }
                else
                {
                    return;
                }
            }

            if (standards.Any())
            {
                var sFileName = sRevitFilename + "_Standards.xlsx";
                var exportDlg = new SaveFileDialog
                {
                    Filter = @"Excel Files|*.xlsx;*.xls", CheckFileExists = false, RestoreDirectory = true,
                    InitialDirectory = ExportFolder, FileName = sFileName
                };
                if (exportDlg.ShowDialog() == DialogResult.OK)
                {
                    var filesToOverwrite = new List<FileInfo>();
                    fiExportFile = new FileInfo(exportDlg.FileName);
                    if (fiExportFile.Exists) filesToOverwrite.Add(fiExportFile);
                    if (DeleteExistFile(filesToOverwrite))
                    {
                        sFileExport = "Standards";
                        ExecuteExport();
                    }
                }
            }
        }

        private void ExecuteExport()
        {
            backgroundWorker = new BackgroundWorker();
            dispatcher = Dispatcher.CurrentDispatcher;

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += Worker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
            progress = new Progress();
            progress.ProcessCanceled += OnProcessCanceled;
            EnableButtons = false;

            backgroundWorker.DoWork += ExportWorker;
            backgroundWorker.RunWorkerAsync();
        }

        private bool DeleteExistFile(List<FileInfo> filesToOverwrite)
        {
            if (filesToOverwrite.Any())
            {
                var message = string.Format(Resources.TheTargetFolderAlreadyContainsFiles, filesToOverwrite.Count)
                    + Environment.NewLine + Environment.NewLine
                    + string.Join(Environment.NewLine, filesToOverwrite.Select(f => f.Name));
                var result = MessageBox.Show(window, message, Resources.OverwriteTargetFiles,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return false;

                foreach (var fileInfo in filesToOverwrite)
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(string.Format(Resources.CouldNotDeleteFile, fileInfo.Name),
                            Resources.UnableToDeleteFile, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
            }

            return true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void OnProcessCanceled(object sender, EventArgs eventArgs)
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Select or deselect all items
        /// </summary>
        /// <param name="list"></param>
        /// <param name="check"></param>
        public void CheckAllItems(ObservableCollection<CheckedListItem<string>> list, bool check)
        {
            foreach (var checkedListItem in list) checkedListItem.IsChecked = check;
        }

        /// <summary>
        ///     Not used - This is part of ICommand implementation can be used to control
        ///     the buttons click behavior (Can fire the click event or not)
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanSubmit(object arg)
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportWorker(object sender, DoWorkEventArgs e)
        {
            dispatcher.Invoke(() =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                var schedules = SchedulesList.Where(sl => sl.IsChecked).ToList();
                var standards = StandardsList.Where(s => s.IsChecked).ToList();

                progress.Start((standards.Count() + schedules.Count()) * 15, "Exporting Revit Schedules");
                if (sFileExport == "Schedules") //Call the function to export schedules
                    ExportSchedules(schedules);
                if (sFileExport == "Standards") //Call the function to export the standards
                    ExportStandards(standards);
                if (sFileExport == "Basic") ExportSchedulesBasic(schedules);
                progress.End();

                if (!cancellationTokenSource.IsCancellationRequested)
                    MessageBox.Show(window, Resources.ExportProcessCompleteMessage, Resources.ProcessCompleteTitle,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            });
            // ***********************
        }

        /// <summary>
        ///     Export Worker Basic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportWorkerBasic(object sender, DoWorkEventArgs e)
        {
            dispatcher.Invoke(() =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                var schedules = SchedulesList.Where(sl => sl.IsChecked).ToList();

                progress.Start(schedules.Count() * 15, "Exporting Revit Schedules");

                //Call the function to export schedules
                ExportSchedulesBasic(schedules);
                progress.End();

                if (!cancellationTokenSource.IsCancellationRequested)
                    MessageBox.Show(window, Resources.ExportProcessCompleteMessage, Resources.ProcessCompleteTitle,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            });
            // ***********************
        }

        /// <summary>
        ///     Export schedules
        /// </summary>
        /// <param name="schedules"></param>
        private void ExportSchedules(List<CheckedListItem<string>> schedules)
        {
            if (schedules.Any())
            {
                ExcelPackage excelPackage = null;
                var createdPackages = new List<ExcelPackage>();
                excelPackage = new ExcelPackage(fiExportFile);
                createdPackages.Add(excelPackage);

                var worksheetNames = new Hashtable();
                var scheduleExporter = new ScheduleExporter(cancellationTokenSource.Token);
                foreach (var schedule in schedules)
                {
                    if (cancellationTokenSource.IsCancellationRequested) break;

                    var viewSchedule = schedule.Object as ViewSchedule;
                    if (viewSchedule == null) continue;

                    //There is a 31 characters limit for a worksheet in excel. We need to trim the end and try to prevent collisons.
                    var name = schedule.Item;

                    //These characters are not allowed in excel worksheet
                    name = Regex.Replace(name, ":|\\?|/|\\\\|\\[|\\]|\\*", " ");

                    name = name.Length > 31 ? name.Substring(0, 28) + "001" : name;

                    var suffixNumber = 2;
                    while (worksheetNames[name] != null)
                    {
                        var suffix = suffixNumber++.ToString().PadLeft(3, '0');
                        name = name.Substring(0, Math.Min(name.Length, 28)) + suffix;
                    }

                    worksheetNames[name] = true;
                    var workSheet = excelPackage.Workbook.Worksheets.Add(name);
                    progress.Increment(5);
                    progress.SetStatus(string.Format(Resources.ExportProgressExporting, viewSchedule.Name));
                    scheduleExporter.ExportViewSchedule(revitDocument, viewSchedule, workSheet, parametersSettings);
                    if (ExportOption == ExportOptions.SeparateFiles
                    ) //Addition of the legend tab for each excel file created
                    {
                        var WorkSheetLegendColor = excelPackage.Workbook.Worksheets.Add(Resources.clLegend);
                        ColorLegend.Add(WorkSheetLegendColor);
                    }

                    progress.Increment(5);
                }

                if (ExportOption == ExportOptions.SeparateTables
                ) //Addition of the legend tab when all the tables are in the same excel file
                {
                    var WorkSheetLegendColor = excelPackage.Workbook.Worksheets.Add(Resources.clLegend);
                    ColorLegend.Add(WorkSheetLegendColor);
                }

                foreach (var createdPackage in createdPackages)
                {
                    createdPackage.Save();
                    createdPackage.Dispose();
                }
            }
        }

        /// <summary>
        ///     Export schedules in basic mode
        /// </summary>
        /// <param name="schedules"></param>
        private void ExportSchedulesBasic(List<CheckedListItem<string>> schedules)
        {
            if (schedules.Any())
            {
                ExcelPackage excelPackage = null;
                var createdPackages = new List<ExcelPackage>();
                excelPackage = new ExcelPackage(fiExportFile);
                createdPackages.Add(excelPackage);

                var worksheetNames = new Hashtable();
                var scheduleExporter = new ScheduleExporter(cancellationTokenSource.Token);
                foreach (var schedule in schedules)
                {
                    if (cancellationTokenSource.IsCancellationRequested) break;
                    var viewSchedule = schedule.Object as ViewSchedule;
                    if (viewSchedule == null) continue;
                    //There is a 31 characters limit for a worksheet in excel. We need to trim the end and try to prevent collisons.
                    var name = schedule.Item;

                    //These characters are not allowed in excel worksheet
                    name = Regex.Replace(name, ":|\\?|/|\\\\|\\[|\\]|\\*", " ");

                    name = name.Length > 31 ? name.Substring(0, 28) + "001" : name;

                    var suffixNumber = 2;
                    while (worksheetNames[name] != null)
                    {
                        var suffix = suffixNumber++.ToString().PadLeft(3, '0');
                        name = name.Substring(0, Math.Min(name.Length, 28)) + suffix;
                    }

                    worksheetNames[name] = true;
                    var workSheet = excelPackage.Workbook.Worksheets.Add(name);
                    progress.Increment(5);
                    progress.SetStatus(string.Format(Resources.ExportProgressExporting, viewSchedule.Name));
                    scheduleExporter.ExportViewScheduleBasic(viewSchedule, workSheet);
                    progress.Increment(5);
                }

                foreach (var createdPackage in createdPackages)
                {
                    createdPackage.Save();
                    createdPackage.Dispose();
                }
            }
        }

        /// <summary>
        ///     Export predefined tables
        /// </summary>
        /// <param name="standardsGroups"></param>
        private void ExportStandards(List<CheckedListItem<string>> standardsGroups)
        {
            var standardsExporter = new StandardsExporter(cancellationTokenSource.Token);
            if (standardsGroups.Any())
            {
                var excelPackage = new ExcelPackage(fiExportFile);
                foreach (var standardGroup in standardsGroups)
                {
                    if (cancellationTokenSource.IsCancellationRequested) return;
                    progress.Increment(5);
                    var workbook = excelPackage.Workbook;
                    progress.SetStatus(string.Format(Resources.Exporting, standardGroup.Item));
                    //Appel la fonction pour l'exportation des fichiers standard / Call the function for exporting standard files
                    standardsExporter.ExportStandard(standardGroup.Id, revitDocument, workbook, parametersSettings);
                    progress.Increment(10);
                }

                excelPackage.Save();
            }
        }

        /// <summary>
        ///     Run worker completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableButtons = true;
            EnableButtonsBasic = true;
        }

        /// <summary>
        ///     Property changed
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}