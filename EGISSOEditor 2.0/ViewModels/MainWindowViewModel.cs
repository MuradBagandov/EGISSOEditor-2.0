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

namespace EGISSOEditor_2._0.ViewModels
{
    internal class MainWindowViewModel : Base.ViewModel
    {
        private IFileRepository<EGISSOFile> _fileRepository;
        private IRepositoryProcedureDialog<EGISSOFile> _repositoryProcedureDialog;
        private IEGISSOFileEditor<EGISSOFile> _EGISSOEditor;
        private IUserDialog _userDialog;

        #region Properties

        #region Files : ObservableCollection<EGISSOFile>
        public ObservableCollection<EGISSOFile> Files
        {
            get => _fileRepository.Items;
        } 
        #endregion

        #region SelectedFiles : ObservableCollection<object>
        private ObservableCollection<object> _selectedFiles = new ObservableCollection<object>();

        public ObservableCollection<object> SelectedFiles
        {
            get => _selectedFiles;
            set => Set(ref _selectedFiles, value);
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
                Filter = "Excel xlsx;|*.xlsx;"
            };

            if (openDialog.ShowDialog() == true)
                await _repositoryProcedureDialog.AddWithShowProgressAsync(openDialog.FileNames);
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

        private bool CanRemoveFilesCommandExecute(object p) => Files?.Count > 0;

        private async void OnRemoveFilesCommandExecuted(object p)=>
            await _repositoryProcedureDialog.RemoveWithShowProgressAsync(Files);

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
        }
            

        #endregion

        #region SaveAsFileCommand

        public ICommand SaveAsFileCommand { get; set; }

        private bool CanSaveAsFileCommandExecute(object p) => SelectedFiles?.Count == 1;

        private void OnSaveAsFileCommandExecuted(object p)
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Excel xlsx; xls|*.xlsx; *.xls"
            };

            if (saveDialog.ShowDialog() == true)
                _repositoryProcedureDialog.SaveAs((EGISSOFile)SelectedFiles.First(), saveDialog.FileName);
        }

        #endregion

        #region SaveAllFileCommand

        public ICommand SaveAllFileCommand { get; set; }

        private bool CanSaveAllFileCommandExecute(object p) => Files.Count > 0;

        private async void OnSaveAllFileCommandExecuted(object p)=> 
            await _repositoryProcedureDialog.SaveWithShowProgressAsync(Files);

        #endregion


        #region FilesStyleCorrectionCommand

        public ICommand FilesStyleCorrectionCommand { get; set; }

        private bool CanFilesStyleCorrectionExecute(object p) => SelectedFiles.Count > 0;

        private async void OnFilesStyleCorrectionExecuted(object p)
        {
            var(progress, cancel, close) = _userDialog.ShowProgress();
            try
            {
                await _EGISSOEditor.FilesStyleCorrectionAsync(SelectedFiles.Select(i => (EGISSOFile)i), progress, cancel);
            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
                _userDialog.ShowMessage("Executed", "1", Services.Enums.ShowMessageIcon.Infomation, Services.Enums.ShowMessageButtons.Ok);
            }
            finally
            {
                close?.Invoke();
            }
        }


        #endregion

        #endregion

        public MainWindowViewModel(IFileRepository<EGISSOFile> fileRepository, IRepositoryProcedureDialog<EGISSOFile> repositoryProcedureDialog, IEGISSOFileEditor<EGISSOFile> EGISSOEditor, IUserDialog userDialog)
        {
            AddFileCommand = new LambdaCommand(OnAddFileCommandExecuted);
            RemoveFileCommand = new LambdaCommand(OnRemoveFileCommandExecuted, CanRemoveFileCommandExecute);
            RemoveFilesCommand = new LambdaCommand(OnRemoveFilesCommandExecuted, CanRemoveFilesCommandExecute);
            SaveFileCommand = new LambdaCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);
            SaveAsFileCommand = new LambdaCommand(OnSaveAsFileCommandExecuted, CanSaveAsFileCommandExecute);
            SaveAllFileCommand = new LambdaCommand(OnSaveAllFileCommandExecuted, CanSaveAllFileCommandExecute);
            FilesStyleCorrectionCommand = new LambdaCommand(OnFilesStyleCorrectionExecuted, CanFilesStyleCorrectionExecute);

            _fileRepository = fileRepository;
            _repositoryProcedureDialog = repositoryProcedureDialog;
            _repositoryProcedureDialog.Repository = _fileRepository;
            _EGISSOEditor = EGISSOEditor;
            _userDialog = userDialog;
        }
    }
}
