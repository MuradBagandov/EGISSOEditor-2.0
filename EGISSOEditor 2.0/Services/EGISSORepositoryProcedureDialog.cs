using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Views.Windows;

namespace EGISSOEditor_2._0.Services
{
    class EGISSORepositoryProcedureDialog : Interfaces.IRepositoryProcedureDialog<EGISSOFile>
    {
        public void Add(IFileRepository<EGISSOFile> repository, string[] files)
        {
            throw new NotImplementedException();
        }

        public void Remove(IFileRepository<EGISSOFile> repository, IEnumerable<EGISSOFile> elements)
        {
            DialogProcedureWindow dialog = new DialogProcedureWindow()
            {
                Owner = App.ActiveWindow,
                Title = "Удаление файлов"
            };

            dialog.ShowDialog();
        }

        public void Save(IFileRepository<EGISSOFile> repository, IEnumerable<EGISSOFile> elements)
        {
            throw new NotImplementedException();
        }
    }
}
