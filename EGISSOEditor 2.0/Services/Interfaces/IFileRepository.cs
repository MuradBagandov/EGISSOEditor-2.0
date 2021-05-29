using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IFileRepository<T> where T: class
    {
        ObservableCollection<T> Items { get; }

        bool Add(string path);

        bool Remove(T element);

        void RemoveAll();

        void Save(T element);

        void SaveAs(T element, string newDirectory);

        void SaveAll();


    }
}
