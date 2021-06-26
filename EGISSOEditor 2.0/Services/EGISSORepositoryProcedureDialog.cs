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

        public async Task AddAsync(string[] files, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            var progressValue = new ProcedureElementsProgessReporter(progress, "Добавление файлов", files.Length);

            //List<string>

            foreach (string file in files)
            {
                progressValue.CurrentElementName = file;
                try
                {
                    if (Repository.Exist(file))
                        throw new ArgumentException($"Файл {file} уже добавлен!");
                    var result = await _EGISSOEditor.IsValidateFileAsync(file, null, cancel);
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

                progressValue.ProcessedElements++;
            }
        }

        public async Task AddWithShowProgressAsync(string[] files)
        {
            var (progress, cancel, close) = _userDialog.ShowProgress();
            await AddAsync(files, progress, cancel);
            close();
        }

        public async Task RemoveAsync(IEnumerable<EGISSOFile> elements, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            var progressValue = new ProcedureElementsProgessReporter(progress, "Удаление файлов", elements.Count());

            DialogResult? dialogResult = DialogResult.None;

            foreach (var item in elements.ToArray())
            {
                await Task.Delay(1);
                cancel.ThrowIfCancellationRequested();
                progressValue.CurrentElementName = item.Name;

                if (item.IsFileChanged)
                {
                    if (dialogResult != DialogResult.YesForAll && dialogResult != DialogResult.NoForAll)
                         dialogResult = _userDialog.ShowMessage($"Файл {item.Name} был изменен! Сохранить изменения?", "Удаление", ShowMessageIcon.Infomation, ShowMessageButtons.YesNoCancelForAll);

                    if (dialogResult == DialogResult.Yes || dialogResult == DialogResult.YesForAll)
                        Repository.Save(item);
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

                progressValue.ProcessedElements++;
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

        public async Task SaveAsync(IEnumerable<EGISSOFile> elements, IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            var progressValue = new ProcedureElementsProgessReporter(progress, "Сохранение файлов", elements.Count());

            foreach (var item in elements.ToArray())
            {
                await Task.Delay(1);
                cancel.ThrowIfCancellationRequested();
                progressValue.CurrentElementName = item.Name;
                try
                {
                    Repository.Save(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }

                progressValue.ProcessedElements++;
            }
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
    }
}
