using System;
using System.Collections.Generic;
using System.Linq;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Views.Windows;
using EGISSOEditor_2._0.Services.Enums;

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
                    if (!_EGISSOEditor.ValidateFile(file))
                    {
                        _userDialog.ShowMessage($"Неккорректный файл - {file}!", "Добавление файла", ShowMessageIcon.Error, ShowMessageButtons.Ok);
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
                Repository.Remove(item);
            }
        }

        public void Save(IEnumerable<EGISSOFile> elements)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            foreach (var item in elements.ToArray())
                try
                {
                    Repository.Save(item);
                }
                catch (Exception e)
                {
                    _userDialog.ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Error, ShowMessageButtons.Ok);
                }
        }

        private void Save(EGISSOFile elements)=>
            Save(new List<EGISSOFile>() { elements });

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
