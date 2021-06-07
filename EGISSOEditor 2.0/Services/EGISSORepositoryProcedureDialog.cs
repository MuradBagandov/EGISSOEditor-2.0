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
    class EGISSORepositoryProcedureDialog : Interfaces.IRepositoryProcedureDialog<EGISSOFile>
    {
        public void Add(IFileRepository<EGISSOFile> repository, string[] files)
        {
            foreach (string file in files)
            {
                try
                {
                    repository.Add(file);
                }
                catch (Exception e)
                {
                    var dialog = new DialogProcedureWindow()
                    {
                        Owner = App.ActiveWindow,
                        Title = "Добавление файла",
                        MessageIcon = ShowMessageIcon.Error,
                        MessageButtons = ShowMessageButtons.Ok,
                        MessageText = e.Message
                    };
                    dialog.ShowDialog();
                }
            }
        }

        public void Remove(IFileRepository<EGISSOFile> repository, IEnumerable<EGISSOFile> elements)
        {
            

            foreach (var item in elements.ToArray())
            {
                if (item.IsFileChanged)
                {
                    var dialog = new DialogProcedureWindow()
                    {
                        Owner = App.ActiveWindow,
                        Title = "Удаление",
                        MessageIcon = ShowMessageIcon.Infomation,
                        MessageText = $"Файл {item.Name} был изменен! Сохранить изменения?"
                    };

                    if ((bool)dialog.ShowDialog())
                    {
                        if (dialog.ShowDialogResult == DialogResult.Yes)
                            Save(repository, item);
                        else if (dialog.ShowDialogResult == DialogResult.Cancel)
                            return;
                    }
                    else
                    {
                        return;
                    }
                }   
                repository.Remove(item);
            }
        }

        public void Save(IFileRepository<EGISSOFile> repository, IEnumerable<EGISSOFile> elements)
        {
            foreach (var item in elements.ToArray())
                try
                {
                    repository.Save(item);
                }
                catch (Exception e)
                {
                    var dialog = new DialogProcedureWindow()
                    {
                        Owner = App.ActiveWindow,
                        Title = "Cохранение",
                        MessageIcon = ShowMessageIcon.Infomation,
                        MessageButtons = ShowMessageButtons.Ok,
                        MessageText = e.Message
                    };
                    dialog.ShowDialog();
                }
        }

        private void Save(IFileRepository<EGISSOFile> repository, EGISSOFile elements)=>
            Save(repository, new List<EGISSOFile>() { elements });

        public void SaveAs(IFileRepository<EGISSOFile> repository, EGISSOFile elements, string newDirectory)
        {
            try
            {
                repository.SaveAs(elements, newDirectory);
            }
            catch (Exception e)
            {
                var dialog = new DialogProcedureWindow()
                {
                    Owner = App.ActiveWindow,
                    Title = "Cохранение",
                    MessageIcon = ShowMessageIcon.Infomation,
                    MessageButtons = ShowMessageButtons.Ok,
                    MessageText = e.Message
                };
                dialog.ShowDialog();
            }
        }
    }
}
