using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Views.Windows;
using EGISSOEditor_2._0.Services.Enums;

namespace EGISSOEditor_2._0.Services
{
    internal class EGISSORepositoryProcedureDialog : IRepositoryProcedureDialog<EGISSOFile>
    {
        public IFileRepository<EGISSOFile> Repository { get; set; }

        public void Add(string[] files)
        {
            if (Repository == null)
                throw new ArgumentNullException(nameof(Repository));

            foreach (string file in files)
            {
                try
                {
                    Repository.Add(file);
                }
                catch (Exception e)
                {
                    ShowMessage(e.Message, "Добавление файла", ShowMessageIcon.Infomation, ShowMessageButtons.Ok);

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
                    var dialogResult = ShowMessage($"Файл {item.Name} был изменен! Сохранить изменения?", "Удаление", ShowMessageIcon.Infomation, ShowMessageButtons.YesNoCancel);

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
                    ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Infomation, ShowMessageButtons.Ok);
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
                ShowMessage(e.Message, "Cохранение", ShowMessageIcon.Infomation, ShowMessageButtons.Ok);
            }
        }

        private DialogResult ShowMessage(string text, string title, ShowMessageIcon icon, ShowMessageButtons buttons)
        {
            var dialog = new DialogProcedureWindow()
            {
                Owner = App.ActiveWindow,
                Title = title,
                MessageIcon = icon,
                MessageButtons = buttons,
                MessageText = text
            };
            return dialog.ShowDialog() == true ? dialog.ShowDialogResult : DialogResult.Cancel;
        }
    }
}
