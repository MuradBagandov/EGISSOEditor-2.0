using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IFileRepository<T> where T: class
    {
        List<T> Items { get; }

        bool Add(string path);

        bool Remove(T element);

        void RemoveAll();

        void Save(T element);

        void SaveAs(T element, string newDirectory);

        void SaveAll();


    }
}
