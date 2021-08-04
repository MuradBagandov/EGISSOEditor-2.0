using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Services;
using EGISSOEditor_2._0.Infrastuctures.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using System.ComponentModel;
using EGISSOEditor_2._0.Infrastuctures.Enums;

namespace EGISSOEditor_2._0.ViewModels
{
    internal class MainWindowViewModel : Base.ViewModel
    {
        private IFileRepository<EGISSOFile> _fileRepository;
        private IRepositoryProcedureDialog<EGISSOFile> _repositoryProcedureDialog;
        private IEGISSOFileEditor<EGISSOFile> _EGISSOEditor;
        private IUserDialog _userDialog;
        private CollectionViewSource _collectionFiles = new CollectionViewSource();

        #region Properties

        #region Files : ICollectionView
        public ICollectionView Files => _collectionFiles?.View;

        #endregion

        #region SelectedFiles : ObservableCollection<object>
        private ObservableCollection<object> _selectedFiles = new ObservableCollection<object>();

        public ObservableCollection<object> SelectedFiles
        {
            get => _selectedFiles;
            set => Set(ref _selectedFiles, value);
        }
        #endregion

        #region CurrentGroupingTypes : GroupingTypes

        private GroupingType _currentGroupingTypes = ApplicationSettings.CurrentGroupingType;

        public GroupingType CurrentGroupingTypes
        {
            get => _currentGroupingTypes;
            set 
            {
                if (Set(ref _currentGroupingTypes, value))
                {
                    UpdateGroupingDescriptionParametrs();
                    ApplicationSettings.CurrentGroupingType = value;
                }
            }
        }
        #endregion

        #region CurrentSortingTypes : GroupingTypes

        private SortingType _currentSortingTypes = ApplicationSettings.CurrentSortingType;

        public SortingType CurrentSortingTypes
        {
            get => _currentSortingTypes;
            set
            {
                if(Set(ref _currentSortingTypes, value))
                {
                    UpdateSortingDescriptionParametrs();
                    ApplicationSettings.CurrentSortingType = value;
                }
            }
        }
        #endregion

        #region SortDescending : bool

        private bool _sortDescending = ApplicationSettings.SortDescending;

        public bool SortDescending
        {
            get => _sortDescending;
            set
            {
                if (Set(ref _sortDescending, value))
                {
                    UpdateSortingDescriptionParametrs();
                    ApplicationSettings.SortDescending = value;
                }
            }
        }
        #endregion

        #region ListViewStyle : bool

        private bool _listViewStyle = ApplicationSettings.ListViewStyle;

        public bool ListViewStyle
        {
            get => _listViewStyle;
            set
            {
                if (Set(ref _listViewStyle, value))
                {
                    ApplicationSettings.ListViewStyle = value;
                }
            }
        }
        #endregion

        #endregion

        #region Commands

        #region AddFileCommand
        public ICommand AddFileCommand { get; set; }

        private async void OnAddFileCommandExecuted(object p)
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Excel xlsx;|*.xlsx; *.xls; *.xlsm; *.xlsb;"
            };

