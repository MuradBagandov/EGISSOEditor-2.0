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

namespace EGISSOEditor_2._0.ViewModels
{
    internal class MainWindowViewModel : Base.ViewModel
    {
        private IFileRepository<EGISSOFile> _fileRepository;

        #region Properties

        #region Files : ObservableCollection<EGISSOFile>
        public ObservableCollection<EGISSOFile> Files
        {
            get => _fileRepository.Items;
        } 
        #endregion

        #region SelectedFiles : ObservableCollection<object>
        private ObservableCollection<object> _selectedFiles;

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

        private void OnAddFileCommandExecuted(object p)
        {
            OpenFileDialog ofdAddFiles = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Excel xlsx; xls|*.xlsx; *.xls"
            };

            if (ofdAddFiles.ShowDialog() == true)
            {
                foreach (string file in ofdAddFiles.FileNames)
                {
                    try
                    {
                        _fileRepository.Add(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "EGISSOEditor", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion

        #region RemoveFileCommand

        public ICommand RemoveFileCommand { get; set; }

        private bool CanRemoveFileCommandExecute(object p) => SelectedFiles?.Count > 0;
        
        private void OnRemoveFileCommandExecuted(object p)
        {
            RemoveFiles(SelectedFiles.Select(i => (EGISSOFile)i));
        }

        #endregion

        #region RemoveFilesCommand

        public ICommand RemoveFilesCommand { get; set; }

        private bool CanRemoveFilesCommandExecute(object p) => Files?.Count > 0;

        private void OnRemoveFilesCommandExecuted(object p)
        {
            RemoveFiles(Files);
        }

        #endregion

        #region SaveFileCommand

        public ICommand SaveFileCommand { get; set; }

        private bool CanSaveFileCommandExecute(object p) => SelectedFiles?.Count > 0;

        private void OnSaveFileCommandExecuted(object p)
        {
            SaveFiles(SelectedFiles.Select(i => (EGISSOFile)i));
        }

        #endregion

        #region SaveAsFileCommand

        public ICommand SaveAsFileCommand { get; set; }

        private bool CanSaveAsFileCommandExecute(object p) => SelectedFiles?.Count == 1;

        private void OnSaveAsFileCommandExecuted(object p)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Excel xlsx; xls|*.xlsx; *.xls"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _fileRepository.SaveAs((EGISSOFile)SelectedFiles.First(), dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "EGISSOEditor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region SaveAllFileCommand

        public ICommand SaveAllFileCommand { get; set; }

        private bool CanSaveAllFileCommandExecute(object p) => Files.Count > 0;

        private void OnSaveAllFileCommandExecuted(object p)
        {
            SaveFiles(Files);
        }


        #endregion

        #endregion

        public MainWindowViewModel(IFileRepository<EGISSOFile> fileRepository)
        {
            AddFileCommand = new LambdaCommand(OnAddFileCommandExecuted);
            RemoveFileCommand = new LambdaCommand(OnRemoveFileCommandExecuted, CanRemoveFileCommandExecute);
            RemoveFilesCommand = new LambdaCommand(OnRemoveFilesCommandExecuted, CanRemoveFilesCommandExecute);
            SaveFileCommand = new LambdaCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);
            SaveAsFileCommand = new LambdaCommand(OnSaveAsFileCommandExecuted, CanSaveAsFileCommandExecute);
            SaveAllFileCommand = new LambdaCommand(OnSaveAllFileCommandExecuted, CanSaveAllFileCommandExecute);

            _fileRepository = fileRepository;
        }

        private void RemoveFiles(IEnumerable<EGISSOFile> files)
        {
            foreach (EGISSOFile item in files.ToArray())
            {
                try
                {
                    _fileRepository.Remove(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "EGISSOEditor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveFiles(IEnumerable<EGISSOFile> files)
        {
            foreach (EGISSOFile item in files.ToArray())
            {
                try
                {
                    _fileRepository.Save(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "EGISSOEditor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
