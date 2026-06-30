using Autodesk.Revit.DB;
using ExcelExporterImporter.Annotations;
using ExcelExporterImporter.Common;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace ExcelExporterImporter.ViewModels
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        private const int UniqueIdColumn = 1;
        private const int UniqueIdRow = 1;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, string> knownStandards = [];
        private readonly ParametersSettings parametersSettings;
        private readonly List<string> readonlyStandards = [];
        private readonly Document revitDocument;
        private readonly Dictionary<string, ViewSchedule> schedulesInModel = [];
        private readonly Window window;

        private BackgroundWorker backgroundWorker;
        private CancellationTokenSource cancellationTokenSource;
        private Dispatcher dispatcher;
        private ExcelPackage excelPackageToImport;
        private readonly FileInfo fiExportFile;
        private Progress progress;
        private readonly string sFileExport;

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
            ImportItemList = [];
            NotImportItemList = [];

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
            get;
            set
            {
                if (value == field) return;
                field = value;
                OnPropertyChanged();
            }
        }

        public bool EnableButtons
        {
            get;
            set
            {
                if (value.Equals(field)) return;
                field = value;
                OnPropertyChanged();
            }
        }

        public bool EnableButtonsBasic
        {
            get;
            set
            {
                if (value.Equals(field)) return;
                field = value;
                OnPropertyChanged();
            }
        }

        public int SelectedTab
        {
            get;
            set
            {
                field = value;
                ButtonText = value == 1 ? Resources.TitleBtnImport : Resources.TitleBtnExport;
                IconButton = value == 1 ? Constants.IconImportButton : Constants.IconExportButton;
                OnPropertyChanged();
            }
        }

        public string ButtonText
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string IconButton
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Parameter which contains the path of the last directory used for an import.
        /// </summary>
        public string LastImportFolder
        {
            get => string.IsNullOrEmpty(Settings.Default.LastImportPath)
                    ? string.IsNullOrEmpty(Settings.Default.LastExportPath)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        : Settings.Default.LastExportPath
                    : Settings.Default.LastImportPath;
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
            get => string.IsNullOrEmpty(Settings.Default.LastExportPath)
                    ? string.IsNullOrEmpty(Settings.Default.LastImportPath)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        : Settings.Default.LastImportPath
                    : Settings.Default.LastExportPath;
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
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property that contains the visibility value for the Export button
        /// </summary>
        public string ShowExportButton
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property that contains the visibility value for the Import button
        /// </summary>
        public string ShowImportButton
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool CheckAllImport
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the check box for all selected schedules
        /// </summary>
        public bool CheckExportSchedule
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Property to manage the value of the check box for all selected standard
        /// </summary>
        public bool CheckExportStandard
        {
            get;
            set
            {
                field = value;
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
            get => (!Settings.Default.LastExportModeUni && !Settings.Default.LastExportModeBid) || Settings.Default.LastExportModeBid;
            set
            {
                if (value) ExportModeUni = false;
                Settings.Default.LastExportModeBid = value;
                OnPropertyChanged();
            }
        }

        public bool UseExportPrefix
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool UseExportPrefixBasic
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string ExportPrefix
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string ExportPrefixBasic
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public ExportOptions ExportOption
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = ExportOptions.SeparateTables;

        public ExportOptionsBasic ExportOptionBasic
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = ExportOptionsBasic.SeparateTables;

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
            OpenFileDialog importDlg = new()
            {
                Filter = @"Excel Files|*.xlsx;*.xls",
                CheckFileExists = true,
                RestoreDirectory = true,
                InitialDirectory = LastImportFolder
            };
            var bExcelFileValid = false;
            if (importDlg.ShowDialog(window) == DialogResult.OK)
            {
                ImportFolder = importDlg.FileName;

                FileInfo fileInfo = new(ImportFolder);
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

                    FileInfo fileInfo = new(ImportFolder);

                    if (fileInfo.IsFileLocked())
                    {
                        MessageBox.Show(window, Resources.FileInUseMessage, Resources.FileInUseTitle,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var selectedWorksheets = ImportItemList.Where(i => i.IsChecked)
                        .Select(item => (ExcelWorksheet)item.Object).ToList();
                    var rowCounts = selectedWorksheets.Sum(ws => ws.Dimension == null ? 0 : ws.Dimension.Rows);
                    var maxProgressValue = (selectedWorksheets.Count() * 10) + rowCounts;

                    progress.Start(maxProgressValue, Resources.ProgressImportingExcelData);
                    ScheduleImporter scheduleImporter = new(cancellationTokenSource.Token);
                    StandardsImporter standardsImporter = new(cancellationTokenSource.Token);
                    List<string> errors = new();
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