            if (openDialog.ShowDialog() == true)
            {
                await _repositoryProcedureDialog.AddWithShowProgressAsync(openDialog.FileNames);
                _collectionFiles.View.Refresh();
            }
                
        }

        #endregion

        #region RemoveFileCommand

        public ICommand RemoveFileCommand { get; set; }

        private bool CanRemoveFileCommandExecute(object p) => SelectedFiles?.Count > 0;
        
        private async void OnRemoveFileCommandExecuted(object p)
        {
            IEnumerable<EGISSOFile> itemsRemove = SelectedFiles.Select(i => (EGISSOFile)i);
            if (itemsRemove.Count() > 10)
                await _repositoryProcedureDialog.RemoveWithShowProgressAsync(itemsRemove);
            else
                await _repositoryProcedureDialog.RemoveAsync(itemsRemove, null, default);
        }


        #endregion

        #region RemoveFilesCommand

        public ICommand RemoveFilesCommand { get; set; }

        private bool CanRemoveFilesCommandExecute(object p) => _fileRepository?.Items?.Count > 0;

        private async void OnRemoveFilesCommandExecuted(object p)=>
            await _repositoryProcedureDialog.RemoveWithShowProgressAsync(_fileRepository.Items);

        #endregion

        #region SaveFileCommand

        public ICommand SaveFileCommand { get; set; }

        private bool CanSaveFileCommandExecute(object p) => SelectedFiles?.Count > 0;

        private async void OnSaveFileCommandExecuted(object p)
        {
            IEnumerable<EGISSOFile> itemsSave = SelectedFiles.Select(i => (EGISSOFile)i);
            if (itemsSave.Count() > 10)
                await _repositoryProcedureDialog.SaveWithShowProgressAsync(itemsSave);
            else
                await _repositoryProcedureDialog.SaveAsync(itemsSave, null, default);
            _collectionFiles.View.Refresh();

        }


        #endregion

        #region SaveAsFileCommand

        public ICommand SaveAsFileCommand { get; set; }

        private bool CanSaveAsFileCommandExecute(object p) => SelectedFiles?.Count == 1;

        private void OnSaveAsFileCommandExecuted(object p)
        {
            var selectFile = (EGISSOFile)SelectedFiles.First();
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Excel xlsx|*.xlsx;",
                FileName = selectFile.Name
            };

            if (saveDialog.ShowDialog() == true)
            {
                _repositoryProcedureDialog.SaveAs(selectFile, saveDialog.FileName);
                _collectionFiles.View.Refresh();
            }
        }

        #endregion

        #region SaveAllFileCommand

        public ICommand SaveAllFileCommand { get; set; }

        private bool CanSaveAllFileCommandExecute(object p) => _fileRepository?.Items?.Count > 0;

        private async void OnSaveAllFileCommandExecuted(object p) 
        {
            await _repositoryProcedureDialog.SaveWithShowProgressAsync(_fileRepository.Items);
            _collectionFiles.View.Refresh();
        }

        #endregion

        #region FilesStyleCorrectionCommand

        public ICommand FilesStyleCorrectionCommand { get; set; }

        private bool CanFilesStyleCorrectionCommandExecute(object p) => SelectedFiles.Count > 0;

        private async void OnFilesStyleCorrectionCommandExecuted(object p)
        {
            var(progress, cancel, close) = _userDialog.ShowProgress();
            try
            {
                await _EGISSOEditor.FilesStyleCorrectionAsync(SelectedFiles.Select(i => (EGISSOFile)i), progress, cancel);
            }
            #pragma warning disable CS0168
            catch (OperationCanceledException e)
            #pragma warning restore CS0168
            {
            }
            catch (Exception e)
            {
                _userDialog.ShowMessage(e.Message, "EGISSOEditor", Services.Enums.ShowMessageIcon.Error);
            }
            finally
            {
                close?.Invoke();
                _collectionFiles.View.Refresh();

            }
        }


        #endregion

        #region MergingFilesCommand

        public ICommand MergingFilesCommand { get; set; }

        private bool CanMergingFilesCommandExecute(object p) => SelectedFiles.Count > 0;

        private async void OnMergingFilesCommandExecuted(object p)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Excel xlsx;|*.xlsx;"
            };

            if (dialog.ShowDialog() == true)
            {
                var (progress, cancel, close) = _userDialog.ShowProgress();
                try
                {
                    if (_fileRepository.Exist(dialog.FileName))
                        throw new Exception($"Файл {dialog.FileName} уже добавлен!");

                    await _EGISSOEditor.MergingFilesAsync(SelectedFiles.Select(i => (EGISSOFile)i), dialog.FileName, progress, cancel);
                    await _repositoryProcedureDialog.AddWithShowProgressAsync(new string[] { dialog.FileName });
                }
                catch (OperationCanceledException e) { }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Ошибка при объединение файлов", Services.Enums.ShowMessageIcon.Error);
                }
                finally
                {
                    close?.Invoke();
                    _collectionFiles.View.Refresh();

                }
            }
        }
        #endregion

        #region ValidateFilesCommand
        public ICommand ValidateFilesCommand { get; set; }

        private bool CanValidateFilesCommandExecute(object p) => SelectedFiles.Count > 0;

        private async void OnValidateFilesCommandExecuted(object p)
        {
            var (progress, cancel, close) = _userDialog.ShowProgress();
            try
            {
                await _EGISSOEditor.ValidateFilesAsync(SelectedFiles.Select(i => (EGISSOFile)i), progress, cancel);
            }
            #pragma warning disable CS0168 
            catch (OperationCanceledException e)
            #pragma warning restore CS0168
            {
            }
            catch (Exception e)
            {
                _userDialog.ShowMessage(e.Message, "EGISSOEditor", Services.Enums.ShowMessageIcon.Error);
            }
            finally
            {
                close?.Invoke();
                _collectionFiles.View.Refresh();

            }
        }

        #endregion

        #endregion

        public MainWindowViewModel(IFileRepository<EGISSOFile> fileRepository, 
            IRepositoryProcedureDialog<EGISSOFile> repositoryProcedureDialog, 
            IEGISSOFileEditor<EGISSOFile> EGISSOEditor, IUserDialog userDialog)
        {
            AddFileCommand = new LambdaCommand(OnAddFileCommandExecuted);
            RemoveFileCommand = new LambdaCommand(OnRemoveFileCommandExecuted, CanRemoveFileCommandExecute);
            RemoveFilesCommand = new LambdaCommand(OnRemoveFilesCommandExecuted, CanRemoveFilesCommandExecute);
            SaveFileCommand = new LambdaCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);
            SaveAsFileCommand = new LambdaCommand(OnSaveAsFileCommandExecuted, CanSaveAsFileCommandExecute);
            SaveAllFileCommand = new LambdaCommand(OnSaveAllFileCommandExecuted, CanSaveAllFileCommandExecute);
            FilesStyleCorrectionCommand = new LambdaCommand(OnFilesStyleCorrectionCommandExecuted, CanFilesStyleCorrectionCommandExecute);
            MergingFilesCommand = new LambdaCommand(OnMergingFilesCommandExecuted, CanMergingFilesCommandExecute);
            ValidateFilesCommand = new LambdaCommand(OnValidateFilesCommandExecuted, CanValidateFilesCommandExecute);

            _fileRepository = fileRepository;
            _repositoryProcedureDialog = repositoryProcedureDialog;
            _repositoryProcedureDialog.Repository = _fileRepository;
            _EGISSOEditor = EGISSOEditor;
            _userDialog = userDialog;

            UpdateGroupingDescriptionParametrs();
            UpdateSortingDescriptionParametrs();
            _collectionFiles.Source = _fileRepository.Items ?? null;
        }

        private void UpdateGroupingDescriptionParametrs()
        {
            _collectionFiles?.GroupDescriptions.Clear();
            if (CurrentGroupingTypes == GroupingType.Location)
                _collectionFiles?.GroupDescriptions.Add(new PropertyGroupDescription("Location"));
            if (CurrentGroupingTypes == GroupingType.Status)
                _collectionFiles?.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
        }

        private void UpdateSortingDescriptionParametrs()
        {
            _collectionFiles?.SortDescriptions.Clear();
            ListSortDirection sortDirection = SortDescending == true ? ListSortDirection.Descending : ListSortDirection.Ascending;
            if (CurrentSortingTypes == SortingType.ByName)
                _collectionFiles?.SortDescriptions.Add(new SortDescription(nameof(EGISSOFile.Name), sortDirection));
            if (CurrentSortingTypes == SortingType.ByStatus)
                _collectionFiles?.SortDescriptions.Add(new SortDescription(nameof(EGISSOFile.Status), sortDirection));
        }
    }
}
