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
    public class ImportViewModel : INotifyPropertyChanged
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
        public ImportViewModel(Window window, Document revitDocument) : this()
        {
            this.revitDocument = revitDocument;
            this.window = window;
            Command = new DelegateCommand<object>(OnSubmit, CanSubmit);
            Title = Resources.Import + " - " + AddinInfo.AddinName;
            EnableButtons = true;
            ButtonText = Resources.TitleBtnImport;
            IconButton = B1Paths.ResourcePath("IconButtonDownload.png");
            ParametersSettings.LoadFromFile(B1Paths.ParametersSettingsPath, out parametersSettings);
            FillLists();
        }

        /// <summary>
        /// </summary>
        public ImportViewModel()
        {
            ImportItemList = new ObservableCollection<CheckedListItem<string>>();
            NotImportItemList = new ObservableCollection<CheckedListItem<string>>();

            OrderedItemsForImport = CollectionViewSource.GetDefaultView(ImportItemList);
            OrderedItemsForImport.SortDescriptions.Add(new SortDescription("Item", ListSortDirection.Ascending));

            OrderedItemsForNotImport = CollectionViewSource.GetDefaultView(NotImportItemList);
            OrderedItemsForNotImport.SortDescriptions.Add(new SortDescription("Item", ListSortDirection.Ascending));
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

                    schedulesInModel.Add(viewSchedule.UniqueId, viewSchedule);
            }

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
                    Import();
                    break;
                case "Cancel":
                    CloseAction();
                    break;
                case "ImportCheck":
                    if (CheckAllImport)
                        CheckAllItems(ImportItemList, true);
                    else
                        CheckAllItems(ImportItemList, false);
                    break;
                case "BrowseImportFolder":
                    SelectImportFolder();
                    break;
            }
        }

        /// <summary>
        ///     Import method
        /// </summary>
        private void Import()
        {
            backgroundWorker = new BackgroundWorker();
            dispatcher = Dispatcher.CurrentDispatcher;

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += Worker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            progress = new Progress();
            progress.ProcessCanceled += OnProcessCanceled;
            EnableButtons = false;

            backgroundWorker.DoWork += ImportWorker;
            backgroundWorker.RunWorkerAsync();
        }

        private void SelectImportFolder()
        {
            var importDlg = new OpenFileDialog
            {
                Filter = @"Excel Files|*.xlsx;*.xls", CheckFileExists = true, RestoreDirectory = true,
                InitialDirectory = LastImportFolder
            };
            var bExcelFileValid = false;
            if (importDlg.ShowDialog(window) == DialogResult.OK)
            {
                ImportFolder = importDlg.FileName;

                var fileInfo = new FileInfo(ImportFolder);
                LastImportFolder = fileInfo.DirectoryName;
                if (fileInfo.IsFileLocked())
                {
                    MessageBox.Show(window, Resources.FileInUseMessage, Resources.FileInUseTitle, MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    ImportFolder = "";
                    return;
                }

                ImportItemList.Clear();
                NotImportItemList.Clear();
                CheckAllImport = false;

                if (excelPackageToImport != null) excelPackageToImport.Dispose();

                excelPackageToImport = new ExcelPackage(fileInfo);

                foreach (var worksheet in excelPackageToImport.Workbook.Worksheets)
                {
                    var itemUniqueId = Convert.ToString(worksheet.Cells[1, 1].Value);
                    if (Constants.LegendUniqueId == itemUniqueId)
                    {
                        bExcelFileValid = true;
                    }
                    else if (schedulesInModel.ContainsKey(itemUniqueId))
                    {
                        bExcelFileValid = true;
                        var viewSchedule = schedulesInModel[itemUniqueId];
                        if (viewSchedule.IsTitleblockRevisionSchedule || viewSchedule.Definition.IsMaterialTakeoff)
                        {
                            NotImportItemList.Add(new CheckedListItem<string>(itemUniqueId,
                                schedulesInModel[itemUniqueId].Name, worksheet));
                            continue;
                        }

                        ImportItemList.Add(new CheckedListItem<string>(itemUniqueId,
                            schedulesInModel[itemUniqueId].Name, worksheet));
                    }
                    else if (knownStandards.ContainsKey(itemUniqueId) && !readonlyStandards.Contains(itemUniqueId))
                    {
                        bExcelFileValid = true;
                        ImportItemList.Add(new CheckedListItem<string>(itemUniqueId, knownStandards[itemUniqueId],
                            worksheet));
                    }
                    else if (readonlyStandards.Contains(itemUniqueId))
                    {
                        NotImportItemList.Add(new CheckedListItem<string>(itemUniqueId, knownStandards[itemUniqueId],
                            worksheet));
                    }
                    else
                    {
                        NotImportItemList.Add(new CheckedListItem<string>(worksheet.Name, worksheet.Name, worksheet));
                    }
                }

                if (ImportItemList.Count == 0)
                {
                    if (bExcelFileValid == false)
                        MessageBox.Show(window, Resources.InvalidExcelFileMessage, Resources.InvalidExcelFile,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show(window, Resources.ImportUnavailableMsg, Resources.ImportUnavailable,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CheckAllItems(ObservableCollection<CheckedListItem<string>> list, bool check)
        {
            foreach (var checkedListItem in list) checkedListItem.IsChecked = check;
        }

        private bool CanSubmit(object arg)
        {
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
        ///     Import worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportWorker(object sender, DoWorkEventArgs e)
        {
            dispatcher.Invoke(() =>
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    if (string.IsNullOrEmpty(ImportFolder))
                    {
                        MessageBox.Show(window, Resources.ImportNoFileSelected, Resources.ImportNoFileSelectedTitle,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!ImportItemList.Any(i => i.IsChecked))
                    {
                        MessageBox.Show(window, Resources.ImportNoSelection, Resources.ImportNoSelectionTitle,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var fileInfo = new FileInfo(ImportFolder);

                    if (fileInfo.IsFileLocked())
                    {
                        MessageBox.Show(window, Resources.FileInUseMessage, Resources.FileInUseTitle,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var selectedWorksheets = ImportItemList.Where(i => i.IsChecked)
                        .Select(item => (ExcelWorksheet) item.Object).ToList();
                    var rowCounts = selectedWorksheets.Sum(ws => ws.Dimension == null ? 0 : ws.Dimension.Rows);
                    var maxProgressValue = selectedWorksheets.Count() * 10 + rowCounts;

                    progress.Start(maxProgressValue, Resources.ProgressImportingExcelData);
                    var scheduleImporter = new ScheduleImporter(cancellationTokenSource.Token);
                    var standardsImporter = new StandardsImporter(cancellationTokenSource.Token);
                    var errors = new List<string>();
                    foreach (var worksheet in selectedWorksheets)
                    {
                        if (cancellationTokenSource.IsCancellationRequested) break;
                        var uniqueId = worksheet.Cells[UniqueIdRow, UniqueIdColumn].Value.ToString();
                        //Remove the schedule guid row.
                        if (schedulesInModel.ContainsKey(uniqueId))
                        {
                            var schedule = schedulesInModel[uniqueId];

                            progress.Increment(5);
                            try
                            {
                                scheduleImporter.ImportViewSchedule(revitDocument, worksheet, schedule, progress,
                                    parametersSettings);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(string.Format(Resources.Schedule2, schedule.Name, ex.Message));
                                Logger.Error(ex.ToString());
                            }

                            progress.Increment(5);
                        }
                        else if (knownStandards.ContainsKey(uniqueId))
                        {
                            progress.Increment(5);
                            try
                            {
                                ImportStandards(uniqueId, standardsImporter, worksheet);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(string.Format(Resources.Standard, knownStandards[uniqueId], ex.Message));
                                Logger.Error(ex.ToString());
                            }

                            progress.Increment(5);
                        }
                    }

                    progress.End();

                    if (errors.Any())
                    {
                        var details = string.Join(Environment.NewLine, errors);
                        MessageBox.Show(window,
                            Resources.TheImportProcessWasCompletedWithErrors + Environment.NewLine + Environment.NewLine + details,
                            Resources.CompletedWithErrors,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                        MessageBox.Show(window, Resources.ImportProcessCompleteMessage, Resources.ProcessCompleteTitle,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception exception)
                {
                    if (progress != null) progress.End();
                    Logger.Error(exception.Message, exception);
                    MessageBox.Show(window, string.Format(Resources.ImportErrorMessage, exception.Message),
                        Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        ///     Import tables export in standard mode
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="standardsImporter"></param>
        /// <param name="worksheet"></param>
        private void ImportStandards(string uniqueId, StandardsImporter standardsImporter, ExcelWorksheet worksheet)
        {
            switch (uniqueId)
            {
                case Constants.StandardsGroupItemLineStylesUniqueId:
                    standardsImporter.ImportLineStyles(revitDocument, worksheet, progress);
                    break;

                case Constants.StandardsGroupItemAnnotationObjectsUniqueId:
                    standardsImporter.ImportAnnotationObjects(revitDocument, worksheet, progress);
                    break;

                case Constants.StandardsGroupItemModelObjectsUniqueId:
                    standardsImporter.ImportModelObjects(revitDocument, worksheet, progress);
                    break;

                case Constants.StandardsGroupItemAnalyticalModelObjectsUniqueId:
                    standardsImporter.ImportAnalyticalModelObjects(revitDocument, worksheet, progress);
                    break;

                case Constants.StandardsGroupItemProjectInformationUniqueId:
                    standardsImporter.ImportProjectInformation(revitDocument, worksheet, progress, parametersSettings);
                    break;

                case Constants.StandardsGroupItemFamilyListingUniqueId:
                case Constants.StandardsGroupItemProjectSharedParametersSettingsUniqueId:
                case Constants.StandardsGroupItemProjectParametersSettingsUniqueId:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("uniqueId",
                        uniqueId + " " + Resources.IsNotKnownStandardGuid);
            }
        }

        /// <summary>
        ///     Run worker completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImportFolder = string.Empty;
            ImportItemList.Clear();
            NotImportItemList.Clear();
            CheckAllImport = false;
            if (excelPackageToImport != null)
            {
                excelPackageToImport.Dispose();
                excelPackageToImport = null;
            }

            EnableButtons = true;
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