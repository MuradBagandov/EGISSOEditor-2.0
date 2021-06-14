using System;
using System.Collections.Generic;
using System.Linq;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Views.Windows;
using EGISSOEditor_2._0.Services.Enums;
using System.Threading.Tasks;
using System.Threading;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Services
{
    internal class EGISSORepositoryProcedureDialog : IRepositoryProcedureDialog<EGISSOFile>
    {
        public IFileRepository<EGISSOFile> Repository { get; set; }
        private IUserDialog _userDialog;
        private IEGISSOFileEditor<EGISSOFile> _EGISSOEditor;

        public EGISSORepositoryProcedureDialog(IUserDialog userDialog, IEGISSOFileEditor<EGISSOFile> EGISSOEditor)
        {
            _userDialog = userDialog;
            _EGISSOEditor = EGISSOEditor;
        }

        public void Add(string[] files)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            foreach (string file in files)
            {
                try
                {
                    if (!_EGISSOEditor.IsValidateFile(file))
                    {
                        _userDialog.ShowMessage($"Некорректный файл - {file}!", "Добавление файла", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                        continue;
                    }
                    Repository.Add(file);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Добавление файла", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }
            }
        }

        public async Task AddWithShowProgressAsync(string[] files)
        {
            var (progress, cancel, close) = _userDialog.ShowProgress();
            await AddAsync(files, progress, cancel);
            close();
        }

        public async Task AddAsync(string[] files, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            ProcedureFilesProgess progressValue = new ProcedureFilesProgess("Добавление файлов", default, 0, files.Length, 0f, 0f);
            float partFileProgress = 1f / progressValue.TotalFiles;
            float currentTotalProgress = progressValue.TotalFilesProgress;

            IProgress<float> progressValidate = new Progress<float>((v) =>
            {
                progressValue.CurrentFileProgress = v;
                progressValue.TotalFilesProgress = currentTotalProgress + v * partFileProgress;
                progress?.Report(progressValue);
            });

            foreach (string file in files)
            {
                progressValue.CurrentFileName = file.ToString();
                currentTotalProgress = progressValue.TotalFilesProgress;
                progress?.Report(progressValue);
                try
                {
                    var result = await _EGISSOEditor.IsValidateFileAsync(file, progressValidate, cancel);
                    if (!result)
                    {
                        _userDialog.ShowMessage($"Некорректный файл - {file}!", "Добавление файла", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                        continue;
                    }
                    cancel.ThrowIfCancellationRequested();
                    Repository.Add(file);
                }
                catch (OperationCanceledException e)
                {
                    return;
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Добавление файла", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }

                progressValue.ProcessedFiles += 1;
                progressValue.TotalFilesProgress = (float)progressValue.ProcessedFiles / progressValue.TotalFiles;
                progress?.Report(progressValue);
            }
            progressValue.IsEndOfProcessed = true;
            progress?.Report(progressValue);
        }

        public void Remove(IEnumerable<EGISSOFile> elements)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));


            foreach (var item in elements.ToArray())
            {
                if (item.IsFileChanged)
                {
                    var dialogResult = _userDialog.ShowMessage($"Файл {item.Name} был изменен! Сохранить изменения?", "Удаление", ShowMessageIcon.Infomation, ShowMessageButtons.YesNoCancel);

                    if (dialogResult == DialogResult.Yes)
                        Save(item);
                    else if (dialogResult == DialogResult.Cancel)
                        return;
                }
                try
                {
                    Repository.Remove(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Удаление", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }
            }
        }

        public async Task RemoveWithShowProgressAsync(IEnumerable<EGISSOFile> elements)
        {
            var (progress, cancel, close) = _userDialog.ShowProgress();
            try
            {
                await RemoveAsync(elements, progress, cancel);
            }
            catch (OperationCanceledException) { }
            finally { close(); }
        }

        public async Task RemoveAsync(IEnumerable<EGISSOFile> elements, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            ProcedureFilesProgess progressValue = new ProcedureFilesProgess("Удаление файлов", default, 0, elements.Count(), 0f, 0f);

            foreach (var item in elements.ToArray())
            {
                await Task.Delay(1);
                cancel.ThrowIfCancellationRequested();
                progressValue.CurrentFileName = item.Name;
                progress?.Report(progressValue);

                if (item.IsFileChanged)
                {
                    var dialogResult = _userDialog.ShowMessage($"Файл {item.Name} был изменен! Сохранить изменения?", "Удаление", ShowMessageIcon.Infomation, ShowMessageButtons.YesNoCancel);

                    if (dialogResult == DialogResult.Yes)
                        Save(item);
                    else if (dialogResult == DialogResult.Cancel)
                        return;
                }
                try
                {
                    Repository.Remove(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Удаление", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }

                progressValue.ProcessedFiles += 1;
                progressValue.TotalFilesProgress = (float)progressValue.ProcessedFiles / progressValue.TotalFiles;
                progress?.Report(progressValue);
            }
            progressValue.IsEndOfProcessed = true;
            progress?.Report(progressValue);
        }
        public void Save(IEnumerable<EGISSOFile> elements)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            foreach (var item in elements.ToArray())
            {
                try
                {
                    Repository.Save(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }  
            }
        }

        public async Task SaveAsync(IEnumerable<EGISSOFile> elements, IProgress<ProcedureFilesProgess> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            ProcedureFilesProgess progressValue = new ProcedureFilesProgess("Сохранение файлов", default, 0, elements.Count(), 0f, 0f);
            foreach (var item in elements.ToArray())
            {
                await Task.Delay(1);
                cancel.ThrowIfCancellationRequested();
                progressValue.CurrentFileName = item.Name;
                progress?.Report(progressValue);
                try
                {
                    Repository.Save(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }

                progressValue.ProcessedFiles += 1;
                progressValue.TotalFilesProgress = (float)progressValue.ProcessedFiles / progressValue.TotalFiles;
                progress?.Report(progressValue);
            }
            progressValue.IsEndOfProcessed = true;
            progress?.Report(progressValue);
        }

        public async Task SaveWithShowProgressAsync(IEnumerable<EGISSOFile> elements)
        {
            var (progress, cancel, close) = _userDialog.ShowProgress();
            try
            {
                await SaveAsync(elements, progress, cancel);
            }
            catch (OperationCanceledException){ }
            finally { close(); }
        }

        public void SaveAs(EGISSOFile elements, string newDirectory)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            try
            {
                Repository.SaveAs(elements, newDirectory);
            }
            catch (Exception e)
            {
                _userDialog.ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Error, ShowMessageButtons.Ok);
            }
        }

        private void Save(EGISSOFile elements) => Save(new List<EGISSOFile>() { elements });

    }
}
